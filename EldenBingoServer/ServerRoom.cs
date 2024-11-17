using EldenBingoCommon;
using Neto.Shared;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Timers;

namespace EldenBingoServer
{
    public class ServerRoom : Room<BingoClientInRoom>
    {
        [JsonProperty]
        private Guid _creatorGuid;
        private System.Timers.Timer? _timer;
        [JsonProperty]
        private Dictionary<int, string> _customTeamNames;

        public ServerRoom(string name, string adminPassword, ClientModel creator, BingoGameSettings gameSettings) : base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
            Match.MatchStatusChanged += match_MatchStatusChanged;
            GameSettings = gameSettings;
            LastActivity = DateTime.Now;
            _creatorGuid = creator?.ClientGuid ?? Guid.Empty;
            _customTeamNames = new Dictionary<int, string>();
        }

        public event EventHandler<RoomEventArgs>? TimerElapsed;
        [JsonProperty]
        public string AdminPassword { get; init; }
        [JsonIgnore]
        public BingoBoardGenerator? BoardGenerator { get; set; }

        [JsonIgnore]
        public IEnumerable<BingoClientModel> ClientModels => Users.Select(c => c.Client);
        [JsonProperty]
        public DateTime CreateTime { get; set; }
        [JsonProperty]
        public DateTime LastActivity { get; set; }
        [JsonProperty]
        public BingoGameSettings GameSettings { get; set; }
        [JsonProperty]
        public bool BoardAlreadyUsed { get; set; }

        public BingoClientInRoom AddUser(BingoClientModel client, string nick, string adminPass, int team)
        {
            client.Room = this;

            bool admin = IsAdminByDefault(client) || IsCorrectAdminPassword(adminPass);
            var cl = new BingoClientInRoom(client, nick, client.ClientGuid, admin, team);

            AddUser(cl);
            updateLastActivity();
            return cl;
        }

        public bool IsAdminByDefault(BingoClientModel client)
        {
            return client.ClientGuid == _creatorGuid;
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

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (!Match.Paused)
            {
                UnpauseMatch();
            }
            Match.MatchStatusChanged += match_MatchStatusChanged;
            if(Match.Board is ServerBingoBoard sb)
            {
                sb.Room = this;
            }
        }

        public IList<Team> GetActiveTeams()
        {
            var teams = Users.ToLookup(p => p.Team);
            var dict = new Dictionary<int, string>();
            foreach (var team in teams)
            {
                if (team.Key == -1)
                    continue;
                var teamPlayers = team.ToList();
                if (_customTeamNames.TryGetValue(team.Key, out var teamName) && !string.IsNullOrWhiteSpace(teamName))
                    dict.Add(team.Key, teamName);
                else if (teamPlayers.Count == 1)
                    dict.Add(team.Key, teamPlayers[0].Nick);
                else
                    dict.Add(team.Key, GetUnifiedName(team.Key, teamPlayers));
            }
            if (Match.Board is ServerBingoBoard serverboard)
            {
                foreach (var sq in serverboard.CheckStatus)
                {
                    foreach (var team in sq.Team)
                    {
                        if (!dict.ContainsKey(team))
                        {
                            dict.Add(team, GetUnifiedName(team, new BingoClientInRoom[0]));
                        }
                    }
                }
            }
            return dict.Select(kv => new Team(kv.Key, kv.Value)).OrderBy(pt => pt.Index).ToList();
        }

        public string GetTeamNameIgnoreUsers(int team)
        {
            var usersOnTeam = Users.Where(t => t.Team == team).ToList();
            if (_customTeamNames.TryGetValue(team, out var teamName) && !string.IsNullOrWhiteSpace(teamName))
                return teamName;
            else
                return BingoConstants.GetTeamName(team);
        }

        public Dictionary<int, int> GetSquaresPerTeam()
        {
            if (Match.Board is not ServerBingoBoard board)
                return new Dictionary<int, int>();
            return board.GetSquaresPerTeam();
        }

        public IDictionary<int, IList<BingoLine>> GetBingos()
        {
            if (Match.Board is not ServerBingoBoard board)
                return new Dictionary<int, IList<BingoLine>>();
            return board.GetBingosPerTeam(GetActiveTeams());
        }

        public void SetTeamName(int team, string name)
        {
            _customTeamNames[team] = name;
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