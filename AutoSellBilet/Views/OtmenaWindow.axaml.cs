using AutoSellBilet.Dao;
using AutoSellBilet.Hardik.Connector;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AutoSellBilet.Hardik.Dop;
using Npgsql;
using System.Collections.Generic;
using System.Linq;
using AutoSellBilet.Views;
using AutoSellBilet.Dao.Date;
using AutoSellBilet.Hardik.Model;
using Microsoft.EntityFrameworkCore;

namespace AutoSellBilet;

public partial class OtmenaWindow : Window
{
    private User _currentUser;
    private List<BronDao> _bronList = new List<BronDao>();
    private List<BiletDao> _biletList = new List<BiletDao>();

    public OtmenaWindow(User currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;
        LoadUserData();
        Get();
    }

    private void LoadUserData()
    {
        if (_currentUser == null) return;

        using (var dbConnect = new DateBaseConnection())
        {
            var allBron = dbConnect.GetAllBron();
            var userBron = allBron.Where(b => b.guid == _currentUser.Guid && b.status == "актуален").ToList();

            _bronList.Clear();
            foreach (var bron in userBron)
            {
                _bronList.Add(new BronDao
                {
                    film = bron.film,
                    mesto = bron.mesto,
                    guid = bron.guid
                });
            }

            ComboBron.ItemsSource = _bronList;

            var allBilets = dbConnect.GetBilet();
            var userBilets = allBilets.Where(b => b.zritel_guid == _currentUser.Guid && b.stasus == "актуален").ToList();

            _biletList.Clear();
            foreach (var bilet in userBilets)
            {
                var seansMesta = GetSeansMestaInfo(dbConnect, bilet.seans_mesta_id);
                _biletList.Add(new BiletDao
                {
                    id = bilet.id,
                    seans_mesta_id = bilet.seans_mesta_id,
                    zritel_guid = bilet.zritel_guid,
                    stasus = bilet.stasus
                });
            }

            ComboBilet.ItemsSource = _biletList;
        }
    }

    private void Get()
    {
        using (var bd = new bdTablicki())
        {
            List<Bron> bron = bd.Brons.Include(a => a.ZritelNavigation).ToList();
            bron = bron.Where(a => a.Zritel == _currentUser.Guid && a.Status == "актуален").ToList();
            ComboBron.ItemsSource = bron;

            List<Bilet> bilet = bd.Bilets.Include(a => a.Zritel).ToList();
            bilet = bilet.Where(a => a.ZritelGuid == _currentUser.Guid && a.Status == "актуален").ToList();
            ComboBilet.ItemsSource = bilet;
        }
    }

    private SeansMestoDao GetSeansMestaInfo(DateBaseConnection db, int seansMestaId)
    {
        using (var cmd = new NpgsqlCommand(@"
            SELECT sm.seans_id, sm.mesto_id, s.movie_name, m.row_number, m.mesto_number
            FROM public.seans_mesta sm
            JOIN public.seans s ON sm.seans_id = s.id
            JOIN public.mesta m ON sm.mesto_id = m.id
            WHERE sm.id = @seansMestaId", db.GetConnection))
        {
            cmd.Parameters.AddWithValue("@seansMestaId", seansMestaId);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new SeansMestoDao
                    {
                        SeansId = reader.GetInt32(reader.GetOrdinal("seans_id")),
                        MestoId = reader.GetInt32(reader.GetOrdinal("mesto_id")),
                        FilmName = reader.GetString(reader.GetOrdinal("movie_name")),
                        RowNumber = reader.GetInt32(reader.GetOrdinal("row_number")),
                        SeatNumber = reader.GetInt32(reader.GetOrdinal("mesto_number"))
                    };
                }
            }
        }
        return null;
    }

    private void CancelBron_Click(object sender, RoutedEventArgs e)
    {
        var selectedBron = ComboBron.SelectedItem as BronDao;
        if (selectedBron == null)
        {
            AllNeded.Message("Выберите бронирование для отмены", this);
            return;
        }

        using (var dbConnect = new DateBaseConnection())
        {
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM public.bron WHERE zritel = @guid AND film = @film AND mesto = @mesto",
                dbConnect.GetConnection))
            {
                cmd.Parameters.AddWithValue("@guid", selectedBron.guid);
                cmd.Parameters.AddWithValue("@film", selectedBron.film);
                cmd.Parameters.AddWithValue("@mesto", selectedBron.mesto);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    AllNeded.Message("Бронирование успешно отменено", this);
                    LoadUserData();
                }
                else
                {
                    AllNeded.Message("Ошибка при отмене бронирования", this);
                }
            }
        }
    }

    private void OtmenaBilet_Click(object sender, RoutedEventArgs e)
    {
        var selectedBron = ComboBron.SelectedItem as Bron;
        if (selectedBron == null)
        {
            AllNeded.Message("Выберите билет для возврата", this);
            return;
        }

        using (var dbConnect = new DateBaseConnection())
        {
            using (var cmd = new NpgsqlCommand(
                "DELETE FROM public.bilets WHERE zritel_guid = @id AND seans_mesta_id = @seans_mesta_id;" +
                "DELETE FROM public.bron WHERE mesto = @zritel_guid",
                dbConnect.GetConnection))
            {
                cmd.Parameters.AddWithValue("@id", selectedBron.Zritel);
                cmd.Parameters.AddWithValue("@seans_mesta_id", selectedBron.Mesto);
                cmd.Parameters.AddWithValue("@zritel_guid", selectedBron.Mesto);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(
                "UPDATE public.seans_mesta SET status = 'свободно' WHERE id = @id",
                dbConnect.GetConnection))
            {
                cmd.Parameters.AddWithValue("@id", selectedBron.Mesto);
                cmd.ExecuteNonQuery();
            }

            AllNeded.Message("Билет успешно возвращен", this);
            LoadUserData();
            Get();
        }
    }


    private void Nazad_Click(object ob, RoutedEventArgs e)
    {
        this.Close();
    }

}