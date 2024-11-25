using EldenBingo.Net;
using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal partial class ClientListControl : ClientUserControl
    {
        public ClientListControl()
        {
            InitializeComponent();
            var factor = this.DefaultScaleFactors();
            _clientList.ItemHeight = Convert.ToInt32(20f * factor.Height);
        }

        public object SelectedItem => _clientList.SelectedItem;

        public override Color BackColor
        {
            get => _clientList.BackColor;
            set => _clientList.BackColor = value;
        }

        protected override void AddClientListeners()
        {
            if (Client != null)
            {
                Client.OnRoomChanged += client_RoomChanged;
                Client.AddListener<ServerJoinRoomAccepted>(joinRoomAccepted);
                Client.AddListener<ServerUserJoinedRoom>(userJoined);
                Client.AddListener<ServerUserLeftRoom>(userLeft);
                Client.AddListener<ServerUserChangedTeam>(userChangedTeam);
            }
        }

        protected override void ClientChanged()
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        protected override void RemoveClientListeners()
        {
            if (Client != null)
            {
                Client.OnRoomChanged -= client_RoomChanged;
                Client.RemoveListener<ServerJoinRoomAccepted>(joinRoomAccepted);
                Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
                Client.RemoveListener<ServerUserLeftRoom>(userLeft);
            }
        }

        private void joinRoomAccepted(ClientModel? _, ServerJoinRoomAccepted joinAccepted)
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        private void userJoined(ClientModel? _, ServerUserJoinedRoom userJoinedArgs)
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        private void userLeft(ClientModel? _, ServerUserLeftRoom userLeftArgs)
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        private void userChangedTeam(ClientModel? model, ServerUserChangedTeam teamChangedArgs)
        {
            if (Client?.Room != null)
            {
                var user = Client.Room.GetUser(teamChangedArgs.UserGuid);
                if (user != null)
                    user.Team = teamChangedArgs.Team;
                updateUsersList(Client.Room);
            }
        }


        private void clearUsersList()
        {
            void clear()
            {
                _clientList.Items.Clear();
            }
            if (InvokeRequired)
            {
                BeginInvoke(clear);
                return;
            }
            clear();
        }

        private void client_RoomChanged(object? sender, RoomChangedEventArgs e)
        {
            if (Client?.Room == null)
                clearUsersList();
        }

        private void updateUsersList(Room room)
        {
            void update()
            {
                Guid selectedGuid = Guid.Empty;
                if (_clientList.SelectedItem is UserInRoom selectedUser)
                {
                    selectedGuid = selectedUser.Guid;
                }
                _clientList.Items.Clear();
                foreach (var user in room.GetClientsSorted())
                {
                    _clientList.Items.Add(user);
                    if (user.Guid == selectedGuid)
                        _clientList.SelectedIndex = _clientList.Items.Count - 1;
                }
            }
            if (InvokeRequired)
            {
                BeginInvoke(update);
                return;
            }
            update();
        }
    }
}
