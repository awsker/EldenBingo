using EldenBingoCommon;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;

namespace EldenBingoServer
{
    internal class MatchLog
    {
        [JsonProperty]
        private string Room { get; set; }
        [JsonProperty]
        private DateTime StartTime { get; set; }
        [JsonProperty]
        private DateTime FinishTime { get; set; }
        [JsonProperty]
        private int MatchLength { get; set; }
        [JsonProperty]
        private LTeam[] Teams { get; set; }
        [JsonProperty]
        private string[] Squares { get; set; }
        [JsonProperty]
        private MatchEvent[] Events { get; set; }

        [JsonIgnore]
        public string LogAsText { get; private set; }
        [JsonIgnore]
        public string LogAsJson { get; private set; }

        public MatchLog(ServerRoom room)
        {
            Room = room.Name;
            
            if (room.Match == null)
            {
                Console.WriteLine($"Couldn't write match log. No match in room");
                return;
            }
            if (room.Match.Board == null)
            {
                Console.WriteLine($"Couldn't write match log. No board generated in room");
                return;
            }
            StartTime = room.Match.MatchStartTime;
            FinishTime = DateTime.Now;
            MatchLength = room.Match.MatchMilliseconds;
            Squares = room.Match.Board.Squares.Select(s => s.Text).ToArray();
            var eventsList = new List<MatchEvent>(room.Match.MatchEvents);
            Events = eventsList.ToArray();
            Teams = getTeams(room);

            initLogAsText(room);

            var teamsTranslationDict = new Dictionary<int, int>();
            for (int i = 0; i < Teams.Length; ++i)
            {
                teamsTranslationDict[Teams[i].TeamIndex] = i;
                Teams[i] = Teams[i] with { TeamIndex = i };
            }
            
            // For the json file, only save actual check events. Remove all bingo events and remap teams to a sequential index
            for (int i = eventsList.Count - 1; i >= 0; --i)
            {
                var e = eventsList[i];
                if (e.EventType == MatchEventType.Bingo)
                    eventsList.RemoveAt(i);
                else
                    eventsList[i] = e with { Team = teamsTranslationDict[e.Team] };
            }
            Events = eventsList.ToArray();
            initLogAsJson();
        }

        private void initLogAsText(ServerRoom room)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Room name: {Room}");
            sb.AppendLine($"Match started at {toNiceDate(StartTime)}");
            foreach (var ev in Events)
            {
                if (ev.EventType == MatchEventType.Bingo)
                {
                    string linename = "unknown";
                    if (ev.SquareIndex < 5)
                    {
                        linename = $"column {ev.SquareIndex + 1}";
                    }
                    else if (ev.SquareIndex < 10)
                    {
                        linename = $"row {ev.SquareIndex - 4}";
                    }
                    else if (ev.SquareIndex == 10)
                    {
                        linename = $"diagonal TL->BR";
                    }
                    else if (ev.SquareIndex == 11)
                    {
                        linename = $"diagonal BL->TR";
                    }
                    sb.AppendLine($"[{Match.MatchTimeToString(ev.Timestamp / 1000)}] {ev.Player} BINGO on {linename}!");
                }
                else
                {
                    var sqText = Squares[ev.SquareIndex];
                    sb.AppendLine($"[{Match.MatchTimeToString(ev.Timestamp / 1000)}] {ev.Player} {(ev.Checked ? "marked" : "unmarked")} '{sqText}' for {room.GetTeamNameIgnoreUsers(ev.Team)}");
                }
            }
            var seconds = Math.Max(0, room.Match.MatchSeconds);
            var hours = seconds / 3600;
            seconds %= 3600;
            var minutes = seconds / 60;
            seconds %= 60;
            if (room.Match.MatchStatus == MatchStatus.Finished)
            {
                sb.AppendLine($"Match finished at {toNiceDate(FinishTime)} with a match time of {hours} hours, {minutes} minutes and {seconds} seconds");
            }
            if (Teams.Length > 0)
            {
                sb.AppendLine($"{(room.Match.MatchStatus == MatchStatus.Finished ? "Final" : "Current")} scoreboard:");
                foreach (var t in Teams)
                {
                    sb.AppendLine($"{t.Name}: {t.Score}");
                }
            }
            LogAsText = sb.ToString();
        }

        private string toNiceDate(DateTime dt)
        {
            return $"{dt.ToShortDateString()} {dt.ToShortTimeString()}";
        }

        private void initLogAsJson()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            var json = JsonConvert.SerializeObject(this, settings);
            LogAsJson = json;
        }

        public void Save(string targetDirectory, ServerRoom serverRoom)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                var fullPath = Path.Combine(targetDirectory, GenerateFileName(serverRoom.Name, true));
                File.WriteAllText(fullPath, LogAsJson);
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Couldn't write match log to directory {targetDirectory}: {ex.Message}");
            }
        }

        private LTeam[] getTeams(ServerRoom serverRoom)
        {
            var teams = new Dictionary<int, TempTeam>();

            // Fetch the current score for all teams
            IDictionary<int, int> teamScores;
            if (serverRoom.Match != null && serverRoom.Match.Board is ServerBingoBoard board)
                teamScores = board.GetScoresForAllTeams();
            else
                teamScores = new Dictionary<int, int>();
            
            // Create empty teams for all teams with checked squares or lobby presence
            foreach (var t in serverRoom.GetActiveTeams())
            {
                int score = 0;
                teamScores.TryGetValue(t.Index, out score);
                teams.Add(t.Index, new TempTeam(t.Index, t.Name, BingoConstants.GetTeamColor(t.Index),new HashSet<string>(), score));
            }
            // Assign all players in lobby to their respective teams
            foreach (var user in serverRoom.Users)
            {
                if (!user.IsSpectator && teams.TryGetValue(user.Team, out var t))
                {
                    t.Players.Add(user.Nick);
                }
            }
            // Go through all events and add players to teams that had activity during the match but might no longer be present
            foreach (var e in Events)
            {
                if (teams.TryGetValue(e.Team, out var t))
                {
                    if (e.EventType == MatchEventType.PlayerCheck && e.Checked)
                    {
                        t.Players.Add(e.Player);
                    }
                }
                else 
                {
                    var playerSet = new HashSet<string>();
                    if (e.EventType == MatchEventType.PlayerCheck && e.Checked)
                    {
                        playerSet.Add(e.Player);
                    }
                    int score = 0;
                    teamScores.TryGetValue(e.Team, out score);
                    teams.Add(e.Team, new TempTeam(e.Team, serverRoom.GetTeamNameIgnoreUsers(e.Team), BingoConstants.GetTeamColor(e.Team), playerSet, score));
                }
            }
            return teams.Values.Select(t => new LTeam(t.TeamIndex, t.Name, t.Color, t.Players.ToArray(), t.Score)).ToArray();
        }

        public string GenerateFileName(string roomName, bool json)
        {
            // Date prefix
            string timestamp = FinishTime.ToString("yyyy-MM-dd-HH-mm");

            // Replace invalid characters with empty strings.
            var invalid = Path.GetInvalidFileNameChars();
            roomName = new string(roomName
                .Select(ch => invalid.Contains(ch) ? '_' : ch)
                .ToArray())
                .Trim();
            return $"{timestamp}_{roomName}." + (json ? "json" : "txt");
        }

    }

    internal record TempTeam(int TeamIndex, string Name, Color Color, ISet<string> Players, int Score);
    internal record LTeam(int TeamIndex, string Name, Color Color, string[] Players, int Score);
}
