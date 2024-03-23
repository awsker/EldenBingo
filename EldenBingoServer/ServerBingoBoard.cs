using EldenBingoCommon;
using System.Collections.Concurrent;

namespace EldenBingoServer
{
    public class CheckStatus
    {
        public CheckStatus()
        {
            Time = DateTime.Now;
            Team = null;
            MarkedBy = new HashSet<Guid>();
            PlayerCounters = new ConcurrentDictionary<Guid, int>();
        }

        public int? Team { get; set; }
        public DateTime Time { get; init; }
        private IDictionary<Guid, int> PlayerCounters { get; init; }
        private ISet<Guid> MarkedBy { get; init; }

        public bool Check(int team)
        {
            if (team >= 0 && Team != team)
            {
                Team = team;
                return true;
            }
            return false;
        }

        public bool Uncheck()
        {
            if (Team != null)
            {
                Team = null;
                return true;
            }
            return false;
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
        internal ServerBingoBoard(ServerRoom room, int size, BingoBoardSquare[] squares, EldenRingClasses[] availableClasses) : base(size, squares, availableClasses)
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

        public CheckStatus[] CheckStatus { get; init; }
        internal ServerRoom Room { get; init; }
        private IDictionary<int, IList<BingoLine>> _lastBingos;

        public IDictionary<int, IList<BingoLine>> BingosPerTeam
        {
            get { return _lastBingos; }
        }

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
                Squares[index].MaxCount,
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
                var maxCount = Squares[i].MaxCount;
                return check.SetCounter(user.Guid, maxCount > 0 ? Math.Clamp(oldCount + change, 0, maxCount) : Math.Max(0, oldCount + change));
            }
        }

        public bool UserClicked(int i, UserInRoom clicker, UserInRoom onBehalfOf)
        {
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
                //Square not owned by any player or team, allow check
                if (check.Team == null)
                {
                    if (!onBehalfOf.IsSpectator)
                        checkChanged = check.Check(onBehalfOf.Team);
                }
                //Allow admin spectators to uncheck squares
                else if (clicker.IsSpectator && clicker.IsAdmin)
                {
                    checkChanged = CheckStatus[i].Uncheck();
                }
                //Square owned by this player's team -> allow it to be toggled off
                else if (check.Team.Value == onBehalfOf.Team)
                {
                    checkChanged = CheckStatus[i].Uncheck();
                }
                if(checkChanged)
                {
                    _lastBingos = Room.GetBingos();
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
                if (!square.Team.HasValue)
                    continue;

                if (squaresCountPerTeam.TryGetValue(square.Team.Value, out int c))
                {
                    squaresCountPerTeam[square.Team.Value] = c + 1;
                }
                else
                {
                    squaresCountPerTeam[square.Team.Value] = 1;
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
                int? team = null;
                for (int i = 0; i < Size; ++i)
                {
                    var s = CheckStatus[index(x, y)];
                    if (!s.Team.HasValue)
                        return;
                    if (team.HasValue && team.Value != s.Team.Value)
                        return;
                    team = s.Team.Value;
                    x += dx;
                    y += dy;
                }
                if (team.HasValue)
                {
                    if (!bingosPerTeam.TryGetValue(team.Value, out var list))
                    {
                        list = bingosPerTeam[team.Value] = new List<BingoLine>();
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
                    if (teamsDict.TryGetValue(team.Value, out var team2))
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