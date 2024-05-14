using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;

namespace SocialDeductionGame.Roles;

public class Godfather : Role, IRoleNightAction
{
    public Godfather() 
    {
        Name = "Godfather";
        IsTown = false;
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //Godfather rules over the mafia
        //Godfather determines who the mafia should kill
        //The Mafioso caries out the godfathers will
        //If the Mafioso is dead or disrupted the godfather kills the target


        //Assuming that the active world for any mafia class is only the worlds where they know the roles of each other
        List<World> worldList = new List<World>();

        //Finding max possibility world
        int Min = Int32.MaxValue;
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.IsActive == true) )
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
            foreach (PossiblePlayer possiblePlayer in possibleWorld.PossiblePlayers.Where(possiblePlayer => possiblePlayer.IsAlive == true && possiblePlayer.PossibleRole.Name == "Sheriff")) 
            {
                sheriffAlive = true;
            }
            if(sheriffAlive) 
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

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.PossibleRole.Name == "Sheriff" && p.IsAlive == true))
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
        if (selectedPlayer != null)
        {
            Player target = new Player(selectedPlayer.ActualPlayer.Id, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "Godfather", target);

            actions.Add(action);

        }
        else
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}