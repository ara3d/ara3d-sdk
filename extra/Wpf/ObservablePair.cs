using System.ComponentModel;

namespace Ara3D.Utils.Wpf
{
    // TODO: this needs to be placed in general purpose utilities class somewhere
    public class ObservablePair<T1, T2> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private T1 _item1;
        public T1 Item1
        {
            get => _item1;
            set => OnPropertyChanged(nameof(Item1), _item1?.Equals(value) != true, _item1 = value);
        }

        private T2 _item2;
        public T2 Item2
        {
            get => _item2;
            set => OnPropertyChanged(nameof(Item2), _item2.Equals(value) != true, _item2 = value);
        }

        public void OnPropertyChanged<T>(string propertyName, bool notify, T _)
        {
            if (notify)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
