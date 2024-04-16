using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SocialDeductionGame.Roles;

public class RoleConverter : JsonConverter<Role>
{
    public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }
        
        // Read the "Name" property to determine the specific Role type
        reader.Read(); // Move to "Name" 
        if (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Name")
        {
            throw new JsonException("Expected property 'Name' to determine the Role type.");
        }
        reader.Read();
        string roleName = reader.GetString();

        Role role;
        switch (roleName)
        {
            case "Villager":
                ReadObject(ref reader);
                role = new Villager();
                break;
            case "Sheriff":
                ReadObject(ref reader);
                role = new Sheriff();
                break;
            case "Godfather":
                ReadObject(ref reader);
                role = new Godfather();
                break;
            case "Escort":
                ReadObject(ref reader);
                role = new Escort();
                break;
            case "Consigliere":
                ReadObject(ref reader);
                role = new Consigliere();
                break;
            case "Consort":
                ReadObject(ref reader);
                role = new Consort();
                break;
            case "Mafioso":
                ReadObject(ref reader);
                role = new Mafioso();
                break;
            case "Vigilante":
                ReadObject(ref reader);
                role = new Vigilante();
                break;
            case "Veteran":
                ReadObject(ref reader);
                role = new Veteran();
                break;
            default:
                throw new JsonException($"Unknown role type: {roleName}");
        }

        return role;
    }

    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
    
    private void ReadObject(ref Utf8JsonReader reader)
    {
        while (reader.TokenType != JsonTokenType.EndObject)
        {
            reader.Read();
        }
    }
}
