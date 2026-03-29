using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinoCinema.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Внутри класса LoginPage
        public bool Auth(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return false; // Тест не пройдет, если поля пустые
            }

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Id, FullName FROM Users WHERE Login = @login AND Password = @pass", conn);
                    cmd.Parameters.Add("@login", System.Data.SqlDbType.NVarChar, 50).Value = login.Trim();
                    cmd.Parameters.Add("@pass", System.Data.SqlDbType.NVarChar, 255).Value = password.Trim();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        DbConfig.CurrentUserId = (int)reader["Id"];
                        DbConfig.CurrentUserFullName = reader["FullName"].ToString();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка БД: " + ex.Message);
                    return false;
                }
            }
            return false;
        }

        // Измененный обработчик кнопки
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (Auth(LoginField.Text, PasswordField.Password))
            {
                MessageBox.Show($"Добро пожаловать, {DbConfig.CurrentUserFullName}!");
                NavigationService.Navigate(new MainPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }

        // Переход на страницу регистрации
        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        // Кнопка назад
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack(); // Возвращает на предыдущую страницу
        }
    }
}
