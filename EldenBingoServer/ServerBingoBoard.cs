using EldenBingoCommon;

namespace EldenBingoServer
{
    public class ServerBingoBoard : BingoBoard
    {
        public CheckStatus[] CheckStatus { get; init; }
        internal ServerRoom Room { get; init; }
        internal ServerBingoBoard(ServerRoom room, string[] squareTexts, string[] tooltips) : base(squareTexts, tooltips)
        {
            CheckStatus = new CheckStatus[25];
            Room = room;
            for (int i = 0; i < 25; ++i)
            {
                CheckStatus[i] = new CheckStatus();
            }
        }

        public void TransferSquareColors(UserInRoom user)
        {
            for (int i = 0; i < 25; ++i)
            {
                var status = CheckStatus[i];
                var sq = Squares[i];
                sq.Team = status.CheckedBy;
                sq.Marked = status.IsMarked(user.Team);
                sq.Counters = status.GetCounters(user, Room.Clients);
            }
        }

        public override byte[] GetBytes(UserInRoom user)
        {
            //Update all squares to the correct color
            TransferSquareColors(user);
            return base.GetBytes(user);
        }

        public override byte[] GetStatusBytes(UserInRoom user)
        {
            //Update all squares to the correct color
            TransferSquareColors(user);
            return base.GetStatusBytes(user);
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

        public bool UserChangeCount(int i, UserInRoom user, int count)
        {
            if (i < 0 || i >= 25 || count == 0)
                return false;
            var check = CheckStatus[i];
            var oldCount = check.GetCounter(user.Team) ?? 0;
            if (user.IsSpectator)
                return false;
            check.SetCounter(user.Team, Math.Max(0, oldCount + count));
            return true;
        }
    }

    public class CheckStatus
    {
        public DateTime Time { get; init; }
        public int? CheckedBy { get; set; }

        private ISet<int> MarkedBy { get; init; }
        private IDictionary<int, int> CountersBy { get; init; }

        public CheckStatus()
        {
            Time = DateTime.Now;
            CheckedBy = null;
            MarkedBy = new HashSet<int>();
            CountersBy = new Dictionary<int, int>();
        }

        public void Check(int team)
        {
            if (team >= 0)
                CheckedBy = team;
        }

        public void Uncheck()
        {
            CheckedBy = null;
        }

        public bool Mark(int team)
        {
            //If no changes need to be made, return false
            return MarkedBy.Add(team);
        }

        public bool Unmark(int team)
        {
            return MarkedBy.Remove(team);
        }

        public bool IsMarked(int team)
        {
            return MarkedBy.Contains(team);
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

        public bool UnsetCounter(int team)
        {
            return CountersBy.Remove(team);
        }

        public int? GetCounter(int team)
        {
            if(CountersBy.TryGetValue(team, out int counter))
            {
                return counter;
            }
            return null;
        }

        public TeamCounter[] GetCounters(UserInRoom recipient, IEnumerable<UserInRoom> users)
        {
            var listOfTeams = Room<UserInRoom>.GetPlayerTeams(users);
            var counters = new TeamCounter[listOfTeams.Count];
            for(int i = 0; i < listOfTeams.Count; ++i)
            {
                var team = listOfTeams[i];
                if (!recipient.IsSpectator && recipient.Team != team.Item1)
                    continue;
                CountersBy.TryGetValue(team.Item1, out int c);
                counters[i] = new TeamCounter(team.Item1, c);
            }
            return counters;
        }
    }
}
