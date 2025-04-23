using System;

namespace AuthSystem
{
    public static class VigenereCipher
    {
        // Метод для шифрування тексту
        public static string Encrypt(string text, string key)
        {
            string result = "";
            key = key.ToUpper();

            for (int i = 0, j = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    c = char.ToUpper(c);
                    result += (char)((c + key[j % key.Length] - 2 * 'A') % 26 + 'A');
                    j++;
                }
                else
                {
                    result += c; // Залишаємо символ без змін, якщо це не літера
                }
            }
            return result;
        }

        // Метод для дешифрування тексту
        public static string Decrypt(string text, string key)
        {
            string result = "";
            key = key.ToUpper();

            for (int i = 0, j = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetter(c))
                {
                    c = char.ToUpper(c);
                    result += (char)((c - key[j % key.Length] + 26) % 26 + 'A');
                    j++;
                }
                else
                {
                    result += c; // Залишаємо символ без змін
                }
            }
            return result;
        }
    }
}
