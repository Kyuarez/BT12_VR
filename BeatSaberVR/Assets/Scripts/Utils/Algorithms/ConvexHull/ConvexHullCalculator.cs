using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSabor.Utils.Algorithms.ConvexHull
{
    public static class ConvexHullCalculator
    {
        public static List<Point2DMapped> MonotoneChain(List<Point2DMapped> points)
        {
            points.Sort((p1, p2) =>
            {
                int compare = p1.mapped.x.CompareTo(p2.mapped.x);

                if (compare == 0)
                    compare = p1.mapped.y.CompareTo(p2.mapped.y);

                return compare;
            });

            List<Point2DMapped> lower = new List<Point2DMapped>();

            for(int i = 0; i < points.Count; i++)
            {
                Point2DMapped point = points[i];
                while(lower.Count >= 2 && 
                    CCW(lower[^2], lower[^1], point) <= 0) //^1 = Count - 1 (뒤에서부터)
                {
                    //반시계가 아닐 땐 마지막 부분 제거
                    lower.RemoveAt(lower.Count - 1);
                }

                lower.Add(point);
            }

            List<Point2DMapped> upper = new List<Point2DMapped>();

            for (int i = points.Count; i >= 0; i--)
            {
                Point2DMapped point = points[i];
                while (upper.Count >= 2 &&
                    CCW(upper[^2], upper[^1], point) <= 0) //^1 = Count - 1 (뒤에서부터)
                {
                    //반시계가 아닐 땐 마지막 부분 제거
                    upper.RemoveAt(lower.Count - 1);
                }

                upper.Add(point);
            }

            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);
            return lower;
        }

        /// <summary>
        /// 회전 방향 판정
        /// </summary>
        /// <returns>0 : 일직선, 1 : 반시계, -1 : 시계</returns>
        static int CCW(Point2DMapped p1, Point2DMapped p2, Point2DMapped p3)
        {
            long cross = (long)(p2.mapped.x - p1.mapped.x) * (long)(p3.mapped.y - p1.mapped.y) -
                (long)(p2.mapped.y - p1.mapped.y) * (long)(p3.mapped.x - p1.mapped.x);

            if (cross == 0)
                return 0;

            return cross > 0 ? 1 : -1;
        }
    }
}
