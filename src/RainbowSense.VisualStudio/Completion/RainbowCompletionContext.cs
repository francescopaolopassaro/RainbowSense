using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace RainbowSense.VisualStudio.Completion;

public sealed class RainbowCompletionContext
{
    private RainbowCompletionContext(string filePath, int position)
    {
        FilePath = filePath;
        Position = position;
    }

    public string FilePath { get; }

    public int Position { get; }

    public string Line { get; private set; } = string.Empty;

    public bool IsRazorFile { get; private set; }

    public bool IsDirectiveContext { get; private set; }

    public bool IsUsingContext { get; private set; }

    public bool IsMarkupContext { get; private set; }

    public bool IsTagContext { get; private set; }

    public string TagName { get; private set; } = string.Empty;

    public string AttributeName { get; private set; } = string.Empty;

    public bool IsInsideComponentTag { get; private set; }

    public static async System.Threading.Tasks.Task<RainbowCompletionContext> CreateAsync(Document document, int position)
    {
        SourceText text;

        try
        {
            text = await document.GetTextAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }

        return Create(document, text, position);
    }

    public static RainbowCompletionContext Create(Document document, int position)
    {
        return CreateAsync(document, position).GetAwaiter().GetResult();
    }

    public static RainbowCompletionContext Create(Document document, SourceText text, int position)
    {
        string filePath = document.FilePath ?? string.Empty;

        if (position < 0 || position > text.Length)
            return null;

        RainbowCompletionContext context = new RainbowCompletionContext(filePath, position);

        context.IsRazorFile = IsRazorFilePath(filePath);

        if (!context.IsRazorFile)
            return context;

        TextLine textLine = text.Lines.GetLineFromPosition(position);
        TextSpan lineSpan = TextSpan.FromBounds(textLine.Start, position);
        string line = text.ToString(TextSpan.FromBounds(lineSpan.Start, position));
        string fullLine = text.ToString(lineSpan);

        context.Line = line;
        context.Analyze(line, fullLine);

        return context;
    }

    public static RainbowCompletionContext Create(string filePath, string text, int position)
    {
        if (filePath == null || text == null || position < 0 || position > text.Length)
            return null;

        RainbowCompletionContext context = new RainbowCompletionContext(filePath, position);

        context.IsRazorFile = IsRazorFilePath(filePath);

        if (!context.IsRazorFile)
            return context;

        SourceText sourceText = SourceText.From(text);

        TextLine textLine = sourceText.Lines.GetLineFromPosition(position);
        TextSpan lineSpan = TextSpan.FromBounds(textLine.Start, position);
        string line = sourceText.ToString(TextSpan.FromBounds(lineSpan.Start, position));
        string fullLine = sourceText.ToString(lineSpan);

        context.Line = line;
        context.Analyze(line, text.Substring(0, position));

        return context;
    }

    private void Analyze(string line, string prefix)
    {
        IsUsingContext = IsInsideUsingDirective(line, prefix);
        IsDirectiveContext = !IsUsingContext && IsAtDirectiveStart(line);

        if (IsInsideTagName(line))
        {
            IsTagContext = true;
            IsMarkupContext = true;

            int start = line.LastIndexOf('<') + 1;
            TagName = line.Substring(start);
            IsInsideComponentTag = false;
            return;
        }

        TagName = GetEnclosingTagName(prefix);
        IsInsideComponentTag = !string.IsNullOrEmpty(TagName);

        string attributeName = GetCurrentAttributeName(line);

        if (!string.IsNullOrEmpty(attributeName))
        {
            AttributeName = attributeName;
            IsMarkupContext = false;
            IsTagContext = false;
            return;
        }

        IsMarkupContext = !IsInsideCodeBlock(prefix);
    }

    private static bool IsInsideUsingDirective(string line, string prefix)
    {
        int index = prefix.LastIndexOf("@using", StringComparison.Ordinal);

        if (index < 0)
            return false;

        return prefix.IndexOf('"', index) < 0;
    }

    private static bool IsAtDirectiveStart(string line)
    {
        int index = line.LastIndexOf('@');

        if (index < 0)
            return false;

        for (int i = index + 1; i < line.Length; i++)
        {
            if (!char.IsLetterOrDigit(line[i]) && line[i] != '_')
                return false;
        }

        return true;
    }

    private static bool IsInsideTagName(string line)
    {
        int open = line.LastIndexOf('<');

        if (open < 0)
            return false;

        if (line.LastIndexOf('>') > open)
            return false;

        string name = line.Substring(open + 1);

        return !name.Contains("/")
            && !name.Contains("=")
            && !name.Contains(" ")
            && name.IndexOf('@') < 0;
    }

    private static string GetCurrentAttributeName(string line)
    {
        int open = line.LastIndexOf('<');

        if (open < 0 || line.LastIndexOf('>') > open)
            return null;

        string tag = line.Substring(open + 1, line.Length - open - 1);

        if (tag.Length == 0 || !char.IsUpper(tag[0]))
            return null;

        int space = tag.IndexOf(' ');

        if (space < 0)
            return string.Empty;

        return tag.Substring(space + 1);
    }

    private static string GetEnclosingTagName(string prefix)
    {
        int index = prefix.Length;

        while (true)
        {
            string segment = prefix.Substring(0, index);
            int open = segment.LastIndexOf('<');
            int close = segment.LastIndexOf('>');

            if (open < 0)
                return string.Empty;

            if (close < open)
                close = segment.Length - 1;

            if (close < open)
                return string.Empty;

            string tag = segment.Substring(open + 1, close - open - 1).Trim();

            if (tag.StartsWith("/", StringComparison.Ordinal)
                || tag.StartsWith("!", StringComparison.Ordinal))
            {
                index = open;
                continue;
            }

            if (tag.StartsWith("@", StringComparison.Ordinal))
            {
                index = open;
                continue;
            }

            int space = tag.IndexOf(' ');
            string name = (space < 0 ? tag : tag.Substring(0, space)).TrimEnd('/');

            return name;
        }
    }

    private static bool IsInsideCodeBlock(string prefix)
    {
        int atCode = prefix.LastIndexOf("@code", StringComparison.Ordinal);
        int atFunctions = prefix.LastIndexOf("@functions", StringComparison.Ordinal);

        int start = Math.Max(atCode, atFunctions);

        if (start < 0)
            return false;

        int depth = 0;

        for (int i = prefix.IndexOf('{', start); i >= 0 && i < prefix.Length; i++)
        {
            char ch = prefix[i];

            if (ch == '{')
            {
                depth++;
            }
            else if (ch == '}')
            {
                depth--;

                if (depth == 0)
                    return false;
            }
        }

        return depth > 0;
    }

    private static bool IsRazorFilePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        return filePath.EndsWith(".razor", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase);
    }
}