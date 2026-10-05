using System.Text;
using System.Text.RegularExpressions;

namespace GamebookAgents.RAG;

public sealed partial class TextChunker
{
    private readonly int _chunkSizeWords;
    private readonly int _overlapWords;

    public TextChunker(int chunkSizeWords, int overlapWords)
    {
        if (chunkSizeWords < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSizeWords));
        }

        if (overlapWords < 0 || overlapWords >= chunkSizeWords)
        {
            throw new ArgumentOutOfRangeException(
                nameof(overlapWords),
                "Overlap must be non-negative and smaller than the chunk size.");
        }

        _chunkSizeWords = chunkSizeWords;
        _overlapWords = overlapWords;
    }

    internal IReadOnlyList<ChunkDraft> Chunk(SourceTextSegment segment)
    {
        if (string.IsNullOrWhiteSpace(segment.Text))
        {
            return [];
        }

        var paragraphs = ParseParagraphs(segment.Text, segment.RecognizeMarkdownHeadings);
        var chunks = new List<ChunkDraft>();
        var currentWords = new List<string>(_chunkSizeWords);
        string? currentSection = null;
        var newWordsSinceLastChunk = 0;

        foreach (var paragraph in paragraphs)
        {
            if (!string.Equals(currentSection, paragraph.Section, StringComparison.Ordinal)
                && currentWords.Count > 0)
            {
                EmitChunk(chunks, currentWords, segment.PageNumber, currentSection);
                currentWords.Clear();
                newWordsSinceLastChunk = 0;
            }

            currentSection = paragraph.Section;
            var paragraphWords = SplitWords(paragraph.Text);

            if (paragraphWords.Count == 0)
            {
                continue;
            }

            // Prefer keeping a paragraph intact when the current chunk is already
            // substantial. Very long paragraphs still split at word boundaries.
            if (currentWords.Count > 0
                && currentWords.Count + paragraphWords.Count > _chunkSizeWords
                && currentWords.Count >= _chunkSizeWords / 2)
            {
                EmitChunk(chunks, currentWords, segment.PageNumber, currentSection);
                currentWords = TakeOverlap(currentWords);
                newWordsSinceLastChunk = 0;
            }

            foreach (var word in paragraphWords)
            {
                currentWords.Add(word);
                newWordsSinceLastChunk++;

                if (currentWords.Count < _chunkSizeWords)
                {
                    continue;
                }

                EmitChunk(chunks, currentWords, segment.PageNumber, currentSection);
                currentWords = TakeOverlap(currentWords);
                newWordsSinceLastChunk = 0;
            }
        }

        if (newWordsSinceLastChunk > 0)
        {
            EmitChunk(chunks, currentWords, segment.PageNumber, currentSection);
        }

        return chunks;
    }

    private static IReadOnlyList<Paragraph> ParseParagraphs(string text, bool recognizeMarkdownHeadings)
    {
        var normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var paragraphs = new List<Paragraph>();
        var buffer = new StringBuilder();
        string? section = null;

        void Flush()
        {
            var value = WhitespaceRegex().Replace(buffer.ToString(), " ").Trim();
            buffer.Clear();

            if (value.Length > 0)
            {
                paragraphs.Add(new Paragraph(value, section));
            }
        }

        foreach (var line in normalized.Split('\n'))
        {
            var trimmed = line.Trim();

            if (recognizeMarkdownHeadings && MarkdownHeadingRegex().Match(trimmed) is { Success: true } match)
            {
                Flush();
                section = match.Groups[1].Value.Trim();
                continue;
            }

            if (trimmed.Length == 0)
            {
                Flush();
                continue;
            }

            if (buffer.Length > 0)
            {
                buffer.Append(' ');
            }

            buffer.Append(trimmed);
        }

        Flush();
        return paragraphs;
    }

    private static List<string> SplitWords(string value) =>
        WhitespaceRegex().Split(value.Trim()).Where(word => word.Length > 0).ToList();

    private List<string> TakeOverlap(IReadOnlyList<string> words)
    {
        if (_overlapWords == 0)
        {
            return [];
        }

        return words.Skip(Math.Max(0, words.Count - _overlapWords)).ToList();
    }

    private static void EmitChunk(
        ICollection<ChunkDraft> chunks,
        IReadOnlyList<string> words,
        int? pageNumber,
        string? section)
    {
        if (words.Count > 0)
        {
            chunks.Add(new ChunkDraft(string.Join(' ', words), pageNumber, section));
        }
    }

    private sealed record Paragraph(string Text, string? Section);

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"^#{1,6}\s+(.+?)\s*#*$")]
    private static partial Regex MarkdownHeadingRegex();
}
