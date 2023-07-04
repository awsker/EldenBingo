using EldenBingoCommon;
using System.Text.Json;

namespace EldenBingoServer
{
    public class BingoBoardGenerator
    {
        private readonly IList<BingoJsonObj> _list;
        private int _randomSeed;
        private Random _random;
        private Random _classRandom;

        public int RandomSeed
        {
            get { return _randomSeed; }
            set
            {
                //Only create a new Random when the seed changes. As long as we're using the same random seed, it will
                //generate a sequence of boards based on that seed, but not the same board every time
                if(value == 0 || value != _randomSeed)
                {
                    _randomSeed = value;
                    _random = value == 0 ? new Random() : new Random(value);
                    _classRandom = value == 0 ? new Random() : new Random(_random.Next());
                }
            }
        }

        public int CategoryLimit { get; set; }

        public BingoBoardGenerator(string json, int randomSeed)
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new ArgumentException("Json is not in the correct format", nameof(json));
            }
            RandomSeed = randomSeed;
            _list = new List<BingoJsonObj>();
            foreach (var row in doc.RootElement.EnumerateArray())
            {
                var text = row.GetProperty("name").GetString();
                if (text == null)
                    continue;

                string? tooltip = null;
                int weight = 1;
                var categories = new HashSet<string>();
                if (row.TryGetProperty("tooltip", out var elem))
                    tooltip = elem.GetString();
                if (row.TryGetProperty("weight", out elem))
                    elem.TryGetInt32(out weight);
                if (row.TryGetProperty("category", out elem))
                {
                    var categoryName = elem.GetString();
                    if(categoryName != null)
                        categories.Add(categoryName);
                }
                if (row.TryGetProperty("categories", out elem))
                {
                    if(elem.GetArrayLength() > 0)
                    {
                        foreach (var e in elem.EnumerateArray())
                        {
                            string categoryNameInner = e.GetString();
                            if (categoryNameInner != null)
                                categories.Add(categoryNameInner);
                        }
                    }
                }
                _list.Add(new BingoJsonObj(text, tooltip, weight, categories.ToArray()));
            }
            if (_list.Count < 25)
            {
                throw new ArgumentException("Json didn't contain at least 25 items", nameof(json));
            }
        }

        public ServerBingoBoard? CreateBingoBoard(ServerRoom room)
        {
            var tempList = shuffleList(_list);
            var squares = new List<BingoJsonObj>();
            var categoryCount = new Dictionary<string, int>();

            bool exceededCategoryLimit = false;
            while(tempList.Count > 0 && squares.Count < 25)
            {
                var potentialSquare = tempList[0];
                tempList.RemoveAt(0);
                if (CategoryLimit > 0)
                {
                    foreach (var category in potentialSquare.Categories)
                    {
                        if (categoryCount.TryGetValue(category, out int count) && count + 1 > CategoryLimit)
                        {
                            exceededCategoryLimit = true;
                            continue; //Category limit would be exceeded with this square, so we skip it
                        } 
                    }
                }
                //YES, include the square on the board
                squares.Add(potentialSquare);
                //Increment count of each category by 1
                foreach (var category in potentialSquare.Categories)
                {
                    categoryCount.TryGetValue(category, out int count);
                    categoryCount[category] = count + 1;
                }
            }
            if(squares.Count != 25)
            {
                return null;
            }
            //If category limit was exceeded by any square:
            //Shuffle the final squares, so we don't get a bias of category-less squares late in the board
            if(exceededCategoryLimit)
                squares = shuffleList(squares);
            balanceBoard(squares);

            return new ServerBingoBoard(room, squares.Select(o => o.Text).ToArray(), squares.Select(o => o.Tooltip).ToArray());
        }

        public EldenRingClasses[] RandomizeAvailableClasses(IEnumerable<EldenRingClasses> availableClasses, int numberOfClasses)
        {
            var availableClassesList = shuffleList(availableClasses);
            var pickedClasses = new List<EldenRingClasses>();
            while(availableClassesList.Count > 0 && pickedClasses.Count < numberOfClasses)
            {
                pickedClasses.Add(availableClassesList[0]);
                availableClassesList.RemoveAt(0);
            }
            return pickedClasses.ToArray();
        }

        private List<T> shuffleList<T>(IEnumerable<T> squares)
        {
            return squares.OrderBy(s => _random.Next()).ToList();
        }

        private void balanceBoard(IList<BingoJsonObj> squares)
        {
            //TODO
        }

        private struct BingoJsonObj
        {
            public BingoJsonObj(string text, string? tooltip = null, int weight = 1, string[]? categories = null)
            {
                Text = text;
                Tooltip = tooltip == null ? string.Empty : tooltip;
                Weight = weight;
                Categories = new HashSet<string>(categories ?? Array.Empty<string>());
            }

            public string Text { get; init; }
            public string Tooltip { get; init; }
            public int Weight { get; init; }
            public ISet<string> Categories { get; init; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}