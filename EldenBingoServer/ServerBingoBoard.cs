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
        }

        private void updateSquareColors()
        {
            for (int i = 0; i < 25; ++i)
            {
                var status = CheckStatus[i];
                Squares[i].Color = status.Player == null ? System.Drawing.Color.Empty : (status.Player.Team == 0 ?
                    status.Player.ConvertedColor : NetConstants.DefaultPlayerColors[status.Player.Team - 1].Color);
            }
        }

        public byte[] GetColorBytes()
        {
            updateSquareColors();
            byte[] buffer = new byte[25 * 4];
            int offset = 0;
            foreach(var sq in Squares)
            {
                Array.Copy(BitConverter.GetBytes(sq.Color.ToArgb()), 0, buffer, offset, 4);
                offset += 4;
            }
            return buffer;
        }

        public override byte[] GetBytes()
        {
            //Update all squares to the correct color
            updateSquareColors();
            return base.GetBytes();
        }

        public bool Uncheck(int i)
        {
            if (i < 0 || i >= 25)
                return false;
            var status = new CheckStatus();
            if (status.Equals(CheckStatus[i]))
                return false;
            CheckStatus[i] = status;
            return true;
        }

        public bool ForceCheckedby(int i, UserInRoom user)
        {
            if (i < 0 || i >= 25)
                return false;
            var status = new CheckStatus(user);
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

            var previousCheck = CheckStatus[i];
            var userCheck = new CheckStatus(user);
            //Square not owned by any player or team, allow check
            if(previousCheck.Player == null)
            {
                if (!user.IsSpectator)
                {
                    CheckStatus[i] = new CheckStatus(user);
                    return true;
                }
            }
            //Square owned by this player or team, or user is admin, so allow it to be toggled off
            else if(previousCheck.Color == userCheck.Color || user.IsAdmin) 
            {
                CheckStatus[i] = new CheckStatus();
                return true;
            }
            //Not allowed to toggle square, owned by other player/team
            return false;
        }
    }

    public struct CheckStatus : IEquatable<CheckStatus>
    {
        public DateTime Time { get; init; }
        public int Team => Player?.Team ?? 0;
        public UserInRoom? Player { get; init; }
        public Color Color { get; init; }

        public CheckStatus(UserInRoom user)
        {
            Time = DateTime.Now;
            Player = user;
            Color = (user.Team == 0 ? Color.FromArgb(user.Color) : NetConstants.DefaultPlayerColors[user.Team - 1].Color);
        }

        public bool Equals(CheckStatus other)
        {
            return Player == other.Player;
        }
    }
}
