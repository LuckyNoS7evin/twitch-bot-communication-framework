
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace backend.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageType
    {
        [EnumMember(Value = "PRERAID")]
        PRERAID,
        [EnumMember(Value = "PRERAID_CANCELLED")]
        PRERAID_CANCELLED,
        [EnumMember(Value = "MENTION")]
        MENTION,
        [EnumMember(Value = "TEAM_MENTION")]
        TEAM_MENTION,
        [EnumMember(Value = "BAN")]
        BAN,
        [EnumMember(Value = "TIMEOUT")]
        TIMEOUT,
        [EnumMember(Value = "GENERAL")]
        GENERAL
    }
}
