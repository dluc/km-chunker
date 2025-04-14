// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.KernelMemory.Chunker;

public sealed class WebScraperException : System.Exception
{
    public WebScraperException(string message) : base(message)
    {
    }
}
