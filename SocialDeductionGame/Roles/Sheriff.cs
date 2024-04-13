using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;

namespace SocialDeductionGame.Roles;

public class Sheriff : Role, IRoleNightAction
{
    public Sheriff()
    {
        Name = "Sheriff";
        IsOnVillagerTeam = true;
        checkedPlayers = new List<string>();
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //Sheriff is the primary information gatherer for the town
        //Sheriff chooses to see one persons roles

        //first
        List<World> worldList = new List<World>();

        //Finding max possibility world
        int Max = 0;
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.isActive == true))
        {
            if (possibleWorld.PossibleScore > Max)
            {
                Max = possibleWorld.PossibleScore;
            }
        };

        //Selecting all worlds with max possibility
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.isActive == true && possibleWorld.PossibleScore == Max))
        {
                worldList.Add(possibleWorld);
        };

        PossiblePlayer selectedPlayer = null;


        //Selecting a world at random from list
        bool candidatesFound = false;
        List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();
        while (!candidatesFound)
        {
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];
            

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayer.Where(p => p.PossibleRole.IsOnVillagerTeam == false && p.IsAlive == true))
            {
                selectedPlayers.Add(p);
            }
            if (selectedPlayers.Count > 0)
            {
                candidatesFound = true;
            }
        }

        //Selecting a random mafia role
        while (selectedPlayer == null)
        {
            if (selectedPlayers.Count == 0)
            {
                break;
            }
            var randomVil = new Random();
            int indexVil = randomVil.Next(selectedPlayers.Count);

            selectedPlayer = selectedPlayers[indexVil];
            if (selectedPlayer.ActualPlayer.Name == player.Name)
            {
                if (selectedPlayers.Count == 1) { Console.WriteLine("PLAYER: " + selectedPlayer.ActualPlayer.Name + " could only target self"); }
                else
                {
                    {
                        selectedPlayer = null;
                    }
                }
            }
        }


        //Announce Selected target to action handler
        if (selectedPlayer != null)
        {
            Player target = new Player(selectedPlayer.ActualPlayer.Name, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "Sheriff", target);

            actions.Add(action);

        }
        else
        {
            Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}