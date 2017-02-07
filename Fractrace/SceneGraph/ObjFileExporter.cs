


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
    // Export wavefront obj file.
    public class ObjFileExporter : SceneExporter
    {
        /// <summary>
        /// 2D integer coordinate.
        /// </summary>
        struct Coord2D { public Coord2D(int x, int y) { this.X = x; this.Y = y; } public int X; public int Y; }

        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public ObjFileExporter(Iterate iter, PictureData pictureData) : base(iter, pictureData)
        {
        }

        protected override string GetFileDescription() { return "Dat (*.obj)|*.obj"; }

        public override bool FileTypeIsSupported(string fileName)
        {
            return fileName.ToLower().EndsWith(".obj");
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
            sw.WriteLine("# wavefront obj file written by Gestaltlupe");
            if (!_valid)
            {
                sw.Close();
                return;
            }
            sw.WriteLine(@"
   mtllib NONE
");
            //  _mesh._coordinates
            int coordSubIndex = 0;
            foreach (float coord in _mesh._coordinates)
            {
                if (coordSubIndex == 0)
                {
                    sw.Write("v ");
                }
                coordSubIndex++;
                sw.Write(coord.ToString(_numberFormatInfo) + " ");
                if (coordSubIndex == 3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine("");
                }
            }

            sw.WriteLine(@"
            g grp1
            usemtl mtlNONE
");
            coordSubIndex = 0;
            foreach (int face in _mesh._faces)
            {
                if (coordSubIndex == 0)
                {
                    sw.Write("f ");
                }
                coordSubIndex++;
                sw.Write((face + 1).ToString() + " ");
                if (coordSubIndex == 3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine("");
                }
            }
            sw.WriteLine(@"
    
#end
");
            sw.Close();
        }

    }
}


