using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System;

namespace PKFramework
{
    public static class CommonUtils
    {
        public static Dictionary<string, int> uniqIdDict;

        public static int GetUniqIdByKey(string key)
        {
            int value = 0;
            if(!uniqIdDict.TryGetValue(key, out value))
            {
                uniqIdDict[key] = value;
            }
            else
            {
                value++;
                uniqIdDict[key] = value;
            }
            return value;
        }

        public static string SHA512Encode(string source)
        {
            string hash = "";
            
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(source);//UTF-8 编码
                byte[] hashBytes = sha512Hash.ComputeHash(buffer);
                hash = BitConverter.ToString(hashBytes);
            }
  
            return hash;
        }
    }

}
