using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApplication
{
    public class GraphService : IGraphService
    {
        private readonly IMemoryCache _memoryCache;
        private SessionCacheService _sessionCache;
        private readonly IOptions<AzureAdOptions> _options;
        private GraphServiceClient _graphClient = null;

        private readonly string contentType = "application/x-www-form-urlencoded";
        // Don't use password grant in your apps. Only use for legacy solutions and automated testing.
        private readonly string grantType = "password";
        private readonly string tokenEndpoint = "https://login.microsoftonline.com/common/oauth2/token";
        private readonly string resourceId = "https%3A%2F%2Fgraph.microsoft.com%2F";
        private static System.DateTimeOffset expiration;

        private static string accessToken = null;
        private static string tokenForUser = null;

        public GraphService(IOptions<AzureAdOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public GraphService(IMemoryCache memoryCache, IOptions<AzureAdOptions> options)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var tenantId = options.Value.TenantId;
            _sessionCache = new SessionCacheService(tenantId, _memoryCache);
        }

        // Get an authenticated Microsoft Graph Service client.
        public GraphServiceClient GetAuthenticatedClient()
        {
            _graphClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    // This sample passes the tenant ID to the sample auth provider to use as a cache key.
                    string accessToken = await GetApplicationAccessTokenAsync(_options.Value.TenantId);

                    // Append the access token to the request.
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }));
            return _graphClient;
        }

        // Gets an access token. First tries to get the access token from the token cache.
        // This sample uses a client secret to authenticate.
        private async Task<string> GetApplicationAccessTokenAsync(string tenantId)
        {
            JObject jResult = null;
            // TODO: Move to Azure Key Vault
            #region Auth Setup
            String urlParameters = String.Format(
            "grant_type={0}&resource={1}&client_id={2}&username={3}&password={4}",
            grantType,
            resourceId,
            _options.Value.MailClientId,
            _options.Value.MailUserName,
            System.Uri.EscapeDataString(_options.Value.MailPassword)
            //The password should be sent encoded, 
            //otherwise it fails (if it contains special chars like # or &)
            ); 
            #endregion

            HttpClient client = new HttpClient();
            var createBody = new StringContent(urlParameters, System.Text.Encoding.UTF8, contentType);

            HttpResponseMessage response = await client.PostAsync(tokenEndpoint, createBody);

            if (response.IsSuccessStatusCode)
            {
                Task<string> responseTask = response.Content.ReadAsStringAsync();
                responseTask.Wait();
                string responseContent = responseTask.Result;
                jResult = JObject.Parse(responseContent);
            }
            else
            {
                string responseTask = await response.Content.ReadAsStringAsync();
                throw new Exception(responseTask);
            }
            accessToken = (string)jResult["access_token"];

            if (!String.IsNullOrEmpty(accessToken))
            {
                //Set AuthenticationHelper values so that the regular MSAL auth flow won't be triggered.
                tokenForUser = accessToken;
                expiration = DateTimeOffset.UtcNow.AddHours(5);
            }

            return accessToken;
        }
    }
}
