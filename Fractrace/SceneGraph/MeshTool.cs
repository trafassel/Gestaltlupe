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
        /// If true needScaling is always set to true
        /// </summary>
        public bool AlwaysScale = false;

        /// <summary>
        /// PictureData of iter with surface coloring from PictureArt
        /// </summary>
        PictureData _pictureData = null;

        /// <summary>
        /// Used iterate
        /// </summary>
        protected Iterate _iterate = null;

        /// <summary>
        /// Valid == false if colors are not defined.
        /// </summary>
        public bool Valid { get { return _valid; } }
        protected bool _valid = true;

        // will be set on init
        double _radius = 0;
        double _centerx = 0;
        double _centery = 0;
        double _centerz = 0;

        Mesh _mesh = null;

        bool _useDistance = false;
        double _maxDist = 0;
        bool _needScaling = false;

        /// <summary>
        /// Initialise Mesh an set radius, centerx, centery and centerz.
        /// </summary>
        public void InitMesh()
        {
            _mesh = new Mesh();
        }


        public Mesh Update(Iterate iter, PictureData pictureData)
        {
            _iterate = iter;
            _pictureData = pictureData;
            // TODO: Update _mesh data 

            List<Coord2D> pointList = new List<Coord2D>();

            int[,] pointIndex = new int[_pictureData.Width + 1, _pictureData.Height + 1];

            int currentIndex = _mesh._coordinates.Count/3; // lenght of pointindex in _mesh??
            for (int i = 0; i < _pictureData.Width; i++)
            {
                for (int j = 0; j < _pictureData.Height; j++)
                {
                    if (_pictureData.Points[i, j] != null)
                    {
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

            // to test for invalid pdata
            double maxcol = 0;

            foreach (Coord2D coord in pointList)
            {
                PixelInfo pInfo = Transform(_pictureData.Points[coord.X, coord.Y]);
                if (pInfo != null && pInfo.Coord != null && pInfo.AdditionalInfo != null)
                {

                    double x, y, z;

                    if (_needScaling || AlwaysScale)
                    {
                        x = (pInfo.Coord.X - _centerx) / _radius;
                        y = (pInfo.Coord.Y - _centery) / _radius;
                        z = (pInfo.Coord.Z - _centerz) / _radius;
                    }
                    else
                    {
                        // Scale by 1000
                        x = 1000.0 * pInfo.Coord.X;
                        y = 1000.0 * pInfo.Coord.Y;
                        z = 1000.0 * pInfo.Coord.Z;
                    }

                    _mesh._coordinates.Add((float)x);
                    _mesh._coordinates.Add((float)y);
                    _mesh._coordinates.Add((float)z);

                    double red = pInfo.AdditionalInfo.red2;
                    double green = pInfo.AdditionalInfo.green2;
                    double blue = pInfo.AdditionalInfo.blue2;

                    _mesh._colors.Add((float)red);
                    _mesh._colors.Add((float)green);
                    _mesh._colors.Add((float)blue);

                    // test for invalid data only
                    if (maxcol < red)
                        maxcol = red;
                    if (maxcol < green)
                        maxcol = green;
                    if (maxcol < blue)
                        maxcol = blue;

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

                            if (_pictureData.Points[i - 1, j] != null)
                            {
                                // triangle 1
                                bool useTriangle = true;
                                if (_useDistance)
                                {
                                    PixelInfo point2 = _pictureData.Points[i - 1, j];
                                    PixelInfo point3 = _pictureData.Points[i - 1, j - 1];
                                    if (Dist(point1, point2) > _maxDist || Dist(point1, point3) > _maxDist)
                                        useTriangle = false;
                                }
                                if (useTriangle)
                                {
                                    _mesh._faces.Add(pointIndex[i, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j - 1]);

                                    _mesh._normales.Add((float)point1.Normal.X);
                                    _mesh._normales.Add((float)point1.Normal.Y);
                                    _mesh._normales.Add((float)point1.Normal.Z);

                                }
                            }
                            if (_pictureData.Points[i, j - 1] != null)
                            {
                                // triangle 2
                                bool useTriangle = true;
                                if (_useDistance)
                                {
                                    PixelInfo point2 = _pictureData.Points[i - 1, j - 1];
                                    PixelInfo point3 = _pictureData.Points[i, j - 1];
                                    if (Dist(point1, point2) > _maxDist || Dist(point1, point3) > _maxDist)
                                        useTriangle = false;
                                }
                                if (useTriangle)
                                {
                                    _mesh._faces.Add(pointIndex[i, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j - 1]);
                                    _mesh._faces.Add(pointIndex[i, j - 1]);

                                    _mesh._normales.Add((float)point1.Normal.X);
                                    _mesh._normales.Add((float)point1.Normal.Y);
                                    _mesh._normales.Add((float)point1.Normal.Z);

                                }
                            }
                        }
                    }
                }
            }
            if (maxcol < 0.0001)
            {
                _valid = false;
            }
            return _mesh;
        }


        public void Init(Iterate iter, PictureData pictureData)
        {
            _iterate = iter;
            _pictureData = pictureData;
            _useDistance = !Fractrace.Basic.ParameterDict.Current.GetBool("Export.X3d.ClosedSurface");
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
                        currentIndex++;
                    }
                }
            }
            if (currentIndex == 0)
            {
                _valid = false; 
                return;
            }
            _radius = maxz - minz + maxy - miny + maxx - minx;
            _centerx = (maxx + minx) / 2.0;
            _centery = (maxy + miny) / 2.0;
            _centerz = (maxz + minz) / 2.0;
            _needScaling = _radius < 0.01;
            // Rounding scale parameters to allow combine different 3d scenes at later time. 
            int noOfDigits = 1;
            double d = 1;
            if (_needScaling || AlwaysScale)
            {
                while (d > _radius)
                {
                    d /= 10.0;
                    noOfDigits++;
                }
                noOfDigits -= 3;
                _radius = d;
                if (noOfDigits > 1)
                {
                    _centerx = Math.Round(_centerx, noOfDigits);
                    _centery = Math.Round(_centery, noOfDigits);
                    _centerz = Math.Round(_centerz, noOfDigits);
                }
            }

            // Maximal Distance to draw triangle.
            double noOfPoints = Math.Max(_pictureData.Width, _pictureData.Height);
            _maxDist = Fractrace.Basic.ParameterDict.Current.GetDouble("Export.X3d.ClosedSurfaceDist") * _radius / noOfPoints;

        }


        public Mesh CreateMesh()
        {
         
            _useDistance = !Fractrace.Basic.ParameterDict.Current.GetBool("Export.X3d.ClosedSurface");

            List<Coord2D> pointList = new List<Coord2D>();

            int[,] pointIndex = new int[_pictureData.Width + 1, _pictureData.Height + 1];

            double minx = Double.MaxValue;
            double miny = Double.MaxValue;
            double minz = Double.MaxValue;
            double maxx = Double.MinValue;
            double maxy = Double.MinValue;
            double maxz = Double.MinValue;

            int currentIndex = 0;
            int pointsFound = 0;
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
                        pointsFound++;
                    }
                    else
                    {
                        pointIndex[i, j] = -1;
                    }
                }
            }


            if (pointsFound < 100)
            {
                _valid = false;
                return null;
            }

            _radius = maxz - minz + maxy - miny + maxx - minx;
            _centerx = (maxx + minx) / 2.0;
            _centery = (maxy + miny) / 2.0;
            _centerz = (maxz + minz) / 2.0;

            _needScaling = _radius < 0.01;

            // Rounding scale parameters to allow combine different 3d scenes at later time. 
            int noOfDigits = 1;
            double d = 1;
            if (_needScaling || AlwaysScale)
            {
                while (d > _radius)
                {
                    d /= 10.0;
                    noOfDigits++;
                }
                noOfDigits -= 3;
                _radius = d;
                if (noOfDigits > 1)
                {
                    _centerx = Math.Round(_centerx, noOfDigits);
                    _centery = Math.Round(_centery, noOfDigits);
                    _centerz = Math.Round(_centerz, noOfDigits);
                }
            }

            // Maximal Distance to draw triangle.
            double noOfPoints = Math.Max(_pictureData.Width, _pictureData.Height);
            _maxDist = Fractrace.Basic.ParameterDict.Current.GetDouble("Export.X3d.ClosedSurfaceDist") * _radius / noOfPoints;

            // to test for invalid pdata
            double maxcol = 0;

            foreach (Coord2D coord in pointList)
            {
                PixelInfo pInfo = Transform(_pictureData.Points[coord.X, coord.Y]);
                if (pInfo != null && pInfo.Coord != null && pInfo.AdditionalInfo != null)
                {

                    double x, y, z;

                    if (_needScaling || AlwaysScale)
                    {
                        x = (pInfo.Coord.X - _centerx) / _radius;
                        y = (pInfo.Coord.Y - _centery) / _radius;
                        z = (pInfo.Coord.Z - _centerz) / _radius;
                    }
                    else
                    {
                        // Scale by 1000
                        x = 1000.0 * pInfo.Coord.X;
                        y = 1000.0 * pInfo.Coord.Y;
                        z = 1000.0 * pInfo.Coord.Z;
                    }

                    _mesh._coordinates.Add((float)x);
                    _mesh._coordinates.Add((float)y);
                    _mesh._coordinates.Add((float)z);

                    double red = pInfo.AdditionalInfo.red2;
                    double green = pInfo.AdditionalInfo.green2;
                    double blue = pInfo.AdditionalInfo.blue2;

                    _mesh._colors.Add((float)red);
                    _mesh._colors.Add((float)green);
                    _mesh._colors.Add((float)blue);

                    // test for invalid data only
                    if (maxcol < red)
                        maxcol = red;
                    if (maxcol < green)
                        maxcol = green;
                    if (maxcol < blue)
                        maxcol = blue;

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

                            if (_pictureData.Points[i - 1, j] != null)
                            {
                                // triangle 1
                                bool useTriangle = true;
                                if (_useDistance)
                                {
                                    PixelInfo point2 = _pictureData.Points[i - 1, j];
                                    PixelInfo point3 = _pictureData.Points[i - 1, j - 1];
                                    if (Dist(point1, point2) > _maxDist || Dist(point1, point3) > _maxDist)
                                        useTriangle = false;
                                }
                                if (useTriangle)
                                {
                                    _mesh._faces.Add(pointIndex[i, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j - 1]);

                                    _mesh._normales.Add((float)point1.Normal.X);
                                    _mesh._normales.Add((float)point1.Normal.Y);
                                    _mesh._normales.Add((float)point1.Normal.Z);

                                }

                            }
                            if (_pictureData.Points[i, j - 1] != null)
                            {
                                // triangle 2
                                bool useTriangle = true;
                                if (_useDistance)
                                {
                                    PixelInfo point2 = _pictureData.Points[i - 1, j - 1];
                                    PixelInfo point3 = _pictureData.Points[i, j - 1];
                                    if (Dist(point1, point2) > _maxDist || Dist(point1, point3) > _maxDist)
                                        useTriangle = false;
                                }

                                if (useTriangle)
                                {
                                    _mesh._faces.Add(pointIndex[i, j]);
                                    _mesh._faces.Add(pointIndex[i - 1, j - 1]);
                                    _mesh._faces.Add(pointIndex[i, j - 1]);

                                    _mesh._normales.Add((float)point1.Normal.X);
                                    _mesh._normales.Add((float)point1.Normal.Y);
                                    _mesh._normales.Add((float)point1.Normal.Z);

                                }

                            }
                        }
                    }
                }
            }
            if (maxcol < 0.0001)
            {
                _valid = false;
            }
            return _mesh;
        }


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


        /// <summary>
        /// Return distance between point1 and point2.
        /// </summary>
        protected double Dist(PixelInfo point1, PixelInfo point2)
        {
            if (point1 == null || point2 == null)
                return -1;
            if (point1.Coord == null || point2.Coord == null)
                return -1;
            double dx = point2.Coord.X - point1.Coord.X;
            double dy = point2.Coord.Y - point1.Coord.Y;
            double dz = point2.Coord.Z - point1.Coord.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }


    }
}
