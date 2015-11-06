using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;

namespace InstaCreator
{
    class Account
    {
        private Random rnd = new Random();
        private readonly Encoding encoding = Encoding.UTF8;

        private const string SecretKey = "3f0a7d75e094c7385e3dbaa026877f2e067cbd1a4dbcf3867748f6b26f257117";
        private string DeviceID = null;
        private string guid = null;
        private string CSRF = null;
        private string mid = null;
        CookieContainer Cookies = new CookieContainer();
  
        public Account(string Username, string Password, string Email)
        {
            DeviceID = "android-" + HMAC(rnd.Next(1000, 9999).ToString(), "1337").ToString().Substring(0, Math.Min(64, 16));
            guid = RandomString(8) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(4) + "-" + RandomString(12);
            mid = "VjzMwwAEAAGFO33NbLSpjPGBnXJ_";
            CSRF = GetCSRF(GetHTML());
            Register(Username, Password, Email);
        }

        private void Register(string Username, string Password, string Email)
        {
            string Config = @"{""username"":""" + Username + @""",""first_name"":""Name"",""password"":""" + Password + @""",""guid"":""" + guid + @""",""email"":""" + Email + @""",""device_id"":""" + DeviceID + @"""}";
            byte[] bytes = ASCIIEncoding.UTF8.GetBytes("signed_body=" + HMAC(Config, SecretKey) + "." + EncodeUrl(Config) + "&ig_sig_key_version=4");
            HttpWebRequest postReq = (HttpWebRequest)WebRequest.Create("https://i.instagram.com/api/v1/accounts/create/");
            postReq.AutomaticDecompression = DecompressionMethods.GZip;
            WebHeaderCollection postHeaders = postReq.Headers;
            postReq.Method = "POST";
            postReq.Host = "i.instagram.com";
            postReq.UserAgent = "Instagram 7.1.1 Android (21/5.0.2; 480dpi; 1080x1776; LGE/Google; Nexus 5; hammerhead; hammerhead; en_US)";
            postReq.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            Cookies = new CookieContainer();
            Cookies.Add(new Cookie("csrftoken", CSRF) { Domain = postReq.Host });
            Cookies.Add(new Cookie("mid", mid) { Domain = postReq.Host });
            postReq.CookieContainer = Cookies;
            postHeaders.Add("Cookie2", "$Version=1");
            postHeaders.Add("Accept-Language", "en-US");
            postHeaders.Add("X-IG-Connection-Type", "WIFI");
            postHeaders.Add("X-IG-Capabilities", "BQ==");
            postHeaders.Add("Accept-Encoding", "gzip");

            Stream postStream = postReq.GetRequestStream();
            postStream.Write(bytes, 0, bytes.Length);
            postStream.Close();
            HttpWebResponse postResponse;
            postResponse = (HttpWebResponse)postReq.GetResponse();
            StreamReader reader = new StreamReader(postResponse.GetResponseStream());
            string Response = reader.ReadToEnd();
        }

        private string GetCSRF(string HTML)
        {
            Regex Regex = new Regex(@"csrf_token"":""(.*)""},""");
            return Regex.Match(HTML).Groups[1].ToString();
        }

        private string GetHTML(string url = "https://instagram.com/")
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            WebHeaderCollection getHeaders = myRequest.Headers;
            myRequest.Method = "GET";
            myRequest.CookieContainer = Cookies;
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return result;
        }

        private string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2");
            return sbinary;
        }

        private string HMAC(string String, string Key)
        {
            var keyByte = encoding.GetBytes(Key);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(encoding.GetBytes(String));
                return ByteToString(hmacsha256.Hash).ToLower();
            }
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
        }

        private string EncodeUrl(string Url)
        {
            return System.Uri.EscapeDataString(Url);
        }
    }
}
