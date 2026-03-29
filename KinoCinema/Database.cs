using System;
using System.Collections.Generic;

namespace KinoCinema
{
    // Модель фильма (соответствует твоей таблице Movies)
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public string AgeRating { get; set; }
        public string ImagePath { get; set; }
        public string Rating { get; set; } // Для вывода рейтинга
    }

    // Модель сеанса
    public class MovieSession
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string HallName { get; set; }
        public decimal Price { get; set; }
    }

    // Глобальные настройки
    public static class DbConfig
    {
        // СТРОКА ПОДКЛЮЧЕНИЯ (Проверь имя сервера!)
        public static string ConnectionString = @"Data Source=DESKTOP-TL8I7LP\SQLEXPRESS;Initial Catalog=CinemaDB;Integrated Security=True;TrustServerCertificate=True";
        // Хранение текущего пользователя
        public static int? CurrentUserId = null;
        public static string CurrentUserFullName = "";
        public static bool IsAdmin = false;
    }
    public class Seat
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Row { get; set; }
        public decimal Price { get; set; }
        // Если IsAvailable = false, кнопка станет КРАСНОЙ (через триггер IsEnabled)
        public bool IsAvailable { get; set; }
        // Нужно для того, чтобы WPF понимал, что место выбрано
        public bool IsSelected { get; set; }
    }

    public class SessionInfo
    {
        public int Id { get; set; }
        public string Time { get; set; }

        // Переопределяем ToString, чтобы в списках по умолчанию 
        // отображалось время, если мы забудем про Binding
        public override string ToString() => Time;
    }
    public class TicketView
    {
        public string MovieTitle { get; set; } // Название фильма
        public string SessionTime { get; set; } // Время сеанса
        public string SeatInfo { get; set; }   // Ряд и место
        public decimal Price { get; set; }      // Цена
    }

}