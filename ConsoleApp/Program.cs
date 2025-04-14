// Copyright (c) Microsoft. All rights reserved.

namespace GetChunks;

internal sealed class Program
{
    // ReSharper disable ArrangeTypeMemberModifiers
    static async Task Main(string[] args)
    {
        var url = "https://raw.githubusercontent.com/MicrosoftDocs/semantic-kernel-docs/refs/heads/main/semantic-kernel/Frameworks/agent/agent-chat.md";

        var chunker = new Microsoft.KernelMemory.Chunker.Chunker();

        var chunks = await chunker.GetChunksAsync(new Uri(url)).ConfigureAwait(false);

        foreach (var chunk in chunks)
        {
            Console.WriteLine(chunk);
            Console.WriteLine("--------------");
        }
    }
}
