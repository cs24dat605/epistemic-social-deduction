using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;

namespace SocialDeductionGame.Roles;

public class Veteran : Role, IRoleNightAction
{
    public Veteran()
    {
        Name = "Veteran";
        IsTown = true;
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //Veteran is a schizofrenic maniac, that will kill anyone that visits him.
        //This includes both team memebers and enemies
        //The Veteran should be alert when he thinks he is most likely to be targetted

        //first
        List<World> worldList = new List<World>();

        //Finding max possibility world
        int Max = Int32.MinValue;
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true))
        {
            if (possibleWorld.Marks > Max)
            {
                Max = possibleWorld.Marks;
            }
        };

        //Selecting all worlds with max possibility
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true && possibleWorld.Marks == Max))
        {
                worldList.Add(possibleWorld);
        };

        //Selecting a world at random from list
        var random = new Random();
        int index = random.Next(worldList.Count);

        World SelectedWorld = worldList[index];

        int playersAlive = 0;
        int mafiaAlive = 0;
        foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.IsAlive == true))
        {
            if(p.PossibleRole.IsTown == false)
            {
                mafiaAlive++;
            }
            playersAlive++;
        }

        bool alert = false;

        //Veteran has two conditions for when he wants to be alert.
        //Either when the random odds for him to get selected get "sufficiently high"
        //That is currently set as when the amount of players is less than or equal to the amount of mafia times 2
        //Or when the world that the Veteran belives most in, is a world where he is a sheriff.
        foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers) 
        { 
            if(p.ActualPlayer.Name == player.Name && p.PossibleRole.Name == "Sheriff")
            {
                alert = true;
            }
        }

        if (mafiaAlive * 2 >= playersAlive)
        {
            alert = true;
        }

        //Announce Selected target to action handler
        if (alert)
        {

            Action action = new Action(player, "Veteran", player);

            actions.Add(action);

        }
        else
        {
            Console.WriteLine("Veteran chose not to go alert this night");
        }
    }
}