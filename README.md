# GamebookAgents

A .NET 10 gamebook-production runner using Microsoft Agent Framework and local models served by LM Studio. Glimmer performs orchestration and generation; Nomic performs embeddings for the local reference-material search.

This public repository contains a runnable early vertical slice of a larger gamebook-production pipeline. It includes book-to-chapter planning, chapter-to-sequence planning and an independent verification stage. Later production prompts and unpublished book content are kept private.

## LM Studio setup

Load these models and start LM Studio's local server:

- generation: `unsloth/muse-glimmer-30b`
- embeddings: `text-embedding-nomic-embed-text-v1.5`

The default endpoint is `http://127.0.0.1:1234/v1/`. Application and RAG settings live in `gamebooksettings.json`. The endpoint and model names can also be overridden with:

- `GAMEBOOK_LM_STUDIO_URL`
- `GAMEBOOK_GENERATION_MODEL`
- `GAMEBOOK_EMBEDDING_MODEL`

No API key is required when LM Studio local-server authentication is disabled.

## Local reference index

Only files listed in `rag.referenceDocuments` in `gamebooksettings.json` are indexed. The repository's existing `builder/source/` directory is the reference-document convention; it currently contains the SRD PDF. PDF, Markdown, and plain-text files are supported.

Index configured references explicitly:

```bash
dotnet run -- rag-index
```

Normal application startup performs the same check. Each file's SHA-256 hash, embedding model, and chunk settings are stored, so an unchanged document is skipped without contacting the embedding endpoint. New or changed documents are extracted, chunked with overlap, embedded with Nomic, and atomically replace their prior chunks.

The persistent index is `.rag/reference-index.json`. It is local generated data and is ignored by Git.

Run a retrieval-only integration check (Glimmer is not used):

```bash
dotnet run -- rag-search "rules for grappling and escaping a grapple"
```

Results print source names, paths, PDF page numbers, chunk numbers, similarity scores, and excerpt text.

Run deterministic tests without LM Studio:

```bash
dotnet run -- rag-self-test
```

## Agent tools and sessions

`SearchReferenceMaterial` is registered on each production-prompt child agent. Agents use it for focused questions about large indexed sources. Known small authoritative files—production prompts, briefs, specifications, validation rules, and plans—continue to use direct file reading.

`RunPrompt` starts a new Glimmer agent and session for one production prompt. Configured large references passed as inputs are represented by a retrieval instruction rather than copied into the model context. The existing one-artifact write guard is shared with the child, and the top-level pipeline also creates a fresh orchestration session for every artifact.

Copy the example configuration before first run:

```bash
cp gamebooksettings.example.json gamebooksettings.json
dotnet run
```
