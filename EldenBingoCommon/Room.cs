using System.Collections.Concurrent;

namespace EldenBingoCommon
{
    public class Room<T> where T : UserInRoom
    {
        public string Name { get; init; }
        public Match Match { get; init; }

        protected ConcurrentDictionary<Guid, T> clients { get; init; }
        public ICollection<T> Clients => clients.Values;

        public int NumClients => clients.Count;

        public Room(string name)
        {
            Name = name;
            clients = new ConcurrentDictionary<Guid, T>();
            Match = new Match();
        }

        public virtual byte[] GetBytes(UserInRoom user)
        {
            var byteList = new List<byte[]>();
            byteList.Add(PacketHelper.GetStringBytes(Name));
            byteList.Add(BitConverter.GetBytes(NumClients));
            foreach(var cl in Clients)
            {
                byteList.Add(cl.GetBytes());
            }
            var includeBoard = Match.MatchStatus >= MatchStatus.Running || user.IsAdmin && user.IsSpectator;
            byteList.Add(includeBoard ? Match.GetBytes(user) : Match.GetBytesWithoutBoard());
            return PacketHelper.ConcatBytes(byteList);
        }

        public virtual void AddClient(T user)
        {
            clients[user.Guid] = user;
        }

        public virtual bool RemoveClient(T client)
        {
            return clients.Remove(client.Guid, out _);
        }

        public T? RemoveClient(Guid guid)
        {
            clients.Remove(guid, out var obj);
            return obj ?? null;
        }

        public virtual IEnumerable<T> GetClientsSorted()
        {
            var cmp = new UserComparer<T>();
            return clients.Values.OrderBy(u => u, cmp).ToList();
        }

        public T? GetClient(Guid userGuid)
        {
            return clients.TryGetValue(userGuid, out var user) ? user : null;
        }

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
                else if(teamPlayers.Count > 1)
                    list.Add(new(team.Key, NetConstants.GetTeamName(team.Key)));
            }

            return list.OrderBy(pt => pt.Item1).ToList();
        }

        public IList<(int, string)> GetPlayerTeams()
        {
            return GetPlayerTeams(Clients);
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
            foreach(var square in Match.Board.Squares.Where(s => s.Team.HasValue))
            {
                if(dict.TryGetValue(square.Team.Value, out int c))
                {
                    dict[square.Team.Value] = c + 1;
                } 
                else
                {
                    dict[square.Team.Value] = 1;
                }
            }
            for(int i = 0; i < list.Count; ++i)
            {
                if (dict.TryGetValue(list[i].Item1, out int c))
                {
                    list[i] = (list[i].Item1, list[i].Item2, c);
                }
            }
            return list;
        }
    }
}
