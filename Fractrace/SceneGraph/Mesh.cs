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

    }
}
