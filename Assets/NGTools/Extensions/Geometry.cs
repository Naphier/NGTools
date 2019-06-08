using System.Collections.Generic;
using System.Linq;
namespace UnityEngine
{
    public class Geometry
    {
        public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180F)) + origin.x;
            float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180F)) + origin.y;

            x = Mathf.Clamp(x, float.MinValue, float.MaxValue);
            y = Mathf.Clamp(y, float.MinValue, float.MaxValue);
           
            return new Vector2(x, y);
        }

        public static float Circumference(float radius)
        {
            if (radius >= float.MaxValue)
                return float.MaxValue;

            if (radius <= float.MinValue)
                return float.MinValue;

            float circumference = (2f * Mathf.PI * radius);
            return circumference;
        }

        public static float Radius(float circumference)
        {
            if (circumference >= float.MaxValue)
                return float.MaxValue;

            if (circumference <= float.MinValue)
                return float.MinValue;


            return (circumference / (2f * Mathf.PI));
        }

        public static List<Vector3> GetFibonacciSphereVertices(int vertexCount = 1, bool randomize = true)
        {
            List<Vector3> vertices = new List<Vector3>();
            vertices.Capacity = vertexCount;

            float random = 1f;

            if (randomize)
                random = Random.Range(0f, 1f) * vertexCount;

            float offset = 2f / vertexCount;
            float increment = Mathf.PI * (3f - Mathf.Sqrt(5)); // what is 3 and 5?

            float x, y, z, r, phi;
            for (int i = 0; i < vertexCount; i++)
            {
                y = ((i * offset) - 1) + (offset / 2f);
                r = Mathf.Sqrt(1f - Mathf.Pow(y, 2));

                phi = ((i + random) % vertexCount) * increment;

                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;

                vertices.Add(new Vector3(x, y, z));
            }


            return vertices;
        }

        private static int GetMidpointIndex(Dictionary<string, int> midpointIndices, List<Vector3> vertices, int i0, int i1)
        {

            var edgeKey = string.Format("{0}_{1}", Mathf.Min(i0, i1), Mathf.Max(i0, i1));

            var midpointIndex = -1;

            if (!midpointIndices.TryGetValue(edgeKey, out midpointIndex))
            {
                var v0 = vertices[i0];
                var v1 = vertices[i1];

                var midpoint = (v0 + v1) / 2f;

                if (vertices.Contains(midpoint))
                    midpointIndex = vertices.IndexOf(midpoint);
                else
                {
                    midpointIndex = vertices.Count;
                    vertices.Add(midpoint);
                    midpointIndices.Add(edgeKey, midpointIndex);
                }
            }


            return midpointIndex;

        }

        /// <remarks>
        ///      i0
        ///     /  \
        ///    m02-m01
        ///   /  \ /  \
        /// i2---m12---i1
        /// </remarks>
        /// <param name="vectors"></param>
        /// <param name="indices"></param>
        public static void Subdivide(List<Vector3> vectors, List<int> indices, bool removeSourceTriangles)
        {
            var midpointIndices = new Dictionary<string, int>();

            var newIndices = new List<int>(indices.Count * 4);

            if (!removeSourceTriangles)
                newIndices.AddRange(indices);

            for (var i = 0; i < indices.Count - 2; i += 3)
            {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var m01 = GetMidpointIndex(midpointIndices, vectors, i0, i1);
                var m12 = GetMidpointIndex(midpointIndices, vectors, i1, i2);
                var m02 = GetMidpointIndex(midpointIndices, vectors, i2, i0);

                newIndices.AddRange(
                    new[] {
                    i0,m01,m02
                    ,
                    i1,m12,m01
                    ,
                    i2,m02,m12
                    ,
                    m02,m01,m12
                    }
                    );

            }

            indices.Clear();
            indices.AddRange(newIndices);
        }

        /// <summary>
        /// create a regular icosahedron (20-sided polyhedron)
        /// </summary>
        /// <remarks>
        /// You can create this programmatically instead of using the given vertex 
        /// and index list, but it's kind of a pain and rather pointless beyond a 
        /// learning exercise.
        /// </remarks>
        public static void Icosahedron(List<Vector3> vertices, List<int> indices)
        {

            indices.AddRange(
                new int[]
                {
                0,4,1,
                0,9,4,
                9,5,4,
                4,5,8,
                4,8,1,
                8,10,1,
                8,3,10,
                5,3,8,
                5,2,3,
                2,7,3,
                7,10,3,
                7,6,10,
                7,11,6,
                11,0,6,
                0,1,6,
                6,1,10,
                9,0,11,
                9,11,2,
                9,2,5,
                7,2,11
                }
                .Select(i => i + vertices.Count)
            );

            var X = 0.525731112119133606f;
            var Z = 0.850650808352039932f;

            vertices.AddRange(
                new[]
                {
                new Vector3(-X, 0f, Z),
                new Vector3(X, 0f, Z),
                new Vector3(-X, 0f, -Z),
                new Vector3(X, 0f, -Z),
                new Vector3(0f, Z, X),
                new Vector3(0f, Z, -X),
                new Vector3(0f, -Z, X),
                new Vector3(0f, -Z, -X),
                new Vector3(Z, X, 0f),
                new Vector3(-Z, X, 0f),
                new Vector3(Z, -X, 0f),
                new Vector3(-Z, -X, 0f)
                }
            );


        }

        /// <summary>
        /// Creates a smoothly subdivided icosahedron. 
        /// </summary>
        /// <remarks>
        /// Vertex count is 2 + 10 * 4 ^ subdivisions (i.e. 12, 42, 162, 642, 2562, 10242...)
        /// Subdivision is slow above 3. 
        /// Times:
        ///     0,1,2 = 0ms
        ///     3 = 43ms
        ///     4 = 605ms
        ///     5 = 9531ms
        ///     6 = ????
        /// </remarks>
        public static List<Vector3> GetSubdividedIcosahedronVertices(int subdivisions = 0)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            Icosahedron(vertices, indices);

            // Subdivide
            for (int i = 0; i < subdivisions; i++)
                Subdivide(vertices, indices, true);

            // Smooth vertices into a spherical shape
            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = vertices[i].normalized;

            return vertices;
        }



        public static Mesh Create9SlicedMesh(float border = 1f, float width = 3f, float height = 3f, float marginUV = 1f)
        {
            Mesh mesh = new Mesh();

            mesh.vertices = new Vector3[] {
                new Vector3(0, 0, 0),
                new Vector3(border, 0, 0),
                new Vector3(width - border, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, border, 0),
                new Vector3(border, border, 0),
                new Vector3(width - border, border, 0),
                new Vector3(width, border, 0),
                new Vector3(0, height - border, 0),
                new Vector3(border, height - border, 0),
                new Vector3(width - border, height - border, 0),
                new Vector3(width, height - border, 0),
                new Vector3(0, height, 0),
                new Vector3(border, height, 0),
                new Vector3(width - border, height, 0),
                new Vector3(width, height, 0)
            };

            mesh.uv = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(marginUV, 0),
                new Vector2(1 - marginUV, 0),
                new Vector2(1, 0),
                new Vector2(0, marginUV),
                new Vector2(marginUV, marginUV),
                new Vector2(1 - marginUV, marginUV),
                new Vector2(1, marginUV),
                new Vector2(0, 1 - marginUV),
                new Vector2(marginUV, 1 - marginUV),
                new Vector2(1 - marginUV, 1 - marginUV),
                new Vector2(1, 1 - marginUV),
                new Vector2(0, 1),
                new Vector2(marginUV, 1),
                new Vector2(1 - marginUV, 1),
                new Vector2(1, 1)
            };

            mesh.triangles = new int[] {
                0, 4, 5,
                0, 5, 1,
                1, 5, 6,
                1, 6, 2,
                2, 6, 7,
                2, 7, 3,
                4, 8, 9,
                4, 9, 5,
                5, 9, 10, // center plane
                5, 10, 6, // center plane
                6, 10, 11,
                6, 11, 7,
                8, 12, 13,
                8, 13, 9,
                9, 13, 14,
                9, 14, 10,
                10, 14, 15,
                10, 15, 11
            };

            return mesh;
        }
    }
}
