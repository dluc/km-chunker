// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.KernelMemory.Chunker;

public interface IChunker
{
    Task<IEnumerable<string>> GetChunksAsync(Uri url, CancellationToken ct);
}