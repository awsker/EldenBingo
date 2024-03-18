using EldenBingoCommon;
using System.Collections.Concurrent;

namespace EldenBingoServer
{
    public class CheckStatus
    {
        public CheckStatus()
        {
            Time = DateTime.Now;
            CheckedBy = null;
            MarkedBy = new HashSet<Guid>();
            PlayerCounters = new ConcurrentDictionary<Guid, int>();
        }

        public int? CheckedBy { get; set; }
        public DateTime Time { get; init; }
        private IDictionary<Guid, int> PlayerCounters { get; init; }
        private ISet<Guid> MarkedBy { get; init; }

        public bool Check(int team)
        {
            if (team >= 0 && CheckedBy != team)
            {
                CheckedBy = team;
                return true;
            }
            return false;
        }

        public bool Uncheck()
        {
            if (CheckedBy != null)
            {
                CheckedBy = null;
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

        public SquareCounter[] GetCountersForPlayer(UserInRoom recipient, IEnumerable<UserInRoom> players)
        {
            var listOfTeams = Room<UserInRoom>.GetPlayerTeams(players);
            var counters = new SquareCounter[listOfTeams.Count];
            for (int i = 0; i < listOfTeams.Count; ++i)
            {
                var team = listOfTeams[i];
                int? teamCount = null;
                if(recipient.IsSpectator)
                {
                    teamCount = GetCounterByTeam(team.Item1, players);
                } 
                else if(recipient.Team == team.Item1)
                {
                    teamCount = GetCounter(recipient);
                }
                counters[i] = new SquareCounter(team.Item1, teamCount ?? 0);
            }
            return counters;
        }
    }

    public class ServerBingoBoard : BingoBoard
    {
        internal ServerBingoBoard(ServerRoom room, BingoBoardSquare[] squares, EldenRingClasses[] availableClasses, int pointsPerBingo) : base(squares, availableClasses, pointsPerBingo)
        {
            CheckStatus = new CheckStatus[25];
            Room = room;
            for (int i = 0; i < 25; ++i)
            {
                CheckStatus[i] = new CheckStatus();
            }
        }

        public CheckStatus[] CheckStatus { get; init; }
        internal ServerRoom Room { get; init; }


        public BingoBoardSquare[] GetSquareDataForUser(UserInRoom user)
        {
            var squares = new BingoBoardSquare[25];
            for (int i = 0; i < 25; ++i)
            {
                squares[i] = GetSquareDataForUser(user, i);
            }
            return squares;
        }

        public BingoBoardSquare GetSquareDataForUser(UserInRoom user, int index)
        {
            var status = CheckStatus[index];
            return new BingoBoardSquare(
                Squares[index].Text,
                Squares[index].Tooltip,
                Squares[index].MaxCount,
                status.CheckedBy,
                status.IsMarked(user),
                status.GetCountersForPlayer(user, Room.Users));
        }

        public bool UserChangeCount(int i, UserInRoom user, int change)
        {
            if (i < 0 || i >= 25 || change == 0)
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

        public bool UserClicked(int i, UserInRoom clicker, UserInRoom? onBehalfOf)
        {
            if (i < 0 || i >= 25)
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
                //Square not owned by any player or team, allow check
                if (check.CheckedBy == null)
                {
                    if (onBehalfOf != null && !onBehalfOf.IsSpectator)
                    {
                        return check.Check(onBehalfOf.Team);
                    }
                    return false;
                }
                if (clicker.IsSpectator && clicker.IsAdmin)
                {
                    return CheckStatus[i].Uncheck();
                }
                //No owner of the action
                if (onBehalfOf == null)
                    return false;

                //Square owned by this player's team -> allow it to be toggled off
                if (check.CheckedBy.Value == onBehalfOf.Team)
                {
                    return CheckStatus[i].Uncheck();
                }
                //Not allowed to toggle square, owned by other player/team
                return false;
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
            if (i < 0 || i >= 25)
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
    }
}