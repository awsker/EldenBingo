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

        public UserInRoom? SelectedUser => _clientList?.SelectedItem as UserInRoom;

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
                Client.AddListener<ServerPromoteToAdmin>(userPromoted);
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

        private void userPromoted(ClientModel? _, ServerPromoteToAdmin userPromotedArgs)
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

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Client?.Room == null || Client.Room.GetUser(Client.ClientGuid)?.IsAdmin != true || SelectedUser == null)
            {
                e.Cancel = true;
                return;
            }
        }

        private async void _banPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Client?.Room == null || Client.Room.GetUser(Client.ClientGuid)?.IsAdmin != true)
            {
                return;
            }
            var selectedUser = SelectedUser;
            if (Client != null && selectedUser != null)
            {
                await Client.SendPacketToServer(new Packet(new ClientBanUserFromRoom(selectedUser.Guid)));
            }
        }

        private async void _promoteToAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Client?.Room == null || Client.Room.GetUser(Client.ClientGuid)?.IsAdmin != true)
            {
                return;
            }
            var selectedUser = SelectedUser;
            if (Client != null && selectedUser != null)
            {
                await Client.SendPacketToServer(new Packet(new ClientPromoteToAdmin(selectedUser.Guid)));
            }
        }
    }
}
