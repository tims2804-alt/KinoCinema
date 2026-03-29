using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KinoCinema.Pages
{
    public partial class HallPage : Page
    {
        private int _sessionId;
        private List<Seat> _selectedSeats = new List<Seat>();
        private decimal _basePrice = 0;

        public HallPage(int sessionId, string movieTitle, string time)
        {
            InitializeComponent();
            _sessionId = sessionId;
            // Убедись, что в XAML есть TextBlock с именами HallTitle и SessionTime
            // Если их нет в твоем упрощенном коде, просто удали эти две строки
            // HallTitle.Text = movieTitle; 
            // SessionTime.Text = time;

            LoadSeats();
        }

        private void LoadSeats()
        {
            var seats = new List<Seat>();

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();

                    // Получаем базовую цену сеанса
                    SqlCommand priceCmd = new SqlCommand("SELECT BasePrice FROM Sessions WHERE Id = @sid", conn);
                    priceCmd.Parameters.AddWithValue("@sid", _sessionId);
                    var result = priceCmd.ExecuteScalar();
                    _basePrice = result != DBNull.Value ? (decimal)result : 0;

                    // ЗАПРОС: Берем все места зала и проверяем через LEFT JOIN, не куплены ли они
                    string sql = @"
                        SELECT s.Id, s.SeatNumber, s.RowNumber, st.PriceModifier,
                        CASE WHEN t.Id IS NULL THEN 1 ELSE 0 END as IsFree
                        FROM Seats s
                        JOIN Sessions sess ON s.HallId = sess.HallId
                        JOIN SeatTypes st ON s.SeatTypeId = st.Id
                        LEFT JOIN Tickets t ON s.Id = t.SeatId AND t.SessionId = sess.Id
                        WHERE sess.Id = @sid
                        ORDER BY s.RowNumber, s.SeatNumber";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sid", _sessionId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        seats.Add(new Seat
                        {
                            Id = (int)reader["Id"],
                            Number = (int)reader["SeatNumber"],
                            Row = (int)reader["RowNumber"],
                            Price = _basePrice * (decimal)reader["PriceModifier"],
                            IsAvailable = Convert.ToInt32(reader["IsFree"]) == 1
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            SeatsGrid.ItemsSource = seats;
        }

        private void Seat_Checked(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Seat seat)
            {
                _selectedSeats.Add(seat);
                UpdateTotal();
            }
        }

        private void Seat_Unchecked(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is Seat seat)
            {
                _selectedSeats.RemoveAll(s => s.Id == seat.Id);
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            // Считаем сумму всех выбранных мест
            decimal total = _selectedSeats.Sum(s => s.Price);

            // Обновляем текст в XAML. 
            // Имя TotalPriceText должно совпадать с тем, что в твоем <TextBlock x:Name="..." />
            if (TotalPriceText != null)
            {
                TotalPriceText.Text = $"Итого: {total} руб.";
            }
        }

        private void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSeats.Count == 0 || DbConfig.CurrentUserId == null)return;

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                foreach (var seat in _selectedSeats)
                {
                    string sql = "INSERT INTO Tickets (SessionId, SeatId, UserId, StatusId, FinalPrice) VALUES (@sid, @seatid, @uid, 1, @price)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sid", _sessionId);
                    cmd.Parameters.AddWithValue("@seatid", seat.Id);
                    cmd.Parameters.AddWithValue("@uid", DbConfig.CurrentUserId);
                    cmd.Parameters.AddWithValue("@price", seat.Price);
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Билеты куплены!");
            NavigationService.Navigate(new ProfilePage());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся на главную страницу к списку фильмов
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new MainPage());
            }
        }
    }
}