namespace EldenBingo.GameInterop
{
    /// <summary>
    /// Returns address of pattern using 'NaiveFor' implementation by uberhalit. Can match 0.
    /// </summary>
    /// <param name="cbMemory">The byte array to scan.</param>
    /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
    /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
    /// <returns>-1 if pattern is not found.</returns>
    internal class PatternScanNaiveFor
    {
        /// <summary>
        /// Length of an SSE2 vector in bytes.
        /// </summary>
        private static long dwStart = 0;

        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            long ix;
            int iy;
            bool bFound = false;
            int dataLength = cbMemory.Length - cbPattern.Length;

            for (ix = 0; ix < dataLength; ix++)
            {
                bFound = true;
                for (iy = cbPattern.Length - 1; iy > -1; iy--)
                {
                    if (szMask[iy] != 'x' || cbPattern[iy] == cbMemory[ix + iy])
                        continue;
                    bFound = false;
                    break;
                }

                if (bFound)
                    return dwStart + ix;
            }

            return -1;
        }
    }
}