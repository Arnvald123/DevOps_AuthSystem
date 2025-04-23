using System;

namespace KeyEncryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string originalKey = "MYDEMOKEY"; 
            string keyword = "KEY"; 

            string encryptedKey = VigenereCipher.Encrypt(originalKey, keyword);
           
            Console.WriteLine($"Зашифрований ключ: {encryptedKey}");
            Console.WriteLine("Натисніть будь-яку клавішу, щоб завершити...");
            Console.ReadKey();
        }
    }
}
