using AutoSellBilet.Dao;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Npgsql;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace AutoSellBilet.Hardik.Connector;
internal class DateBaseConnection : IDisposable
{
    private readonly NpgsqlConnection _connection;

    public DateBaseConnection()
    {
        string connect = "Server=79.174.88.58;Port=16639;Database=Osipenko;User Name=Osipenko;Password=Osipenko123.;";
        _connection = new NpgsqlConnection(connect);
        _connection.Open();
    }

    public NpgsqlConnection GetConnection => _connection;

    public List<KinoDao> GetAllKino()
    {
        var kino = new List<KinoDao>();
        using (var cmd = new NpgsqlCommand("SELECT * FROM public.kino", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                kino.Add(new KinoDao
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Nomer_zala = reader.GetInt32(reader.GetOrdinal("nomer_zala")),
                    vrema = reader.GetDateTime(reader.GetOrdinal("vrema")),

                });
            }
            return kino;
        }
    }

    public SeansDao GetSeansForFilm(string filmName)
    {
        using (var cmd = new NpgsqlCommand(
            "SELECT * FROM public.seans WHERE movie_name = @filmName LIMIT 1",
            _connection))
        {
            cmd.Parameters.AddWithValue("@filmName", filmName);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new SeansDao
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        kino_name = reader.GetString(reader.GetOrdinal("movie_name")),
                        zal_id = reader.GetInt32(reader.GetOrdinal("hall_id")),
                        start_time = reader.GetDateTime(reader.GetOrdinal("start_time")),
                        base_prise = reader.GetDecimal(reader.GetOrdinal("base_price"))
                    };
                }
            }
        }
        return null;
    }

    public List<MestoDao> GetAvailableSeatsForFilm(string filmName)
    {
        var seats = new List<MestoDao>();

        using (var cmd = new NpgsqlCommand("SELECT m.* FROM public.mesta m INNER JOIN public.zals z ON m.zal_id = z.nomer INNER JOIN public.kino k ON k.nomer_zala = z.nomer INNER JOIN public.seans s ON s.movie_name = k.\"Name\" AND s.hall_id = z.nomer LEFT JOIN public.bilets b ON b.mesto_id = m.id AND b.seans_id = s.id WHERE k.\"Name\" = @filmName AND b.id IS NULL", _connection))
    {
        cmd.Parameters.AddWithValue("@filmName", filmName);
        
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                seats.Add(new MestoDao
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    zal_id = reader.GetInt32(reader.GetOrdinal("zal_id")),
                    row_number = reader.GetInt32(reader.GetOrdinal("row_number")),
                    mesto_number = reader.GetInt32(reader.GetOrdinal("mesto_number"))
                });
            }
        }
    }
    
    return seats;
}

    public List<MestoDao> GetSeatsByFilm(string filmName)
    {
        var seats = new List<MestoDao>();

        using (var cmd = new NpgsqlCommand("SELECT m.*,CASE WHEN b.id IS NULL THEN 'свободно' ELSE 'занято' END as status FROM public.mesta m INNER JOIN public.zals z ON m.zal_id = z.nomer INNER JOIN public.kino k ON k.nomer_zala = z.nomer INNER JOIN public.seans s ON s.movie_name = k.\"Name\" AND s.hall_id = z.nomer LEFT JOIN public.bilets b ON b.mesto_id = m.id AND b.seans_id = s.id WHERE k.\"Name\" = @filmName ORDER BY m.row_number, m.mesto_number", _connection))
    {
        cmd.Parameters.AddWithValue("@filmName", filmName);

        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                seats.Add(new MestoDao
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    zal_id = reader.GetInt32(reader.GetOrdinal("zal_id")),
                    row_number = reader.GetInt32(reader.GetOrdinal("row_number")),
                    mesto_number = reader.GetInt32(reader.GetOrdinal("mesto_number")),
                    status = reader.GetString(reader.GetOrdinal("status"))
                });
            }
        }
    }
    
    return seats;
}

    public List<BronDao> GetAllBron()
{
    var bron = new List<BronDao>();
    using (var cmd = new NpgsqlCommand("SELECT * FROM public.bron", _connection))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            bron.Add(new BronDao
            {
                film = reader.GetString(reader.GetOrdinal("film")),
                mesto = reader.GetFloat(reader.GetOrdinal("mesto")),
                guid = reader.GetGuid(reader.GetOrdinal("guid")),

            });
        }
        return bron;
    }
}

public List<UsersDao> GetAllUsers()
{
    var user = new List<UsersDao>();
    using (var cmd = new NpgsqlCommand("SELECT * FROM public.users", _connection))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            user.Add(new UsersDao
            {
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                guid = reader.GetGuid(reader.GetOrdinal("guid")),

            });
        }
        return user;
    }
}

public List<ZalsDao> GetAllZals()
{
    var zals = new List<ZalsDao>();
    using (var cmd = new NpgsqlCommand("SELECT * FROM public.zals", _connection))
    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            zals.Add(new ZalsDao
            {
                Nomer = reader.GetInt32(reader.GetOrdinal("nomer"))

            });
        }
        return zals;
    }
}

    public List<SeansDao> GetAllSeans()
    {
        var seanses = new List<SeansDao>();
        using (var cmd = new NpgsqlCommand("SELECT * FROM public.seans", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                seanses.Add(new SeansDao
                {
                   Id = reader.GetInt32(reader.GetOrdinal("id")),
                   zal_id = reader.GetInt32(reader.GetOrdinal("hall_id")),
                   kino_name = reader.GetString(reader.GetOrdinal("movie_name")),
                   start_time = reader.GetDateTime(reader.GetOrdinal("start_time")),
                   base_prise = reader.GetDecimal(reader.GetOrdinal("base_price"))

                });
            }
            return seanses;
        }
    }

    public bool AddBron(Guid userId, string film, double mesto)
{
    using (var cmd = new NpgsqlCommand(
        "INSERT INTO public.bron (zritel, film, mesto) VALUES (@zritel, @film, @mesto)",
        _connection))
    {
        cmd.Parameters.AddWithValue("@zritel", userId);
        cmd.Parameters.AddWithValue("@film", film);
        cmd.Parameters.AddWithValue("@mesto", mesto);

        return cmd.ExecuteNonQuery() > 0;
    }
}

public bool AddUser(string Name, string Pass)
{
    using (var cmd = new NpgsqlCommand(
        "INSERT INTO public.users (\"Name\", \"Password\") VALUES (@Name, @pass)",
        _connection))
    {
        cmd.Parameters.AddWithValue("@Name", Name);
        cmd.Parameters.AddWithValue("@pass", Pass);

        return cmd.ExecuteNonQuery() > 0;
    }
}

    public int GetSeansIdForFilm(string filmName)
    {
        using (var cmd = new NpgsqlCommand(
            "SELECT id FROM public.seans WHERE movie_name = @filmName LIMIT 1",
            _connection))
        {
            cmd.Parameters.AddWithValue("@filmName", filmName);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public bool AddBilet(int Mesto_id, int Seans, Guid zril, decimal Stoimost)
{
    using (var cmd = new NpgsqlCommand(
        "INSERT INTO public.users (mesto_id, seans_id, zritel_guid, stoimost) VALUES (@mesto, @seans, @zril, @stoimost)",
        _connection))
    {
        cmd.Parameters.AddWithValue("@mesto", Mesto_id);
        cmd.Parameters.AddWithValue("@seans", Seans);
        cmd.Parameters.AddWithValue("@zril", zril);
        cmd.Parameters.AddWithValue("@stoimost", Stoimost);

        return cmd.ExecuteNonQuery() > 0;
    }
}

public void Dispose()
{
    _connection?.Close();
    _connection?.Dispose();
}
}
