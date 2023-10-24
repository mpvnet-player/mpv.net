
using MpvNet.Windows.WPF.ViewModels;

namespace MpvNet.Windows.WPF.Views;

public partial class AboutWindow
{

    public AboutWindow()
    {
        InitializeComponent();
        var vm = new AboutViewModel();
        DataContext = vm;
        vm.CloseAction = Close;
    }
}
