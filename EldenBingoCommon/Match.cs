using System.Drawing;

namespace EldenBingoCommon
{
    public class Match
    {
        public BingoBoard? Board { get; set; }
        public MatchStatus MatchStatus { get; private set; }
        //public DateTime StatusChangedLocalDateTime { get; private set; }
        public int ServerTimer { get; private set; }

        public event EventHandler? MatchStatusChanged;
        public event EventHandler? MatchTimerChanged;

        private System.Timers.Timer? _timer;

        public bool Running => MatchStatus == MatchStatus.Starting || MatchStatus == MatchStatus.Running;

        public int MatchSeconds
        {
            get
            {
                return ServerTimer;
            }
        }

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
                return $"{(negative ? "-": string.Empty)}{hours.ToString("00")}:{minutes.ToString("00")}:{seconds.ToString("00")}";
            }
        }

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
            if(hasBoard)
                Board = new BingoBoard(buffer, ref offset);
        }

        private void updateMatchStatus()
        {
            UpdateMatchStatus(MatchStatus, ServerTimer, Board);
        }

        public void UpdateMatchStatus(MatchStatus status, int timer, BingoBoard? board = null)
        {
            bool statusChanged = false, timerChanged = false;

            if (MatchStatus != status)
            {
                MatchStatus = status;
                statusChanged = true;
                onMatchStatusChanged();
            }
            if(ServerTimer != timer)
            {
                ServerTimer = timer;
                timerChanged = true;
            }
            if(statusChanged)
                onMatchStatusChanged();
            
            if (board != null)
                Board = board;
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            if(_timer == null && Running)
            {
                _ = Task.Run(() => {
                    onMatchTimerChanged();
                    _timer = new System.Timers.Timer(1000);
                    _timer.Elapsed += (o, e) =>
                    {
                        ServerTimer += 1;
                        onMatchTimerChanged();
                    };
                    _timer.Start();
                });
            } 
            else if (timerChanged)
                onMatchTimerChanged();
        }

        private void onMatchStatusChanged()
        {
            MatchStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void onMatchTimerChanged()
        {
            MatchTimerChanged?.Invoke(this, EventArgs.Empty);
        }

        public byte[] GetBytes(UserInRoom user)
        {
            return PacketHelper.ConcatBytes(
                new[] { (byte)MatchStatus },
                BitConverter.GetBytes(MatchSeconds),
                BitConverter.GetBytes(Board != null), //Include bingo board
                Board?.GetBytes(user) ?? Array.Empty<byte>());
        }

        public byte[] GetBytesWithoutBoard()
        {
            return PacketHelper.ConcatBytes(
                new[] { (byte)MatchStatus },
                BitConverter.GetBytes(MatchSeconds),
                BitConverter.GetBytes(false)); //Don't include bingo board
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
    }

    public enum MatchStatus
    {
        NotRunning,
        Starting,
        Running,
        Paused,
        Finished
    }

    
}
