

using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.TomoGeometry {
    public class VecRotMandel2d: TomoFormula {


      int gr = 20;

      /// <summary>
      /// Initialisierung
      /// </summary>
      public override void Init() {
        base.Init();
        // Hier kann z.B. pow oder gr aus den Einstellungen gelesen werden.
      }


      public long InSet1(double ar, double ai, double aj, double br, double bi, double bj, double bk, long zkl, bool invers) {

        double xx, yy, zz;
        long tw;
        int n;
        //          ai = 0; aj = 0; ak = 0;

        double x = ar, y = ai, z = aj;

        xx = x * x; yy = y * y; zz = z * z;
        tw = 0;
        double r = Math.Sqrt(xx + yy + zz);
        Fractrace.Geometry.VecRotation vecRot = new Fractrace.Geometry.VecRotation();

        x = 1; // Um den Startwinkel eindeutig zu definieren.
        for (n = 1; n < zkl; n++) {
          double r_xy = Math.Sqrt(xx + yy);

          double theta = Math.Atan2(Math.Sqrt(xx + yy), z);
          double phi = Math.Atan2(y, x);

          vecRot.X = 0;
          vecRot.Y = 0;
          vecRot.Z = 1;
          vecRot.Angle = phi;

          Fractrace.Geometry.Vec3 pos = new Fractrace.Geometry.Vec3(x, y, z);
          Fractrace.Geometry.Vec3 newPos = vecRot.getTransform(pos);

          x = newPos.X;
          y = newPos.Y;
          z = newPos.Z;

          y += bj;
          x += br;
          z += bi;

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


      public override long InSet(double ar, double ai, double aj, double br, double bi, double bj, double bk, long zkl, bool invers) {

        double xx, yy, zz;
        long tw;
        int n;
        double x = ar, y = ai, z = aj;

        xx = x * x; yy = y * y; zz = z * z;
        tw = 0;
        double r = Math.Sqrt(xx + yy + zz);
        Fractrace.Geometry.VecRotation vecRot = new Fractrace.Geometry.VecRotation();
        for (n = 1; n < zkl; n++) {
          double r_xy = Math.Sqrt(xx + yy);
          double theta = Math.Atan2(Math.Sqrt(xx + yy), z);
          double phi = Math.Atan2(y, x);
          // Erste Rotation

          /*
          vecRot.x = 0;
          if (y >= 0 && z >= 0) {
            vecRot.y = -z;
            vecRot.z = y;
          }
          else if (y < 0 && z >= 0) {
            vecRot.y = z;
            vecRot.z = -y;
          } else if (y >= 0 && z < 0) {
            vecRot.y = -z;
            vecRot.z = y;
          } else if (y < 0 && z < 0) {
            vecRot.y = -z;
            vecRot.z = -y;
          }*/
          vecRot.X = z;
          vecRot.Y = 0;
          vecRot.Z = y;

          vecRot.Angle = phi;
          double vecr=Math.Sqrt(vecRot.X*vecRot.X+vecRot.Y*vecRot.Y+vecRot.Z*vecRot.Z);
          if (vecr != 0) {
            vecRot.X /= vecr;
            vecRot.Y /= vecr;
            vecRot.Z /= vecr;
          }

          Fractrace.Geometry.Vec3 pos = new Fractrace.Geometry.Vec3(x, y, z);
          Fractrace.Geometry.Vec3 newPos = vecRot.getTransform(pos);
          x = newPos.X;
          y = newPos.Y;
          z = newPos.Z;
          xx = x * x; yy = y * y; zz = z * z;// aak = ak * ak;
          r = Math.Sqrt(xx + yy + zz);
          x *= r;
          y *= r;
          z *= r;
          y += bj;
          x += br;
          z += bi;
          if (r > gr) {
            tw = n; break;
          }
        }

        // Hier könnte die zweite Rotation hin

        // Hinzugefügt, um bei der Ansicht von innen nur den Kern zu zeirgrn. 
        if (invers) {
          if (tw == 0)
            tw = 1;
          else
            tw = 0;
        }
        return (tw);
      }

    }
}
