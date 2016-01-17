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
    public class X3DomExporter : SceneExporter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public X3DomExporter(Iterate iter, PictureData pictureData) : base(iter, pictureData)
        {

        }


        MeshTool _meshTool = null;

        protected override void CreateMesh()
        {
            _meshTool = new MeshTool(_iterate, _pictureData);
            _meshTool.AlwaysScale = true;
            _meshTool.InitMesh();

            /*
            _mesh = _meshTool.CreateMesh();
            if (!_meshTool.Valid)
                _valid = false;
                */
        }


        /// <summary>
        /// In VRML used number format.
        /// </summary>
        protected static System.Globalization.NumberFormatInfo _numberFormatInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US").NumberFormat;


        public void Init(Iterate iter, PictureData pictureData)
        {
            CreateMesh();
            _meshTool.Init(iter, pictureData);
        }


        public void Update(Iterate iter, PictureData pictureData)
        {
           _mesh = _meshTool.Update( iter,  pictureData);
        }


        public override void Export(string fileName)
        {
          

            float size = 100;
            // scale resultmesh to fit into [-size,-size,-size]-[size,size,size] box.
            _mesh.ComputeBoundingBox();
            float centerx = (_mesh.MaxBBox.X + _mesh.MinBBox.X) / 2.0f;
            float centery = (_mesh.MaxBBox.Y + _mesh.MinBBox.Y) / 2.0f;
            float centerz = (_mesh.MaxBBox.Z + _mesh.MinBBox.Z) / 2.0f;
            float radiusx = _mesh.MaxBBox.X - _mesh.MinBBox.X;
            float radiusy = _mesh.MaxBBox.Y - _mesh.MinBBox.Y;
            float radiusz = _mesh.MaxBBox.Z - _mesh.MinBBox.Z;
            float radius = radiusx;
            if (radiusy > radius)
                radius = radiusy;
            if (radiusz > radius)
                radius = radiusz;
            float scale = size / radius;

            StringBuilder normalString = new StringBuilder();
            StringBuilder normalIndex = new StringBuilder();

            StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));

            if (!_valid)
            {
                sw.Close();
                return;
            }

            sw.WriteLine(@"

<!DOCTYPE html PUBLIC "" -//W3C//DTD XHTML 1.0 Strict//EN""
        ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"" >
<html xmlns = ""http://www.w3.org/1999/xhtml"" >

    <head >
        <meta http-equiv=""Content-Type"" content=""text/html;charset=utf-8"" />
        <title >WebGL</title>
    </head>
    <body>

    <X3D showStat=""false"" showLog=""false"" x=""0px"" y=""0px"" width=""100%"" height=""950px"">
       <Scene>
          <Background skyColor='0 0 0'/>
          <Viewpoint position='0 0 3' ></Viewpoint>
          <Transform translation='0 0 0' rotation=""1 0 0 0"">
          <Shape >
            <IndexedFaceSet colorPerVertex=""FALSE"" normalPerVertex=""FALSE"" coordIndex=""
");

            // coordindex
            int coordSubIndex = 0;
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

            sw.WriteLine(@""">
              <Coordinate point = """);

            // coords
            coordSubIndex = 0;
            foreach (float coord in _mesh._coordinates)
            {
                coordSubIndex++;
                sw.WriteLine(coord.ToString(_numberFormatInfo) + " ");
                if (coordSubIndex == 3)
                {
                    coordSubIndex = 0;
                    sw.WriteLine(", ");
                }
            }

            sw.WriteLine(@"""/>
              <Color color = """);

            // colors

            // Color per face and normals
            for (int faceIndex = 0; faceIndex < _mesh._faces.Count; faceIndex = faceIndex + 3)
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
                normalIndex.Append(face.ToString() + " ");
            }

            sw.WriteLine(@"""/>");

            sw.WriteLine(@"
              <Normal vector=""");

            sw.WriteLine(normalString.ToString());

            sw.WriteLine(@""" normalIndex=""");

            sw.WriteLine(normalIndex.ToString());

            sw.WriteLine(@"""/>");

            double shininess = ParameterDict.Current.GetDouble("Export.X3d.Shininess");

            sw.WriteLine(@"
              </IndexedFaceSet >
              <Appearance>");

            if(shininess>0)
               sw.WriteLine(@"
                  <Material ambientIntensity=""0.5"" diffuseColor=""1 1 1"" emissiveColor=""0 0 0"" shininess=""" +
                   shininess.ToString(_numberFormatInfo) + @""" specularColor=""1 1 1"" />");
            else
                sw.WriteLine(@"
                  <Material ambientIntensity=""1"" diffuseColor=""0 0 0"" emissiveColor=""0 0 0"" shininess=""0"" specularColor=""0 0 0"" />");


            sw.WriteLine(@"
              </Appearance>
            </Shape >
          </Transform >                          
        </Scene>  
      </X3D>   
    <script type = ""text/javascript"" src = ""http://x3dom.org/x3dom/release/x3dom.js"" ></script>      
  </body>
</html>");

            sw.Close();
        }
        

    }
}
