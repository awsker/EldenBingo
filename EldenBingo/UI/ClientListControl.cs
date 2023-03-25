using EldenBingo.Net;
using EldenBingo.Net.DataContainers;
using EldenBingoCommon;

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

        protected virtual void ClientChanged() 
        {
            if (Client?.Room != null)
                updateUsersList(Client.Room);
        }

        protected virtual void AddClientListeners()
        {
            Client.IncomingData += client_IncomingData;
            Client.RoomChanged += client_RoomChanged;
        }

        protected virtual void RemoveClientListeners()
        {
            Client.IncomingData -= client_IncomingData;
            Client.RoomChanged -= client_RoomChanged;
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if (e.Object is JoinedRoomData roomData)
            {
                updateUsersList(roomData.Room);
            }
            if (e.Object is UserJoinedLeftRoomData userJoinData)
            {
                updateUsersList(userJoinData.Room);
            }
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
                foreach (var user in room.Clients)
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
    }
}
