using System.ComponentModel;
using System.Runtime.CompilerServices;
using WhiteBit.Desktop.Annotations;

namespace WhiteBit.Desktop;

public class MessageModel:INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private decimal _solUsdt;
    private decimal _btcUsdt;

    public decimal SolUsdt
    {
        get => _solUsdt;
        set
        {
            _solUsdt = value;
            OnPropertyChanged();
        }
    }
    public decimal BtcUsdt
    {
        get => _btcUsdt;
        set
        {
            _btcUsdt = value;
            OnPropertyChanged();
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}