using SocialDeductionGame.Roles;

namespace SocialDeductionGame.Actions;

public class ActionManager
{
    public List<Action> Actions { get; set; }
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
        //Sort actions by
        //Role blockers first
        //Then Godfather, Mafioso and sheriff
        List<Action> actions = new List<Action>();
        
        foreach (var e in this.Actions)
        {
            int index = 0;
            switch (e.typeOfAction)
            {
                case "RoleBlock": 
                    actions.Insert(0, e); 
                    break;
                case "Godfather":
                    index = 0;
                    foreach(var x in actions)
                    {
                        if (x.typeOfAction == "RoleBlock")
                        {
                            index ++;
                        }
                    }
                    actions.Insert(index, e);
                    break;
                case "Mafioso":
                    index = 0;
                    foreach (var x in actions)
                    {
                        if (x.typeOfAction == "RoleBlock")
                        {
                            index++;
                        }
                    }
                    actions.Insert(index, e);
                    break;
                case "Sheriff":
                    index = 0;
                    foreach (var x in actions)
                    {
                        if (x.typeOfAction == "RoleBlock")
                        {
                            index++;
                        }
                    }
                    actions.Insert(index, e);
                    break;

            }
        }

        //Removing actions that are roleblocked
        int[] ints = new int[this.Actions.Count];
        foreach (var e in actions.Where(e => e.typeOfAction is "RoleBlock"))
        {
            int playerindex = 0;
            int ii = 0;
            
            foreach (var x in actions)
            {
                        
                if(e.target.Name == x.player.Name)
                {
                    ints[ii] = playerindex;
                    ii++;
                }
                playerindex ++;
            }
        }
        for (int ii = 0; ii < ints.Length; ii++)
        {
            if (ints[ii] != 0)
            {
                //Note that roleblockers may be removed from the actions list
                //this is not a problem as their action has been performed
                actions.RemoveAt(ii);
            }
        }

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
            //Change worlds that are not in accordance to new information to inactive
            foreach (var x in sheriffIndex)
            {
                if(e.player.Name == players[x].Name)
                {

                }
            }
            
        }
    }
}
