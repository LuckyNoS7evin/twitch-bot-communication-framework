namespace backend.Models
{
    public class ChannelBotLinkModel
    {
        public string ChannelId { get; set; }
        public string ChannelDisplayName { get; set; }
        public bool ChannelAccepted { get; set; }
        public string BotId { get; set; }
        public string BotDisplayName { get; set; }
        public bool BotAccepted { get; set; }
        public bool PendingResponse { get; set; }
    }
}
