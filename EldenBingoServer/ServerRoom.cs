using EldenBingoCommon;
using System.Net;
using System.Timers;

namespace EldenBingoServer
{
    public class ServerRoom : Room<ClientInRoom>
    {
        private Guid _creatorGuid;
        private string? _creatorIp;
        private string? _creatorName;
        private System.Timers.Timer? _timer;

        public ServerRoom(string name, string adminPassword, ClientModel creator) : base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
            _creatorGuid = creator.UserGuid;
            _creatorIp = Server.GetClientIp(creator);
            Match.MatchStatusChanged += match_MatchStatusChanged;
        }

        public event EventHandler? TimerElapsed;

        public string AdminPassword { get; init; }
        public BingoBoardGenerator? BoardGenerator { get; set; }
        public IEnumerable<ClientModel> ClientModels => Clients.Select(c => c.Client);
        public DateTime CreateTime { get; init; }

        public ClientInRoom AddClient(ClientModel client, string nick, string adminPass, int team)
        {
            client.Room = this;

            if (_creatorGuid == client.UserGuid)
                _creatorName = nick;
            bool admin = IsAdminByDefault(client, nick) || IsCorrectAdminPassword(adminPass);
            var cl = new ClientInRoom(client, nick, client.UserGuid, admin, team);

            AddClient(cl);
            return cl;
        }

        public bool IsAdminByDefault(ClientModel client, string name)
        {
            return client.UserGuid == _creatorGuid ||
                name == _creatorName && _creatorIp != null && _creatorIp == Server.GetClientIp(client);
        }

        public bool IsCorrectAdminPassword(string pass)
        {
            return !string.IsNullOrWhiteSpace(AdminPassword) && AdminPassword == pass;
        }

        public ClientInRoom? RemoveClient(ClientModel client)
        {
            //ClientInRoom should have same Guid as clientModel
            return RemoveClient(client.UserGuid);
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            TimerElapsed?.Invoke(_timer, EventArgs.Empty);
            stopTimer();
        }

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if (Match.MatchStatus == MatchStatus.Starting)
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
    }
}