using System;


namespace Fractrace.Geometry
{

    /// <summary>
    /// A point with 3D coordinates.
    /// </summary>
    public class Vec3
    {

        public Vec3()
        {
        }


        public Vec3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public Vec3(Vec3 other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }


        public double X = 0;

        public double Y = 0;

        public double Z = 0;


        /// <summary>
        /// Return distance to other point.
        /// </summary>
        public double Dist(Vec3 other)
        {
            return Diff(other).Norm;
        }


        /// <summary>
        /// Return path to other point as vector.
        /// </summary>
        public Vec3 Diff(Vec3 other)
        {
            return (new Vec3(this.X - other.X, Y - other.Y, Z - other.Z));
        }


        /// <summary>
        /// Return distance to (0,0,0).
        /// </summary>
        /// <returns></returns>
        public double Norm
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }


        /// <summary>
        /// Multiply each component with t.
        /// </summary>
        public Vec3 Mult(double t)
        {
            return new Vec3(X * t, Y * t, Z * t);
        }


        /// <summary>
        /// + Operator
        /// </summary>
        public Vec3 Sum(Vec3 other)
        {
            return new Vec3(X + other.X, Y + other.Y, Z + other.Z);
        }


        /// <summary>
        /// Add point other.
        /// </summary>
        public void Add(Vec3 other)
        {
            X += other.X;
            Y += other.Y;
            Z += other.Z;
        }


        /// <summary>
        /// Subtract componentwise.
        /// </summary>
        public void Subtract(Vec3 other)
        {
            X -= other.X;
            Y -= other.Y;
            Z -= other.Z;
        }


        /// <summary>
        /// Subtract componentwise.
        /// </summary>
        public void Subtract(double x, double y, double z)
        {
            X -= x;
            Y -= y;
            Z -= z;
        }


        /// <summary>
        /// Normalize all entries.
        /// </summary>
        public void Normalize()
        {
            double d = X * X + Y * Y + Z * Z;
            if (d != 0)
            {
                d = Math.Sqrt(d);
                X /= d;
                Y /= d;
                Z /= d;
            }
        }


    }
}
