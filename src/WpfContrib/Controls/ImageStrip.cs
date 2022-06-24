namespace Avalon.Windows.Controls;

/// <summary>
///     Represents a control that displays a single image from a strip of images.
/// </summary>
public class ImageStrip : Control
{
    /// <summary>
    ///     Gets or sets the current frame.
    /// </summary>
    /// <value>The frame.</value>
    public int Frame
    {
        get { return (int)GetValue(FrameProperty); }
        set { SetValue(FrameProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Frame" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrameProperty =
        DependencyProperty.Register("Frame", typeof(int), typeof(ImageStrip),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
    /// <summary>
    ///     Gets or sets the size of the frame.
    ///     <remarks>
    ///         If the <see cref="Orientation" /> is set to <see cref="F:Orientation.Horizontal" />, this represents the
    ///         height of the frame; otherwise, the width.
    ///     </remarks>
    /// </summary>
    /// <value>The size of the frame.</value>
    public double FrameSize
    {
        get { return (double)GetValue(FrameSizeProperty); }
        set { SetValue(FrameSizeProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="FrameSize" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrameSizeProperty =
        DependencyProperty.Register("FrameSize", typeof(double), typeof(ImageStrip),
            new FrameworkPropertyMetadata(0D, FrameworkPropertyMetadataOptions.AffectsRender));
    /// <summary>
    ///     Gets or sets the image.
    /// </summary>
    /// <value>The image.</value>
    public ImageSource Image
    {
        get { return (ImageSource)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Image" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageStrip),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    /// <summary>
    ///     Gets or sets the orientation.
    /// </summary>
    /// <value>The orientation.</value>
    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Orientation" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ImageStrip),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender));
    /// <summary>
    ///     Draws the current frame from the image strip.
    /// </summary>
    /// <param name="drawingContext"></param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        if (Image != null)
        {
            Rect rect = new(0, 0, RenderSize.Width, RenderSize.Height);

            ImageBrush brush = new(Image)
            {
                Stretch = Stretch.None,
                Viewbox = (Orientation == Orientation.Vertical)
                    ? new Rect(0, ((Frame + 0.5) * FrameSize / Image.Height) - 0.5, 1, 1)
                    : new Rect(((Frame + 0.5) * FrameSize / Image.Width) - 0.5, 0, 1, 1)
            };

            drawingContext.DrawRectangle(brush, null, rect);
        }
    }
}
