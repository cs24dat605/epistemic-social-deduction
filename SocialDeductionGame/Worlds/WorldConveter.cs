using System.Text.Json;
using System.Text.Json.Serialization;
using SocialDeductionGame.Communication;

namespace SocialDeductionGame.Worlds;

public class WorldConverter : JsonConverter<World>
{
    public override World Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of object");
        }

        List<PossiblePlayer> possiblePlayers = new List<PossiblePlayer>();
        bool isActive = true; // Default to true
        int marks = 0; // Default to 0
        List<Message> accusations = new List<Message>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected property name");
            }

            string propertyName = reader.GetString();

            reader.Read();

            switch (propertyName)
            {
                case "PossiblePlayers":
                    possiblePlayers = JsonSerializer.Deserialize<List<PossiblePlayer>>(ref reader, options);
                    break;
                case "IsActive":
                    isActive = reader.GetBoolean();
                    break;
                case "Marks":
                    reader.GetInt32();
                    marks = 0;
                    break;
                case "Accusations":
                    JsonSerializer.Deserialize<List<Message>>(ref reader, options);
                    accusations = new List<Message>();
                    break;
                default:
                    throw new JsonException($"Unexpected property: {propertyName}");
            }
        }

        return new World(possiblePlayers) 
        {
            IsActive = isActive,
            Marks = marks,
            Accusations = accusations
        };
    }

    public override void Write(Utf8JsonWriter writer, World world, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("PossiblePlayers");
        JsonSerializer.Serialize(writer, world.PossiblePlayers, options);
        writer.WriteBoolean("IsActive", world.IsActive);
        writer.WriteNumber("Marks", world.Marks);
        writer.WritePropertyName("Accusations");
        JsonSerializer.Serialize(writer, world.Accusations, options);
        writer.WriteEndObject();
    }
}