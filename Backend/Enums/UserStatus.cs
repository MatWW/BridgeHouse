using System.Text.Json.Serialization;

namespace Backend.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserStatus
{
    IN_LOBBY,
    INVITED,
    AT_TABLE,
    IN_GAME
}
