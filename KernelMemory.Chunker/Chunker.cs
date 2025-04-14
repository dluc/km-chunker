// Copyright (c) Microsoft. All rights reserved.

using Microsoft.KernelMemory.Chunker.Internals;

namespace Microsoft.KernelMemory.Chunker;

public class Chunker : IChunker
{
    public async Task<IEnumerable<string>> GetChunksAsync(Uri url, CancellationToken ct = default)
    {
        using (var scraper = new WebScraper())
        {
            var text = await scraper.GetAsync(url, ct).ConfigureAwait(false);

            if (url.AbsolutePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                var mdChunker = new MarkDownChunker();
                return mdChunker.Split(text, new MarkDownChunkerOptions
                {
                    MaxTokensPerChunk = 1000,
                    Overlap = 0,
                    ChunkHeader = null
                });
            }

            var txtChunker = new PlainTextChunker();
            return txtChunker.Split(text, new PlainTextChunkerOptions
            {
                MaxTokensPerChunk = 1000,
                Overlap = 0,
                ChunkHeader = null
            });
        }
    }
}