using AutoSellBilet.Hardik.Connector;
using AutoSellBilet.Hardik.Dop;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSellBilet;

public partial class RegistrationWindow : Window
{
    public RegistrationWindow()
    {
        InitializeComponent();
    }

    private async void KnopkaRega_Click(object ob, RoutedEventArgs e)
    {

        string login = UserName.Text;
        string Password = UserPass.Text;

        if (UserName == null && UserPass == null)
        {
            await AllNeded.Message("��� ����� �������������", this);
            return;
        }


        using (var bd = new DateBaseConnection())
        {
            var users = bd.GetAllUsers();
            var user = users.FirstOrDefault(u => u.Name == login && u.Password == Password);

            if (user != null)
            {
                await AllNeded.Message("������������ � ����� ������ ��� ����������", this);
                return;
            }

            bool uspeh = bd.AddUser(login, Password);

            if (uspeh)
            {
                await AllNeded.Message("�������� �����������", this);
                var next = new LoginWindow();
                next.Show();
                this.Close();
                return;
            }
            else
            {
                await AllNeded.Message("������ �����������", this);
                return;
            }
        }
    }

    private void Vozvrat_Click(object o, RoutedEventArgs e)
    {
        var next = new LoginWindow();
        next.Show();
        this.Close();
    }
}