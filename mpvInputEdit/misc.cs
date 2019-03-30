using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace mpvInputEdit
{
    public class InputItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Menu { get; set; } = "";
        public string Command { get; set; } = "";

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Key = "";

        public string Key {
            get {
                return _Key;
            }
            set {
                _Key = value;
                NotifyPropertyChanged();
            }
        }
    }
}