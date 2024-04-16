using System.Security.Cryptography;

namespace SocialDeductionGame.Roles;

public class Villager : Role
{
    public Villager()
    {
        Name = "Villager";
        IsTown = true;
    }
}