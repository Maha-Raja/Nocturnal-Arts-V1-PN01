using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Zoom
{
    public class ZoomMeeting
    {
        public Nullable<Int64> id { get; set; }
        public string topic { get; set; }
        public int type { get; set; }
        public DateTime start_time { get; set; }
        public int duration { get; set; }
        public string start_url { get; set; }
        public string password { get; set; }
        public ZoomMeetingSettings settings { get; set; }

        public static long ToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000;
            return epoch;
        }

        public static string GenerateToken(string apiKey, string apiSecret, string meetingNumber, string ts, string role)
        {
            char[] padding = { '=' };
            string message = String.Format("{0}{1}{2}{3}", apiKey, meetingNumber, ts, role);
            apiSecret = apiSecret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(apiSecret);
            byte[] messageBytesTest = encoding.GetBytes(message);
            string msgHashPreHmac = System.Convert.ToBase64String(messageBytesTest);
            byte[] messageBytes = encoding.GetBytes(msgHashPreHmac);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string msgHash = System.Convert.ToBase64String(hashmessage);
                string token = String.Format("{0}.{1}.{2}.{3}.{4}", apiKey, meetingNumber, ts, role, msgHash);
                var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
                return System.Convert.ToBase64String(tokenBytes).TrimEnd(padding);
            }
        }

        public static string ZoomAPIToken(string ZoomApiKey, string ZoomApiSecret)
        {
            DateTime Expiry = DateTime.UtcNow.AddMinutes(50);
            string ApiKey = ZoomApiKey;
            string ApiSecret = ZoomApiSecret;

            int ts = (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            // Create Security key  using private key above:
            // note that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecret));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Finally create a Token
            var header = new JwtHeader(credentials);

            //Zoom Required Payload
            var payload = new JwtPayload
            {
                { "iss", ApiKey},
                { "exp", ts },
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);
            //string Token = tokenString;
            return tokenString;
        }

        public static ZoomMeeting CreateMeeting(string ZoomApiKey, string ZoomApiSecret, string ZoomUserId, string name, DateTime startTime, int duration)
        {

            var reqObj = new ZoomMeeting()
            {
                topic = name,
                start_time = startTime,
                duration = duration*60,
                type = 2,
                settings = new ZoomMeetingSettings()
                {
                    host_video = true,
                    participant_video = true,
                    join_before_host = false,
                    mute_upon_entry = true,
                    watermark = false,
                    use_pmi = false,
                    approval_type = 1,
                    audio = "both",
                    enforce_login = false
                }
            };

            var token = ZoomMeeting.ZoomAPIToken(ZoomApiKey, ZoomApiSecret);
            //Create new Request
            string BaseUrl = String.Format("https://api.zoom.us/v2/users/{0}/meetings", ZoomUserId);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(BaseUrl);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.ContentType = "application/json;";
            myHttpWebRequest.Accept = "application/json;";
            myHttpWebRequest.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            using (var streamWriter = new StreamWriter(myHttpWebRequest.GetRequestStream()))
            {

                string json = JsonConvert.SerializeObject(reqObj);
                streamWriter.Write(json);
            }

            ZoomMeeting respObj;
            //Get the associated response for the above request
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            using (StreamReader MyStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), true))
            {
                respObj = JsonConvert.DeserializeObject<ZoomMeeting>(MyStreamReader.ReadToEnd());
            }

            myHttpWebResponse.Close();
            myHttpWebResponse.Dispose();

            return respObj;
        }

        public static List<ZoomParticipant> GetMeetingParticipantList(string MeetingId, string ZoomApiKey, string ZoomApiSecret)
        {
            var token = ZoomMeeting.ZoomAPIToken(ZoomApiKey, ZoomApiSecret);
            //Create new Request
            string BaseUrl = String.Format("https://api.zoom.us/v2/metrics/meetings/{0}/participants", MeetingId);
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(BaseUrl);
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.ContentType = "application/json;";
            myHttpWebRequest.Accept = "application/json;";
            myHttpWebRequest.Headers.Add("Authorization", String.Format("Bearer {0}", token));

            List<ZoomParticipant> respObj;
            //Get the associated response for the above request
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            using (StreamReader MyStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), true))
            {
                var jsonStr = MyStreamReader.ReadToEnd();
                var objectType = new
                {
                    page_count = 1,
                    page_size = 30,
                    total_records = 2,
                    next_page_token = "",
                    participants = new List<ZoomParticipant>()
                };
                var apiResponse = JsonConvert.DeserializeAnonymousType(jsonStr, objectType);
                respObj = apiResponse.participants;
            }

            myHttpWebResponse.Close();
            myHttpWebResponse.Dispose();

            return respObj;
        }
    }

    public class ZoomMeetingSettings
    {
        public bool host_video { get; set; }
        public bool participant_video { get; set; }
        public bool join_before_host { get; set; }
        public bool mute_upon_entry { get; set; }
        public bool watermark { get; set; }
        public bool use_pmi { get; set; }
        public int approval_type { get; set; }
        public string audio { get; set; }
        public bool enforce_login { get; set; }
    }
}
