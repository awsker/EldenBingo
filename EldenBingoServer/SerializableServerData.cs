using Neto.Server;
using System.Collections.Concurrent;

namespace EldenBingoServer
{
    public record SerializableServerData(int Version, ConcurrentDictionary<string, ServerRoom> Rooms, ConcurrentDictionary<string, ClientIdentity> Identities);
}
