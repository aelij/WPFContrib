using System.Windows;
using System.Windows.Controls;

namespace Avalon.Windows.Controls
{
    /// <summary>
    ///     Selects the appropriate template for a <see cref="TaskDialogButtonData" /> item.
    /// </summary>
    public class TaskDialogButtonTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        ///     Returns a <see cref="T:System.Windows.DataTemplate" /> for <see cref="TaskDialogButtonData" />.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var fe = container as FrameworkElement;
            if (fe != null && item is TaskDialogButtonData)
            {
                return fe.FindResource(new DataTemplateKey(typeof (TaskDialogButtonData))) as DataTemplate;
            }

            return null;
        }
    }
}