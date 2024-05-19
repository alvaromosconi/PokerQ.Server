using System.Text.Json.Serialization;

namespace Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Stage
{
    Initial = 0,
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown
}
