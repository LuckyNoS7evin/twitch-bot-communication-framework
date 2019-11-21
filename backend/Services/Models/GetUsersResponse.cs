using Newtonsoft.Json;

namespace backend.Services.Models
{
        public class GetUsersResponse
        {
            [JsonProperty(PropertyName = "data")]
            public User[] Users { get; protected set; }
        }
}
