using EldenBingoCommon;

namespace EldenBingo.UI
{
    public partial class GameSettingsControl : UserControl
    {
        public GameSettingsControl()
        {
            InitializeComponent();
            fillClassList();
            updateEnabling();
        }

        private void fillClassList()
        {
            foreach(var cl in Enum.GetValues(typeof(EldenRingClasses)))
            {
                _classesListBox.Items.Add(cl);
            }
        }

        private void updateEnabling()
        {
            _numClassesUpDown.Enabled = _classLimitCheckBox.Checked;
            _classesListBox.Enabled = _classLimitCheckBox.Checked;
        }

        public BingoGameSettings Settings
        {
            get
            {
                var classSet = new HashSet<EldenRingClasses>();
                foreach(var checkedClass in _classesListBox.CheckedIndices)
                {
                    classSet.Add((EldenRingClasses)checkedClass);
                }
                return new BingoGameSettings(
                    _classLimitCheckBox.Checked,
                    classSet,
                    Convert.ToInt32(_numClassesUpDown.Value),//Number of classes to pick
                    Convert.ToInt32(_maxCategoryUpDown.Value),//Max number of squares in the same category
                    Convert.ToInt32(_randomSeedUpDown.Value) //Random seed
                ); 
            }
            set
            {
                for(int i = 0; i < _classesListBox.Items.Count; i++)
                {
                    _classesListBox.SetItemChecked(i, false);
                }
                _classLimitCheckBox.Checked = value.RandomClasses;
                foreach(var checkedClass in value.ValidClasses)
                {
                    _classesListBox.SetItemChecked((int)checkedClass, true);
                }
                _numClassesUpDown.Value = value.NumberOfClasses;
                _maxCategoryUpDown.Value = value.CategoryLimit;
                _randomSeedUpDown.Value = value.RandomSeed;
                updateEnabling();
            }
        }

        private void _classLimitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            updateEnabling();
        }
    }
}
