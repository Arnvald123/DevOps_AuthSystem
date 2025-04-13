using System.Linq;
using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class AddUserWindow : Window
    {
        private List<User> users;

        public AddUserWindow()
        {
            InitializeComponent();
            users = AuthManager.LoadUsers();
        }

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = UsernameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(newUsername))
            {
                MessageBox.Show("Ім'я користувача не може бути пустим!");
                return;
            }

            if (users.Any(u => u.Username == newUsername))
            {
                MessageBox.Show("Користувач з таким ім'ям вже існує!");
                return;
            }

            User newUser = new User
            {
                Username = newUsername,
                PasswordHash = "",
                IsAdmin = IsAdminCheckBox.IsChecked == true,
                IsBlocked = false
            };

            users.Add(newUser);
            AuthManager.SaveUsers(users);

            MessageBox.Show($"Користувач {newUsername} успішно доданий!");

            // Викликаємо оновлення списку в `AdminWindow`
            if (Owner is AdminWindow adminWindow)
            {
                adminWindow.UpdateUserList();
            }

            this.Close();
        }
    }
}
