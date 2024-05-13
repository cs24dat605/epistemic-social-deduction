using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using SocialDeductionGame.Actions;
using System.Collections.Generic;
using System.Numerics;

namespace SocialDeductionGame
{
    public class Game
    {
        public GameConfiguration GameConfig = new GameConfiguration();
        public List<Player> Players { get; set; }
        
        private int _round;

        public int Round => _round;

        private static Game _instance;
        private bool _gameFinished;

        private long startTime = 0;

        //At the end of the game, if town has won, set to true
        //else the mafia has won
        //used for data collection
        public bool townWin = false;

        public List <int> correctVotes { get; set; }
        
        public bool shouldPrint = true;
        
        public static Game Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Game();
                }
                return _instance;
            }
        }
        
        public void StartGame(List<World> allWorlds)
        {
            _gameFinished = false;
            _round = 0;

            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (Game.Instance.shouldPrint)
                Console.WriteLine("Moving worlds to player");
            WorldManager.MoveWorldsToPlayers(allWorlds);
            
            if (Game.Instance.shouldPrint)
                Console.WriteLine($"Time taken to move to player: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - curTime}");

            startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            correctVotes = new List<int>();
            for(int i = 0; i < Players.Count; i++)
            {
                correctVotes.Add(0);
            }
            
            while (!_gameFinished)
            {
                if (Game.Instance.shouldPrint)
                    Console.WriteLine($"Round: {Round}");
                startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                
                RunDayPhase();
                RunNightPhase();
                
                CheckIfFinished();
                
                _round++;

                if (_gameFinished)
                {
                    if (Game.Instance.shouldPrint)
                        Console.WriteLine($"Game time taken: {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTime}");
                    break;
                }
            }
        }
        
        private void CheckIfFinished()
        {
            bool townWins = !Players.Any(p => p.IsAlive && !p.Role.IsTown);
            bool mafiaWins = Players.Count(p => p.IsAlive && !p.Role.IsTown) >= Players.Count(p => p.IsAlive && p.Role.IsTown);

            if (townWins)
            {
                Console.WriteLine($"Town wins! Round:{_round}");
                townWin = true;
            }
            else if (mafiaWins)
            {
                Console.WriteLine($"Mafia wins! Round:{_round}");
                townWin = false;
            }

            _gameFinished = townWins || mafiaWins;
        }

        public List<Player> CreatePlayers()
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("Creating Players");
            
            List<Player> playerList = new List<Player>();
            List<Role> availableRoles = GameConfig.GetRoleCounts();

            for (int i = 0; i < GameConfig.Players; i++)
            {
                Player newPlayer = new Player(
                    i,
                    availableRoles[i]
                );
                
                playerList.Add(newPlayer);
                
            }
            
            return playerList;
        }
        
        private void RunDayPhase()
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("Day Phase");
            
            // Preform day action before voting might need changing?
            foreach (Player player in Players.Where(player => player.IsAlive))
            {
                if (player is {Role: IRoleDayAction dayAction})
                {
                    dayAction.PerformDayAction(Players);
                }
            }
            
            Random random = new Random();

            // Allowing up to 3 communications per turn
            for (var _ = 0; _ < 2; _++)
            {
                foreach (Player player in Players.Where(player => player.IsAlive).OrderBy(_ => random.Next()))
                {
                    if (player.Role.blackmailed == null || player.Role.blackmailed == false)
                        player.Communicate();
                }
            }

            foreach (Player player in Players.Where(player => player.IsAlive).OrderBy(_ => random.Next()))
            {
                player.Role.blackmailed = false;
            }

            // Voting stuff
            List<VotingPlayer> votingPlayers = new List<VotingPlayer>();
            foreach (Player player in Players.Where (player => player.IsAlive == true))
            {
                VotingPlayer voting = new VotingPlayer(player, 0);
                votingPlayers.Add(voting);
            }
            int i = 0;
            foreach (Player player in Players.Where(player => player.IsAlive == true))
            {
                if (player.IsAlive)
                {
                    int MaxPossiblescore = Int32.MinValue;

                    //MaxPossible score
                    foreach (World world in player.PossibleWorlds.Where(world => world.IsActive == true))
                    {
                        if(MaxPossiblescore < world.Marks)
                        {
                            MaxPossiblescore = world.Marks;
                        }
                    }

                    //Generating a list of all equally most possible worlds
                    List<World> worldList = [.. player.PossibleWorlds.Where(world => world.Marks == MaxPossiblescore && world.IsActive == true)];

                    //Select at random which of the most likely worlds to choose
                    random = new Random();
                    int index = random.Next(0, worldList.Count);

                    World SelectedWorld = worldList[index];

                    List<PossiblePlayer> playerList = new List<PossiblePlayer>();
                    if (player.Role.IsTown)
                    {
                        foreach (PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer => susPlayer.PossibleRole.IsTown == false && susPlayer.IsAlive  == true))
                        {
                            playerList.Add(susPlayer);
                        }
                        if(playerList.Count == 0)
                        {
                            int x = random.Next(0, SelectedWorld.PossiblePlayers.Count);
                            playerList.Add(SelectedWorld.PossiblePlayers[x]);
                        }
                    }
                    else 
                    {
                        foreach(PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer => susPlayer.PossibleRole.IsTown == true && susPlayer.IsAlive == true))
                        {
                            playerList.Add(susPlayer);
                        }
                        if (playerList.Count == 0)
                        {
                            int x = random.Next(0, SelectedWorld.PossiblePlayers.Count);
                            playerList.Add(SelectedWorld.PossiblePlayers[x]);
                        }
                    }


                    index = random.Next(playerList.Count);
                    PossiblePlayer SelectedPlayer = playerList[index];

                    foreach (VotingPlayer votingPlayer in votingPlayers)
                    {
                        if (votingPlayer.VotedPlayer == SelectedPlayer.ActualPlayer)
                        {
                            votingPlayer.Votes++;

                            //Data stuff
                            break;
                        }
                    }

                    if ((player.Role.IsTown && !SelectedPlayer.ActualPlayer.Role.IsTown) ||
                                (!player.Role.IsTown && SelectedPlayer.ActualPlayer.Role.IsTown))
                    {
                        correctVotes[i]++;
                    }
                    if (Game.Instance.shouldPrint)
                        Console.WriteLine("I " + player.Name + " am voting for " + SelectedPlayer.Name + " because I think they are a " + SelectedPlayer.PossibleRole.Name + "");
                }
                i++;
            }

            //MaxVotes
            int MaxVotes = 0;
            foreach (VotingPlayer votedPlayer in votingPlayers)
            {
                // Console.WriteLine(votedPlayer.VotedPlayer.Name + " " + votedPlayer.Votes.ToString());
                if (MaxVotes < votedPlayer.Votes)
                {
                   MaxVotes = votedPlayer.Votes;
                }
            }

            //Generating a list of all player that have gotten equal votes
            List<VotingPlayer> TrialList = [.. votingPlayers.Where(Vp => Vp.Votes == MaxVotes)];
            if (TrialList.Count == 1)
            {
                //Kill this player
                foreach (Player p in Players.Where(p => p.IsAlive))
                {
                    foreach (World world in p.PossibleWorlds)
                    {
                        foreach (PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer => possiblePlayer.Name == TrialList[0].VotedPlayer.Name))
                        {
                            possiblePlayer.IsAlive = false;
                        }
                    }
                }
                foreach (Player p in Players.Where(p => p.Name == TrialList[0].VotedPlayer.Name))
                {
                    foreach (Player player in Players.Where(player => player.IsAlive == true))
                    {
                        foreach (World world in p.PossibleWorlds)
                        {
                            foreach(PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer => possiblePlayer.Name == TrialList[0].VotedPlayer.Name))
                            {
                                if (possiblePlayer.PossibleRole.Name != possiblePlayer.ActualPlayer.Role.Name)
                                {
                                    world.IsActive = false;
                                }
                            }
                        }
                    }
                    p.Kill();

                    
                }
                

            }
            //If a majority vote has not been cast, no one is killed

            // Console.WriteLine("Marks");
            // foreach (World pWorlds in Players[0].PossibleWorlds)
            // {
            //     Console.Write($" {pWorlds.Marks}");
            // }
        }
        
        private void RunNightPhase()
        {
            if (Game.Instance.shouldPrint)
                Console.WriteLine("Night Phase");

            List<Actions.Action> actions = new List<Actions.Action>();

            foreach (var player in Players)
            {
                if (player is { IsAlive: true, Role: IRoleNightAction nightAction })
                {
                    nightAction.PerformNightAction(player, actions); 
                    
                    
                }
            }

            ActionManager actionManager = new ActionManager(actions);

            actionManager.HandleActions(Players);
        }
    }
};

