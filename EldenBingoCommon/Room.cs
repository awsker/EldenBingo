using System.Collections.Concurrent;

namespace EldenBingoCommon
{
    public class Room<T> : INetSerializable where T : UserInRoom 
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

        public virtual byte[] GetBytes()
        {
            var byteList = new List<byte[]>();
            byteList.Add(PacketHelper.GetStringBytes(Name));
            byteList.Add(BitConverter.GetBytes(NumClients));
            foreach(var cl in Clients)
            {
                byteList.Add(cl.GetBytes());
            }
            byteList.Add(Match.GetBytes());
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
            var cmp = new UserComparer();
            return clients.Values.OrderBy(u => u, cmp).ToList();
        }

        public T? GetClient(Guid userGuid)
        {
            return clients.TryGetValue(userGuid, out var user) ? user : null;
        }


        private class UserComparer : IComparer<T>
        {
            public int Compare(T? x, T? y)
            {
                var xval = 0;
                var yval = 0;
                if (x == null || y == null)
                    return 0;
                if (x.IsSpectator)
                    xval += x.IsAdmin ? 1 : 1000;
                else
                    xval += x.Team * 10;
                if (y.IsSpectator)
                    yval += y.IsAdmin ? 1 : 1000;
                else
                    yval += y.Team * 10;
                if (x.IsAdmin)
                    xval -= 5;
                if (y.IsAdmin)
                    yval -= 5;

                if (xval == yval) //Exact same rank, sort by nickname
                    return x.Nick.CompareTo(y.Nick);
                else return xval - yval;
            }
        }

    }
}
