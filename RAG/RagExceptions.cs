namespace GamebookAgents.RAG;

public class RagException : Exception
{
    public RagException(string message)
        : base(message)
    {
    }

    public RagException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class EmbeddingServiceException : RagException
{
    public EmbeddingServiceException(string message)
        : base(message)
    {
    }

    public EmbeddingServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class RagStoreException : RagException
{
    public RagStoreException(string message)
        : base(message)
    {
    }

    public RagStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
