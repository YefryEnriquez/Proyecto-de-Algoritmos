using System.Security.Cryptography;
using System.Text;

namespace CajeroAutomatico.Utils
{
    public static class Hasher
    {
        // SHA-256 por defecto
        public static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BytesToHexString(bytes); // HEX en mayúsculas
            }
        }

        // MD5 solo para fines académicos (no usar en producción)
        public static string Md5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BytesToHexString(bytes);
            }
        }

        // Hash básico: suma de códigos de caracteres (demostrativo)
        public static string SumaCaracteres(string input)
        {
            long sum = 0;
            foreach (var ch in input)
                sum += ch;
            return sum.ToString();
        }

        // Método auxiliar para convertir bytes a HEX en mayúsculas (reemplazo de Convert.ToHexString)
        private static string BytesToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}
