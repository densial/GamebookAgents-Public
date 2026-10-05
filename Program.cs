using System.ClientModel;
using System.ClientModel.Primitives;
using GamebookAgents.Configuration;
using GamebookAgents.Functions;
using GamebookAgents.RAG;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

var projectRoot = ProjectPathGuard.ProjectRootPath;
GamebookOptions settings;

try
{
    settings = await GamebookOptions.LoadAsync(projectRoot);
}
catch (Exception exception) when (exception is IOException
                                  or UnauthorizedAccessException
                                  or InvalidDataException)
{
    Console.Error.WriteLine($"Configuration error: {exception.Message}");
    Environment.ExitCode = 1;
    return;
}

if (args.FirstOrDefault()?.Equals("rag-self-test", StringComparison.OrdinalIgnoreCase) == true)
{
    await RagSelfTests.RunAsync();
    return;
}

using var embeddingHttpClient = new HttpClient
{
    BaseAddress = settings.GetLmStudioBaseUri(),
    Timeout = Timeout.InfiniteTimeSpan
};

var embeddingService = new EmbeddingService(
    embeddingHttpClient,
    settings.EmbeddingModel,
    settings.Rag.EmbeddingBatchSize);
var ragStore = new RagStore(settings.Rag.ResolveIndexPath(projectRoot));
var textChunker = new TextChunker(
    settings.Rag.ChunkSizeWords,
    settings.Rag.ChunkOverlapWords);
var documentIndexer = new DocumentIndexer(
    projectRoot,
    embeddingService,
    textChunker,
    ragStore,
    settings.Rag);
var ragSearchService = new RagSearchService(embeddingService, ragStore);
var ragTools = new RagTools(ragSearchService, settings.Rag.DefaultTopK);

try
{
    await documentIndexer.IndexConfiguredDocumentsAsync();
}
catch (Exception exception) when (exception is RagException
                                  or FileNotFoundException
                                  or NotSupportedException
                                  or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Reference indexing failed: {exception.Message}");
    Environment.ExitCode = 1;
    return;
}

var command = args.FirstOrDefault();
if (command?.Equals("rag-index", StringComparison.OrdinalIgnoreCase) == true)
{
    Console.WriteLine($"Reference index is ready: {ragStore.IndexPath}");
    return;
}

if (command?.Equals("rag-search", StringComparison.OrdinalIgnoreCase) == true)
{
    var query = string.Join(' ', args.Skip(1));
    if (string.IsNullOrWhiteSpace(query))
    {
        Console.Error.WriteLine("Usage: dotnet run -- rag-search \"rules for grappling and escaping a grapple\"");
        Environment.ExitCode = 2;
        return;
    }

    try
    {
        var results = await ragSearchService.SearchAsync(query, settings.Rag.DefaultTopK);
        Console.WriteLine(RagTools.FormatResults(results));
    }
    catch (Exception exception) when (exception is RagException or ArgumentException)
    {
        Console.Error.WriteLine($"Reference search failed: {exception.Message}");
        Environment.ExitCode = 1;
    }

    return;
}

using var generationHttpClient = new HttpClient
{
    Timeout = Timeout.InfiniteTimeSpan
};

var clientOptions = new OpenAIClientOptions
{
    Endpoint = settings.GetLmStudioBaseUri(),
    Transport = new HttpClientPipelineTransport(generationHttpClient),
    NetworkTimeout = Timeout.InfiniteTimeSpan
};

var chatClient = new OpenAIClient(
        // The OpenAI SDK requires a credential object, but LM Studio ignores this
        // placeholder when its local server authentication is disabled.
        new ApiKeyCredential("not-required"),
        clientOptions)
    .GetChatClient(settings.GenerationModel);

var artifactWriteTool = new ArtifactWriteTool();
var runPromptTool = new RunPromptTool(
    chatClient,
    artifactWriteTool,
    ragTools,
    projectRoot,
    settings.Rag);

AIAgent agent = chatClient.AsAIAgent(
    name: "GamebookOrchestrator",
    instructions: """
        You are an autonomous orchestration agent responsible for one bounded iteration of a gamebook production pipeline.

        Your goal in each run is to create or update exactly one required output artifact, not to complete the entire pipeline in one session. The filesystem is durable pipeline state. Inspect directory listings and file names to determine the next incomplete artifact. Do not read the contents of completed artifacts, production prompts, or project registries to select the work; RunPrompt starts a fresh session that reads the required contents.

        Use RunPrompt to execute the applicable production prompt in an isolated fresh Glimmer session. Pass the prompt path, required project input paths, and concise context identifying the book root and chapter or artifact. The child session reads required contents, searches reference material, and writes the artifact.

        Operating rules:
        - Inspect the workspace before making assumptions.
        - Treat project prompt, specification, plan, and validation files as authoritative.
        - Do not invent the contents of files you have not read.
        - Determine the next applicable production prompt and its prerequisites from the workspace.
        - Use directory listings and file names to check artifact existence; do not read existing artifact contents unless the selected prompt explicitly requires reviewing them before a rerun.
        - Do not read production prompt or registry contents in this orchestration session. Pass their paths to RunPrompt so the child session reads them.
        - Create or update no more than one output file during this run.
        - Once WriteFile succeeds, including inside RunPrompt, stop using tools and return a concise summary naming the artifact.
        - Do not attempt a second write. The caller starts a fresh session for the next artifact.
        - If no required artifacts remain, return PIPELINE_COMPLETE without writing a file.
        - If blocked, report the precise blocker without writing an unrelated artifact.
        - Use ReportStatus for meaningful progress during long work, not for trivial operations.
        """,
    tools:
    [
        AIFunctionFactory.Create(DirectoryFunctions.ListDirectory, name: "ListDirectory"),
        AIFunctionFactory.Create(runPromptTool.RunPrompt, name: "RunPrompt"),
        AIFunctionFactory.Create(StatusFunctions.ReportStatus, name: "ReportStatus")
    ]);

Console.WriteLine(
    $"GamebookBuilder is starting using {settings.GenerationModel} via {settings.GetLmStudioBaseUri()}.");

const string artifactPrompt = """
    Inspect the workspace and complete the next single required gamebook artifact.

    Determine the next artifact from directory listings and file names only. Do not read existing artifacts, production prompts, or registries in this orchestration session. Call RunPrompt with the applicable prompt path, required input file paths, and concise book/chapter context. Stop after the one artifact is written. If the pipeline is complete, return PIPELINE_COMPLETE. If it is blocked, report the specific blocker.
    """;

const int maximumArtifactRuns = 500;

for (var runNumber = 1; runNumber <= maximumArtifactRuns; runNumber++)
{
    artifactWriteTool.BeginRun();
    var session = await agent.CreateSessionAsync();

    Console.WriteLine($"Starting artifact run {runNumber} with a fresh orchestration session.");

    var response = await agent.RunAsync(artifactPrompt, session);
    Console.WriteLine(response.Text);

    if (artifactWriteTool.WrittenPath is null)
    {
        Console.WriteLine("No artifact was written; stopping the pipeline runner.");
        break;
    }

    Console.WriteLine($"Artifact completed: {artifactWriteTool.WrittenPath}");

    if (runNumber == maximumArtifactRuns)
    {
        Console.WriteLine($"Stopped after reaching the safety limit of {maximumArtifactRuns} artifact runs.");
    }
}
