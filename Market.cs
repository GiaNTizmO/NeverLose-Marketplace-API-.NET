using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NeverLose_Market_Api
{
    public class NLMarket
    {
        private string _apikey;
        private int _uid;
        private WebClient wc = new WebClient();

        public NLMarket()
        {
            throw new Exception("Calling constructor without arguments is forbidden!");
        }

        public NLMarket(string apiKey, int userid)
        {
            _apikey = apiKey;
            _uid = userid;
        }

        public string GenerateSignature(JObject j, string secret)
        {
            j = new JObject(
                j.Properties().OrderBy(p => p.Name)
           );
            string str_to_hash = String.Empty;
            foreach (var xj in j)
            {
                if (xj.Key == "amount") //fucking ghetto solution, because converting JToken to .net object changes dot (.) to comma (,) // need fix this
                    str_to_hash += xj.Key + xj.Value.ToString().Replace(',', '.');
                else
                    str_to_hash += xj.Key + xj.Value;
            }
            str_to_hash = str_to_hash + secret;
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.Default.GetBytes(str_to_hash))).Replace("-", "").ToLowerInvariant();
        }

        public bool ValidateSignature(JObject j, string secret)
        {
            JToken nl_sig;
            j.TryGetValue("signature", out nl_sig);
            j.Remove("signature");
            string our_sig = GenerateSignature(j, secret);
            return nl_sig.ToString() == our_sig;
        }
    }
}