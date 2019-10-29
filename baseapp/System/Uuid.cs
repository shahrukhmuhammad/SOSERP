using System;
using System.Security.Cryptography;

namespace BaseApp
{
    public interface IUuid
    {
        string Id(int numDigits = 16);
        string Random(int length = 4);
    }
}

namespace BaseApp.System
{
    public class Uuid : IUuid
    {
        private char[] _charMap = { '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private RNGCryptoServiceProvider _provider;
        public Uuid()
        {
            _provider = new RNGCryptoServiceProvider();
        }
        private void Next(byte[] bytes)
        {
            _provider.GetBytes(bytes);
        }
        public string Id(int numDigits = 16)
        {
            var basis = new byte[0];
            int byteCount = 16;
            var randBytes = new byte[byteCount - basis.Length];
            Next(randBytes);
            var bytes = new byte[byteCount];
            Array.Copy(basis, 0, bytes, byteCount - basis.Length, basis.Length);
            Array.Copy(randBytes, 0, bytes, 0, randBytes.Length);

            ulong lo = (((ulong)BitConverter.ToUInt32(bytes, 8)) << 32) | BitConverter.ToUInt32(bytes, 12);
            ulong hi = (((ulong)BitConverter.ToUInt32(bytes, 0)) << 32) | BitConverter.ToUInt32(bytes, 4);
            ulong mask = 0x1F;

            var chars = new char[26];
            int charIdx = 25;

            ulong work = lo;
            for (int i = 0; i < 26; i++)
            {
                if (i == 12)
                {
                    work = ((hi & 0x01) << 4) & lo;
                }
                else if (i == 13)
                {
                    work = hi >> 1;
                }
                byte digit = (byte)(work & mask);
                chars[charIdx] = _charMap[digit];
                charIdx--;
                work = work >> 5;
            }

            var ret = new string(chars, 26 - numDigits, numDigits);
            return ret;
        }
        public string Random(int length = 4)
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789$~#@-";
            var stringChars = new char[length];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];
            return new string(stringChars);
        }
    }
}
