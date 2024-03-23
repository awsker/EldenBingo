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

        public virtual void AddUser(T user)
        {
            UsersDict[user.Guid] = user;
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

        public virtual bool RemoveUser(T client)
        {
            return UsersDict.Remove(client.Guid, out _);
        }

        public T? RemoveUser(Guid guid)
        {
            UsersDict.Remove(guid, out var obj);
            return obj ?? null;
        }

        protected string GetUnifiedName(int team, IList<T> teamPlayers)
        {
            string shortestName = string.Empty;
            for (int i = 0; i < teamPlayers.Count; ++i)
            {
                if (i == 0 || teamPlayers[i].Nick.Length < shortestName.Length)
                    shortestName = teamPlayers[i].Nick;
            }
            //If all names starts with the same sequence (CptDomo, CptDomo2, CptDomo-Spec etc..),
            //use the shortest of these as the team name
            if (!string.IsNullOrWhiteSpace(shortestName) && teamPlayers.All(p => p.Nick.StartsWith(shortestName)))
                return shortestName;
            return BingoConstants.GetTeamName(team);
        }
    }
}