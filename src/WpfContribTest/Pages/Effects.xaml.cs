using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Avalon.Windows.Media.Effects;
using Avalon.Windows.Utility;

namespace WpfContribTest.Pages;

/// <summary>
///     Interaction logic for Effects.xaml
/// </summary>
public partial class Effects : Page
{
    private static readonly ReadOnlyCollection<EffectColors> s_effectColors = new List<EffectColors>
    {
        new EffectColors(Colors.Thistle, Colors.Plum, Colors.HotPink),
        new EffectColors(Colors.Plum, Colors.Orange, Colors.Fuchsia),
        new EffectColors(Colors.LightSteelBlue, Colors.Snow, Colors.LightSlateGray),
        new EffectColors(Colors.DimGray, Colors.SandyBrown, Colors.HotPink),
        new EffectColors(Colors.PaleGoldenrod, Colors.Goldenrod, Colors.HotPink),
        new EffectColors(Colors.Silver, Colors.Khaki, Colors.HotPink),
        new EffectColors(Colors.LightBlue, Colors.Khaki, Colors.PaleGreen),
        new EffectColors(Colors.LightGray, Colors.Aquamarine, Colors.LightSteelBlue),
        new EffectColors(Colors.HotPink, Colors.Gold, Colors.LightBlue),
        new EffectColors(Colors.PaleGreen, Colors.Gold, Colors.Aquamarine),
        new EffectColors(Colors.CadetBlue, Colors.LightSkyBlue, Colors.LemonChiffon),
        new EffectColors(Colors.Brown, Colors.Khaki, Colors.Violet),
        new EffectColors(Colors.Silver, Colors.Khaki, Colors.HotPink),
        new EffectColors(Colors.PaleVioletRed, Colors.Khaki, Colors.HotPink),
        new EffectColors(Colors.PaleTurquoise, Colors.PaleGoldenrod, Colors.Plum),
        new EffectColors(Colors.Aquamarine, Colors.Gold, Colors.CornflowerBlue),
        new EffectColors(Colors.CadetBlue, Colors.Khaki, Colors.LemonChiffon),
        new EffectColors(Colors.Thistle, Colors.Khaki, Colors.HotPink),
    }.AsReadOnly();

    private static readonly ReadOnlyCollection<ReadOnlyCollection<Point>> s_points =
        new List<ReadOnlyCollection<Point>>
        {
            new List<Point>
            {
                new Point(0.0, 0.0),
                new Point(0.0, 0.5),
                new Point(0.5, 0.0),
                new Point(1.0, 0.0),
            }.AsReadOnly(),
            new List<Point>
            {
                new Point(0.9, 0.0),
                new Point(0.9, 0.7),
                new Point(0.5, 1.0),
            }.AsReadOnly(),
            new List<Point>
            {
                new Point(0.5, 0.0),
                new Point(0.9, 0.0),
                new Point(0.8, 0.3),
                new Point(0.9, 0.8),
            }.AsReadOnly(),
            new List<Point>
            {
                new Point(0.2, 0.9),
                new Point(0.4, 0.8),
                new Point(0.6, 0.9),
            }.AsReadOnly(),
            new List<Point>
            {
                new Point(0.9, 0.2),
                new Point(0.1, 0.0),
            }.AsReadOnly(),
            new List<Point>
            {
                new Point(0.0, 0.5),
                new Point(0.2, 0.8),
                new Point(0.5, 0.9),
            }.AsReadOnly(),
        }.AsReadOnly();

    private readonly Random _random = new();
    private readonly IList<string> _imagePaths;

    private DispatcherTimer _imageTimer;
    private DispatcherTimer _colorTimer;
    private Storyboard _effectStoryboard;
    public Effects()
    {
        InitializeComponent();

        _imagePaths = ImageHelper.GetPaths();

        Loaded += OnLoaded;
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        if (_imagePaths.Count > 0)
        {
            ChangeImage();

            _imageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _imageTimer.Tick += delegate
            { ChangeImage(); };
            _imageTimer.Start();

            ChangeColors();

            _colorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _colorTimer.Tick += delegate
            { ChangeColors(); };
            _colorTimer.Start();
        }
    }

    private void ChangeColors()
    {
        // randomly select light colors
        EffectColors colors = s_effectColors[_random.Next(0, s_effectColors.Count)];

        Storyboard lightStoryboard = new();
        lightStoryboard.AddAnimation(new ColorAnimation(colors.Fill, Duration.Automatic), "lightEffect",
            PointLightEffect.FillColorProperty);
        lightStoryboard.AddAnimation(new ColorAnimation(colors.Light1, Duration.Automatic), "lightEffect",
            PointLightEffect.Light1ColorProperty);
        lightStoryboard.AddAnimation(new ColorAnimation(colors.Light2, Duration.Automatic), "lightEffect",
            PointLightEffect.Light2ColorProperty);
        lightStoryboard.Begin(this);
    }

    private void ChangeImage()
    {
        // randomly select image
        image.Source = new BitmapImage(new Uri(_imagePaths[_random.Next(0, _imagePaths.Count)]));

        PrepareStoryboard();
        AddSceneAnimations();
        AddLightAnimations();
        _effectStoryboard.Begin(this, true);
    }

    private void PrepareStoryboard()
    {
        if (_effectStoryboard == null)
        {
            _effectStoryboard = new Storyboard();
        }
        else
        {
            _effectStoryboard.Remove(this);
            _effectStoryboard.Children.Clear();
        }
    }

    private void AddLightAnimations()
    {
        // randomly select light path
        int pointIndex = _random.Next(0, s_points.Count / 2) * 2;

        _effectStoryboard.AddAnimation(CreatePointAnimation(pointIndex), "lightEffect",
            PointLightEffect.Light1PositionProperty);
        _effectStoryboard.AddAnimation(CreatePointAnimation(pointIndex + 1), "lightEffect",
            PointLightEffect.Light2PositionProperty);
    }

    private void AddSceneAnimations()
    {
        Duration duration = new(TimeSpan.FromSeconds(30));

        image.ClearValue(MarginProperty);

        switch (_random.Next(0, 3))
        {
            case 0:
                // slide
                const int amount = 500;
                image.Margin = new Thickness(0, 0, -amount, 0);
                _effectStoryboard.AddAnimation(
                    new DoubleAnimation(-amount, 0, new Duration(TimeSpan.FromSeconds(amount / 2))), "imageTranslate",
                    TranslateTransform.XProperty);
                break;
            case 1:
                // scale in
                _effectStoryboard.AddAnimation(new DoubleAnimation(1.5, 1, duration), "imageScale",
                    ScaleTransform.ScaleXProperty);
                _effectStoryboard.AddAnimation(new DoubleAnimation(1.5, 1, duration), "imageScale",
                    ScaleTransform.ScaleYProperty);
                break;
            case 2:
                // scale out
                _effectStoryboard.AddAnimation(new DoubleAnimation(1, 1.5, duration), "imageScale",
                    ScaleTransform.ScaleXProperty);
                _effectStoryboard.AddAnimation(new DoubleAnimation(1, 1.5, duration), "imageScale",
                    ScaleTransform.ScaleYProperty);
                break;
            default:
                break;
        }
    }

    private static AnimationTimeline CreatePointAnimation(int index)
    {
        PointAnimationUsingKeyFrames anim = new()
        {
            Duration = new Duration(TimeSpan.FromSeconds(30))
        };

        foreach (Point point in s_points[index])
        {
            _ = anim.KeyFrames.Add(new LinearPointKeyFrame(point));
        }

        return anim;
    }
    private class EffectColors
    {
        public EffectColors()
        {
        }

        public EffectColors(Color fill, Color light1, Color light2)
        {
            Fill = fill;
            Light1 = light1;
            Light2 = light2;
        }

        public Color Fill { get; set; }
        public Color Light1 { get; set; }
        public Color Light2 { get; set; }
    }
}