using AutoSellBilet.Dao;
using AutoSellBilet.Hardik.Model;
using Npgsql;
using System;
using System.Collections.Generic;

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
        using (var cmd = new NpgsqlCommand("SELECT \"Name\", nomer_zala, vrema FROM public.kino", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                kino.Add(new KinoDao
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Nomer_zala = reader.GetInt32(reader.GetOrdinal("nomer_zala")),
                    vrema = reader.GetDateTime(reader.GetOrdinal("vrema"))
                });
            }
        }
        return kino;
    }

    public List<BiletDao> GetBilet()
    {
        var bilet = new List<BiletDao>();
        using (var cmd = new NpgsqlCommand("SELECT * FROM public.bilets", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                bilet.Add(new BiletDao
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    seans_mesta_id = reader.GetInt32(reader.GetOrdinal("seans_mesta_id")),
                    zritel_guid = reader.GetGuid(reader.GetOrdinal("zritel_guid")),
                    stasus = reader.GetString(reader.GetOrdinal("status")),
                });
            }
        }
        return bilet;
    }

    public SeansDao GetSeansForFilm(string filmName)
    {
        using (var cmd = new NpgsqlCommand(
            "SELECT id, movie_name, hall_id, start_time, base_price FROM public.seans WHERE movie_name = @filmName LIMIT 1",
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

    public List<MestoDao> GetAvailableSeatsForSession(int sessionId)
    {
        var seats = new List<MestoDao>();
        using (var cmd = new NpgsqlCommand(@"
            SELECT m.id, m.zal_id, m.row_number, m.mesto_number 
            FROM public.mesta m 
            JOIN public.seans_mesta sm ON m.id = sm.mesto_id 
            WHERE sm.seans_id = @sessionId AND sm.status = 'свободно'",
            _connection))
        {
            cmd.Parameters.AddWithValue("@sessionId", sessionId);

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

    public List<SeatStatusDao> GetSeatsBySession(int sessionId)
    {
        var seats = new List<SeatStatusDao>();
        using (var cmd = new NpgsqlCommand("SELECT m.row_number, m.mesto_number, sm.status, sm.price, u.\"Name\" as user_name FROM public.seans_mesta sm JOIN public.mesta m ON sm.mesto_id = m.id LEFT JOIN public.bilets b ON sm.id = b.seans_mesta_id LEFT JOIN public.users u ON b.zritel_guid = u.guid WHERE sm.seans_id = @sessionId ORDER BY m.row_number, m.mesto_number",
            _connection))
        {
            cmd.Parameters.AddWithValue("@sessionId", sessionId);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    seats.Add(new SeatStatusDao
                    {
                        RowNumber = reader.GetInt32(reader.GetOrdinal("row_number")),
                        SeatNumber = reader.GetInt32(reader.GetOrdinal("mesto_number")),
                        status = reader.GetString(reader.GetOrdinal("status")),
                        price = reader.GetDecimal(reader.GetOrdinal("price")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("user_name")) ? null : reader.GetString(reader.GetOrdinal("user_name"))
                    }
                    );
                }
            }


            return seats;
        }
    }

    public List<BronDao> GetAllBron()
    {
        var bron = new List<BronDao>();
        using (var cmd = new NpgsqlCommand("SELECT zritel, film, mesto, status FROM public.bron", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                bron.Add(new BronDao
                {
                    guid = reader.GetGuid(reader.GetOrdinal("zritel")),
                    film = reader.GetString(reader.GetOrdinal("film")),
                    mesto = reader.GetDouble(reader.GetOrdinal("mesto")),
                    status = reader.GetString(reader.GetOrdinal("status"))
                });
            }
        }
        return bron;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        using (var cmd = new NpgsqlCommand("SELECT \"Name\", \"Password\", guid FROM public.users", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                users.Add(new User
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    Guid = reader.GetGuid(reader.GetOrdinal("guid"))
                });
            }
        }
        return users;
    }

    public List<ZalsDao> GetAllZals()
    {
        var zals = new List<ZalsDao>();
        using (var cmd = new NpgsqlCommand("SELECT nomer, maxmesto FROM public.zals", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                zals.Add(new ZalsDao
                {
                    Nomer = reader.GetInt32(reader.GetOrdinal("nomer")),
                    MaxMesto = reader.IsDBNull(reader.GetOrdinal("maxmesto")) ? 0 : reader.GetInt32(reader.GetOrdinal("maxmesto"))
                });
            }
        }
        return zals;
    }
    
    public List<MestoDao> GetAllMesta()
    {
        var mesto = new List<MestoDao>();
        using (var cmd = new NpgsqlCommand("SELECT * FROM public.mesta", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                mesto.Add(new MestoDao
                {
                    Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                    //Id = reader.GetInt32(reader.GetOrdinal("id")),
                    mesto_number = reader.GetInt32(reader.GetOrdinal("mesto_number")),
                    row_number = reader.GetInt32(reader.GetOrdinal("row_number")),
                    zal_id = reader.GetInt32(reader.GetOrdinal("zal_id")),
                    status = reader.IsDBNull(reader.GetOrdinal("status")) ? "Свободен" : reader.GetString(reader.GetOrdinal("status"))
                });
            }
        }
        return mesto;
    }

    public List<SeansDao> GetAllSeans()
    {
        var seanses = new List<SeansDao>();
        using (var cmd = new NpgsqlCommand("SELECT id, movie_name, hall_id, start_time, base_price FROM public.seans", _connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                seanses.Add(new SeansDao
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    kino_name = reader.GetString(reader.GetOrdinal("movie_name")),
                    zal_id = reader.GetInt32(reader.GetOrdinal("hall_id")),
                    start_time = reader.GetDateTime(reader.GetOrdinal("start_time")),
                    base_prise = reader.GetDecimal(reader.GetOrdinal("base_price"))
                });
            }
        }
        return seanses;
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

    public bool AddUser(string name, string password)
    {
        using (var cmd = new NpgsqlCommand(
            "INSERT INTO public.users (\"Name\", \"Password\") VALUES (@name, @password)",
            _connection))
        {
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@password", password);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public int GetSeansIdForFilmAndTime(string filmName, DateTime startTime)
    {
        using (var cmd = new NpgsqlCommand(
            "SELECT id FROM public.seans WHERE movie_name = @filmName AND start_time = @startTime LIMIT 1",
            _connection))
        {
            cmd.Parameters.AddWithValue("@filmName", filmName);
            cmd.Parameters.AddWithValue("@startTime", startTime);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public List<BronDao> GetBronByUser(Guid userGuid)
    {
        var bron = new List<BronDao>();
        using (var cmd = new NpgsqlCommand(
            "SELECT zritel, film, mesto FROM public.bron WHERE zritel = @guid",
            _connection))
        {
            cmd.Parameters.AddWithValue("@guid", userGuid);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    bron.Add(new BronDao
                    {
                        guid = reader.GetGuid(reader.GetOrdinal("zritel")),
                        film = reader.GetString(reader.GetOrdinal("film")),
                        mesto = reader.GetDouble(reader.GetOrdinal("mesto"))
                    });
                }
            }
        }
        return bron;
    }

    public List<BiletDao> GetBiletByUser(Guid userGuid)
    {
        var bilet = new List<BiletDao>();
        using (var cmd = new NpgsqlCommand(
            "SELECT id, seans_mesta_id, zritel_guid, status FROM public.bilets WHERE zritel_guid = @guid",
            _connection))
        {
            cmd.Parameters.AddWithValue("@guid", userGuid);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    bilet.Add(new BiletDao
                    {
                        id = reader.GetInt32(reader.GetOrdinal("id")),
                        seans_mesta_id = reader.GetInt32(reader.GetOrdinal("seans_mesta_id")),
                        zritel_guid = reader.GetGuid(reader.GetOrdinal("zritel_guid")),
                        stasus = reader.GetString(reader.GetOrdinal("status"))
                    });
                }
            }
        }
        return bilet;
    }

    public bool AddBiletBron(int seansMestaId, string film, Guid zritelGuid, string status = "актуален")
    {
        decimal price = 0;
        using (var priceCmd = new NpgsqlCommand(
            "SELECT price FROM public.seans_mesta WHERE id = @id", _connection))
        {
            priceCmd.Parameters.AddWithValue("@id", seansMestaId);
            var result = priceCmd.ExecuteScalar();
            if (result != null) price = Convert.ToDecimal(result);
        }

        using (var cmd = new NpgsqlCommand(@"
            INSERT INTO public.bron (mesto, film , zritel, status, prise) 
            VALUES (@seansMestaId, @filmName, @zritelGuid, @status, @prise)",
            _connection))
        {
            cmd.Parameters.AddWithValue("@seansMestaId", seansMestaId);
            cmd.Parameters.AddWithValue("@zritelGuid", zritelGuid);
            cmd.Parameters.AddWithValue("@filmName", film);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@prise", price);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool AddBilet(int seansMestaId, Guid zritelGuid, string status = "актуален")
    {
        using (var cmd = new NpgsqlCommand(@"
        INSERT INTO public.bilets (seans_mesta_id, zritel_guid, status) 
        VALUES (@seansMestaId, @zritelGuid, @status)",
            _connection))
        {
            cmd.Parameters.AddWithValue("@seansMestaId", seansMestaId);
            cmd.Parameters.AddWithValue("@zritelGuid", zritelGuid);
            cmd.Parameters.AddWithValue("@status", status);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public int GetSeansMestaId(int sessionId, int seatId)
    {
        using (var cmd = new NpgsqlCommand(
            "SELECT id FROM public.seans_mesta WHERE seans_id = @sessionId AND mesto_id = @seatId LIMIT 1",
            _connection))
        {
            cmd.Parameters.AddWithValue("@sessionId", sessionId);
            cmd.Parameters.AddWithValue("@seatId", seatId);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }

    public bool UpdateSeatStatus(int seansMestaId, string status)
    {
        using (var cmd = new NpgsqlCommand(
            "UPDATE public.seans_mesta SET status = @status WHERE id = @id",
            _connection))
        {
            cmd.Parameters.AddWithValue("@id", seansMestaId);
            cmd.Parameters.AddWithValue("@status", status);

            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }

}