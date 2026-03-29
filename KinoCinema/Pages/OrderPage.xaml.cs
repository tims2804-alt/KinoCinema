using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KinoCinema.Pages
{
    public partial class OrderPage : Page
    {
        private List<Seat> _selectedSeats;
        private int _sessionId;

        public OrderPage(List<Seat> seats, int sessionId, string movieTitle, string time)
        {
            InitializeComponent();
            _selectedSeats = seats;
            _sessionId = sessionId;

            // Заполняем поля из переданных данных
            TxtMovie.Text = movieTitle;
            TxtSession.Text = $"Сеанс: {time}";
            TxtSeats.Text = "Места: " + string.Join(", ", seats.Select(s => $"Ряд {s.Row}, Место {s.Number}"));
            TxtTotal.Text = $"{seats.Sum(s => s.Price)} руб.";
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (DbConfig.CurrentUserId == null)
            {
                MessageBox.Show("Ошибка: Пользователь не авторизован!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                conn.Open();
                // Используем транзакцию, чтобы или купились все билеты, или ни одного (в случае сбоя)
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (var seat in _selectedSeats)
                    {
                        string sql = @"INSERT INTO Tickets (SessionId, SeatId, UserId, StatusId, FinalPrice, PurchaseDate) 
                                       VALUES (@sid, @seatid, @uid, 2, @price, GETDATE())";

                        SqlCommand cmd = new SqlCommand(sql, conn, transaction);
                        cmd.Parameters.AddWithValue("@sid", _sessionId);
                        cmd.Parameters.AddWithValue("@seatid", seat.Id);
                        cmd.Parameters.AddWithValue("@uid", DbConfig.CurrentUserId);
                        cmd.Parameters.AddWithValue("@price", seat.Price);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Билеты успешно оформлены! Приятного просмотра.");

                    // По ТЗ: возврат на главную страницу
                    NavigationService.Navigate(new MainPage());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Ошибка при сохранении билетов: " + ex.Message);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}