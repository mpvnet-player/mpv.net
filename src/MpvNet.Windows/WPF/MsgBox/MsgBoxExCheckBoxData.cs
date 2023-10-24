
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MpvNet.Windows.WPF.MsgBox;

public class MsgBoxExCheckBoxData : INotifyPropertyChanged
{
    private bool isModified = false;

    public bool IsModified {
        get => isModified;
        set {
            if (value != isModified)
            {
                isModified = true;
                NotifyPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            if (propertyName != "IsModified")
                IsModified = true;
        }
    }

    private string? checkBoxText;
    private bool checkBoxIsChecked;

    public string? CheckBoxText {
        get => checkBoxText;
        set {
            if (value != checkBoxText)
                checkBoxText = value; NotifyPropertyChanged();
        }
    }

    public bool CheckBoxIsChecked {
        get => checkBoxIsChecked;
        set {
            if (value != checkBoxIsChecked)
            {
                checkBoxIsChecked = value;
                NotifyPropertyChanged();
            }
        }
    }
}
