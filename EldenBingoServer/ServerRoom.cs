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
        private Dictionary<int, string> _customTeamNames;

        public ServerRoom(string name, string adminPassword, ClientModel creator, BingoGameSettings gameSettings) : base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
            _creatorGuid = creator.ClientGuid;
            _creatorIp = Server.GetClientIp(creator);
            Match.MatchStatusChanged += match_MatchStatusChanged;
            GameSettings = gameSettings;
            LastActivity = DateTime.Now;
            _customTeamNames = new Dictionary<int, string>();
        }

        public event EventHandler<RoomEventArgs>? TimerElapsed;

        public string AdminPassword { get; init; }
        public BingoBoardGenerator? BoardGenerator { get; set; }
        public IEnumerable<BingoClientModel> ClientModels => Users.Select(c => c.Client);
        public DateTime CreateTime { get; init; }
        public DateTime LastActivity { get; set; }
        public BingoGameSettings GameSettings { get; set; }
        public bool BoardAlreadyUsed { get; set; }

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
                    if (sq.Team.HasValue && !dict.ContainsKey(sq.Team.Value))
                    {
                        dict.Add(sq.Team.Value, GetUnifiedName(sq.Team.Value, new BingoClientInRoom[0]));
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

        /*
        /// <summary>
        /// Get number of checked squares per team
        /// </summary>
        /// <returns>Team, TeamName, Count</returns>
        public IList<TeamScore> GetSquaresPerTeam()
        {
            var list = new List<TeamScore>();
            var activeTeams = GetActiveTeams();
            foreach (var team in GetActiveTeams())
            {
                list.Add(new TeamScore(team.Index, team.Name, 0));
            }
            if (Match?.Board == null)
            {
                return list;
            }
            var squaresCountPerTeam = getSquaresPerTeam();
            //var bingosPerTeam = getBingosPerTeam(activeTeams);
            for (int i = 0; i < list.Count; ++i)
            {
                if (squaresCountPerTeam.TryGetValue(list[i].Team, out int squares))
                {
                    var score = list[i];
                    score.Score += squares;
                    list[i] = score;
                }
                if (bingosPerTeam.TryGetValue(list[i].Team, out var bingoLines))
                {
                    var score = list[i];
                    score.Score += bingoLines.Count * GameSettings.PointsPerBingoLine;
                    list[i] = score;
                }
            }
            return list;
        }*/

        public Dictionary<int, int> GetSquaresPerTeam()
        {
            var squaresCountPerTeam = new Dictionary<int, int>();
            if (Match.Board is not ServerBingoBoard board)
            {
                return squaresCountPerTeam;
            }
            foreach (var square in board.CheckStatus)
            {
                if (!square.Team.HasValue)
                    continue;

                if (squaresCountPerTeam.TryGetValue(square.Team.Value, out int c))
                {
                    squaresCountPerTeam[square.Team.Value] = c + 1;
                }
                else
                {
                    squaresCountPerTeam[square.Team.Value] = 1;
                }
            }
            return squaresCountPerTeam;
        }

        public IDictionary<int, IList<BingoLine>> GetBingos()
        {
            if(Match.Board is not ServerBingoBoard board)
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