using EldenBingoCommon;
using System.Drawing;

namespace EldenBingoServer
{
    public class ServerBingoBoard : BingoBoard
    {
        public CheckStatus[] CheckStatus { get; init; }
        public ServerBingoBoard(string[] squareTexts, string[] tooltips) : base(squareTexts, tooltips)
        {
            CheckStatus = new CheckStatus[25];
            for (int i = 0; i < 25; ++i)
            {
                CheckStatus[i] = new CheckStatus();
            }
        }

        public void TransferSquareColors()
        {
            for (int i = 0; i < 25; ++i)
            {
                var status = CheckStatus[i];
                Squares[i].Color = status.Player == null ? Color.Empty : (status.Player.Team == 0 ?
                    status.Player.ConvertedColor : NetConstants.DefaultPlayerColors[status.Player.Team - 1].Color);
            }
        }

        public byte[] GetColorBytes(UserInRoom user)
        {
            byte[] buffer = new byte[25 * 5];
            int offset = 0;
            for(int i = 0; i < 25; ++i)
            {
                var sq = Squares[i];
                var check = CheckStatus[i];
                Array.Copy(BitConverter.GetBytes(sq.Color.ToArgb()), 0, buffer, offset, 4);
                buffer[offset + 4] = check.IsMarked(user) ? (byte)1 : (byte)0;
                offset += 5;
            }
            return buffer;
        }

        private void transferMarked(UserInRoom user)
        {
            for (int i = 0; i < 25; ++i)
            {
                var sq = Squares[i];
                sq.Marked = CheckStatus[i].IsMarked(user);
            }
        }

        public override byte[] GetBytes(UserInRoom user)
        {
            //Update all squares to the correct color
            TransferSquareColors();
            transferMarked(user);
            return base.GetBytes(user);
        }

        /*
        public bool Uncheck(int i)
        {
            if (i < 0 || i >= 25)
                return false;
            if (CheckStatus[i].Player == null)
                return false;
            CheckStatus[i].Uncheck();
            return true;
        }
        */

        public bool ForceCheckedby(int i, UserInRoom user)
        {
            if (i < 0 || i >= 25)
                return false;
            var status = new CheckStatus();
            status.Check(user);
            if (status.Equals(CheckStatus[i]))
                return false;
            CheckStatus[i] = status;
            return true;
        }

        public bool UserClicked(int i, UserInRoom user)
        {
            if (i < 0 || i >= 25)
                return false;

            //Spectators that are not admins cannot click
            if (user.IsSpectator && !user.IsAdmin)
                return false;

            var check = CheckStatus[i];
            var status = new CheckStatus();
            status.Check(user);
            //Square not owned by any player or team, allow check
            if (check.Player == null)
            {
                if (!user.IsSpectator)
                {
                    check.Check(user);
                    return true;
                }
            }
            //Square owned by this player or team, or user is admin, so allow it to be toggled off
            else if(check.Color == status.Color || user.IsAdmin) 
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
    }

    public class CheckStatus : IEquatable<CheckStatus>
    {
        private struct PlayerTeam
        {
            public int Team;
            public Guid Player;
            public PlayerTeam(int team, Guid player)
            {
                Team = team;
                Player = player;
            }
        }

        public DateTime Time { get; init; }
        public int Team => Player?.Team ?? 0;
        public UserInRoom? Player { get; set; }
        public Color Color { get; set; }

        private IDictionary<Guid, PlayerTeam> MarkedBy { get; init; }

        public CheckStatus()
        {
            Time = DateTime.Now;
            Player = null;
            Color = Color.Empty;
            MarkedBy = new Dictionary<Guid, PlayerTeam>();
        }

        public void Check(UserInRoom user)
        {
            Player = user;
            Color = (user.Team == 0 ? Color.FromArgb(user.Color) : NetConstants.DefaultPlayerColors[user.Team - 1].Color);
        }

        public void Uncheck()
        {
            Player = null;
            Color = Color.Empty;
        }

        public bool Equals(CheckStatus other)
        {
            return Player == other.Player;
        } 

        public bool Mark(UserInRoom user)
        {
            var team = getTeamModifiedForMarking(user);
            //If no changes need to be made, return false
            if (MarkedBy.TryGetValue(user.Guid, out var pt) && pt.Player == user.Guid && pt.Team == team)
                return false;
            Unmark(user);
            MarkedBy[user.Guid] = new PlayerTeam(team, user.Guid);
            return true;
        }

        public bool Unmark(UserInRoom user)
        {
            bool changed = false;
            var team = getTeamModifiedForMarking(user);
            if (team > 0)
            {
                foreach(var pt in MarkedBy.Values.ToList())
                {
                    //Remove all markings by players on the same team
                    if (pt.Team == team)
                    {
                        MarkedBy.Remove(pt.Player);
                        changed = true;
                    }
                }
            }
            changed |= MarkedBy.Remove(user.Guid);
            return changed;
        }

        public bool IsMarked(UserInRoom user)
        {
            var team = getTeamModifiedForMarking(user);
            return MarkedBy.ContainsKey(user.Guid) || (team > 0 && MarkedBy.Values.Any(m => m.Team == team));
        }

        private int getTeamModifiedForMarking(UserInRoom user)
        {
            return user.IsSpectator ? 100 : user.Team;
        }
    }
}
