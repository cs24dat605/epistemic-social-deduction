using System.Collections.Concurrent;

namespace SocialDeductionGame.Roles;
using Action = SocialDeductionGame.Actions.Action;
using SocialDeductionGame.Worlds;

public class Escort : Role, IRoleNightAction
{
    public Escort() 
    {
        Name = "Escort";
        IsTown = true;
    }

    public void PerformNightAction(Player player, ConcurrentBag<Action> actions)
    {
        //Sheriff is the primary information gatherer for the town
        //Sheriff chooses to see one persons roles

        //first
        List<World> worldList = new List<World>();

        //Finding max possibility world
        int Min = Int32.MaxValue;
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true))
        {
            if (possibleWorld.Marks < Min)
            {
                Min = possibleWorld.Marks;
            }
        };

        //Selecting all worlds with max possibility
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true && possibleWorld.Marks == Min))
        {
            worldList.Add(possibleWorld);
        };

        PossiblePlayer selectedPlayer = null;


        //Selecting a world at random from list
        bool candidatesFound = false;
        List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();
        int i = 0;
        while (!candidatesFound)
        {
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];


            //not targetting consorts, as they cannot be roleblocked
            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole.IsTown == false && p.PossibleRole.Name != "Consort" && p.IsAlive == true))
            {
                selectedPlayers.Add(p);
            }
            if (selectedPlayers.Count > 0)
            {
                candidatesFound = true;
            }
            else
            {
                i++;
            }
            if (worldList.Count < i)
            {
                //Makes it such that there are no infinite runs
                return;
            }
        }



        //Selecting a random mafia role
        while (selectedPlayer == null) {
            if (selectedPlayers.Count == 0)
            {
                break;
            }
            var randomVil = new Random();
            int indexVil = randomVil.Next(selectedPlayers.Count);

            selectedPlayer = selectedPlayers[indexVil];
            if(selectedPlayer.ActualPlayer.Name == player.Name) 
            {
                if (selectedPlayers.Count == 1)
                {
                    if (Game.Instance.shouldPrint)
                        Console.WriteLine("PLAYER: " + selectedPlayer.ActualPlayer.Name + " could only target self");
                }
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
            Player target = new Player(selectedPlayer.ActualPlayer.Id, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "RoleBlock", target);

            actions.Add(action);

        }
        else
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }

}