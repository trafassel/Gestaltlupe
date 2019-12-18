using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;
using Fractrace.Geometry;
using Fractrace.DataTypes;
using Fractrace.TomoGeometry;

namespace Fractrace
{


    /// <summary>
    /// Fest implementierte Formeln.
    /// </summary>
    public class Formulas
    {

        /* global variables */
        public double[] col = new double[256];

        // Parameter, die durch die Eingabemaske definiert werden
        private static double gr = 10;
    
        PictureData pData = null;

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public Formulas(PictureData pData)
        {
            this.pData = pData;
        }


        /// <summary>
        /// Initialisierung
        /// </summary>
        public Formulas()
        {

        }

        /// <summary>
        /// Zusätzliche Projektion.
        /// </summary>
        public Projection Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                _projection = value;
            }

        }
        protected Projection _projection = null;


        protected List<Transform3D> _transforms = new List<Transform3D>();

        /// <summary>
        /// Zugriff auf die Liste der Zusatztransformationen, z.B. weitere Stauchungen
        /// oder Drehungen.
        /// </summary>
        public List<Transform3D> Transforms
        {
            get
            {
                return _transforms;
            }
        }


        protected TomoFormula mInternFormula = null;

        public TomoFormula InternFormula
        {
            get
            {
                return mInternFormula;
            }

            set
            {
                mInternFormula = value;
            }
        }





        /* Berechnung des Winkels des Lotes der Oberfläche
           mit Lichtquelle  (Betrachter selbst ) */
        public double Winkel(long zykl, double x, double y, double z, double zz,
               double xd, double yd, double zd, double zzd,
               double wix, double wiy, double wiz,
               double jx, double jy, double jz, double jzz, int formula, bool perspective, bool invers)
        {

            if (perspective)
            {
                return 0;
            }
            double m = 0, n = 0, xv = 0, yv = 0, zv = 0, winkel = 0, yn = 0, diff = 0;
            double xn = 0, zn = 0, zzn = 0, xm = 0, ym = 0, zm = 0, zzm = 0;
            double ox = 0, oy = 0, oz = 0, rx = 0, ry = 0, rz = 0;
            double[] tief = new double[5];
            double startwert = 0;
            int k = 0;
            int wert = 0, pu = 0;

            double distance = 0.19;
            n = 0;
            for (k = 4; k >= 0; k--)
            {
                switch (k)
                {
                    case 0:     /* oben */
                        xn = x + distance * xd; yn = y - distance * yd;
                        zn = z + 2 * distance * zd; zzn = zz + 2 * distance * zzd;
                        break;
                    case 1:     /* rechts  */
                        xn = x + 2 * distance * xd; yn = y + distance * yd;
                        zn = z + distance * zd; zzn = zz + distance * zzd;
                        break;
                    case 2:     /* unten */
                        xn = x - distance * xd; yn = y + distance * yd;
                        zn = z - 2 * distance * zd; zzn = zz - 2 * distance * zzd;
                        break;
                    case 3:     /* links  */
                        xn = x - 2 * distance * xd; yn = y - distance * yd;
                        zn = z - distance * zd; zzn = zz - distance * zzd;
                        break;
                    case 4:     /* mitte  */
                        xn = x; yn = y; zn = z; zzn = zz;
                        break;
                }
                zn = zn + n * zd; xn = xn - n * xd;
                yn = yn + n * yd; zzn = zzn + n * zzd;
                if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                {
                    for (m = 0; m >= -4.0; m -= 0.2)
                    {
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            break;
                    }
                }
                else
                {
                    for (m = 0; m <= 4.0; m += 0.2)
                    {
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
                    }
                }
                if ((m > -3.9) && (m < 3.9))
                {
                    startwert = m + 0.2; diff = 0.1;
                    while (diff >= 0.0001)
                    {
                        m = startwert - diff;
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                            startwert = m + diff;
                        else startwert = m;
                        diff /= 2.0;
                    }
                    if (k == 4)
                    {
                        n = m;
                    }
                    tief[k] = m;
                }
                else tief[k] = 10;
            }
            tief[4] = 0;
            for (k = 0; k < 4; k++)
            {
                pu = k + 1; if (k == 3) pu = 0;
                /* Die drei Punkte entsprechen tief[4] tief[k] und tief[pu]   */
                /* Zuerst wird tief abgezogen                                 */
                if ((tief[k] < 9) && (tief[pu] < 9) && tief[4] < 9)
                {
                    switch (k)
                    {
                        case 0:
                            ox = 0.2 - tief[k]; rx = 0.4 - tief[pu];
                            oy = -0.2 + tief[k]; ry = 0.2 + tief[pu];
                            oz = 0.4 + tief[k]; rz = 0.2 + tief[pu];
                            break;
                        case 1:
                            ox = 0.4 - tief[k]; rx = -0.2 - tief[pu];
                            oy = 0.2 + tief[k]; ry = 0.2 + tief[pu];
                            oz = 0.2 + tief[k]; rz = -0.4 + tief[pu];
                            break;
                        case 2:
                            ox = -0.2 - tief[k]; rx = -0.4 - tief[pu];
                            oy = 0.2 + tief[k]; ry = -0.2 + tief[pu];
                            oz = -0.4 + tief[k]; rz = -0.2 + tief[pu];
                            break;
                        case 3:
                            ox = -0.4 - tief[k]; rx = 0.2 - tief[pu];
                            oy = -0.2 + tief[k]; ry = -0.2 + tief[pu];
                            oz = -0.2 + tief[k]; rz = 0.4 + tief[pu];
                            break;
                    }

                    /* Dann wird das Kreuzprodukt gebildet, um einen
                       Vergleichsvektor zu haben.                                 */
                    xv = oy * rz - oz * ry;
                    yv = oz * rx - ox * rz;
                    zv = ox * ry - oy * rx;
                    /* Der Winkel ist nun das Skalarprodukt mit (1,-1,-1)= Lichtstrahl */
                    /* mit Vergleichsvektor (Beide nachträglich normiert )             */
                    winkel = Math.Asin((-xv + yv + zv) / (Math.Sqrt(xv * xv + yv * yv + zv * zv) * Math.Sqrt(3.0))) / Math.PI * 2.0;
                    //	  wert=(256*2.2)* (acos(winkel)/3.14159);
                    //  if(winkel<0) winkel=-winkel;
                    wert = (int)(256 - (256 * winkel));
                    // end of wert:
                    //+(rand()%100)/99.;
                    //  cout << " " << wert;
                    if (wert < 0) wert = -wert;
                    col[k] = 256 - wert;
                    //  if (col[k]>maxcol) 
                    // 	    {maxcol=col[k]; cout << "max:" << maxcol << "\n";}
                    // 	  if (col[k]<mincol) 
                    // 	    {mincol=col[k]; cout << "min:" << mincol << "\n";}
                    if (col[k] > 256 - 1) col[k] = 256 - 1;
                    if (col[k] < 1) col[k] = 1;
                }
                else col[k] = 1;
            }
            return ((int)col[0]);
        }


        /// <summary>
        /// Wendet die in mProjection enthaltenen Transformationen an. 
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public Vec3 GetTransform(double x, double y, double z)
        {
            Vec3 retVal = new Vec3();

            double a, f, re, im, xmi, ymi, zmi;

            try
            {
                if (_projection != null)
                {
                    Vec3 projPoint = _projection.Transform(new Vec3(x, y, z));
                    x = projPoint.X;
                    y = projPoint.Y;
                    z = projPoint.Z;
                }

                if (_transforms.Count > 0)
                {
                    Vec3 vec = new Vec3(x, y, z);
                    foreach (Transform3D trans in _transforms)
                    {
                        vec = trans.Transform(vec);
                    }
                    x = vec.X;
                    y = vec.Y;
                    z = vec.Z;
                }

                /* Einbeziehung des Winkels  */
                f = Math.PI / 180.0;
                // Drehung
                xmi = 0; ymi = 0; zmi = 0;
                x -= xmi; y -= ymi; z -= zmi;
                re = Math.Cos(mGlobalAngleZ * f); im = Math.Sin(mGlobalAngleZ * f);
                a = re * x - im * y;
                y = re * y + im * x;
                x = a;
                // Neigung
                re = Math.Cos(mGlobalAngleY * f); im = Math.Sin(mGlobalAngleY * f);
                a = re * z - im * x;
                x = re * x + im * z;
                z = a;
                // Kippen
                re = Math.Cos(mGlobalAngleX * f); im = Math.Sin(mGlobalAngleX * f);
                a = re * y - im * z;
                z = re * z + im * y;
                y = a;
                x += xmi; y += ymi; z += zmi;

                retVal.X = x;
                retVal.Y = y;
                retVal.Z = z;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                retVal.X = x;
                retVal.Y = y;
                retVal.Z = z;
            }

            return retVal;

        }


        /// <summary>
        /// Transform the point (x,y,z) corresponding the defined rotations.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <returns></returns>
        public Vec3 GetTransformWithoutProjection(double x, double y, double z)
        {
            Vec3 retVal = new Vec3();

            double a, f, re, im, xmi, ymi, zmi;

            try
            {

                if (_projection != null)
                {
                    Vec3 projPoint = _projection.ReverseTransform(new Vec3(x, y, z));
                    x = projPoint.X;
                    y = projPoint.Y;
                    z = projPoint.Z;
                }

                if (_transforms.Count > 0)
                {
                    Vec3 vec = new Vec3(x, y, z);
                    foreach (Transform3D trans in _transforms)
                    {
                        vec = trans.Transform(vec);
                    }
                    x = vec.X;
                    y = vec.Y;
                    z = vec.Z;
                }

                /* Einbeziehung des Winkels  */
                f = Math.PI / 180.0;
                /*xmi=(x1-x2)/2;ymi=(y1+y2)/2;zmi=(z1+z2)/2;*/
                // Drehung
                xmi = 0; ymi = 0; zmi = 0;
                x -= xmi; y -= ymi; z -= zmi;
                re = Math.Cos(mGlobalAngleZ * f); im = Math.Sin(mGlobalAngleZ * f);
                a = re * x - im * y;
                y = re * y + im * x;
                x = a;
                // Neigung
                re = Math.Cos(mGlobalAngleY * f); im = Math.Sin(mGlobalAngleY * f);
                a = re * z - im * x;
                x = re * x + im * z;
                z = a;
                // Kippen
                re = Math.Cos(mGlobalAngleX * f); im = Math.Sin(mGlobalAngleX * f);
                a = re * y - im * z;
                z = re * z + im * y;
                y = a;
                x += xmi; y += ymi; z += zmi;

                retVal.X = x;
                retVal.Y = y;
                retVal.Z = z;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                retVal.X = x;
                retVal.Y = y;
                retVal.Z = z;
            }

            return retVal;

        }


        protected double mGlobalAngleX = 0;

        protected double mGlobalAngleY = 0;

        protected double mGlobalAngleZ = 0;



        /// <summary>
        /// Test, if the given point is element of the difined set.
        /// </summary>
        public bool TestPoint(double x, double y, double z, bool inverse)
        {
            long we = -1;
            switch (old_formula)
            {
                case -2: /* Interne Formel verwenden: als Jula-Menge */
                    we = 1;
                    if (mInternFormula != null)
                    {
                        if (mInternFormula is GestaltFormula)
                        {
                            if (mInternFormula.additionalPointInfo != null)
                                mInternFormula.additionalPointInfo.Clear();
                            GestaltFormula gestaltFormula = (GestaltFormula)mInternFormula;
                            bool notinset = !gestaltFormula.GetBool(x, y, z);
                            if (notinset)
                                we = 0;
                            if (inverse) { if (we == 0) we = 1; else we = 0; }
                        }
                        else
                            we = mInternFormula.InSet(x, y, z, old_jx, old_jy, old_jz, old_jzz, old_zykl, inverse);
                    }
                    break;


                case -1: /* Interne Formel verwenden: als Mandelbrotmenge */
                    we = 1;
                    if (mInternFormula != null)
                    {
                        if (mInternFormula is GestaltFormula)
                        {
                            if (mInternFormula.additionalPointInfo != null)
                                mInternFormula.additionalPointInfo.Clear();
                            GestaltFormula gestaltFormula = (GestaltFormula)mInternFormula;
                            bool notinset = !gestaltFormula.GetBool(x, y, z);
                            if (notinset)
                                we = 0;
                            if (inverse) { if (we == 0) we = 1; else we = 0; }
                        }
                        else
                            we = mInternFormula.InSet(old_jx, old_jy, old_jz, x, y, z, old_jzz, old_zykl, inverse);
                    }
                    break;
            }

            if (we == -1)
                return false;

            if (we == 0)
                return true;

            if (we > 0 && we < old_zykl)
                return false;


            return true;
        }

        protected double old_jx = 0;
        protected double old_jy = 0;
        protected double old_jz = 0;
        protected double old_jzz = 0;
        protected int old_formula = 0;
        protected long old_zykl = 20;

        /// <summary>
        /// Die Berechnung wird gestartet.
        /// </summary>
        /// <param name="x">x-Position in Ansichtskoordinaten</param>
        /// <param name="y">y-Position in Ansichtskoordinaten</param>
        /// <param name="z">z-Position in Ansichtskoordinaten</param>
        /// <param name="zz">zz-Position in Ansichtskoordinaten (alt)</param>
        /// <param name="zykl">Benutzte Zyklen</param>
        /// <param name="wix">Globaler Winkel X</param>
        /// <param name="wiy">Globaler Winkel Y</param>
        /// <param name="wiz">Globaler Winkel Z</param>
        /// <param name="jx">Julia Parameter x</param>
        /// <param name="jy">Julia Parameter y</param>
        /// <param name="jz">Julia Parameter z</param>
        /// <param name="jzz">Julia Parameter zz</param>
        /// <param name="formula">Formel-ID</param>
        /// <param name="invers">Gibt an, ob von innen gerechnet wird.</param>
        /// <returns></returns>
        public int Rechne(double x, double y, double z, double zz, long zykl,
               double wix, double wiy, double wiz,
               double jx, double jy, double jz, double jzz, int formula, bool invers)
        {
            if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z))
                return -1;



            old_jx = jx;
            old_jy = jy;
            old_jz = jz;
            old_jzz = jzz;

            long we = 0;
            double a, f, re, im, xmi, ymi, zmi;

            mGlobalAngleX = wix;

            mGlobalAngleY = wiy;

            mGlobalAngleZ = wiz;

            try
            {
                if (_projection != null)
                {
                    Vec3 projPoint = _projection.Transform(new Vec3(x, y, z));
                    x = projPoint.X;
                    y = projPoint.Y;
                    z = projPoint.Z;
                }

                if (_transforms.Count > 0)
                {
                    Vec3 vec = new Vec3(x, y, z);
                    foreach (Transform3D trans in _transforms)
                    {
                        vec = trans.Transform(vec);
                    }
                    x = vec.X;
                    y = vec.Y;
                    z = vec.Z;
                }

                /* Einbeziehung des Winkels  */
                // Backward compatibility to old formulas with rotation with center=(0,0,0)
                if (wiz != 0 || wiy != 0.0 || wiz != 0)
                {
                    f = Math.PI / 180.0;
                    // Drehung
                    xmi = 0; ymi = 0; zmi = 0;
                    x -= xmi; y -= ymi; z -= zmi;
                    re = Math.Cos(wiz * f); im = Math.Sin(wiz * f);
                    a = re * x - im * y;
                    y = re * y + im * x;
                    x = a;
                    // Neigung
                    re = Math.Cos(wiy * f); im = Math.Sin(wiy * f);
                    a = re * z - im * x;
                    x = re * x + im * z;
                    z = a;
                    // Kippen
                    re = Math.Cos(wix * f); im = Math.Sin(wix * f);
                    a = re * y - im * z;
                    z = re * z + im * y;
                    y = a;
                    x += xmi; y += ymi; z += zmi;
                }

                // Weitere Transformationen:

                switch (formula)
                {
                    case -2: /* Interne Formel verwenden: als Jula-Menge */
                        we = 1;
                        if (mInternFormula != null)
                        {
                            if (mInternFormula is GestaltFormula)
                            {
                                if (mInternFormula.additionalPointInfo != null)
                                    mInternFormula.additionalPointInfo.Clear();
                                GestaltFormula gestaltFormula = (GestaltFormula)mInternFormula;
                                bool inset = gestaltFormula.GetBool(x, y, z);
                                if (inset)
                                    we = 0;
                                if (invers) { if (we == 0) we = 1; else we = 0; }
                            }
                            else
                                we = mInternFormula.InSet(x, y, z, jx, jy, jz, jzz, zykl, invers);
                        }
                        break;


                    case -1: /* Interne Formel verwenden: als Mandelbrotmenge */
                        we = 1;
                        if (mInternFormula != null)
                        {
                            if (mInternFormula is GestaltFormula)
                            {
                                if (mInternFormula.additionalPointInfo != null)
                                    mInternFormula.additionalPointInfo.Clear();
                                GestaltFormula gestaltFormula = (GestaltFormula)mInternFormula;
                                bool inset = gestaltFormula.GetBool(x, y, z);
                                if (inset)
                                    we = 0;
                                if (invers) { if (we == 0) we = 1; else we = 0; }
                            }
                            else
                                we = mInternFormula.InSet(jx, jy, jz, x, y, z, zz, zykl, invers);
                        }
                        break;


                   

                }
                return ((int)we);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return 0;
            }

        }


        protected double LinearSin(double angle)
        {
            int angleInt = (int)(angle - 0.5);
            angleInt = angleInt / 2;
            angleInt = angleInt / 2;
            angle = angle - angleInt;
            if (angle > 3)
            {
                angle = angle - 3;
            }
            else
                if (angle > 1)
                angle = 2 - angle;

            return angle - 1;
        }


        protected double LinearTan2(double angle1, double angle2)
        {
            double angle = 0;
            if (angle2 != 0)
                angle = angle1 / angle2;

            double sin = LinearSin(angle);
            double cos = LinearCos(angle);

            double retVal = 0;
            if (cos != 0)
                retVal = sin / cos;

            return retVal;
        }



        protected double LinearCos(double angle)
        {
            int angleInt = (int)(angle - 0.5);
            angleInt = angleInt / 2;
            angleInt = angleInt / 2;
            angle = angle - angleInt;
            angle = -angle;
            if (angle < -2)
                angle = 2 + angle;

            return angle + 1;

        }





        // Raycast part.
        public double RayCastAt(long zykl, double x, double y, double z, double zz,
        double xd, double yd, double zd, double zzd,
        double wix, double wiy, double wiz,
        double jx, double jy, double jz, double jzz, int formula, bool invers, int pixelX, int pixelY, bool use4Points)
        {

            if (zd == 0)
            {
                Console.WriteLine("Error in RayCastAt: zd==0");
                return 0;
            }

            double m = 0;
            double yn = 0, diff = 0;
            double xn = 0, zn = 0, zzn = 0, xm = 0, ym = 0, zm = 0, zzm = 0;
            double[] tief = new double[5];
            double[] xpos = new double[5];
            double[] ypos = new double[5]; // Die ungenaue Variante von tief[]
            double[] zpos = new double[5];

            double startwert = 0;
            int k = 0;

            double distance = 0.09;

            double zDistance = distance * 6.0;

            // Eventuell während der Berechnung entstehende Zusatzinfos für alle 4 Punkte.
            AdditionalPointInfo[] pinfoSet = null;

            // Use combination of all computed AdditionalPointInfo for smoother colors.
            AdditionalPointInfo additionalPointInfoCombination = new AdditionalPointInfo();
            additionalPointInfoCombination.blue = 0;
            additionalPointInfoCombination.red = 0;
            additionalPointInfoCombination.green = 0;

            var lastColor = mInternFormula.additionalPointInfo;
            AdditionalPointInfo[] lastinfoSet = new AdditionalPointInfo[5];


            if (mInternFormula != null && mInternFormula.additionalPointInfo != null)
            {
                pinfoSet = new AdditionalPointInfo[5];
            }

            k = 0;
            {
                xn = x;
                yn = y;
                zn = z + zDistance * zd; zzn = zz + zDistance * zzd;

                xpos[0] = xn;
                ypos[0] = yn;
                zpos[0] = zn;

                if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                {
                    for (m = 0; m >= -4.0; m -= 0.2)
                    {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            break;
                    }
                }
                else
                {
                    for (m = 0; m <= 4.0; m += 0.2)
                    {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
                    }
                }
                if(mInternFormula.additionalPointInfo!=null)
                if (mInternFormula.additionalPointInfo.isEmpty())
                {
                }
                else
                {
                    lastColor = mInternFormula.additionalPointInfo.Clone();
                }

                if ((m > -3.9) && (m < 3.9))
                {
                    startwert = m + 0.2; diff = 0.1;
                    while (diff >= 0.00001)
                    {
                        m = startwert - diff;
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                            startwert = m + diff;
                        else startwert = m;
                        diff /= 2.0;
                    }
                    tief[0] = m;
                }
                else
                {

                    // Fast Kopie der obigen Formeln
                    if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                    {
                        for (m = 0; m >= -8.0; m -= 0.02)
                        {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            {
                                if (!(Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (m = 0; m <= 8.0; m += 0.02)
                        {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                            {
                                if (Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                                {
                                    m -= 0.02;
                                    break;
                                }
                            }
                        }
                    }
                    if ((m > -7.9) && (m < 7.9))
                    {
                        startwert = m + 0.02; diff = 0.01;
                        while (diff >= 0.00001)
                        {
                            m = startwert - diff;
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                                startwert = m + diff;
                            else startwert = m;
                            diff /= 2.0;
                        }
                        tief[0] = m;
                    }
                    else
                    {
                        tief[0] = 0;
                    }
                }
                if (pinfoSet != null)
                {
                    pinfoSet[k] = new AdditionalPointInfo(lastColor);
                }
            }


            PixelInfo pInfo = null;
            if (pData.Points[pixelX, pixelY] == null)
            {
                pInfo = new PixelInfo();
                pData.Points[pixelX, pixelY] = pInfo;
                pInfo.Coord.X = xpos[0];

                pInfo.Coord.Y = ypos[0] + tief[0] * yd;
                pInfo.Coord.Z = zpos[0];
            }
            else
            {
                pInfo = pData.Points[pixelX, pixelY];
            }

            if (pinfoSet != null)
                pInfo.AdditionalInfo = pinfoSet[0];

            if (pinfoSet != null)
            {
                if (pInfo != null)
                {
                    pInfo.AdditionalInfo = pinfoSet[0];
                    pInfo.IsInside = !invers;
                }
            }

            return ((int)col[0]);
        }



        // Transform according to _projection and _transforms
        protected Vec3 Transform(double x, double y, double z)
        {
            if (_projection != null)
            {
                Vec3 projPoint = _projection.Transform(new Vec3(x, y, z));
                x = projPoint.X;
                y = projPoint.Y;
                z = projPoint.Z;
            }

            if (_transforms.Count > 0)
            {
                Vec3 vec = new Vec3(x, y, z);
                foreach (Transform3D trans in _transforms)
                {
                    vec = trans.Transform(vec);
                }
                x = vec.X;
                y = vec.Y;
                z = vec.Z;
            }
            return new Vec3(x, y, z);
        }



        /// <summary>
        /// Im Gegensatz zu klasischen Funktion Winkel wird hier von vorn gerechnet,
        /// die isometrische Ansicht entfällt.
        /// </summary>
        /// <param name="zykl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="zz"></param>
        /// <param name="xd"></param>
        /// <param name="yd"></param>
        /// <param name="zd"></param>
        /// <param name="zzd"></param>
        /// <param name="wix"></param>
        /// <param name="wiy"></param>
        /// <param name="wiz"></param>
        /// <param name="jx"></param>
        /// <param name="jy"></param>
        /// <param name="jz"></param>
        /// <param name="jzz"></param>
        /// <param name="formula"></param>
        /// <param name="use4Points">Hier wird unterschieden, ob nur dieser Punkt, oder auch seine Nachbarpixel 
        /// betrachtet werden.</param>
        /// <returns></returns>
        public double FixPoint(long zykl, double x, double y, double z, double zz,
        double xd, double yd, double zd, double zzd,
        double wix, double wiy, double wiz,
        double jx, double jy, double jz, double jzz, int formula, bool invers, int pixelX, int pixelY, bool use4Points)
        {
            if (zd == 0)
            {
                Console.WriteLine("Error in WinkelPerspective: zd==0");
                return 0;

            }

            double dust = 0;
            double dustCount = 0;

            double m = 0;
            double xv = 0, yv = 0, zv = 0, winkel = 0, yn = 0, diff = 0;
            double xn = 0, zn = 0, zzn = 0, xm = 0, ym = 0, zm = 0, zzm = 0;
            double ox = 0, oy = 0, oz = 0, rx = 0, ry = 0, rz = 0;
            double[] tief = new double[5];
            double[] xpos = new double[5];
            double[] ypos = new double[5]; // Die ungenaue Variante von tief[]
            double[] zpos = new double[5];

            double startwert = 0;
            int k = 0;
            double wert = 0;
            int pu = 0;

            double distance = 0.09;

            double xDistance = distance * 6.0;
            double zDistance = distance * 6.0;

            // Eventuell während der Berechnung entstehende Zusatzinfos für alle 4 Punkte.
            AdditionalPointInfo[] pinfoSet = null;

            // Use combination of all computed AdditionalPointInfo for smoother colors.
            AdditionalPointInfo additionalPointInfoCombination = new AdditionalPointInfo();
            additionalPointInfoCombination.blue = 0;
            additionalPointInfoCombination.red = 0;
            additionalPointInfoCombination.green = 0;


            if (mInternFormula != null && mInternFormula.additionalPointInfo != null)
            {
                pinfoSet = new AdditionalPointInfo[5];
            }

            for (k = 4; k >= 0; k--)
            {
                switch (k)
                {
                    case 2:     /* oben */
                        xn = x;
                        yn = y;
                        zn = z - zDistance * zd; zzn = zz + zDistance * zzd;
                        break;
                    case 1:     /* rechts  */
                        xn = x + xDistance * xd;
                        yn = y;
                        zn = z; zzn = zz;
                        break;
                    case 0:     /* unten */
                        xn = x;
                        yn = y;
                        zn = z + zDistance * zd; zzn = zz + zDistance * zzd;
                        break;
                    case 3:     /* links  */
                        xn = x - xDistance * xd;
                        yn = y;
                        zn = z; zzn = zz;
                        break;
                    case 4:     /* mitte  */
                        xn = x;
                        yn = y;
                        zn = z;
                        zzn = zz;
                        break;
                }

                xpos[k] = xn;
                ypos[k] = yn;
                zpos[k] = zn;

                if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                {
                    for (m = 0; m >= -4.0; m -= 0.2)
                    {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            break;
                    }
                }
                else
                {
                    for (m = 0; m <= 4.0; m += 0.2)
                    {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
                    }
                }
                if ((m > -3.9) && (m < 3.9))
                {
                    startwert = m + 0.2; diff = 0.1;
                    while (diff >= 0.00001)
                    {
                        m = startwert - diff;
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                            startwert = m + diff;
                        else startwert = m;
                        diff /= 2.0;
                    }
                    tief[k] = m;
                }
                else
                {

                    // Fast Kopie der obigen Formeln
                    if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                    {
                        for (m = 0; m >= -8.0; m -= 0.02)
                        {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            {
                                //dustCount += 1;
                                if (!(Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                                {
                                    break;
                                }
                                else
                                {
                                    //   break;
                                    //dust += 1;

                                    // Staubzähler erhöhen
                                }
                            }
                        }
                    }
                    else
                    {
                        for (m = 0; m <= 8.0; m += 0.02)
                        {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                            {
                                //dustCount += 1;
                                if (Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)
                                {
                                    m -= 0.02;
                                    break;
                                }
                                else
                                {
                                   // dust += 1;
                                    // Staubzähler erhöhen
                                    //m -= 0.02;
                                    //break;
                                }
                            }
                        }
                    }
                    //dustCount += 1;
                    if ((m > -7.9) && (m < 7.9))
                    {
                        startwert = m + 0.02; diff = 0.01;
                        while (diff >= 0.00001)
                        {
                            m = startwert - diff;
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                                startwert = m + diff;
                            else startwert = m;
                            diff /= 2.0;
                        }
                        tief[k] = m;
                    }
                    else
                    {
                        //dust += 1;
                        tief[k] = 20;
                    }

                    // Ende: Fast Kopie der obigen Formeln




                }

                if (pinfoSet != null)
                {
                    pinfoSet[k] = new AdditionalPointInfo(mInternFormula.additionalPointInfo);
                }
            }

            // Die Normalen der 4 Randpunkte
            Vec3[] normals = new Vec3[4];

            for (k = 0; k < 4; k++)
            {
                dustCount += 1;
                if (tief[k] > 19)
                    dust++;
                pu = k + 1; if (k == 3) pu = 0;
                /* Die drei Punkte entsprechen tief[4] tief[k] und tief[pu]   */
                /* Zuerst wird tief abgezogen                                 */
                if ((tief[k] < 19) && (tief[pu] < 19) && tief[4] < 19)
                {


                    Vec3 o1 = GetTransform(xpos[k], ypos[k] + tief[k] * yd, zpos[k]);
                    Vec3 o2 = GetTransform(xpos[4], ypos[4] + tief[4] * yd, zpos[4]);
                    Vec3 r1 = GetTransform(xpos[pu], ypos[pu] + tief[pu] * yd, zpos[pu]);
                    Vec3 r2 = GetTransform(xpos[4], ypos[4] + tief[4] * yd, zpos[4]);

                    /*
                    ox = xpos[k] - xpos[4];
                    oy = ypos[k] + tief[k] * yd - ypos[4] - tief[4] * yd;
                    oz = zpos[k] - zpos[4];

                    rx = xpos[pu] - xpos[4];
                    ry = ypos[pu] + tief[pu] * yd - ypos[4] - tief[4] * yd;
                    rz = zpos[pu] - zpos[4];
                     */
                    ox = o1.X - o2.X; oy = o1.Y - o2.Y; oz = o1.Z - o2.Z;
                    rx = r1.X - r2.X; ry = r1.Y - r2.Y; rz = r1.Z - r2.Z;


                    /*
                    ox = xpos[k] - xpos[4];
                    oy = ypos[k] + tief[k] * yd - ypos[4] - tief[4] * yd;
                    oz = zpos[k] - zpos[4];

                    rx = xpos[pu] - xpos[4];
                    ry = ypos[pu] + tief[pu] * yd - ypos[4] - tief[4] * yd;
                    rz = zpos[pu] - zpos[4];
                     */

                    // Apply Transformation
                    /*        
                 Vec3 ovec = GetTransform(ox, oy, oz);
                 Vec3 rvec = GetTransform(rx, ry, rz);

                 ox = ovec.X; oy = ovec.Y; oz = ovec.Z;
                 rx = rvec.X; ry = rvec.Y; rz = rvec.Z;
                    */


                    /* Dann wird das Kreuzprodukt gebildet, um einen
                       Vergleichsvektor zu haben.                                 */
                    xv = oy * rz - oz * ry;
                    yv = oz * rx - ox * rz;
                    zv = ox * ry - oy * rx;

                    normals[k] = new Vec3(xv, yv, zv);

                    /* Der Winkel ist nun das Skalarprodukt mit (0,-1,0)= Lichtstrahl */
                    /* mit Vergleichsvektor (Beide nachträglich normiert )             */
                    winkel = Math.Acos((yv) / (Math.Sqrt(xv * xv + yv * yv + zv * zv)));
                    winkel = 1 - winkel;

                    if (winkel < 0)
                        winkel = 0;
                    if (winkel > 1)
                        winkel = 1;

                    wert = (256 - (256 * winkel));
                    if (wert < 0) wert = 0;
                    col[k] = 256 - wert;
                    if (col[k] > 256 - 1) col[k] = 256 - 1;
                    if (col[k] < 1) col[k] = 1;


                    // Pixelposition im ausgegebenen Bild
                    int indexX = 0, indexY = 0;
                    if (use4Points)
                    {

                        switch (k)
                        {
                            case 0:
                                indexX = pixelX + 1;
                                indexY = pixelY;
                                break;

                            case 1:
                                indexX = pixelX + 1;
                                indexY = pixelY + 1;
                                break;

                            case 2:
                                indexX = pixelX;
                                indexY = pixelY + 1;
                                break;

                            case 3:
                                indexX = pixelX;
                                indexY = pixelY;
                                break;

                        }

                        if (k == 3)
                        {
                            if (indexX < pData.Width && indexY < pData.Height)
                            {
                                if (pData.Points[indexX, indexY] == null)
                                {
                                    PixelInfo pInfo = new PixelInfo();
                                    pInfo.Coord.X = xpos[k];
                                    pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                                    pInfo.Coord.Z = zpos[k];
                                    pInfo.Normal = normals[k];
                                    pData.Points[indexX, indexY] = pInfo;
                                    if (pinfoSet != null)
                                        pInfo.AdditionalInfo = pinfoSet[k];
                                }
                            }
                        }
                    }
                    else
                    {
                        PixelInfo pInfo = null;
                        if (pData.Points[pixelX, pixelY] == null)
                        {
                            // TODO: Später Querschnitt aus allen Einzelwinkeln bestimmen
                            pInfo = new PixelInfo();
                            pData.Points[pixelX, pixelY] = pInfo;
                            pInfo.Coord.X = xpos[k];
                            pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                            pInfo.Coord.Z = zpos[k];
                        }
                        else
                        {
                            pInfo = pData.Points[pixelX, pixelY];
                        }
                        pInfo.Normal = normals[k];
                        if (pinfoSet != null)
                            pInfo.AdditionalInfo = pinfoSet[k];
                        // TODO: Auch die Normalen übertragen
                    }

                }
                else
                {

                    int indexX = 0, indexY = 0;
                    if (pixelX >= 0 && pixelY >= 0)
                    {
                        switch (k)
                        {
                            case 0:
                                indexX = pixelX + 1;
                                indexY = pixelY;
                                break;

                            case 1:
                                indexX = pixelX + 1;
                                indexY = pixelY + 1;
                                break;

                            case 2:
                                indexX = pixelX;
                                indexY = pixelY + 1;
                                break;

                            case 3:
                                indexX = pixelX;
                                indexY = pixelY;
                                break;

                        }
                        if (indexX < pData.Width && indexY < pData.Height)
                            pData.Points[indexX, indexY] = null;
                    }
                    col[k] = 0;
                }
            }

            /*
              // Oberflächenkrümmung bestimmen
              double derivation = 0;
              // Zentrum der 4 Randpunkte mit dem Mittelpunkt vergleichen.
              double ycenter = 0;

              if (tief[4] < 19) {
                int pointsCount = 0;
                double ymax = double.MinValue;
                double ymin = double.MaxValue;
                double currentY = 0;
                for (k = 0; k < 4; k++) {
                  if (tief[k] < 19) {
                    currentY = ypos[k] + tief[k] * yd;
                    ycenter += currentY;
                    if (currentY > ymax)
                      ymax = currentY;
                    if (currentY < ymin)
                      ymin = currentY;
                    pointsCount++;
                  }
                }
                if (pointsCount > 0)
                  ycenter = ycenter / ((double)pointsCount);

                
                double maxdiff = Math.Max((yd + xd + zd) / 2.0, ymax - ymin);

                  // derivation soll eigentlich die lokale Erhöhung anzeigen,
                  // wird aber zur Zeit nicht mehr in PictureArt verwendet
                derivation = 2.0 * (ypos[4] - ycenter) / (maxdiff);

                // Dasselbe über die Normalen:
                double winkel1 = 0;
                double winkel2 = 0;
                if (normals[0] != null && normals[2] != null)
                  winkel1 = Math.Acos((normals[0].X * normals[2].X + normals[0].Y * normals[2].Y + normals[0].Z * normals[2].Z) /
                      (Math.Sqrt(normals[0].X * normals[0].X + normals[0].Y * normals[0].Y + normals[0].Z * normals[0].Z) *
                      Math.Sqrt(normals[2].X * normals[2].X + normals[2].Y * normals[2].Y + normals[2].Z * normals[2].Z)));
                if (normals[1] != null && normals[3] != null)
                  winkel2 = Math.Acos((normals[1].X * normals[3].X + normals[1].Y * normals[3].Y + normals[1].Z * normals[3].Z) /
                      (Math.Sqrt(normals[1].X * normals[1].X + normals[1].Y * normals[1].Y + normals[1].Z * normals[1].Z) *
                      Math.Sqrt(normals[3].X * normals[3].X + normals[3].Y * normals[3].Y + normals[3].Z * normals[3].Z)));

                if (derivation < 0)
                  derivation = -Math.Max(winkel, winkel2);
                else
                  derivation = Math.Max(winkel, winkel2);

                if (pixelX >= 0 && pixelY >= 0) {
                  if (pixelX < pData.Width && pixelY < pData.Height)
                    if (pData.Points[pixelX, pixelY] != null)
                      pData.Points[pixelX, pixelY].derivation = derivation;
                  if (pixelX + 1 < pData.Width && pixelY < pData.Height)
                    if (pData.Points[pixelX + 1, pixelY] != null)
                      pData.Points[pixelX + 1, pixelY].derivation = derivation;
                  if (pixelX < pData.Width && pixelY + 1 < pData.Height)
                    if (pData.Points[pixelX, pixelY + 1] != null)
                      pData.Points[pixelX, pixelY + 1].derivation = derivation;
                  if (pixelX + 1 < pData.Width && pixelY + 1 < pData.Height)
                    if (pData.Points[pixelX + 1, pixelY + 1] != null)
                      pData.Points[pixelX + 1, pixelY + 1].derivation = derivation;
                }

               
              } */

            // Farben feiner machen:
            if (pinfoSet != null)
            {
                PixelInfo pInfo = pData.Points[pixelX, pixelY];
                if (pInfo != null)
                {
                    pInfo.AdditionalInfo = pinfoSet[0];
                    for (int i = 1; i <= 4; i++)
                    {
                        if (pInfo.AdditionalInfo != null && pinfoSet[i] != null)
                        {
                            pInfo.AdditionalInfo.red += pinfoSet[i].red;
                            pInfo.AdditionalInfo.green += pinfoSet[i].green;
                            pInfo.AdditionalInfo.blue += pinfoSet[i].blue;

                        }
                    }
                    pInfo.IsInside = !invers;
                }
            }

            if (pData.Points[pixelX, pixelY] == null)
            {
                PixelInfo pInfo = new PixelInfo();
                pInfo.Coord.X = xpos[k];
                pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                pInfo.Coord.Z = zpos[k];
                // pInfo.Normal = normals[k];
                pData.Points[pixelX, pixelY] = pInfo;
                if (pinfoSet != null)
                    pInfo.AdditionalInfo = pinfoSet[k];
            }

            PixelInfo pInfo2 = pData.Points[pixelX, pixelY];
            if (pInfo2 != null)
            {
                pInfo2.dustlevel = dustCount > 0 ? dust / dustCount : 0;
            }

            //           if (pData.Points[pixelX, pixelY]!=null)
            //        pData.Points[pixelX, pixelY].dustlevel= dustCount > 0 ? dust / dustCount : 0;


            return ((int)col[0]);
        }



    }
}