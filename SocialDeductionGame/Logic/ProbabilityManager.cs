namespace SocialDeductionGame.Logic;

public class ProbabilityManager
{
    private static readonly List<ProbabilityItem> _probabilities = [
        new ProbabilityItem("Communicate", 0.8, DefaultRandomResult)
    ];
    
    private static Random _random = new Random();
    
    public static bool ShouldEventOccur(string key)
    {
        var item = _probabilities.FirstOrDefault(p => p.Key == key);

        if (item == null)
            throw new ArgumentException($"Probability key '{key}' not found!");

        // Call the function with the Chance value to get the random result
        return item.Function(item.Chance);
    }
    
    private static bool DefaultRandomResult(double chance)
    {
        return _random.NextDouble() <= chance;
    }
}