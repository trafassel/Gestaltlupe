using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class Mesh
    {

        public List<float> _coordinates = new List<float>();

        public List<float> _colors = new List<float>();

        public List<int> _faces = new List<int>();

        // Normal per face
        public List<float> _normales = new List<float>();


        float GetX(int i)
        {
            return _coordinates[3 * i];
        }

        float GetY(int i)
        {
            return _coordinates[3 * i + 1];
        }

        float GetZ(int i)
        {
            return _coordinates[3 * i + 2];
        }

        float GetRed(int i)
        {
            return _colors[3 * i];
        }

        float GetGreen(int i)
        {
            return _colors[3 * i + 1];
        }

        float GetBlue(int i)
        {
            return _colors[3 * i + 2];
        }

        /// <summary>
        /// Return point index of first point in triangle.
        /// </summary>
        int GetPoint1(int i)
        {
            return _faces[3 * i];
        }

        /// <summary>
        /// Return point index of second point in triangle.
        /// </summary>
        int GetPoint2(int i)
        {
            return _faces[3 * i + 1];
        }

        /// <summary>
        /// Return point index of third point in triangle.
        /// </summary>
        int GetPoint3(int i)
        {
            return _faces[3 * i + 2];
        }

        /// <summary>
        /// Coordinate of minimal point in the mesh boundingbox (if computed with ComputeBoundingBox)
        /// </summary>
        private FPoint _minBBox = null;
        public FPoint MinBBox { get { return _minBBox; } }

        /// <summary>
        /// Coordinate of maximal point in the mesh boundingbox (if computed with ComputeBoundingBox)
        /// </summary>
        private FPoint _maxBBox = null;
        public FPoint MaxBBox { get { return _maxBBox; } }


        public void ComputeBoundingBox()
        {
            float minx = float.MaxValue;
            float miny = float.MaxValue;
            float minz = float.MaxValue;
            float maxx = float.MinValue;
            float maxy = float.MinValue;
            float maxz = float.MinValue;

            int noCoords = _coordinates.Count / 3;
            for (int i = 0; i < noCoords; i++)
            {
                int coordIndex = 3 * i;
                float x = _coordinates[coordIndex];
                float y = _coordinates[coordIndex + 1];
                float z = _coordinates[coordIndex + 2];

                if (minx > x)
                    minx = x;
                if (miny > y)
                    miny = y;
                if (minz > z)
                    minz = z;
                if (maxx < x)
                    maxx = x;
                if (maxy < y)
                    maxy = y;
                if (maxz < z)
                    maxz = z;
            }
            _minBBox = new FPoint(minx,miny,minz);
            _maxBBox = new FPoint(maxx, maxy, maxz);
        }


    }
}
