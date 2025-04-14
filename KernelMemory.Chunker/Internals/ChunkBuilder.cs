// Copyright (c) Microsoft. All rights reserved.

using System.Text;

namespace Microsoft.KernelMemory.Chunker.Internals;

internal sealed class ChunkBuilder
{
    public readonly StringBuilder FullContent = new();
    public readonly StringBuilder NextSentence = new();
}
