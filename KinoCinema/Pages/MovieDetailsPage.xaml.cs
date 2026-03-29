using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KinoCinema.Pages
{
    public partial class MovieDetailsPage : Page
    {
        private Movie _selectedMovie;

        public MovieDetailsPage(Movie selectedMovie)
        {
            InitializeComponent();
            _selectedMovie = selectedMovie;

            // Заполняем данные
            MovieTitle.Text = _selectedMovie.Title;
            MovieGenre.Text = _selectedMovie.Genre;
            MovieDescription.Text = _selectedMovie.Description;

            // Загрузка картинки
            if (!string.IsNullOrEmpty(_selectedMovie.ImagePath))
            {
                try
                {
                    MovieImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_selectedMovie.ImagePath, UriKind.RelativeOrAbsolute));
                }
                catch { }
            }

            // Запускаем загрузку сеансов
            LoadSessionsFromDb();
        }

        private void LoadSessionsFromDb()
        {
            var sessions = new List<SessionInfo>();

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    // Запрос: берем время начала и ID сеанса
                    string sql = @"SELECT Id, StartTime FROM Sessions 
                                   WHERE MovieId = @mid AND StartTime > GETDATE()";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@mid", _selectedMovie.Id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        sessions.Add(new SessionInfo
                        {
                            Id = (int)reader["Id"],
                            Time = Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm")
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки сеансов: " + ex.Message);
                }
            }

            // Если сеансов нет — выведем заглушку
            if (sessions.Count == 0)
            {
                // Можно добавить TextBlock для надписи "Сеансов нет"
            }

            SessionsList.ItemsSource = sessions;
        }

        private void Session_Click(object sender, RoutedEventArgs e)
        {
            // Получаем данные о сеансе из нажатой кнопки
            var session = (sender as Button).DataContext as SessionInfo;

            if (DbConfig.CurrentUserId == null)
            {
                MessageBox.Show("Пожалуйста, войдите в систему!");
                NavigationService.Navigate(new LoginPage());
                return;
            }

            if (session != null)
            {
                // Переходим в зал, передавая ID сеанса, название фильма и время
                NavigationService.Navigate(new HallPage(session.Id, _selectedMovie.Title, session.Time));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}