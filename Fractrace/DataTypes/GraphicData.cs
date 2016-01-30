using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes
{


    /// <summary>
    /// Enthält Informationen zu einem Punkt in der 3D Szene
    /// </summary>
    public class VoxelInfo
    {
        public double i = 0;
        public double j = 0;
        public double k = 0;
        // True, if associated point is inside. 
        public bool inside = false;
    }


    public class VecInfo
    {
        public double x = 0;
        public double y = 0;
        public double z = 0;
    }


    /// <summary>
    /// Information of computed surface.
    /// </summary>
    public class GraphicData
    {

        protected int width = 1000;
        protected int height = 1000;

        public int[,] Picture = null;
        public VecInfo[,] Normals = null; 
        // Original coordinates
        public VoxelInfo[,] PointInfo = null;
        public double[,] ColorInfo = null;
        public double[,] ColorInfoDeph = null;

        public GraphicData(int width, int height)
        {
            this.width = width;
            this.height = height;
            Picture = new int[width + 1, height + 1];
            Normals = new VecInfo[width + 1, height + 1];
            PointInfo = new VoxelInfo[width + 1, height + 1];
            ColorInfo = new double[width + 1, height + 1];
            ColorInfoDeph = new double[width + 1, height + 1];

            for (int i = 0; i <= width; i++)
            {
                for (int j = 0; j <= height; j++)
                {
                    PointInfo[i, j] = null;
                    Normals[i, j] = null;
                    Picture[i, j] = 0;
                    ColorInfo[i, j] = 0;
                    ColorInfoDeph[i, j] = 0;
                }
            }
        }

    }
}
