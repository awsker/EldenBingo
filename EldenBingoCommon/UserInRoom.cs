using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EldenBingoCommon
{
    public class UserInRoom : INetSerializable, INotifyPropertyChanged
    {
        public string Nick { get; set; }
        public Guid Guid { get; set; }
        public int Color { get; set; }
        public bool IsAdmin { get; init; }
        public int Team { get; init; }
        public bool IsSpectator { get; init; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Drawing.Color ConvertedColor => System.Drawing.Color.FromArgb(Color);

        public UserInRoom(string nick, Guid guid, int color, bool isAdmin, int team, bool spectator)
        {
            Nick = nick;
            Guid = guid;
            Color = color;
            IsAdmin = isAdmin;
            Team = team;
            IsSpectator = spectator;
        }

        public UserInRoom(byte[] bytes, ref int offset)
        {
            Nick = PacketHelper.ReadString(bytes, ref offset);
            Guid = PacketHelper.ReadGuid(bytes, ref offset);
            Color = PacketHelper.ReadInt(bytes, ref offset);
            IsAdmin = PacketHelper.ReadBoolean(bytes, ref offset);
            Team = PacketHelper.ReadInt(bytes, ref offset);
            IsSpectator = PacketHelper.ReadBoolean(bytes, ref offset);
        }

        public byte[] GetBytes()
        {
            var nickBytes = PacketHelper.GetStringBytes(Nick);
            var guidBytes = Guid.ToByteArray();
            var colorBytes = BitConverter.GetBytes(Color);
            var adminByte = BitConverter.GetBytes(IsAdmin);
            var teamBytes = BitConverter.GetBytes(Team);
            var spectatorByte = BitConverter.GetBytes(IsSpectator);
            return PacketHelper.ConcatBytes(nickBytes, guidBytes, colorBytes, adminByte, teamBytes, spectatorByte);
        }

        public override string ToString()
        {
            var str = Nick;
            var suffix = new List<string>();
            
            if (!IsSpectator && Team != 0)
                suffix.Add($"Team {Team}");
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
