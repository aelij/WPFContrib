using System.Diagnostics;
using System.Drawing;
using System.Reflection;

namespace Avalon.Windows.Utility;

/// <summary>
///     Encapsulates methods for manipulation of the visual tree.
/// </summary>
public static class UIHelpers
{
    static UIHelpers()
    {
        // register the hyperlink click event so we could launch the browser.
        EventManager.RegisterClassHandler(typeof(Hyperlink), Hyperlink.ClickEvent,
            new RoutedEventHandler(OnHyperlinkClick));
    }
    private delegate object InvokeMethodDelegate(object o, object[] parameters);

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns></returns>
    public static bool EnsureAccess(MethodBase method)
    {
        return EnsureAccess((Dispatcher)null, method, null);
    }

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public static bool EnsureAccess(MethodBase method, params object[] parameters)
    {
        return EnsureAccess(null, method, null, parameters);
    }

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="o">The object.</param>
    /// <returns></returns>
    public static bool EnsureAccess(MethodBase method, object o)
    {
        return EnsureAccess((Dispatcher)null, method, o);
    }

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="o">The object.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public static bool EnsureAccess(MethodBase method, object o, params object[] parameters)
    {
        return EnsureAccess(null, method, o, parameters);
    }

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="method">The method.</param>
    /// <param name="o">The object.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public static bool EnsureAccess(Dispatcher dispatcher, MethodBase method, object o, params object[] parameters)
    {
        if (dispatcher == null)
        {
            if (o is DispatcherObject dispatcherObj)
            {
                dispatcher = dispatcherObj.Dispatcher;
            }
            else if (Application.Current != null)
            {
                dispatcher = Application.Current.Dispatcher;
            }
            else
            {
                throw new ArgumentException(SR.UIHelpers_NoDispatcher, nameof(dispatcher));
            }
        }

        bool hasAccess = dispatcher.CheckAccess();

        if (!hasAccess)
        {
            _ = dispatcher.Invoke(DispatcherPriority.Normal,
                new InvokeMethodDelegate(method.Invoke), o, new object[] { parameters });
        }

        return hasAccess;
    }

    /// <summary>
    ///     Ensures the calling thread is the thread associated with the <see cref="Dispatcher" />.
    /// </summary>
    /// <param name="o">The object.</param>
    /// <param name="method">The method.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns></returns>
    public static bool EnsureAccess(this DispatcherObject o, MethodBase method, params object[] parameters)
    {
        return EnsureAccess(o.Dispatcher, method, o, parameters);
    }
    /// <summary>
    ///     Finds the logical ancestor according to the predicate.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public static DependencyObject FindLogicalAncestor(this DependencyObject startElement,
        Predicate<DependencyObject> condition)
    {
        DependencyObject o = startElement;
        while ((o != null) && !condition(o))
        {
            o = LogicalTreeHelper.GetParent(o);
        }
        return o;
    }

    /// <summary>
    ///     Finds the logical ancestor by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startElement">The start element.</param>
    /// <returns></returns>
    public static T FindLogicalAncestorByType<T>(this DependencyObject startElement) where T : DependencyObject
    {
        return (T)FindLogicalAncestor(startElement, o => o is T);
    }

    /// <summary>
    ///     Finds the logical root.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns></returns>
    public static DependencyObject FindLogicalRoot(this DependencyObject startElement)
    {
        DependencyObject o = null;
        while (startElement != null)
        {
            o = startElement;
            startElement = LogicalTreeHelper.GetParent(startElement);
        }
        return o;
    }

    /// <summary>
    ///     Finds the visual ancestor according to the predicate.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public static DependencyObject FindVisualAncestor(this DependencyObject startElement,
        Predicate<DependencyObject> condition)
    {
        DependencyObject o = startElement;
        while ((o != null) && !condition(o))
        {
            o = VisualTreeHelper.GetParent(o);
        }
        return o;
    }

    /// <summary>
    ///     Finds the visual ancestor by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startElement">The start element.</param>
    /// <returns></returns>
    public static T FindVisualAncestorByType<T>(this DependencyObject startElement) where T : DependencyObject
    {
        return (T)FindVisualAncestor(startElement, o => o is T);
    }

    /// <summary>
    ///     Finds the visual descendant.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    public static DependencyObject FindVisualDescendant(this DependencyObject startElement,
        Predicate<DependencyObject> condition)
    {
        if (startElement != null)
        {
            if (condition(startElement))
            {
                return startElement;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(startElement); ++i)
            {
                DependencyObject o = FindVisualDescendant(VisualTreeHelper.GetChild(startElement, i), condition);
                if (o != null)
                {
                    return o;
                }
            }
        }
        return null;
    }

    /// <summary>
    ///     Finds the visual descendant by type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startElement">The start element.</param>
    /// <returns></returns>
    public static T FindVisualDescendantByType<T>(this DependencyObject startElement) where T : DependencyObject
    {
        return (T)FindVisualDescendant(startElement, o => o is T);
    }

    /// <summary>
    ///     Finds the visual root.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns></returns>
    public static DependencyObject FindVisualRoot(this DependencyObject startElement)
    {
        return FindVisualAncestor(startElement, o => VisualTreeHelper.GetParent(o) == null);
    }

    /// <summary>
    ///     Gets the visual children.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <returns></returns>
    public static IEnumerable<Visual> GetVisualChildren(this Visual parent)
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; ++i)
        {
            yield return (Visual)VisualTreeHelper.GetChild(parent, i);
        }
    }
    private static Matrix s_dpiTransformToDevice;

    /// <summary>
    ///     Gets a matrix that transforms the coordinates of this target to the device that is associated with the rendering
    ///     destination.
    /// </summary>
    public static Matrix DpiTransformToDevice
    {
        get
        {
            EnsureDpiData();
            return s_dpiTransformToDevice;
        }
    }

    private static Matrix s_dpiTransformFromDevice;

    /// <summary>
    ///     Gets a matrix that transforms the coordinates of the device that is associated with the rendering destination of
    ///     this target.
    /// </summary>
    public static Matrix DpiTransformFromDevice
    {
        get
        {
            EnsureDpiData();
            return s_dpiTransformFromDevice;
        }
    }

    private static double s_dpiX;

    /// <summary>
    ///     Gets the system horizontal dots per inch (dpi).
    /// </summary>
    public static double DpiX
    {
        get
        {
            EnsureDpiData();
            return s_dpiX;
        }
    }

    private static double s_dpiY;

    /// <summary>
    ///     Gets the system vertical dots per inch (dpi).
    /// </summary>
    public static double DpiY
    {
        get
        {
            EnsureDpiData();
            return s_dpiX;
        }
    }

    /// <summary>
    ///     Safely gets the system DPI. Using <see cref="PresentationSource" /> will not work in partial trust.
    /// </summary>
    private static void EnsureDpiData()
    {
        if (s_dpiX == 0)
        {
            using (Bitmap bitmap = new(1, 1))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                s_dpiX = g.DpiX;
                s_dpiY = g.DpiY;
            }

            s_dpiTransformToDevice = Matrix.Identity;
            s_dpiTransformToDevice.Scale(s_dpiX / 96d, s_dpiY / 96d);

            s_dpiTransformFromDevice = Matrix.Identity;
            s_dpiTransformFromDevice.Scale(96d / s_dpiX, 96d / s_dpiY);
        }
    }
    /// <summary>
    ///     Gets the generated containers of all items in an <see cref="ItemsControl" />.
    /// </summary>
    /// <param name="itemsControl">The items control.</param>
    /// <returns></returns>
    public static IEnumerable<DependencyObject> GetItemContainers(this ItemsControl itemsControl)
    {
        for (int i = 0; i < itemsControl.Items.Count; ++i)
        {
            yield return itemsControl.ItemContainerGenerator.ContainerFromIndex(i);
        }
    }
    private static int s_launchBrowserRequests;
    private const int MaxBrowserRequests = 3;

    /// <summary>
    ///     Gets or sets a value indicating whether clicking a <see cref="Hyperlink" /> that has a URI
    ///     automatically launches the browser.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the browser is launched automatically; otherwise, <c>false</c>. The default is <c>false</c>.
    /// </value>
    public static bool IsAutomaticBrowserLaunchEnabled { get; set; }

    /// <summary>
    ///     Launches the browser.
    ///     <remarks>Provides accidental click flood protection.</remarks>
    /// </summary>
    /// <param name="uri">The URI.</param>
    public static void LaunchBrowser(Uri uri)
    {
        if (!uri.IsAbsoluteUri)
        {
            return;
        }

        if (s_launchBrowserRequests >= MaxBrowserRequests)
        {
            return;
        }

        _ = Interlocked.Increment(ref s_launchBrowserRequests);
        _ = ThreadPool.QueueUserWorkItem(LaunchBrowserCallback, uri);
    }

    private static void LaunchBrowserCallback(object state)
    {
        try
        {
            ProcessStartInfo startInfo = new()
            {
                UseShellExecute = true,
                FileName = ((Uri)state).AbsoluteUri
            };

            _ = Process.Start(startInfo);
        }
        finally
        {
            _ = Interlocked.Decrement(ref s_launchBrowserRequests);
        }
    }

    private static void OnHyperlinkClick(object sender, RoutedEventArgs e)
    {
        if (IsAutomaticBrowserLaunchEnabled)
        {
            Uri uri = ((Hyperlink)e.Source).NavigateUri;
            if (uri != null)
            {
                LaunchBrowser(uri);
            }
        }
    }
}
