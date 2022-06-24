﻿using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Avalon.Windows.Converters;

/// <summary>
///     Converts a string of formatted text to a <see cref="Inline" />s.
///     <remarks>
///         Supported markup:
///         <list>
///             <item>[b] - bold</item>
///             <item>[i] - italics</item>
///             <item>[u] - underline</item>
///             <item>[h url] - hyperlink</item>
///             <item>[nl/] - line break</item>
///             <item>[/] - close tag</item>
///             <item>[[ - escape for '[' character</item>
///         </list>
///     </remarks>
/// </summary>
[ValueConversion(typeof(string), typeof(IEnumerable<Inline>))]
public class FormattedTextConverter : ValueConverter
{
    private enum InlineType
    {
        Run,
        LineBreak,
        Hyperlink,
        Bold,
        Italic,
        Underline
    }
    /// <summary>
    ///     Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the GTMT#binding source.</param>
    /// <param name="targetType">The type of the GTMT#binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    ///     A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string source = (string)value;

        if (string.IsNullOrEmpty(source))
        {
            return Binding.DoNothing;
        }

        List<Inline> inlines = new();

        string[] lines = source.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            StringBuilder sb = new();
            Span parentSpan = new();

            for (int i = 0; i < line.Length; ++i)
            {
                var current = line[i];
                var next = (i + 1 < line.Length) ? line[i + 1] : (char?)null;

                if (current == '[' && next != '[')
                {
                    string text = sb.ToString();
                    sb = new StringBuilder();

                    i += (next == '/') ? 2 : 1;
                    current = line[i];

                    while (i < line.Length && current != ']')
                    {
                        _ = sb.Append(current);

                        ++i;
                        if (i < line.Length)
                        {
                            current = line[i];
                        }
                    }

                    if (text.Length > 0)
                    {
                        parentSpan.Inlines.Add(text);
                    }

                    if (next == '/' && parentSpan.Parent != null)
                    {
                        parentSpan = (Span)parentSpan.Parent;
                    }
                    else
                    {
                        string[] tag = sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tag.Length > 0)
                        {
                            InlineType inlineType = GetInlineType(tag[0].TrimEnd('/'));
                            if (inlineType == InlineType.LineBreak)
                            {
                                parentSpan.Inlines.Add(new LineBreak());
                            }
                            else if (inlineType != InlineType.Run)
                            {
                                string tagParam = (tag.Length > 1) ? tag[1] : null;

                                Span newParentSpan = CreateSpan(inlineType, tagParam);
                                parentSpan.Inlines.Add(newParentSpan);
                                parentSpan = newParentSpan;
                            }
                        }
                    }

                    sb = new StringBuilder();
                }
                else
                {
                    if (current == '[' && next == '[')
                    {
                        ++i;
                    }
                    _ = sb.Append(current);
                }
            }

            if (sb.Length > 0)
            {
                parentSpan.Inlines.Add(sb.ToString());
            }

            inlines.Add(parentSpan);
            inlines.Add(new LineBreak());
        }

        return inlines.ToArray();
    }

    private static InlineType GetInlineType(string type) => type switch
    {
        "b" => InlineType.Bold,
        "i" => InlineType.Italic,
        "u" => InlineType.Underline,
        "h" => InlineType.Hyperlink,
        "nl" => InlineType.LineBreak,
        _ => InlineType.Run,
    };

    private static Span CreateSpan(InlineType inlineType, string param)
    {
        switch (inlineType)
        {
            case InlineType.Hyperlink:
                Hyperlink link = new();

                Uri uri;
                if (Uri.TryCreate(param, UriKind.Absolute, out uri))
                {
                    link.NavigateUri = uri;
                }

                return link;
            case InlineType.Bold:
                return new Bold();
            case InlineType.Italic:
                return new Italic();
            case InlineType.Underline:
                return new Underline();
            default:
                return new Span();
        }
    }
}
