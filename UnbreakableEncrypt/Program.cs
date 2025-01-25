
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;

class Program
{
    static async Task Main(string[] args)
    {
        var operationalMode = new Dictionary<ConsoleKey, Action>()
        {
            { ConsoleKey.E, ExecuteEncryption },
            { ConsoleKey.D, ExecuteDecryption }
        };

        while (true)
        {
            Console.Clear();
            RenderHeader();
            Console.WriteLine("Select your operation (d => Decrypting, e => Encrypting):");
            var inputKey = Console.ReadKey().Key;
            Console.Clear();

            if (operationalMode.ContainsKey(inputKey))
            {
                operationalMode[inputKey].Invoke();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPress any key to repeat or Esc to exit...");
            if (Console.ReadKey().Key == ConsoleKey.Escape) break;
        }
    }

    static void ExecuteEncryption()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Enter the word to encrypt:");
        Console.ForegroundColor = ConsoleColor.White;
        var plaintext = Console.ReadLine();
        Console.Clear();

        var encryptionKeys = GenerateKeys();
        var cipherText = EncryptWord(plaintext, encryptionKeys[0], encryptionKeys[1], encryptionKeys[2]);

        DisplayEncryptionResults(cipherText, encryptionKeys);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Press D to decrept the current encrypted word: ");
        var key = Console.ReadKey();
        Console.WriteLine();
        if (key.Key == ConsoleKey.D)
        {
            var decryptedText = DecryptWord(cipherText, encryptionKeys[0], encryptionKeys[1], encryptionKeys[2]);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Decrypted Word: " + decryptedText);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }


    static void ExecuteDecryption()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Enter the encrypted text:");
        Console.ForegroundColor = ConsoleColor.White;
        var cipherText = Console.ReadLine();
        Console.Clear();

        var keys = RetrieveDecryptionKeys();
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Magenta;
        var decryptedOutput = DecryptWord(cipherText, keys[0], keys[1], keys[2]);
        Console.WriteLine("Decrypted Output: " + decryptedOutput);
    }

    static List<string> GenerateKeys()
    {
        var keys = new List<string>
        {
            Guid.NewGuid().ToString() + "-" + Guid.NewGuid() + "-" + Guid.NewGuid(),
            Guid.NewGuid().ToString() + "-" + Guid.NewGuid() + "-" + Guid.NewGuid(),
            Guid.NewGuid().ToString() + "-" + Guid.NewGuid() + "-" + Guid.NewGuid()
        };
        return keys;
    }

    static void DisplayEncryptionResults(string cipherText, List<string> keys)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ciphered Text: " + cipherText);
        Console.ForegroundColor = ConsoleColor.Magenta;
        for (int i = 0; i < keys.Count; i++)
        {
            Console.WriteLine($"Key{i + 1}: " + keys[i]);
        }
    }

    static List<string> RetrieveDecryptionKeys()
    {
        var decryptionKeys = new List<string>();
        for (int i = 1; i <= 3; i++)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Enter Key {i}:");
            decryptionKeys.Add(Console.ReadLine());
        }
        return decryptionKeys;
    }

    static void RenderHeader()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Unbreakable Encrypter By Wisam Idris");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("https://github.com/wisamidris7");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
    }
    private static string EncryptWord(string word, string key1, string key2, string key3)
    {
        var image = ExportImage(word, key1);
        return MultiLayerEncrypt(Convert.ToBase64String(image), key2, key3);
    }

    private static string DecryptWord(string encryptedText, string key1, string key2, string key3)
    {
        try
        {
            var image = MultiLayerDecrypt(encryptedText, key2, key3);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(Convert.FromBase64String(image));
            return ReadImage(memoryStream, key1);
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("EncryptedText or one of keys was wrong for more details blow");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(ex.GetType().Name + ":" + ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ex.StackTrace);
            Console.ReadKey();
            return string.Empty;
        }
    }
    private static string ReadImage(MemoryStream memoryStream, string key)
    {
        var characters = "";
        using (Bitmap bitmap = new Bitmap(memoryStream))
        {
            var collectedPixels = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    var color = bitmap.GetPixel(x, y);
                    if (color.R == 255 && color.B == 255 && color.G == 255)
                    {
                        if (collectedPixels != 0)
                        {
                            var character = Encoding.UTF8.GetString([Convert.ToByte(collectedPixels)]);
                            characters += character;
                        }
                        collectedPixels = 0;
                    }
                    else if (color.R == 0 && color.B == 0 && color.G == 0)
                    {
                        collectedPixels++;
                    }
                    else
                    {
                        Console.WriteLine(color);
                    }
                }
            }
            if (collectedPixels != 0)
            {
                var character = Encoding.UTF8.GetString([Convert.ToByte(collectedPixels)]);
                characters += character;
            }
            collectedPixels = 0;
        }
        return MultiLayerDecrypt(characters, key, string.Join("", key.Reverse()));
    }
    static string MultiLayerEncrypt(string plainText, string keyString1, string keyString2)
    {
        string aesEncrypted = EncryptAES(plainText, keyString1, keyString2);
        string tripleDESEncrypted = EncryptTripleDES(aesEncrypted, keyString2);
        string rc2Encrypted = EncryptRC2(tripleDESEncrypted, keyString1);
        string rijndaelEncrypted = EncryptRijndael(rc2Encrypted, keyString1);

        return rijndaelEncrypted;
    }

    static byte[] GenerateMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            return md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }

    static byte[] ReverseArray(byte[] array)
    {
        Array.Reverse(array);
        return array;
    }

    static string EncryptAES(string plainText, string keyString1, string keyString2)
    {
        byte[] key = GenerateMD5Hash(keyString1);
        byte[] iv = GenerateMD5Hash(keyString2);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    static string EncryptTripleDES(string plainText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (TripleDES tripleDESAlg = TripleDES.Create())
        {
            tripleDESAlg.Mode = CipherMode.ECB;
            tripleDESAlg.Key = key;
            //tripleDESAlg.IV = iv;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, tripleDESAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    static string EncryptRC2(string plainText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (RC2 rc2Alg = RC2.Create())
        {
            rc2Alg.Mode = CipherMode.ECB;
            rc2Alg.Key = key;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, rc2Alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    static string EncryptRijndael(string plainText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (Rijndael rijndaelAlg = Rijndael.Create())
        {
            rijndaelAlg.Key = key;
            rijndaelAlg.IV = iv;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, rijndaelAlg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    static string MultiLayerDecrypt(string encryptedText, string keyString1, string keyString2)
    {
        string rijndaelDecrypted = DecryptRijndael(encryptedText, keyString1);
        string rc2Decrypted = DecryptRC2(rijndaelDecrypted, keyString1);
        string tripleDESDecrypted = DecryptTripleDES(rc2Decrypted, keyString2);
        string aesDecrypted = DecryptAES(tripleDESDecrypted, keyString1, keyString2);

        return aesDecrypted;
    }

    static string DecryptAES(string encryptedText, string keyString1, string keyString2)
    {
        byte[] key = GenerateMD5Hash(keyString1);
        byte[] iv = GenerateMD5Hash(keyString2);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    static string DecryptTripleDES(string encryptedText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (TripleDES tripleDESAlg = TripleDES.Create())
        {
            tripleDESAlg.Mode = CipherMode.ECB;
            tripleDESAlg.Key = key;
            //tripleDESAlg.IV = iv;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, tripleDESAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    static string DecryptRC2(string encryptedText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (RC2 rc2Alg = RC2.Create())
        {
            rc2Alg.Mode = CipherMode.ECB;
            rc2Alg.Key = key;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, rc2Alg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    static string DecryptRijndael(string encryptedText, string keyString)
    {
        byte[] key = GenerateMD5Hash(keyString);
        byte[] iv = ReverseArray(key);

        using (Rijndael rijndaelAlg = Rijndael.Create())
        {
            rijndaelAlg.Key = key;
            rijndaelAlg.IV = iv;

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, rijndaelAlg.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

    private static byte[] ExportImage(string word, string key)
    {
        word = MultiLayerEncrypt(word, key, string.Join("", key.Reverse()));
        var bytesInts = new List<int>();
        foreach (var character in word)
        {
            var byteId = Encoding.UTF8.GetBytes([character]);
            var byteInt = int.Parse(byteId.FirstOrDefault().ToString());
            bytesInts.Add(byteInt);
        }
        int width = (bytesInts.Sum(e => e + 1) - 1);
        int height = 1;
        // Create a new bitmap
        using (Bitmap bitmap = new Bitmap(width, height))
        {
            var currentX = 0;
            var currentY = 0;
            foreach (var item in bytesInts)
            {
                for (int i = 0; i < item; i++)
                {
                    for (int h = 0; h < height - 1; h++)
                    {
                        bitmap.SetPixel(currentX, currentY, Color.Black);
                        currentY++;
                    }
                    currentY = 0;
                    currentX++;
                }
                if (currentX != width)
                {
                    for (int h = 0; h < height; h++)
                    {
                        bitmap.SetPixel(currentX, currentY, Color.White);
                        currentY++;
                    }
                    currentY = 0;
                    currentX++;
                }
            }
            MemoryStream memoryStream = new MemoryStream();
            // Save the bitmap to a file
            bitmap.Save(memoryStream, ImageFormat.Png);

            return memoryStream.ToArray();
        }
    }
}
