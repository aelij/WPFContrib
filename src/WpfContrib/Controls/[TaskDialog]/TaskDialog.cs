using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using Avalon.Internal.Win32;
using Avalon.Windows.Utility;

namespace Avalon.Windows.Controls;

/// <summary>
///     Represents a task dialog control.
/// </summary>
[TemplatePart(Name = "PART_ButtonList", Type = typeof(ItemsControl)),
 TemplatePart(Name = "PART_CommandLinkList", Type = typeof(ItemsControl)),
 TemplatePart(Name = "PART_RadioButtonList", Type = typeof(Selector))]
[StyleTypedProperty(Property = "WindowStyle", StyleTargetType = typeof(Window))]
public partial class TaskDialog : HeaderedContentControl
{
    internal const TaskDialogButtons DefaultButton = TaskDialogButtons.OK;

    private TaskDialogWindow _window;
    private bool _isWindowCreated;

    private ItemsControl _buttonList;
    private ItemsControl _commandLinkList;

    private Selector _radioButtonList;
    /// <summary>
    ///     Initializes the <see cref="TaskDialog" /> class.
    /// </summary>
    static TaskDialog()
    {
        FocusableProperty.OverrideMetadata(typeof(TaskDialog), new FrameworkPropertyMetadata(false));
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskDialog),
            new FrameworkPropertyMetadata(typeof(TaskDialog)));

        EventManager.RegisterClassHandler(typeof(TaskDialog), ButtonBase.ClickEvent,
            new RoutedEventHandler(OnButtonClick));

        CommandManager.RegisterClassCommandBinding(typeof(TaskDialog),
            new CommandBinding(CopyContentCommand, ExecuteCopyContentCommand));
        CommandManager.RegisterClassCommandBinding(typeof(TaskDialog),
            new CommandBinding(CancelCommand, ExecuteCancelCommand));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDialog" /> class.
    /// </summary>
    public TaskDialog()
    {
        SetValue(s_buttonsPropertyKey, new ObservableCollection<object>());
        SetValue(s_radioButtonsPropertyKey, new ObservableCollection<object>());
        SetValue(s_commandLinksPropertyKey, new ObservableCollection<object>());

        Buttons.CollectionChanged += OnButtonsCollectionChanged;
        Loaded += OnLoaded;
        IsVisibleChanged += OnIsVisibleChanged;
    }
    /// <summary>
    ///     Gets or sets a value indicating whether cancellation of the dialog is allowed.
    /// </summary>
    /// <value>
    ///     <c>true</c> if cancellation of the dialog is allowed; otherwise, <c>false</c>.
    /// </value>
    public bool AllowDialogCancellation
    {
        get { return (bool)GetValue(AllowDialogCancellationProperty); }
        set { SetValue(AllowDialogCancellationProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="AllowDialogCancellation" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty AllowDialogCancellationProperty =
        DependencyProperty.Register("AllowDialogCancellation", typeof(bool), typeof(TaskDialog),
            new FrameworkPropertyMetadata(true));
    /// <summary>
    ///     Gets or sets a value indicating whether to display the verification checkbox.
    /// </summary>
    /// <value><c>true</c> if the verification checkbox is displayed; otherwise, <c>false</c>.</value>
    public bool ShowVerification
    {
        get { return (bool)GetValue(ShowVerificationProperty); }
        set { SetValue(ShowVerificationProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ShowVerification" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowVerificationProperty =
        DependencyProperty.Register("ShowVerification", typeof(bool), typeof(TaskDialog),
            new FrameworkPropertyMetadata(false));

    /// <summary>
    ///     Gets or sets a value indicating whether the verification checkbox is checked.
    /// </summary>
    /// <value><c>true</c> if the verification is checked; <c>false</c> if the verification is unchecked; otherwise null.</value>
    public bool? IsVerificationChecked
    {
        get { return (bool?)GetValue(IsVerificationCheckedProperty); }
        set { SetValue(IsVerificationCheckedProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="IsVerificationChecked" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsVerificationCheckedProperty =
        DependencyProperty.Register("IsVerificationChecked", typeof(bool?), typeof(TaskDialog),
            new FrameworkPropertyMetadata(false));
    /// <summary>
    ///     Gets or sets the verification content.
    /// </summary>
    /// <value>The verification content.</value>
    public object Verification
    {
        get { return GetValue(VerificationProperty); }
        set { SetValue(VerificationProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Verification" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerificationProperty =
        DependencyProperty.Register("Verification", typeof(object), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the data template used to display the <see cref="Verification" /> content.
    /// </summary>
    /// <value>The template.</value>
    public DataTemplate VerificationTemplate
    {
        get { return (DataTemplate)GetValue(VerificationTemplateProperty); }
        set { SetValue(VerificationTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="VerificationTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerificationTemplateProperty =
        DependencyProperty.Register("VerificationTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
    /// </summary>
    /// <value>The template selector.</value>
    public DataTemplateSelector VerificationTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(VerificationTemplateSelectorProperty); }
        set { SetValue(VerificationTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="VerificationTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty VerificationTemplateSelectorProperty =
        DependencyProperty.Register("VerificationTemplateSelector", typeof(DataTemplateSelector),
            typeof(TaskDialog), new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the owner.
    ///     <remarks>
    ///         The owner helps determine the owner window the dialog will belong to.
    ///     </remarks>
    /// </summary>
    /// <value>The owner.</value>
    public DependencyObject Owner
    {
        get { return (DependencyObject)GetValue(OwnerProperty); }
        set { SetValue(OwnerProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Owner" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty OwnerProperty =
        DependencyProperty.Register("Owner", typeof(DependencyObject), typeof(TaskDialog),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets the selected item.
    ///     <remarks>
    ///         The selected item will be either a button or a command link.
    ///         The item type is either a <see cref="TaskDialogIcon" />,
    ///         an item from the <see cref="Buttons" /> collection or
    ///         an item from the <see cref="CommandLinks" /> collection, respectively.
    ///     </remarks>
    /// </summary>
    /// <value>The selected item.</value>
    public object SelectedItem
    {
        get { return GetValue(SelectedItemProperty); }
        private set { SetValue(s_selectedItemPropertyKey, value); }
    }

    private static readonly DependencyPropertyKey s_selectedItemPropertyKey =
        DependencyProperty.RegisterReadOnly("SelectedItem", typeof(object), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Identifies the <see cref="SelectedItem" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedItemProperty = s_selectedItemPropertyKey.DependencyProperty;
    /// <summary>
    ///     Gets or sets a value indicating whether to display the progress bar.
    /// </summary>
    /// <value><c>true</c> if the progress bar is displayed; otherwise, <c>false</c>.</value>
    public bool ShowProgressBar
    {
        get { return (bool)GetValue(ShowProgressBarProperty); }
        set { SetValue(ShowProgressBarProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ShowProgressBar" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowProgressBarProperty =
        DependencyProperty.Register("ShowProgressBar", typeof(bool), typeof(TaskDialog),
            new FrameworkPropertyMetadata(false));

    /// <summary>
    ///     Gets or sets a value indicating whether the progress bar is indeterminate.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this the progress bar is indeterminate; otherwise, <c>false</c>.
    /// </value>
    public bool IsProgressIndeterminate
    {
        get { return (bool)GetValue(IsProgressIndeterminateProperty); }
        set { SetValue(IsProgressIndeterminateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="IsProgressIndeterminate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsProgressIndeterminateProperty =
        DependencyProperty.Register("IsProgressIndeterminate", typeof(bool), typeof(TaskDialog),
            new UIPropertyMetadata(false));
    /// <summary>
    ///     Gets or sets the window title.
    /// </summary>
    /// <value>The window title.</value>
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Title" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
        Window.TitleProperty.AddOwner(typeof(TaskDialog));
    /// <summary>
    ///     Gets or sets a value indicating whether the is footer visible.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the is footer visible; otherwise, <c>false</c>.
    /// </value>
    public bool ShowFooter
    {
        get { return (bool)GetValue(ShowFooterProperty); }
        set { SetValue(ShowFooterProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ShowFooter" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowFooterProperty =
        DependencyProperty.Register("ShowFooter", typeof(bool), typeof(TaskDialog),
            new FrameworkPropertyMetadata(false));

    /// <summary>
    ///     Gets or sets the footer content.
    /// </summary>
    /// <value>The footer content.</value>
    public object Footer
    {
        get { return GetValue(FooterProperty); }
        set { SetValue(FooterProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Footer" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register("Footer", typeof(object), typeof(TaskDialog), new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the data template used to display the <see cref="Footer" /> content.
    /// </summary>
    /// <value>The template.</value>
    public DataTemplate FooterTemplate
    {
        get { return (DataTemplate)GetValue(FooterTemplateProperty); }
        set { SetValue(FooterTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="FooterTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FooterTemplateProperty =
        DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
    /// </summary>
    /// <value>The template selector.</value>
    public DataTemplateSelector FooterTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(FooterTemplateSelectorProperty); }
        set { SetValue(FooterTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="FooterTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FooterTemplateSelectorProperty =
        DependencyProperty.Register("FooterTemplateSelector", typeof(DataTemplateSelector), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets a value indicating whether the expansion content is expanded.
    /// </summary>
    /// <value>
    ///     <c>true</c> if expansion content is expanded; otherwise, <c>false</c>.
    /// </value>
    public bool IsExpanded
    {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="IsExpanded" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the expansion position.
    /// </summary>
    /// <value>The expansion position.</value>
    public TaskDialogExpansionPosition ExpansionPosition
    {
        get { return (TaskDialogExpansionPosition)GetValue(ExpansionPositionProperty); }
        set { SetValue(ExpansionPositionProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionPosition" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionPositionProperty =
        DependencyProperty.Register("ExpansionPosition", typeof(TaskDialogExpansionPosition), typeof(TaskDialog),
            new FrameworkPropertyMetadata(TaskDialogExpansionPosition.None));
    /// <summary>
    ///     Gets or sets the expansion content.
    /// </summary>
    /// <value>The expansion content.</value>
    public object ExpansionContent
    {
        get { return GetValue(ExpansionContentProperty); }
        set { SetValue(ExpansionContentProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionContent" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionContentProperty =
        DependencyProperty.Register("ExpansionContent", typeof(object), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the data template used to display the <see cref="ExpansionContent" />.
    /// </summary>
    /// <value>The template.</value>
    public DataTemplate ExpansionContentTemplate
    {
        get { return (DataTemplate)GetValue(ExpansionContentTemplateProperty); }
        set { SetValue(ExpansionContentTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionContentTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionContentTemplateProperty =
        DependencyProperty.Register("ExpansionContentTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
    /// </summary>
    /// <value>The template selector.</value>
    public DataTemplateSelector ExpansionContentTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(ExpansionContentTemplateSelectorProperty); }
        set { SetValue(ExpansionContentTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionContentTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionContentTemplateSelectorProperty =
        DependencyProperty.Register("ExpansionContentTemplateSelector", typeof(DataTemplateSelector),
            typeof(TaskDialog), new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the content of the expansion button.
    /// </summary>
    /// <value>The content of the expansion button.</value>
    public object ExpansionButtonContent
    {
        get { return GetValue(ExpansionButtonContentProperty); }
        set { SetValue(ExpansionButtonContentProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionButtonContent" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionButtonContentProperty =
        DependencyProperty.Register("ExpansionButtonContent", typeof(object), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the data template used to display the <see cref="ExpansionButtonContent" />.
    /// </summary>
    /// <value>The template.</value>
    public DataTemplate ExpansionButtonContentTemplate
    {
        get { return (DataTemplate)GetValue(ExpansionButtonContentTemplateProperty); }
        set { SetValue(ExpansionButtonContentTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionButtonContentTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionButtonContentTemplateProperty =
        DependencyProperty.Register("ExpansionButtonContentTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
    /// </summary>
    /// <value>The template selector.</value>
    public DataTemplateSelector ExpansionButtonContentTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(ExpansionButtonContentTemplateSelectorProperty); }
        set { SetValue(ExpansionButtonContentTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ExpansionButtonContentTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ExpansionButtonContentTemplateSelectorProperty =
        DependencyProperty.Register("ExpansionButtonContentTemplateSelector", typeof(DataTemplateSelector),
            typeof(TaskDialog), new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the main icon.
    /// </summary>
    /// <value>The main icon.</value>
    [TypeConverter(typeof(TaskDialogIconConverter))]
    public ImageSource MainIcon
    {
        get { return (ImageSource)GetValue(MainIconProperty); }
        set { SetValue(MainIconProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="MainIcon" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty MainIconProperty =
        DependencyProperty.Register("MainIcon", typeof(ImageSource), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the footer icon.
    /// </summary>
    /// <value>The footer icon.</value>
    [TypeConverter(typeof(TaskDialogIconConverter))]
    public ImageSource FooterIcon
    {
        get { return (ImageSource)GetValue(FooterIconProperty); }
        set { SetValue(FooterIconProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="FooterIcon" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty FooterIconProperty =
        DependencyProperty.Register("FooterIcon", typeof(ImageSource), typeof(TaskDialog),
            new FrameworkPropertyMetadata()); private TaskDialogButtons _standardButtons;

    /// <summary>
    ///     Gets or sets the standard buttons.
    /// </summary>
    /// <value>The standard buttons.</value>
    [Obsolete(
        "Use the Buttons property and add a TaskDialogButtonData that accepts TaskDialogButtons in the constructor."
        )]
    public TaskDialogButtons StandardButtons
    {
        get { return _standardButtons; }
        set
        {
            // remove all current standard buttons
            foreach (
                TaskDialogButtonData buttonData in
                    Buttons.OfType<TaskDialogButtonData>().Where(b => b.Button != TaskDialogButtons.No).ToArray())
            {
                _ = Buttons.Remove(buttonData);
            }
            // add new ones according to value
            foreach (TaskDialogButtonData buttonData in TaskDialogButtonData.FromStandardButtons(value).Reverse())
            {
                Buttons.Insert(0, buttonData);
            }

            _standardButtons = value;
        }
    }

    /// <summary>
    ///     Added to support the deprecated <see cref="P:StandardButtons" /> property.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    ///     The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing
    ///     the event data.
    /// </param>
    private void OnButtonsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        _standardButtons = Buttons.OfType<TaskDialogButtonData>().Where(b => b.Button != TaskDialogButtons.None)
            .Aggregate(TaskDialogButtons.None, (b, bd) => b | bd.Button);
    }

    /// <summary>
    ///     Gets the button collection.
    /// </summary>
    /// <value>The button collection.</value>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public ObservableCollection<object> Buttons
    {
        get { return (ObservableCollection<object>)GetValue(ButtonsProperty); }
    }

    private static readonly DependencyPropertyKey s_buttonsPropertyKey =
        DependencyProperty.RegisterReadOnly("Buttons", typeof(ObservableCollection<object>), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Identifies the <see cref="Buttons" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonsProperty = s_buttonsPropertyKey.DependencyProperty;

    /// <summary>
    ///     Gets or sets the radio button collection.
    /// </summary>
    /// <value>The radio button collection.</value>
    public ObservableCollection<object> RadioButtons
    {
        get { return (ObservableCollection<object>)GetValue(RadioButtonsProperty); }
    }

    private static readonly DependencyPropertyKey s_radioButtonsPropertyKey =
        DependencyProperty.RegisterReadOnly("RadioButtons", typeof(ObservableCollection<object>),
            typeof(TaskDialog), new FrameworkPropertyMetadata());

    /// <summary>
    ///     Identifies the <see cref="RadioButtons" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadioButtonsProperty = s_radioButtonsPropertyKey.DependencyProperty;

    /// <summary>
    ///     Gets or sets the command link collection.
    /// </summary>
    /// <value>The command link collection.</value>
    public ObservableCollection<object> CommandLinks
    {
        get { return (ObservableCollection<object>)GetValue(CommandLinksProperty); }
    }

    private static readonly DependencyPropertyKey s_commandLinksPropertyKey =
        DependencyProperty.RegisterReadOnly("CommandLinks", typeof(ObservableCollection<object>),
            typeof(TaskDialog), new FrameworkPropertyMetadata());

    /// <summary>
    ///     Identifies the <see cref="CommandLinks" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandLinksProperty = s_commandLinksPropertyKey.DependencyProperty;
    /// <summary>
    ///     Gets or sets the button container style.
    /// </summary>
    /// <value>The button container style.</value>
    public Style ButtonContainerStyle
    {
        get { return (Style)GetValue(ButtonContainerStyleProperty); }
        set { SetValue(ButtonContainerStyleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ButtonContainerStyle" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonContainerStyleProperty =
        DependencyProperty.Register("ButtonContainerStyle", typeof(Style), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the button container style selector.
    /// </summary>
    /// <value>The button container style selector.</value>
    public StyleSelector ButtonContainerStyleSelector
    {
        get { return (StyleSelector)GetValue(ButtonContainerStyleSelectorProperty); }
        set { SetValue(ButtonContainerStyleSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ButtonContainerStyleSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonContainerStyleSelectorProperty =
        DependencyProperty.Register("ButtonContainerStyleSelector", typeof(StyleSelector), typeof(TaskDialog),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the command link container style.
    /// </summary>
    /// <value>The command link container style.</value>
    public Style CommandLinkContainerStyle
    {
        get { return (Style)GetValue(CommandLinkContainerStyleProperty); }
        set { SetValue(CommandLinkContainerStyleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="CommandLinkContainerStyle" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandLinkContainerStyleProperty =
        DependencyProperty.Register("CommandLinkContainerStyle", typeof(Style), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the command link container style selector.
    /// </summary>
    /// <value>The command link container style selector.</value>
    public StyleSelector CommandLinkContainerStyleSelector
    {
        get { return (StyleSelector)GetValue(CommandLinkContainerStyleSelectorProperty); }
        set { SetValue(CommandLinkContainerStyleSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="CommandLinkContainerStyleSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandLinkContainerStyleSelectorProperty =
        DependencyProperty.Register("CommandLinkContainerStyleSelector", typeof(StyleSelector), typeof(TaskDialog),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the radio button container style.
    /// </summary>
    /// <value>The radio button container style.</value>
    public Style RadioButtonContainerStyle
    {
        get { return (Style)GetValue(RadioButtonContainerStyleProperty); }
        set { SetValue(RadioButtonContainerStyleProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="RadioButtonContainerStyle" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadioButtonContainerStyleProperty =
        DependencyProperty.Register("RadioButtonContainerStyle", typeof(Style), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the radio button container style selector.
    /// </summary>
    /// <value>The radio button container style selector.</value>
    public StyleSelector RadioButtonContainerStyleSelector
    {
        get { return (StyleSelector)GetValue(RadioButtonContainerStyleSelectorProperty); }
        set { SetValue(RadioButtonContainerStyleSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="RadioButtonContainerStyleSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadioButtonContainerStyleSelectorProperty =
        DependencyProperty.Register("RadioButtonContainerStyleSelector", typeof(StyleSelector), typeof(TaskDialog),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the button template.
    /// </summary>
    /// <value>The button template.</value>
    public DataTemplate ButtonTemplate
    {
        get { return (DataTemplate)GetValue(ButtonTemplateProperty); }
        set { SetValue(ButtonTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ButtonTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonTemplateProperty =
        DependencyProperty.Register("ButtonTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the button template selector.
    /// </summary>
    /// <value>The button template selector.</value>
    public DataTemplateSelector ButtonTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(ButtonTemplateSelectorProperty); }
        set { SetValue(ButtonTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="ButtonTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonTemplateSelectorProperty =
        DependencyProperty.Register("ButtonTemplateSelector", typeof(DataTemplateSelector), typeof(TaskDialog),
            new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the command link template.
    /// </summary>
    /// <value>The command link template.</value>
    public DataTemplate CommandLinkTemplate
    {
        get { return (DataTemplate)GetValue(CommandLinkTemplateProperty); }
        set { SetValue(CommandLinkTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="CommandLinkTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandLinkTemplateProperty =
        DependencyProperty.Register("CommandLinkTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the command link template selector.
    /// </summary>
    /// <value>The command link template selector.</value>
    public DataTemplateSelector CommandLinkTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(CommandLinkTemplateSelectorProperty); }
        set { SetValue(CommandLinkTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="CommandLinkTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandLinkTemplateSelectorProperty =
        DependencyProperty.Register("CommandLinkTemplateSelector", typeof(DataTemplateSelector),
            typeof(TaskDialog), new FrameworkPropertyMetadata());
    /// <summary>
    ///     Gets or sets the radio button template.
    /// </summary>
    /// <value>The radio button template.</value>
    public DataTemplate RadioButtonTemplate
    {
        get { return (DataTemplate)GetValue(RadioButtonTemplateProperty); }
        set { SetValue(RadioButtonTemplateProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="RadioButtonTemplate" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadioButtonTemplateProperty =
        DependencyProperty.Register("RadioButtonTemplate", typeof(DataTemplate), typeof(TaskDialog),
            new FrameworkPropertyMetadata());

    /// <summary>
    ///     Gets or sets the radio button template selector.
    /// </summary>
    /// <value>The radio button template selector.</value>
    public DataTemplateSelector RadioButtonTemplateSelector
    {
        get { return (DataTemplateSelector)GetValue(RadioButtonTemplateSelectorProperty); }
        set { SetValue(RadioButtonTemplateSelectorProperty, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="RadioButtonTemplateSelector" /> dependency property.
    /// </summary>
    public static readonly DependencyProperty RadioButtonTemplateSelectorProperty =
        DependencyProperty.Register("RadioButtonTemplateSelector", typeof(DataTemplateSelector),
            typeof(TaskDialog), new FrameworkPropertyMetadata());
    /// <summary>
    /// The default key for the <see cref="WindowStyle"/> property.
    /// Can be used to base a custom style on the default one.
    /// </summary>
    public static readonly ResourceKey DefaultWindowStyleKey = new ComponentResourceKey(typeof(TaskDialogWindow), "TaskDialogWindowStyle");

    /// <summary>
    /// Identifies the <see cref="WindowStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty WindowStyleProperty = DependencyProperty.Register(
        "WindowStyle", typeof(Style), typeof(TaskDialog), new FrameworkPropertyMetadata(CreateDefaultWindowStyle()));

    private static Style CreateDefaultWindowStyle()
    {
        var style = new Style(typeof(Window));
        style.Setters.Add(new Setter(Window.ResizeModeProperty, ResizeMode.NoResize));
        style.Setters.Add(new Setter(Window.ShowInTaskbarProperty, false));
        style.Setters.Add(new Setter(Window.SizeToContentProperty, SizeToContent.WidthAndHeight));
        style.Setters.Add(new Setter(Window.WindowStyleProperty, System.Windows.WindowStyle.SingleBorderWindow));
        style.Setters.Add(new Setter(BindingHelpers.WindowStartupLocationProperty, WindowStartupLocation.CenterOwner));
        style.Seal();
        return style;
    }

    /// <summary>
    /// Gets or sets the <see cref="Style"/> of the <see cref="Window"/>
    /// used to open the task dialog in the <see cref="Show()"/> method.
    /// </summary>
    /// <value>The window style.</value>
    public Style WindowStyle
    {
        get { return (Style)GetValue(WindowStyleProperty); }
        set { SetValue(WindowStyleProperty, value); }
    }
    /// <summary>
    ///     Identifies the <c>CopyContent</c> routed command.
    /// </summary>
    public static readonly RoutedCommand CopyContentCommand = new("CopyContent", typeof(TaskDialog),
        new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control) });

    /// <summary>
    ///     Executes the <see cref="CopyContentCommand" />.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
    private static void ExecuteCopyContentCommand(object sender, ExecutedRoutedEventArgs e)
    {
        ((TaskDialog)sender).CopyContent();
    }

    /// <summary>
    ///     Identifies the <c>Cancel</c> routed command.
    /// </summary>
    public static readonly RoutedCommand CancelCommand = new("Cancel", typeof(TaskDialog),
        new InputGestureCollection { new KeyGesture(Key.Escape) });

    /// <summary>
    ///     Executes the <see cref="CancelCommand" />.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs" /> instance containing the event data.</param>
    private static void ExecuteCancelCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = (TaskDialog)sender;

        if (dialog.AllowDialogCancellation)
        {
            dialog.SelectedItem = null;

            dialog.Close();
        }
    }
    /// <summary>
    ///     Occurs when the dialog is closed.
    /// </summary>
    public event RoutedEventHandler Closed
    {
        add { AddHandler(ClosedEvent, value); }
        remove { RemoveHandler(ClosedEvent, value); }
    }

    /// <summary>
    ///     Identifies the <see cref="Closed" /> routed event.
    /// </summary>
    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed",
        RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TaskDialog));
    /// <summary>
    ///     Gets the <see cref="TaskDialogResult" />.
    /// </summary>
    /// <value>The result.</value>
    public TaskDialogResult Result
    {
        get
        {
            var result = new TaskDialogResult
            {
                Button = SelectedItem,
                ButtonData = SelectedItem as TaskDialogButtonData,
                IsVerified = IsVerificationChecked == true
            };

            if (_radioButtonList != null)
            {
                result.RadioButton = _radioButtonList.SelectedItem;
            }

            return result;
        }
    }

    /// <summary>
    ///     Gets the text of the entire content of the dialog.
    ///     <remarks>
    ///         This can be copied to the clipboard using Ctrl-C or the <see cref="CopyContentCommand" />.
    ///     </remarks>
    /// </summary>
    /// <value>The content text.</value>
    public string ContentText
    {
        get
        {
            var sb = new StringBuilder();

            // title
            if (!string.IsNullOrEmpty(Title))
            {
                _ = sb.Append('[');
                _ = sb.Append(SR.TaskDialog_WindowTitle);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                _ = sb.AppendLine(Title);
                _ = sb.AppendLine();
            }

            // header
            AppendContent(Header, SR.TaskDialog_MainInstruction, sb);

            // content
            AppendContent(Content, SR.TaskDialog_Content, sb);

            // expansion (header)
            if (ExpansionPosition == TaskDialogExpansionPosition.Header)
            {
                AppendContent(ExpansionContent, SR.TaskDialog_ExpandedInformation, sb);
            }

            // radio buttons
            if (_radioButtonList != null && RadioButtons.Count > 0)
            {
                _ = sb.Append('[');
                _ = sb.Append(SR.TaskDialog_RadioButtons);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                foreach (RadioButton radioButton in _radioButtonList.GetItemContainers().OfType<RadioButton>())
                {
                    _ = sb.AppendFormat("[{0}] {1} ", GetBooleanString(radioButton.IsChecked),
                        GetContentText(radioButton.Content));
                }
                _ = sb.AppendLine();
                _ = sb.AppendLine();
            }

            // command links
            if (CommandLinks.Count > 0)
            {
                _ = sb.Append('[');
                _ = sb.Append(SR.TaskDialog_CommandLinks);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                foreach (object o in CommandLinks)
                {
                    _ = sb.AppendFormat(" -> {0}", GetContentText(o));
                    _ = sb.AppendLine();
                }
                _ = sb.AppendLine();
            }

            // expand button
            if (ExpansionPosition != TaskDialogExpansionPosition.None)
            {
                _ = sb.AppendFormat("[{0}] {1} ", IsExpanded ? '^' : 'V', GetContentText(ExpansionButtonContent));
            }

            // verification
            string verificationText = GetContentText(Verification);
            if (!string.IsNullOrEmpty(verificationText))
            {
                _ = sb.AppendFormat("[{0}] {1} ", GetBooleanString(IsVerificationChecked), verificationText);
            }

            _ = sb.AppendLine();
            _ = sb.AppendLine();

            // buttons
            if (Buttons.Count > 0)
            {
                _ = sb.Append('[');
                _ = sb.Append(SR.TaskDialog_Buttons);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                _ = sb.Append("   ");
                foreach (object o in Buttons)
                {
                    _ = sb.AppendFormat("[{0}] ", GetContentText(o));
                }
            }

            _ = sb.AppendLine();
            _ = sb.AppendLine();

            // footer
            string footerText = GetContentText(Footer);
            if (!string.IsNullOrEmpty(footerText))
            {
                _ = sb.Append('[');
                _ = sb.Append(SR.TaskDialog_Footer);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                _ = sb.AppendLine(footerText);
                _ = sb.AppendLine();
            }

            // expansion (footer)
            if (ExpansionPosition == TaskDialogExpansionPosition.Footer)
            {
                AppendContent(ExpansionContent, SR.TaskDialog_ExpandedInformation, sb);
            }

            return sb.ToString();
        }
    }
    /// <summary>
    ///     Displays the <see cref="TaskDialog" /> in a <see cref="Window" />.
    ///     <remarks>
    ///         The control must not be parented prior to calling this method.
    ///     </remarks>
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance already has a parent.</exception>
    public void Show()
    {
        if (Parent != null)
        {
            throw new InvalidOperationException(SR.TaskDialog_NoParent);
        }

        Window ownerWindow = null;
        if (Owner != null)
        {
            ownerWindow = Window.GetWindow(Owner);
        }

        _window = new TaskDialogWindow
        {
            Content = this,
            Owner = ownerWindow,
            Title = Title,
            IsClosable = AllowDialogCancellation,
            Style = WindowStyle
        };

        _window.Closed += OnWindowClosed;

        _isWindowCreated = true;
        _ = _window.ShowDialog();

        _window.Closed -= OnWindowClosed;

        _window.Content = null;
        _window = null;
        _isWindowCreated = false;
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header)
    {
        return ShowCore(null, header, null, null, TaskDialogButtons.OK, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string title)
    {
        return ShowCore(null, header, null, title, DefaultButton, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header)
    {
        return ShowCore(owner, header, null, null, DefaultButton, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string title, TaskDialogButtons standardButtons)
    {
        return ShowCore(null, header, null, title, standardButtons, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header, string title)
    {
        return ShowCore(owner, header, null, title, DefaultButton, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string content, string title)
    {
        return ShowCore(null, header, content, title, DefaultButton, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string content, string title,
        TaskDialogButtons standardButtons)
    {
        return ShowCore(null, header, content, title, standardButtons, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header, string content, string title)
    {
        return ShowCore(owner, header, content, title, DefaultButton, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <param name="icon">The icon.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string content, string title,
        TaskDialogButtons standardButtons, TaskDialogIcon icon)
    {
        return ShowCore(null, header, content, title, standardButtons, icon, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header, string content, string title,
        TaskDialogButtons standardButtons)
    {
        return ShowCore(owner, header, content, title, standardButtons, TaskDialogIcon.None, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <param name="icon">The icon.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header, string content, string title,
        TaskDialogButtons standardButtons, TaskDialogIcon icon)
    {
        return ShowCore(owner, header, content, title, standardButtons, icon, false);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink" />s instead of <see cref="Button" />s.</param>
    /// <param name="buttons">The buttons.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(string header, string content, string title,
        TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks,
        params TaskDialogButtonData[] buttons)
    {
        return ShowCore(null, header, content, title, standardButtons, icon, useCommandLinks, buttons);
    }

    /// <summary>
    ///     Displays a <see cref="TaskDialog" />.
    /// </summary>
    /// <param name="owner">The owner.</param>
    /// <param name="header">The header.</param>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="standardButtons">The standard buttons.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="useCommandLinks">if set to <c>true</c>, use <see cref="CommandLink" />s instead of <see cref="Button" />s.</param>
    /// <param name="buttons">The buttons.</param>
    /// <returns></returns>
    public static TaskDialogResult Show(DependencyObject owner, string header, string content, string title,
        TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks,
        params TaskDialogButtonData[] buttons)
    {
        return ShowCore(owner, header, content, title, standardButtons, icon, useCommandLinks, buttons);
    }

    private static TaskDialogResult ShowCore(DependencyObject owner, string header, string content, string title,
        TaskDialogButtons standardButtons, TaskDialogIcon icon, bool useCommandLinks,
        params TaskDialogButtonData[] buttons)
    {
        var taskDialog = new TaskDialog
        {
            Owner = owner,
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

        taskDialog.Show();

        return taskDialog.Result;
    }
    /// <summary>
    ///     Hooks up template parts.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _buttonList = GetTemplateChild("PART_ButtonList") as ItemsControl;
        _commandLinkList = GetTemplateChild("PART_CommandLinkList") as ItemsControl;
        _radioButtonList = GetTemplateChild("PART_RadioButtonList") as Selector;
    }

    /// <summary>
    ///     Attempts to set focus to the default button.
    /// </summary>
    private void FocusDefaultButton()
    {
        // first we try to find an item (button or a commandlink) whose item is
        // a TaskDialogButtonData or a Button and the IsDefault is set to true
        Button defaultButton = FindDefaultButton(Buttons, _buttonList);
        if (defaultButton == null)
        {
            if (_commandLinkList != null)
            {
                defaultButton = FindDefaultButton(CommandLinks, _commandLinkList);
            }

            // otherwise, we take the first button or commandlink, whichever exists
            if (defaultButton == null)
            {
                if (Buttons.Count > 0 && _buttonList != null)
                {
                    defaultButton = _buttonList.GetItemContainers().FirstOrDefault() as Button;
                }
                else if (_commandLinkList != null && CommandLinks.Count > 0)
                {
                    defaultButton = _commandLinkList.GetItemContainers().FirstOrDefault() as Button;
                }
            }
        }
        if (defaultButton != null)
        {
            _ = defaultButton.Focus();
        }
    }

    /// <summary>
    ///     Finds the default button in the list of objects.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="itemsControl">The items control.</param>
    /// <returns></returns>
    private static Button FindDefaultButton(IList<object> items, ItemsControl itemsControl)
    {
        Button defaultButton = null;

        foreach (object item in items)
        {
            if (item is TaskDialogButtonData buttonData)
            {
                if (buttonData.IsDefault)
                {
                    defaultButton = itemsControl.ItemContainerGenerator.ContainerFromItem(buttonData) as Button;
                    if (defaultButton != null)
                    {
                        break;
                    }
                }
            }
            else
            {
                if (item is Button button)
                {
                    if (button.IsDefault)
                    {
                        defaultButton = button;
                        break;
                    }
                }
            }
        }

        return defaultButton;
    }

    /// <summary>
    ///     Copies the value of <see cref="ContentText" /> to the clipboard.
    /// </summary>
    private void CopyContent()
    {
        try
        {
            Clipboard.SetText(ContentText);
        }
        catch (COMException)
        {
        } // clipboard may fail
    }

    /// <summary>
    ///     Formats a nullable boolean value as a character. Used by <see cref="ContentText" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    private static char GetBooleanString(bool? value)
    {
        if (value == null)
        {
            return 'O';
        }
        return value == true ? 'X' : ' ';
    }

    /// <summary>
    ///     Appends the given content and adds a title. Used by <see cref="ContentText" />.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="title">The title.</param>
    /// <param name="sb">The sb.</param>
    private static void AppendContent(object content, string title, StringBuilder sb)
    {
        if (content != null)
        {
            string textContent = GetContentText(content);

            if (textContent.Length > 0)
            {
                _ = sb.Append('[');
                _ = sb.Append(title);
                _ = sb.Append(']');
                _ = sb.AppendLine();
                _ = sb.AppendLine(textContent);
                _ = sb.AppendLine();
                _ = sb.AppendLine();
            }
        }
    }

    /// <summary>
    ///     Gets a textual representation of the specified content object. Used by <see cref="ContentText" />.
    ///     <remarks>
    ///         If the content is a <see cref="Visual" />, its visual tree is walked and all text from <see cref="TextBlock" />
    ///         s
    ///         and <see cref="FlowDocument" />s is retrieved. Otherwise, the <see cref="object.ToString" /> method is called.
    ///     </remarks>
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>A textual representation of the specified content object.</returns>
    private static string GetContentText(object content)
    {
        var stringBuilder = new StringBuilder();

        DependencyObject contentObject = content as Visual;
        if (contentObject != null)
        {
            // the lambda always return false, so the entire visual tree is walked
            _ = contentObject.FindVisualDescendant(o =>
            {
                if (o is TextBlock tb && !string.IsNullOrEmpty(tb.Text))
                {
                    _ = stringBuilder.AppendLine(tb.Text);
                }
                else
                {
                    FlowDocument doc = null;
                    if (o is FlowDocumentPageViewer pageViewer)
                    {
                        doc = pageViewer.Document as FlowDocument;
                    }
                    else
                    {
                        if (o is FlowDocumentReader reader)
                        {
                            doc = reader.Document;
                        }
                        else
                        {
                            if (o is FlowDocumentScrollViewer scrollViewer)
                            {
                                doc = scrollViewer.Document;
                            }
                            else
                            {
                                if (o is RichTextBox rtb)
                                {
                                    doc = rtb.Document;
                                }
                            }
                        }
                    }

                    if (doc != null)
                    {
                        string text = new TextRange(doc.ContentStart, doc.ContentEnd).Text;
                        if (text.Length > 0)
                        {
                            _ = stringBuilder.AppendLine(text);
                        }
                    }
                }
                return false;
            });
        }
        else
        {
            _ = stringBuilder.Append(content);
        }

        return stringBuilder.ToString();
    }

    private void Close()
    {
        if (_isWindowCreated)
        {
            // the window's Closed event will raise the dialog's Closed event
            CloseWindow();
        }
        else
        {
            OnClosed();
            RaiseEvent(new RoutedEventArgs(ClosedEvent));
        }
    }

    partial void OnClosed();

    private void CloseWindow()
    {
        _window.IsClosable = true;
        _window.Close();
    }
    private static void OnButtonClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is Button button)
        {
            if (e.Source is TaskDialog dialog)
            {
                object selectedItem = null;

                if (dialog._buttonList != null)
                {
                    selectedItem = dialog._buttonList.ItemContainerGenerator.ItemFromContainer(button);
                }
                if ((selectedItem == null || selectedItem == DependencyProperty.UnsetValue) &&
                    dialog._commandLinkList != null)
                {
                    selectedItem = dialog._commandLinkList.ItemContainerGenerator.ItemFromContainer(button);
                }

                // we only close the dialog if there's a valid selected item
                // since other buttons may appear in the template
                if (selectedItem != null && selectedItem != DependencyProperty.UnsetValue)
                {
                    dialog.SelectedItem = selectedItem;

                    dialog.Close();
                }
            }
        }
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // if no buttons are selected, display the default button
        if (Buttons.Count == 0 && CommandLinks.Count == 0)
        {
            Buttons.Add(new TaskDialogButtonData(DefaultButton));
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        FocusDefaultButton();
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClosedEvent));
    }
    private class TaskDialogWindow : Window
    {
        public bool IsClosable { private get; set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsClosable)
            {
                e.Cancel = true;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // play sound according to the dialog icon
            PlaySound();

            // get this window's handle
            var hwnd = new HandleRef(null, new WindowInteropHelper(this).Handle);

            if (!IsClosable)
            {
                // change the window style to hide the system menu (and the close button with it)
                var style =
                    (NativeMethods.WindowStyles)
                        NativeMethods.GetWindowLong(hwnd, NativeMethods.WindowLongValue.Style);
                _ = NativeMethods.SetWindowLong(hwnd, NativeMethods.WindowLongValue.Style,
                    (IntPtr)(style ^ NativeMethods.WindowStyles.SysMenu));
            }
            else
            {
                // hide icon
                _ = NativeMethods.SendMessage(hwnd, NativeMethods.WindowMessage.SetIcon, (IntPtr)1, IntPtr.Zero);
                _ = NativeMethods.SendMessage(hwnd, NativeMethods.WindowMessage.SetIcon, IntPtr.Zero, IntPtr.Zero);

                // disable irrelevant items from system menu
                NativeMethods.SetSystemMenuItems(hwnd, false, NativeMethods.SystemMenu.Maximize,
                    NativeMethods.SystemMenu.Restore,
                    NativeMethods.SystemMenu.Minimize,
                    NativeMethods.SystemMenu.Size);
            }

            // Change the extended window style to not show a window icon
            var extendedStyle =
                (NativeMethods.WindowExStyles)
                    NativeMethods.GetWindowLong(hwnd, NativeMethods.WindowLongValue.ExtendedStyle);
            _ = NativeMethods.SetWindowLong(hwnd, NativeMethods.WindowLongValue.ExtendedStyle,
                (IntPtr)(extendedStyle | NativeMethods.WindowExStyles.DlgModalFrame));
        }

        private void PlaySound()
        {
            var dialog = (TaskDialog)Content;
            TaskDialogIcon? icon = TaskDialogIconConverter.ConvertTo(dialog.MainIcon);
            string soundName = null;

            switch (icon)
            {
                case TaskDialogIcon.Warning:
                    soundName = "SystemExclamation";
                    break;
                case TaskDialogIcon.Error:
                    soundName = "SystemHand";
                    break;
                case TaskDialogIcon.Information:
                    soundName = "SystemAsterisk";
                    break;
                case TaskDialogIcon.Question:
                    soundName = "SystemQuestion";
                    break;
            }

            if (soundName != null)
            {
                NativeMethods.PlaySound(soundName);
            }
        }
    }
}
