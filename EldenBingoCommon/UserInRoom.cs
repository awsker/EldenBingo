namespace EldenBingoCommon
{
    public class UserInRoom
    {
        public UserInRoom(string nick, Guid guid, bool isAdmin, int team)
        {
            Nick = nick;
            Guid = guid;
            IsAdmin = isAdmin;
            Team = team;
        }

        public UserInRoom(UserInRoom copy)
        {
            Nick = copy.Nick;
            Guid = copy.Guid;
            IsAdmin = copy.IsAdmin;
            Team = copy.Team;
        }

        //public event PropertyChangedEventHandler? PropertyChanged;

        public System.Drawing.Color Color
        {
            get
            {
                if (IsSpectator && IsAdmin)
                    return BingoConstants.AdminSpectatorColor;
                return BingoConstants.GetTeamColor(Team);
            }
        }

        public System.Drawing.Color ColorBright
        {
            get
            {
                if (IsSpectator && IsAdmin)
                    return BingoConstants.AdminSpectatorColor;
                return BingoConstants.GetTeamColorBright(Team);
            }
        }

        public Guid Guid { get; set; }
        public bool IsAdmin { get; set; }
        public string Nick { get; set; }
        public int Team { get; set; }
        public bool IsSpectator => Team == -1;

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
        /*
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }*/
    }
}