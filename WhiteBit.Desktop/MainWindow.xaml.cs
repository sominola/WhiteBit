using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WhiteBit.WebSocket;
using WhiteBit.WebSocket.Enums;

namespace WhiteBit.Desktop;

public partial class MainWindow : Window
{
    private bool _solListen;
    private bool _btcListen;

    private MessageModel _model;
    private readonly WhiteBitWebSocket _client;

    public MainWindow()
    {
        InitializeComponent();
        _model = new MessageModel
        {
            SolUsdt = 100,
            BtcUsdt = 123
        };
        DataContext = _model;
        _client = new WhiteBitWebSocket();
        _client.StartListen();
    }

    private async void LabelSolClicked(object sender, MouseButtonEventArgs e)
    {
        if (!_solListen)
        {
            await _client.ListenLastPrice(Tickers.SOL_USDT, (value) => _model.SolUsdt = value);
            _solListen = true;
        }
        else
        {
            await _client.UnListenLastPrice(Tickers.SOL_USDT);
            _solListen = false;
        }
    }

    private async void LabelBtcClicked(object sender, MouseButtonEventArgs e)
    {
        if (!_btcListen)
        {
            await _client.ListenLastPrice(Tickers.BTC_USDT, (value) => _model.BtcUsdt = value);
            _btcListen = true;
        }
        else
        {
            await _client.UnListenLastPrice(Tickers.BTC_USDT);
            _btcListen = false;
        }
    }
}