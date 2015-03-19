using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Avalon.Windows.Controls
{
    partial class TaskDialog
    {
        #region Fields

        private InlineModalDialog _inlineModalDialog;

        #endregion

        #region Static - Common

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header)
        {
            return ShowInlineCore(null, header, null, string.Empty, TaskDialogButtons.OK, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string title)
        {
            return ShowInlineCore(null, header, null, title, DefaultButton, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header)
        {
            return ShowInlineCore(owner, header, null, string.Empty, DefaultButton, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string title, TaskDialogButtons standardButtons)
        {
            return ShowInlineCore(null, header, null, title, standardButtons, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header, string title)
        {
            return ShowInlineCore(owner, header, null, title, DefaultButton, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string content, string title)
        {
            return ShowInlineCore(null, header, content, title, DefaultButton, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string content, string title, TaskDialogButtons standardButtons)
        {
            return ShowInlineCore(null, header, content, title, standardButtons, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header, string content, string title)
        {
            return ShowInlineCore(owner, header, content, title, DefaultButton, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string content, string title, TaskDialogButtons standardButtons, TaskDialogIcon icon)
        {
            return ShowInlineCore(null, header, content, title, standardButtons, icon, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header, string content, string title, TaskDialogButtons standardButtons)
        {
            return ShowInlineCore(owner, header, content, title, standardButtons, TaskDialogIcon.None, false);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header, string content, string title, TaskDialogButtons standardButtons, TaskDialogIcon icon)
        {
            return ShowInlineCore(owner, header, content, title, standardButtons, icon, false);
        }

        #endregion

        #region Static - TaskDialogButtonData

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink"/>s instead of <see cref="Button"/>s.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(string header, string content, string title, TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks, params TaskDialogButtonData[] buttons)
        {
            return ShowInlineCore(null, header, content, title, standardButtons, icon, useCommandLinks, buttons);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="standardButtons">The standard buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink"/>s instead of <see cref="Button"/>s.</param>
        /// <param name="buttons">The buttons.</param>
        /// <returns></returns>
        public static TaskDialogResult ShowInline(DependencyObject owner, string header, string content, string title, TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks, params TaskDialogButtonData[] buttons)
        {
            return ShowInlineCore(owner, header, content, title, standardButtons, icon, useCommandLinks, buttons);
        }

        #endregion

        #region Static - Strings

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButtonIndex">Default index of the button.</param>
        /// <param name="buttonContents">The button contents.</param>
        /// <returns></returns>
        public static int? ShowInline(string header, TaskDialogIcon icon, int defaultButtonIndex, params string[] buttonContents)
        {
            return ShowInlineCore(null, header, null, string.Empty, icon, false, defaultButtonIndex, buttonContents);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButtonIndex">Default index of the button.</param>
        /// <param name="buttonContents">The button contents.</param>
        /// <returns></returns>
        public static int? ShowInline(DependencyObject owner, string header, TaskDialogIcon icon, int defaultButtonIndex, params string[] buttonContents)
        {
            return ShowInlineCore(owner, header, null, string.Empty, icon, false, defaultButtonIndex, buttonContents);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink" />s instead of <see cref="Button" />s.</param>
        /// <param name="defaultButtonIndex">Default index of the button.</param>
        /// <param name="buttonContents">The button contents.</param>
        /// <returns></returns>
        public static int? ShowInline(string header, string content, string title, TaskDialogIcon icon, bool useCommandLinks, int defaultButtonIndex, params string[] buttonContents)
        {
            return ShowInlineCore(null, header, content, title, icon, useCommandLinks, defaultButtonIndex, buttonContents);
        }

        /// <summary>
        /// Displays a <see cref="TaskDialog"/> using an <see cref="InlineModalDialog" />.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="header">The header.</param>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink" />s instead of <see cref="Button" />s.</param>
        /// <param name="defaultButtonIndex">Default index of the button.</param>
        /// <param name="buttonContents">The button contents.</param>
        /// <returns></returns>
        public static int? ShowInline(DependencyObject owner, string header, string content, string title, TaskDialogIcon icon, bool useCommandLinks, int defaultButtonIndex, params string[] buttonContents)
        {
            return ShowInlineCore(owner, header, content, title, icon, useCommandLinks, defaultButtonIndex, buttonContents);
        }

        #endregion

        #region Core Show Methods

        private static int? ShowInlineCore(DependencyObject owner, string header, string content, string title, TaskDialogIcon icon, bool useCommandLinks, int defaultButtonIndex, params string[] buttonContents)
        {
            TaskDialogButtonData[] buttons = buttonContents.Select((s, i) => new TaskDialogButtonData(i, s, null, i == defaultButtonIndex)).ToArray();

            TaskDialogResult result = ShowInlineCore(owner, header, content, title, TaskDialogButtons.None, icon, useCommandLinks, buttons);

            int? index = null;

            if (result.ButtonData != null)
            {
                index = result.ButtonData.Value;
            }

            return index;
        }

        private static TaskDialogResult ShowInlineCore(DependencyObject owner, string header, string content, string title, TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks, params TaskDialogButtonData[] buttons)
        {
            if (title == null)
            {
                title = string.Empty;
            }

            if (content == string.Empty)
            {
                content = null;
            }

            var taskDialog = new TaskDialog
            {
                Title = title,
                Header = header,
                Content = content,
                MainIcon = TaskDialogIconConverter.ConvertFrom(icon)
            };

            foreach (TaskDialogButtonData buttonData in TaskDialogButtonData.FromStandardButtons(standardButtons))
            {
                taskDialog.Buttons.Add(buttonData);
            }

            if (useCommandLinks)
            {
                foreach (TaskDialogButtonData buttonData in buttons)
                {
                    taskDialog.CommandLinks.Add(buttonData);
                }
            }
            else
            {
                foreach (TaskDialogButtonData buttonData in buttons)
                {
                    taskDialog.Buttons.Add(buttonData);
                }
            }

            if (owner == null && Application.Current != null)
            {
                owner = Application.Current.MainWindow;
            }

            taskDialog.Background = SystemColors.WindowBrush;

            taskDialog.ShowInline(owner);

            return taskDialog.Result;
        }

        /// <summary>
        /// Shows the task dialog using an <see cref="InlineModalDialog"/>.
        /// </summary>
        /// <param name="owner"></param>
        public void ShowInline(DependencyObject owner)
        {
            _inlineModalDialog = new InlineModalDialog { Content = this, Owner = owner, Header = Title };
            _inlineModalDialog.Show();
        }

        partial void OnClosed()
        {
            if (_inlineModalDialog != null)
            {
                _inlineModalDialog.Close();
                _inlineModalDialog = null;
            }
        }

        #endregion
    }
}
