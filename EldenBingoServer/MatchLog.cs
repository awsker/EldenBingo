using EldenBingoCommon;
using Newtonsoft.Json;
using System.Drawing;

namespace EldenBingoServer
{
    internal class MatchLog
    {
        public string Room { get; set; }
        public DateTime DateTime { get; set; }
        public int MatchLength { get; set; }
        public LTeam[] Teams { get; set; }
        public string[] Squares { get; set; }
        public LEvent[] Events { get; set; }


        public MatchLog()
        {
            DateTime = DateTime.Now;
            Teams = Array.Empty<LTeam>();
            Squares = Array.Empty<string>();
            Events = Array.Empty<LEvent>();
        }

        public void Save(string targetDirectory, ServerRoom serverRoom)
        {
            if (serverRoom.Match == null)
            {
                Console.WriteLine($"Couldn't write match log. No match in room");
                return;
            }
            if (serverRoom.Match.Board == null)
            {
                Console.WriteLine($"Couldn't write match log. No board generated in room");
                return;
            }
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                DateTime = DateTime.Now;
                MatchLength = serverRoom.Match.MatchSeconds;
                Teams = getTeams(serverRoom);
                var teamsTranslationDict = new Dictionary<int, int>();
                for (int i = 0; i < Teams.Length; ++i)
                {
                    teamsTranslationDict[Teams[i].TeamIndex] = i;
                }
                Squares = serverRoom.Match.Board.Squares.Select(s => s.Text).ToArray();
                for (int i = 0; i < Events.Length; ++i)
                {
                    var e = Events[i];
                    Events[i] = e with { Team = teamsTranslationDict[e.Team] };
                }
                var settings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                };
                var json = JsonConvert.SerializeObject(this, settings);
                var fullPath = Path.Combine(targetDirectory, generateFileName());
                File.WriteAllText(fullPath, json);
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Couldn't write match log to directory {targetDirectory}: {ex.Message}");
            }
        }

        private LTeam[] getTeams(ServerRoom serverRoom)
        {
            var teams = new Dictionary<int, TempTeam>();
            // Create empty teams for all teams with checked squares or lobby presence
            foreach (var t in serverRoom.GetActiveTeams())
            {
                teams.Add(t.Index, new TempTeam(t.Index, t.Name, BingoConstants.GetTeamColor(t.Index),new HashSet<string>()));
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
                    if (!e.Referee && e.Checked)
                    {
                        t.Players.Add(e.Player);
                    }
                }
                else 
                {
                    var playerSet = new HashSet<string>();
                    if (!e.Referee && e.Checked)
                    {
                        playerSet.Add(e.Player);
                    }
                    teams.Add(e.Team, new TempTeam(e.Team, serverRoom.GetTeamNameIgnoreUsers(e.Team), BingoConstants.GetTeamColor(e.Team), playerSet));
                }
            }
            return teams.Values.Select(t => new LTeam(t.TeamIndex, t.Name, t.Color, t.Players.ToArray())).ToArray();
        }

        private string generateFileName()
        {
            // Date prefix
            string timestamp = DateTime.ToString("yyyy-MM-dd-HH-mm");

            // Join all team names with underscores
            string teamNames = string.Join("_", Teams.Select(t => sanitizeTeamName(t.Name)));

            // Final filename
            return $"{timestamp}_{teamNames}.json";
        }

        private string sanitizeTeamName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Replace(' ', '_'); // Replace spaces as well
        }
    }
    internal record TempTeam(int TeamIndex, string Name, Color Color, ISet<string> Players);
    internal record LTeam([property: JsonIgnore] int TeamIndex, string Name, Color Color, string[] Players);
    internal record class LEvent(int Timestamp, int SquareIndex, int Team, string Player, bool Checked, bool Referee);
}
