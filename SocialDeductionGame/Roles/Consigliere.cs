using SocialDeductionGame.Worlds;
using Action = SocialDeductionGame.Actions.Action;

namespace SocialDeductionGame.Roles;

public class Consigliere : Role, IRoleNightAction
{
    public Consigliere()
    {
        Name = "Consigliere";
        IsOnVillagerTeam = false;
        checkedPlayers = new List<string>();
    }

    public void PerformNightAction(Player player, List<Action> actions)
    {
        //The Consigliere is the mafia investigative role.
        //The consigliere functions just like the sheriff


        //Assuming that the active world for any mafia class is only the worlds where they know the roles of each other
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

        //Looking checking if sheriff is alive
        foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.isActive == true && possibleWorld.PossibleScore == Max))
        {

            bool sheriffAlive = false;
            foreach (PossiblePlayer possiblePlayer in possibleWorld.PossiblePlayer.Where(possiblePlayer => possiblePlayer.IsAlive == true && possiblePlayer.PossibleRole is Sheriff))
            {
                if (!player.Role.checkedPlayers.Contains(possiblePlayer.ActualPlayer.Name)) { sheriffAlive = true; }
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

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayer.Where(p => p.PossibleRole is Sheriff && p.IsAlive == true))
            {
                //Selecting the last sheriff (if there is more than one) 
                //TODO: Make it such that the sheriff that is chosen is the sheriff that has the most amount of marks
                selectedPlayer = p;
            }
        }

        else
        {
            foreach (World possibleWorld in player.PossibleWorlds.Where(possibleWorld => possibleWorld.isActive == true && possibleWorld.PossibleScore == Max))
            {
                worldList.Add(possibleWorld);
            };
            //Selecting a world at random
            var random = new Random();
            int index = random.Next(worldList.Count);

            World SelectedWorld = worldList[index];
            List<PossiblePlayer> selectedPlayers = new List<PossiblePlayer>();

            foreach (PossiblePlayer p in SelectedWorld.PossiblePlayer.Where(p => p.PossibleRole.IsOnVillagerTeam == true && p.IsAlive == true && !player.Role.checkedPlayers.Contains(p.ActualPlayer.Name)))
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
                    if (selectedPlayers.Count == 1) { Console.WriteLine("PLAYER: " + selectedPlayer.ActualPlayer.Name + " could only target self"); }
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
            Player target = new Player(selectedPlayer.ActualPlayer.Name, selectedPlayer.ActualPlayer.Role);

            Action action = new Action(player, "Consigliere", target);

            actions.Add(action);

            player.Role.checkedPlayers.Add(target.Name);

        }
        else
        {
            Console.WriteLine("ERROR, target player not found for player: " + player.Name + " With the role: " + player.Role);
        }
    }
}