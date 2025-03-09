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
    public ObservableCollection<ClientChats> Clients { get; set; } = new ObservableCollection<ClientChats>();

    private ClientChats _selectedClient;

    TcpServer _tcpServer;

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
        _tcpServer = new TcpServer();
        _tcpServer.ClientsUpdated += OnClientsUpdated;
        _tcpServer.ConnectToServerAsync(login, password);
    }

    // обновление списка клиентов и чатов
    private void OnClientsUpdated(List<ClientChats> updatedClients)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // Получаем логины текущих клиентов
            var existingClientLogins = Clients.Select(c => c.Login).ToList();

            // Получаем логины обновленных клиентов
            var updatedClientLogins = updatedClients.Select(c => c.Login).ToList();

            // Находим клиентов, которые больше не в списке
            var removedClientLogins = existingClientLogins.Except(updatedClientLogins).ToList();

            // Удаляем клиентов, которые больше не в списке
            foreach (var login in removedClientLogins)
            {
                var clientToRemove = Clients.FirstOrDefault(c => c.Login == login);
                if (clientToRemove != null)
                {
                    Clients.Remove(clientToRemove);
                }
            }

            // Находим клиентов, которые новые в списке
            var newClientLogins = updatedClientLogins.Except(existingClientLogins).ToList();

            // Добавляем новых клиентов
            foreach (var login in newClientLogins)
            {
                var newClient = updatedClients.FirstOrDefault(c => c.Login == login);
                if (newClient != null)
                {
                    Clients.Add(new ClientChats { Login = newClient.Login, ChatHistory = newClient.ChatHistory });
                }
            }
        });
    }

    // при выборе мышкой другого чата
    private void ClientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedClient = ClientList.SelectedItem as ClientChats;
        if (_selectedClient != null)
        {
            ChatHistory.Text = _selectedClient.ChatHistory;
        }
    }

    // кнопка "Отправить"
    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedClient != null && !string.IsNullOrEmpty(MessageInput.Text))
        {
            string message = MessageInput.Text;
            _selectedClient.ChatHistory += $"Я: {message}\n";
            _tcpServer.Send(_selectedClient.Login, message);
            ChatHistory.Text = _selectedClient.ChatHistory;
            MessageInput.Clear();
        }
    }
}