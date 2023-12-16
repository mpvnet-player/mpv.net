
using System.Windows.Controls;
using System.Windows;

namespace MpvNet.Windows.WPF;

public class ComboBoxTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        ContentPresenter presenter = (ContentPresenter)container;

        if (presenter.TemplatedParent is ComboBox)
            return (DataTemplate)presenter.FindResource("ComboBoxCollapsedDataTemplate");
        else // Templated parent is ComboBoxItem
            return (DataTemplate)presenter.FindResource("ComboBoxExpandedDataTemplate");
    }
}
