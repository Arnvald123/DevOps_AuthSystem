using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AuthSystem.Managers;
using AuthSystem.Models;

namespace AuthSystem
{
    public partial class UserWindow : Window
    {
        private readonly List<(string Question, string Answer)> authenticationQuestions = new List<(string, string)>
{
    ("Скільки буде 2 + 2?", "4"),
    ("Скільки сторін у трикутника?", "3"),
    ("Який колір має небо вдень?", "синій"),
    ("Скільки букв у слові 'Кіт'?", "3"),
    ("Скільки буде 10 - 7?", "3")
};


        private List<User> users;
        private User currentUser;
        private DispatcherTimer authenticationTimer;
        private int totalQuestionsAsked = 0; // Лічильник запитань

        public UserWindow(User user)
        {
            InitializeComponent();
            users = AuthManager.LoadUsers();
            currentUser = user;

            // Ініціалізація таймера
            authenticationTimer = new DispatcherTimer();
            authenticationTimer.Interval = TimeSpan.FromMinutes(1); // Інтервал: 1 хвилина
            authenticationTimer.Tick += (s, e) => ShowAuthenticationQuestions(); // Обробник події
            authenticationTimer.Start(); // Запуск таймера
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

        private void ShowAuthenticationQuestions()
        {
            if (totalQuestionsAsked >= 15) // Якщо досягнуто максимум запитань
            {
                MessageBox.Show("Досягнуто максимальну кількість запитань. Перевірка завершена.");
                authenticationTimer.Stop(); // Зупиняємо таймер
                return;
            }

            MessageBox.Show("Час перевірки активності!");

            int questionsInIteration = 3; // Кількість запитань в ітерації
            Random random = new Random(); // Генератор випадкових чисел

            for (int i = 0; i < questionsInIteration; i++)
            {
                if (totalQuestionsAsked >= 15) break; // Захист від перевищення ліміту

                // Вибираємо випадкове запитання
                var questionData = authenticationQuestions[random.Next(authenticationQuestions.Count)];
                string question = questionData.Question;
                string correctAnswer = questionData.Answer;

                string userAnswer = Microsoft.VisualBasic.Interaction.InputBox(question, "Аутентифікація", "");

                if (userAnswer.ToLower() != correctAnswer.ToLower()) // Перевірка відповіді (нечутлива до регістру)
                {
                    MessageBox.Show("Неправильна відповідь! Ваш доступ буде заблоковано.");
                    this.Close(); // Закриваємо вікно
                    return;
                }

                totalQuestionsAsked++; // Збільшуємо лічильник запитань
            }

            MessageBox.Show("Всі запитання пройдено правильно! Продовжуйте роботу.");
        }


        protected override void OnClosed(EventArgs e)
        {
            authenticationTimer.Stop(); // Зупиняємо таймер
            base.OnClosed(e);
        }
    }
}
