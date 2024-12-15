using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Utilities.Extensions
{
    //not my code, i just copied it from random site
    public static class Hash
    {
        public static string HashText(this string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
