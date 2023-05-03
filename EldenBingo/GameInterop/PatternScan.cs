using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace EldenBingo.GameInterop
{
    /// <summary>
    /// Pattern scan implementation 'LazySIMD' - by uberhalit
    /// https://github.com/uberhalit
    ///
    /// Uses SIMD instructions on SSE2-supporting processors, the longer the pattern the more efficient this should get.
    /// Requires RyuJIT compiler for hardware acceleration which **should** be enabled by default on newer VS versions.
    /// Ideally a pattern would be a multiple of (xmm0 register size) / 8 so all available space gets used in calculations.
    /// Can be optimized further as currently the compiler adds a few unnecessary array boundry checks.
    /// </summary>
    internal class PatternScanLazySIMD
    {
        /// <summary>
        /// Length of an SSE2 vector in bytes.
        /// </summary>
        private const int SIMDLENGTH128 = 16;

        /// <summary>
        /// Returns address of pattern using 'LazySIMD' implementation by uberhalit. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            ref byte pCxMemory = ref MemoryMarshal.GetArrayDataReference(cbMemory);
            ref byte pCxPattern = ref MemoryMarshal.GetArrayDataReference(cbPattern);

            ReadOnlySpan<ushort> matchTable = BuildMatchIndexes(szMask, szMask.Length);
            int matchTableLength = matchTable.Length;
            Vector128<byte>[] patternVectors = PadPatternToVector128(cbPattern);
            ref Vector128<byte> pVec = ref patternVectors[0];
            int vectorLength = patternVectors.Length;

            Vector128<byte> firstByteVec = Vector128.Create(pCxPattern);
            ref Vector128<byte> pFirstVec = ref firstByteVec;

            int simdJump = SIMDLENGTH128 - 1;
            int searchLength = cbMemory.Length - (SIMDLENGTH128 > cbPattern.Length ? SIMDLENGTH128 : cbPattern.Length);
            for (int position = 0; position < searchLength; position++, pCxMemory = ref Unsafe.Add(ref pCxMemory, 1))
            {
                int findFirstByte = Sse2.MoveMask(Sse2.CompareEqual(pFirstVec, Unsafe.As<byte, Vector128<byte>>(ref pCxMemory)));
                if (findFirstByte == 0)
                {
                    position += simdJump;
                    pCxMemory = ref Unsafe.Add(ref pCxMemory, simdJump);
                    continue;
                }
                int offset = BitOperations.TrailingZeroCount((uint)findFirstByte);

                position += offset;
                pCxMemory = ref Unsafe.Add(ref pCxMemory, offset);

                int iMatchTableIndex = 0;
                bool found = true;
                for (int i = 0; i < vectorLength; i++)
                {
                    int compareResult = Sse2.MoveMask(Sse2.CompareEqual(Unsafe.Add(ref pVec, i), Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pCxMemory, 1 + i * SIMDLENGTH128))));

                    for (; iMatchTableIndex < matchTableLength; iMatchTableIndex++)
                    {
                        int matchIndex = matchTable[iMatchTableIndex];
                        if (i > 0) matchIndex -= i * SIMDLENGTH128;
                        if (matchIndex >= SIMDLENGTH128)
                            break;
                        if ((compareResult >> matchIndex & 1) == 1)
                            continue;
                        found = false;
                        break;
                    }

                    if (!found)
                        break;
                }
                if (found)
                    return position;
            }
            return -1;
        }

        /// <summary>
        /// Initializes a new 'PatternScanLazySIMD'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        internal static void Init(in byte[] cbMemory)
        {
            Vector128<byte> tmp = Vector128.Create((byte)0); // used to pre-load dependency if GC has over-optimized us out of existence already...
            if (!Sse2.IsSupported)
                throw new NotSupportedException("SIMD not HW accelerated...");
        }

        /// <summary>
        /// Builds a table that indicates which positions in pattern should be matched, the first match is skipped and all indexes are shifted to the left by 1.
        /// </summary>
        /// <param name="szMask">The mask.</param>
        /// <param name="maskLength">The length of the mask.</param>
        /// <returns></returns>
        private static ReadOnlySpan<ushort> BuildMatchIndexes(string szMask, int maskLength)
        {
            ushort[] fullMatchTable = new ushort[maskLength];

            int matchCount = 0;
            for (ushort i = 1; i < maskLength; ++i)
            {
                if (szMask[i] != 'x') continue;
                fullMatchTable[matchCount] = (ushort)(i - 1);
                matchCount++;
            }

            ReadOnlySpan<ushort> matchTable = new ReadOnlySpan<ushort>(fullMatchTable).Slice(0, matchCount);
            return matchTable;
        }

        /// <summary>
        /// Generates byte-Vectors that are right-padded with 0 from a pattern. The first byte is skipped.
        /// </summary>
        /// <param name="cbPattern">The pattern.</param>
        /// <returns></returns>
        private static Vector128<byte>[] PadPatternToVector128(in byte[] cbPattern)
        {
            int patternLen = cbPattern.Length;
            int vectorCount = (int)Math.Ceiling((patternLen - 1) / (float)SIMDLENGTH128);
            Vector128<byte>[] patternVectors = new Vector128<byte>[vectorCount];
            ref byte pPattern = ref cbPattern[1];
            patternLen--;
            for (int i = 0; i < vectorCount; i++)
            {
                if (i < vectorCount - 1)
                    patternVectors[i] = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pPattern, i * SIMDLENGTH128));
                else
                {
                    int o = i * SIMDLENGTH128;
                    patternVectors[i] = Vector128.Create(
                            Unsafe.Add(ref pPattern, o + 0),
                            o + 1 < patternLen ? Unsafe.Add(ref pPattern, o + 1) : (byte)0,
                            o + 2 < patternLen ? Unsafe.Add(ref pPattern, o + 2) : (byte)0,
                            o + 3 < patternLen ? Unsafe.Add(ref pPattern, o + 3) : (byte)0,
                            o + 4 < patternLen ? Unsafe.Add(ref pPattern, o + 4) : (byte)0,
                            o + 5 < patternLen ? Unsafe.Add(ref pPattern, o + 5) : (byte)0,
                            o + 6 < patternLen ? Unsafe.Add(ref pPattern, o + 6) : (byte)0,
                            o + 7 < patternLen ? Unsafe.Add(ref pPattern, o + 7) : (byte)0,
                            o + 8 < patternLen ? Unsafe.Add(ref pPattern, o + 8) : (byte)0,
                            o + 9 < patternLen ? Unsafe.Add(ref pPattern, o + 9) : (byte)0,
                            o + 10 < patternLen ? Unsafe.Add(ref pPattern, o + 10) : (byte)0,
                            o + 11 < patternLen ? Unsafe.Add(ref pPattern, o + 11) : (byte)0,
                            o + 12 < patternLen ? Unsafe.Add(ref pPattern, o + 12) : (byte)0,
                            o + 13 < patternLen ? Unsafe.Add(ref pPattern, o + 13) : (byte)0,
                            o + 14 < patternLen ? Unsafe.Add(ref pPattern, o + 14) : (byte)0,
                            o + 15 < patternLen ? Unsafe.Add(ref pPattern, o + 15) : (byte)0
                            );
                }
            }
            return patternVectors;
        }
    }
}