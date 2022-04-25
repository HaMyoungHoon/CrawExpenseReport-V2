using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CrawExpenseReport.Data
{
    public static class FAmhohwa
    {
    }

    public class FAES128
    {
        public static string Encrypt(string data, string key)
        {
            while (key.Length < 16)
            {
                key += "0";
            }
            while (key.Length > 16)
            {
                key = key.Substring(0, 16);
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] dataArray = Encoding.UTF8.GetBytes(data);

            RijndaelManaged algo = new RijndaelManaged();
            algo.Mode = CipherMode.ECB;
            algo.Padding = PaddingMode.PKCS7;
            algo.Key = keyArray;

            ICryptoTransform trans = algo.CreateEncryptor();
            byte[] ret = trans.TransformFinalBlock(dataArray, 0, dataArray.Length);

            return FConverter.HexByteToHexStr(ret);
        }
        public static string Decrypt(string data, string key)
        {
            while (key.Length < 16)
            {
                key += "0";
            }
            while (key.Length > 16)
            {
                key = key.Substring(0, 16);
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] dataArray = FConverter.HexStrToByte(data);

            RijndaelManaged algo = new RijndaelManaged();
            algo.Mode = CipherMode.ECB;
            algo.Padding = PaddingMode.PKCS7;
            algo.Key = keyArray;

            ICryptoTransform trans = algo.CreateDecryptor();
            byte[] ret = trans.TransformFinalBlock(dataArray, 0, dataArray.Length);

            return Encoding.UTF8.GetString(ret).Replace("\0", "");
        }
    }
}
