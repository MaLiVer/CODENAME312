using System.Configuration;
using System.Data;
using System.Windows;

namespace TestWPF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string[] CommandLineArgs { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (e.Args.Length < 2)
        {
            CommandLineArgs = new string[] { "user1", "password1" };
        }
        else
        {
            CommandLineArgs = e.Args;
        }

        base.OnStartup(e);
    }
}

