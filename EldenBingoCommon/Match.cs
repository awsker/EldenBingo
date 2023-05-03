using System.Drawing;

namespace EldenBingoCommon
{
    public enum MatchStatus
    {
        NotRunning,
        Starting,
        Running,
        Paused,
        Finished
    }

    public class Match
    {
        public Match()
        {
            MatchStatus = MatchStatus.NotRunning;
            ServerTimer = 0;
            updateMatchStatus();
        }

        public Match(byte[] buffer, ref int offset)
        {
            MatchStatus = (MatchStatus)PacketHelper.ReadByte(buffer, ref offset);
            ServerTimer = PacketHelper.ReadInt(buffer, ref offset);
            var hasBoard = PacketHelper.ReadBoolean(buffer, ref offset);
            if (hasBoard)
                Board = new BingoBoard(buffer, ref offset);
        }

        public event EventHandler? MatchStatusChanged;

        public BingoBoard? Board { get; set; }

        public int MatchMilliseconds
        {
            get
            {
                return ServerTimer + (Running ? Convert.ToInt32((DateTime.Now - StatusChangedLocalDateTime).TotalMilliseconds) : 0);
            }
        }

        public int MatchSeconds
        {
            get
            {
                return Convert.ToInt32(Math.Floor(MatchMilliseconds / 1000d));
            }
        }

        public MatchStatus MatchStatus { get; private set; }
        public bool Running => MatchStatus == MatchStatus.Starting || MatchStatus == MatchStatus.Running;
        public int ServerTimer { get; private set; }
        public DateTime StatusChangedLocalDateTime { get; private set; }
        //public event EventHandler? MatchTimerChanged;

        //private System.Timers.Timer? _timer;
        public string TimerString
        {
            get
            {
                var seconds = MatchSeconds;
                bool negative = seconds < 0;
                if (negative)
                {
                    seconds = -seconds;
                }
                var hours = seconds / 3600;
                seconds %= 3600;
                var minutes = seconds / 60;
                seconds %= 60;
                return $"{(negative ? "-" : string.Empty)}{hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";
            }
        }

        public static string MatchStatusToString(MatchStatus status, out Color color)
        {
            switch (status)
            {
                case MatchStatus.NotRunning:
                    color = Color.CadetBlue;
                    return "Not Running";

                case MatchStatus.Starting:
                    color = Color.Orange;
                    return "Starting...";

                case MatchStatus.Running:
                    color = Color.Green;
                    return "Running";

                case MatchStatus.Paused:
                    color = Color.CadetBlue;
                    return "Paused";

                case MatchStatus.Finished:
                    color = Color.CadetBlue;
                    return "Match Finished";
            }
            color = Color.White;
            return string.Empty;
        }

        public byte[] GetBytes(UserInRoom user)
        {
            return PacketHelper.ConcatBytes(
                new[] { (byte)MatchStatus },
                BitConverter.GetBytes(MatchMilliseconds),
                BitConverter.GetBytes(Board != null), //Include bingo board
                Board?.GetBytes(user) ?? Array.Empty<byte>());
        }

        public byte[] GetBytesWithoutBoard()
        {
            return PacketHelper.ConcatBytes(
                new[] { (byte)MatchStatus },
                BitConverter.GetBytes(MatchMilliseconds),
                BitConverter.GetBytes(false)); //Don't include bingo board
        }

        public void UpdateMatchStatus(MatchStatus status, int timer, BingoBoard? board = null)
        {
            ServerTimer = timer;
            StatusChangedLocalDateTime = DateTime.Now;
            MatchStatus = status;
            if (board != null)
                Board = board;
            onMatchStatusChanged();
        }

        private void onMatchStatusChanged()
        {
            MatchStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void updateMatchStatus()
        {
            UpdateMatchStatus(MatchStatus, ServerTimer, Board);
        }
    }
}