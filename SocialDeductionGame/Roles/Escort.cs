namespace SocialDeductionGame.Roles;
using Action = SocialDeductionGame.Actions.Action;
using SocialDeductionGame.Worlds;

public class Escort : Role, IRoleNightAction
{
    public Escort() 
    {
        Name = "Escort";
        IsOnVillagerTeam = true;
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
        var random = new Random();
        int index = random.Next(worldList.Count);

        World SelectedWorld = worldList[index];
        List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();

        //not targetting consorts, as they cannot be roleblocked
        foreach (PossiblePlayer p in SelectedWorld.PossiblePlayer.Where(p => p.PossibleRole.IsOnVillagerTeam == false && p.PossibleRole is not Consort))
        {
            selectedPlayers.Add(p);
        }

        //Selecting a random mafia role
        while (selectedPlayer == null) {
            var randomVil = new Random();
            int indexVil = random.Next(selectedPlayers.Count);

            selectedPlayer = selectedPlayers[indexVil];
            if(selectedPlayer.ActualPlayer.Name == player.Name) 
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

            Action action = new Action(player, "RoleBlock", target);

            actions.Add(action);

        }
        else
        {
            Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }

}