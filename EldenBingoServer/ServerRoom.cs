using EldenBingoCommon;

namespace EldenBingoServer
{
    internal class ServerRoom : Room<ClientInRoom>
    {
        public string AdminPassword { get; init; }
        public DateTime CreateTime { get; init; }

        public BingoBoardGenerator BoardGenerator { get; set; }

        public ServerRoom(string name, string adminPassword): base(name)
        {
            AdminPassword = adminPassword;
            CreateTime = DateTime.Now;
        }

        public bool IsCorrectAdminPassword(string pass)
        {
            return !string.IsNullOrWhiteSpace(AdminPassword) && AdminPassword == pass;
        }

        public ClientInRoom AddClient(ClientModel client, string nick, int color, bool admin, int team, bool spectator)
        {
            client.Room = this;
            var cl = new ClientInRoom(client, nick, client.UserGuid, color, admin, team, spectator);
            AddClient(cl);
            return cl;
        }

        public ClientInRoom? RemoveClient(ClientModel client)
        {
            //ClientInRoom should have same Guid as clientModel
            return RemoveClient(client.UserGuid);
        }

        public IEnumerable<ClientModel> ClientModels => Clients.Select(c => c.Client);
    }
}
