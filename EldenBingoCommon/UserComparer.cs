namespace EldenBingoCommon
{
    public class UserComparer<T> : IComparer<T> where T : UserInRoom
    {
        private int val(T? x)
        {
            if (x == null)
                return 0;
            var xval = 0;
            if (x.IsSpectator)
                xval += x.IsAdmin ? 1 : 10000;
            else
                xval += x.Team * 10;
            if (x.IsAdmin)
                xval -= 5;
            return xval;
        }

        public int Compare(T? x, T? y)
        {
            if (x == null || y == null)
                return 0;
            var xval = val(x);
            var yval = val(y);
            
            if (xval == yval) //Exact same rank, sort by nickname
                return x.Nick.CompareTo(y.Nick);
            else return xval - yval;
        }
    }
}
