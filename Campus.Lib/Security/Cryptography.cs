using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campus.Lib.Security
{
    public class Cryptography
    {
        public string DecryptStringConnection(string connectionString)
        {
            Byte[] b = Convert.FromBase64String(connectionString);
            string decryptConnectionString = System.Text.ASCIIEncoding.ASCII.GetString(b);
            return decryptConnectionString;
        }
        public string EncryptStringConnection(string connectionString)
        {
            Byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(connectionString);
            string encryptConnectionString = Convert.ToBase64String(b);
            return encryptConnectionString;
        }
    }
}
