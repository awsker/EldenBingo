using System.Drawing;
using System.Diagnostics.CodeAnalysis;

namespace EldenBingoCommon
{
    public struct PlayerTeam : IEqualityComparer<PlayerTeam>, INetSerializable
    {
        public int Team;
        public Guid Player;
        public Color Color;
        public PlayerTeam(int team, Guid player, Color c)
        {
            Team = team;
            Player = player;
            Color = c;
        }

        public PlayerTeam(UserInRoom user)
        {
            Team = user.IsSpectator ? 1000 : user.Team;
            Player = user.Guid;
            Color = user.ConvertedColor;
        }

        public PlayerTeam(byte[] bytes, ref int offset)
        {
            Team = PacketHelper.ReadInt(bytes, ref offset);
            Player = PacketHelper.ReadGuid(bytes, ref offset);
            Color = Color.FromArgb(PacketHelper.ReadInt(bytes, ref offset));
        }

        public static IList<PlayerTeam> GetPlayerTeams(IEnumerable<UserInRoom> users, out ISet<int> teams)
        {
            var cmp = new UserComparer<UserInRoom>();
            var sortedList = users.OrderBy(c => c, cmp).ToList();

            var list = new List<PlayerTeam>();
            teams = new HashSet<int>();
            foreach (var user in sortedList)
            {
                if (!user.IsSpectator && (user.Team == 0 || !teams.Contains(user.Team)))
                {
                    list.Add(new PlayerTeam(user));
                }
                if (user.Team > 0)
                    teams.Add(user.Team);
            }
            return list;
        }

        public bool Equals(PlayerTeam x, PlayerTeam y)
        {
            return (x.Team > 0 && x.Team == y.Team || x.Player == y.Player);
        }

        public int GetHashCode([DisallowNull] PlayerTeam obj)
        {
            return obj.Team > 0 ? obj.Team.GetHashCode() : obj.Player.GetHashCode();
        }

        public byte[] GetBytes()
        {
            return PacketHelper.ConcatBytes(
            BitConverter.GetBytes(Team),
            Player.ToByteArray(),
            BitConverter.GetBytes(Color.ToArgb()));
        }
    }
}
