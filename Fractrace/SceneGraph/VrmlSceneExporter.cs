using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class VrmlSceneExporter: SceneExporter
    {
        /// <summary>
        /// 2D integer coordinate.
        /// </summary>
        struct Coord2D { public Coord2D(int x, int y) { this.X = x; this.Y = y; } public int X; public int Y; }


        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public VrmlSceneExporter(Iterate iter, PictureData pictureData)
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
        /// Will be created in method Export()
        /// </summary>
        Mesh _mesh = null;


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



        public override void Export(string fileName)
        {
            MeshTool _meshTool = new MeshTool(_iterate, _pictureData);
            _mesh = _meshTool.CreateMesh();

         

            // _mesh._colors
            // string line = ((float)red).ToString(_numberFormatInfo) + " " +
            // ((float)green).ToString(_numberFormatInfo) + " " +
            // ((float)blue).ToString(_numberFormatInfo) + ", ";
            //  // Mark errors with red color.
            // if (line.ToLower().Contains("nan"))
            //    line = " 1 0 0, ";

            StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));
            sw.WriteLine("#VRML V2.0 utf8");


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

            //  _mesh._coordinates
            // sw.WriteLine(x.ToString(_numberFormatInfo) + " " + y.ToString(_numberFormatInfo) + " " + z.ToString(_numberFormatInfo) + ", ");
            int coordSubIndex = 0;
            foreach(float coord in _mesh._coordinates)
            {
                coordSubIndex++;
                sw.WriteLine(coord.ToString(_numberFormatInfo)+" ");
                if(coordSubIndex==3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine(", ");
                }
            }
           

            sw.WriteLine(@"
]
}
color
 Color{
color [
");

            /*
             string line = ((float)red).ToString(_numberFormatInfo) + " " +
                                  ((float)green).ToString(_numberFormatInfo) + " " +
                                  ((float)blue).ToString(_numberFormatInfo) + ", ";

                    // Mark errors with red color.
                    if (line.ToLower().Contains("nan"))
                        line = " 1 0 0, ";
                        */

            coordSubIndex = 0;
            foreach (float color in _mesh._colors)
            {
                coordSubIndex++;
                sw.WriteLine(color.ToString(_numberFormatInfo) + " ");
                if (coordSubIndex == 3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine(", ");
                }
            }

            sw.WriteLine(@"
]
}
normalPerVertex FALSE");

            sw.WriteLine(@"
coordIndex [
");

            // sw.WriteLine(pointIndex[i, j].ToString() + " " + pointIndex[i - 1, j].ToString() + " " + pointIndex[i - 1, j - 1].ToString() + " -1 ");
            coordSubIndex = 0;
            foreach (int face in _mesh._faces)
            {
                coordSubIndex++;
                sw.WriteLine(face.ToString() + " ");
                if (coordSubIndex == 3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine("-1 ");
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
