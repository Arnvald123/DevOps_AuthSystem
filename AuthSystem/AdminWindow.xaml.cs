using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;
using System.IO;

namespace AuthSystem
{
    public partial class AdminWindow : Window
    {
        private List<User> users; // Список користувачів
        private bool isDemoMode = true; // Початково активний демо-режим
        private const string ActivationKey = "WCBOQMUIW"; // Зашифрований ключ


        public AdminWindow()
        {
            InitializeComponent();
            users = AuthManager.LoadUsers(); // Завантаження користувачів
            UpdateUserList();
        }

        // Обробка натискання кнопки активації
        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            string userInput = ActivationTextBox.Text.Trim(); 
            string decryptedKey = VigenereCipher.Decrypt(userInput, "KEY"); 

            if (decryptedKey == "MYDEMOKEY") // Оригінальний ключ для перевірки
            {
                isDemoMode = false; 
                MessageBox.Show("Повний режим активовано!");
            }
            else
            {
                MessageBox.Show("Неправильний ключ активації.");
            }
        }


        // Оновлення списку користувачів
        public void UpdateUserList()
        {
            users = AuthManager.LoadUsers(); // Завантажити користувачів
            UsersListBox.ItemsSource = users.Select(u =>
                $"{u.Username} (Адміністратор: {u.IsAdmin}, Заблокований: {u.IsBlocked})").ToList();
        }

        // Збереження таблиці користувачів
        private void SaveUserTable_Click(object sender, RoutedEventArgs e)
        {
            if (isDemoMode)
            {
                MessageBox.Show("Ця функція недоступна у демо-версії.");
                return;
            }

            // Діалог вибору шляху для збереження
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = "UserTable.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveUserTableToFile(saveFileDialog.FileName);
                MessageBox.Show("Таблицю користувачів збережено успішно!");
            }
        }

        // Метод запису таблиці користувачів у файл
        private void SaveUserTableToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Ім'я користувача, Пароль (хеш), Статус");
                    foreach (var user in users)
                    {
                        string status = user.IsBlocked ? "Заблокований" : "Активний";
                        writer.WriteLine($"{user.Username}, {user.PasswordHash}, {status}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Сталася помилка під час збереження: {ex.Message}");
            }
        }

        // Додавання нового користувача
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow addUserWindow = new AddUserWindow();
            addUserWindow.ShowDialog();
            UpdateUserList();
            AuthManager.LogAction("ADMIN", "Додано нового користувача.");
        }

        // Зміна пароля
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

        // Блокування користувача
        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;

            string username = users[UsersListBox.SelectedIndex].Username;
            var user = users.First(u => u.Username == username);

            user.IsBlocked = true;
            AuthManager.SaveUsers(users);
            UpdateUserList();
            MessageBox.Show($"Користувач {username} заблокований!");
            AuthManager.LogAction("ADMIN", $"Користувач {username} заблокований.");
        }

        // Розблокування користувача
        private void UnblockUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;

            string username = users[UsersListBox.SelectedIndex].Username;
            var user = users.First(u => u.Username == username);

            user.IsBlocked = false;
            AuthManager.SaveUsers(users);
            UpdateUserList();
            MessageBox.Show($"Користувач {username} розблокований!");
            AuthManager.LogAction("ADMIN", $"Користувач {username} розблокований.");
        }

        // Видалення користувача
        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedIndex == -1) return;

            string username = users[UsersListBox.SelectedIndex].Username;
            if (username == "ADMIN")
            {
                MessageBox.Show("Адміністратора не можна видалити!");
                return;
            }

            var confirm = MessageBox.Show($"Ви впевнені, що хочете видалити {username}?",
                                          "Підтвердження", MessageBoxButton.YesNo);
            if (confirm == MessageBoxResult.Yes)
            {
                users = users.Where(u => u.Username != username).ToList();
                AuthManager.SaveUsers(users);
                UpdateUserList();
                MessageBox.Show($"Користувач {username} успішно видалений.");
                AuthManager.LogAction("ADMIN", $"Користувач {username} видалений.");
            }
        }
    }
}
