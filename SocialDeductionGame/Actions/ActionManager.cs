using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;

namespace SocialDeductionGame.Actions;

public class ActionManager
{
    public List<Action> Actions { get; set; }
    public bool? missFireByVigilante { get; set; } 
    public ActionManager(List<Action> actions)
    {
        Actions = actions;
    }
    
    public void AddAction(Action action)
    {
        this.Actions.Add(action);
    }

    public void HandleActions(List<Player> players)
    {
        List<Action> actions = new List<Action>();

        List<string> killTargets = new List<string>();
        List<string> doctorProtected = new List<string>();

        foreach (var e in this.Actions)
        {
            actions.Add(e);
        }

        //Kill off Vigilante before anything happens
        if (missFireByVigilante != null || missFireByVigilante == true)
        {
            int targetIndex = 0;
            int ii = 0;
            foreach (var p in players)
            {
                if (p.Role.Name == "Vigilante")
                {
                    targetIndex = ii;
                }
                ii++;
            }
            //Kill target
            // TODO: Players should also update their belives here, since this is a public announcement
            foreach (Player player in players.Where(player => player.IsAlive == true))
            {
                foreach (World world in player.PossibleWorlds)
                {
                    world.PossiblePlayers[targetIndex].IsAlive = false;
                }
            }
            players[targetIndex].IsAlive = false;

            //Remove vigilante action
            for (ii = 0; ii < actions.Count;)
            {
                if (actions[ii].player.Role.Name == "Vigilante")
                {
                    actions.RemoveAt(ii);
                    break;
                }
            }
        }

        //Escort / Consort action
        //Removing actions that are roleblocked
        List<int> ints = new List<int>();
        foreach (var e in actions.Where(e => e.typeOfAction is "RoleBlock"))
        {
            int playerindex = 0;

            foreach (var x in actions)
            {
                //First check = if the target is in the list of actions
                //Second check = if the target is an escort or a consort, then they should not be removed from the list
                //Third check = if the target is a veteran, and the veteran is alert (they are in the list of actions) then the veteran action is not removed
                if (e.target.Name == x.player.Name && !(x.target.Role.Name == "Escort" || x.target.Role.Name == "Consort") && x.typeOfAction != "Veteran")
                {
                    if (!ints.Contains(playerindex)) ints.Add(playerindex);
                }
                playerindex++;
            }
        }
        for (int x = 0; x < ints.Count; x++)
        {
            for (int y = x; y < ints.Count; y++)
            {
                if (ints[y] > ints[x])
                {
                    ints[y]--;
                }
            }
        }
        foreach (int ii in ints)
        {
            //Note that roleblockers may be removed from the actions list
            //this is not a problem as their action has been performed


            actions.RemoveAt(ii);
        }

        //Sheriff action
        //find index of sheriff(s)
        List<int> sheriffIndex = new List<int>();
        int i = 0;
        foreach (var x in players)
        {
            if (x.Role is Sheriff)
            {
                sheriffIndex.Add(i);
            }
            i++;
        }
        foreach (var e in actions.Where(e => e.typeOfAction is "Sheriff"))
        {
            //Find the index of target
            int ii = 0;
            int targetIndex = 0;
            foreach (var x in players)
            {
                if (x.Name == e.target.Name)
                {
                    targetIndex = ii;
                    break;
                }
                ii++;
            }


            //Change worlds that are not in accordance to new information to inactive
            foreach (var x in sheriffIndex)
            {
                if (e.player.Name == players[x].Name) //Double checking
                {
                    foreach (var y in players[x].PossibleWorlds.Where(y => y.IsActive != false))
                    {
                        if (y.PossiblePlayers[targetIndex].PossibleRole.IsTown != players[targetIndex].Role.IsTown)
                        {
                            y.IsActive = false;

                            // TODO: Here it should also update the belifes of the investigator
                        }
                    }
                }
            }
        }

        //Investigator action
        //find index of investigators(s)
        List<int> investigatorIndex = new List<int>();
        i = 0;
        foreach (var x in players)
        {
            if (x.Role is Investigator)
            {
                investigatorIndex.Add(i);
            }
            i++;
        }
        foreach (var e in actions.Where(e => e.typeOfAction is "Investigator"))
        {
            //Find the index of target
            int ii = 0;
            int targetIndex = 0;
            foreach (var x in players)
            {
                if (x.Name == e.target.Name)
                {
                    targetIndex = ii;
                    break;
                }
                ii++;
            }


            List<string> investigatorRoles = new List<string>(); 
            switch (players[targetIndex].Role)
            {
                case Doctor:
                case Escort:
                case Blackmailer:
                    investigatorRoles.Add("Doctor");
                    investigatorRoles.Add("Escort");
                    investigatorRoles.Add("Blackmailer");
                    break;
                case Veteran:
                case Consigliere:
                case Consort:
                    investigatorRoles.Add("Veteran");
                    investigatorRoles.Add("Consigliere");
                    investigatorRoles.Add("Consort");
                    break;
                case Investigator:
                case Sheriff:
                case Godfather:
                    investigatorRoles.Add("Investigator");
                    investigatorRoles.Add("Sheriff");
                    investigatorRoles.Add("Godfather");
                    break;
                case Vigilante:
                case Villager:
                case Mafioso:
                    investigatorRoles.Add("Vigilante");
                    investigatorRoles.Add("Villager");
                    investigatorRoles.Add("Mafioso");
                    break ;
            }

            //Change worlds that are not in accordance to new information to inactive
            foreach (var x in investigatorIndex)
            {
                if (e.player.Name == players[x].Name) //Double checking
                {
                    foreach (var y in players[x].PossibleWorlds.Where(y => y.IsActive != false))
                    {
                        if (!investigatorRoles.Contains(y.PossiblePlayers[targetIndex].PossibleRole.Name))
                        {
                            y.IsActive = false;

                            // TODO: Here it should also update the belifes of the investigator
                        }
                    }
                }
            }
        }

        //Consigliere action
        //find index of Consiglieres
        List<int> consigliereIndex = new List<int>();
        i = 0;
        foreach (var x in players)
        {
            if (x.Role is Consigliere)
            {
                consigliereIndex.Add(i);
            }
            i++;
        }
        foreach (var e in actions.Where(e => e.typeOfAction is "Consigliere"))
        {
            //Find the index of target
            int ii = 0;
            int targetIndex = 0;
            foreach (var x in players)
            {
                if (x.Name == e.target.Name)
                {
                    targetIndex = ii;
                    break;
                }
                ii++;
            }


            //Change worlds that are not in accordance to new information to inactive
            foreach (var x in consigliereIndex)
            {
                if (e.player.Name == players[x].Name) //double checking
                {
                    foreach (var y in players[x].PossibleWorlds.Where(y => y.IsActive != false))
                    {
                        if (y.PossiblePlayers[targetIndex].PossibleRole.Name != players[targetIndex].Role.Name)
                        {
                            y.IsActive = false;

                            // TODO: Here it should also update the belifes of the investigator
                        }
                    }
                }
            }

            // Not used?
            // foreach (var x in players.Where(x => x.Role is Consigliere))
            // {
            //     int active = 0;
            //     int inactive = 0;
            //     foreach (World world in x.PossibleWorlds)
            //     {
            //         if (world.IsActive == true)
            //         {
            //             active++;
            //         }
            //         else inactive++;
            //     }
            // }
        }


        //Godfather and Mafioso action
        //Godfather kill order or Mafioso kill order
        bool godfatherOrder = false;
        bool mafiosoOrder = false;
        foreach (var x in actions)
        {
            if (x.typeOfAction is "Godfather")
            {
                godfatherOrder = true;
            }
            if (x.typeOfAction is "Mafioso")
            {
                mafiosoOrder = true;
            }
        }

        //If both are true, then Mafioso should change his target to Godfather target, and Godfather action should dissapear
        if (godfatherOrder && mafiosoOrder)
        {
            Action tempAction = null;
            foreach (Action x in actions.Where(x => x.typeOfAction == "Godfather"))
            {
                tempAction = x;
            }

            //Find mafioso in the list of actions
            int ii = 0;
            foreach (Action x in actions)
            {
                if (x.typeOfAction == "Mafioso")
                {
                    actions[ii].target = tempAction.target;
                }
                ii++;
            }

            ii = 0;
            int godfatherIndex = 0;
            foreach (Action x in actions)
            {
                if (x.player.Role.Name == "Godfather")
                {
                    godfatherIndex = ii;
                }
                ii++;
            }
            actions.RemoveAt(godfatherIndex);
        }

        //Only godfather alive
        if (godfatherOrder && !mafiosoOrder)
        {
            //find action index
            int ii = 0;
            int killerIndex = 0;
            foreach (Action x in actions)
            {
                if (x.typeOfAction == "Godfather")
                {
                    killerIndex = ii;
                    break;
                }
                ii++;
            }


            //Find index of target
            ii = 0;
            foreach (Player player in players)
            {
                if (player.Name == actions[killerIndex].target.Name)
                {
                    killTargets.Add(player.Name);
                    break;
                }
                ii++;
            }
        }

        //Only mafioso alive or mafioso ordered by a godfather
        if (mafiosoOrder)
        {
            //find action index
            int ii = 0;
            int killerIndex = 0;
            foreach (Action x in actions)
            {
                if (x.typeOfAction == "Mafioso")
                {
                    killerIndex = ii;
                    break;
                }
                ii++;
            }


            //Find index of target
            ii = 0;
            int targetIndex = 0;
            foreach (Player player in players)
            {
                if (player.Name == actions[killerIndex].target.Name)
                {
                    killTargets.Add(player.Name);
                    targetIndex = ii;
                    break;
                }
                ii++;
            }
        }

        //Vigilante action
        foreach (var e in actions.Where(e => e.typeOfAction is "Vigilante"))
        {
            //find action index
            int ii = 0;
            int killerIndex = 0;
            foreach (Action x in actions)
            {
                if (x.typeOfAction == "Vigilante")
                {
                    killerIndex = ii;
                    break;
                }
                ii++;
            }


            //Find index of target
            ii = 0;
            int targetIndex = 0;
            foreach (Player player in players)
            {
                if (player.Name == actions[killerIndex].target.Name)
                {
                    killTargets.Add(player.Name);
                    targetIndex = ii;
                    if (player.Role.IsTown == true)
                    {
                        missFireByVigilante = true;
                        player.Role.forceAction = true;
                    }
                    break;
                }
                ii++;
            }
        }

        //Veteran action
        foreach (var e in actions.Where(e => e.typeOfAction is "Veteran"))
        {
            foreach (Action action in actions.Where(action => action.target.Name == e.player.Name && action.player.Name != action.target.Name))
            {
                //Find index of target
                int ii = 0;
                int targetIndex = 0;
                foreach (Player player in players)
                {
                    if (player.Name == action.player.Name)
                    {
                        killTargets.Add((player.Name));
                        targetIndex = ii;
                        break;
                    }
                    ii++;
                }
            }
        }

        //Blackmailer action
        foreach (var e in actions.Where(e => e.typeOfAction is "Blackmailer"))
        {
            foreach (Player player in players.Where(player => player.Name == e.target.Name))
            {
                player.Role.blackmailed = true;
                break;
            }
        }
        
        //Doctor action
        foreach(var e in actions.Where(e => e.typeOfAction is "Doctor"))
        {
            foreach (Player player in players.Where(player => player.Name == e.target.Name))
            {
                doctorProtected.Add((player.Name));
            }
        }


        //Kill players
        foreach (var e in killTargets)
        {
            bool skip = false;
            if (!doctorProtected.Contains(e))
            {
                foreach (Player player in players.Where(player => player.Name == e))
                {
                    //If the target is an active veteran, he does not die
                    if (player.Role.Name == "Veteran")
                    {
                        foreach(var action in actions) 
                        {
                            if (action.typeOfAction is "Veteran" && action.player.Name == player.Name) skip = true;
                        }
                    }
                    if (skip) break;

                    foreach(Player p in players.Where(p => p.IsAlive))
                    {
                        foreach(World world in p.PossibleWorlds)
                        {
                            foreach(PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer => possiblePlayer.Name == e))
                            {
                                possiblePlayer.IsAlive = false;
                            }
                        }
                    }
                    foreach (Player p in players.Where(p => p.Name == e))
                    {
                        p.IsAlive = false;
                    }
                }
            }
        }
    }
}
