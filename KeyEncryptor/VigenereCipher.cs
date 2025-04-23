public static class VigenereCipher
{
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
                result += c; // Залишаємо символ без змін
            }
        }
        return result;
    }
}
