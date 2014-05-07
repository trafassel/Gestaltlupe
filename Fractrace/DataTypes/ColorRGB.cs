using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fractrace.DataTypes {


    /// <summary>
    /// Datatype to handle color in rgb values where each entry is a double.
    /// </summary>
    public class ColorRGB {

        /// <summary>
        /// Red component of the corresponding color. Valid, if Red is in [0,1].
        /// </summary>
        double Red = 0;

        /// <summary>
        /// Green component of the corresponding color. Valid, if Green is in [0,1].
        /// </summary>
        double Green = 0;

        /// <summary>
        /// Blue component of the corresponding color. Valid, if Blue is in [0,1].
        /// </summary>
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
        /// Convert this to System.Drawing.Color datatype.
        /// </summary>
        public Color Color {
            get {
                if (Red < 0)
                    Red = 0;
                if (Red > 1)
                    Red = 1;

                if (Green < 0)
                    Green = 0;
                if (Green > 1)
                    Green = 1;

                if (Blue < 0)
                    Blue = 0;
                if (Blue > 1)
                    Blue = 1;

                return Color.FromArgb((int)(255.0 * Red), (int)(255.0 * Green), (int)(255.0 * Blue));
            }
        }

    }
}
