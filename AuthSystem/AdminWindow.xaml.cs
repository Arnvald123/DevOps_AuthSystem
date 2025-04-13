using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class AdminWindow : Window
    {
        private List<User> users;

        public AdminWindow()
        {
            InitializeComponent();
            users = AuthManager.LoadUsers();
            UpdateUserList();
        }

        public void UpdateUserList()
        {
            users = AuthManager.LoadUsers(); // Завантажуємо список
            UsersListBox.ItemsSource = null; // Очищаємо старий список
            UsersListBox.ItemsSource = users.Select(u =>$"{u.Username} (Адміністратор: {u.IsAdmin}, Заблокований: {u.IsBlocked})").ToList();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.ShowDialog();
            UpdateUserList(); // Оновлюємо список після додавання
        }


        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;
            string username = users[UsersListBox.SelectedIndex].Username;

            string newPassword = Microsoft.VisualBasic.Interaction.InputBox("Введіть новий пароль:", "Зміна пароля", "");
            if (string.IsNullOrWhiteSpace(newPassword)) return;

            users.First(u => u.Username == username).PasswordHash = User.HashPassword(newPassword);
            AuthManager.SaveUsers(users);
            UpdateUserList();
        }

        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;
            string username = users[UsersListBox.SelectedIndex].Username;

            var user = users.First(u => u.Username == username);
            user.IsBlocked = !user.IsBlocked;
            AuthManager.SaveUsers(users);
            UpdateUserList();
            MessageBox.Show($"Користувач {username} заблокований!");
        }

        private void UnblockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return; // Перевіряємо, чи щось вибрано

            string username = users[UsersListBox.SelectedIndex].Username;
            var user = users.First(u => u.Username == username);

            user.IsBlocked = false;
            AuthManager.SaveUsers(users);
            UpdateUserList();

            MessageBox.Show($"Користувач {username} успішно розблокований!");
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;

            string username = users[UsersListBox.SelectedIndex].Username;

            // Забороняємо видалення адміністратора
            if (username == "ADMIN")
            {
                MessageBox.Show("Неможливо видалити адміністратора!");
                return;
            }

            var confirm = MessageBox.Show($"Ви впевнені, що хочете видалити {username}?",
                                          "Підтвердження", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes) return;

            users = users.Where(u => u.Username != username).ToList();
            AuthManager.SaveUsers(users);
            UpdateUserList();

            MessageBox.Show($"Користувач {username} успішно видалений!");
        }

    }
}
