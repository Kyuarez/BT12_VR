using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BeatSabor.Utils.Algorithms.ConvexHull
{
    /// <summary>
    /// 두 점 A, B에 대한 선
    /// </summary>
    public struct Line
    {
        public readonly Vector3 DotA;
        public readonly Vector3 DotB;

        public Line(Vector3 dotA, Vector3 dotB)
        {
            this.DotA = dotA;
            this.DotB = dotB;
        }

        public float Distance()
        {
            return Vector3.Distance(DotA, DotB);
        }
    }
}
