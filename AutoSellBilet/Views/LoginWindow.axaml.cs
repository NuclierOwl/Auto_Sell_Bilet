using AutoSellBilet.Hardik.Connector;
using AutoSellBilet.Hardik.Dop;
using AutoSellBilet.Views;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;

namespace AutoSellBilet;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void KnopkaWhoda_Click(object sender, RoutedEventArgs e)
    {
        string login = UserName.Text;
        string Password = UserPass.Text;

        using (var dbConnection = new DateBaseConnection())
        {
            var users = dbConnection.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Name == login && u.Password == Password);
            var next = new MainWindow(user);

            if (user != null)
            {
                next.Show();
                this.Close();
            }
            else
            {
                AllNeded.Worning("¬веди логин и пароль", this);
            }

        }
    }

    private void KnopkaRega_Click(object ob, RoutedEventArgs e)
    {
        var next = new RegistrationWindow();
        next.Show();
        this.Close();
    }
}