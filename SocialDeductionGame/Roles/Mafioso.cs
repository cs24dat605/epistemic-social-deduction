namespace SocialDeductionGame.Roles;
using Action = SocialDeductionGame.Actions.Action;
using SocialDeductionGame.Worlds;

public class Mafioso : Role, IRoleNightAction
{
    public Mafioso() 
    {
        Name = "Mafioso";
        IsTown = false;
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //Mafioso has to do the killing on behalf of the Godfather
        //In case the godfather is killed or otherwise disrupted, the Mafioso has to target a villager in the same manner as the Godfather



        //A carbon copy of the code from the Godfather
        //Assuming that the active world for any mafia class is only the worlds where they know the roles of each other
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

        //Looking checking if sheriff is alive
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true && possibleWorld.Marks == Min))
        {
            bool sheriffAlive = false;
            foreach (PossiblePlayer possiblePlayer in possibleWorld.PossiblePlayers.Where(possiblePlayer => possiblePlayer.IsAlive == true && possiblePlayer.PossibleRole is Sheriff))
            {
                sheriffAlive = true;
            }
            if (sheriffAlive)
            {
                worldList.Add(possibleWorld);
            }
        };

        PossiblePlayer selectedPlayer = null;

        //If the sheriff is alive
        if (worldList.Count != 0)
        {

            //Selecting a world at random
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole is Sheriff && p.IsAlive == true))
            {
                //Selecting the last sheriff (if there is more than one) 
                //TODO: Make it such that the sheriff that is chosen is the sheriff that has the most amount of marks
                selectedPlayer = p;
            }
        }

        else
        {
            foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true && possibleWorld.Marks == Min))
            {
                worldList.Add(possibleWorld);
            };
            //Selecting a world at random
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];
            List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole.IsTown == true && p.IsAlive == true))
            {
                selectedPlayers.Add(p);
            }

            while (selectedPlayer == null)
            {
                if (selectedPlayers.Count == 0)
                {
                    break;
                }
                var randomVil = new Random();
                int indexVil = random.Next(selectedPlayers.Count);

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
        }


        //Announce Selected target to action handler
        //Event role this time is mafioso ofc.
        if (selectedPlayer != null)
        {
            Player target = new Player(selectedPlayer.ActualPlayer.Id, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "Mafioso", target);

            actions.Add(action);

        }
        else
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}