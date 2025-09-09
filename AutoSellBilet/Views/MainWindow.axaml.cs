using AutoSellBilet.Dao;
using AutoSellBilet.ViewModels;
using Avalonia.Controls;

namespace AutoSellBilet.Views;
    public partial class MainWindow : Window
    {
        public MainWindow(UsersDao currentUser = null)
        {
            InitializeComponent();
            DataContext = new MainViewModel(currentUser);
        }
    }