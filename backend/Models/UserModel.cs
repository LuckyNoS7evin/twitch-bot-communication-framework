using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string Secret {get;set;}
    }
}
