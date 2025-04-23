using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AuthSystem.Models;

namespace AuthSystem.Managers
{
    public class AuthManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.txt");
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "registration_log.txt");

        public static void LogAction(string username, string action)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(action))
                return;

            if (!File.Exists(LogFilePath)) // Якщо файл ще не створений
            {
                File.WriteAllText(LogFilePath, "Дата і час, Користувач, Дія\n"); // Додаємо заголовок до файлу
            }

            string logEntry = $"{DateTime.Now}, {username}, {action}";
            File.AppendAllText(LogFilePath, logEntry + Environment.NewLine); // Додаємо запис до файлу
        }

        public static List<User> LoadUsers()
        {
            if (!File.Exists(FilePath))
            {
                MessageBox.Show($"Файл {FilePath} не знайдено! Створюється новий файл...");
                File.WriteAllText(FilePath, "ADMIN,,true,false,true"); // Створюємо файл з адміністратором
            }

            var lines = File.ReadAllLines(FilePath);
            return lines.Select(line =>
            {
                var parts = line.Split(',');

                // Запобігання помилкам формату
                if (parts.Length < 5)
                {
                    MessageBox.Show($"Помилка: Неправильний формат у рядку \"{line}\"");
                    return null;
                }

                return new User
                {
                    Username = parts[0],
                    PasswordHash = parts[1],
                    IsAdmin = bool.Parse(parts[2]),
                    IsBlocked = bool.Parse(parts[3]),
                    HasPasswordRestrictions = bool.Parse(parts[4])
                };
            }).Where(user => user != null).ToList();
        }

        public static void SaveUsers(List<User> users)
        {
            var lines = users.Select(u => $"{u.Username},{u.PasswordHash},{u.IsAdmin},{u.IsBlocked},{u.HasPasswordRestrictions}");
            File.WriteAllLines(FilePath, lines);
        }

        public static void AddUser(User user)
        {
            var users = LoadUsers();
            users.Add(user);
            SaveUsers(users);
        }

        public static bool IsPasswordValid(string password)
        {
            return password.Any(char.IsLetter) && password.Any(char.IsPunctuation);
        }
    }
}
