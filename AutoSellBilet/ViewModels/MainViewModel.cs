using AutoSellBilet.Dao;
using AutoSellBilet.Hardik.Connector;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AutoSellBilet.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<KinoDao> _films = new();
        private ObservableCollection<ZalsDao> _halls = new();
        private ObservableCollection<UsersDao> _users = new();
        private ObservableCollection<MestoDao> _seats = new();
        private ObservableCollection<SeansDao> _seans = new();
        private KinoDao _selectedFilm;
        private UsersDao _selectedUser;
        private UsersDao _currentUser;
        private MestoDao _selectedSeat;
        private decimal _ticketPrice;
        private int _hallNumber;
        private string _priceInfo;

        public MainViewModel(UsersDao currentUser = null)
        {
            CurrentUser = currentUser;
            LoadData();

            // Подписываемся на изменения выбранных элементов
            this.WhenAnyValue(x => x.SelectedFilm, x => x.SelectedSeat)
                .Subscribe(_ => UpdateTicketInfo());
        }

        public string Greeting => CurrentUser != null
            ? $"Добро пожаловать, {CurrentUser.Name}!"
            : "Система продажи билетов в кино";

        public UsersDao CurrentUser
        {
            get => _currentUser;
            set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        public ObservableCollection<KinoDao> Kino
        {
            get => _films;
            set => this.RaiseAndSetIfChanged(ref _films, value);
        }

        public ObservableCollection<UsersDao> Users
        {
            get => _users;
            set => this.RaiseAndSetIfChanged(ref _users, value);
        }

        public ObservableCollection<MestoDao> Seats
        {
            get => _seats;
            set => this.RaiseAndSetIfChanged(ref _seats, value);
        }

        public KinoDao SelectedFilm
        {
            get => _selectedFilm;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedFilm, value);
                LoadSeatsForFilm();
            }
        }

        public UsersDao SelectedUser
        {
            get => _selectedUser;
            set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
        }

        public MestoDao SelectedSeat
        {
            get => _selectedSeat;
            set => this.RaiseAndSetIfChanged(ref _selectedSeat, value);
        }

        public decimal TicketPrice
        {
            get => _ticketPrice;
            set => this.RaiseAndSetIfChanged(ref _ticketPrice, value);
        }

        public int HallNumber
        {
            get => _hallNumber;
            set => this.RaiseAndSetIfChanged(ref _hallNumber, value);
        }

        public string PriceInfo
        {
            get => _priceInfo;
            set => this.RaiseAndSetIfChanged(ref _priceInfo, value);
        }

        public void LoadData()
        {
            using (var db = new DateBaseConnection())
            {
                Kino = new ObservableCollection<KinoDao>(db.GetAllKino());
                Users = new ObservableCollection<UsersDao>(db.GetAllUsers());
                _halls = new ObservableCollection<ZalsDao>(db.GetAllZals());
                _seans = new ObservableCollection<SeansDao>(db.GetAllSeans());
            }
        }

        private void LoadSeatsForFilm()
        {
            if (SelectedFilm == null)
            {
                Seats.Clear();
                return;
            }

            using (var db = new DateBaseConnection())
            {
                var seats = db.GetSeatsByFilm(SelectedFilm.Name);
                Seats = new ObservableCollection<MestoDao>(seats);
            }

            UpdateHallInfo();
        }

        private void UpdateHallInfo()
        {
            if (SelectedFilm != null && SelectedFilm.Nomer_zala.HasValue)
            {
                HallNumber = (int)SelectedFilm.Nomer_zala.Value;
            }
            else
            {
                HallNumber = 0;
            }
        }

        private void UpdateTicketInfo()
        {
            if (SelectedFilm != null && SelectedSeat != null)
            {
                using (var db = new DateBaseConnection())
                {
                    var seans = db.GetSeansForFilm(SelectedFilm.Name);
                    if (seans != null)
                    {
                        TicketPrice = seans.base_prise;
                        PriceInfo = $"Стоимость: {TicketPrice} руб. | Зал: {HallNumber} | Ряд: {SelectedSeat.row_number} | Место: {SelectedSeat.mesto_number}";
                    }
                }
            }
            else
            {
                TicketPrice = 0;
                PriceInfo = "Выберите фильм и место для расчета стоимости";
            }
        }

        public bool BuyTicket()
        {
            if (SelectedFilm == null || SelectedUser == null || SelectedSeat == null)
                return false;

            using (var db = new DateBaseConnection())
            {
                int seansId = db.GetSeansIdForFilm(SelectedFilm.Name);

                if (seansId <= 0)
                    return false;

                return db.AddBilet(
                    SelectedSeat.Id,
                    seansId,
                    SelectedUser.guid,
                    TicketPrice
                );
            }
        }
    }
}