using System.Text.Json;

namespace EldenBingoServer
{
    public class BingoBoardGenerator
    {
        readonly private IList<BingoJsonObj> _list;

        public BingoBoardGenerator(string json)
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new ArgumentException("Json is not in the correct format", nameof(json));
            }
            _list = new List<BingoJsonObj>();
            foreach (var row in doc.RootElement.EnumerateArray())
            {
                var text = row.GetProperty("name").GetString();
                if (text == null)
                    continue;

                string? tooltip = null;
                int weight = 1;
                if (row.TryGetProperty("tooltip", out var elem))
                    tooltip = elem.GetString();
                if (row.TryGetProperty("weight", out elem))
                    elem.TryGetInt32(out weight);
                _list.Add(new BingoJsonObj(text, tooltip, weight));
            }
            if(_list.Count < 25)
            {
                throw new ArgumentException("Json didn't contain at least 25 items", nameof(json));
            }
        }

        public ServerBingoBoard CreateBingoBoard(int seed)
        {
            var random = seed == 0 ? new Random() : new Random(seed);

            var tempList = new List<BingoJsonObj>(_list);
            var squares = new BingoJsonObj[25];
            for(int i = 0; i < 25; ++i)
            {
                var r = random.Next(tempList.Count);
                squares[i] = tempList[r];
                tempList.RemoveAt(r);
            }
            balanceBoard(tempList);

            return new ServerBingoBoard(squares.Select(o => o.Text).ToArray(), squares.Select(o => o.Tooltip).ToArray());
        }

        private void balanceBoard(IList<BingoJsonObj> squares)
        {
            //TODO
        }

        private struct BingoJsonObj
        {
            public string Text { get; init; }
            public string Tooltip { get; init; }
            public int Weight { get; init; }

            public BingoJsonObj(string text, string? tooltip = null, int weight = 1)
            {
                Text = text;
                Tooltip = tooltip == null ? string.Empty : tooltip;
                Weight = weight;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
