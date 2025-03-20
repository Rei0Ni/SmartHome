using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SmartHome.Application.Configuration;
using SmartHome.Application.Interfaces;

namespace SmartHome.Application.Services
{
    public class TotpService : ITotpService
    {
        private readonly TotpSettings _totpSettings;

        public TotpService(IOptions<TotpSettings> totpSettings)
        {
            _totpSettings = totpSettings.Value;
        }

        public string GenerateSecretKey()
        {
            byte[] key = new byte[20];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return Base32Encoding.Encode(key);
        }

        public string GenerateQrCodeUrl(string userName, string secretKey)
        {
            string encodedUserName = WebUtility.UrlEncode(userName);
            string encodedSecretKey = WebUtility.UrlEncode(secretKey);
            string url = $"otpauth://totp/{encodedUserName}?secret={encodedSecretKey}&issuer={_totpSettings.Issuer}";
            return url;
        }

        public bool ValidateTotpCode(string secretKey, string code)
        {
            try
            {
                byte[] key = Base32Encoding.Decode(secretKey);
                long timeStep = GetCurrentTimeStep();
                string expectedCode1 = GenerateTotpCode(key, timeStep);
                string expectedCode2 = GenerateTotpCode(key, timeStep - 1);
                string expectedCode3 = GenerateTotpCode(key, timeStep + 1);
                return code == expectedCode1 || code == expectedCode2 || code == expectedCode3;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error validating TOTP code: {ex.Message}");
                return false;
            }
        }

        public long GetCurrentTimeStep()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return timeSpan.Ticks / TimeSpan.TicksPerSecond / _totpSettings.ExpirySeconds;
        }

        public string GenerateTotpCode(string secretKey, long timeStep)
        {
            byte[] key = Base32Encoding.Decode(secretKey);
            return GenerateTotpCode(key, timeStep);
        }

        public string GenerateTotpCode(byte[] key, long timeStep)
        {
            byte[] timeBytes = BitConverter.GetBytes(timeStep);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timeBytes);
            }

            using (var hmac = new HMACSHA1(key))
            {
                byte[] hash = hmac.ComputeHash(timeBytes);
                int offset = hash[hash.Length - 1] & 0x0F;
                int truncatedHash = (hash[offset] & 0x7F) << 24
                                      | (hash[offset + 1] & 0xFF) << 16
                                      | (hash[offset + 2] & 0xFF) << 8
                                      | (hash[offset + 3] & 0xFF);

                int code = truncatedHash % (int)Math.Pow(10, _totpSettings.CodeLength);
                return code.ToString($"D{_totpSettings.CodeLength}");
            }
        }

        public static class Base32Encoding
        {
            private static readonly char[] _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();

            public static string Encode(byte[] data)
            {
                if (data == null || data.Length == 0)
                {
                    return string.Empty;
                }

                int inBits = data.Length * 8;
                int outBits = (int)Math.Ceiling(inBits / 5.0);
                int padding = (outBits * 5 - inBits) / 5;

                char[] result = new char[outBits];
                int bitIndex = 0;
                int buffer = data[0];
                int bufferBits = 8;
                int charIndex = 0;

                while (bitIndex < inBits)
                {
                    int shift = 5 - (bitIndex % 5);
                    if (bufferBits < shift)
                    {
                        buffer <<= 8;
                        bufferBits += 8;
                        if ((bitIndex / 8) + 1 < data.Length)
                        {
                            buffer |= data[(bitIndex / 8) + 1];
                        }
                    }

                    int index = (buffer >> (bufferBits - shift)) & 0x1F;
                    result[charIndex++] = _base32Chars[index];
                    bufferBits -= shift;
                    bitIndex += shift;
                }

                for (int i = 0; i < padding; i++)
                {
                    result[outBits - 1 - i] = '=';
                }

                return new string(result);
            }

            public static byte[] Decode(string base32)
            {
                if (string.IsNullOrEmpty(base32))
                {
                    return Array.Empty<byte>();
                }

                base32 = base32.TrimEnd('=').ToUpper();
                int paddingCount = base32.Length - (base32.Length * 5 / 8) * 8 / 5;
                int inBits = base32.Length * 5;
                int outBits = inBits / 8;
                byte[] result = new byte[outBits];
                int bitIndex = 0;
                int buffer = 0;
                int bufferBits = 0;
                int charIndex = 0;

                while (charIndex < base32.Length)
                {
                    int val = Array.IndexOf(_base32Chars, base32[charIndex++]);
                    if (val < 0)
                    {
                        continue;
                    }

                    buffer = (buffer << 5) | val;
                    bufferBits += 5;

                    if (bufferBits >= 8)
                    {
                        int shift = bufferBits - 8;
                        result[bitIndex++] = (byte)((buffer >> shift) & 0xFF);
                        bufferBits -= 8;
                    }
                }
                return result;
            }
        }
    }
}
