namespace EldenBingo.UI
{
    internal class ClientUserControl : UserControl
    {
        private Client? _client;

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
        { }

        protected virtual void ClientChanged()
        { }

        protected virtual void RemoveClientListeners()
        { }
    }
}