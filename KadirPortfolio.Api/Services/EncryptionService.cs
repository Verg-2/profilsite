using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace KadirPortfolio.Api.Services
{
    public interface IEncryptionService
    {
        (string CipherText, string IV) Encrypt(string plainText);
        string Decrypt(string cipherText, string iv);
    }

    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _masterKey;

        public EncryptionService(IConfiguration configuration)
        {
            var keyString = configuration["Encryption:MasterKey"] 
                            ?? Environment.GetEnvironmentVariable("ENCRYPTION_MASTER_KEY");

            var saltString = configuration["Encryption:Salt"] 
                            ?? Environment.GetEnvironmentVariable("ENCRYPTION_SALT") 
                            ?? "VarsayilanGuvenliRastgeleUzunSaltDegeri_99!";

            if (string.IsNullOrEmpty(keyString) || keyString.Length < 16)
            {
                throw new InvalidOperationException("Kritik Hata: Güvenli bir şifreleme anahtarı tanımlanmamış!");
            }

            // PBKDF2 ile güvenli 256-bit anahtar üretimi
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                keyString, 
                Encoding.UTF8.GetBytes(saltString),
                150000, // Yüksek iterasyon sayısı
                HashAlgorithmName.SHA256))
            {
                _masterKey = pbkdf2.GetBytes(32); // 256-bit key
            }
        }

        public (string CipherText, string IV) Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            byte[] nonce = new byte[12]; // GCM için 12 byte nonce standarttır
            RandomNumberGenerator.Fill(nonce);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16]; // Auth tag

            using (var aesGcm = new AesGcm(_masterKey, tagSizeInBytes: 16))
            {
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            // GCM şifrelemede Nonce ve Tag değerlerini de saklamamız gerekir.
            // IV olarak nonce değerini, CipherText olarak ise ciphertext + tag kombinasyonunu saklayabiliriz.
            byte[] combinedCipherAndTag = new byte[ciphertext.Length + tag.Length];
            Buffer.BlockCopy(ciphertext, 0, combinedCipherAndTag, 0, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, combinedCipherAndTag, ciphertext.Length, tag.Length);

            return (Convert.ToBase64String(combinedCipherAndTag), Convert.ToBase64String(nonce));
        }

        public string Decrypt(string cipherText, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            if (string.IsNullOrEmpty(iv))
                throw new ArgumentNullException(nameof(iv));

            byte[] nonce = Convert.FromBase64String(iv);
            byte[] combined = Convert.FromBase64String(cipherText);

            int tagSize = 16;
            int ciphertextSize = combined.Length - tagSize;

            byte[] ciphertext = new byte[ciphertextSize];
            byte[] tag = new byte[tagSize];

            Buffer.BlockCopy(combined, 0, ciphertext, 0, ciphertextSize);
            Buffer.BlockCopy(combined, ciphertextSize, tag, 0, tagSize);

            byte[] decryptedBytes = new byte[ciphertextSize];

            using (var aesGcm = new AesGcm(_masterKey, tagSizeInBytes: 16))
            {
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
