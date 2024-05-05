using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;
using SocialDeductionGame.Actions;

namespace SocialDeductionGame.Roles;

public class Vigilante : Role, IRoleNightAction
{
    public Vigilante()
    {
        Name = "Vigilante";
        IsTown = true;
        forceAction = false;
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //Vigilante is the primary killer for the town.
        //The vigilantes powers are however very limited,
        //If the vigilante kills any town member, he will commit suicide the next night
        //This will decrease the amount of town members by 2
        //Because of this, the vigilante has to be very sure on his killing


        //If the vigilante has shot a villager, he is forced to perfom an action
        if(player.Role.forceAction)
        {
            //What this action entails doesn't matter
            Action action = new Action(player, "Vigilante", player);
            return;
        }

        //first
        List<World> worldList = new List<World>();
        
        var sortedWorlds = player.PossibleWorlds
            .Where(world => world.IsActive)
            .OrderByDescending(world => world.Marks) 
            .ToList(); 

        // //Finding max possibility world
        // int Max = Int32.MinValue;
        // foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true))
        // {
        //     if (possibleWorld.Marks > Max)
        //     {
        //         Max = possibleWorld.Marks;
        //     }
        // };
        //
        // //Finding world with second highest possiblity
        // int SecondMax = 0;
        // foreach(World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true && possibleWorld.Marks < Max))
        // {
        //     if (possibleWorld.Marks > SecondMax)
        //     {
        //         SecondMax = possibleWorld.Marks;
        //     }
        // }
        
        //Vigilante has to be certain on his action, as it can mean life or death
        //How certain has the vigilante be for him to shoot?
        //2 indicates that he has to be twice as certain on these worlds than the rest.
        if (sortedWorlds[0].Marks == 0 || sortedWorlds[0].Marks == sortedWorlds[1].Marks || sortedWorlds[0].Marks < sortedWorlds[1].Marks * 2)
        {
            Console.WriteLine("The Vigilante sleeps tonight");
            return;
        }
        
        worldList = player.PossibleWorlds
            .Where(possibleWorld => possibleWorld.IsActive && possibleWorld.Marks == sortedWorlds[0].Marks)
            .ToList(); // Materialize the list

        PossiblePlayer selectedPlayer = null;

        //Selecting a world at random from list
        bool candidatesFound = false;
        List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();
        while (!candidatesFound)
        {
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];
            
            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole.IsTown == false && p.IsAlive == true))
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
            Player target = new Player(selectedPlayer.ActualPlayer.Id, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "Vigilante", target);

            actions.Add(action);

        }
        else
        {
            Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}