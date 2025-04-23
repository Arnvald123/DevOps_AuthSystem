namespace AuthSystem.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public bool HasPasswordRestrictions { get; set; }

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Пароль не може бути пустим.");
            }

            double numericValue = ConvertPasswordToNumber(password);
            double hashValue = 5.0 * Math.Log(numericValue); // Формула a * ln(x)

            long integerHashValue = (long)hashValue; // Переведення у ціле число
            return integerHashValue.ToString("X"); // Шістнадцятковий формат
        }


        private static double ConvertPasswordToNumber(string password)
        {
            double numericValue = 0;
            foreach (char c in password)
            {
                numericValue += c * c;
            }
            return numericValue;
        }
    }
}

