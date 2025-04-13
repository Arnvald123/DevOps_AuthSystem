using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class UserWindow : Window
    {
        private List<User> users;
        private User currentUser;

        public UserWindow(User user)
        {
            InitializeComponent();
            users = AuthManager.LoadUsers();
            currentUser = user;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string oldPassword = OldPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (currentUser.PasswordHash != User.HashPassword(oldPassword))
            {
                MessageBox.Show("Неправильний старий пароль!");
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають!");
                return;
            }

            if (currentUser.HasPasswordRestrictions && !AuthManager.IsPasswordValid(newPassword))
            {
                MessageBox.Show("Пароль має містити хоча б одну букву і один знак пунктуації!");
                return;
            }

            currentUser.PasswordHash = User.HashPassword(newPassword);
            AuthManager.SaveUsers(users);
            MessageBox.Show("Пароль успішно змінено!");
            this.Close();
        }



    }
}
