// Copyright (c) Microsoft. All rights reserved.

namespace Microsoft.KernelMemory.Chunker.Internals;

/// <summary>
/// Trie structure for fast multi-string matching
/// </summary>
internal sealed class SeparatorTrie
{
    private sealed class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; } = new();
        public string? Separator { get; set; }
    }

    private readonly TrieNode _root = new();

    public int Length => this._root.Children.Count;

    public SeparatorTrie(HashSet<string> separators)
    {
        foreach (var separator in separators)
        {
            this.Insert(separator);
        }
    }

    public string? MatchLongest(string text, int startIndex)
    {
        var node = this._root;
        string? longestMatch = null;

        for (int i = startIndex; i < text.Length; i++)
        {
            if (!node.Children.TryGetValue(text[i], out node))
            {
                break;
            }

            if (node.Separator != null)
            {
                longestMatch = node.Separator; // Keep the longest match
            }
        }

        return longestMatch;
    }

    private void Insert(string separator)
    {
        var node = this._root;
        foreach (char c in separator)
        {
            if (!node.Children.TryGetValue(c, out TrieNode? value))
            {
                value = new TrieNode();
                node.Children[c] = value;
            }

            node = value;
        }

        node.Separator = separator;
    }
}
