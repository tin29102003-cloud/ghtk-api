using ClientAuthertication;
using Microsoft.AspNetCore.Authentication;

namespace Ghtk.Api.AuthenticationHanler
{
    public class RemoteAuthenticationHanler : IClientSourceAuthenticationHandler
    {
        private static HttpClient _httpClient = new();
        private string authenticantionServiceUrl;
        public RemoteAuthenticationHanler(string authenticantionServiceUrl) {
        
        this.authenticantionServiceUrl = authenticantionServiceUrl; 
        }
        public bool Validate(string clientSource)
        {
            if(string.IsNullOrEmpty(clientSource))
            {
                return false;
            }
            var response = _httpClient.GetAsync($"{authenticantionServiceUrl}/api/clientsource/{clientSource}").Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
