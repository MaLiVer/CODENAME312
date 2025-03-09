using Dark.Net;
using System.Collections.ObjectModel;
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
    public ObservableCollection<string> Clients { get; set; } = new ObservableCollection<string>();

    public MainWindow()
    {
        // подключаем темную тему из NuGet - DarkNet
        DarkNet.Instance.SetWindowThemeWpf(this, Theme.Dark);

        // штатные строки
        InitializeComponent();
        ClientList.ItemsSource = Clients;
        SendButton.Click += SendButton_Click;

        // параметры командрой строки для теста
        string login = App.CommandLineArgs[0];
        string password = App.CommandLineArgs[1];

        // создаем класс server
        TcpServer tcpServer = new TcpServer();
        tcpServer.ClientsUpdated += OnClientsUpdated;
        tcpServer.ConnectToServerAsync(login, password);
    }

    private void OnClientsUpdated(List<string> clients)
    {
        // Обновляем список клиентов в UI через Dispatcher
        Application.Current.Dispatcher.Invoke(() =>
        {
            Clients.Clear();
            foreach (var client in clients)
            {
                Clients.Add(client);
            }
        });
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