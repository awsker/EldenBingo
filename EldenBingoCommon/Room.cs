using System.Collections.Concurrent;

namespace EldenBingoCommon
{
    public class Room<T> where T : UserInRoom
    {
        public Room(string name)
        {
            Name = name;
            UsersDict = new ConcurrentDictionary<Guid, T>();
            Match = new Match();
        }

        public ICollection<T> Users => UsersDict.Values;
        public Match Match { get; init; }
        public string Name { get; init; }
        public int NumUsers => UsersDict.Count;
        protected ConcurrentDictionary<Guid, T> UsersDict { get; init; }

        public static IList<(int, string)> GetPlayerTeams(IEnumerable<T> players)
        {
            var teams = players.ToLookup(p => p.Team);
            var list = new List<(int, string)>();
            foreach (var team in teams)
            {
                if (team.Key == -1)
                    continue;
                var teamPlayers = team.ToList();
                if (teamPlayers.Count == 1)
                    list.Add(new(team.Key, teamPlayers[0].Nick));
                else if (teamPlayers.Count > 1)
                    list.Add(new(team.Key, getUnifiedName(team.Key, teamPlayers)));
            }

            return list.OrderBy(pt => pt.Item1).ToList();
        }

        private static string getUnifiedName(int team, IList<T> teamPlayers)
        {
            string shortestName = string.Empty;
            for(int i = 0; i < teamPlayers.Count; ++i)
            {
                if (i == 0 || teamPlayers[i].Nick.Length < shortestName.Length)
                    shortestName = teamPlayers[i].Nick;
            }
            //If all names starts with the same sequence (CptDomo, CptDomo2, CptDomo-Spec etc..),
            //use the shortest of these as the team name
            if (teamPlayers.All(p => p.Nick.StartsWith(shortestName)))
                return shortestName;
            return BingoConstants.GetTeamName(team);
        }

        public virtual void AddUser(T user)
        {
            UsersDict[user.Guid] = user;
        }

        /// <summary>
        /// Get number of checked squares per team
        /// </summary>
        /// <returns>Team, TeamName, Count</returns>
        public IList<(int, string, int)> GetCheckedSquaresPerTeam()
        {
            var list = new List<(int, string, int)>();
            foreach (var pt in GetPlayerTeams())
            {
                list.Add(new(pt.Item1, pt.Item2, 0));
            }
            if (Match?.Board == null)
            {
                return list;
            }
            var dict = new Dictionary<int, int>();
            foreach (var square in Match.Board.Squares)
            {
                if (!square.Team.HasValue)
                    continue;

                if (dict.TryGetValue(square.Team.Value, out int c))
                {
                    dict[square.Team.Value] = c + 1;
                }
                else
                {
                    dict[square.Team.Value] = 1;
                }
            }
            for (int i = 0; i < list.Count; ++i)
            {
                if (dict.TryGetValue(list[i].Item1, out int c))
                {
                    list[i] = (list[i].Item1, list[i].Item2, c);
                }
            }
            return list;
        }

        public T? GetUser(Guid userGuid)
        {
            return UsersDict.TryGetValue(userGuid, out var user) ? user : null;
        }

        public virtual IEnumerable<T> GetClientsSorted()
        {
            var cmp = new UserComparer<T>();
            return UsersDict.Values.OrderBy(u => u, cmp).ToList();
        }

        public IList<(int, string)> GetPlayerTeams()
        {
            return GetPlayerTeams(Users);
        }

        public virtual bool RemoveUser(T client)
        {
            return UsersDict.Remove(client.Guid, out _);
        }

        public T? RemoveUser(Guid guid)
        {
            UsersDict.Remove(guid, out var obj);
            return obj ?? null;
        }
    }
}