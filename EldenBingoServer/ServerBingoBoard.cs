using EldenBingoCommon;

namespace EldenBingoServer
{
    public class CheckStatus
    {
        public CheckStatus()
        {
            Time = DateTime.Now;
            CheckedBy = null;
            MarkedBy = new HashSet<int>();
            CountersBy = new Dictionary<int, int>();
        }

        public int? CheckedBy { get; set; }
        public DateTime Time { get; init; }
        private IDictionary<int, int> CountersBy { get; init; }
        private ISet<int> MarkedBy { get; init; }

        public void Check(int team)
        {
            if (team >= 0)
                CheckedBy = team;
        }

        public int? GetCounter(int team)
        {
            if (CountersBy.TryGetValue(team, out int counter))
            {
                return counter;
            }
            return null;
        }

        public TeamCounter[] GetCounters(UserInRoom recipient, IEnumerable<UserInRoom> users)
        {
            var listOfTeams = Room<UserInRoom>.GetPlayerTeams(users);
            var counters = new TeamCounter[listOfTeams.Count];
            for (int i = 0; i < listOfTeams.Count; ++i)
            {
                var team = listOfTeams[i];
                if (!recipient.IsSpectator && recipient.Team != team.Item1)
                    continue;
                CountersBy.TryGetValue(team.Item1, out int c);
                counters[i] = new TeamCounter(team.Item1, c);
            }
            return counters;
        }

        public bool IsMarked(int team)
        {
            return MarkedBy.Contains(team);
        }

        public bool Mark(int team)
        {
            //If no changes need to be made, return false
            return MarkedBy.Add(team);
        }

        public void SetCounter(int team, int? counter)
        {
            if (team < 0)
                return;

            if (counter.HasValue)
                CountersBy[team] = counter.Value;
            else
                CountersBy.Remove(team);
        }

        public void Uncheck()
        {
            CheckedBy = null;
        }

        public bool Unmark(int team)
        {
            return MarkedBy.Remove(team);
        }

        public bool UnsetCounter(int team)
        {
            return CountersBy.Remove(team);
        }
    }

    public class ServerBingoBoard : BingoBoard
    {
        internal ServerBingoBoard(ServerRoom room, string[] squareTexts, string[] tooltips) : base(squareTexts, tooltips)
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

        public BingoSquareStatus[] GetSquareDataForUser(UserInRoom user)
        {
            var squares = new BingoSquareStatus[25];
            for (int i = 0; i < 25; ++i)
            {
                var status = CheckStatus[i];
                squares[i] = new BingoSquareStatus(status.CheckedBy, status.IsMarked(user.Team), status.GetCounters(user, Room.Users));
            }
            return squares;
        }

        public bool UserChangeCount(int i, UserInRoom user, int change)
        {
            if (i < 0 || i >= 25 || change == 0)
                return false;
            var check = CheckStatus[i];
            var oldCount = check.GetCounter(user.Team) ?? 0;
            if (user.IsSpectator)
                return false;
            check.SetCounter(user.Team, Math.Max(0, oldCount + change));
            return true;
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
            //Square not owned by any player or team, allow check
            if (check.CheckedBy == null)
            {
                if (onBehalfOf != null && !onBehalfOf.IsSpectator)
                {
                    check.Check(onBehalfOf.Team);
                    return true;
                }
                return false;
            }
            if (clicker.IsSpectator && clicker.IsAdmin)
            {
                CheckStatus[i].Uncheck();
                return true;
            }
            //No owner of the action
            if (onBehalfOf == null)
                return false;

            //Square owned by this player's team -> allow it to be toggled off
            if (check.CheckedBy.Value == onBehalfOf.Team)
            {
                CheckStatus[i].Uncheck();
                return true;
            }
            //Not allowed to toggle square, owned by other player/team
            return false;
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
            if (check.IsMarked(user.Team))
                return check.Unmark(user.Team);
            else
                return check.Mark(user.Team);
        }
    }
}