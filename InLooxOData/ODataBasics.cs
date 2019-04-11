using Default;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InLooxOData
{
    public static class ODataBasics
    {
        public class TokenResponse
        {
            [JsonProperty(PropertyName = "error")]
            public string Error { get; private set; }

            [JsonProperty(PropertyName = "error_description")]
            public string ErrorDescription { get; private set; }

            [JsonProperty(PropertyName = "access_token")]
            public string AccessToken { get; private set; }

            [JsonProperty(PropertyName = "token_type")]
            public string TokenType { get; private set; }

            [JsonProperty(PropertyName = "expires_in")]
            public int ExpiresIn { get; private set; }

            public TokenAccount[] GetAccounts()
            {
                var accountsText = ErrorDescription;
                var entries = accountsText.Split('#');
                var accounts = entries.Select(k =>
                {
                    var splitted = k.Split('|');
                    return new TokenAccount(splitted[1], Guid.Parse(splitted[0]));
                }).ToArray();

                return accounts;
            }

            public class TokenAccount
            {
                public TokenAccount(string name, Guid id)
                {
                    Name = name;
                    Id = id;
                }

                public string Name { get; }
                public Guid Id { get; }
            }
        }

        public static Container GetInLooxContext(Uri odataEndPoint, string bearerToken)
        {
            var context = new Container(odataEndPoint);
            context.SendingRequest2 += (sender, eventArgs) =>
            {
                eventArgs.RequestMessage.SetHeader("Authorization", "bearer " + bearerToken);
            };

            context.ReceivingResponse += (sender, e) =>
            {
                // setup location header for odata 4
                var header = e.ResponseMessage.Headers as Dictionary<string, string>;
                header.Add("Location", odataEndPoint.ToString());
            };

            return context;
        }

        public static async Task<TokenResponse> GetToken(Uri endPoint, string username, string password, Guid? clientId = null)
        {
            var tokenUrl = new Uri(endPoint, "oauth2/token");

            var values = new Dictionary<string, string>{
                { "username", username },
                { "password", password },
                {"grant_type","password" },
            };

            if (clientId != null)
                values.Add("client_id", clientId.ToString());

            var content = new FormUrlEncodedContent(values);

            var client = new HttpClient();
            var response = await client.PostAsync(tokenUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TokenResponse>(responseString);
        }

        public static DataServiceCollection<T> GetDSCollection<T>(IEnumerable<T> query)
        {
            var name = typeof(T).Name.ToLower().Split('.').Last();
            return new DataServiceCollection<T>(query, TrackingMode.AutoChangeTracking, name, null, null);
        }

        public static DataServiceCollection<T> GetDSCollection<T>(Container ctx)
        {
            var name = typeof(T).Name.ToLower().Split('.').Last();
            return new DataServiceCollection<T>(ctx, name, null, null);
        }
    }
}
