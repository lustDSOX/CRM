using CRM.Models;
using System.IO.Compression;
using System.Reflection.Metadata;
using System.Text;

namespace CRM.Classes
{
    public class EncryptDecrypt
    {
        public static string Decrypt(byte[] bytes)
        {
            string result = "";
            try
            {
                using (var input = new MemoryStream(bytes))
                {
                    using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(gzip))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                result = Encoding.UTF8.GetString(bytes);
            }
            return result;
        }
        public static byte[] Encrypt(string message)
        {
            byte[] tempHtml = Encoding.UTF8.GetBytes(message.ToCharArray());
            byte[] compressedBytes;
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress))
                {
                    gzip.Write(tempHtml, 0, tempHtml.Length);
                }
                compressedBytes = output.ToArray();
            }
            return compressedBytes;
        }
    }
}
