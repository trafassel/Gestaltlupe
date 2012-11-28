using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;
using Fractrace.Geometry;
using Fractrace.DataTypes;
using Fractrace.TomoGeometry;

namespace Fractrace {


/// <summary>
/// Fest implementierte Formeln.
/// </summary>
    public class Formulas {

        /* global variables */
        public double[] col = new double[256];

        // Parameter, die durch die Eingabemaske definiert werden
        private static double gr = 10;
        //private double zn=0, xn=0, yn=0, zzn=0; 
        /* Hier stehen die Formeln, die den
           verschiedenen Zahlendarstellungen entsprechen */

        PictureData pData = null;


        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="pData"></param>
        public Formulas(PictureData pData) {
            this.pData = pData;
        }




        /// <summary>
        /// Initialisierung
        /// </summary>
        public Formulas() {

        }

        /// <summary>
        /// Zusätzliche Projektion.
        /// </summary>
        public Projection Projection {
            get {
                return mProjection;
            }
            set {
                mProjection = value;
            }

        }
        protected Projection mProjection = null;


        protected List<Transform3D> mTransforms = new List<Transform3D>();

        /// <summary>
        /// Zugriff auf die Liste der Zusatztransformationen, z.B. weitere Stauchungen
        /// oder Drehungen.
        /// </summary>
        public List<Transform3D> Transforms {
            get {
                return mTransforms;
            }
        }


        protected TomoFormula mInternFormula = null;

        public TomoFormula InternFormula {
            get {
                return mInternFormula;
            }

            set {
                mInternFormula = value;
            }
        }

    
        long Komp(double ar, double ai, double br, double bi, long zkl) {
            // formula:  z=z*z-d
            double aar, aai;
            double dist;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; tw = 0L;
            for (n = 1; n < zkl; n++) {
                ai = ar * ai; ai += ai + bi;
                dist = (aar + aai);
                //ai/=dist;
                ar = aar - aai + br;
                ar /= dist;
                aar = ar * ar; aai = ai * ai;
                if ((aar + aai) > gr) { tw = n; break; }
            }
            return (tw);
        }

        long Dra(double ar, double ai, double br, double bi, long zkl) {
            double aar, aai, c, d;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; tw = 0L;
            for (n = 1; n < zkl; n++) {
                c = ar - aar + aai;
                d = -ar * ai; d += d + ai;
                ar = br * c - bi * d;
                ai = bi * c + br * d;
                aar = ar * ar; aai = ai * ai;
                if ((aar + aai) > gr) { tw = n; break; }
            }
            return (tw);
        }

        long H1(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {
            double aar, aai, aaj, aak;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak; tw = 0L;
            for (n = 1; n < zkl; n++) {
                ai = 2.0 * ar * ai + bi;
                aj = 2.0 * ar * aj + bj;
                ak = 2.0 * ar * ak + bk;
                ar = aar - aai - aaj - aak + br;
                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                if ((aar + aai + aaj + aak) > gr) { tw = n; break; }
            }
            return (tw);
        }


        long Wuerfel(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {
            if (br > -1 && br < 1 && bi > -1 && bi < 1 && bj > -1 && bj < 1) {

                return 0;
            }
            return 1;
        }


        //static int minCycle = 100;


      /// <summary>
      /// Quadratische Version (ohne Winkel)
      /// </summary>
      /// <param name="ar"></param>
      /// <param name="ai"></param>
      /// <param name="aj"></param>
      /// <param name="ak"></param>
      /// <param name="br"></param>
      /// <param name="bi"></param>
      /// <param name="bj"></param>
      /// <param name="bk"></param>
      /// <param name="zkl"></param>
      /// <param name="invers"></param>
      /// <returns></returns>
        long Mandelbulb3DPow2(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl, bool invers) {
          
      

          // Umbenennen in Mandelbulb3D Pow2 (sollte eigentlich die Erweiterung des Mandelbulb sein). 
double xx, yy, zz;
          long tw;
          int n;
          ai = 0; aj = 0; ak = 0;

          double x=ai,y=aj,z=ak;

          xx = x * x; yy = y * y; zz = z * z; 
          tw = 0L;
          double r = Math.Sqrt(xx + yy + zz);

          for (n = 1; n < zkl; n++) {
            double r_xy=Math.Sqrt(xx+yy);
            double a = 1;
            
            if(r_xy!=0.0)
             a= 1 - zz / r_xy;

            y=2 *x*y*a+br;
            x = (xx - yy) * a+bi;
            z = 2 * z * r_xy+bj;

            xx = x * x; yy = y * y; zz = z * z;// aak = ak * ak;
            r = Math.Sqrt(xx + yy + zz);
            


            if (r > gr) { 
              tw = n; break; 
            }

          }

          if (invers) {
            if (tw == 0)
              tw = 1;
            else
              tw = 0;
          }
          return (tw);

        }


        /// <summary>
        /// Quadratische Version (ohne Winkel)
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="ai"></param>
        /// <param name="aj"></param>
        /// <param name="ak"></param>
        /// <param name="br"></param>
        /// <param name="bi"></param>
        /// <param name="bj"></param>
        /// <param name="bk"></param>
        /// <param name="zkl"></param>
        /// <param name="invers"></param>
        /// <returns></returns>
        long Mandelbulb3DPow8(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl, bool invers) {



          // Umbenennen in Mandelbulb3D Pow2 (sollte eigentlich die Erweiterung des Mandelbulb sein). 
          double xx, yy, zz;
          long tw;
          int n;
          ai = 0; aj = 0; ak = 0;

          double x = ai, y = aj, z = ak;

          xx = x * x; yy = y * y; zz = z * z;
          tw = 0L;
          double r = Math.Sqrt(xx + yy + zz);

          for (n = 1; n < zkl; n++) {
            double r_xy = Math.Sqrt(xx + yy);
            double a = 1;

            if (r_xy != 0.0)
              a = 1 - zz / r_xy;

            y = 2 * x * y * a + br;
            x = (xx - yy) * a + bi;
            z = 2 * z * r_xy + bj;

            xx = x * x; yy = y * y; zz = z * z;// aak = ak * ak;
            r = Math.Sqrt(xx + yy + zz);



            if (r > gr) {
              tw = n; break;
            }

          }

          if (invers) {
            if (tw == 0)
              tw = 1;
            else
              tw = 0;
          }
          return (tw);

        }


      
      /// <summary>
      /// Benutzung der Vektorrotation.
      /// </summary>
      /// <param name="ar"></param>
      /// <param name="ai"></param>
      /// <param name="aj"></param>
      /// <param name="ak"></param>
      /// <param name="br"></param>
      /// <param name="bi"></param>
      /// <param name="bj"></param>
      /// <param name="bk"></param>
      /// <param name="zkl"></param>
      /// <param name="invers"></param>
      /// <returns></returns>
      long H7(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl, bool invers) {

          double xx, yy, zz;
          long tw;
          int n;
          ai = 0; aj = 0; ak = 0;

          double x = 1, y = 0, z = 0;

          xx = x * x; yy = y * y; zz = z * z;
          tw = 0;
          double r = Math.Sqrt(xx + yy + zz);
          VecRotation vecRot = new VecRotation();

          x = 1; // Um den Startwinkel eindeutig zu definieren.
          for (n = 1; n < zkl; n++) {
            double r_xy = Math.Sqrt(xx + yy);
            /*
            vecRot.x = x;
            vecRot.y = y;
            vecRot.z = z;
            vecRot.angle = x;
             * */

            double theta = Math.Atan2(Math.Sqrt(xx + yy), z);
            double phi = Math.Atan2(y, x);

            vecRot.x = y;
            vecRot.y = x;
            vecRot.z = z;
            vecRot.angle = theta;
         //   vecRot.angle = 0.03;
            vecRot.x = x;
            vecRot.y = z;
            vecRot.z = y;
            vecRot.angle = phi;

            /*
            vecRot.x = 0.4;
            vecRot.y = 0.2;
            vecRot.z = 0.8;
            vecRot.angle = phi;
            */
            y += bj;
            x += bi;
            z += br;
            Vec3 pos=new Vec3(x,y,z);
            Vec3 newPos= vecRot.getTransform(pos);

            x = newPos.X;
            y = newPos.Y;
            z = newPos.Z;

            xx = x * x; yy = y * y; zz = z * z;// aak = ak * ak;
            r = Math.Sqrt(xx + yy + zz);

            x *= r;
            y *= r;
            z *= r;
            if (r > gr) {
              tw = n; break;
            }

          }

          if (invers) {
            if (tw == 0)
              tw = 1;
            else
              tw = 0;
          }
          return (tw);

        }


        long Mandelbulb3D(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl, bool invers) {
            double aar, aai, aaj;
            long tw;
            int n;
            int pow = 8; // n=8 entspricht dem Mandelbulb
            double theta, phi;

            double r_n = 0;
            aar = ar * ar; aai = ai * ai; aaj = aj * aj; //aak = ak * ak; 
            tw = 0L;
            double r = Math.Sqrt(aar + aai + aaj);

            for (n = 1; n < zkl; n++) {

                theta = Math.Atan2(Math.Sqrt(aar + aai), aj);
                phi = Math.Atan2(ai, ar);
                r_n = Math.Pow(r, pow);
                ar = r_n * Math.Sin(theta * pow) * Math.Cos(phi * pow);
                ai = r_n * Math.Sin(theta * pow) * Math.Sin(phi * pow);
                aj = r_n * Math.Cos(theta * pow);

                ar += br;
                ai += bi;
                aj += bj;

                aar = ar * ar; aai = ai * ai; aaj = aj * aj;// aak = ak * ak;
                r = Math.Sqrt(aar + aai + aaj);

                if (r > gr) { tw = n; break; }

            }

            if (invers) {
                if (tw == 0)
                    tw = 1;
                else
                    tw = 0;
            }
            return (tw);
        }

        /// <summary>
        /// Abgeänderte Form vom Mandelbulb
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="ai"></param>
        /// <param name="aj"></param>
        /// <param name="ak"></param>
        /// <param name="br"></param>
        /// <param name="bi"></param>
        /// <param name="bj"></param>
        /// <param name="bk"></param>
        /// <param name="zkl"></param>
        /// <param name="invers"></param>
        /// <returns></returns>
        long H6(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl, bool invers) {
            double aar, aai, aaj, aak;
            long tw;
            int n;
            int pow = 8; // n=8 entspricht dem Mandelbulb
            double theta, phi;
            double theta1;

            double r_n = 0;
            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
            tw = 0L;
            double r = aar + aai + aaj + aak;
            double sintheta = 0;
            for (n = 1; n < zkl; n++) {

                theta = Math.Atan2(aar + aai, aj);
                theta1 = Math.Atan2(aar + aai, ak);
                phi = Math.Atan2(ai, ar);
                r_n = Math.Pow(r, pow);
                sintheta = Math.Sin(theta * pow) + Math.Sin(theta1 * pow);
                ar = r_n * (sintheta * Math.Cos(phi * pow));
                ai = r_n * (sintheta * Math.Sin(phi * pow));
                aj = r_n * (Math.Cos(theta * pow));
                ak = r_n * (Math.Cos(theta1 * pow));

                ar += br;
                ai += bi;
                aj += bj;
                ak += bk;

                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                r = Math.Sqrt(aar + aai + aaj + aak);

                if (r > gr) { tw = n; break; }

            }

            if (invers) {
                if (tw == 0)
                    tw = 1;
                else
                    tw = 0;
            }
            return (tw);
        }


        long H2(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {
            double aar, aai, aaj, aak;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak; tw = 0L;
            for (n = 1; n < zkl; n++) {
                ar = aar - aai - aaj - aak - br;
                ai = 2 * ai * ar - bi;
                aj = 2 * (ar * aj) - bj;
                if (aj > 1) aj = 1;
                if (aj < -1) aj = -1;
                ak = 2 * (ar * ak) - bk;
                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                if ((aar + aai + aaj + aak) > gr) { tw = n; break; }
            }
            return (tw);
        }

        long H3(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {


            double aar, aai, aaj, aak;
            double d;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak; tw = 0L;
            d = aar + aai + aaj + aak;
            for (n = 1; n < zkl; n++) {
                ak = 2 * ak * ar * aj + d * bk;
                ai = 2 * ai * ar * aj + d * bi;
                aj = aaj - (aai + aak + aar) / 2.0 + d * bj;
                ar = aar - (aai + aak + aaj) / 2.0 + d * br;
                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                d = aar + aai + aaj + aak;
                if ((d) > gr) { tw = n; break; }
            }
            return (tw);
        }

        long H4(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {


            double aar, aai, aaj, aak;
            double d;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak; tw = 0L;
            d = Math.Sqrt(aar + aai + aaj + aak);
            // d = Math.Sqrt(d);
            // d = Math.Sqrt(d);
            for (n = 1; n < zkl; n++) {
                ak = 2.0 * ak * ar * aj + bk;
                ai = 2.0 * ai * ar * aj + bi;
                //  ai = 2.0 * ar * ai + d * bi;
                aj = aaj - (aai + aak + aar) / 2.0 + bj;
                ar = aar - (aai + aak + aaj) / 2.0 + br;
                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                d = Math.Sqrt(aar + aai + aaj + aak);
                //   d = Math.Sqrt(d);
                //  d = Math.Sqrt(d);
                if ((d) > gr) { tw = n; break; }
            }
            return (tw);
        }


        long Qu(double ar, double ai, double aj, double ak, double br, double bi, double bj, double bk, long zkl) {
            double aar, aai, aaj, aak;
            long tw;
            int n;

            aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak; tw = 0L;
            for (n = 1; n < zkl; n++) {
                ai = 2 * ai * ar + bi;
                ak = 2 * (ak * ar - ai * aj) + bk;
                aj = aaj - aak + aar - aai + bj;
                ar = aar - aai - aak - aaj + br;
                aar = ar * ar; aai = ai * ai; aaj = aj * aj; aak = ak * ak;
                if ((aar + aai + aaj + aak) > gr) { tw = n; break; }
            }
            return (tw);
        }


        /* Berechnung des Winkels des Lotes der Oberfläche
           mit Lichtquelle  (Betrachter selbst ) */


        //double zn = 0;




        public double Winkel(long zykl, double x, double y, double z, double zz,
               double xd, double yd, double zd, double zzd,
               double wix, double wiy, double wiz,
               double jx, double jy, double jz, double jzz, int formula, bool perspective, bool invers) {

            if (perspective) {
                return 0;
                /*WinkelPerspective(zykl, x, y, z, zz,
                xd, yd, zd, zzd,
                wix, wiy, wiz,
                jx, jy, jz, jzz, formula, invers);*/

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
            for (k = 4; k >= 0; k--) {
                switch (k) {
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
                if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                    for (m = 0; m >= -4.0; m -= 0.2) {
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            break;
                    }
                } else {
                    for (m = 0; m <= 4.0; m += 0.2) {
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
                    }
                }
                if ((m > -3.9) && (m < 3.9)) {
                    startwert = m + 0.2; diff = 0.1;
                    while (diff >= 0.0001) {
                        m = startwert - diff;
                        zm = zn + m * zd; xm = xn - m * xd;
                        ym = yn + m * yd; zzm = zzn + m * zzd;
                        if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                            startwert = m + diff;
                        else startwert = m;
                        diff /= 2.0;
                    }
                    if (k == 4) {
                        n = m;
                    }
                    tief[k] = m;
                } else tief[k] = 10;
            }
            tief[4] = 0;
            for (k = 0; k < 4; k++) {
                pu = k + 1; if (k == 3) pu = 0;
                /* Die drei Punkte entsprechen tief[4] tief[k] und tief[pu]   */
                /* Zuerst wird tief abgezogen                                 */
                if ((tief[k] < 9) && (tief[pu] < 9) && tief[4] < 9) {
                    switch (k) {
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
                } else col[k] = 1;
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
        public Vec3 GetTransform(double x, double y, double z) {
          Vec3 retVal=new Vec3();
            
            double a, f, re, im, xmi, ymi, zmi;

            try {
              if (mProjection != null) {
                Vec3 projPoint = mProjection.Transform(new Vec3(x, y, z));
                x = projPoint.X;
                y = projPoint.Y;
                z = projPoint.Z;
              }

              if (mTransforms.Count > 0) {
                Vec3 vec = new Vec3(x, y, z);
                foreach (Transform3D trans in mTransforms) {
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
            } catch (Exception ex) {
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
        public Vec3 GetTransformWithoutProjection(double x, double y, double z) {
            Vec3 retVal = new Vec3();

            double a, f, re, im, xmi, ymi, zmi;

            try {

              if (mProjection != null) {
                Vec3 projPoint = mProjection.ReverseTransform(new Vec3(x, y, z));
                x = projPoint.X;
                y = projPoint.Y;
                z = projPoint.Z;
              }

                if (mTransforms.Count > 0) {
                    Vec3 vec = new Vec3(x, y, z);
                    foreach (Transform3D trans in mTransforms) {
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
            } catch (Exception ex) {
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
        /// Don't work with the internal formulas.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool TestPoint(double x, double y, double z,bool inverse) {
            long we=-1;
            switch (old_formula) {
                case -2: /* Interne Formel verwenden: als Jula-Menge */
                    we = 1;
                    if (mInternFormula != null)
                        we = mInternFormula.InSet(x, y, z, old_jx, old_jy, old_jz, old_jzz, old_zykl, inverse);
                    break;


                case -1: /* Interne Formel verwenden: als Mandelbrotmenge */
                    we = 1;
                    if (mInternFormula != null)
                        we = mInternFormula.InSet(old_jx, old_jy, old_jz, x, y, z, old_jzz, old_zykl, inverse);
                    break;
            }

            if (we == -1)
                return false;

            if (we == 0)
                return true;

            if (we > 0 && we<old_zykl)
                return false;


            return true;
        }

        protected double old_jx = 0;
        protected double old_jy = 0;
        protected double old_jz = 0;
        protected double old_jzz = 0;
        protected int old_formula = 0;
        protected long old_zykl=20;

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
               double jx, double jy, double jz, double jzz, int formula, bool invers) {

                   old_jx = jx;
                   old_jy = jy;
                   old_jz = jz;
                   old_jzz = jzz;
                   old_formula = formula;

            long we = 0;
            double a, f, re, im, xmi, ymi, zmi;

            mGlobalAngleX = wix;

            mGlobalAngleY = wiy;
          
            mGlobalAngleZ = wiz;

            try {
              if (mProjection != null) {
                Vec3 projPoint = mProjection.Transform(new Vec3(x, y, z));
                x = projPoint.X;
                y = projPoint.Y;
                z = projPoint.Z;
              }

              if (mTransforms.Count > 0) {
                Vec3 vec = new Vec3(x, y, z);
                foreach (Transform3D trans in mTransforms) {
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

              // Weitere Transformationen:


              /*
            if (mProjection != null) {
                Vec3 projPoint = mProjection.Transform(new Vec3(x, y, z));
                x = projPoint.X;
                y = projPoint.Y;
                z = projPoint.Z;
            }*/

              switch (formula) {
                case -2: /* Interne Formel verwenden: als Jula-Menge */
                  we = 1;
                  if (mInternFormula != null)
                    we = mInternFormula.InSet(x, y, z, jx, jy, jz, jzz, zykl, invers);
                  break;


                case -1: /* Interne Formel verwenden: als Mandelbrotmenge */
                  we = 1;
                  if (mInternFormula != null)
                    we = mInternFormula.InSet(jx, jy, jz, x, y, z, zz, zykl, invers);
                  break;


                case 0:   /* Apfel k  */
                  //we= Komp(z,zz,x,y,zykl);    
                  we = 1;
                  if ((x * x + y * y + z * z) < zykl / 100.0)
                    we = 0L;
                  break;

                case 1:  /* Julia k   */
                  we = Komp(x, y, z, zz, zykl);
                  break;

                case 2:  /* Misch_a k */
                  we = Komp(x, z, y, zz, zykl);
                  break;

                case 3:  /* Misch_b k */
                  we = Komp(zz, y, z, x, zykl);
                  break;

                case 4:  /* Drachen_a  */
                  we = Dra(z, zz, x, y, zykl);
                  break;

                case 5:  /* Drachen_b  */
                  we = Dra(x, y, z, zz, zykl);
                  break;

                case 6:  /* Drachen_c   */
                  we = Dra(x, z, y, zz, zykl);
                  break;

                case 7:  /* Drachen_d     */
                  we = Dra(zz, y, z, x, zykl);
                  break;

                case 8:   /* Julia Qu    */
                  we = Qu(x, y, z, zz, jx, jy, jz, jzz, zykl);
                  break;

                case 9:   /* Misch_a Qu  */
                  we = Qu(x, y, z, zz, y, z, zz, x, zykl);
                  break;

                case 10:  /* Misch_b Qu   */
                  we = Qu(y, zz, z, x, x, y, z, zz, zykl);
                  break;

                case 11:  /* Misch_c Qu    */
                  we = Qu(x, y, z, zz, y, x, z, zz, zykl);
                  break;

                case 12:  /* Apfel_a H1  */
                  we = H1(jx, jy, jz, jzz, x, y, z, zz, zykl);
                  break;

                case 13:  /* Apfel_b H1  */
                  we = H1(zz, x, y, z, jzz, jx, jy, jz, zykl);
                  break;

                case 14:  /* Julia_a H1 */
                  we = H1(x, y, z, zz, jx, jy, jz, jzz, zykl);
                  break;

                case 15:  /* Julia_b H1 */
                  we = H1(zz, x, y, z, jzz, jx, jy, jz, zykl);
                  break;

                case 16:  /* Apfel_a H2 */
                  we = H2(jx, jy, jz, jzz, x, y, z, zz, zykl);
                  break;

                case 17:   /* Apfel_b H2 */
                  we = H2(jzz, jx, jy, jz, zz, x, y, z, zykl);
                  break;

                case 18:  /* Julia_a H2  */
                  we = H2(x, y, z, zz, jx, jy, jz, jzz, zykl);
                  break;

                case 19:  /* Julia_b H2  */
                  we = H2(zz, x, y, z, jzz, jx, jy, jz, zykl);
                  break;

                case 20:  /* Apfel H3    */
                  we = H3(jx, jy, jz, jzz, x, y, z, zz, zykl);
                  break;

                case 21:  /* Julia H3 (Ides Fraktal)   */
                  we = H3(x, y, z, zz, jx, jy, jz, jzz, zykl);
                  break;

                case 22:  /* Misch_a H3   */
                  we = H3(x, y, z, zz, y, z, zz, x, zykl);
                  break;

                case 23:  /* Misch_b H3   */
                  we = H3(y, zz, z, x, x, y, z, zz, zykl);
                  break;

                case 24:   /* Mandelbulb3D  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Mandelbulb3D(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);

                  break;

                case 25:   /* Würfel  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Wuerfel(jx, jy, jz, jzz, x, y, z, zz, zykl);

                  break;

                case 26:  /* Julia H3 (Ides Fraktal)   */
                  we = H3(x, y, zz, z, jx, jy, jz, jzz, zykl);
                  break;

                case 27:  /* Julia H4 (Ides Fraktal, verändert)   */
                  we = H4(x, y, z, zz, jx, jy, jz, jzz, zykl);
                  break;

                case 28:  /* Mandelbulb 3D Julia */
                  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl, invers);
                  break;

                case 29:  /* Abgeändertes Mandelbulb 3D */
                  we = H6(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);
                  break;

                case 30:   /* Mandelbulb3D  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Mandelbulb3D(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);
                  break;

                case 31:   /* Mandelbulb3D  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl, invers);
                  break;
                case 32:   /* Mandelbulb3D pow2 */
                  we = Mandelbulb3DPow2(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);
                  break;

                case 33:   /* Mandelbulb3D  pow 2*/
                  we = Mandelbulb3DPow2(x, y, z, zz, jx, jy, jz, jzz, zykl, invers);
                  break;

                case 34:   /* Mandelbulb3D  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Mandelbulb3DPow8(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);
                  break;

                case 35:   /* Mandelbulb3D  */
                  //  we = Mandelbulb3D(x, y, z, zz, jx, jy, jz, jzz, zykl);

                  we = Mandelbulb3DPow8(x, y, z, zz, jx, jy, jz, jzz, zykl, invers);
                  break;

                case 36: // Mandel mit Vektorrotation
                  we = H7(jx, jy, jz, jzz, x, y, z, zz, zykl, invers);
                  break;

                case 37: // Julia mit Vektorrotation
                  we = H7(x, y, z, zz, jx, jy, jz, jzz, zykl, invers);
                  break;





              }
              return ((int)we);
            } catch (Exception ex) {
              System.Diagnostics.Debug.WriteLine(ex.ToString());
              return 0;
            }

        }


        protected double LinearSin(double angle) {
          int angleInt = (int)(angle-0.5);
          angleInt = angleInt / 2;
          angleInt = angleInt / 2;
          angle = angle - angleInt;
          if (angle > 3) {
            angle = angle - 3;
          } else
          if (angle > 1)
            angle = 2 - angle;

          return angle - 1;
        }


        protected double LinearTan2(double angle1,double angle2) {
          double angle = 0;
          if (angle2 != 0)
              angle=  angle1 / angle2;

          double sin = LinearSin(angle);
          double cos = LinearCos(angle);

          double retVal = 0;
          if (cos != 0)
            retVal = sin / cos;

          return retVal;
        }



        protected double LinearCos(double angle) {
          int angleInt = (int)(angle - 0.5);
          angleInt = angleInt / 2;
          angleInt = angleInt / 2;
          angle = angle - angleInt;
          angle = -angle;
          if (angle < -2)
            angle = 2 + angle;

          return angle + 1;

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
        public double WinkelPerspective(long zykl, double x, double y, double z, double zz,
        double xd, double yd, double zd, double zzd,
        double wix, double wiy, double wiz,
        double jx, double jy, double jz, double jzz, int formula, bool invers, int pixelX, int pixelY,bool use4Points) {
            if (zd == 0) {
                Console.WriteLine("Error in WinkelPerspective: zd==0");
                return 0;

            }


            PixelInfo[] borderPoints = new PixelInfo[4];

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
          // Testweise verkleinert:
           // distance = 0.06;

            double xDistance = distance * 6.0;
            double zDistance = distance * 6.0;
          // debug only
          //  zDistance = distance * 2.0;

            // Eventuell während der Berechnung entstehende Zusatzinfos für alle 4 Punkte.
            AdditionalPointInfo[] pinfoSet=null;
            if (mInternFormula != null && mInternFormula.additionalPointInfo!=null)
            {
                pinfoSet = new AdditionalPointInfo[5];
            }

            for (k = 4; k >= 0; k--) {
                switch (k) {
                    case 2:     /* oben */
                        xn = x;
                        yn = y;
                        zn = z - zDistance * zd; zzn = zz - zDistance * zzd;
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

                if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                    for (m = 0; m >= -4.0; m -= 0.2) {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                            break;
                    }
                } else {
                    for (m = 0; m <= 4.0; m += 0.2) {
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
                    }
                }
                if ((m > -3.9) && (m < 3.9)) {
                    startwert = m + 0.2; diff = 0.1;
                    while (diff >= 0.00001) {
                        m = startwert - diff;
                        zm = zn; xm = xn;
                        ym = yn + m * yd; zzm = zzn;
                        if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                            startwert = m + diff;
                        else startwert = m;
                        diff /= 2.0;
                    }
                    tief[k] = m;
                } else {

                    // Fast Kopie der obigen Formeln
                    if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                        for (m = 0; m >= -8.0; m -= 0.02) {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)) {
                                if (!(Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)) {
                                    break;
                                } else {
                                    //   break;
                                    // Staubzähler erhöhen
                                }
                            }
                        }
                    } else {
                        for (m = 0; m <= 8.0; m += 0.02) {
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                                if (Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                                    m -= 0.02;
                                    break;
                                } else {
                                    // Staubzähler erhöhen
                                    //m -= 0.02;
                                    //break;
                                }
                            }
                        }
                    }
                    if ((m > -7.9) && (m < 7.9)) {
                        startwert = m + 0.02; diff = 0.01;
                        while (diff >= 0.00001) {
                            m = startwert - diff;
                            zm = zn; xm = xn;
                            ym = yn + m * yd; zzm = zzn;
                            if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                                startwert = m + diff;
                            else startwert = m;
                            diff /= 2.0;
                        }
                        tief[k] = m;
                    } else {
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
            
            for (k = 0; k < 4; k++) {
                pu = k + 1; if (k == 3) pu = 0;
                /* Die drei Punkte entsprechen tief[4] tief[k] und tief[pu]   */
                /* Zuerst wird tief abgezogen                                 */
                if ((tief[k] < 19) && (tief[pu] < 19) && tief[4] < 19) {

                    ox = xpos[k] - xpos[4];
                    oy = ypos[k] + tief[k] * yd - ypos[4] - tief[4] * yd;
                    oz = zpos[k] - zpos[4];

                    rx = xpos[pu] - xpos[4];
                    ry = ypos[pu] + tief[pu] * yd - ypos[4] - tief[4] * yd;
                    rz = zpos[pu] - zpos[4];

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


                   
                    if (use4Points) {
                      PixelInfo pInfo = null;
                      if (borderPoints[k] == null) {
                        pInfo = new PixelInfo();
                        borderPoints[k] = pInfo;
                      } else {
                        pInfo = borderPoints[k];
                      }

                      pInfo.Coord.X = xpos[k];
                      pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                      pInfo.Coord.Z = zpos[k];
                      pInfo.frontLight = winkel;
                      pInfo.Normal = normals[k];
                      pInfo.IsInside = !invers;

                       if (pinfoSet != null)
                          pInfo.AdditionalInfo = pinfoSet[k];
                    } else {
                      PixelInfo pInfo = null;
                        if (pData.Points[pixelX, pixelY] == null) {
                          // TODO: Später Querschnitt aus allen Einzelwinkeln bestimmen
                          pInfo = new PixelInfo();
                          pData.Points[pixelX, pixelY] = pInfo;
                          pInfo.Coord.X = xpos[k];
                          pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                          pInfo.Coord.Z = zpos[k];
                          pInfo.IsInside = !invers;
                        } else {
                          pInfo = pData.Points[pixelX, pixelY];
                          pInfo.IsInside = !invers;
                        }
                        pInfo.Normal = normals[k];
                        pInfo.frontLight += winkel / 4.0;
                  if (k == 0||k==3) { // Debug only (um die reale Position zu ermitteln)   
                        if (pinfoSet != null)
                          pInfo.AdditionalInfo = pinfoSet[k];
                  }
                        // TODO: Auch die Normalen übertragen
                      
                    }
                  
                }
                else {

                    int indexX = 0, indexY = 0;
                    if (pixelX >= 0 && pixelY >= 0) {
                      switch (k) {
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

      
          // Zusatzinformationen der anderen Pixel aufbauen

            if (use4Points) {

              int indexX = pixelX;
              int indexY = pixelY;
              int kNeigbour1 = 0;
              int kNeigbour2 = 0;
              for (k = 0; k < 4; k++) {
             
                  switch (k) {

                  case 0:
                    indexX = pixelX + 1;
                    indexY = pixelY + 1;
                    kNeigbour1 = 1;
                    kNeigbour2 = 2;
                    break;

                  case 1:
                    indexX = pixelX;
                    indexY = pixelY + 1;
                    kNeigbour1 = 2;
                    kNeigbour2 = 3;
                    break;

                  case 2:
                    indexX = pixelX;
                    indexY = pixelY;
                    kNeigbour1 = 3;
                    kNeigbour2 = 0;
                    break;

                  case 3:
                    indexX = pixelX + 1;
                    indexY = pixelY;
                    kNeigbour1 = 0;
                    kNeigbour2 = 1;
                    break;

                }
              
                PixelInfo otherP1 = borderPoints[kNeigbour1];
                PixelInfo otherP2 = borderPoints[kNeigbour2];
                if (otherP1 == null)
                  otherP1 = otherP2;
                if (otherP2 == null)
                  otherP2 = otherP1;
                if (otherP1 != null) {
                   PixelInfo pInfo = new PixelInfo();
                   pInfo.IsInside = !invers;
                      pInfo.Coord.X = (otherP1.Coord.X+otherP2.Coord.X)/2.0;
                      pInfo.Coord.Y = (otherP1.Coord.Y + otherP2.Coord.Y) / 2.0;
                      pInfo.Coord.Z = (otherP1.Coord.Z + otherP2.Coord.Z) / 2.0;
                      pInfo.frontLight = (otherP1.frontLight+otherP2.frontLight)/2.0;
                      pInfo.Normal.X = (otherP1.Normal.X + otherP2.Normal.X) / 2.0;
                      pInfo.Normal.Y = (otherP1.Normal.Y + otherP2.Normal.Y) / 2.0;
                      pInfo.Normal.Z = (otherP1.Normal.Z + otherP2.Normal.Z) / 2.0;
                      if (otherP1.AdditionalInfo != null && otherP2.AdditionalInfo != null) {
                        AdditionalPointInfo pinfo = new AdditionalPointInfo();
                        pinfo.blue = (otherP1.AdditionalInfo.blue + otherP2.AdditionalInfo.blue) / 2.0;
                        pinfo.green = (otherP1.AdditionalInfo.green + otherP2.AdditionalInfo.green) / 2.0;
                        pinfo.red = (otherP1.AdditionalInfo.red + otherP2.AdditionalInfo.red) / 2.0;
                        pInfo.AdditionalInfo = pinfo;
                      }

                      if (indexX < pData.Width && indexY < pData.Height) {
                        pData.Points[indexX, indexY] = pInfo;
                      }

                }


              }
            }



            return ((int)col[0]);
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
        double jx, double jy, double jz, double jzz, int formula, bool invers, int pixelX, int pixelY, bool use4Points) {
          if (zd == 0) {
            Console.WriteLine("Error in WinkelPerspective: zd==0");
            return 0;

          }

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
          if (mInternFormula != null && mInternFormula.additionalPointInfo != null) {
            pinfoSet = new AdditionalPointInfo[5];
          }

          for (k = 4; k >= 0; k--) {
            switch (k) {
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

            if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
              for (m = 0; m >= -4.0; m -= 0.2) {
                zm = zn; xm = xn;
                ym = yn + m * yd; zzm = zzn;
                if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0))
                  break;
              }
            } else {
              for (m = 0; m <= 4.0; m += 0.2) {
                zm = zn; xm = xn;
                ym = yn + m * yd; zzm = zzn;
                if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) { m -= 0.2; break; }
              }
            }
            if ((m > -3.9) && (m < 3.9)) {
              startwert = m + 0.2; diff = 0.1;
              while (diff >= 0.00001) {
                m = startwert - diff;
                zm = zn; xm = xn;
                ym = yn + m * yd; zzm = zzn;
                if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                  startwert = m + diff;
                else startwert = m;
                diff /= 2.0;
              }
              tief[k] = m;
            } else {

              // Fast Kopie der obigen Formeln
              if (Rechne(xn, yn, zn, zzn, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                for (m = 0; m >= -8.0; m -= 0.02) {
                  zm = zn; xm = xn;
                  ym = yn + m * yd; zzm = zzn;
                  if (!(Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)) {
                    if (!(Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0)) {
                      break;
                    } else {
                      //   break;
                      // Staubzähler erhöhen
                    }
                  }
                }
              } else {
                for (m = 0; m <= 8.0; m += 0.02) {
                  zm = zn; xm = xn;
                  ym = yn + m * yd; zzm = zzn;
                  if (Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                    if (Rechne(xm, ym + yd / 2.0, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers) > 0) {
                      m -= 0.02;
                      break;
                    } else {
                      // Staubzähler erhöhen
                      //m -= 0.02;
                      //break;
                    }
                  }
                }
              }
              if ((m > -7.9) && (m < 7.9)) {
                startwert = m + 0.02; diff = 0.01;
                while (diff >= 0.00001) {
                  m = startwert - diff;
                  zm = zn; xm = xn;
                  ym = yn + m * yd; zzm = zzn;
                  if (0L == Rechne(xm, ym, zm, zzm, zykl, wix, wiy, wiz, jx, jy, jz, jzz, formula, invers))
                    startwert = m + diff;
                  else startwert = m;
                  diff /= 2.0;
                }
                tief[k] = m;
              } else {
                tief[k] = 20;
              }

              // Ende: Fast Kopie der obigen Formeln




            }

            if (pinfoSet != null) {
              pinfoSet[k] = new AdditionalPointInfo(mInternFormula.additionalPointInfo);
            }
          }

          // Die Normalen der 4 Randpunkte
          Vec3[] normals = new Vec3[4];

          for (k = 0; k < 4; k++) {
            pu = k + 1; if (k == 3) pu = 0;
            /* Die drei Punkte entsprechen tief[4] tief[k] und tief[pu]   */
            /* Zuerst wird tief abgezogen                                 */
            if ((tief[k] < 19) && (tief[pu] < 19) && tief[4] < 19) {

              ox = xpos[k] - xpos[4];
              oy = ypos[k] + tief[k] * yd - ypos[4] - tief[4] * yd;
              oz = zpos[k] - zpos[4];

              rx = xpos[pu] - xpos[4];
              ry = ypos[pu] + tief[pu] * yd - ypos[4] - tief[4] * yd;
              rz = zpos[pu] - zpos[4];

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
              if (use4Points) {

                switch (k) {
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

                if (k == 3) {
                  if (indexX < pData.Width && indexY < pData.Height) {
                    if (pData.Points[indexX, indexY] == null) {
                      PixelInfo pInfo = new PixelInfo();
                      pInfo.Coord.X = xpos[k];
                      pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                      pInfo.Coord.Z = zpos[k];
                      pInfo.frontLight = winkel;
                      pInfo.Normal = normals[k];
                      pData.Points[indexX, indexY] = pInfo;
                      if (pinfoSet != null)
                        pInfo.AdditionalInfo = pinfoSet[k];
                    }
                  }
                }
              } else {
                PixelInfo pInfo = null;
                if (pData.Points[pixelX, pixelY] == null) {
                  // TODO: Später Querschnitt aus allen Einzelwinkeln bestimmen
                  pInfo = new PixelInfo();
                  pData.Points[pixelX, pixelY] = pInfo;
                  pInfo.Coord.X = xpos[k];
                  pInfo.Coord.Y = ypos[k] + tief[k] * yd;
                  pInfo.Coord.Z = zpos[k];
                } else {
                  pInfo = pData.Points[pixelX, pixelY];
                }
                pInfo.Normal = normals[k];
                pInfo.frontLight += winkel / 4.0;
                if (pinfoSet != null)
                  pInfo.AdditionalInfo = pinfoSet[k];
                // TODO: Auch die Normalen übertragen
              }

            } else {

              int indexX = 0, indexY = 0;
              if (pixelX >= 0 && pixelY >= 0) {
                switch (k) {
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
          return ((int)col[0]);
        }


    }
}
