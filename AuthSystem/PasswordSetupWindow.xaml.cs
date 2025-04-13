using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class PasswordSetupWindow : Window
    {
        private User currentUser;
        private List<User> users;

        public PasswordSetupWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            users = AuthManager.LoadUsers();
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають!");
                return;
            }

            if (!AuthManager.IsPasswordValid(newPassword))
            {
                MessageBox.Show("Пароль має містити хоча б одну букву і один знак пунктуації!");
                return;
            }

            // Змінюємо пароль користувача
            currentUser.PasswordHash = User.HashPassword(newPassword);

            // Завантажуємо список користувачів заново, щоб зберегти зміни
            var users = AuthManager.LoadUsers();

            // **Шукаємо користувача в списку та оновлюємо його пароль**
            var userToUpdate = users.FirstOrDefault(u => u.Username == currentUser.Username);
            if (userToUpdate != null)
            {
                userToUpdate.PasswordHash = currentUser.PasswordHash;
            }

            // **Зберігаємо оновлений список**
            AuthManager.SaveUsers(users);

            MessageBox.Show("Пароль успішно встановлено!");
            this.Close();
        }

    }
}
