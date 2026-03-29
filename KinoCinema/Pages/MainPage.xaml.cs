using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace KinoCinema.Pages
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            // Сначала подписываемся на события, потом грузим
            LoadMovies();
        }

        private void DoFilter()
        {
            // Эта проверка ВАЖНА, чтобы не было зависания при запуске
            if (!IsLoaded || SortCombo == null || SearchBox == null) return;

            string search = SearchBox.Text;
            string sort = (SortCombo.SelectedItem as ComboBoxItem)?.Content.ToString();
            LoadMovies(search, sort);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => DoFilter();
        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => DoFilter();

        public void LoadMovies(string search = "", string sort = "По названию")
        {
            var list = new List<Movie>();
            string orderBy = (sort == "По рейтингу") ? "Rating DESC" : "Title ASC";

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string sql = $@"SELECT M.*, G.GenreName FROM Movies M 
                                    JOIN Genres G ON M.GenreId = G.Id 
                                    WHERE M.Title LIKE @search 
                                    ORDER BY {orderBy}";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string imgFromDb = reader["ImagePath"].ToString();
                            list.Add(new Movie
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"].ToString(),
                                // УПРОЩАЕМ ПУТЬ: WPF сам найдет файл в папке Images, если она в ресурсах или в bin
                                ImagePath = "/Images/" + imgFromDb,
                                Genre = reader["GenreName"].ToString(),
                                Rating = reader["Rating"] != DBNull.Value ?
                                         Convert.ToDouble(reader["Rating"]).ToString("F1") : "0.0"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Если ошибка в SQL, мы её увидим
                    System.Diagnostics.Debug.WriteLine("Ошибка: " + ex.Message);
                }
            }
            MoviesList.ItemsSource = list;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            if (DbConfig.CurrentUserId != null)
                NavigationService.Navigate(new ProfilePage());
            else
                NavigationService.Navigate(new LoginPage());
        }

        private void MovieClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is Movie movie)
                NavigationService.Navigate(new MovieDetailsPage(movie));
        }
    }
}