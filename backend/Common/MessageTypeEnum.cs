using System.ComponentModel.DataAnnotations;

namespace backend.Common
{
    public enum MessageType
    {
        PRERAID,
        PRERAID_CANCELLED,
        MENTION,
        TEAM_MENTION,
        BAN,
        TIMEOUT,
        GENERAL
    }
}
