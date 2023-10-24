
using CommunityToolkit.Mvvm.ComponentModel;

using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF.ViewModels;

public class ViewModelBase : ObservableObject
{
    public Theme Theme => Theme.Current!;
}
