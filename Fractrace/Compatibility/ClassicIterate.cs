using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.Geometry;
using Fractrace;

namespace Fractrace.Compability {


    public class ClassicIterate {

        double[] col = null;

        double xd, yd, zd, zzd;
        double x, y, z, zz;

        public static double ALLZOOM = 2;

        public static double DEPHFACTOR = 2;

        Graphics grLabel = null;

        GraphicData GData = null;

        protected bool mAbort = false;

        public void Abort() {
            mAbort = true;
        }

        /// <summary>
        /// Liefert Informationen zu den gezeichneten
        /// </summary>
        public GraphicData GraphicInfo {
            get {
                return GData;
            }
        }


        protected int width = 1000;

        protected int height = 1000;

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public ClassicIterate(int width, int height) {
            GData = new GraphicData(width, height);
            this.width = width;
            this.height = height;
        }


        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="grLabel"></param>
        /// <param name="grLabel_1"></param>
        /// <param name="grLabel_2"></param>
        public void Init(Graphics grLabel) {
            this.grLabel = grLabel;
        }

        double percent = 0;

        /// <summary>
        /// Liefert den Status der Berechnung
        /// </summary>
        /// 
        public double CurrentStatus {
            get {
                return percent;
            }
        }


        // the main loop
        public void frac_iterate(FracValues act_val, int zyklen, int raster, int pScreensize, int formula, bool perspective) {

            Formulas formulas = new Formulas();


            col = formulas.col;

            int MAXX = (int)ParameterDict.Exemplar.GetDouble("View.MaxXDrawing");
            int MAXY = (int)ParameterDict.Exemplar.GetDouble("View.MaxYDrawing");
            int MAXZ = (int)ParameterDict.Exemplar.GetDouble("View.MaxZDrawing");

            int MINX = (int)ParameterDict.Exemplar.GetDouble("View.MinXDrawing");
            int MINY = (int)ParameterDict.Exemplar.GetDouble("View.MinYDrawing");
            int MINZ = (int)ParameterDict.Exemplar.GetDouble("View.MinZDrawing");

            double viewZoom = ParameterDict.Exemplar.GetDouble("View.Zoom");
            if (viewZoom == 0)
                viewZoom = 1;

            double screensize = viewZoom * pScreensize;

            const int MAXX_ITER = 450;
            const int MAXY_ITER = 200;
            const int MAXZ_ITER = 290;

            const int MINX_ITER = 0;
            const int MINY_ITER = 0;
            const int MINZ_ITER = 0;

            double xCenter1 = pScreensize * (MAXX_ITER - MINX_ITER) / 2.0;
            double xCenter2 = screensize * (MAXX_ITER - MINX_ITER) / 2.0;
            int xxViewCorrect = (int)(xCenter1 - xCenter2);

            double yCenter1 = pScreensize * (MAXY_ITER - MINY_ITER) / 2.0;
            double yCenter2 = screensize * (MAXY_ITER - MINY_ITER) / 2.0;
            int yyViewCorrect = (int)(yCenter1 - yCenter2);



            // Original: 
            /*
            const int MAXX_ITER = 450;
            const int MAXY_ITER = 200;
            const int MAXZ_ITER = 290;

            const int MINX_ITER = -450;
            const int MINY_ITER = -200;
            const int MINZ_ITER = -290;
             */


            double fa1;//, zyklen = 5;
            int xschl = 0, yschl = 0, zschl = 0, xx = 0, yy = 0;
            int d = 0; // Radius, Position und Zaehler
            double alpha = 0, r = 0; // Winkel und Radius
            double wix = 0, wiy = 0, wiz = 0;
            double jx = 0, jy = 0, jz = 0, jzz = 0;

            int minYDrawing = (int)ParameterDict.Exemplar.GetDouble("minYDrawing");
            int maxYDrawing = (int)ParameterDict.Exemplar.GetDouble("maxYDrawing");

            int minCycle = (int)ParameterDict.Exemplar.GetDouble("Formula.Static.MinCycle");
            if (minCycle == 0)
                minCycle = 100;

            int cycleAdd = 56;

            // int formula = 0;

            double universalZoom = ALLZOOM * screensize;
            // double universalZoomZ = ALLZOOM * screensize;

            //double universalZoom = ALLZOOM;
            Pen p = new Pen(Color.FromArgb(100, 200, 50));
            SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));

            int alive = 1, zaebil;

            //formula = 24;

            int t1 = 0;
            int colour_type = 0;

            int G_Parameter__number_of_frames = 1;
            int G_Parameter__actual_frame = 0;
            do {
                // now starts the iteration
                for (zaebil = G_Parameter__number_of_frames; zaebil > 0; zaebil--) {
                    G_Parameter__actual_frame++;
                    // zu Begin wird nur mit einem Bild gearbeitet:
                    //G_Parameter->make_aktual_tupel(act_val);  
                    alive = 0;
                    wix = act_val.arc.x;
                    wiy = act_val.arc.y;
                    wiz = act_val.arc.z;
                    jx = act_val.initial_tupel.x;
                    jy = act_val.initial_tupel.y;
                    jz = act_val.initial_tupel.z;
                    jzz = act_val.initial_tupel.zz;

                    // noch nicht gezoomt
                    GlobalObjects.G_Zoom.mark = 0;

                    xd = raster * (act_val.end_tupel.x - act_val.start_tupel.x) / (MAXX_ITER * screensize);
                    yd = raster * (act_val.end_tupel.y - act_val.start_tupel.y) / (MAXY_ITER * screensize);
                    zd = raster * (act_val.end_tupel.z - act_val.start_tupel.z) / (MAXZ_ITER * screensize);
                    zzd = raster * (act_val.end_tupel.zz - act_val.start_tupel.zz) / (MAXZ_ITER * screensize);

                    // alt:
                    z = act_val.end_tupel.z + zd;
                    zz = act_val.end_tupel.zz + zzd;


                    // double percentCount = 100.0 / (3.0 * universalZoom * MAXZ);


                    double xcenter = (act_val.start_tupel.x + act_val.end_tupel.x) / 2.0;
                    double ycenter = (act_val.start_tupel.y + act_val.end_tupel.y) / 2.0;
                    double zcenter = (act_val.start_tupel.z + act_val.end_tupel.z) / 2.0;

                    // double dephFactor = (act_val.end_tupel.y - act_val.start_tupel.y) / (MAXX * MAXY * MAXZ * 20.0);
                    // if (dephFactor < 0)
                    //     dephFactor = -dephFactor;
                    double dephFactor = 0;

                    double currentDeph = 0;
                    bool isBorder = true;
                    bool isZborder = true;
                    bool isXborder = true;
                    bool isYborder = true;

                    double currentDephSize = 1;

                    // Projektion initialisieren und der Berechnung zuordnen:
                    double cameraDeph = act_val.end_tupel.y - act_val.start_tupel.y;
                    Vec3 camera = new Vec3(xcenter, act_val.end_tupel.y + cameraDeph, zcenter);
                    Vec3 viewPoint = new Vec3(xcenter, act_val.end_tupel.y, zcenter);
                    Projection proj = new Projection(camera, viewPoint);



                    for (zschl = (int)(screensize * MAXZ_ITER); zschl >= (screensize * MINZ_ITER); zschl -= raster) {

                        Console.WriteLine("zschl=" + zschl.ToString());
                        //currentDephSize += 0.00453;
                        isZborder = false;
                        /*
                        if (zschl < universalZoom * MINZ + raster) {
                            isZborder = true;
                        } else
                            if (zschl > universalZoom * MAXZ - raster) {
                                isZborder = true;
                            }
                         */

                        if (zschl < (screensize * MINZ_ITER) + raster) {
                            isZborder = true;
                        } else
                            if (zschl > (screensize * MAXZ_ITER) - raster) {
                                isZborder = true;
                            }


                        //  percent += percentCount;
                        // Neuzeichnen aktivieren
                        System.Windows.Forms.Application.DoEvents();
                        z = act_val.end_tupel.z - (double)zd * (screensize * MAXZ_ITER - zschl) / (raster);

                        Console.WriteLine("percent=" + percent.ToString());
                        // TODO: Nach jedem Z neu zeichnen

                        zz = act_val.end_tupel.zz - (double)zzd * (screensize * MAXZ_ITER - zschl) / (raster);

                        bool minYDetected = false;
                        for (xschl = (int)(screensize * MINX_ITER); xschl <= screensize * MAXX_ITER; xschl += raster) {
                            // Console.WriteLine("xschl=" + xschl.ToString());

                            isXborder = false;

                            isXborder = false;

                            /*
                            if (xschl > universalZoom * MAXX - raster) {
                                isXborder = true;
                            } else if (xschl < universalZoom * MINX + raster) {
                                isXborder = true;
                            }*/

                            if (xschl > screensize * MAXX_ITER - raster) {
                                isXborder = true;
                            } else if (xschl < screensize * MINX_ITER + raster) {
                                isXborder = true;
                            }


                            if (mAbort) {
                                mAbort = false;
                                return;
                            }

                            // TODO: Hier die Tiefe hinzurechnen (mit größerem z sollte auch xd größer werden)
                            // Besser: Abstand zur Kamera berechnen und entsprechend x,y,z setzen
                            x = act_val.start_tupel.x + (double)xd * xschl / (raster);

                            int analogDephtInfo = 455;
                            double satColor = 0.1;
                            // Primitive Bildmanipulation
                            double tewared = 4;
                            double tewagreen = 4;
                            double tewablue = 4;

                            currentDeph = 1;




                            double currentDephSize1 = currentDephSize;
                            double miny = 0;

                            //       for (yschl = (int)universalZoom * MAXY; yschl >= universalZoom * MINY; yschl -= raster) {


                            //for (yschl = (int)maxYDrawing; yschl >= minYDrawing; yschl -= raster) {

                            for (yschl = (int)(screensize * MAXY_ITER); yschl >= screensize * MINZ_ITER; yschl -= raster) {

                                // Console.WriteLine("yschl=" + yschl.ToString());

                                if (minYDetected) {
                                    analogDephtInfo -= 10;
                                    //if(perspective)
                                    //    currentDephSize1 += 0.0153;

                                }
                                if (perspective)
                                    currentDephSize1 += DEPHFACTOR;
                                /*
                                if (yschl > universalZoom * MAXY - raster)
                                    isYborder = true;
                                else if (yschl < universalZoom * MINY + raster)
                                    isYborder = true;
                                else
                                    isYborder = false;
                                */
                                isYborder = false;

                                /*
                                if (yschl > maxYDrawing - raster)
                                    isYborder = true;
                                else if (yschl < minYDrawing + raster)
                                    isYborder = true;
                                */

                                if (yschl > screensize * MAXY_ITER - raster)
                                    isYborder = true;
                                else if (yschl < screensize * MINY_ITER + raster)
                                    isYborder = true;


                                //currentDeph += yd * 0.01;
                                // Aktivieren wenn Perspektive gewünscht:


                                //wt = 1;

                                if (perspective) {
                                    xx = xschl;
                                    yy = MAXZ - zschl;
                                } else {
                                    xx = xschl + yschl + 50 + xxViewCorrect;
                                    yy = (int)(yschl - zschl + screensize * MAXZ_ITER + 50) + yyViewCorrect;
                                }

                                //    if (xx > 0 && xx < width && yy > 0 && yy < height && yschl <= maxYDrawing && yschl >= minYDrawing) {
                                //        if (xx > 0 && xx < width && yy > 0 && yy < height && yschl <= maxYDrawing && yschl >= minYDrawing) {
                                if (xx > 0 && xx < width && yy > 0 && yy < height) {

                                    // debug only
                                    if (isYborder) {

                                    }
                                    // ende debug only

                                    if ((GData.Picture)[xx, yy] == 0 || (GData.Picture)[xx, yy] == 2) { // aha, noch zeichnen
                                        // Test, ob Schnitt mit Begrenzung vorliegt  
                                        //if ((zschl == MAXZ) || (xschl == 1) || (yschl == MAXY)) wt = 0;
                                        // TODO: Hier die Tiefe hinzurechnen (mit größerem z sollte auch yd größer werden)
                                        y = act_val.end_tupel.y - (double)yd * (screensize * MAXY_ITER - yschl) / (raster);

                                        /*
                                        if (GlobalObjects.G_Parameter.fspin > 0) {
                                            // y und z (+zz) Koordinaten umrechen
                                            r = Math.Sqrt(y * y + z * z);
                                            alpha = Math.Acos(y / r); // winkel 
                                            if (z < 0)
                                                alpha += Math.PI;
                                            d = (int)(GlobalObjects.G_Parameter.fspin * alpha / (Math.PI * 2));
                                            y = r * (Math.Cos(alpha / GlobalObjects.G_Parameter.fspin + d * (Math.PI * 2) / GlobalObjects.G_Parameter.fspin));
                                            z = r * (Math.Sin(alpha / GlobalObjects.G_Parameter.fspin + d * (Math.PI * 2) / GlobalObjects.G_Parameter.fspin));
                                        }
                                         */


                                        t1 = 0;

                                        t1 = 1;
                                        fa1 = 0;//test=1;

                                        // Tiefe in x,y,z hineinrechnen


                                        double xProjection = (x - xcenter) * currentDephSize1 + xcenter;
                                        double yProjection = (y - ycenter) * currentDephSize1 + ycenter;
                                        double zProjection = (z - zcenter) * currentDephSize1 + zcenter;

                                        int usedCycles = 0;
                                        bool inverse = false;
                                        if ((GData.Picture)[xx, yy] == 0)
                                            usedCycles = formulas.Rechne(xProjection, y, zProjection, zz, zyklen,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);

                                        if ((GData.Picture)[xx, yy] == 2) {// Invers rechnen
                                            inverse = true;
                                            usedCycles = formulas.Rechne(xProjection, y, zProjection, zz, minCycle,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);
                                            if (usedCycles == 0) {
                                                //(GData.Picture)[xx, yy] = 1;
                                            }
                                        }

                                        // debug only

                                        // ende debug only

                                        if (usedCycles == 0) {

                                            if (!minYDetected)
                                                miny = yschl;
                                            minYDetected = true;

                                            GData.ColorInfoDeph[xx, yy] = analogDephtInfo;

                                            // iteration ist nicht abgebrochen also weiterrechnen
                                            (GData.Picture)[xx, yy] = 1; // Punkt als gesetzt markieren
                                            VoxelInfo vInfo = new VoxelInfo();
                                            GData.PointInfo[xx, yy] = vInfo;
                                            vInfo.i = x;
                                            vInfo.j = y;
                                            vInfo.k = z;
                                            vInfo.l = zz;



                                            if (isXborder || isYborder || isZborder) { // es liegt Schnitt mit Begrenzung vor
                                                colour_type = 0; // COL; // Farbige Darstellung
                                                //col[3] = Formulas.Rechne(xProjection, yProjection, zProjection, zz, zyklen + 27,
                                                //        wix, wiy, wiz,
                                                //        jx, jy, jz, jzz, formula);

                                                /*
                                                col[0] = Formulas.Rechne(x + xd / 2, y, z, zz, zyklen + 27,
                                                        wix, wiy, wiz,
                                                        jx, jy, jz, jzz, formula);
                                                col[1] = Formulas.Rechne(x, y + yd / 2, z, zz, zyklen + 27,
                                                        wix, wiy, wiz,
                                                        jx, jy, jz, jzz, formula);
                                                col[2] = Formulas.Rechne(x - xd / 2, y + yd / 2, z, zz, zyklen + 27,
                                                        wix, wiy, wiz,
                                                        jx, jy, jz, jzz, formula);
                                                 */
                                                //fa1 = col[3]; // mit dieser Farbe wird auf den anderen Canvas gemalt  

                                                fa1 = formulas.Rechne(xProjection, y, zProjection, zz, zyklen + cycleAdd,
                                                 wix, wiy, wiz,
                                                 jx, jy, jz, jzz, formula, false);

                                                if (fa1 == 0) {
                                                    if (minCycle < 45) {
                                                        fa1 = -1;
                                                        (GData.Picture)[xx, yy] = 2; // Punkt nicht als gesetzt markieren
                                                    } else {
                                                        fa1 = 255;
                                                    }
                                                } else
                                                    fa1 = 255 * fa1 / (zyklen + cycleAdd);

                                                // debug only: alle Farbwerte auf 1 setzen
                                                col[0] = col[1] = col[2] = col[3] = 255;
                                                //fa1 = 255;
                                            } else {// innerer Punkt
                                                colour_type = 1; // GREY;
                                                if (inverse)
                                                    fa1 = formulas.Winkel(minCycle, xProjection, yProjection, zProjection, zz, xd * currentDephSize1, yd * currentDephSize1, zd * currentDephSize1, zzd,
       wix, wiy, wiz,
       jx, jy, jz, jzz, formula, perspective, inverse);
                                                else
                                                    fa1 = formulas.Winkel(zyklen, xProjection, yProjection, zProjection, zz, xd * currentDephSize1, yd * currentDephSize1, zd * currentDephSize1, zzd,
                                                           wix, wiy, wiz,
                                                           jx, jy, jz, jzz, formula, perspective, inverse);
                                                fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                                            }

                                            // winkel arbeitet auch mit der globalen variablen col

                                            // TODO: Zeichnen


                                            if (raster > 2) {
                                                if (colour_type == 0) {
                                                    if (fa1 >= 0) {
                                                        p.Color = Color.White;

                                                        if (isXborder)
                                                            p.Color = Color.FromArgb((int)fa1, (int)(fa1 / 2.0), (int)(fa1 / 2.0));
                                                        if (isYborder)
                                                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1), (int)(fa1 / 2.0));
                                                        if (isZborder)
                                                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1 / 2.0), (int)(fa1));



                                                        //    p.Color = Color.FromArgb((int)fa1, (int)(fa1/2.0), (int)(fa1/2.0));
                                                        GData.ColorInfo2[xx, yy] = fa1;
                                                        grLabel.DrawRectangle(p, xx, yy, raster / 2, raster / 2);
                                                    }
                                                } else {
                                                    brush.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                                                    //grLabel_1.FillRectangle(brush, xschl + 10, yschl + 10, raster, raster);
                                                    //grLabel_2.FillRectangle(brush, xschl + 10, 290 - zschl + 10, raster, raster);

                                                    double redMin = 1;
                                                    if (inverse) {
                                                        redMin = 0.5;
                                                    }
                                                    p.Color = Color.FromArgb((int)(redMin * col[3]), (int)col[3], (int)col[3]);

                                                    grLabel.FillRectangle(brush, xx, yy, raster / 2, raster / 2);

                                                    p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                                                    grLabel.FillRectangle(brush, xx + raster / 2, yy, raster, raster / 2);

                                                    p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                                                    grLabel.FillRectangle(brush, xx + raster / 2, yy + raster / 2, raster, raster);

                                                    p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                                                    grLabel.FillRectangle(brush, xx, yy + raster / 2, raster / 2, raster);
                                                }

                                            } else if (raster == 2) {

                                                if (analogDephtInfo < 0)
                                                    analogDephtInfo = 0;


                                                // TODO: Aus der Berecnung genauere Werte für analogDephtInfo
                                                // ermitteln
                                                GData.ColorInfoDeph[xx, yy] = analogDephtInfo;
                                                if (xx < width - 1)
                                                    GData.ColorInfoDeph[xx + 1, yy] = analogDephtInfo;
                                                if (yy < height - 1)
                                                    GData.ColorInfoDeph[xx, yy + 1] = analogDephtInfo;
                                                if ((yy < height - 1) && (xx < width - 1))
                                                    GData.ColorInfoDeph[xx + 1, yy + 1] = analogDephtInfo;

                                                if (colour_type == 0) {
                                                    if (fa1 >= 0) {
                                                        p.Color = Color.White;
                                                        /*
                                                        if (isXborder)
                                                            p.Color = Color.FromArgb((int)fa1, (int)(fa1 / 2.0), (int)(fa1 / 2.0));
                                                        if (isYborder)
                                                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1), (int)(fa1 / 2.0));
                                                        if (isZborder)
                                                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1 / 2.0), (int)(fa1));
                                                        */
                                                        if (isXborder)
                                                            p.Color = Color.FromArgb((int)fa1, 0, 0);
                                                        if (isYborder)
                                                            p.Color = Color.FromArgb(0, (int)(fa1), 0);
                                                        if (isZborder)
                                                            p.Color = Color.FromArgb(0, 0, (int)(fa1));


                                                        //    p.Color = Color.FromArgb((int)fa1, (int)(fa1/2.0), (int)(fa1/2.0));
                                                        GData.ColorInfo2[xx, yy] = fa1;
                                                        grLabel.DrawRectangle(p, xx, yy, 1, 1);
                                                    } else {
                                                        // hier soll später weitergezeichnet werden.
                                                        p.Color = Color.Blue;
                                                        grLabel.DrawRectangle(p, xx, yy, (float)0.5, (float)0.5);
                                                        (GData.Picture)[xx, yy] = 2;
                                                    }
                                                } else {

                                                    double redMin = 1;
                                                    if (inverse) {
                                                        redMin = 0.5;
                                                    }
                                                    p.Color = Color.FromArgb((int)(redMin * col[3]), (int)col[3], (int)col[3]);
                                                    DrawPoint(col[3], xx, yy);

                                                    GData.ColorInfo[xx, yy] = col[3];

                                                    p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                                                    DrawPoint(col[0], xx + 1, yy);
                                                    GData.ColorInfo[xx + 1, yy] = col[0];

                                                    p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                                                    DrawPoint(col[1], xx + 1, yy + 1);
                                                    GData.ColorInfo[xx + 1, yy + 1] = col[1];

                                                    p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                                                    DrawPoint(col[2], xx, yy + 1);
                                                    GData.ColorInfo[xx, yy + 1] = col[2];

                                                    // Wenn Farbänderung zum Nachbarknoten zu groß ist, diesen Punkt neu
                                                    // berechnen
                                                    if (xx > 0 && yy > 0 && 1 == 2) {
                                                        double tempX = x - xd / 2.0;
                                                        double tempY = y - yd / 2.0;
                                                        double tempZ = z;
                                                        for (int xxi = xx; xxi <= xx + 1; xxi++, tempX += xd) {
                                                            for (int yyi = yy; yyi <= yy + 1; yyi++, tempY += yd) {
                                                                double currentCol = GData.ColorInfo[xxi, yyi];
                                                                if (currentCol < 40) {
                                                                    if (Math.Abs(GData.ColorInfo[xxi, yyi - 1] - currentCol) > 90 || Math.Abs(GData.ColorInfo[xxi - 1, yyi] - currentCol) > 90 || Math.Abs(GData.ColorInfo[xxi - 1, yyi - 1] - currentCol) > 130) {
                                                                        // Statt einem Punkt werden 4 berechnet und der Durchschnitt davon genommen
                                                                        xProjection = (tempX - xcenter) * currentDephSize1 + xcenter;
                                                                        yProjection = (tempY - ycenter) * currentDephSize1 + ycenter;
                                                                        zProjection = (tempZ - zcenter) * currentDephSize1 + zcenter;
                                                                        if (inverse)
                                                                            fa1 = formulas.Winkel(minCycle, xProjection, yProjection, zProjection, zz, xd / 1.5 * currentDephSize1, yd / 1.5 * currentDephSize1, zd / 1.5 * currentDephSize1, zzd,
                                                                                  wix, wiy, wiz,
                                                                                  jx, jy, jz, jzz, formula, perspective, inverse);
                                                                        else
                                                                            fa1 = formulas.Winkel(zyklen, xProjection, yProjection, zProjection, zz, xd / 1.5 * currentDephSize1, yd / 1.5 * currentDephSize1, zd / 1.5 * currentDephSize1, zzd,
                                                                              wix, wiy, wiz,
                                                                              jx, jy, jz, jzz, formula, perspective, inverse);
                                                                        // suche die ähnlichste Farbe zu den Randpunkten

                                                                        // fa1 = (int)(col[0] + col[1] + col[2] + col[3]) / 4;
                                                                        fa1 = 0;
                                                                        for (int i = 0; i < 4; i++) {
                                                                            if (col[i] > fa1) fa1 = col[i];
                                                                        }
                                                                        //fa1 = (int)(Math.Max(col[0],col[1],col[2],col[3]));
                                                                        /*
                                                    int col_a = (int)analogDephtInfo * fa1 / 256;
                                                    int colA = (int)(fa1 * tewared * satColor);
                                                    if (col_a > 255) col_a = 255;
                                                    if (col_a < 0) col_a = 0;
                                                    if (colA > 255) colA = 255;
                                                    if (colA < 0) colA = 0;
                                                    p.Color = Color.FromArgb(col_a,colA,colA);
                                                                         * */
                                                                        p.Color = Color.FromArgb((int)(redMin * fa1), (int)fa1, (int)fa1);
                                                                        DrawPoint((int)fa1, xxi, yyi);
                                                                        //grLabel.DrawRectangle(p, xxi, yyi, 1, 1);
                                                                        //GData.ColorInfo2[xxi, yyi] = fa1;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            } else {
                                                p.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                                                GData.ColorInfo2[xx, yy] = fa1;
                                                grLabel.DrawRectangle(p, xx, yy, 1, 1);

                                            }
                                        }
                                    }
                                }
                                //isYborder = false;
                            }
                            //isXborder = false;
                        }
                        //isZborder = false;
                    }
                }
            } while (alive == 1);
        }


        // Perspektivische Darstellung
        public void Generate(FracValues act_val, int zyklen, int raster, double screensize, int formula, bool perspective) {

            Formulas formulas = new Formulas();
            col = formulas.col;

            // debug only
            // Bestimmung der wirklich benutzten Minimal und MaximalWerte
            double xMinTest = double.MaxValue;
            double xMaxTest = double.MinValue;
            double yMinTest = double.MaxValue;
            double yMaxTest = double.MinValue;
            double zMinTest = double.MaxValue;
            double zMaxTest = double.MinValue;


            // ende debug only
            int MAXX_ITER = width;
            int MAXY_ITER = width + height;
            int MAXZ_ITER = height;

            int MINX_ITER = 0;
            int MINY_ITER = 0;
            int MINZ_ITER = 0;

            double xzDephFactor = 1;
            //double yDephFactor = 10 * screensize;
            double yDephFactor = 2 * raster * screensize;
            // yDephFactor |  raster  | screensize
            //  0.4        |     2    | 0,1
            //  0.8        |     2    | 0,2
            //  4          |     2    | 1
            //  yDephFactor = 4;

            double fa1;//, zyklen = 5;
            int xschl = 0, yschl = 0, zschl = 0, xx = 0, yy = 0;
            double wix = 0, wiy = 0, wiz = 0;


            double jx = 0, jy = 0, jz = 0, jzz = 0;


            jx = ParameterDict.Exemplar.GetDouble("Formula.Static.jx");
            jy = ParameterDict.Exemplar.GetDouble("Formula.Static.jy");
            jz = ParameterDict.Exemplar.GetDouble("Formula.Static.jz");
            jzz = ParameterDict.Exemplar.GetDouble("Formula.Static.jzz");
            act_val.end_tupel.zz = act_val.start_tupel.zz = ParameterDict.Exemplar.GetDouble("Formula.zz");

            // Innenbereich
            int minCycle = (int)ParameterDict.Exemplar.GetDouble("minCycle");
            if (minCycle == 0)
                minCycle = 100;

            // Offset für den Maximalzyklus für die klassische 2D-Darstellung 
            int cycleAdd = 56;

            Pen p = new Pen(Color.FromArgb(100, 200, 50));
            SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));


            int colour_type = 0;


            wix = act_val.arc.x;
            wiy = act_val.arc.y;
            wiz = act_val.arc.z;
            /*
                    jx = act_val.initial_tupel.x;
                    jy = act_val.initial_tupel.y;
                    jz = act_val.initial_tupel.z;
                    jzz = act_val.initial_tupel.zz;
             */

            // noch nicht gezoomt
            GlobalObjects.G_Zoom.mark = 0;

            xd = raster * (act_val.end_tupel.x - act_val.start_tupel.x) / (MAXX_ITER - MINX_ITER);
            yd = raster * (act_val.end_tupel.y - act_val.start_tupel.y) / (MAXY_ITER - MINY_ITER);
            zd = raster * (act_val.end_tupel.z - act_val.start_tupel.z) / (MAXZ_ITER - MINZ_ITER);
            zzd = raster * (act_val.end_tupel.zz - act_val.start_tupel.zz) / (MAXZ_ITER - MINZ_ITER);

            // alt:
            // z = act_val.end_tupel.z + zd;
            // zz = act_val.end_tupel.zz + zzd;
            z = act_val.end_tupel.z + zd;
            zz = act_val.end_tupel.zz + zzd;

            double xcenter = (act_val.start_tupel.x + act_val.end_tupel.x) / 2.0;
            double ycenter = (act_val.start_tupel.y + act_val.end_tupel.y) / 2.0;
            double zcenter = (act_val.start_tupel.z + act_val.end_tupel.z) / 2.0;


            bool isZborder = true;
            bool isXborder = true;
            bool isYborder = true;

            double currentDephSize = 1;

            // Projektion initialisieren und der Berechnung zuordnen:
            double cameraDeph = act_val.end_tupel.y - act_val.start_tupel.y;
            cameraDeph *= 0.3;
            Vec3 camera = new Vec3(xcenter, act_val.end_tupel.y + cameraDeph, zcenter);
            Vec3 viewPoint = new Vec3(xcenter, act_val.end_tupel.y, zcenter);
            Projection proj = new Projection(camera, viewPoint);
            formulas.Projection = proj;
            for (zschl = (int)(MAXZ_ITER); zschl >= (MINZ_ITER); zschl -= raster) {

                //   Console.WriteLine("zschl=" + zschl.ToString());
                isZborder = false;

                if (zschl < (MINZ_ITER) + raster) {
                    isZborder = true;
                } else
                    if (zschl > (MAXZ_ITER) - raster) {
                        isZborder = true;
                    }


                System.Windows.Forms.Application.DoEvents();
                z = act_val.end_tupel.z - (double)zd * (MAXZ_ITER - zschl) / (raster);
                //Console.WriteLine("z=" + z.ToString());
                // TODO: Nach jedem Z neu zeichnen

                zz = act_val.end_tupel.zz - (double)zzd * (MAXZ_ITER - zschl) / (raster);

                bool minYDetected = false;
                for (xschl = (int)(MINX_ITER); xschl <= MAXX_ITER; xschl += raster) {
                    // Console.WriteLine("xschl=" + xschl.ToString());

                    if (mAbort) {
                        mAbort = false;
                        return;
                    }

                    // TODO: Hier die Tiefe hinzurechnen (mit größerem z sollte auch xd größer werden)
                    // Besser: Abstand zur Kamera berechnen und entsprechend x,y,z setzen
                    x = act_val.start_tupel.x + (double)xd * xschl / (raster);

                    int analogDephtInfo = 455;

                    double currentDephSize1 = currentDephSize;
                    double miny = 0;

                    isYborder = true;
                    for (yschl = (int)(MAXY_ITER); yschl >= MINY_ITER; yschl -= raster) {

                        // Console.WriteLine("yschl=" + yschl.ToString());

                        if (perspective) {
                            xx = xschl;
                            yy = MAXZ_ITER - zschl;
                        } else {
                            xx = xschl + yschl + 50;
                            yy = (int)(yschl - zschl + screensize * MAXZ_ITER + 50);
                        }


                        // debug only
                        /*
                        xMinTest = Math.Min(x, xMinTest);
                        xMaxTest = Math.Max(x, xMaxTest);
                        yMinTest = Math.Min(y, yMinTest);
                        yMaxTest = Math.Max(y, yMaxTest);
                        zMinTest = Math.Min(z, zMinTest);
                        zMaxTest = Math.Max(z, zMaxTest);
                        */
                        // ende debug only: Bestimmung der realen Grenzen

                        if (xx >= 0 && xx < width && yy >= 0 && yy < height) {

                            // debug only
                            xMinTest = Math.Min(x, xMinTest);
                            xMaxTest = Math.Max(x, xMaxTest);
                            yMinTest = Math.Min(y, yMinTest);
                            yMaxTest = Math.Max(y, yMaxTest);
                            zMinTest = Math.Min(z, zMinTest);
                            zMaxTest = Math.Max(z, zMaxTest);

                            // ende debug only: Bestimmung der realen Grenzen



                            if ((GData.Picture)[xx, yy] == 0 || (GData.Picture)[xx, yy] == 2) { // aha, noch zeichnen
                                // Test, ob Schnitt mit Begrenzung vorliegt  
                                //if ((zschl == MAXZ) || (xschl == 1) || (yschl == MAXY)) wt = 0;
                                // TODO: Hier die Tiefe hinzurechnen (mit größerem z sollte auch yd größer werden)
                                y = act_val.end_tupel.y - (double)yd * (MAXY_ITER - yschl) / (raster);




                                fa1 = 0;//test=1;

                                // Tiefe in x,y,z hineinrechnen


                                double xProjection = (x - xcenter) * currentDephSize1 + xcenter;
                                double yProjection = (y - ycenter) * currentDephSize1 + ycenter;
                                double zProjection = (z - zcenter) * currentDephSize1 + zcenter;


                                int usedCycles = 0;
                                bool inverse = false;
                                if ((GData.Picture)[xx, yy] == 0)
                                    usedCycles = formulas.Rechne(xProjection, y, zProjection, zz, zyklen,
                                          wix, wiy, wiz,
                                          jx, jy, jz, jzz, formula, inverse);

                                if ((GData.Picture)[xx, yy] == 2) {// Invers rechnen
                                    inverse = true;
                                    usedCycles = formulas.Rechne(xProjection, y, zProjection, zz, minCycle,
                                          wix, wiy, wiz,
                                          jx, jy, jz, jzz, formula, inverse);

                                }


                                if (usedCycles == 0) {

                                    if (!minYDetected)
                                        miny = yschl;
                                    minYDetected = true;

                                    GData.ColorInfoDeph[xx, yy] = analogDephtInfo;

                                    // iteration ist nicht abgebrochen also weiterrechnen
                                    (GData.Picture)[xx, yy] = 1; // Punkt als gesetzt markieren
                                    VoxelInfo vInfo = new VoxelInfo();
                                    GData.PointInfo[xx, yy] = vInfo;
                                    vInfo.i = x;
                                    vInfo.j = y;
                                    vInfo.k = z;
                                    vInfo.l = zz;



                                    if (isYborder) { // es liegt Schnitt mit Begrenzung vor
                                        colour_type = 0; // COL; // Farbige Darstellung
                                        //col[3] = Formulas.Rechne(xProjection, yProjection, zProjection, zz, zyklen + 27,
                                        //        wix, wiy, wiz,
                                        //        jx, jy, jz, jzz, formula);

                                        /*
                                        col[0] = Formulas.Rechne(x + xd / 2, y, z, zz, zyklen + 27,
                                                wix, wiy, wiz,
                                                jx, jy, jz, jzz, formula);
                                        col[1] = Formulas.Rechne(x, y + yd / 2, z, zz, zyklen + 27,
                                                wix, wiy, wiz,
                                                jx, jy, jz, jzz, formula);
                                        col[2] = Formulas.Rechne(x - xd / 2, y + yd / 2, z, zz, zyklen + 27,
                                                wix, wiy, wiz,
                                                jx, jy, jz, jzz, formula);
                                         */
                                        //fa1 = col[3]; // mit dieser Farbe wird auf den anderen Canvas gemalt  

                                        fa1 = formulas.Rechne(xProjection, y, zProjection, zz, zyklen + cycleAdd,
                                         wix, wiy, wiz,
                                         jx, jy, jz, jzz, formula, false);

                                        if (fa1 == 0) {
                                            if (minCycle < 45) {
                                                fa1 = -1;
                                                (GData.Picture)[xx, yy] = 2; // Punkt nicht als gesetzt markieren
                                            } else {
                                                fa1 = 255;
                                            }
                                        } else
                                            fa1 = 255 * fa1 / (zyklen + cycleAdd);

                                        // debug only: alle Farbwerte auf 1 setzen
                                        col[0] = col[1] = col[2] = col[3] = 255;
                                        //fa1 = 255;
                                    } else {// innerer Punkt
                                        colour_type = 1; // GREY;
                                        //double xzDephFactor = currentDephSize1 * 10*screensize;
                                        //double yDephFactor = currentDephSize1 * 10 *screensize;

                                        // debug only: im Zweifel die Abstände viel kleiner setzen
                                        double tempFac = 0.1;
                                        //xzDephFactor *= tempFac;
                                        //yDephFactor *= 1;


                                        // ende debug only

                                        if (inverse)
                                            fa1 = formulas.Winkel(minCycle, xProjection, yProjection, zProjection, zz, xd * xzDephFactor, yd * yDephFactor, zd * xzDephFactor, zzd,
wix, wiy, wiz,
jx, jy, jz, jzz, formula, perspective, inverse);
                                        else
                                            fa1 = formulas.Winkel(zyklen, xProjection, yProjection, zProjection, zz, xd * xzDephFactor, yd * yDephFactor, zd * xzDephFactor, zzd * xzDephFactor,
                                                   wix, wiy, wiz,
                                                   jx, jy, jz, jzz, formula, perspective, inverse);
                                        fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                                    }

                                    // winkel arbeitet auch mit der globalen variablen col

                                    // TODO: Zeichnen


                                    if (raster > 2) {
                                        if (colour_type == 0) {
                                            if (fa1 >= 0) {
                                                p.Color = Color.White;

                                                if (isXborder)
                                                    p.Color = Color.FromArgb((int)fa1, (int)(fa1 / 2.0), (int)(fa1 / 2.0));
                                                if (isYborder)
                                                    p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1), (int)(fa1 / 2.0));
                                                if (isZborder)
                                                    p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1 / 2.0), (int)(fa1));



                                                //    p.Color = Color.FromArgb((int)fa1, (int)(fa1/2.0), (int)(fa1/2.0));
                                                GData.ColorInfo2[xx, yy] = fa1;
                                                grLabel.DrawRectangle(p, xx, yy, raster / 2, raster / 2);
                                            }
                                        } else {
                                            brush.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                                            //grLabel_1.FillRectangle(brush, xschl + 10, yschl + 10, raster, raster);
                                            //grLabel_2.FillRectangle(brush, xschl + 10, 290 - zschl + 10, raster, raster);

                                            double redMin = 1;
                                            if (inverse) {
                                                redMin = 0.5;
                                            }
                                            p.Color = Color.FromArgb((int)(redMin * col[3]), (int)col[3], (int)col[3]);

                                            grLabel.FillRectangle(brush, xx, yy, raster / 2, raster / 2);

                                            p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                                            grLabel.FillRectangle(brush, xx + raster / 2, yy, raster, raster / 2);

                                            p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                                            grLabel.FillRectangle(brush, xx + raster / 2, yy + raster / 2, raster, raster);

                                            p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                                            grLabel.FillRectangle(brush, xx, yy + raster / 2, raster / 2, raster);
                                        }

                                    } else if (raster == 2) {

                                        if (analogDephtInfo < 0)
                                            analogDephtInfo = 0;


                                        // TODO: Aus der Berecnung genauere Werte für analogDephtInfo
                                        // ermitteln
                                        GData.ColorInfoDeph[xx, yy] = analogDephtInfo;
                                        if (xx < width - 1)
                                            GData.ColorInfoDeph[xx + 1, yy] = analogDephtInfo;
                                        if (yy < height - 1)
                                            GData.ColorInfoDeph[xx, yy + 1] = analogDephtInfo;
                                        if ((yy < height - 1) && (xx < width - 1))
                                            GData.ColorInfoDeph[xx + 1, yy + 1] = analogDephtInfo;

                                        if (colour_type == 0) {
                                            if (fa1 >= 0) {
                                                p.Color = Color.White;
                                                /*
                                                if (isXborder)
                                                    p.Color = Color.FromArgb((int)fa1, (int)(fa1 / 2.0), (int)(fa1 / 2.0));
                                                if (isYborder)
                                                    p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1), (int)(fa1 / 2.0));
                                                if (isZborder)
                                                    p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1 / 2.0), (int)(fa1));
                                                */
                                                if (isXborder)
                                                    p.Color = Color.FromArgb((int)fa1, 0, 0);
                                                if (isYborder)
                                                    p.Color = Color.FromArgb(0, (int)(fa1), 0);
                                                if (isZborder)
                                                    p.Color = Color.FromArgb(0, 0, (int)(fa1));


                                                //    p.Color = Color.FromArgb((int)fa1, (int)(fa1/2.0), (int)(fa1/2.0));
                                                GData.ColorInfo2[xx, yy] = fa1;
                                                grLabel.DrawRectangle(p, xx, yy, 1, 1);
                                            } else {
                                                // hier soll später weitergezeichnet werden.
                                                p.Color = Color.Blue;
                                                grLabel.DrawRectangle(p, xx, yy, (float)0.5, (float)0.5);
                                                (GData.Picture)[xx, yy] = 2;
                                            }
                                        } else {

                                            double redMin = 1;
                                            if (inverse) {
                                                redMin = 0.5;
                                            }
                                            p.Color = Color.FromArgb((int)(redMin * col[3]), (int)col[3], (int)col[3]);
                                            DrawPoint(col[3], xx, yy);

                                            GData.ColorInfo[xx, yy] = col[3];

                                            p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                                            DrawPoint(col[0], xx + 1, yy);
                                            GData.ColorInfo[xx + 1, yy] = col[0];

                                            p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                                            DrawPoint(col[1], xx + 1, yy + 1);
                                            GData.ColorInfo[xx + 1, yy + 1] = col[1];

                                            p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                                            DrawPoint(col[2], xx, yy + 1);
                                            GData.ColorInfo[xx, yy + 1] = col[2];

                                            // Wenn Farbänderung zum Nachbarknoten zu groß ist, diesen Punkt neu
                                            // berechnen
                                            if (xx > 0 && yy > 0 && 1 == 2) {
                                                double tempX = x - xd / 2.0;
                                                double tempY = y - yd / 2.0;
                                                double tempZ = z;
                                                for (int xxi = xx; xxi <= xx + 1; xxi++, tempX += xd) {
                                                    for (int yyi = yy; yyi <= yy + 1; yyi++, tempY += yd) {
                                                        double currentCol = GData.ColorInfo[xxi, yyi];
                                                        if (currentCol < 40) {
                                                            if (Math.Abs(GData.ColorInfo[xxi, yyi - 1] - currentCol) > 90 || Math.Abs(GData.ColorInfo[xxi - 1, yyi] - currentCol) > 90 || Math.Abs(GData.ColorInfo[xxi - 1, yyi - 1] - currentCol) > 130) {
                                                                // Statt einem Punkt werden 4 berechnet und der Durchschnitt davon genommen
                                                                xProjection = (tempX - xcenter) * currentDephSize1 + xcenter;
                                                                yProjection = (tempY - ycenter) * currentDephSize1 + ycenter;
                                                                zProjection = (tempZ - zcenter) * currentDephSize1 + zcenter;

                                                                if (inverse)
                                                                    fa1 = formulas.Winkel(minCycle, xProjection, yProjection, zProjection, zz, xd * xzDephFactor, yd * yDephFactor, zd * xzDephFactor, zzd,
                                                                                wix, wiy, wiz,
                                                                                   jx, jy, jz, jzz, formula, perspective, inverse);
                                                                else
                                                                    fa1 = formulas.Winkel(zyklen, xProjection, yProjection, zProjection, zz, xd * xzDephFactor, yd * yDephFactor, zd * xzDephFactor, zzd * xzDephFactor,
                                                                           wix, wiy, wiz,
                                                                           jx, jy, jz, jzz, formula, perspective, inverse);
                                                                /*
                                                                if (inverse)
                                                                    fa1 = formulas.Winkel(minCycle, xProjection, yProjection, zProjection, zz, xd / 1.5 * currentDephSize1, yd / 1.5 * currentDephSize1, zd / 1.5 * currentDephSize1, zzd,
                                                                          wix, wiy, wiz,
                                                                          jx, jy, jz, jzz, formula, perspective, inverse);
                                                                else
                                                                    fa1 = formulas.Winkel(zyklen, xProjection, yProjection, zProjection, zz, xd / 1.5 * currentDephSize1, yd / 1.5 * currentDephSize1, zd / 1.5 * currentDephSize1, zzd,
                                                                      wix, wiy, wiz,
                                                                      jx, jy, jz, jzz, formula, perspective, inverse);
                                                                */
                                                                // suche die ähnlichste Farbe zu den Randpunkten

                                                                // fa1 = (int)(col[0] + col[1] + col[2] + col[3]) / 4;
                                                                fa1 = 0;
                                                                for (int i = 0; i < 4; i++) {
                                                                    if (col[i] > fa1) fa1 = col[i];
                                                                }
                                                                //fa1 = (int)(Math.Max(col[0],col[1],col[2],col[3]));
                                                                /*
                                            int col_a = (int)analogDephtInfo * fa1 / 256;
                                            int colA = (int)(fa1 * tewared * satColor);
                                            if (col_a > 255) col_a = 255;
                                            if (col_a < 0) col_a = 0;
                                            if (colA > 255) colA = 255;
                                            if (colA < 0) colA = 0;
                                            p.Color = Color.FromArgb(col_a,colA,colA);
                                                                 * */
                                                                p.Color = Color.FromArgb((int)(redMin * fa1), (int)fa1, (int)fa1);
                                                                DrawPoint((int)fa1, xxi, yyi);
                                                                //grLabel.DrawRectangle(p, xxi, yyi, 1, 1);
                                                                //GData.ColorInfo2[xxi, yyi] = fa1;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    } else {
                                        p.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                                        GData.ColorInfo2[xx, yy] = fa1;
                                        grLabel.DrawRectangle(p, xx, yy, 1, 1);

                                    }
                                }
                            }
                        }
                        isYborder = false;
                    }
                }
            }


            /*
            xMinTest = Math.Min(x, xMinTest);
            xMaxTest = Math.Max(x, xMaxTest);
            yMinTest = Math.Min(y, yMinTest);
            yMaxTest = Math.Max(y, yMaxTest);
            zMinTest = Math.Min(z, zMinTest);
            zMaxTest = Math.Max(z, zMaxTest);
             */

            Console.WriteLine("x: [" + act_val.start_tupel.x.ToString() + "; " + act_val.end_tupel.x.ToString() + "] real=[" + xMinTest.ToString() + "; " + xMaxTest.ToString() + "]");
            Console.WriteLine("y: [" + act_val.start_tupel.y.ToString() + "; " + act_val.end_tupel.y.ToString() + "] real=[" + yMinTest.ToString() + "; " + yMaxTest.ToString() + "]");
            Console.WriteLine("z: [" + act_val.start_tupel.z.ToString() + "; " + act_val.end_tupel.z.ToString() + "] real=[" + zMinTest.ToString() + "; " + zMaxTest.ToString() + "]");


        }


        /// <summary>
        /// Neuzeichnen:
        /// </summary>
        public void Repaint(double dephFactor) {
            double maxDephInfo = -100000;
            double minDephInfo = 100000;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    Pen p = new Pen(Color.FromArgb(0, 0, 0));
                    grLabel.DrawRectangle(p, i, j, 2, 2);
                    if (maxDephInfo < GData.ColorInfoDeph[i, j])
                        maxDephInfo = GData.ColorInfoDeph[i, j];
                    if (minDephInfo > GData.ColorInfoDeph[i, j])
                        minDephInfo = GData.ColorInfoDeph[i, j];
                }
            }
            double dephInfo = 0;
            double dephDiff = maxDephInfo - minDephInfo;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    //  if ((GData.Picture)[i, j] != 0) {
                    dephInfo = GData.ColorInfoDeph[i, j];
                    //  }
                    Pen p = new Pen(Color.FromArgb(100, 200, 50));
                    double fa1 = 0;
                    double fa2 = GData.ColorInfo2[i, j];


                    /*if(dephInfo>0)
                        fa1 = 70*((60+dephInfo - minDephInfo) / (dephDiff));
                    else 
                   // fa1 = GData.ColorInfo2[i, j];
                        fa1 = 0;
                     */


                    /*
                    double deph = (dephInfo - minDephInfo) / dephDiff;
                    fa1 = (dephFactor * deph);

                        if (fa1 < 0)
                            fa1 = 0;
                        if (fa1 > 255)
                            fa1 = 255;
                     */

                    /*
                    // Schatten berücksichtigen:
                        int shadowcount = 0;
                        for (int shadowi = i - 10; shadowi < i + 10; shadowi++) {
                            for (int shadowj = j - 10; shadowj < j + 10; shadowj++) {
                                if (shadowi >= 0 && shadowi < width && shadowj > 0 && shadowj < height) {
                                    double diffi = i- shadowi;
                                    double diffj = j- shadowj;
                                    double dist = Math.Sqrt(diffi * diffi + diffj * diffj);
                                    double dephInfo1 = GData.ColorInfoDeph[i, j];
                                    double dephInfo2 = GData.ColorInfoDeph[shadowi, shadowj];
                                    double dephDist = dephInfo2 - dephInfo1;
                                    if (dephDist > 0.02 * dist) {
                                        shadowcount++;
                                    }
                                }
                            }


                        }

                        if (shadowcount == 0)
                            shadowcount = 1;


                        double fa3 = 0;
                        fa3 = 0.5 * fa2 * (100 - shadowcount) / 20.0;

                        if (fa3 < 0)
                            fa3 = 0;
                        if (fa3 > 255)
                            fa3 = 255;
                    */
                    // p.Color = Color.FromArgb((int)fa1,(int) (fa2/shadowcount), (int)fa2);
                    p.Color = Color.FromArgb((int)fa2, (int)(fa2), (int)fa2);

                    grLabel.DrawRectangle(p, (float)i, (float)j, (float)0.5, (float)0.5);
                    //GData.ColorInfo[xx, yy] = col[3];


                    //  grLabel.DrawRectangle(p, xx, yy, 1, 1);
                }
            }


        }

        protected void DrawPoint(double col, int x, int y) {
            Pen p = new Pen(Color.FromArgb(100, 200, 50));
            p.Color = Color.FromArgb((int)col, (int)col, (int)col);
            grLabel.DrawRectangle(p, x, y, (float)0.5, (float)0.5);
            if (x < width && y < height)
                GData.ColorInfo2[x, y] = col;
        }

    }
}
