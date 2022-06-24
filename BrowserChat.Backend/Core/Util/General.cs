﻿using BrowserChat.Util;

namespace BrowserChat.Backend.Core.Util
{
    public static class General
    {

        public static string EncryptString(string str)
        {
            return Encrypt(str);
        }

        public static string DesencryptString(string str)
        {
            return Desencrypt(str);
        }

        public static string EncryptStringEncoded(string str)
        {
            return System.Web.HttpUtility.UrlEncode(Encrypt(str));
        }

        public static string DesencryptStringEncoded(string str)
        {
            return Desencrypt(System.Web.HttpUtility.UrlDecode(str));
            
        }

        private static string Encrypt(string str)
        {
            return BrowserChat.Util.StringCipher.Encrypt(ConfigurationHelper.config.GetValue<string>("EncryptionKey"), str);
        }

        private static string Desencrypt(string str)
        {
            return BrowserChat.Util.StringCipher.Decrypt(ConfigurationHelper.config.GetValue<string>("EncryptionKey"), str);
        }
    }
}
