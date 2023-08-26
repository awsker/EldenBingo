using EldenBingoCommon;
using Neto.Shared;
using System.Timers;

namespace EldenBingoServer
{
    public class ServerRoom : Room<BingoClientInRoom>
    {
        private Guid _creatorGuid;
        private string? _creatorIp;
        private string? _creatorName;
        private System.Timers.Timer? _timer;

        public ServerRoom(string name, string adminPassword, ClientModel creator, BingoGameSettings gameSettings) : base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
            _creatorGuid = creator.ClientGuid;
            _creatorIp = Server.GetClientIp(creator);
            Match.MatchStatusChanged += match_MatchStatusChanged;
            GameSettings = gameSettings;
            LastActivity = DateTime.Now;
        }

        public event EventHandler<RoomEventArgs>? TimerElapsed;

        public string AdminPassword { get; init; }
        public BingoBoardGenerator? BoardGenerator { get; set; }
        public IEnumerable<BingoClientModel> ClientModels => Users.Select(c => c.Client);
        public DateTime CreateTime { get; init; }
        public DateTime LastActivity { get; set; }
        public BingoGameSettings GameSettings { get; set; }

        public BingoClientInRoom AddUser(BingoClientModel client, string nick, string adminPass, int team)
        {
            client.Room = this;

            if (_creatorGuid == client.ClientGuid)
                _creatorName = nick;
            bool admin = IsAdminByDefault(client, nick) || IsCorrectAdminPassword(adminPass);
            var cl = new BingoClientInRoom(client, nick, client.ClientGuid, admin, team);

            AddUser(cl);
            updateLastActivity();
            return cl;
        }

        public bool IsAdminByDefault(BingoClientModel client, string name)
        {
            return client.ClientGuid == _creatorGuid ||
                name == _creatorName && _creatorIp != null && _creatorIp == Server.GetClientIp(client);
        }

        public bool IsCorrectAdminPassword(string pass)
        {
            return !string.IsNullOrWhiteSpace(AdminPassword) && AdminPassword == pass;
        }

        public BingoClientInRoom? RemoveUser(BingoClientModel client)
        {
            updateLastActivity();
            //ClientInRoom should have same Guid as clientModel
            return RemoveUser(client.ClientGuid);
        }

        public void PauseMatch()
        {
            Match.Pause();
            stopTimer();
        }

        public void UnpauseMatch()
        {
            Match.Unpause();
            if (Match.MatchMilliseconds < 0 && (Match.MatchStatus == MatchStatus.Starting || Match.MatchStatus == MatchStatus.Preparation))
            {
                restartAndListenToTimer(Match.MatchMilliseconds * -1);
            }
            else
            {
                stopTimer();
            }
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            stopTimer();
            TimerElapsed?.Invoke(_timer, new RoomEventArgs(this));
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if (Match.MatchStatus == MatchStatus.Starting)
            {
                if (Match.MatchMilliseconds < 0)
                    restartAndListenToTimer(Match.MatchMilliseconds * -1);
            }
            else if (Match.MatchStatus == MatchStatus.Preparation)
            {
                if (Match.MatchMilliseconds < 0)
                    restartAndListenToTimer(Match.MatchMilliseconds * -1);
            }
            else
            {
                stopTimer();
            }
        }

        private void restartAndListenToTimer(int milliseconds)
        {
            stopTimer();
            _timer = new System.Timers.Timer();
            _timer.Interval = milliseconds;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void stopTimer()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= _timer_Elapsed;
                _timer.Stop();
            }
        }

        private void updateLastActivity()
        {
            LastActivity = DateTime.Now;
        }
    }
}