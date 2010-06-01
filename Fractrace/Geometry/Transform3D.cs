using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Geometry
{
    public class Transform3D
    {


        public Transform3D()
        {
        }


        /// <summary>
        /// Initialisierung, hier können die Variablenwerte aus den globalen Einstellungen
        /// geholt werden.
        /// </summary>
        public virtual void Init()
        {


        }



        /// <summary>
        /// Dies ist einer reverse Transformation.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual Vec3 Transform(Vec3 input)
        {
            Vec3 p1 = new Vec3(input);
            return (p1);
        }

    }
}
