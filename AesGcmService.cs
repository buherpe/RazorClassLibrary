using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace RazorClassLibrary
{
    // https://stackoverflow.com/a/60891115
    public class AesGcmService : IDisposable
    {
        private readonly AesGcm _aes;

        public AesGcmService(string password)
        {
            var key = new Rfc2898DeriveBytes(password, new byte[8], 1000, HashAlgorithmName.SHA256).GetBytes(16);

            _aes = new AesGcm(key);
        }

        public string Encrypt(string plain)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plain);

            var nonceSize = AesGcm.NonceByteSizes.MaxSize;
            var tagSize = AesGcm.TagByteSizes.MaxSize;
            var cipherSize = plainBytes.Length;

            var encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            var encryptedData = encryptedDataLength < 1024 ? stackalloc byte[encryptedDataLength] : new byte[encryptedDataLength].AsSpan();

            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            RandomNumberGenerator.Fill(nonce);

            _aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string cipher)
        {
            var encryptedData = Convert.FromBase64String(cipher).AsSpan();

            var nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            var tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            var cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            var plainBytes = cipherSize < 1024 ? stackalloc byte[cipherSize] : new byte[cipherSize];
            _aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public void Dispose()
        {
            _aes.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
