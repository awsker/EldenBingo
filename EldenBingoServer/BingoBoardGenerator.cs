using EldenBingoCommon;
using Newtonsoft.Json.Linq;

namespace EldenBingoServer
{
    public class BingoBoardGenerator
    {
        private readonly IList<BingoJsonObj> _list;
        private int _randomSeed;
        private Random _random;

        public BingoBoardGenerator(string json, int randomSeed)
        {
            var token = JToken.Parse(json);
            if (token is not JArray squareArray)
            {
                throw new ArgumentException("Json is not in the correct format", nameof(json));
            }
            RandomSeed = randomSeed;
            _list = new List<BingoJsonObj>();
            foreach (var square in squareArray)
            {
                string? name = square.Value<string>("name");
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                string? tooltip = square.Value<string>("tooltip");
                int? count = square.Value<int?>("count");
                int? weight = square.Value<int?>("weight");
                string? category = square.Value<string>("category");
                int? center = square.Value<int?>("center");

                var categories = new HashSet<string>();
                
                if(category != null)
                    categories.Add(category.Trim());

                var categoryArray = square.Value<JArray>("categories");
                if(categoryArray != null)
                {
                    foreach(var v in categoryArray.OfType<JValue>())
                    {
                        if(v.Value is string c)
                        {
                            categories.Add(c.Trim());
                        }
                    }
                }
                _list.Add(new BingoJsonObj(name, tooltip, count.GetValueOrDefault(0), weight.GetValueOrDefault(1), categories.ToArray(), (CenterType)center.GetValueOrDefault(0)));
            }
        }

        public int RandomSeed
        {
            get { return _randomSeed; }
            set
            {
                //Only create a new Random when the seed changes. As long as we're using the same random seed, it will
                //generate a sequence of boards based on that seed, but not the same board every time
                if (value == 0 || value != _randomSeed)
                {
                    _randomSeed = value;
                    _random = value == 0 ? new Random() : new Random(value);
                }
            }
        }

        public int CategoryLimit { get; set; }

        public ServerBingoBoard? CreateBingoBoard(ServerRoom room)
        {
            var squareList = new List<BingoJsonObj>(shuffleList(_list, _random));
            var squares = new List<BingoJsonObj>();
            var categoryCount = new Dictionary<string, int>();

            var numSquares = room.GameSettings.BoardSize * room.GameSettings.BoardSize;
            bool anySquareFailedCategoryLimit = false;

            //var anyCenterSpecifics = squareList.Any(s => s.CenterType > CenterType.None);
            BingoJsonObj? centerSquare = null;

            if(numSquares % 2 == 1)
            {
                for (int i = squareList.Count - 1; i >= 0; --i)
                {
                    //Remove all forced center squares (backwards) and pick the first one to be center square
                    if (squareList[i].CenterType == CenterType.ForcedCenter)
                    {
                        centerSquare = squareList[i];
                        squareList.RemoveAt(i);
                    }
                }
            }
            //If center square was found, increase all its categories used by 1
            if(centerSquare.HasValue)
            {
                //Decrease how many squares we're looking for
                --numSquares;
                foreach (var category in centerSquare.Value.Categories)
                {
                    categoryCount.TryGetValue(category, out int count);
                    categoryCount[category] = count + 1;
                }
            }

            while (squareList.Count > 0 && squares.Count < numSquares)
            {
                bool thisSquareFailedCategoryCheck = false;
                //Pick the first square in queue as the potential square (since we already removed all ForcedCenter squares)
                BingoJsonObj potentialSquare = squareList[0];
                squareList.RemoveAt(0);
                if (CategoryLimit > 0)
                {
                    foreach (var category in potentialSquare.Categories)
                    {
                        if (categoryCount.TryGetValue(category, out int count) && count + 1 > CategoryLimit)
                        {
                            anySquareFailedCategoryLimit = true;
                            thisSquareFailedCategoryCheck = true;
                            break;
                        }
                    }
                }
                if(thisSquareFailedCategoryCheck)
                {
                    //Try next square instead
                    continue;
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
            if (squares.Count != numSquares)
            {
                return null;
            }
            //If category limit was exceeded by any square:
            //Shuffle the final squares, so we don't get a bias of category-less squares late in the board
            if (anySquareFailedCategoryLimit)
                squares = shuffleList(squares, _random).ToList();

            if(centerSquare.HasValue)
            {
                //Add center square to the middle of the board, after reshuffle
                squares.Insert(numSquares / 2, centerSquare.Value);
            }
            //Balance the board, lock center square if it's set
            balanceBoard(squares, centerSquare.HasValue);

            EldenRingClasses[] classes;
            //Always randomize classes, even if they're not needed - to ensure consistency in random number generation
            if (room.GameSettings.RandomClasses && room.GameSettings.NumberOfClasses > 0)
            {
                classes = randomizeAvailableClasses(room.GameSettings.ValidClasses, room.GameSettings.NumberOfClasses);
            }
            else
            {
                classes = Array.Empty<EldenRingClasses>();
                _random.Next(); //Skip a number to ensure consistency in random number generation
            }
            return new ServerBingoBoard(room,
                room.GameSettings.BoardSize,
                squares.Select(s => 
                    new BingoBoardSquare(
                        s.Text, 
                        s.Tooltip, 
                        s.Count, 
                        null, 
                        false, 
                        Array.Empty<SquareCounter>()
                    )
                ).ToArray(), 
            classes);
        }

        private EldenRingClasses[] randomizeAvailableClasses(IEnumerable<EldenRingClasses> availableClasses, int numberOfClasses)
        {
            var classRandom = new Random(_random.Next());
            var classesQueue = new Queue<EldenRingClasses>(shuffleList(availableClasses, classRandom));
            var pickedClasses = new List<EldenRingClasses>();
            while (classesQueue.Count > 0 && pickedClasses.Count < numberOfClasses)
            {
                pickedClasses.Add(classesQueue.Dequeue());
            }
            return pickedClasses.ToArray();
        }

        private IEnumerable<T> shuffleList<T>(IEnumerable<T> squares, Random random)
        {
            return squares.OrderBy(s => random.Next()).ToList();
        }

        private void balanceBoard(IList<BingoJsonObj> squares, bool centerLocked)
        {
            //TODO
        }

        private struct BingoJsonObj
        {
            public BingoJsonObj(string text, string? tooltip = null, int count = 1, int weight = 1, string[]? categories = null, CenterType center = CenterType.None)
            {
                Text = text;
                Tooltip = tooltip == null ? string.Empty : tooltip;
                Count = Math.Max(0, count);
                Weight = weight;
                Categories = new HashSet<string>(categories ?? Array.Empty<string>());
                CenterType = center;
            }

            public string Text { get; init; }
            public string Tooltip { get; init; }
            public int Count { get; init; }
            public int Weight { get; init; }
            public ISet<string> Categories { get; init; }
            public CenterType CenterType { get; init; }

            public override string ToString()
            {
                return Text;
            }
        }

        private enum CenterType
        {
            None,
            ForcedCenter,
        }
    }
}