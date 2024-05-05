namespace SocialDeductionGame.Logic;

public class ProbabilityItem
{
    public string Key { get; set; }
    public double Chance { get; set; }
    public Func<double, bool> Function { get; set; }

    public ProbabilityItem(string key, double chance, Func<double, bool> function)
    {
        Key = key;
        Chance = chance;
        Function = function;
    }
}