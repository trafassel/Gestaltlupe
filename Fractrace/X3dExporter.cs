using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Fractrace.DataTypes;

namespace Fractrace
{

    /// <summary>
    /// Export surface data of last iterate with surface color, defined in last picture art rendering.
    /// </summary>
    public class X3dExporter
    {

        /// <summary>
        /// 2D integer coordinate.
        /// </summary>
        struct Coord2D { public Coord2D(int x, int y) { this.X = x; this.Y = y; } public int X; public int Y; }

        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public X3dExporter(Iterate iter, PictureData pictureData)
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
        /// In VRML used number format.
        /// </summary>
        protected static System.Globalization.NumberFormatInfo _numberFormatInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US").NumberFormat;


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
                    tempPoint.Normal = input.Normal;
                    return tempPoint;
                }
            }
            return input;
        }


        /// <summary>
        /// Create and save VRML file.
        /// </summary>
        public void Save(string fileName)
        {
            if (_iterate == null)
                return;

            StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));
            sw.WriteLine("#VRML V2.0 utf8");

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

            sw.WriteLine(@"
      Transform{
translation 0 0 0
center 0 0 0
rotation 0 0 1 0
scale 1 1 1
children
 [
 Shape{
geometry
IndexedFaceSet{
coord
 Coordinate{
point [
");



            double radius = maxz - minz + maxy - miny + maxx - minx;
            double centerx = (maxx + minx) / 2.0;
            double centery = (maxy + miny) / 2.0;
            double centerz = (maxz + minz) / 2.0;

            bool needScaling = radius < 0.002;

            // Rounding scale parameters to allow combine different 3d scenes at later time. 
            int noOfDigits = 1;
            double d = 1;
            if (needScaling)
            {
                while (d > radius)
                {
                    d /= 10.0;
                    noOfDigits++;
                }
                noOfDigits -= 3;
                radius = d;
                centerx = Math.Round(centerx, noOfDigits);
                centery = Math.Round(centery, noOfDigits);
                centerz = Math.Round(centerz, noOfDigits);
            }

            foreach (Coord2D coord in pointList)
            {
                PixelInfo pInfo = Transform(_pictureData.Points[coord.X, coord.Y]);
                if (pInfo != null && pInfo.Coord != null && pInfo.AdditionalInfo != null)
                {

                    double x, y, z;
                    if (needScaling)
                    {
                        x = (pInfo.Coord.X - centerx) / radius;
                        y = (pInfo.Coord.Y - centery) / radius;
                        z = (pInfo.Coord.Z - centerz) / radius;
                    }
                    else
                    {
                        // Scale by 1000

                        x = 1000.0 * pInfo.Coord.X;
                        y = 1000.0 * pInfo.Coord.Y;
                        z = 1000.0 * pInfo.Coord.Z;
                    }


                    sw.WriteLine(x.ToString(_numberFormatInfo) + " " + y.ToString(_numberFormatInfo) + " " + z.ToString(_numberFormatInfo) + ", ");
                }
            }

            sw.WriteLine(@"
]
}
color
 Color{
color [
");

            // Colors
            foreach (Coord2D coord in pointList)
            {
                PixelInfo pInfo = _pictureData.Points[coord.X, coord.Y];
                if (pInfo != null && pInfo.Coord != null && pInfo.AdditionalInfo != null)
                {
                    double red = pInfo.AdditionalInfo.red2;
                    double green = pInfo.AdditionalInfo.green2;
                    double blue = pInfo.AdditionalInfo.blue2;

                    string line = ((float)red).ToString(_numberFormatInfo) + " " +
                                  ((float)green).ToString(_numberFormatInfo) + " " +
                                  ((float)blue).ToString(_numberFormatInfo) + ", ";

                    // Mark errors with red color.
                    if (line.ToLower().Contains("nan"))
                        line = " 1 0 0, ";
                    sw.WriteLine(line);
                }
            }

            sw.WriteLine(@"
]
}
normalPerVertex FALSE");

            sw.WriteLine(@"
coordIndex [
");

            // Maximal Distance to draw triangle.
            double noOfPoints = Math.Max(_pictureData.Width, _pictureData.Height);
            double maxDist = Fractrace.Basic.ParameterDict.Current.GetDouble("Export.X3d.ClosedSurfaceDist") * radius / noOfPoints;

            // surface mesh
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

                            bool useDistance = !Fractrace.Basic.ParameterDict.Current.GetBool("Export.X3d.ClosedSurface");
                            if (_pictureData.Points[i - 1, j] != null)
                            {
                                // triangle 1

                                PixelInfo point2 = _pictureData.Points[i - 1, j];
                                PixelInfo point3 = _pictureData.Points[i - 1, j - 1];
                                if (!useDistance || (Dist(point1, point2) < maxDist && Dist(point2, point3) < maxDist && Dist(point3, point2) < maxDist))
                                    sw.WriteLine(pointIndex[i, j].ToString() + " " + pointIndex[i - 1, j].ToString() + " " + pointIndex[i - 1, j - 1].ToString() + " -1 ");
                            }
                            if (_pictureData.Points[i, j - 1] != null)
                            {
                                // triangle 2
                                PixelInfo point2 = _pictureData.Points[i - 1, j - 1];
                                PixelInfo point3 = _pictureData.Points[i, j - 1];
                                if (!useDistance || (Dist(point1, point2) < maxDist && Dist(point2, point3) < maxDist && Dist(point3, point2) < maxDist))
                                    sw.WriteLine(pointIndex[i, j].ToString() + " " + pointIndex[i - 1, j - 1].ToString() + " " + pointIndex[i, j - 1].ToString() + " -1 ");
                            }

                        }
                    }
                }
            }

            sw.WriteLine(@"
]
ccw TRUE
solid TRUE
texCoordIndex []
creaseAngle 0
convex TRUE
normalIndex []
colorPerVertex TRUE
}
appearance
Appearance{
material
Material{
ambientIntensity 1
diffuseColor 1 1 1
emissiveColor 1 1 1
shininess 0
specularColor 0 0 0
transparency 0
}
}
}
]
}
Background{
groundColor []
skyColor [ 0.2 0.2 0.25]
skyAngle []
backUrl []
frontUrl []
bottomUrl []
rightUrl []
groundAngle []
leftUrl []
topUrl []
}
");

            sw.Close();

        }


    }
}
