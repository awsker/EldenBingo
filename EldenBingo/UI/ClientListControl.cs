using EldenBingo.Net;
using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.UI
{
    internal class ClientListControl : RichListBox
    {
        private Client? _client;

        public ClientListControl() : base()
        {
        }

        public Client? Client
        {
            get { return _client; }
            set
            {
                if (_client != value)
                {
                    if (_client != null)
                        RemoveClientListeners();
                    _client = value;
                    ClientChanged();
                    if (_client != null)
                        AddClientListeners();
                }
            }
        }

        protected virtual void AddClientListeners()
        {
            Client.OnRoomChanged += client_RoomChanged;
            Client.AddListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            Client.AddListener<ServerUserJoinedRoom>(userJoined);
            Client.AddListener<ServerUserLeftRoom>(userLeft);
        }

        protected virtual void ClientChanged()
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        protected virtual void RemoveClientListeners()
        {
            Client.OnRoomChanged -= client_RoomChanged;
            Client.RemoveListener<ServerJoinRoomAccepted>(joinRoomAccepted);
            Client.RemoveListener<ServerUserJoinedRoom>(userJoined);
            Client.RemoveListener<ServerUserLeftRoom>(userLeft);
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

        private void clearUsersList()
        {
            void clear()
            {
                Items.Clear();
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
                if (SelectedItem is UserInRoom selectedUser)
                {
                    selectedGuid = selectedUser.Guid;
                }
                Items.Clear();
                foreach (var user in room.GetClientsSorted())
                {
                    Items.Add(user);
                    if (user.Guid == selectedGuid)
                        SelectedIndex = Items.Count - 1;
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