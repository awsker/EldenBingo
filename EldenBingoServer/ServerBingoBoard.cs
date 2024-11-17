using EldenBingoCommon;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace EldenBingoServer
{
    public class CheckStatus
    {
        public CheckStatus()
        {
            Team = Array.Empty<int>();
            MarkedBy = new HashSet<Guid>();
            PlayerCounters = new ConcurrentDictionary<Guid, int>();
        }
        [JsonProperty]
        public int[] Team { get; set; }
        [JsonProperty]
        private IDictionary<Guid, int> PlayerCounters { get; init; }
        [JsonProperty]
        private ISet<Guid> MarkedBy { get; init; }

        public bool IsChecked()
        {
            return Team.Length > 0;
        }

        public bool IsChecked(int team)
        {
            return Team.Contains(team);
        }

        public bool Check(int team)
        {
            if (team >= 0)
            {
                var teams = new List<int>(Team);
                if (teams.Contains(team))
                    return false;
                teams.Add(team);
                teams.Sort();
                Team = teams.ToArray();
                return true;
            }
            return false;
        }

        public bool Uncheck(int team)
        {
            var teams = new List<int>(Team);
            var removed = teams.Remove(team);
            if (removed)
                Team = teams.ToArray();
            return removed;
        }

        public bool Mark(UserInRoom player)
        {
            //If no changes need to be made, return false
            return MarkedBy.Add(player.Guid);
        }

        public bool Unmark(UserInRoom player)
        {
            return MarkedBy.Remove(player.Guid);
        }

        public bool IsMarked(UserInRoom player)
        {
            return MarkedBy.Contains(player.Guid);
        }

        public int? GetCounter(UserInRoom player)
        {
            if (PlayerCounters.TryGetValue(player.Guid, out int counter))
            {
                return counter;
            }
            return null;
        }

        public int? GetCounterByTeam(int team, IEnumerable<UserInRoom> players)
        {
            int? counter = null;
            foreach(var player in players.Where(p => p.Team == team))
            {
                var c = GetCounter(player);
                if (c.HasValue)
                {
                    counter = Math.Max(counter ?? 0, c.Value);
                }
            }
            return counter;
        }

        public bool SetCounter(Guid player, int? counter)
        {
            if (counter.HasValue && counter.Value > 0)
            {
                PlayerCounters.TryGetValue(player, out int c);
                PlayerCounters[player] = counter.Value;
                return counter != c;
            }
            else
                return PlayerCounters.Remove(player);
        }

        public SquareCounter[] GetCountersForPlayer(UserInRoom recipient, IEnumerable<UserInRoom> players, IList<Team> teams)
        {
            var counters = new SquareCounter[teams.Count];
            for (int i = 0; i < teams.Count; ++i)
            {
                var team = teams[i];
                int? teamCount = null;
                if(recipient.IsSpectator)
                {
                    teamCount = GetCounterByTeam(team.Index, players);
                } 
                else if(recipient.Team == team.Index)
                {
                    teamCount = GetCounter(recipient);
                }
                counters[i] = new SquareCounter(team.Index, teamCount ?? 0);
            }
            return counters;
        }
    }

    public class ServerBingoBoard : BingoBoard
    {
        public ServerBingoBoard(ServerRoom room, int size, bool lockout, BingoBoardSquare[] squares, EldenRingClasses[] availableClasses) : base(size, lockout, squares, availableClasses)
        {
            var sizeSqr = size * size;
            CheckStatus = new CheckStatus[sizeSqr];
            Room = room;
            for (int i = 0; i < CheckStatus.Length; ++i)
            {
                CheckStatus[i] = new CheckStatus();
            }
            _lastBingos = new Dictionary<int, IList<BingoLine>>();
        }

        [JsonProperty]
        public CheckStatus[] CheckStatus { get; init; }
        [JsonIgnore]
        internal ServerRoom Room { get; set; }
        [JsonProperty]
        private IDictionary<int, IList<BingoLine>> _lastBingos;

        [JsonIgnore]
        public IDictionary<int, IList<BingoLine>> BingosPerTeam
        {
            get { return _lastBingos; }
        }

        [JsonIgnore]
        public ISet<BingoLine> BingoSet
        {
            get { return new HashSet<BingoLine>(_lastBingos.Values.SelectMany(t => t)); }
        }

        public BingoBoardSquare[] GetSquareDataForUser(UserInRoom user)
        {
            var squares = new BingoBoardSquare[SquareCount];
            var teams = Room.GetActiveTeams();
            for (int i = 0; i < SquareCount; ++i)
            {
                squares[i] = GetSquareDataForUser(user, i, teams);
            }
            return squares;
        }

        public BingoBoardSquare GetSquareDataForUser(UserInRoom user, int index, IList<Team>? teams = null)
        {
            var status = CheckStatus[index];
            teams ??= Room.GetActiveTeams();
            return new BingoBoardSquare(
                Squares[index].Text,
                Squares[index].Tooltip,
                status.Team,
                status.IsMarked(user),
                status.GetCountersForPlayer(user, Room.Users, teams));
        }

        public bool UserChangeCount(int i, UserInRoom user, int change)
        {
            if (i < 0 || i >= SquareCount || change == 0)
                return false;
            var check = CheckStatus[i];
            lock (check)
            {
                var oldCount = check.GetCounter(user) ?? 0;
                if (user.IsSpectator)
                    return false;
                return check.SetCounter(user.Guid, Math.Max(0, oldCount + change));
            }
        }

        public bool UserClicked(int i, UserInRoom clicker, UserInRoom onBehalfOf, out int teamChanged)
        {
            teamChanged = -1;
            if (i < 0 || i >= SquareCount)
                return false;

            //Spectators that are not admins cannot click
            if (clicker.IsSpectator && !clicker.IsAdmin)
                return false;

            //Only admins can click for other players
            if (clicker != onBehalfOf && !clicker.IsAdmin)
                return false;

            var check = CheckStatus[i];
            lock (check)
            {
                bool checkChanged = false;
                if (Lockout)
                {
                    var team = check.Team.FirstOrDefault(-1);
                    //Square not owned by any player or team, allow check
                    if (team == -1)
                    {
                        if (!onBehalfOf.IsSpectator)
                        {
                            checkChanged = check.Check(onBehalfOf.Team);
                            teamChanged = onBehalfOf.Team;
                        }
                    }
                    //Allow admin spectators to uncheck squares
                    else if (clicker.IsSpectator && clicker.IsAdmin)
                    {
                        checkChanged = CheckStatus[i].Uncheck(team);
                        teamChanged = team;
                    }
                    //Square owned by this player's team -> allow it to be toggled off
                    else if (team == onBehalfOf.Team)
                    {
                        checkChanged = CheckStatus[i].Uncheck(team);
                        teamChanged = team;
                    }
                    if (checkChanged)
                    {
                        _lastBingos = Room.GetBingos();
                    }
                }
                else
                {
                    //Square owned by this player's team -> allow it to be toggled off
                    if (check.IsChecked(onBehalfOf.Team))
                    {
                        checkChanged = check.Uncheck(onBehalfOf.Team);
                        teamChanged = onBehalfOf.Team;
                    }
                    //Allow admin spectators to uncheck squares with only 1 check without specifying the team
                    else if (clicker.IsSpectator && clicker.IsAdmin && check.Team.Length == 1)
                    {
                        checkChanged = CheckStatus[i].Uncheck(check.Team[0]);
                        teamChanged = check.Team[0];
                    }
                    //Otherwise check the square for the selected team if it's not spectator
                    else if (!onBehalfOf.IsSpectator)
                    {
                        checkChanged = check.Check(onBehalfOf.Team);
                        teamChanged = onBehalfOf.Team;
                    }
                    if (checkChanged)
                    {
                        _lastBingos = Room.GetBingos();
                    }
                }
                return checkChanged;

            }
        }

        /// <summary>
        //
        /// </summary>
        /// <param name="i">Index to mark</param>
        /// <param name="user">User marking</param>
        /// <returns>True if any markings were changed</returns>
        public bool UserMarked(int i, UserInRoom user)
        {
            if (i < 0 || i >= SquareCount)
                return false;

            var check = CheckStatus[i];
            lock (check)
            {
                if (check.IsMarked(user))
                    return check.Unmark(user);
                else
                    return check.Mark(user);
            }
        }


        public Dictionary<int, int> GetSquaresPerTeam()
        {
            var squaresCountPerTeam = new Dictionary<int, int>();
            
            foreach (var square in CheckStatus)
            {
                foreach (var t in square.Team)
                {
                    if (squaresCountPerTeam.TryGetValue(t, out int c))
                        squaresCountPerTeam[t] = c + 1;
                    else
                        squaresCountPerTeam[t] = 1;
                }
            }
            return squaresCountPerTeam;
        }


        public IDictionary<int, IList<BingoLine>> GetBingosPerTeam(IList<Team> teams)
        {
            var teamsDict = teams.ToDictionary(t => t.Index, t => t);
            var bingosPerTeam = new Dictionary<int, IList<BingoLine>>();
            
            void findBingo(int startx, int starty, int dx, int dy)
            {
                int index(int x, int y) { return x + y * Size; }
                int x = startx;
                int y = starty;
                var squaresCountPerTeam = new Dictionary<int, int>();
                for (int i = 0; i < Size; ++i)
                {
                    var s = CheckStatus[index(x, y)];
                    foreach (var t in s.Team)
                    {
                        if (squaresCountPerTeam.TryGetValue(t, out int c))
                            squaresCountPerTeam[t] = c + 1;
                        else
                            squaresCountPerTeam[t] = 1;
                    }
                    x += dx;
                    y += dy;
                }
                foreach(var teamCount in squaresCountPerTeam)
                {
                    if (teamCount.Value != Size)
                        continue; //Skip teams that didn't fill the entire line

                    if (!bingosPerTeam.TryGetValue(teamCount.Key, out var list))
                    {
                        list = bingosPerTeam[teamCount.Key] = new List<BingoLine>();
                    }
                    int bingoType = 0;
                    int lineIndex = 0;
                    if (dx > 0 && dy > 0)
                        bingoType = 2;
                    else if (dx > 0 && dy < 0)
                        bingoType = 3;
                    else if (dy > 0)
                    {
                        bingoType = 0;
                        lineIndex = startx;
                    }
                    else if (dx > 0)
                    {
                        bingoType = 1;
                        lineIndex = starty;
                    }
                    if (teamsDict.TryGetValue(teamCount.Key, out var team2))
                    {
                        list.Add(new BingoLine(team2.Index, team2.Name, bingoType, lineIndex));
                    }
                }
            }
            for (int x = 0; x < Size; ++x)
            {
                findBingo(x, 0, 0, 1);
            }

            for (int y = 0; y < Size; ++y)
            {
                findBingo(0, y, 1, 0);
            }
            //Top-left to bottom-right
            findBingo(0, 0, 1, 1);
            //Bottom-left to top-right
            findBingo(0, Size - 1, 1, -1);

            return bingosPerTeam;
        }


    }
}