using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeatSabor.Utils.Algorithms.ConvexHull
{
    public class MeshSlicer
    {
        const float ESP = 0.00001f;

        public static void Slice(Mesh mesh, Plane plane, out Mesh lower, out Mesh upper)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<Vector3> lowerVertices = new List<Vector3>();
            List<Vector3> upperVertices = new List<Vector3>();
            List<int> lowerTriangles = new List<int>();
            List<int> upperTriangles = new List<int>();

            List<Vector3> sectionPoints = new List<Vector3>();
            
            //삼각형 단위 순회
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i1 = triangles[i];
                int i2 = triangles[i + 1];
                int i3 = triangles[i + 2];

                Vector3 p1 = vertices[i1];
                Vector3 p2 = vertices[i2];
                Vector3 p3 = vertices[i3];

                float d1 = SignedDistance(p1, plane);
                float d2 = SignedDistance(p1, plane);
                float d3 = SignedDistance(p1, plane);

                bool up1 = d1 >= ESP;
                bool up2 = d2 >= ESP;
                bool up3 = d3 >= ESP;
                bool down1 = d1 <= -ESP;
                bool down2 = d2 <= -ESP;
                bool down3 = d3 <= -ESP;

                //@tk 여기에 걸치지 않는다 = 잘렸다.
                //위에 떠있다. 
                if (up1 && up2 && up3) 
                {
                    upperTriangles.Add(i1);
                    upperTriangles.Add(i2);
                    upperTriangles.Add(i3);
                    continue;
                }

                //아래에 있다. 
                if (down1 && down2 && down3)
                {
                    lowerTriangles.Add(i1);
                    lowerTriangles.Add(i2);
                    lowerTriangles.Add(i3);
                    continue;
                }
                
                //잘렸다.
                Clip(p1, p2, p3, true, d1, d2, d3, plane, upperVertices, upperTriangles, sectionPoints);
                Clip(p1, p2, p3, false, d1, d2, d3, plane, lowerVertices, lowerTriangles, sectionPoints);
            }

            //단면이 존재 할 경우...
            if (sectionPoints.Count >= 3)
            {
                List<Point2DMapped> point2DMapList = sectionPoints.Select(point => new Point2DMapped(point, plane)).ToList();
                List<Point2DMapped> hull = ConvexHullCalculator.MonotoneChain(point2DMapList);
                TriangleCalculateCap(hull, true, upperVertices, upperTriangles);
                TriangleCalculateCap(hull, false, lowerVertices, lowerTriangles);
            }

            upper = new Mesh();
            upper.SetVertices(upperVertices);
            upper.SetTriangles(upperTriangles, 0);
            upper.RecalculateNormals();

            lower = new Mesh();
            lower.SetVertices(lowerVertices);
            lower.SetTriangles(lowerTriangles, 0);
            lower.RecalculateNormals();
            
        }

        static void TriangleCalculateCap(List<Point2DMapped> hull, bool isUpper, List<Vector3> targetVertices, List<int> targetTriangles)
        {
            if(hull.Count < 3) 
                return;

            int i0 = 0;

            for (int i = 0; i < hull.Count - 1; i++) 
            {
                if (isUpper)
                {
                    targetVertices.Add(hull[i0].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                    targetVertices.Add(hull[i].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                    targetVertices.Add(hull[i + 1].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                }
                else
                {
                    targetVertices.Add(hull[i0].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                    targetVertices.Add(hull[i + 1].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                    targetVertices.Add(hull[i].point);
                    targetTriangles.Add(targetTriangles.Count - 1);
                }
            }
        }

        static float SignedDistance(Vector3 point, Plane plane)
        {
            return Vector3.Dot(plane.normal, point) + plane.distance;
        }

        /// <summary>
        /// 평면에 교차하는 삼각형을 자르는 메소드 
        /// TODO : Vertices 메모리 최적화(이미 사용하던 Vertex를 다른 삼각형에서 사용할 때, Vertices 배열 늘리지 말고, 사용 위치만 주기)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="isUpper"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="d3"></param>
        /// <param name="plane"></param>
        /// <param name="outTriangles"></param>
        static void Clip(
            Vector3 p1, Vector3 p2, Vector3 p3, 
            bool isUpper, 
            float d1, float d2, float d3, 
            Plane plane, 
            List<Vector3> outVertices, 
            List<int> outTriangles,
            List<Vector3> outSectionVertices)
        {
            //잘리고 남은 다각형
            //최대 점이 3개 이상이 되게
            List<Vector3> polygon = new List<Vector3>();
            List<Vector3> sectionVertices = new List<Vector3>();
            
            bool a = isUpper ? d1 >= ESP : d1 <= -ESP;
            bool b = isUpper ? d2 >= ESP : d2 <= -ESP;
            bool c = isUpper ? d3 >= ESP : d3 <= -ESP;

            //edge p1p2
            if (a) 
            {
                polygon.Add(p1);
            }
            //a^b : a점과 b점이 서로 반대됨. (upper, lower), 이럴 땐 교점 구해야함.
            if(a ^ b)
            {
                polygon.Add(Intersect(p1, p2, d1, d2, plane, sectionVertices));
            }

            //edge p2p3
            if (b)
            {
                polygon.Add(p2);
            }
            if (b ^ c)
            {
                polygon.Add(Intersect(p2, p3, d2, d3, plane, sectionVertices));
            }

            //edge p3p1
            if (c)
            {
                polygon.Add(p3);
            }
            if (c ^ a)
            {
                polygon.Add(Intersect(p3, p1, d3, d1, plane, sectionVertices));
            }

            //잘린부분이 면을 형성하지 못한 경우(점이 3개 미만)
            if (polygon.Count < 3)
                return;

            if(polygon.Count == 3)
            {
                if (isUpper)
                {
                    outVertices.Add(polygon[0]);
                    outTriangles.Add(outTriangles.Count - 1);
                    outVertices.Add(polygon[1]);
                    outTriangles.Add(outTriangles.Count - 1);
                    outVertices.Add(polygon[2]);
                    outTriangles.Add(outTriangles.Count - 1);

                    if(polygon.Count == 4)
                    {
                        outVertices.Add(polygon[0]);
                        outTriangles.Add(outTriangles.Count - 1);
                        outVertices.Add(polygon[2]);
                        outTriangles.Add(outTriangles.Count - 1);
                        outVertices.Add(polygon[3]);
                        outTriangles.Add(outTriangles.Count - 1);
                    }
                }
                else
                {
                    outVertices.Add(polygon[2]);
                    outTriangles.Add(outTriangles.Count - 1);
                    outVertices.Add(polygon[1]);
                    outTriangles.Add(outTriangles.Count - 1);
                    outVertices.Add(polygon[0]);
                    outTriangles.Add(outTriangles.Count - 1);

                    if (polygon.Count == 4)
                    {
                        outVertices.Add(polygon[3]);
                        outTriangles.Add(outTriangles.Count - 1);
                        outVertices.Add(polygon[2]);
                        outTriangles.Add(outTriangles.Count - 1);
                        outVertices.Add(polygon[0]);
                        outTriangles.Add(outTriangles.Count - 1);
                    }
                }
            }
            else
            {
                throw new System.Exception("삼각형 만들어야 하는데 다각형이 만들어짐");
            }

            outSectionVertices.AddRange(sectionVertices);
        }

        static Vector3 Intersect(Vector3 p1, Vector3 p2, float d1, float d2, Plane plane, List<Vector3> sectionVertices)
        {
            float t = d1 / (d1 - d2);
            Vector3 result = p1 + t * (p2 - p1);
            sectionVertices.Add(result);
            return result;
        }
    }
}
