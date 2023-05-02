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
                sq.CheckOwner = status.CheckedBy ?? new PlayerTeam();
                sq.Marked = status.IsMarked(user);
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
                    check.Check(onBehalfOf);
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

            //Square owned by this player or team -> allow it to be toggled off
            if (check.CheckedBy.Value.Team == onBehalfOf.Team || check.CheckedBy.Value.Player == onBehalfOf.Guid)
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
            if (check.IsMarked(user))
                return check.Unmark(user);
            else
                return check.Mark(user);
        }

        public bool UserChangeCount(int i, UserInRoom user, int count)
        {
            if (i < 0 || i >= 25 || count == 0)
                return false;
            var check = CheckStatus[i];
            var oldCount = check.GetCounter(user) ?? 0;
            check.SetCounter(user, Math.Max(0, oldCount + count));
            return true;
        }
    }

    public class CheckStatus
    {
        public DateTime Time { get; init; }
        public PlayerTeam? CheckedBy { get; set; }

        private ISet<PlayerTeam> MarkedBy { get; init; }
        private IDictionary<PlayerTeam, int> CountersBy { get; init; }

        public CheckStatus()
        {
            Time = DateTime.Now;
            CheckedBy = null;
            MarkedBy = new HashSet<PlayerTeam>();
            CountersBy = new Dictionary<PlayerTeam, int>();
        }

        public void Check(UserInRoom user)
        {
            CheckedBy = new PlayerTeam(user);
        }

        public void Uncheck()
        {
            CheckedBy = null;
        }

        public bool Mark(UserInRoom user)
        {
            //If no changes need to be made, return false
            return MarkedBy.Add(new PlayerTeam(user));
        }

        public bool Unmark(UserInRoom user)
        {
            return MarkedBy.Remove(new PlayerTeam(user));
        }

        public bool IsMarked(UserInRoom user)
        {
            return MarkedBy.Contains(new PlayerTeam(user));
        }

        public void SetCounter(UserInRoom user, int? counter)
        {
            if (user.IsSpectator)
                return;

            if (counter.HasValue)
                CountersBy[new PlayerTeam(user)] = counter.Value;
            else
                CountersBy.Remove(new PlayerTeam(user));

        }

        public bool UnsetCounter(UserInRoom user)
        {
            return CountersBy.Remove(new PlayerTeam(user));
        }

        public int? GetCounter(UserInRoom user)
        {
            if(CountersBy.TryGetValue(new PlayerTeam(user), out int counter))
            {
                return counter;
            }
            return null;
        }

        public ColorCounter[] GetCounters(UserInRoom recipient, IEnumerable<UserInRoom> users)
        {
            var listOfPlayersAndTeams = PlayerTeam.GetPlayerTeams(users, out var teams);
            var counters = new ColorCounter[listOfPlayersAndTeams.Count];
            for(int i = 0; i < listOfPlayersAndTeams.Count; ++i)
            {
                var pt = listOfPlayersAndTeams[i];
                if (!recipient.IsSpectator && recipient.Guid != pt.Player && recipient.Team != pt.Team)
                    continue;
                CountersBy.TryGetValue(pt, out int c);
                counters[i] = new ColorCounter(pt.Color, c);
            }
            return counters;
        }
    }
}
