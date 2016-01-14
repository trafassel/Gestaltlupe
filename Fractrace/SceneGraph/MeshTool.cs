using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class MeshTool
    {

        /// <summary>
        /// 2D integer coordinate.
        /// </summary>
        struct Coord2D { public Coord2D(int x, int y) { this.X = x; this.Y = y; } public int X; public int Y; }

        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public MeshTool(Iterate iter, PictureData pictureData)
        {
            _iterate = iter;
            _pictureData = pictureData;
        }

        /// <summary>
        /// PictureData of iter with surface coloring from PictureArt
        /// </summary>
        PictureData _pictureData = null;

        /// <summary>
        /// Used iterate
        /// </summary>
        protected Iterate _iterate = null;


        /// <summary>
        /// Use transformations of _iterate.LastUsedFormulas to transfrom given point.
        /// </summary>
        public PixelInfo Transform(PixelInfo input)
        {
            if (_iterate != null)
            {
                if (_iterate.LastUsedFormulas != null)
                {
                    Geometry.Vec3 vec = _iterate.LastUsedFormulas.GetTransform(input.Coord.X, input.Coord.Y, input.Coord.Z);
                    PixelInfo tempPoint = new PixelInfo();
                    tempPoint.Coord = vec;
                    tempPoint.AdditionalInfo = input.AdditionalInfo;
                    // Transform normal??
                    tempPoint.Normal = input.Normal;
                    return tempPoint;
                }
            }
            return input;
        }

        public Mesh CreateMesh()
        {
            Mesh _mesh = new Mesh();

            List<Coord2D> pointList = new List<Coord2D>();
            Dictionary<KeyValuePair<int, int>, int> indexList = new Dictionary<KeyValuePair<int, int>, int>();

            int[,] pointIndex = new int[_pictureData.Width + 1, _pictureData.Height + 1];

            double minx = Double.MaxValue;
            double miny = Double.MaxValue;
            double minz = Double.MaxValue;
            double maxx = Double.MinValue;
            double maxy = Double.MinValue;
            double maxz = Double.MinValue;

            int currentIndex = 0;
            for (int i = 0; i < _pictureData.Width; i++)
            {
                for (int j = 0; j < _pictureData.Height; j++)
                {
                    if (_pictureData.Points[i, j] != null)
                    {
                        PixelInfo point = Transform(_pictureData.Points[i, j]);

                        if (minx > point.Coord.X)
                            minx = point.Coord.X;
                        if (miny > point.Coord.Y)
                            miny = point.Coord.Y;
                        if (minz > point.Coord.Z)
                            minz = point.Coord.Z;
                        if (maxx < point.Coord.X)
                            maxx = point.Coord.X;
                        if (maxy < point.Coord.Y)
                            maxy = point.Coord.Y;
                        if (maxz < point.Coord.Z)
                            maxz = point.Coord.Z;

                        Coord2D coord = new Coord2D(i, j);
                        pointIndex[i, j] = currentIndex;
                        pointList.Add(coord);
                        currentIndex++;
                    }
                    else
                    {
                        pointIndex[i, j] = -1;
                    }
                }
            }

            double radius = maxz - minz + maxy - miny + maxx - minx;
            double centerx = (maxx + minx) / 2.0;
            double centery = (maxy + miny) / 2.0;
            double centerz = (maxz + minz) / 2.0;

            foreach (Coord2D coord in pointList)
            {
                PixelInfo pInfo = Transform(_pictureData.Points[coord.X, coord.Y]);
                if (pInfo != null && pInfo.Coord != null && pInfo.AdditionalInfo != null)
                {

                    double x, y, z;
                    x = (pInfo.Coord.X - centerx) / radius;
                    y = (pInfo.Coord.Y - centery) / radius;
                    z = (pInfo.Coord.Z - centerz) / radius;

                    _mesh._coordinates.Add((float)x);
                    _mesh._coordinates.Add((float)y);
                    _mesh._coordinates.Add((float)z);

                    double red = pInfo.AdditionalInfo.red2;
                    double green = pInfo.AdditionalInfo.green2;
                    double blue = pInfo.AdditionalInfo.blue2;

                    _mesh._colors.Add((float)red);
                    _mesh._colors.Add((float)green);
                    _mesh._colors.Add((float)blue);

                    
                }
            }


            for (int i = 0; i < _pictureData.Width; i++)
            {
                for (int j = 0; j < _pictureData.Height; j++)
                {
                    if (_pictureData.Points[i, j] != null)
                    {

                        PixelInfo point1 = _pictureData.Points[i, j];
                        if (point1.Coord.X > 1000 && point1.Coord.X < -1000 && point1.Coord.Y > 1000 && point1.Coord.Y < -1000 && point1.Coord.Z > 1000 && point1.Coord.Z < -1000)
                        {
                            System.Diagnostics.Debug.WriteLine("Error");
                        }
                        if (i > 0 && j > 0 && _pictureData.Points[i - 1, j - 1] != null)
                        {

//                            bool useDistance = !Fractrace.Basic.ParameterDict.Current.GetBool("Export.X3d.ClosedSurface");
                            if (_pictureData.Points[i - 1, j] != null)
                            {
                                // triangle 1
                                _mesh._faces.Add(pointIndex[i, j]);
                                _mesh._faces.Add(pointIndex[i - 1, j]);
                                _mesh._faces.Add(pointIndex[i - 1, j - 1]);

                                // TODO: Add Normal
                            }
                            if (_pictureData.Points[i, j - 1] != null)
                            {
                                // triangle 2
                                _mesh._faces.Add(pointIndex[i, j]);
                                _mesh._faces.Add(pointIndex[i - 1, j - 1]);
                                _mesh._faces.Add(pointIndex[i, j - 1]);

                                // TODO: Add normal
                            }
                        }
                    }
                }
            }




            return _mesh;
        }
    }
}
