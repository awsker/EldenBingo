using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EldenBingoCommon
{
    public class UserInRoom : INetSerializable, INotifyPropertyChanged
    {
        public string Nick { get; set; }
        public Guid Guid { get; set; }
        public bool IsAdmin { get; init; }
        public int Team { get; init; }
        public bool IsSpectator => Team == -1;

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Drawing.Color Color
        {
            get
            {
                if (IsSpectator)
                    return IsAdmin ? NetConstants.AdminSpectatorColor : NetConstants.SpectatorColor;
                return NetConstants.GetTeamColor(Team);
            }
        }

        public UserInRoom(string nick, Guid guid, bool isAdmin, int team)
        {
            Nick = nick;
            Guid = guid;
            IsAdmin = isAdmin;
            Team = team;
        }

        public UserInRoom(byte[] bytes, ref int offset)
        {
            Nick = PacketHelper.ReadString(bytes, ref offset);
            Guid = PacketHelper.ReadGuid(bytes, ref offset);
            IsAdmin = PacketHelper.ReadBoolean(bytes, ref offset);
            Team = PacketHelper.ReadInt(bytes, ref offset);
        }

        public byte[] GetBytes()
        {
            var nickBytes = PacketHelper.GetStringBytes(Nick);
            var guidBytes = Guid.ToByteArray();
            var adminByte = BitConverter.GetBytes(IsAdmin);
            var teamBytes = BitConverter.GetBytes(Team);
            return PacketHelper.ConcatBytes(nickBytes, guidBytes, adminByte, teamBytes);
        }

        public override string ToString()
        {
            var str = Nick;
            var suffix = new List<string>();
            
            if (IsAdmin)
                suffix.Add("Admin");
            if (IsSpectator)
                suffix.Add("Spectator");
            return str + (suffix.Any() ? $" [{string.Join(", ", suffix)}]" : string.Empty);
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
