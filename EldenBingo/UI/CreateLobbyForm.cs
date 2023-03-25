using EldenBingo.Net.DataContainers;
using EldenBingoCommon;
using System.Drawing;

namespace EldenBingo.UI
{
    public partial class CreateLobbyForm : Form
    {
        private readonly Client _client;

        private Color _color;

        public CreateLobbyForm(Client client, bool create)
        {
            InitializeComponent();
            _client = client;
            if (!create)
                Text = "Join Lobby";

            if(create && client != null)
            {
                client.IncomingData += client_IncomingData;
            }
            initTeamComboBox();
            loadSettings();
            
        }

        private void client_IncomingData(object? sender, ObjectEventArgs e)
        {
            if(e.Object is AvailableRoomNameData d && RoomName == string.Empty)
            {
                RoomName = d.Name;
            }
        }

        private void initTeamComboBox()
        {
            foreach(Team item in Enum.GetValues(typeof(Team)))
            {
                if (item == Team.None || item == Team.Spectator)
                    _teamComboBox.Items.Add(item.ToString());
                else
                {
                    int i = ((int)item) - 1;
                    _teamComboBox.Items.Add($"{NetConstants.DefaultPlayerColors[i].Name} Team");
                }
            }
            _teamComboBox.SelectedIndex = 0;
            _teamComboBox.SelectedIndexChanged += (o, e) => setPanelColor();
        }

        public string RoomName
        {
            get { return _roomNameTextBox.Text; }
            set { _roomNameTextBox.Text = value; }
        }

        public string AdminPassword
        {
            get { return _adminPasswordTextBox.Text; }
            set { _adminPasswordTextBox.Text = value; }
        }

        public string Nickname
        {
            get { return _nicknameTextBox.Text; }
            set { _nicknameTextBox.Text = value; }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                setPanelColor();
            }
        }

        private void setPanelColor()
        {
            if (Team == Team.None)
                _colorPanel.BackColor = _color;
            else if (Team == Team.Spectator)
                _colorPanel.BackColor = NetConstants.SpectatorColor;
            else
                _colorPanel.BackColor = NetConstants.DefaultPlayerColors[(int)Team - 1].Color;
        }

        public Team Team {
            get { return (Team)_teamComboBox.SelectedIndex; }
            set { _teamComboBox.SelectedIndex = (int)value; }
        }

        private void loadSettings()
        {
            Nickname = Properties.Settings.Default.Nickname;
            var color = Properties.Settings.Default.Color;
            if (color.A < 255)
                Color = NetConstants.DefaultPlayerColors[0].Color;
            else
                Color = color;
            if (Properties.Settings.Default.Team < _teamComboBox.Items.Count)
                _teamComboBox.SelectedIndex = Properties.Settings.Default.Team;
        }

        private void saveSettings()
        {
            Properties.Settings.Default.Nickname = Nickname;
            Properties.Settings.Default.Color = Color;
            Properties.Settings.Default.Team = _teamComboBox.SelectedIndex;
            Properties.Settings.Default.Save();
        }
        
        private bool validate()
        {
            if (string.IsNullOrWhiteSpace(_roomNameTextBox.Text))
            {
                errorProvider1.SetError(_roomNameTextBox, "Room name not set");
                return false;
            } 
            else
            {
                errorProvider1.SetError(_roomNameTextBox, null);
            }

            if (string.IsNullOrWhiteSpace(_nicknameTextBox.Text))
            {
                errorProvider1.SetError(_nicknameTextBox, "Nickname not set");
                return false;
            }
            else
            {
                errorProvider1.SetError(_nicknameTextBox, null);
            }
            return true;
        }

        private void CreateLobbyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client.IncomingData -= client_IncomingData;
        }

        private void _createButton_Click(object sender, EventArgs e)
        {
            if(validate())
            {
                DialogResult = DialogResult.OK;
                saveSettings();
                Close();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private ColorGridForm? _colorForm;

        private void _colorPanel_Click(object sender, EventArgs e)
        {
            if (_colorForm != null || Team != Team.None)
                return;
            _colorForm = new ColorGridForm();
            var p = new Point(Left + _colorPanel.Left, Top + _colorPanel.Top);
            _colorForm.Location = p;
            _colorForm.ColorClicked += (o, e) =>
            {
                if (_colorForm?.SelectedColor != null)
                    Color = _colorForm.SelectedColor.Value;
                _colorForm?.Close();
                _colorForm = null;
            };
            _colorForm.Show(this);
        }
    }
}
