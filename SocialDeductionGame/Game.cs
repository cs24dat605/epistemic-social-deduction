using System.Collections.Concurrent;
using SocialDeductionGame.Roles;
using SocialDeductionGame.Worlds;
using SocialDeductionGame.Actions;
using SocialDeductionGame.Logic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Action = SocialDeductionGame.Actions.Action;
using SocialDeductionGame.Communication;

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

        public List <int> amountOfVotes { get; set; }
        
        public bool shouldPrint = false;
        
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
            amountOfVotes = new List<int>();
            for(int i = 0; i < Players.Count; i++)
            {
                correctVotes.Add(0);
                amountOfVotes.Add(0);
            }

            int x = 0;
            List<int> y = new List<int>();
            foreach(var player in Players) 
            {
                if(!player.Role.IsTown)
                {
                    y.Add(x);
                }
                foreach(var world in player.PossibleWorlds)
                {
                    if (world.PossiblePlayers[x].PossibleRole.Name != player.Role.Name)
                    {
                        world.IsPrivateActive = false;
                    }
                }
                x++;
            }
            foreach (var player in Players)
            {
                if (player.Role.IsTown)
                {
                    break;
                }
                foreach (var world in player.PossibleWorlds)
                {
                    foreach(int k in y)
                    {
                        if (world.PossiblePlayers[k].PossibleRole.IsTown)
                        {
                            world.IsPrivateActive = false;
                        }
                    }
                }
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

            foreach (Player player in Players)
            {
                player.Role.blackmailed = false;
            }

            // Voting stuff
            List<VotingPlayer> votingPlayers = new List<VotingPlayer>();
            foreach (Player player in Players.Where (player => player.IsAlive))
            {
                VotingPlayer voting = new VotingPlayer(player, 0);
                votingPlayers.Add(voting);
            }

            Parallel.ForEach(Players.Where(player => player.IsAlive).OrderBy(_ => random.Next()), (player, state) =>
            {
                if (player == null)
                    state.Break();
                    
                int MinPossiblescore = Int32.MaxValue;

                //MaxPossible score
                foreach (World world in player.PossibleWorlds.Where(world => world.IsPrivateActive))
                {
                    if (MinPossiblescore > world.Marks)
                    {
                        MinPossiblescore = world.Marks;
                    }
                }

                //Generating a list of all equally most possible worlds
                List<World> worldList =
                [
                    .. player.PossibleWorlds.Where(world => world.Marks == MinPossiblescore && world.IsPrivateActive == true)
                ];
                random = new Random();
                int index = random.Next(0, worldList.Count);

                World SelectedWorld = worldList[index];


                List<PossiblePlayer> playerList = new List<PossiblePlayer>();
                if (player.Role.IsTown)
                {
                    foreach (PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer =>
                                 susPlayer.PossibleRole.IsTown == false && susPlayer.IsAlive == true))
                    {
                        playerList.Add(susPlayer);
                    }
                    if (playerList.Count == 0)
                    {
                        foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.IsAlive))
                        {
                            playerList.Add(p);
                        }
                    }
                }
                else
                {
                    foreach (PossiblePlayer susPlayer in SelectedWorld.PossiblePlayers.Where(susPlayer =>
                                 susPlayer.PossibleRole.IsTown && susPlayer.IsAlive))
                    {
                        playerList.Add(susPlayer);
                    }
                    if (playerList.Count == 0)
                    {
                        foreach (PossiblePlayer p in SelectedWorld.PossiblePlayers.Where(p => p.IsAlive))
                        {
                            playerList.Add(p);
                        }
                    }
                }

                List<int> marks = new List<int>();
                int suspectIndex = 0; 
                foreach (var p in playerList)
                {
                    if (playerList.Count == 0)
                        break;
                    marks.Add(0);
                    foreach (var a in player.Accusations)
                    {
                        switch(a.Type)
                        {
                            case 0:
                                break;
                            case 1:
                                if(a.Accuser.Id == p.Id)
                                {
                                    PossiblePlayer accused = SelectedWorld.PossiblePlayers.Find(accused => accused.Id == a.Accused.Id && accused.PossibleRole.IsTown != a.Role.IsTown);
                                    if (accused != null)
                                        marks[suspectIndex]++;
                                }
                                break;
                            case 2:
                                if(a.Accused.Id == p.Id)
                                {
                                    PossiblePlayer accused = SelectedWorld.PossiblePlayers.Find(accused => accused.Id == p.Id && a.Role.IsTown != p.PossibleRole.IsTown);
                                    if (accused != null)
                                        marks[suspectIndex]++;
                                }
                                break;
                            case 3:
                                if(a.PlayerAsk.Id == p.Id)
                                {
                                    if (a.Response == "Yes")
                                    {
                                        PossiblePlayer accused = SelectedWorld.PossiblePlayers.Find(accused => accused.Id == a.Accused.Id && a.Role.Name != accused.PossibleRole.Name);
                                        if (accused != null)
                                            marks[suspectIndex]++;
                                    }
                                    else
                                    {
                                        PossiblePlayer accused = SelectedWorld.PossiblePlayers.Find(accused => accused.Id == a.Accused.Id && a.Role.Name == accused.PossibleRole.Name);
                                        if (accused != null)
                                            marks[suspectIndex]++;
                                    }
                                }
                                break;
                        }
                        
                    }
                    suspectIndex++;
                }

                //int maxMarkIndex = marks.IndexOf(marks.Max());
                int maxMarkIndex = random.Next(playerList.Count);
                PossiblePlayer SelectedPlayer = playerList[maxMarkIndex];

                foreach (VotingPlayer votingPlayer in votingPlayers)
                {
                    if (votingPlayer.VotedPlayer.Name == SelectedPlayer.ActualPlayer.Name)
                    {
                        votingPlayer.IncrementVote();

                        //Data stuff
                        break;
                    }
                }

                int i = player.Id;
                amountOfVotes[i]++;

                if ((player.Role.IsTown && !SelectedPlayer.ActualPlayer.Role.IsTown) ||
                    (!player.Role.IsTown && SelectedPlayer.ActualPlayer.Role.IsTown))
                {
                    correctVotes[i]++;
                }

                if (Game.Instance.shouldPrint)
                    Console.WriteLine("I " + player.Name + " am voting for " + SelectedPlayer.Name +
                                      " because I think they are a " + SelectedPlayer.PossibleRole.Name + "");
            });

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
                Parallel.ForEach(Players.Where(p => p.IsAlive), player =>
                {
                    foreach (World world in player.PossibleWorlds)
                    {
                        foreach (PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer =>
                                     possiblePlayer.Name == TrialList[0].VotedPlayer.Name))
                        {
                            possiblePlayer.IsAlive = false;
                        }
                    }
                });
                Parallel.ForEach(Players.Where(p => p.Name == TrialList[0].VotedPlayer.Name && p.IsAlive), p =>
                {
                    foreach (World world in p.PossibleWorlds)
                    {
                        foreach (PossiblePlayer possiblePlayer in world.PossiblePlayers.Where(possiblePlayer =>
                                     possiblePlayer.Name == TrialList[0].VotedPlayer.Name))
                        {
                            if (possiblePlayer.PossibleRole.Name != possiblePlayer.ActualPlayer.Role.Name)
                            {
                                world.IsActive = false;
                                world.IsPrivateActive = false;
                            }
                        }
                    }

                    p.Kill();
                });
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
            long starttime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (Game.Instance.shouldPrint)
                Console.WriteLine("Night Phase");
            
            ConcurrentBag<Action> actions = new ConcurrentBag<Action>();
            
            
            Parallel.ForEach(Players, player =>
            {
                if (player is { IsAlive: true, Role: IRoleNightAction nightAction })
                {
                    nightAction.PerformNightAction(player, actions);
                }
            }); 

            ActionManager actionManager = new ActionManager(actions);

            actionManager.HandleActions(Players);
            if (shouldPrint)
                Console.WriteLine($"Nighttime: {Convert.ToDouble(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - starttime)/1000}s");
        }
    }
};

