using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SQNotificationService.Models.Database
{
    public static class UniqueIds
    {
        public static string NewExternalReferenceId()
        {
            int maxSize = 11;
            char[] chars = new char[62];
            string a;
            a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);

		    foreach(byte b in data )
		    { 
                result.Append(chars[b % (chars.Length - 1)]); 
            }
		    return result.ToString();
	   }


        public static string NewMessageCode()
        {
            // TODO,, maxSize or characterLimit should be a config setting.
            int maxSize = 4;
            char[] chars = new char[58];
            string a;
            a = "abcdefghjkmnopqrstuvwxyzABCDEFGHJKMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

        
    }
}