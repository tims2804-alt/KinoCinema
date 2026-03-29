using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KinoCinema.Pages
{
    public partial class ProfilePage : Page
    {
        // Путь к файлу для оффлайн-режима
        private string cachePath = "tickets_cache.txt";

        public ProfilePage()
        {
            InitializeComponent();
            LoadUserTickets();
        }

        private void LoadUserTickets()
        {
            var ticketsList = new List<TicketView>();

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    // Запрос тянет: Название фильма, Время, Ряд, Место и Цену
                    string sql = @"
                SELECT M.Title, S.StartTime, St.RowNumber, St.SeatNumber, T.FinalPrice
                FROM Tickets T
                JOIN Sessions S ON T.SessionId = S.Id
                JOIN Movies M ON S.MovieId = M.Id
                JOIN Seats St ON T.SeatId = St.Id
                WHERE T.UserId = @uid";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@uid", DbConfig.CurrentUserId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ticketsList.Add(new TicketView
                        {
                            MovieTitle = reader["Title"].ToString(),
                            SessionTime = Convert.ToDateTime(reader["StartTime"]).ToString("dd.MM HH:mm"),
                            SeatInfo = $"Ряд {reader["RowNumber"]}, Место {reader["SeatNumber"]}",
                            Price = Convert.ToDecimal(reader["FinalPrice"])
                        });
                    }

                    UserTicketsList.ItemsSource = ticketsList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки билетов: " + ex.Message);
                }
            }
        }

        private void LoadFromCache()
        {
            if (File.Exists(cachePath))
            {
                var cachedTickets = new List<object>();
                var lines = File.ReadAllLines(cachePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        cachedTickets.Add(new
                        {
                            MovieTitle = parts[0],
                            SessionTime = parts[1],
                            SeatInfo = parts[2],
                            Price = parts[3]
                        });
                    }
                }
                UserTicketsList.ItemsSource = cachedTickets;
            }
        }
        // Добавь это внутрь класса ProfilePage
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            
                // Если назад идти некуда (например, открыли профиль сразу), 
                // просто переходим на главную
               NavigationService.Navigate(new MainPage());
            
        }
    }
}