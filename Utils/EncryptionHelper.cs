using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace HexiUtils
{
    public class EncryptionHelper
    {
        public static string MD5Encryption(string str)
        {
            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] OutBytes = md5.ComputeHash(data);
            string OutString = "";
            OutString = BitConverter.ToString(OutBytes).Replace("-", "");
            //string OutString = "";
            //for (int i = 0; i < OutBytes.Length; i++)
            //{
            //    OutString += OutBytes[i].ToString();
            //}
            // return OutString.ToUpper();
            return OutString;
        }
    }
}
