using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class GameSettingsControl : UserControl
    {
        public GameSettingsControl()
        {
            InitializeComponent();
            fillBoardSizeList();
            fillClassList();
            updateEnabling();
            _randomSeedUpDown.ValueChanged += (o, e) => SeedChanged?.Invoke();
        }

        public int CurrentSeed => Convert.ToInt32(_randomSeedUpDown.Value);

        private int BoardSize
        {
            get { return _boardSizeComboBox.SelectedIndex + 3; }
            set { _boardSizeComboBox.SelectedIndex = value - 3; }
        }

        public Action? SeedChanged;

        public BingoGameSettings Settings
        {
            get
            {
                var classSet = new HashSet<EldenRingClasses>();
                foreach (var checkedClass in _classesListBox.CheckedIndices)
                {
                    classSet.Add((EldenRingClasses)checkedClass);
                }
                return new BingoGameSettings(
                    BoardSize,
                    _lockoutCheckBox.Checked,
                    _classLimitCheckBox.Checked,
                    classSet,
                    Convert.ToInt32(_numClassesUpDown.Value),//Number of classes to pick
                    Convert.ToInt32(_maxCategoryUpDown.Value),//Max number of squares in the same category
                    Convert.ToInt32(_randomSeedUpDown.Value), //Random seed
                    Convert.ToInt32(_preparationTimeUpDown.Value), //Preparation time in seconds
                    Convert.ToInt32(_bonusPointsUpDown.Value) //Bonus points for a bingo line
                );
            }
            set
            {
                BoardSize = value.BoardSize;
                _lockoutCheckBox.Checked = value.Lockout;
                for (int i = 0; i < _classesListBox.Items.Count; i++)
                {
                    _classesListBox.SetItemChecked(i, false);
                }
                _classLimitCheckBox.Checked = value.RandomClasses;
                foreach (var checkedClass in value.ValidClasses)
                {
                    _classesListBox.SetItemChecked((int)checkedClass, true);
                }
                _numClassesUpDown.Value = value.NumberOfClasses;
                _maxCategoryUpDown.Value = value.CategoryLimit;
                _randomSeedUpDown.Value = value.RandomSeed;
                _preparationTimeUpDown.Value = value.PreparationTime;
                _bonusPointsUpDown.Value = value.PointsPerBingoLine;
                updateEnabling();
            }
        }

        private void fillBoardSizeList()
        {
            for(int i = BingoConstants.BoardSizeMin; i <= BingoConstants.BoardSizeMax; ++i)
            {
                _boardSizeComboBox.Items.Add($"{i}x{i}");
            };
        }

        private void fillClassList()
        {
            foreach (var cl in Enum.GetValues(typeof(EldenRingClasses)))
            {
                _classesListBox.Items.Add(cl);
            }
        }

        private void updateEnabling()
        {
            _numClassesUpDown.Enabled = _classLimitCheckBox.Checked;
            _classesListBox.Enabled = _classLimitCheckBox.Checked;
        }

        private void _classLimitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            updateEnabling();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _randomSeedUpDown.Value = 0;
        }
    }
}