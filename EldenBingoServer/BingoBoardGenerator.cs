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
            var tempList = new List<BingoJsonObj>(_list);
            var squares = new BingoJsonObj[25];
            int? limit = CategoryLimit > 0 ? CategoryLimit : null;
            var categoryCount = new Dictionary<string, int>();
            for (int i = 0; i < 25; ++i)
            {
                if(tempList.Count == 0)
                {
                    return null; //No squares left, not possible to create bingo board
                }
                var r = _random.Next(tempList.Count);
                var potentialSquare = tempList[r];
                if (limit.HasValue)
                {
                    foreach (var category in potentialSquare.Categories)
                    {
                        if (categoryCount.TryGetValue(category, out int count) && count + 1 > limit.Value)
                        {
                            --i;
                            tempList.RemoveAt(r); //Remove square and try a new square
                            continue;
                        } 
                    }
                }
                //Increment count of each category by 1
                foreach (var category in potentialSquare.Categories)
                {
                    categoryCount.TryGetValue(category, out int count);
                    categoryCount[category] = count + 1;
                }
                tempList.RemoveAt(r);
                squares[i] = potentialSquare;
            }
            balanceBoard(tempList);

            return new ServerBingoBoard(room, squares.Select(o => o.Text).ToArray(), squares.Select(o => o.Tooltip).ToArray());
        }

        public EldenRingClasses[] RandomizeAvailableClasses(IEnumerable<EldenRingClasses> availableClasses, int numberOfClasses)
        {
            var availableClassesList = new List<EldenRingClasses>(availableClasses);
            var pickedClasses = new List<EldenRingClasses>();
            for(int i = Math.Min(numberOfClasses, availableClassesList.Count); i > 0; --i)
            {
                var r = _classRandom.Next(availableClassesList.Count);
                var cl = availableClassesList[r];
                pickedClasses.Add(cl);
                availableClassesList.RemoveAt(r);
            }
            return pickedClasses.ToArray();
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