using backend.Services.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TwitchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public TwitchService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<Dictionary<string,string>> GetUserDisplayNames(List<string> userIds)
        {
            var users = string.Join("&id=", userIds);

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/users?id={users}");
            request.Headers.Add("Client-ID", _configuration["Authorization:ClientId"]);

            var result = await _httpClient.SendAsync(request);
            if (result.IsSuccessStatusCode)
            {
                var clipResult = JsonConvert.DeserializeObject<GetUsersResponse>( await result.Content.ReadAsStringAsync());
                return clipResult.Users.Select(x => x).ToDictionary(x => x.Id, x => x.DisplayName);
            }

            return null;

        }
    }
}
