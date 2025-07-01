using UnityEngine;

namespace BeatSabor.Utils.Algorithms.ConvexHull
{
    public struct Plane
    {
        public Vector3 normal;
        public float distance;
        
        /// <summary>
        /// 법선벡터와 이 평면이 법선벡터로부터 얼마나 떨어져 있는가.
        /// </summary>
        /// <param name="normal">법선벡터</param>
        /// <param name="point">평면위의 임의점</param>
        public Plane(Vector3 normal, Vector3 point)
        {
            this.normal = normal;
            distance = Vector3.Dot(normal, point);
        }

        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;
            this.distance = Vector3.Dot(normal, p1);
        }
    }
}
