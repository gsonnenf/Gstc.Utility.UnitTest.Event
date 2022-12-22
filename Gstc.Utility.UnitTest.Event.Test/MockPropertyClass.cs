using System.ComponentModel;

namespace Gstc.Utility.UnitTest.Event.Test;

public class MockPropertyClass<T> : INotifyPropertyChanged {

    public event PropertyChangedEventHandler? PropertyChanged;
    private T? _myProperty1;
    private T? _myProperty2;

    public T? MyProperty1 {
        get => _myProperty1;
        set {
            _myProperty1 = value;
            OnPropertyChanged(nameof(MyProperty1));
        }
    }
    public T? MyProperty2 {
        get => _myProperty2;
        set {
            _myProperty2 = value;
            OnPropertyChanged(nameof(MyProperty2));
        }
    }

    public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public int PropertyChangedNumberOfCallbacks => PropertyChanged!.GetInvocationList().Length;
}
