using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class BotLinkRequestModel
    {
        [Required]
        public string ChannelId { get; set; }
        [Required]
        public string BotId { get; set; }
    }
}
