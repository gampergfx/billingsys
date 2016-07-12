using System;
using System.IO;

namespace FraudDetector
{
    class Program
    {
        static void Main(string[] args)
        {
            var canWorTill = DateTimeOffset.Now.AddYears(1);
            var encoded = Base64Encode(canWorTill.ToString("O"));

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadat.ccc");

            File.WriteAllText(path,encoded);
            ///////
            /// 
            var data = File.ReadAllText(path);
            var decodedData = Base64Decode(data);
            var date = DateTimeOffset.Parse(decodedData);

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
