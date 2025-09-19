using AutoSellBilet.Dao;
using AutoSellBilet.Dao.Date;
using AutoSellBilet.Hardik.Connector;
using AutoSellBilet.Hardik.Dop;
using AutoSellBilet.Dao.Model;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoSellBilet.Hardik.Date;

namespace AutoSellBilet.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<KinoDao> _films = new();
        private ObservableCollection<ZalsDao> _halls = new();
        private ObservableCollection<User> _users = new();
        private ObservableCollection<MestoDao> _seats = new();
        private ObservableCollection<SeansDao> _seans = new();
        private KinoDao _selectedFilm;
        private User _selectedUser;
        private User _currentUser;
        private List<Mestum> SeansMest;
        private Mestum _selectedSeansMest;
        private MestoDao _selectedSeat;
        private decimal _ticketPrice;
        private int _hallNumber;
        private string _priceInfo;

        public MainViewModel(User currentUser = null)
        {
            CurrentUser = currentUser;
            LoadData();

            this.WhenAnyValue(x => x.SelectedFilm, x => x.SelectedSeat)
                .Subscribe(_ => UpdateTicketInfo());
        }

        public string Greeting => CurrentUser != null
            ? $"Добро пожаловать, {CurrentUser.Name}!"
            : "Система продажи билетов в кино";

        public User CurrentUser
        {
            get => _currentUser;
            set => this.RaiseAndSetIfChanged(ref _currentUser, value);
        }

        public ObservableCollection<KinoDao> Kino
        {
            get => _films;
            set => this.RaiseAndSetIfChanged(ref _films, value);
        }

        public ObservableCollection<User> Users
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

        public MestoDao SelectedSeat
        {
            get => _selectedSeat;
            set => this.RaiseAndSetIfChanged(ref _selectedSeat, value);
        }
        public Mestum SelectedSeatM
        {
            get => _selectedSeansMest;
            set => this.RaiseAndSetIfChanged(ref _selectedSeansMest, value);
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

        public void LoadData() // подгрузка данных
        {
            using (var db = new DateBaseConnection())
            {
                Kino = new ObservableCollection<KinoDao>(db.GetAllKino());
                Users = new ObservableCollection<User>(db.GetAllUsers());
                _halls = new ObservableCollection<ZalsDao>(db.GetAllZals());
                Seats = new ObservableCollection<MestoDao>(db.GetAllMesta());
                _seans = new ObservableCollection<SeansDao>(db.GetAllSeans());


            }
        }

        private void Get()
        {
            using (var db = new dbBileter())
            {
                List<Mestum> mestas = db.Mesta
                    .Include(m=> m.Zal)
                    .Where(m=> m.MestoNumber == SelectedFilm.Nomer_zala && m.Status == "свободно")
                    .ToList();

                SeansMest = mestas;
            }
        }

        private void LoadSeatsForFilm() // подгрузка мест под фильм
        {
            if (SelectedFilm == null)
            {
                Seats.Clear();
                return;
            }

            using (var db = new DateBaseConnection())
            {
                int sessionId = db.GetSeansIdForFilmAndTime(SelectedFilm.Name, SelectedFilm.vrema);

                if (sessionId > 0)
                {
                    var seats = db.GetAvailableSeatsForSession(sessionId);
                    Seats = new ObservableCollection<MestoDao>(seats);
                }
                else
                {
                    Seats.Clear();
                }
            }

            UpdateHallInfo();
        }

        private void UpdateHallInfo()
        {
            if (SelectedFilm != null && SelectedFilm.Nomer_zala.HasValue)
            {
                HallNumber = SelectedFilm.Nomer_zala.Value;
            }
            else
            {
                HallNumber = 0;
            }
        }

        private void UpdateTicketInfo() // строчка с инофой о билете под боксами
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

        public bool BuyTicket() // покубка билетов
        {
            var curentUser = CurrentUser;
           if (SelectedFilm == null || curentUser == null || SelectedSeat == null)
            {
                return false;
            }

           

            using (var db = new DateBaseConnection())
            {
                int seansId = db.GetSeansIdForFilmAndTime(SelectedFilm.Name, SelectedFilm.vrema);
                var bronik = db.GetAllBron();
                var bron = bronik.FirstOrDefault(a=> a.film == SelectedFilm.Name);

                if (seansId <= 0)
                    return false;

                int seansMestaId = db.GetSeansMestaId(seansId, SelectedSeat.Id);

                if (seansMestaId <= 0)
                    return false;



                if (bron != null)
                {
                    db.UpdateSeatStatus(seansMestaId, "занято");
                    db.AddBilet(seansMestaId, CurrentUser.Guid, SelectedFilm.Name, "актуален");
                    LoadData();
                    return true;
                }
                else {
                    db.UpdateSeatStatus(seansMestaId, "занято");
                    db.AddBiletBron(seansMestaId, SelectedFilm.Name, CurrentUser.Guid);
                    db.AddBilet(seansMestaId, CurrentUser.Guid, SelectedFilm.Name, "актуален");
                    LoadData();
                    return true;
                }
            }
        }
        public void Otmena()
        {
            var next = new OtmenaWindow(CurrentUser);

            next.Show();
        }
    }
}