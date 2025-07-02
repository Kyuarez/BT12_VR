using UnityEngine;

namespace BeatSabor.Utils.Algorithms.ConvexHull
{
    /// <summary>
    /// 월드좌표계 point와 특정 평면에 맵핑된 mapped 평면상대좌표계 데이터
    /// </summary>
    public struct Point2DMapped
    {
        public readonly Vector3 point;
        public readonly Vector2 mapped;

        public Point2DMapped(Vector3 point, Plane plane)
        {
            this.point = point;
            Vector3 n = plane.normal;
            float d = plane.distance;

            Vector3 p0 = -d * n; //평면 좌표계의 원점
            Vector3 u = Vector3.Cross(n, (Mathf.Abs(n.y) < 0.999) ? Vector3.up : Vector3.right).normalized; //평면 상대좌표
            Vector3 v = Vector3.Cross(n, u);

            Vector3 rel = point - p0;//좌표계 위치 변환
            this.mapped = new Vector2(Vector3.Dot(rel, u), Vector3.Dot(rel, v));
        }
    }
}
