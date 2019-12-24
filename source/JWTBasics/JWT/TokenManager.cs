using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Security.Cryptography;

using JWTBasics.Models;

namespace JWTBasics.JWT
{
    public class TokenManager
    {
        private const string _secret = "150e0ba8d939f84397c0f5ded561db65b85bc01fc6a6c2655a37dac88accc9ff99babfe188e2701f7f7dfce5896cd4e5b9398b61a744a8d806aacdaa34e427bd";
        public string CreateToken(User user)
        {
            TokenHeader header = new TokenHeader() { alg = "HS256", typ = "JWT" };
            TokenPayload payload = new TokenPayload()
            { user = user.Name, privilege = user.Privilege };

            string headerJson = JsonConvert.SerializeObject(header);
            byte[] headerJsonBytes = Encoding.UTF8.GetBytes(headerJson);

            string payloadJson = JsonConvert.SerializeObject(payload);
            byte[] payloadJsonBytes = Encoding.UTF8.GetBytes(payloadJson);

            string headerBase64Encoded = WebEncoders.Base64UrlEncode(headerJsonBytes);
            string payloadBase64Encoded = WebEncoders.Base64UrlEncode(payloadJsonBytes);

            string signature = "";

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
            {
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(
                    String.Format("{0}.{1}", headerBase64Encoded, payloadBase64Encoded)));
                signature = WebEncoders.Base64UrlEncode(signatureBytes);
            }

            return (signature.Length > 0)
                ? String.Format("{0}.{1}.{2}", headerBase64Encoded, payloadBase64Encoded, signature)
                : String.Empty;
        }

        public User GetUser(string token)
        {
            string[] parts = token.Split('.');
            if(parts.Length == 3)
            {
                string signature = "";
                using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
                {
                    byte[] signatureBytes = hmac.ComputeHash(
                        Encoding.UTF8.GetBytes(String.Format("{0}.{1}", parts[0], parts[1])));
                    signature = WebEncoders.Base64UrlEncode(signatureBytes);
                }

                if(signature == parts[2])
                {
                    string headerJson = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(parts[0]));
                    string payloadJson = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(parts[1]));

                    TokenHeader header = JsonConvert.DeserializeObject<TokenHeader>(headerJson);
                    TokenPayload payload = JsonConvert.DeserializeObject<TokenPayload>(payloadJson);

                    return new User() { Name = payload.user, Privilege = payload.privilege };
                }
            }
            return new User();
        }
    }

    public class TokenHeader
    {
        public string alg { get; set; }
        public string typ { get; set; }
    }

    public class TokenPayload
    {
        public string user { get; set; }
        public string privilege { get; set; }
    }
}
