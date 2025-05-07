namespace RoundRobin
{
    public class Player
    {
        public string Name = "";
        public int MatchNumber;
        public int Column;
        public List<Player> MUs = new List<Player>();
        public bool IsPivot()
        {
            return ((MatchNumber==0) && (Column == 0));
        }
        public override string ToString()
        {
            return Name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return Name == ((Player)obj).Name;
        }
    }
    internal class Program
    {
        public static void PlotMUs(List<Player> players)
        {
            Player[] col0 = new Player[players.Count/2];
            Player[] col1 = new Player[players.Count/2];
            foreach (Player player in players) // Add player to position
            {
                Player[] arr = player.Column switch
                {
                    0 => col0,
                    1 => col1,
                    _ => col0,
                };
                arr[player.MatchNumber] = player;
            }
            foreach (Player player in  col0)
            {
                Console.Write(player.Name);
                Console.Write('\t');
            }
            Console.WriteLine();
            foreach (Player player in col1)
            {
                Console.Write(player.Name);
                Console.Write('\t');
            }
            Console.WriteLine('\n');
        }
        static void Main(string[] args)
        {
            List<Player> players = new List<Player>();
            Console.WriteLine("Number of players?");
            int plNum = int.Parse(Console.ReadLine());
            for(int i = 0; i < plNum; i++) // Create a new player
            {
                Player pl = new Player()
                {
                    Name = $"P{i+1}",
                    MatchNumber = i/2,
                    Column = i%2,
                };
                players.Add(pl);
            }
            if(plNum % 2 != 0) // Odd number of players
            {
                Player pl = new Player()
                {
                    Name = "BYE",
                    MatchNumber = plNum / 2,
                    Column = plNum % 2,
                };
                players.Add(pl);
                plNum++; // Always even
            }
            // Now the cycle of round robin generation
            Console.WriteLine("Number of games?");
            int nMu = int.Parse(Console.ReadLine());
            Console.WriteLine("Rotation number?");
            int rot = int.Parse(Console.ReadLine());
            for(int game = 0; game < nMu; game++) // Calculate MU until got all games
            {
                PlotMUs(players); // Just show the current mu
                // First iterate and check who fights who
                Dictionary<int, Player> thisWeek = new Dictionary<int, Player>(); // Will store the indices of games for this week
                foreach(Player player in players)
                {
                    if(thisWeek.TryGetValue(player.MatchNumber, out Player opponent))
                    {
                        // Found opponent for p!
                        player.MUs.Add(opponent);
                        opponent.MUs.Add(player);
                    }
                    else
                    {
                        // Otherwise p advertises themselves
                        thisWeek.Add(player.MatchNumber, player);
                    }
                }
                // Done, all MUs generated, now to design the next one
                foreach(Player player in players)
                {
                    if (player.IsPivot()) continue; // Pivot always will remain in place
                    // Otherwise rotate player by rot
                    for(int step = 0; step < rot;  step++)
                    {
                        if(player.Column == 1)
                        {
                            if(player.MatchNumber == (plNum/2)-1) // Reached end
                            {
                                player.Column = 0; // Move left
                            }
                            else
                            {
                                player.MatchNumber++; // Can continue going down
                            }
                        }
                        else if (player.Column == 0)
                        {
                            if (player.MatchNumber == 1) // Reached end
                            {
                                player.MatchNumber = 0; // move top left
                                player.Column = 1;
                            }
                            else
                            {
                                player.MatchNumber--; // Can continue going up
                            }
                        }
                        else
                        {
                            throw new Exception("wtf");
                        }
                    }
                }
            }
            // Ok now finally I print all in order
            foreach (Player player in players)
            {
                Console.Write(player.Name + ":\t");
                foreach(Player opp in player.MUs)
                {
                    Console.Write(opp + "\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            // Now some analysis. for each player, check how many MU in common w rest
            foreach (Player player in players)
            {
                Dictionary<Player, int> Commons = new Dictionary<Player, int>();
                foreach(Player opps in player.MUs) // Who do I fight? Only these person's mu will be identical
                {
                    foreach(Player common in opps.MUs)
                    {
                        if (common == player) continue;
                        if(Commons.ContainsKey(common))
                            Commons[common]++;
                        else
                            Commons[common] = 1;
                    }
                }
                // All right found commons, now I'll sort
                SortedDictionary<int, List<Player>> sorted = new SortedDictionary<int, List<Player>>();
                foreach (KeyValuePair<Player, int> comm in Commons)
                {
                    if(!sorted.ContainsKey(comm.Value))
                    {
                        sorted[comm.Value] = new List<Player>();
                    }
                    sorted[comm.Value].Add(comm.Key);
                }
                // Finally I print
                Console.Write(player.Name + ":\n");
                foreach (KeyValuePair<int, List<Player>> result in sorted)
                {
                    Console.Write("\t" + result.Key + ":\t");
                    foreach(Player pl in result.Value)
                    {
                        Console.Write(pl.ToString() + "\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
        }
    }
}
