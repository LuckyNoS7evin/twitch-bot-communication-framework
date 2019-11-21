using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class BotLinkAcceptModel
    {
        [Required]
        public string ChannelId { get; set; }
        [Required]
        public string BotId { get; set; }
        [Required]
        public bool Accepted {get;set;}
    }
}
