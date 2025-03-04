using Dark.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TcpClient _client;
    private NetworkStream _stream;

    public MainWindow()
    {
        // подключаем темную тему из NuGet - DarkNet
        DarkNet.Instance.SetWindowThemeWpf(this, Theme.Dark);

        // штатные строки
        InitializeComponent();
        SendButton.Click += SendButton_Click;

        // параметры командрой строки для теста
        string login = App.CommandLineArgs[0];
        string password = App.CommandLineArgs[1];

        // создаем класс server
        TcpServer tcpServer = new TcpServer();
        tcpServer.ConnectToServerAsync(login, password);
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        string message = MessageInput.Text;
        if (!string.IsNullOrEmpty(message))
        {
            ChatHistory.Text += $"You: {message}\n";
            MessageInput.Clear();
        }
    }
}