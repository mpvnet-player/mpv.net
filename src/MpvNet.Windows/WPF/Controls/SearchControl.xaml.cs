
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF.Controls;

public partial class SearchControl : UserControl
{
    string? _hintText;
    bool _gotFocus;

    public bool HideClearButton { get; set; }

    public SearchControl() => InitializeComponent();

    public Theme? Theme => Theme.Current;

    public string HintText {
        get => _hintText ??= "";
        set {
            _hintText = value;
            UpdateControls();
        }
    }

    [RelayCommand]
    void Clear()
    {
        Text = "";
        Keyboard.Focus(SearchTextBox);
    }

    void UpdateControls()
    {
        HintTextBlock.Text = string.IsNullOrEmpty(Text) ? HintText : "";

        if (string.IsNullOrEmpty(Text) || HideClearButton || Text.Length > 30)
        {
            SearchTextBox.Padding = new Thickness(2);
            SearchClearButton.Visibility = Visibility.Hidden;
        }
        else
        {
            SearchTextBox.Padding = new Thickness(2, 2, 20, 2);
            SearchClearButton.Visibility = Visibility.Visible;
        }
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
            typeof(SearchControl), new PropertyMetadata(OnCustomerChangedCallBack));

    static void OnCustomerChangedCallBack(
        DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            (sender as SearchControl)?.UpdateControls();

    void SearchTextBox_GotFocus(object sender, RoutedEventArgs e) => _gotFocus = true;

    void SearchTextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_gotFocus)
        {
            SearchTextBox?.SelectAll();
            _gotFocus = false;
        }
    }

    void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && !string.IsNullOrEmpty(Text))
        {
            Text = "";
            e.Handled = true;
        }
    }
}
