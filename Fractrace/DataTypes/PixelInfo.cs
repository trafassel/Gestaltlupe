using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Geometry;
using Fractrace.TomoGeometry;

namespace Fractrace.DataTypes {

    /// <summary>
    /// Enthält Zusatzinformationen, die später in einer Bildbearbeitung benutzt werden können,
    /// um mehr Informationen sichtbar zu machen
    /// </summary>
    public class PixelInfo {

        public PixelInfo() {

        }

        public AdditionalPointInfo AdditionalInfo = null;
      
        /// <summary>
        /// Realkoordinaten des Pixels
        /// </summary>
        public Vec3 Coord = new Vec3(0,0,0);

        /// <summary>
        /// Oberflächennormale der Gestalt bei diesem Pixel.
        /// </summary>
        public Vec3 Normal = new Vec3(0, 0, 0);

        /// <summary>
        /// Intensität der Lichteinstrahlung von vorne.
        /// </summary>
        public double frontLight = 0;

        /// <summary>
        /// Erste Ableitung der Oberfläche. Gibt den Grad der Krümmung an.
        /// </summary>
        public double derivation = 0;


      /// <summary>
      /// Anzahl der Iterationen. Dient der Farbdarstellung des Schnittes.
      /// </summary>
        public double iterations = -1;


        /// <summary>
        /// True, if the corresponding pixel is part of the inside view
        /// </summary>
        public bool IsInside = false;

        public PixelInfo Clone()
        {
            PixelInfo retVal = new PixelInfo();
            retVal.Coord.X = Coord.X;
            retVal.Coord.Y = Coord.Y;
            retVal.Coord.Z = Coord.Z;
            retVal.Normal.X = Normal.X;
            retVal.Normal.Y = Normal.Y;
            retVal.Normal.Z = Normal.Z;
            retVal.frontLight = frontLight;
            retVal.derivation = derivation;
            retVal.iterations = iterations;
            if (AdditionalInfo != null)
                retVal.AdditionalInfo = AdditionalInfo.Clone();
            return retVal;
        }
        
    }
}
