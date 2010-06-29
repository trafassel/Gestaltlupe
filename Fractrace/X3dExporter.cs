using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Fractrace.Basic;
using Fractrace.DataTypes;

namespace Fractrace {

  /// <summary>
  /// Dient dem Export der dargestellten 3D Geometrie.
  /// </summary>
  public class X3dExporter {

    /// <summary>
    /// Initializes a new instance of the <see cref="X3dExporter"/> class.
    /// </summary>
    /// <param name="iter">The iter.</param>
    public X3dExporter(Iterate iter) {
      mIterate = iter;
    }


    protected Iterate mIterate = null;


    protected static System.Globalization.NumberFormatInfo mNumberFormatInfo = System.Globalization.CultureInfo.CreateSpecificCulture("en-US").NumberFormat;



    /// <summary>
    /// Liefert den Punktabstand
    /// </summary>
    /// <param name="point1">The point1.</param>
    /// <param name="point2">The point2.</param>
    /// <returns></returns>
    protected double Dist(PixelInfo point1, PixelInfo point2) {
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
    /// Wendet die mIterate.LastUsedFormulas verwendete Transformation auf die übergebenen Punktkoordinaten an.
    /// Wenn die Transformationsinformationen nicht vorliegen werden die Ursprungskoordinaten geliefert.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public PixelInfo AddTransform(PixelInfo input) {
      if (mIterate != null) {
        if (mIterate.LastUsedFormulas != null) {
          Geometry.Vec3 vec = mIterate.LastUsedFormulas.GetTransform(input.Coord.X, input.Coord.Y, input.Coord.Z);
          PixelInfo tempPoint = new PixelInfo();
          tempPoint.Coord = vec;
          tempPoint.AdditionalInfo = input.AdditionalInfo;
          tempPoint.Normal = input.Normal; // Die normale wird noch nicht berücksichtigt.
          return tempPoint;
        }
      }
      return input;
    }


    /// <summary>
    /// Die Geometrie aus iter wird in der VRML-Datei fileName abgelegt.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void Save(string fileName) {
      if (mIterate == null)
        return;
      StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));
      sw.WriteLine("#VRML V2.0 utf8");

      List<KeyValuePair<int,int>> pointList=new List<KeyValuePair<int,int>>(); 
      Dictionary<KeyValuePair<int,int>,int> indexList=new Dictionary<KeyValuePair<int,int>,int>();

       int[,] pointIndex = new int[ mIterate.PictureData.Width+1,  mIterate.PictureData.Height+1];

       //   Points = new PixelInfo[width + 1, height + 1];


       double minx = Double.MaxValue;
       double miny = Double.MaxValue;
       double minz = Double.MaxValue;
       double maxx = Double.MinValue;
       double maxy = Double.MinValue;
       double maxz = Double.MinValue;



      int currentIndex=0;
for (int i = 0; i < mIterate.PictureData.Width; i++) {
  for (int j = 0; j < mIterate.PictureData.Height; j++) {
    if (mIterate.PictureData.Points[i, j] != null) {
      PixelInfo point = AddTransform(mIterate.PictureData.Points[i, j]);
      /*
      if (mIterate != null) {
        if (mIterate.LastUsedFormulas != null) {
          Geometry.Vec3 vec = mIterate.LastUsedFormulas.GetTransform(point.Coord.X, point.Coord.Y, point.Coord.Z);
          PixelInfo tempPoint = new PixelInfo();
          tempPoint.Coord = vec;
          tempPoint.AdditionalInfo = point.AdditionalInfo;
          tempPoint.Normal = point.Normal; // Die normale wird noch nicht berücksichtigt.
          point = tempPoint;
        }
      }
       */
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


      KeyValuePair<int,int> coord =new KeyValuePair<int,int>(i,j);
      pointIndex[i,j]=currentIndex;
      pointList.Add(coord);

      currentIndex++;
    } else {
       pointIndex[i,j]=-1;
    }
  }
}

  // Nachbarpunkte mit diesem Abstand werden als nicht zusammenhängend betrachtet.
double maxDist=0;
      double tempDist= maxx-minx;
      if (maxDist < tempDist)
        maxDist = tempDist;
      tempDist = maxy - miny;
      if (maxDist < tempDist)
        maxDist = tempDist;
      tempDist = maxz - minz;
      if (maxDist < tempDist)
        maxDist = tempDist;

      double noOfPoints = Math.Max(mIterate.PictureData.Width, mIterate.PictureData.Height);

      maxDist = 5.0* maxDist / noOfPoints;

      sw.WriteLine(@"
      DEF Transform  Transform{
translation 0 0 0
center 0 0 0
rotation 0 0 1 0
scale 1 1 1
children
 [
DEF Shape  Shape{
geometry
DEF IndexedFaceSet  IndexedFaceSet{
coord
 Coordinate{
point [
");
  
 // Hier die Koordinaten eintragen:
 // z.B: point [ 0 0 0.3, 1 0 0, 1 1 0, 0 1 0]

      double oldx = 0;
      double oldy = 0;
      double oldz = 0;

foreach (  KeyValuePair<int,int> entry in pointList) {
  if( mIterate.PictureData.Points[entry.Key,entry.Value]!=null) { 
    PixelInfo pInfo= AddTransform(mIterate.PictureData.Points[entry.Key,entry.Value]);
        if (pInfo != null) {
          if (pInfo.Coord != null) {
            // Skalierung unter Einstellungen festlegen
            double x = 1000.0 * pInfo.Coord.X;
            double y = 1000.0 * pInfo.Coord.Y;
            double z = 1000.0 * pInfo.Coord.Z;
            if (pInfo.Coord.X < 1000 && pInfo.Coord.X > -1000 && pInfo.Coord.Y < 1000 && pInfo.Coord.Y > -1000 &&pInfo.Coord.Z < 1000 && pInfo.Coord.Z > -1000) { // nicht der beste Schutz vor Ausreißern
              sw.WriteLine(x.ToString(mNumberFormatInfo) + " " + y.ToString(mNumberFormatInfo) + " " + z.ToString(mNumberFormatInfo) + ", ");
              oldx = x;
              oldy = y;
              oldz = z;
            } else {
              sw.WriteLine(oldx.ToString(mNumberFormatInfo) + " " + oldy.ToString(mNumberFormatInfo) + " " + oldz.ToString(mNumberFormatInfo) + ", ");
            }
          }
        }
      }
  }
 
 sw.WriteLine(@"
]
}
color
 Color{
color [
");

 // Hier die Farben eintragen:
 // z.B: color [ 0 1 0, 1 1 0, 1 0 0, 0 0 1]

    foreach (  KeyValuePair<int,int> entry in pointList) {
  if( mIterate.PictureData.Points[entry.Key,entry.Value]!=null) { 
    PixelInfo pInfo= mIterate.PictureData.Points[entry.Key,entry.Value];
        if (pInfo != null) {
// foreach (PixelInfo pInfo in mIterate.PictureData.Points) {
   if (pInfo != null) {
     if (pInfo.Coord != null) {
       if (pInfo.AdditionalInfo != null) {
         double red=pInfo.AdditionalInfo.red;
         double green=pInfo.AdditionalInfo.green;
          double blue=pInfo.AdditionalInfo.blue;

         double norm=Math.Sqrt(red*red+green*green+blue*blue);
         if (norm != 0) {
           red = Math.Abs(red / norm);
           green = Math.Abs(green / norm);
           blue = Math.Abs(blue / norm);
         } else {
           red = 0;
           green = 0;
           blue = 0;
         }

         string line=red.ToString(mNumberFormatInfo) + " " + green.ToString(mNumberFormatInfo) + " " + blue.ToString(mNumberFormatInfo) + ", ";
        
         // Sollte eigentlich nicht mehr notwendig sein: bei Problemen wird die rote Farbe gezeigt.
         if(line.ToLower().Contains("nan"))
           line=" 1 0 0, ";
         sw.WriteLine(line);
       }
//       sw.WriteLine(pInfo.Coord.X.ToString(mNumberFormatInfo) + " " + pInfo.Coord.Y.ToString(mNumberFormatInfo) + " " + pInfo.Coord.Z.ToString(mNumberFormatInfo) + ", ");
     }
   }

 }
  }
    }

sw.WriteLine(@"
]
}
normalPerVertex FALSE
colorIndex [
");

 // Jetzt die Farben den Punkten zuweisen (einfaches Hochzählen):
 // z.B: colorIndex [0, 1, 2]
currentIndex = 0;

foreach (  KeyValuePair<int,int> entry in pointList) {
  if( mIterate.PictureData.Points[entry.Key,entry.Value]!=null) { 
    PixelInfo pInfo= mIterate.PictureData.Points[entry.Key,entry.Value];
        if (pInfo != null) {
//foreach (PixelInfo pInfo in mIterate.PictureData.Points) {
  if (pInfo != null) {
    if (pInfo.Coord != null) {
      if (pInfo.AdditionalInfo != null) {
        sw.Write(currentIndex.ToString() + ", ");
        currentIndex++;
      }
    }
  }
}
  }
       }

sw.WriteLine(@"
]
coordIndex [
");

 // Die Koordinaten anpassen: 
// z.B: coordIndex [0, 1, 2, 3, -1]

for (int i = 0; i < mIterate.PictureData.Width; i++) {
  for (int j = 0; j < mIterate.PictureData.Height; j++) {
    if (mIterate.PictureData.Points[i, j] != null) {
      // Dreieck 1:
      if (i > 0 && j > 0 && mIterate.PictureData.Points[i - 1, j - 1] != null) {
        if (mIterate.PictureData.Points[i - 1, j] != null) {
          // Nur dann Kante einfügen, wenn sich der Punktabstand nicht wesentlich vom vorherigen Punkt unterscheidet.
          PixelInfo point1 = mIterate.PictureData.Points[i, j];
          PixelInfo point2 = mIterate.PictureData.Points[i-1, j];
          PixelInfo point3 = mIterate.PictureData.Points[i-1, j-1];
          if(Dist(point1,point2)<maxDist && Dist(point2,point3)<maxDist && Dist(point3,point2)<maxDist)   
            sw.WriteLine(pointIndex[i, j].ToString() + " " + pointIndex[i - 1, j].ToString() + " " + pointIndex[i - 1, j - 1].ToString() + " -1 ");
        }
        if (mIterate.PictureData.Points[i, j-1] != null) {
          // Nur dann Kante einfügen, wenn sich der Punktabstand nicht wesentlich vom vorherigen Punkt unterscheidet
          PixelInfo point1 = mIterate.PictureData.Points[i, j];
          PixelInfo point2 = mIterate.PictureData.Points[i - 1, j-1];
          PixelInfo point3 = mIterate.PictureData.Points[i , j - 1];
          if (Dist(point1, point2) < maxDist && Dist(point2, point3) < maxDist && Dist(point3, point2) < maxDist)   
          sw.WriteLine(pointIndex[i, j].ToString() + " " + pointIndex[i - 1, j-1].ToString() + " " + pointIndex[i, j - 1].ToString() + " -1 ");
        }
      }
    }
  }
}


sw.WriteLine(@"
]
ccw FALSE
solid FALSE
texCoordIndex []
creaseAngle 0
convex TRUE
normalIndex []
colorPerVertex TRUE
}
appearance
DEF Appearance  Appearance{
material
DEF Material  Material{
ambientIntensity 0.2
diffuseColor 0.8 0.8 0.8
emissiveColor 0 0 0
shininess 0.2
specularColor 0 0 0
transparency 0
}
}
}
]
}
DEF Background0  Background{
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
      /*
sw.WriteLine(@"
");
sw.WriteLine(@"
");
sw.WriteLine(@"
");
sw.WriteLine(@"
");
sw.WriteLine(@"
");
       * */
sw.Close();
      

    }

  }
}
