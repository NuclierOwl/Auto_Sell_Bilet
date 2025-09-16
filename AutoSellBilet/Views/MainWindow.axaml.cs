using AutoSellBilet.Dao;
using AutoSellBilet.Hardik.Model;
using AutoSellBilet.ViewModels;
using Avalonia.Controls;

namespace AutoSellBilet.Views;
public partial class MainWindow : Window
{
    User CurrentUser;
    public MainWindow(User currentUser = null)
    {
        InitializeComponent();
        DataContext = new MainViewModel(currentUser);
        CurrentUser = currentUser;
    }
}