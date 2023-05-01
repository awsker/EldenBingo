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

        public IList<(PlayerTeam, int)> GetCheckedSquaresPerPlayerTeam()
        {
            var list = new List<(PlayerTeam, int)>();
            foreach (var pt in PlayerTeam.GetPlayerTeams(Clients, out _))
            {
                list.Add(new(pt, 0));
            }
            if (Match?.Board == null)
            {
                return list;
            }
            var dict = new Dictionary<PlayerTeam, int>();
            foreach(var square in Match.Board.Squares.Where(s => s.Checked))
            {
                if(dict.TryGetValue(square.CheckOwner, out int c))
                {
                    dict[square.CheckOwner] = c + 1;
                } 
                else
                {
                    dict[square.CheckOwner] = 1;
                }
            }
            for(int i = 0; i < list.Count; ++i)
            {
                if (dict.TryGetValue(list[i].Item1, out int c))
                {
                    list[i] = (list[i].Item1, c);
                }
            }
            return list;
        }
    }
}
