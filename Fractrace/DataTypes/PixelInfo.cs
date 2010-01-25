using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Fractrace.Geometry;

namespace Fractrace.DataTypes {

    /// <summary>
    /// Enthält Zusatzinformationen, die später in einer Bildbearbeitung benutzt werden können,
    /// um mehr Informationen sichtbar zu machen
    /// </summary>
    public class PixelInfo {

        public PixelInfo() {

        }


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

        
    }
}
