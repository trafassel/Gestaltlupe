using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.Geometry {

    /// <summary>
    /// Ein Punkt im 3D-Raum
    /// </summary>
    public class Vec3 {

        public Vec3(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public Vec3(Vec3 other) {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }


        public double X = 0;

        public double Y = 0;

        public double Z = 0;


        /// <summary>
        /// Liefert den Abstand zu einem anderen Punkt.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public double Dist(Vec3 other) {
            return Diff(other).Norm;
        }


        /// <summary>
        /// Zieht other ab und liefert die Differenz.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Vec3 Diff(Vec3 other) {
            return( new Vec3(this.X - other.X, Y - other.Y, Z - other.Z));
        }


        /// <summary>
        /// Liefert den Abstand zum Ursprung.
        /// </summary>
        /// <returns></returns>
        public double Norm {
            get {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }


        /// <summary>
        /// Multipliziert jeden Eintrag mit t.
        /// </summary>
        /// <param name="t"></param>
        public Vec3 Mult(double t) {
            return new Vec3(X * t, Y * t, Z * t);
        }


        /// <summary>
        /// + Operator
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Vec3 Add(Vec3 other) {
            return new Vec3(X +other.X, Y +other.Y, Z+other.Z);
        }

    }
}
