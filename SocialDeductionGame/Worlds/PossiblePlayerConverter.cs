using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Worlds;

public class PossiblePlayerConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(PossiblePlayer);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        PossiblePlayer possiblePlayer = (PossiblePlayer)value;
        Role role = possiblePlayer.PossibleRole;

        int playerIndex = Game.Instance.Players.FindIndex(p => p.Name == possiblePlayer.Name);
        
        writer.WriteStartObject();
        writer.WritePropertyName("AP"); // ActualPlayer
        writer.WriteValue(playerIndex);

        List<Role> roleList = Game.Instance.GameConfig.GetRoles();
        int roleIndex = roleList.FindIndex(r => r.Name == role.Name);
        
        writer.WritePropertyName("PR"); // PossibleRole
        writer.WriteValue(roleIndex);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        int pRoleID = (int)jo["PR"]; // PossibleRole
        int aPlayerID = (int)jo["AP"]; // ActualPlayer
        
        List<Role> roleList = Game.Instance.GameConfig.GetRoles();

        Role pRole = roleList[pRoleID];
        Player aPlayer = Game.Instance.Players[aPlayerID];
        
        return new PossiblePlayer(pRole, aPlayer);
    }
}