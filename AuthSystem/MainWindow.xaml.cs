using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class MainWindow : Window
    {
        private List<User> users;

        public MainWindow()
        {
            InitializeComponent();
            users = AuthManager.LoadUsers(); // Завантажуємо користувачів з `users.txt`
        }


        private int failedAttempts = 0;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var users = AuthManager.LoadUsers();
            MessageBox.Show($"Знайдено {users.Count} користувачів");

            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                MessageBox.Show("Користувача не знайдено!");
                return;
            }

            if (user.IsBlocked)
            {
                MessageBox.Show("Цей користувач заблокований!");
                return;
            }

            if (string.IsNullOrEmpty(user.PasswordHash)) // Якщо пароля немає, не враховуємо це як невдалу спробу
            {
                MessageBox.Show("Це ваш перший вхід! Задайте новий пароль.");
                PasswordSetupWindow passwordSetupWindow = new PasswordSetupWindow(user);
                passwordSetupWindow.ShowDialog();

                users = AuthManager.LoadUsers(); // Оновлюємо список користувачів після встановлення пароля
                return; // ВИХІД, щоб не рахувати спробу як невдалу!
            }


            if (user.PasswordHash != User.HashPassword(password))
            {
                failedAttempts++;
                MessageBox.Show($"Неправильний пароль! Залишилося спроб: {3 - failedAttempts}");

                if (failedAttempts >= 3)
                {
                    MessageBox.Show("Перевищено кількість спроб! Програму буде завершено.");
                    Application.Current.Shutdown();
                }
                return;
            }

            MessageBox.Show($"Вхід успішний! Вітаємо, {username}");

            if (user.IsAdmin)
            {
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
            }
            else
            {
                UserWindow userWindow = new UserWindow(user);
                userWindow.Show();
            }

            this.Close();
        }




    }
}
