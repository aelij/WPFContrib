using System.IO;
using System.Security;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WpfContribTest;

/// <summary>
///     Helper for locating image files.
///     Adopted from Kevin Moore's Bag-o-Tricks (http://j832.com/BagOTricks/).
/// </summary>
internal static class ImageHelper
{
    public static IList<string> GetPaths()
    {
        IList<string> picturePaths = null;

        for (int i = 0; i < s_defaultPicturePaths.Length; i++)
        {
            picturePaths = GetPaths(s_defaultPicturePaths[i]);
            if (picturePaths.Count > 0)
            {
                break;
            }
        }

        return picturePaths;
    }

    internal static IList<string> GetPaths(string sourceDirectory)
    {
        if (!string.IsNullOrEmpty(sourceDirectory))
        {
            try
            {
                DirectoryInfo di = new(sourceDirectory);
                if (di.Exists)
                {
                    List<string> imagePaths = new();
                    foreach (string imagePath in GetImageFiles(di))
                    {
                        imagePaths.Add(imagePath);
                        if (imagePaths.Count > MaxImageReturnCount)
                        {
                            break;
                        }
                    }

                    return imagePaths;
                }
            }
            catch (IOException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
        }
        return Array.Empty<string>();
    }

    internal static IEnumerable<string> GetImageFiles(DirectoryInfo directory)
    {
        foreach (FileInfo image in DefaultImageSearchPattern.Split(':').SelectMany(s => directory.GetFiles(s)))
        {
            yield return image.FullName;
        }
        foreach (DirectoryInfo subDir in directory.GetDirectories())
        {
            foreach (string subDirImage in GetImageFiles(subDir))
            {
                yield return subDirImage;
            }
        }
    }

    public static IList<BitmapImage> GetBitmapImages(int maxCount)
    {
        IList<string> imagePaths = GetPaths();
        if (maxCount < 0)
        {
            maxCount = imagePaths.Count;
        }

        BitmapImage[] images = new BitmapImage[Math.Min(imagePaths.Count, maxCount)];
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = new BitmapImage(new Uri(imagePaths[i]));
        }
        return images;
    }

    private static readonly string[] s_defaultPicturePaths = GetDefaultPicturePaths();

    private static string[] GetDefaultPicturePaths()
    {
        if (BrowserInteropHelper.IsBrowserHosted)
        {
            return Array.Empty<string>();
        }
        return new[]
        {
            Path.Combine(Environment.GetEnvironmentVariable("PUBLIC"), @"Pictures\Sample Pictures\"),
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Web\Screen")
        };
    }

    private const string DefaultImageSearchPattern = "*.jpg:*.png";
    private const int MaxImageReturnCount = 50;
}
