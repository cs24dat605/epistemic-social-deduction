using System.Security.Cryptography;

namespace SocialDeductionGame.Roles;

public class MafiaVillager : Role
{
    public MafiaVillager()
    {
        Name = "MafiaVillager";
        IsTown = false;
    }
}