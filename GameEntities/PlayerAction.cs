using System.Text.Json.Serialization;

namespace Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlayerAction
{
    Check,
    Bet,
    Call,
    Fold,
    AllIn,
    None
}
