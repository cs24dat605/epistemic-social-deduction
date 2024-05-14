using System.Collections.Concurrent;
using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;

namespace SocialDeductionGame.Roles;

public class Investigator : Role, IRoleNightAction
{
    public Investigator()
    {
        Name = "Investigator";
        IsTown = true;
        checkedPlayers = new List<string>();
    }

    public void PerformNightAction(Player player, ConcurrentBag<Action> actions)
    {
        //Investigator is the primary information gatherer for the town
        //Investigator chooses to see one persons roles

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
            

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole.IsTown == false && p.IsAlive == true && !player.Role.checkedPlayers.Contains(p.ActualPlayer.Name)))
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
            if(worldList.Count < i)
            {
                //Makes it such that there are no infinite runs
                return;
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

            Action action = new Action(player, "Investigator", target);

            actions.Add(action);

            player.Role.checkedPlayers.Add(target.Name);

        }
        else
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}