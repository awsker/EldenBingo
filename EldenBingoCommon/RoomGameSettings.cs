using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EldenBingoCommon
{
    public record struct RoomGameSettings(int RandomSeed, ISet<EldenRingClasses> ValidClasses, int? NumberOfClasses, int? CategoryLimit);
}
