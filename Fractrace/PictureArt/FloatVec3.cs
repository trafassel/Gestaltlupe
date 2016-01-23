using Fractrace.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.PictureArt
{
    public class FloatVec3
    {
 

        public FloatVec3()
        {
        }


        public FloatVec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        public FloatVec3(FloatVec3 other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }


        public float X = 0;

        public float Y = 0;

        public float Z = 0;


        /// <summary>
        /// Return distance to other point.
        /// </summary>
        public float Dist(FloatVec3 other)
        {
            return Diff(other).Norm;
        }


        /// <summary>
        /// Return path to other point as vector.
        /// </summary>
        public FloatVec3 Diff(FloatVec3 other)
        {
            return (new FloatVec3(this.X - other.X, Y - other.Y, Z - other.Z));
        }


        /// <summary>
        /// Return distance to (0,0,0).
        /// </summary>
        /// <returns></returns>
        public float Norm
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }


        /// <summary>
        /// Multiply each component with t.
        /// </summary>
        /// <param name="t"></param>
        public FloatVec3 Mult(float t)
        {
            return new FloatVec3(X * t, Y * t, Z * t);
        }


        /// <summary>
        /// + Operator
        /// </summary>
        public FloatVec3 Sum(FloatVec3 other)
        {
            return new FloatVec3(X + other.X, Y + other.Y, Z + other.Z);
        }


        /// <summary>
        /// Add point other.
        /// </summary>
        public void Add(FloatVec3 other)
        {
            X += other.X;
            Y += other.Y;
            Z += other.Z;
        }


        /// <summary>
        /// Subtract componentwise.
        /// </summary>
        public void Subtract(FloatVec3 other)
        {
            X -= other.X;
            Y -= other.Y;
            Z -= other.Z;
        }


        /// <summary>
        /// Subtract componentwise.
        /// </summary>
        public void Subtract(float x, float y, float z)
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
            float d = X * X + Y * Y + Z * Z;
            if (d != 0)
            {
                d = (float)Math.Sqrt(d);
                X /= d;
                Y /= d;
                Z /= d;
            }
        }

    }
}
