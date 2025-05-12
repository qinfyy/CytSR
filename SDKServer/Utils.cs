using System.Security.Cryptography;
using System.Text;

namespace SDKServer
{
    public static class Utils
    {
        public static string GenerateToken(string accountUid)
        {
            Random random = new Random();
            int randomValue = random.Next();
            string combinedInput = accountUid + randomValue.ToString("X8");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedInput));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
