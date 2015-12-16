using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CchWebAPI.Support
{
    public class CCHEncrypt : Dictionary<string, string>
    {
        #region Private Vars
        private Guid userKey;
        private Guid secretKey;
        private byte[] _keyBytes { get { return userKey.ToByteArray().Take<byte>(8).ToArray(); } }
        private byte[] _secretBytes { get { return secretKey.ToByteArray().Take<byte>(8).ToArray(); } }
        private String _checkSumKey = "__$$";
        private Exception err = null;
        #endregion
        public Exception Error { get { return this.err; } }
        public Guid UserKey { set { this.userKey = value; } }
        public Guid SecretKey { set { this.secretKey = value; } }
        public CCHEncrypt() { }
        public CCHEncrypt(String encryptedData)
        {
            String data = Decrypt(encryptedData);
            String checksum = null;
            String[] args = data.Split('&');
            foreach (String arg in args)
            {
                int i = arg.IndexOf('=');
                if (i != -1)
                {
                    String key = arg.Substring(0, i);
                    String value = arg.Substring(i + 1);
                    if (key == _checkSumKey)
                        checksum = value;
                    else
                        base.Add(HttpUtility.UrlDecode(key), HttpUtility.UrlDecode(value));
                }
            }
            if (checksum == null || checksum != ComputeChecksum())
                base.Clear();
        }
        public CCHEncrypt(String encryptedData, Guid uKey, Guid sKey)
        {
            this.userKey = uKey;
            this.secretKey = sKey;
            String data = Decrypt(encryptedData);
            String checksum = null;
            String[] args = data.Split('&');
            foreach (String arg in args)
            {
                int i = arg.IndexOf('=');
                if (i != -1)
                {
                    String key = arg.Substring(0, i);
                    String value = arg.Substring(i + 1);
                    if (key == _checkSumKey)
                        checksum = value;
                    else
                        base.Add(HttpUtility.UrlDecode(key), HttpUtility.UrlDecode(value));
                }
            }
            if (checksum == null || checksum != ComputeChecksum())
                base.Clear();
        }
        public override string ToString()
        {
            StringBuilder content = new StringBuilder();
            foreach (string key in base.Keys)
            {
                if (content.Length > 0)
                    content.Append('&');
                content.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(base[key]));
            }
            if (content.Length > 0)
                content.Append('&');
            content.AppendFormat("{0}={1}", _checkSumKey, ComputeChecksum());

            return Encrypt(content.ToString());
        }
        private String ComputeChecksum()
        {
            int checksum = 0;
            foreach (KeyValuePair<string, string> pair in this)
            {
                checksum += pair.Key.Sum(c => c - '0');
                checksum += pair.Value.Sum(c => c - '0');
            }
            return checksum.ToString("X");
        }
        private String Encrypt (String text)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] textData = Encoding.UTF8.GetBytes(text);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(
                    ms,
                    des.CreateEncryptor(_secretBytes, _keyBytes),
                    CryptoStreamMode.Write);
                cs.Write(textData, 0, textData.Length);
                cs.FlushFinalBlock();
                return GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                this.err = ex;
                return String.Empty;
            }
        }
        private String Decrypt(String text)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] textData = GetBytes(text);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(
                    ms,
                    des.CreateDecryptor(_secretBytes, _keyBytes),
                    CryptoStreamMode.Write);
                cs.Write(textData, 0, textData.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                this.err = ex;
                return ex.Message;
            }
        }
        private String GetString(byte[] data)
        {
            StringBuilder results = new StringBuilder();
            foreach (byte b in data)
                results.Append(b.ToString("X2"));
            return results.ToString();
        }
        private byte[] GetBytes(string data)
        {
            byte[] result = new byte[data.Length / 2];
            for (int i = 0; i < data.Length; i += 2)
                result[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            return result;
        }
    }
}