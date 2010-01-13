using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Fractrace.DataTypes {


    public class ColorRGB {

        double Red = 0;

        double Green = 0;

        double Blue = 0;


        /// <summary>
        /// Constructer.
        /// </summary>
        public ColorRGB() {

        }

        /// <summary>
        /// Constructer.
        /// </summary>
        public ColorRGB(double col) {
            Red = col;
            Green = col;
            Blue = col;
        }


        /// <summary>
        /// Constructer.
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public ColorRGB(double red, double green,double blue) {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }


        /// <summary>
        /// Liefert die entsprechende Zeichenfarbe.
        /// </summary>
        public Color Color {
            get {
                if (Red < 0)
                    Red = 0;
                if (Red > 255)
                    Red = 255;

                if (Green < 0)
                    Green = 0;
                if (Green > 255)
                    Green = 255;
                if (Blue < 0)
                    Blue = 0;
                if (Blue > 255)
                    Blue = 255;
                return Color.FromArgb((int)(255.0 * Red), (int)(255.0 * Green), (int)(255.0 * Blue));
            }
        }

    }
}
