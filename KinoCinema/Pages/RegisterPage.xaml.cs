using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient; // Проверь, что этот пакет установлен

namespace KinoCinema.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        // Внутри класса RegisterPage
        public bool RegisterUser(string login, string pass, string confirmPass)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass)) return false;
            if (pass != confirmPass) return false;

            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();
                    // Проверка существования
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Login = @login", conn);
                    checkCmd.Parameters.AddWithValue("@login", login.Trim());
                    if ((int)checkCmd.ExecuteScalar() > 0) return false;

                    // Вставка
                    string sql = @"INSERT INTO Users (Login, Password, FullName, Email, RoleId) VALUES (@login, @pass, @name, @email, 2)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@login", login.Trim());
                    cmd.Parameters.AddWithValue("@pass", pass.Trim());
                    cmd.Parameters.AddWithValue("@name", login.Trim());
                    cmd.Parameters.AddWithValue("@email", "test@test.ru");

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (RegisterUser(RegLoginField.Text, RegPasswordField.Password, ConfirmPasswordField.Password))
            {
                MessageBox.Show("Регистрация прошла успешно!");
                NavigationService.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Ошибка регистрации (проверьте данные или логин занят)");
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new LoginPage());
        }
    }
}