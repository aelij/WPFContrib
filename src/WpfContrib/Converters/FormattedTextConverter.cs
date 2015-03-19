using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;

namespace Avalon.Windows.Converters
{
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
    [ValueConversion(typeof (string), typeof (IEnumerable<Inline>))]
    public class FormattedTextConverter : ValueConverter
    {
        #region InlineType Enum

        private enum InlineType
        {
            Run,
            LineBreak,
            Hyperlink,
            Bold,
            Italic,
            Underline
        }

        #endregion

        #region Methods

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
            string source = (string) value;

            if (string.IsNullOrEmpty(source))
            {
                return Binding.DoNothing;
            }

            List<Inline> inlines = new List<Inline>();

            string[] lines = source.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                StringBuilder sb = new StringBuilder();
                Span parentSpan = new Span();

                for (int i = 0; i < line.Length; ++i)
                {
                    var current = line[i];
                    var next = (i + 1 < line.Length) ? line[i + 1] : (char?) null;

                    if (current == '[' && next != '[')
                    {
                        string text = sb.ToString();
                        sb = new StringBuilder();

                        i += (next == '/') ? 2 : 1;
                        current = line[i];

                        while (i < line.Length && current != ']')
                        {
                            sb.Append(current);

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
                            parentSpan = (Span) parentSpan.Parent;
                        }
                        else
                        {
                            string[] tag = sb.ToString().Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
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
                        sb.Append(current);
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

        private static InlineType GetInlineType(string type)
        {
            switch (type)
            {
                case "b":
                    return InlineType.Bold;
                case "i":
                    return InlineType.Italic;
                case "u":
                    return InlineType.Underline;
                case "h":
                    return InlineType.Hyperlink;
                case "nl":
                    return InlineType.LineBreak;
                default:
                    return InlineType.Run;
            }
        }

        private static Span CreateSpan(InlineType inlineType, string param)
        {
            Span span;
            switch (inlineType)
            {
                case InlineType.Hyperlink:
                    Hyperlink link = new Hyperlink();

                    Uri uri;
                    if (Uri.TryCreate(param, UriKind.Absolute, out uri))
                    {
                        link.NavigateUri = uri;
                    }

                    span = link;
                    break;
                case InlineType.Bold:
                    span = new Bold();
                    break;
                case InlineType.Italic:
                    span = new Italic();
                    break;
                case InlineType.Underline:
                    span = new Underline();
                    break;
                default:
                    span = new Span();
                    break;
            }

            return span;
        }

        #endregion
    }
}