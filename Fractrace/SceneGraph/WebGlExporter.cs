using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class WebGlExporter : SceneExporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public WebGlExporter(Iterate iter, PictureData pictureData):base( iter,  pictureData)
        {

        }

        protected override void CreateMesh()
        {
            MeshTool _meshTool = new MeshTool(_iterate, _pictureData);
            _meshTool.AlwaysScale = true;
            _mesh = _meshTool.CreateMesh();
            if (!_meshTool.Valid)
                _valid = false;
        }


        /// <summary>
        /// In VRML used number format.
        /// </summary>
        protected static System.Globalization.NumberFormatInfo _numberFormatInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US").NumberFormat;


        public override void Export(string fileName)
        {
            CreateMesh();

            float size = 6;
            // scale resultmesh to fit into [-size,-size,-size]-[size,size,size] box.
            _mesh.ComputeBoundingBox();
            float centerx=(_mesh.MaxBBox.X+_mesh.MinBBox.X)/ 2.0f;
            float centery = (_mesh.MaxBBox.Y + _mesh.MinBBox.Y) / 2.0f;
            float centerz = (_mesh.MaxBBox.Z + _mesh.MinBBox.Z) / 2.0f;
            float radiusx = _mesh.MaxBBox.X - _mesh.MinBBox.X;
            float radiusy = _mesh.MaxBBox.Y - _mesh.MinBBox.Y;
            float radiusz = _mesh.MaxBBox.Z - _mesh.MinBBox.Z;
            float radius = radiusx;
            if(radiusy>radius)
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

            sw.WriteLine(_htmlStart);

            StringBuilder sbColors = new StringBuilder();
            StringBuilder sbVertices = new StringBuilder();
            StringBuilder sbNormales = new StringBuilder();

            int noFaces = _mesh._faces.Count / 3;
            // Color per face:
            for (int faceIndex = 0; faceIndex < _mesh._faces.Count; faceIndex = faceIndex + 3)
            {
                // use color of first point in face
                int pointIndex = _mesh._faces[faceIndex];
                int colorindex = 3 * pointIndex;
                string line = _mesh._colors[colorindex].ToString(_numberFormatInfo) + ", " + _mesh._colors[colorindex + 1].ToString(_numberFormatInfo) +
                    ", " + _mesh._colors[colorindex + 2].ToString(_numberFormatInfo) + ", 1,";
                // Mark errors with red color.
                if (line.ToLower().Contains("nan"))
                    line = " 1, 0, 0, 1,";
                sbColors.AppendLine(line);
                sbColors.AppendLine(line);
                sbColors.AppendLine(line);

                for (int i=0;i<3;i++)
                {
                    int coordIndex = 3* _mesh._faces[faceIndex+i];

                    float x = _mesh._coordinates[coordIndex];
                    float y = _mesh._coordinates[coordIndex+1];
                    float z = _mesh._coordinates[coordIndex+2];

                    x = scale * (x - centerx);
                    y = scale * (y - centery);
                    z = scale * (z - centerz);

                    sbVertices.AppendLine(
                    x.ToString(_numberFormatInfo) + ", " + y.ToString(_numberFormatInfo) +
                    ", " + z.ToString(_numberFormatInfo) + ", "
                    );
                }

                line = _mesh._normales[faceIndex].ToString(_numberFormatInfo) + ", " + _mesh._normales[faceIndex + 1].ToString(_numberFormatInfo) +
                      ", " + _mesh._normales[faceIndex + 2].ToString(_numberFormatInfo) + ", ";
                sbNormales.AppendLine(line);
                sbNormales.AppendLine(line);
                sbNormales.AppendLine(line);
            }



            int noColors = noFaces * 3;
            int noCoords = noFaces * 3;
            int noNormals = noFaces * 3;

            sw.WriteLine(@"
var vertices = [
");
            sw.WriteLine(sbVertices.ToString());

            sw.WriteLine(@"
];
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
    triangleVertexPositionBufferID.itemSize = 3; 
    triangleVertexPositionBufferID.numItems = " + noCoords.ToString() + @";  

  triangleVertexColorBufferID = gl.createBuffer();
	gl.bindBuffer(gl.ARRAY_BUFFER,triangleVertexColorBufferID);
	var colors = [
");
            sw.WriteLine(sbColors.ToString());

         

            sw.WriteLine(@"
	];
	gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(colors), gl.STATIC_DRAW);
	triangleVertexColorBufferID.itemSize = 4;
	triangleVertexColorBufferID.numItems = "+ noColors.ToString() + @";
");

            sw.WriteLine(@"
	triangleVertexNormalBufferID = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, triangleVertexNormalBufferID);
    var vertexNormals = [
");
            sw.WriteLine(sbNormales.ToString());
            sw.WriteLine(@"
   ];
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexNormals), gl.STATIC_DRAW);
    triangleVertexNormalBufferID.itemSize = 3;
    triangleVertexNormalBufferID.numItems = "+ noNormals.ToString()+ @";
  }");
            sw.WriteLine(_htmlEnd);
            sw.Close();

        }

        private string _htmlStart = @"
<html>

<head>
<title>WebGL</title>
<meta http-equiv=""content-type"" content=""text/html; charset=ISO-8859-1"">

<script type = ""text/javascript"" src=""sylvester.js""></script>
<script type = ""text/javascript"" src=""glpsutils.js""></script>

<script id = ""shader-fs"" type=""x-shader/x-fragment"">
  #ifdef GL_ES
  precision highp float;
  #endif
  varying vec4 vColor;
  
  varying vec3 vLightWeighting;
	void main(void)
        {
            gl_FragColor = vec4(vColor.rgb * vLightWeighting, vColor.a);
        }
</script>

<script id = ""shader-vs"" type=""x-shader/x-vertex"">
  attribute vec3 aVertexPosition;
  attribute vec4 aVertexColor;
  attribute vec3 aVertexNormal;

  uniform mat4 uMVMatrix;
  uniform mat4 uPMatrix;
  uniform mat4 uNMatrix; // Normal Matrix
  varying vec4 vColor;
  uniform vec3 u_vAmbientColor;
  uniform vec3 u_vLightingDirection;
  uniform vec3 u_vDirectionalLightColor;
  uniform bool u_bUseLighting;
        varying vec3 vLightWeighting;
  
  
  void main(void)
        {
            vColor = aVertexColor;
            gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);
            if (!u_bUseLighting)
            {
                vLightWeighting = vec3(1.0, 1.0, 1.0);
            }
            else {
                vec4 transformedNormal = uNMatrix * vec4(aVertexNormal, 1.0);
                float fDirectionalLightWeighting = max(dot(transformedNormal.xyz, u_vLightingDirection), 0.0);
                vLightWeighting = u_vAmbientColor + u_vDirectionalLightColor * fDirectionalLightWeighting;
            }
        }
</script>

<script type = ""application/x-javascript"" >
  function Point(x, y)
        {
            this.x = x;
            this.y = y;
        }

        var gl;
        var shaderProgram;
        var vertexPositionAttribute;
        var pMatrix;
        var mvMatrix;

        var fZnear = 0.1;
        var fZfar = 100.0;

        var triangleVertexPositionBufferID;
        var triangleVertexColorBufferID;

        var g_fAnimationXPos = 0.0;
        var g_fAnimationYAngle = 0.0;
        var g_fAnimationXAngle = 0.0;
        var g_fTranslationTimeArg = 0.0;
        var g_fZZoomlevel = -5.0;
        var g_fTranslatX = 0.0;
        var g_fTranslatY = 0.0;

        var g_lastpoint = new Point(0, 0);
        var g_bMousedown = false;
        var g_DrawInterval = 0;

        function initBuffers()
        {
            triangleVertexPositionBufferID = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, triangleVertexPositionBufferID);
            ";

        private string _htmlEnd = @"
  function drawScene()  {
    gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

    pMatrix = createPerspectiveMatrix(45, 1.0, fZnear, fZfar); 
    mvMatrix =Matrix.I(4);
    mvMatrix = mvMatrix.x(create3DTranslationMatrix(Vector.create([g_fTranslatX, g_fTranslatY, g_fZZoomlevel])).ensure4x4());
	mvMatrix = mvMatrix.x(create3DTranslationMatrix(Vector.create([g_fAnimationXPos, 0.0, 0.0])).ensure4x4());
	mvMatrix = mvMatrix.x(Matrix.Rotation(g_fAnimationYAngle*Math.PI / 180.0, Vector.create([0,1,0])).ensure4x4());
	mvMatrix = mvMatrix.x(Matrix.Rotation(g_fAnimationXAngle*Math.PI / 180.0, Vector.create([1,0,0])).ensure4x4());
   
	gl.bindBuffer(gl.ARRAY_BUFFER, triangleVertexColorBufferID);
    gl.vertexAttribPointer(vertexColorAttribute, triangleVertexColorBufferID.itemSize, gl.FLOAT, false, 0, 0);
 
    gl.bindBuffer(gl.ARRAY_BUFFER, triangleVertexPositionBufferID);
 	gl.vertexAttribPointer(vertexPositionAttribute, triangleVertexPositionBufferID.itemSize, gl.FLOAT, false, 0, 0);
	
	// Normales:
    gl.bindBuffer(gl.ARRAY_BUFFER, triangleVertexNormalBufferID);
    gl.vertexAttribPointer(vertexNormalAttribute, triangleVertexNormalBufferID.itemSize, gl.FLOAT, false, 0, 0);

	var lighting = true; 
	var bLightingUniform = gl.getUniformLocation(shaderProgram, ""u_bUseLighting"");
    gl.uniform1i(bLightingUniform, lighting);
    if (lighting) {
      gl.uniform3f(
        ambientColorUniform,
		0,0,0
      );

	  // Headlight
      var lightingDirection = Vector.create([0, 0, -1.0]);
        lightingDirection.elements[0]=0;
      lightingDirection.elements[1]=0;
      lightingDirection.elements[2]=-1.0;
      var adjustedLD = lightingDirection.toUnitVector().x(-1);
        gl.uniform3f( lightingDirectionUniform,     adjustedLD.elements[0], adjustedLD.elements[1], adjustedLD.elements[2] );

      gl.uniform3f(
        directionalColorUniform,
		1, 1, 1 
      );
	}
    setMatrixUniforms();
    gl.drawArrays(gl.TRIANGLES, 0, triangleVertexPositionBufferID.numItems);
  }

function webGLStart()
{
    initGL();
    initShaders();

    var canvas = document.getElementById(""WebGL-canvas"");
    canvas.onmousedown = mouseDown;
    canvas.onmouseup = mouseUp;
    canvas.onmousemove = mouseMove;
    canvas.addEventListener('DOMMouseScroll', mouseWheel, false);
    document.addEventListener(""keydown"", keyDown, false);
    gl.clearColor(0.0, 0.0, 0.0, 1.0);
    gl.clearDepth(1.0);
    gl.enable(gl.DEPTH_TEST);
    gl.depthFunc(gl.LEQUAL);
    initBuffers();
    drawScene();
}

function mouseWheel(wheelEvent)
{
    if (wheelEvent.detail > 0)
        g_fZZoomlevel += 0.3;
    else
        g_fZZoomlevel -= 0.3;
    drawScene();
}

function mouseMove(einEvent)
{
    if (g_bMousedown == true)
    {
        g_fAnimationYAngle += (einEvent.clientX - g_lastpoint.x);
        g_fAnimationXAngle += (einEvent.clientY - g_lastpoint.y);
        //drawScene();
    }
    g_lastpoint.x = einEvent.clientX;
    g_lastpoint.y = einEvent.clientY;
}

function mouseDown(einEvent)
{
    g_bMousedown = true;
    g_DrawInterval = setInterval(drawScene, 40);
}

function mouseUp(einEvent)
{
    g_bMousedown = false;
    clearInterval(g_DrawInterval);
}

function keyDown(einEvent)
{
    switch (einEvent.keyCode)
    {

        case 37:  //  left cursor
                g_fTranslatX -= 0.2;
                break;
            case 38:  //  up cursor
                g_fZZoomlevel += 0.3;
                break;
            case 39:  //  right cursor
                g_fTranslatX += 0.2;
                break;
            case 40:  //  cursor down
                g_fZZoomlevel -= 0.3;
                break;
            default:
                break;
    }
    drawScene();
}
</script>
</head>
<body onload = ""webGLStart()"" >
  <canvas id=""WebGL-canvas"" style=""border: none;"" width=""1000"" height=""1000""></canvas>
<br><br>
</body>
</html>
";
    }
}
