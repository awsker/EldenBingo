using EldenBingoCommon;
using System.Net;
using System.Timers;

namespace EldenBingoServer
{
    public class ServerRoom : Room<ClientInRoom>
    {
        public string AdminPassword { get; init; }
        public DateTime CreateTime { get; init; }
        private Guid _creatorGuid;
        private string? _creatorIp;
        private string _creatorName;
        private System.Timers.Timer? _timer;

        public BingoBoardGenerator BoardGenerator { get; set; }

        public event EventHandler<EventArgs> TimerElapsed;

        public ServerRoom(string name, string adminPassword, ClientModel creator): base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
            _creatorGuid = creator.UserGuid;
            _creatorIp = getClientIp(creator);
            Match.MatchStatusChanged += match_MatchStatusChanged;
        }

        private string? getClientIp(ClientModel client)
        {
            if (client.TcpClient.Client.RemoteEndPoint is IPEndPoint ip)
                return ip.Address.ToString();
            return null;
        }

        public bool IsCorrectAdminPassword(string pass)
        {
            return !string.IsNullOrWhiteSpace(AdminPassword) && AdminPassword == pass;
        }

        public bool IsAdminByDefault(ClientModel client, string name)
        {
            return client.UserGuid == _creatorGuid ||
                name == _creatorName && _creatorIp != null && _creatorIp == getClientIp(client);
        }

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

        public ClientInRoom? RemoveClient(ClientModel client)
        {
            //ClientInRoom should have same Guid as clientModel
            return RemoveClient(client.UserGuid);
        }

        public IEnumerable<ClientModel> ClientModels => Clients.Select(c => c.Client);

        private void match_MatchStatusChanged(object? sender, EventArgs e)
        {
            if(Match.MatchStatus == MatchStatus.Starting)
            {
                if(Match.MatchMilliseconds < 0)
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

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            TimerElapsed?.Invoke(_timer, EventArgs.Empty);
            stopTimer();
        }
    }
}
