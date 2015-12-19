using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.TomoGeometry {
    public class Mandelbulb: TomoFormula {


        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init() {
            base.Init();
            // Hier kann z.B. pow oder gr aus den Einstellungen gelesen werden.
        }


        public override long InSet(double ar, double ai, double aj, double br, double bi, double bj, double bk, long zkl, bool invers) {
            double aar, aai, aaj;
            long tw;
            int n;
            int pow = 8; // n=8 entspricht dem Mandelbulb
            double gr = 10; // Ab diesem Wert liegt mit Sicherheit Nichtzugehörigkeit zur Menge vor.
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
    }
}
