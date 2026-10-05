using System.Net.Http.Json;
using System.Text.Json;

namespace GamebookAgents.RAG;

public sealed class EmbeddingService
{
    private const string DocumentPrefix = "search_document: ";
    private const string QueryPrefix = "search_query: ";

    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly int _batchSize;
    private readonly object _dimensionGate = new();
    private int? _observedDimension;

    public EmbeddingService(HttpClient httpClient, string model, int batchSize)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _model = string.IsNullOrWhiteSpace(model)
            ? throw new ArgumentException("An embedding model is required.", nameof(model))
            : model;
        _batchSize = batchSize > 0
            ? batchSize
            : throw new ArgumentOutOfRangeException(nameof(batchSize));
    }

    public string Model => _model;

    public async Task<float[]> EmbedDocumentAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await EmbedAsync([PrepareInput(text, DocumentPrefix)], cancellationToken);
        return embeddings[0];
    }

    public async Task<float[]> EmbedQueryAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await EmbedAsync([PrepareInput(text, QueryPrefix)], cancellationToken);
        return embeddings[0];
    }

    public async Task<IReadOnlyList<float[]>> EmbedDocumentsAsync(
        IReadOnlyList<string> texts,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(texts);

        if (texts.Count == 0)
        {
            return [];
        }

        var results = new List<float[]>(texts.Count);

        for (var offset = 0; offset < texts.Count; offset += _batchSize)
        {
            var count = Math.Min(_batchSize, texts.Count - offset);
            var inputs = new string[count];

            for (var index = 0; index < count; index++)
            {
                inputs[index] = PrepareInput(texts[offset + index], DocumentPrefix);
            }

            results.AddRange(await EmbedAsync(inputs, cancellationToken));
        }

        return results;
    }

    private async Task<IReadOnlyList<float[]>> EmbedAsync(
        IReadOnlyList<string> inputs,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "embeddings",
                new
                {
                    model = _model,
                    input = inputs,
                    encoding_format = "float"
                },
                cancellationToken);

            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var details = responseText.Length > 2_000
                    ? responseText[..2_000]
                    : responseText;

                throw new EmbeddingServiceException(
                    $"LM Studio embeddings request failed with HTTP {(int)response.StatusCode} "
                    + $"({response.ReasonPhrase}). Model: {_model}. Response: {details}");
            }

            return ParseResponse(responseText, inputs.Count);
        }
        catch (EmbeddingServiceException)
        {
            throw;
        }
        catch (HttpRequestException exception)
        {
            throw new EmbeddingServiceException(
                $"Could not reach LM Studio for embeddings at {_httpClient.BaseAddress}. "
                + $"Ensure the server is running and model '{_model}' is available. {exception.Message}",
                exception);
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new EmbeddingServiceException(
                $"LM Studio embeddings request timed out for model '{_model}'.",
                exception);
        }
        catch (JsonException exception)
        {
            throw new EmbeddingServiceException(
                $"LM Studio returned a malformed embeddings response for model '{_model}'. {exception.Message}",
                exception);
        }
    }

    private IReadOnlyList<float[]> ParseResponse(string responseText, int expectedCount)
    {
        using var document = JsonDocument.Parse(responseText);

        if (document.RootElement.ValueKind != JsonValueKind.Object
            || !document.RootElement.TryGetProperty("data", out var data)
            || data.ValueKind != JsonValueKind.Array)
        {
            throw new EmbeddingServiceException(
                "LM Studio embeddings response did not contain a data array.");
        }

        var indexedResults = new SortedDictionary<int, float[]>();

        foreach (var item in data.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object
                || !item.TryGetProperty("index", out var indexElement)
                || !indexElement.TryGetInt32(out var index)
                || index < 0
                || index >= expectedCount)
            {
                throw new EmbeddingServiceException(
                    "LM Studio embeddings response contained an invalid embedding index.");
            }

            if (!item.TryGetProperty("embedding", out var embeddingElement)
                || embeddingElement.ValueKind != JsonValueKind.Array)
            {
                throw new EmbeddingServiceException(
                    $"LM Studio embeddings response item {index} did not contain a numeric embedding array.");
            }

            var values = new List<float>();

            foreach (var value in embeddingElement.EnumerateArray())
            {
                if (!value.TryGetSingle(out var number) || !float.IsFinite(number))
                {
                    throw new EmbeddingServiceException(
                        $"LM Studio embeddings response item {index} contained a non-finite or non-numeric value.");
                }

                values.Add(number);
            }

            if (values.Count == 0)
            {
                throw new EmbeddingServiceException(
                    $"LM Studio embeddings response item {index} contained an empty vector.");
            }

            ValidateDimension(values.Count);

            if (!indexedResults.TryAdd(index, values.ToArray()))
            {
                throw new EmbeddingServiceException(
                    $"LM Studio embeddings response contained duplicate index {index}.");
            }
        }

        if (indexedResults.Count != expectedCount
            || indexedResults.Keys.Where((value, index) => value != index).Any())
        {
            throw new EmbeddingServiceException(
                $"LM Studio returned {indexedResults.Count} embeddings for {expectedCount} inputs.");
        }

        return indexedResults.Values.ToArray();
    }

    private void ValidateDimension(int dimension)
    {
        lock (_dimensionGate)
        {
            if (_observedDimension is null)
            {
                _observedDimension = dimension;
                return;
            }

            if (_observedDimension != dimension)
            {
                throw new EmbeddingServiceException(
                    $"Embedding dimension changed from {_observedDimension} to {dimension} for model '{_model}'.");
            }
        }
    }

    private static string PrepareInput(string text, string prefix)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text to embed cannot be empty.", nameof(text));
        }

        return $"{prefix}{text.Trim()}";
    }
}
