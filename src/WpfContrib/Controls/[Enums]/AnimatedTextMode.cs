namespace Avalon.Windows.Controls
{
    /// <summary>
    ///     Specifies the type of the animation in the <see cref="AnimatedTextBlock" /> control.
    /// </summary>
    public enum AnimatedTextMode
    {
        /// <summary>
        ///     No animation.
        /// </summary>
        None,

        /// <summary>
        ///     Reveal the text.
        /// </summary>
        Reveal,

        /// <summary>
        ///     Hide the text.
        /// </summary>
        Hide,

        /// <summary>
        ///     Reveal the text and then hide it.
        /// </summary>
        RevealAndHide,

        /// <summary>
        ///     Highlight a segment of the text and move on.
        /// </summary>
        Spotlight
    }
}