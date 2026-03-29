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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверяем, что обязательные поля не пустые
            if (string.IsNullOrWhiteSpace(RegLoginField.Text) || string.IsNullOrWhiteSpace(RegPasswordField.Password))
            {
                MessageBox.Show("Заполните логин и пароль!");
                return;
            }

            // 2. Сравниваем пароли
            if (RegPasswordField.Password != ConfirmPasswordField.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // 3. Работа с базой данных
            using (SqlConnection conn = new SqlConnection(DbConfig.ConnectionString))
            {
                try
                {
                    conn.Open();

                    // Сначала проверим, нет ли уже такого логина в базе
                    string checkSql = "SELECT COUNT(*) FROM Users WHERE Login = @login";
                    SqlCommand checkCmd = new SqlCommand(checkSql, conn);
                    checkCmd.Parameters.AddWithValue("@login", RegLoginField.Text.Trim());

                    int userExists = (int)checkCmd.ExecuteScalar();
                    if (userExists > 0)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует!");
                        return;
                    }

                    // 4. Пишем SQL-запрос на вставку
                    // Обрати внимание: RoleId ставим 2 (обычный клиент)
                    string sql = @"INSERT INTO Users (Login, Password, FullName, Email, RoleId) 
                                   VALUES (@login, @pass, @name, @email, 2)";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@login", RegLoginField.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", RegPasswordField.Password.Trim());

                    // Если у тебя на форме есть поля для имени и почты, укажи их. 
                    // Если нет — передаем пустые строки или null.
                    cmd.Parameters.AddWithValue("@name", RegLoginField.Text.Trim()); // Временно пишем логин как имя
                    cmd.Parameters.AddWithValue("@email", "example@mail.ru"); // Заглушка, если нет поля на форме

                    // 5. Выполняем запись в базу
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Регистрация прошла успешно!");
                        NavigationService.Navigate(new LoginPage());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка БД: " + ex.Message);
                }
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