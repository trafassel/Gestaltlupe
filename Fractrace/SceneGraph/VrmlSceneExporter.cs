using Fractrace.Basic;
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
        public VrmlSceneExporter(Iterate iter, PictureData pictureData):base(iter,pictureData)
        {
        }

        protected override string GetFileDescription() { return "VRML|*.wrl"; }

        public override bool FileTypeIsSupported(string fileName)
        {
            return fileName.ToLower().EndsWith(".wrl");
        }

        /// <summary>
        /// In VRML used number format.
        /// </summary>
        protected static System.Globalization.NumberFormatInfo _numberFormatInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US").NumberFormat;

       

        public override void Export(string fileName)
        {
            CreateMesh();
            StringBuilder normalString = new StringBuilder();
            StringBuilder normalIndex = new StringBuilder();

            StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));
            sw.WriteLine("#VRML V2.0 utf8");

            if(!_valid)
            {
                sw.Close();
                return;
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

            //  _mesh._coordinates
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
            
            // Color per face:
            for(int faceIndex = 0; faceIndex < _mesh._faces.Count; faceIndex = faceIndex + 3)
            {
                    // use color of first point in face
                    int pointIndex = _mesh._faces[faceIndex];
                    int colorIndex = 3 * pointIndex;
                    string line = _mesh._colors[colorIndex].ToString(_numberFormatInfo) + " " + _mesh._colors[colorIndex + 1].ToString(_numberFormatInfo) +
                        " " + _mesh._colors[colorIndex + 2].ToString(_numberFormatInfo) + ", ";
                    // Mark errors with red color.
                    if (line.ToLower().Contains("nan"))
                        line = " 1 0 0, ";
                    sw.WriteLine(line);

                normalString.AppendLine(_mesh._normales[faceIndex].ToString(_numberFormatInfo) + " " + _mesh._normales[faceIndex + 1].ToString(_numberFormatInfo) +
                        " " + _mesh._normales[faceIndex + 2].ToString(_numberFormatInfo) + ", ");

                int face = faceIndex / 3;
                normalIndex.Append(face.ToString()+" ");
            }

            sw.WriteLine(@"
]
}
normalPerVertex FALSE");

            sw.WriteLine(@"
coordIndex [
");

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
convex TRUE");

            sw.WriteLine(@"
normal Normal {
      vector[");
            sw.WriteLine(normalString.ToString());
            sw.WriteLine("]}");


            sw.WriteLine(@"
normalIndex [");
            sw.WriteLine(normalIndex.ToString());

            sw.WriteLine(@"]
colorPerVertex FALSE
}
appearance
Appearance{
material
Material{
");

            double shininess = ParameterDict.Current.GetDouble("Export.X3d.Shininess");
            if (shininess>0)
                sw.WriteLine(@"
ambientIntensity 0.5
diffuseColor 1 1 1
emissiveColor 0 0 0
shininess "+ shininess.ToString(_numberFormatInfo) + @"
specularColor 1 1 1
transparency 0");
            else
                sw.WriteLine(@"
ambientIntensity 1
diffuseColor 1 1 1
emissiveColor 1 1 1
shininess 0
specularColor 1 1 1
transparency 0");


            sw.WriteLine(@"
        }
}
}
]
}
Background{
groundColor []
skyColor [ 0 0 0]
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
