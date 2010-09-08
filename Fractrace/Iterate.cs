using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.Geometry;

namespace Fractrace {


  /// <summary>
  /// Hier werden die Voxelpunkte abgefahren und die Berechnungsroutine aufgerufen.
  /// </summary>
  public class Iterate {

    Graphics grLabel = null;

    GraphicData GData = null;

    PictureData PData = null;

    protected bool mAbort = false;

    protected static bool mPause = false;


    /// <summary>
    /// Globales Anhalten der Berechnung.
    /// </summary>
    public static bool Pause {
      get {
        return mPause;
      }
      set {
        mPause = value;
      }
    }


    /// <summary>
    /// Von Außen wurde Abbruch angestoßen. 
    /// </summary>
    public void Abort() {
      mAbort = true;
      Console.WriteLine("Abort()");
    }


    /// <summary>
    /// Liefert true, wenn Abort aufgerufen wurde.
    /// </summary>
    public bool InAbort {
      get {
        return mAbort;
      }
    }


    /// <summary>
    /// Liefert Informationen zu den gezeichneten Pixeln.
    /// </summary>
    public GraphicData GraphicInfo {
      get {
        return GData;
      }
    }


    /// <summary>
    /// Liefert Zusatz-Informationen zu dem gezeichneten Bild
    /// </summary>
    public PictureData PictureData {
      get {
        return PData;
      }
    }


    protected int width = 1000;

    protected int height = 1000;


    /// <summary>
    /// Initialisation
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Iterate(int width, int height) {
      GData = new GraphicData(width, height);
      PData = new PictureData(width, height);
      this.width = width;
      this.height = height;
    }


    /// <summary>
    /// Das Steuerelement, dass die Berechnung angestoßen hat.
    /// </summary>
    IAsyncComputationStarter mStarter = null;


    /// <summary>
    /// Initialisation
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public Iterate(int width, int height, IAsyncComputationStarter starter, bool isRightView) {
      mStarter = starter;
      GData = new GraphicData(width, height);
      PData = new PictureData(width, height);
      this.width = width;
      this.height = height;
      this.mIsRightView = isRightView;
    }


    /// <summary>
    /// Wird bei der Stereoansicht verwendet. Hier wird unterschieden, ob
    /// es sich um eine Sicht vom rechten Auge handelt. 
    /// </summary>
    protected bool mIsRightView = false;



    /// <summary>
    /// Liefert den Status der Berechnung (von 0-100)
    /// </summary>
    double percent = 0;


    /// <summary>
    /// Liefert den Status der Berechnung (von 0-100)
    /// </summary>
    /// 
    public double CurrentStatus {
      get {
        return percent;
      }
    }


    FracValues m_act_val = null;
    int m_zyklen = 0;
    int m_raster = 0;
    double m_screensize = 0;
    int m_formula = 0;
    bool m_perspective = true;



    /// <summary>
    /// Dient als Lock variable.
    /// </summary>
    protected object generateLock = new object();

    protected object startCountLock = new object();

    protected int startCount = 0;


    /// <summary>
    /// Von hier aus wird die Berechnung in Einzelthreads aufgesplittet.
    /// </summary>
    /// <param name="act_val"></param>
    /// <param name="zyklen"></param>
    /// <param name="raster"></param>
    /// <param name="screensize"></param>
    /// <param name="formula"></param>
    /// <param name="perspective"></param>
    public void StartAsync(FracValues act_val, int zyklen, int raster, double screensize, int formula, bool perspective) {
      m_act_val = act_val;
      m_zyklen = zyklen;
      m_raster = raster;
      m_screensize = screensize;
      m_formula = formula;
      m_perspective = perspective;

      availableY = 0;
      int noOfThreads = ParameterDict.Exemplar.GetInt("Computation.NoOfThreads");
      if (noOfThreads == 1) {
        Generate(act_val, zyklen, raster, screensize, formula, perspective);
        mStarter.ComputationEnds();
        return;
      }
      startCount = noOfThreads;
      PData = new PictureData(width, height);

      for (int i = 0; i < noOfThreads; i++) {
        System.Threading.ThreadStart tStart = new System.Threading.ThreadStart(Start);
        System.Threading.Thread thread = new System.Threading.Thread(tStart);
        thread.Start();
      }
    }


    /// <summary>
    /// ruft Generate(m_act_val,  m_zyklen,  m_raster,  m_screensize,  m_formula,  m_perspective) auf.
    /// </summary>
    protected void Start() {
      Generate(m_act_val, m_zyklen, m_raster, m_screensize, m_formula, m_perspective);
      lock (startCountLock) {
        startCount--;
      }

      if (startCount == 0) {
        if (mStarter != null) {
          mStarter.ComputationEnds();
        }
        // Endereignis aufrufen
      }
    }


    protected int availableY = 0;


    /// <summary>
    /// Liefert true, wenn die angegebene z-Koordinate berechnet werden darf. Wird für die Synchronisation der asynchronen Prozesse verwendet.
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    protected bool IsAvailable(int y) {
      if (mAbort)
        return false;
      bool retVal = true;
      lock (generateLock) {
        while (mPause) {
          System.Threading.Thread.Sleep(10);
          System.Windows.Forms.Application.DoEvents();
          if (mAbort)
            return false;
        }
        if (y < availableY)
          retVal = false;
        else {
          availableY = y + 1;
          Console.WriteLine("Accepted y=" + y.ToString());
          mStarter.Progress(100.0 * (y / (double)MAXZ_ITER));
          retVal = true;
        }
      }
      return retVal;
    }

    protected int MAXX_ITER = 0;
    protected int MAXY_ITER = 0;
    protected int MAXZ_ITER = 0;



    protected Formulas mLastUsedFormulas = null;

    /// <summary>
    /// Die im zuletzt gestarteten Thread verwendete Formula-Klasse kann benutzt werden, um aus den x,y,z-Raumkoordinaten
    /// die benutzten Koordinaten der mathematischen Menge zu ermitteln.
    /// </summary>
    public Formulas LastUsedFormulas {
      get {
        return mLastUsedFormulas;
      }
    }


    /// <summary>
    /// Berechung eines Einzelbildes.
    /// </summary>
    /// <param name="act_val"></param>
    /// <param name="zyklen"></param>
    /// <param name="raster"></param>
    /// <param name="screensize"></param>
    /// <param name="formula"></param>
    /// <param name="perspective"></param>
    protected void Generate(FracValues act_val, int zyklen, int raster, double screensize, int formula, bool perspective) {
      double[] col = null;
      double xd, yd, zd, zzd;
      double x, y, z, zz;

      act_val = act_val.Clone();

      Formulas formulas = new Formulas(PData);
      mLastUsedFormulas = formulas;
      if (ParameterDict.Exemplar["Intern.Formula.Source"].Trim() == "") {
        formulas.InternFormula = new Fractrace.TomoGeometry.VecRotMandel2d();
      } else {
        Fractrace.TomoGeometry.TomoFormulaFactory fac = new Fractrace.TomoGeometry.TomoFormulaFactory();
        formulas.InternFormula = fac.CreateFromString(ParameterDict.Exemplar["Intern.Formula.Source"]);
      }
      if (formulas.InternFormula == null)
        return;
      formulas.InternFormula.Init();

      Rotation rot = new Rotation();
      rot.Init();

      formulas.Transforms.Add(rot);

      if (mIsRightView) {
        RightEyeView stereoTransform = new RightEyeView();
        stereoTransform.Init();
        formulas.Transforms.Add(stereoTransform);
      }


      col = formulas.col;

      MAXX_ITER = width;
      MAXY_ITER = (int)(ParameterDict.Exemplar.GetDouble("View.Deph") * screensize);
      MAXZ_ITER = height;
      if (MAXY_ITER < 120) {
        MAXY_ITER = MAXX_ITER;
      }

      int MINX_ITER = 0;
      int MINY_ITER = 0;
      int MINZ_ITER = 0;
      double fa1;
      int xschl = 0, yschl = 0, zschl = 0, xx = 0, yy = 0;
      double wix = 0, wiy = 0, wiz = 0;
      double jx = 0, jy = 0, jz = 0, jzz = 0;

      jx = ParameterDict.Exemplar.GetDouble("Formula.Static.jx");
      jy = ParameterDict.Exemplar.GetDouble("Formula.Static.jy");
      jz = ParameterDict.Exemplar.GetDouble("Formula.Static.jz");
      jzz = ParameterDict.Exemplar.GetDouble("Formula.Static.jzz");
      act_val.end_tupel.zz = act_val.start_tupel.zz = ParameterDict.Exemplar.GetDouble("Formula.zz");

      // Innenbereich
      int minCycle = (int)ParameterDict.Exemplar.GetDouble("Formula.Static.MinCycle");
      if (minCycle == 0)
        minCycle = 100;

      // Offset für den Maximalzyklus für die klassische 2D-Darstellung 
      int cycleAdd = 1056;

      Pen p = new Pen(Color.FromArgb(100, 200, 50));
      SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0));
      int colour_type = 0;

      wix = act_val.arc.x;
      wiy = act_val.arc.y;
      wiz = act_val.arc.z;

      xd = raster * (act_val.end_tupel.x - act_val.start_tupel.x) / (MAXX_ITER - MINX_ITER);
      yd = raster * (act_val.end_tupel.y - act_val.start_tupel.y) / (MAXY_ITER - MINY_ITER);
      zd = raster * (act_val.end_tupel.z - act_val.start_tupel.z) / (MAXZ_ITER - MINZ_ITER);
      zzd = raster * (act_val.end_tupel.zz - act_val.start_tupel.zz) / (MAXZ_ITER - MINZ_ITER);

      double xcenter = (act_val.start_tupel.x + act_val.end_tupel.x) / 2.0;
      double ycenter = (act_val.start_tupel.y + act_val.end_tupel.y) / 2.0;
      double zcenter = (act_val.start_tupel.z + act_val.end_tupel.z) / 2.0;

      bool isYborder = true;

      // Projektion initialisieren und der Berechnung zuordnen:
      // TODO: Projektion über Einstellungen abwählbar machen           
      double cameraDeph = act_val.end_tupel.y - act_val.start_tupel.y;
      cameraDeph *= ParameterDict.Exemplar.GetDouble("Transformation.Perspective.Cameraposition");
      Vec3 camera = new Vec3(xcenter, act_val.end_tupel.y + cameraDeph, zcenter);
      Vec3 viewPoint = new Vec3(xcenter, act_val.end_tupel.y, zcenter);
      Projection proj = new Projection(camera, viewPoint);
      if (ParameterDict.Exemplar.GetBool("View.Perspective"))
        formulas.Projection = proj;



      // Bei der Postererstellung werden die Parameter der räumlichen Projektion auf das mittlere Bild 
      // ausgerichtet und anschließend die Grenzen verschoben
      double xPoster = ParameterDict.Exemplar.GetInt("View.PosterX");
      double zPoster = ParameterDict.Exemplar.GetInt("View.PosterZ");

      double xDiff = act_val.end_tupel.x - act_val.start_tupel.x;
      double zDiff = act_val.end_tupel.z - act_val.start_tupel.z;
      act_val.end_tupel.x += xDiff * xPoster;
      act_val.start_tupel.x += xDiff * xPoster;
      act_val.end_tupel.z += zDiff * zPoster;
      act_val.start_tupel.z += zDiff * zPoster;

      // Start der Iteration in der Reihenfolge: z,x,y (y entspricht der Tiefe)
      z = act_val.end_tupel.z + zd;
      zz = act_val.end_tupel.zz + zzd;

      for (zschl = (int)(MAXZ_ITER); zschl >= (MINZ_ITER); zschl -= raster) {

        // Nur wenn der Scheduler die Erlaubnis gibt, zschl zu benutzen,
        // die Berechnung ausführen (sonst nächste Iteration)
        if (IsAvailable(MAXZ_ITER - zschl)) {

          System.Windows.Forms.Application.DoEvents();
          z = act_val.end_tupel.z - (double)zd * (MAXZ_ITER - zschl) / (raster);
          zz = act_val.end_tupel.zz - (double)zzd * (MAXZ_ITER - zschl) / (raster);

          bool minYDetected = false;
          for (xschl = (int)(MINX_ITER); xschl <= MAXX_ITER; xschl += raster) {
            if (mAbort) {
              return;
            }

            x = act_val.start_tupel.x + (double)xd * xschl / (raster);
            double miny = 0;
            isYborder = true;
            for (yschl = (int)(MAXY_ITER); yschl >= MINY_ITER; yschl -= raster) {
              xx = xschl;
              yy = MAXZ_ITER - zschl;

              if (mAbort) {
                return;
              }

              if (xx >= 0 && xx < width && yy >= 0 && yy < height) {
                if ((GData.Picture)[xx, yy] == 0 || (GData.Picture)[xx, yy] == 2) { // aha, noch zeichnen
                  // Test, ob Schnitt mit Begrenzung vorliegt  
                  y = act_val.end_tupel.y - (double)yd * (MAXY_ITER - yschl) / (raster);

                  fa1 = 0;

                  int usedCycles = 0;
                  bool inverse = false;
                  if (GData == null) {
                    System.Diagnostics.Debug.WriteLine("Error: GData == null");
                    return;
                  }
                  if ((GData.Picture)[xx, yy] == 0)
                    usedCycles = formulas.Rechne(x, y, z, zz, zyklen,
                          wix, wiy, wiz,
                          jx, jy, jz, jzz, formula, inverse);

                  if ((GData.Picture)[xx, yy] == 2) {// Invers rechnen
                    inverse = true;
                    usedCycles = formulas.Rechne(x, y, z, zz, minCycle,
                          wix, wiy, wiz,
                          jx, jy, jz, jzz, formula, inverse);
                  }

                  if (usedCycles == 0) {
                    if (!minYDetected)
                      miny = yschl;
                    minYDetected = true;
                    // Iteration ist nicht abgebrochen, also weiterrechnen:
                    int oldPictureInfo = (GData.Picture)[xx, yy]; // pictureInfo wird eventuell zurückgesetzt, wenn 
                    // die Farbberechnung wiederholt wird.
                    (GData.Picture)[xx, yy] = 1; // Punkt als gesetzt markieren
                    VoxelInfo vInfo = new VoxelInfo();
                    GData.PointInfo[xx, yy] = vInfo;
                    vInfo.i = x;
                    vInfo.j = y;
                    vInfo.k = z;
                    vInfo.l = zz;

                    cycleAdd = 1024;
                    if (minCycle != 51 && minCycle >= 0) {
                      cycleAdd = minCycle - zyklen;
                    }
                    if (isYborder) { // es liegt Schnitt mit Begrenzung vor
                      colour_type = 0; // COL; // Farbige Darstellung

                      fa1 = formulas.Rechne(x, y, z, zz, zyklen + cycleAdd,
                       wix, wiy, wiz,
                       jx, jy, jz, jzz, formula, false);

                      if (fa1 == 0) {
                        if (minCycle != 51) {
                          fa1 = -1;
                          (GData.Picture)[xx, yy] = 2; // Punkt nicht als gesetzt markieren
                        } else {
                          fa1 = 255;
                        }
                      } else
                        fa1 = 255 * fa1 / (zyklen + cycleAdd);

                      // debug only: alle Farbwerte auf 1 setzen
                      col[0] = col[1] = col[2] = col[3] = 255;
                    } else {// innerer Punkt
                      colour_type = 1; // GREY;

                      if (inverse) {
                        if (raster == 1) {
                          fa1 = formulas.FixPoint(minCycle, x, y, z, zz,
                         xd, yd, zd, zzd,
                         wix, wiy, wiz,
                         jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                        } else {
                          fa1 = formulas.WinkelPerspective(minCycle, x, y, z, zz,
                            xd, yd, zd, zzd,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                        }
                      }
                      else {
                        if (raster == 1) {
                          fa1 = formulas.FixPoint(zyklen, x, y, z, zz,
   xd, yd, zd, zzd,
   wix, wiy, wiz,
   jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                          fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;


                        } else {
                          fa1 = formulas.WinkelPerspective(zyklen, x, y, z, zz,
                            xd, yd, zd, zzd,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                          fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                        }
                      }
                    }

                    if (raster > 2) {
                      if (colour_type == 0) {
                        if (fa1 >= 0) {
                          /*
                          p.Color = Color.White;
                          if (isXborder)
                            p.Color = Color.FromArgb((int)fa1, (int)(fa1 / 2.0), (int)(fa1 / 2.0));
                          if (isYborder)
                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1), (int)(fa1 / 2.0));
                          if (isZborder)
                            p.Color = Color.FromArgb((int)(fa1 / 2.0), (int)(fa1 / 2.0), (int)(fa1));
                          */
                          GData.ColorInfo2[xx, yy] = fa1;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx, yy, raster / 2, raster / 2);
                        }
                      } else {
                        //brush.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                        //double redMin = 1;
                        //if (inverse) {
                        //  redMin = 0.5;
                        //}
                        //p.Color = Color.FromArgb((int)(redMin * col[3]), (int)col[3], (int)col[3]);
                        //if (mStarter == null)
                        //  grLabel.FillRectangle(brush, xx, yy, raster / 2, raster / 2);

                        //p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                        //if (mStarter == null)
                        //  grLabel.FillRectangle(brush, xx + raster / 2, yy, raster, raster / 2);

                        //p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                        //if (mStarter == null)
                        //  grLabel.FillRectangle(brush, xx + raster / 2, yy + raster / 2, raster, raster);

                        //p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                        //if (mStarter == null)
                        //  grLabel.FillRectangle(brush, xx, yy + raster / 2, raster / 2, raster);
                      }

                    } else if (raster == 2) {
                      if (colour_type == 0) {
                        // Es liegt also Schnittpunkt mit dem virtuellen Bildschirm vor. In diesem Fall
                        // wird die klassische 2D Darstellung des Fraktals verwendet.
                        if (fa1 >= 0) {
                          //p.Color = Color.FromArgb(0, (int)(fa1), 0);
                          GData.ColorInfo2[xx, yy] = fa1;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx, yy, (float)0.5, (float)0.5);
                          PixelInfo pixelInfo = new PixelInfo();
                          pixelInfo.frontLight = -fa1;
                          pixelInfo.iterations = usedCycles;
                          PData.Points[xx, yy] = pixelInfo;
                          // TODO: Bessere Möglichkeit der Schneidung schaffen.

                          cycleAdd = minCycle;
                          fa1 = formulas.Rechne(x + xd / 2.0, y, z, zz, zyklen + cycleAdd,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, false);
                          pixelInfo = new PixelInfo();
                          pixelInfo.iterations = fa1;
                          if (fa1 < 1)
                            fa1 = 255;
                          else
                            fa1 = 255.0 * fa1 / (zyklen + cycleAdd);
                          //p.Color = Color.FromArgb(0, (int)(fa1), 0);
                          GData.ColorInfo2[xx + 1, yy] = fa1;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx + 1, yy, (float)0.5, (float)0.5);

                          // Debug: Querschnitt wieder einfügen
                          pixelInfo.frontLight = -fa1;
                          PData.Points[xx + 1, yy] = pixelInfo;

                          fa1 = formulas.Rechne(x, y, z - zd / 2.0, zz, zyklen + cycleAdd,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, false);
                          pixelInfo = new PixelInfo();
                          pixelInfo.iterations = fa1;
                          if (fa1 < 1)
                            fa1 = 255;
                          else
                            fa1 = 255 * fa1 / (zyklen + cycleAdd);
                          //p.Color = Color.FromArgb(0, (int)(fa1), 0);
                          GData.ColorInfo2[xx, yy + 1] = fa1;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx, yy + 1, (float)0.5, (float)0.5);
                          //pixelInfo = new PixelInfo();
                          pixelInfo.frontLight = -fa1;
                          PData.Points[xx, yy + 1] = pixelInfo;

                          fa1 = formulas.Rechne(x + xd / 2.0, y, z - zd / 2.0, zz, zyklen + cycleAdd,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, false);
                          pixelInfo = new PixelInfo();
                          pixelInfo.iterations = fa1;
                          if (fa1 < 1)
                            fa1 = 255;
                          else
                            fa1 = 255 * fa1 / (zyklen + cycleAdd);
                          //p.Color = Color.FromArgb(0, (int)(fa1), 0);
                          GData.ColorInfo2[xx + 1, yy + 1] = fa1;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx + 1, yy + 1, (float)0.5, (float)0.5);
                          //pixelInfo = new PixelInfo();
                          pixelInfo.frontLight = -fa1;
                          PData.Points[xx + 1, yy + 1] = pixelInfo;

                        } else {
                          // hier soll später weitergezeichnet werden.
                          //p.Color = Color.Blue;
                          //if (mStarter == null)
                          //  grLabel.DrawRectangle(p, xx, yy, (float)0.5, (float)0.5);
                          (GData.Picture)[xx, yy] = 2;
                        }
                      } else {
                        //double redMin = 1;
                        //if (inverse) {
                        //  redMin = 0.5;
                        //}

                        // Wenn eine der Infos null ist, nochmal in höherer Auflösung rechnen
                        //                              pixelInfo = new PixelInfo();
                        //                          pixelInfo.frontLight = -fa1;

                        /*
                        PixelInfo pInfo1 = PData.Points[xx, yy];
                        PixelInfo pInfo2 = PData.Points[xx + 1, yy];
                        PixelInfo pInfo3 = PData.Points[xx, yy + 1];
                        PixelInfo pInfo4 = PData.Points[xx + 1, yy + 1];
                        
                         // Test, ob aller 4 Einzelpunkte korrekt berechnet wurden.
                          if(pInfo1!=null && pInfo2!=null && pInfo3!=null && pInfo4!=null) { 
                        */
                        //p.Color = Color.FromArgb((int)(col[3]), (int)col[3], (int)col[3]);
                        //DrawPoint(col[3], xx, yy);
                        //double fa = 0;
                        if (PData.Points[xx, yy] != null)
                          GData.ColorInfo[xx, yy] = col[3];
                        else {
                          // 
                          GData.ColorInfo[xx, yy] = -1;
                          /*
                          fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                             minCycle,
                             x, y, z, zz,
                     0.5 * xd, yd, 0.5 * zd, zzd,
                     wix, wiy, wiz,
                     jx, jy, jz, jzz, formula, inverse, xx, yy, raster);
                          GData.ColorInfo[xx, yy] = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                           */
                        }

                        //p.Color = Color.FromArgb((int)(redMin * col[0]), (int)col[0], (int)col[0]);
                        //DrawPoint(col[0], xx + 1, yy);
                        if (PData.Points[xx + 1, yy] != null)
                          GData.ColorInfo[xx + 1, yy] = col[0];
                        else {
                          GData.ColorInfo[xx+1, yy] = -1;
                          /*
                          fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                             minCycle,
                             x + 0.5 * xd, y, z, zz,
                     0.5 * xd, yd, 0.5 * zd, zzd,
                     wix, wiy, wiz,
                     jx, jy, jz, jzz, formula, inverse, xx, yy, raster);
                          GData.ColorInfo[xx + 1, yy] = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                           */
                        }

                        //p.Color = Color.FromArgb((int)(redMin * col[1]), (int)col[1], (int)col[1]);
                        //DrawPoint(col[1], xx + 1, yy + 1);
                        if (PData.Points[xx + 1, yy + 1] != null)
                          GData.ColorInfo[xx + 1, yy + 1] = col[1];
                        else {
                          GData.ColorInfo[xx+1, yy+1] = -1;
                          /*
                          fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                             minCycle,
                             x + 0.5 * xd, y, z - 0.5 * zd, zz,
                     0.5 * xd, yd, 0.5 * zd, zzd,
                     wix, wiy, wiz,
                     jx, jy, jz, jzz, formula, inverse, xx, yy, raster);
                          GData.ColorInfo[xx + 1, yy + 1] = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                           */
                        }

                        //p.Color = Color.FromArgb((int)(redMin * col[2]), (int)col[2], (int)col[2]);
                        //DrawPoint(col[2], xx, yy + 1);
                        if (PData.Points[xx, yy + 1] != null)
                          GData.ColorInfo[xx, yy + 1] = col[2];
                        else {
                          GData.ColorInfo[xx, yy+1] = -1;
                          /*
                          fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                             minCycle,
                             x, y, z - 0.5 * zd, zz,
                     0.5 * xd, yd, 0.5 * zd, zzd,
                     wix, wiy, wiz,
                     jx, jy, jz, jzz, formula, inverse, xx, yy, raster);
                          GData.ColorInfo[xx, yy + 1] = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                           */
                        }
                        // Wenn Farbänderung zum Nachbarknoten zu groß ist, diesen Punkt neu
                        // berechnen
                          
                        /*
                  if (xx > 0 && yy > 0) {
                    double tempX = x - xd / 2.0;
                    double tempY = y;
                    double tempZ = z - zd / 2.0;
                    for (int xxi = xx; xxi <= xx + 1; xxi++, tempX += xd) {
                      for (int yyi = yy; yyi <= yy + 1; yyi++, tempZ += zd) {
                        double currentCol = GData.ColorInfo[xxi, yyi];
                        if (currentCol < 0.01) {
                          if (inverse)
                            fa1 = formulas.WinkelPerspective(minCycle, tempX, y, tempZ, zz, 
                              xd * xzDephFactor * 0.5, yd * yDephFactor, zd * xzDephFactor * 0.5, zzd,
                              wix, wiy, wiz,
                              jx, jy, jz, jzz, formula, inverse, xxi, yyi);
                          else
                            fa1 = formulas.WinkelPerspective(zyklen, tempX, y, tempZ, zz, 
                              xd * xzDephFactor, yd * yDephFactor, zd * xzDephFactor, zzd * xzDephFactor,
                              wix, wiy, wiz,
                              jx, jy, jz, jzz, formula, inverse, xxi, yyi);
                          fa1 = 0;
                          for (int i = 0; i < 4; i++) {
                            fa1 += col[i];
                          }
                          fa1 = fa1 / 4.0;
                          p.Color = Color.FromArgb((int)(fa1), (int)fa1, (int)fa1);
                          DrawPoint((int)fa1, xxi, yyi);
                        }
                      }
                    }
                  }
                        */

                        // }

                        /*
                            else {
                                // Die vier Einzelpunkte nachberechnen
                              
                                //if ((GData.Picture)[xx, yy] == 1)
                                //    (GData.Picture)[xx, yy] =0;

                                int newPictureInfo = (GData.Picture)[xx, yy];
                                (GData.Picture)[xx, yy] = oldPictureInfo; 

                                PixelInfo pInfo = new PixelInfo();
                             pInfo.frontLight=1;
                          
                             pInfo.Coord.X = x;
                             pInfo.Coord.Y = y;
                             pInfo.Coord.Z = 0;
                             
                         
                             PData.Points[xx, yy] = null;
                             PData.Points[xx + 1, yy] = null;
                             PData.Points[xx, yy + 1] = null;
                             PData.Points[xx + 1, yy + 1] = null;

                            
                                double fa=ComputeColor(formulas,act_val.start_tupel.y,act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                                    minCycle,
                                    x, y, z, zz,
                            0.25*xd , yd , 0.25*zd , zzd ,
                            wix, wiy, wiz,
                            jx, jy, jz, jzz, formula, inverse, xx, yy,raster);

                                fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                                      minCycle,
                                      x+0.5*xd, y, z-0.5*zd, zz,
                              0.25 * xd, yd, 0.25 * zd, zzd,
                              wix, wiy, wiz,
                              jx, jy, jz, jzz, formula, inverse, xx+1, yy+1, raster);

                                fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                                      minCycle,
                                      x+xd/2.0, y, z, zz,
                              0.25 * xd, yd, 0.25 * zd, zzd,
                              wix, wiy, wiz,
                              jx, jy, jz, jzz, formula, inverse, xx+1, yy, raster);

                                fa = ComputeColor(formulas, act_val.start_tupel.y, act_val.end_tupel.y, MINY_ITER, MAXY_ITER,
                                      minCycle,
                                      x, y, z-zd/2.0, zz,
                              0.25 * xd, yd, 0.25 * zd, zzd,
                              wix, wiy, wiz,
                              jx, jy, jz, jzz, formula, inverse, xx, yy+1, raster);
                            

                             //   pInfo.frontLight = fa;
                                (GData.Picture)[xx, yy] = newPictureInfo; 

                            }*/
                      }
                    } else {
                      p.Color = Color.FromArgb((int)fa1, (int)fa1, (int)fa1);
                      GData.ColorInfo2[xx, yy] = fa1;
                      //if (mStarter == null)
                      //  grLabel.DrawRectangle(p, xx, yy, 1, 1);
                    }
                  }
                }
              }
              isYborder = false;
            }
          }
        }
      }


    }



    /*
    public double ComputeColor(Formulas formulas, double yMin, double yMax, int yDimMin, int yDimMax,
        long zykl, double x, double y, double z, double zz,
        double xd, double yd, double zd, double zzd,
        double wix, double wiy, double wiz,
        double jx, double jy, double jz, double jzz, int formula, bool invers, int pixelX, int pixelY, int raster) {


      double color = 0;

      for (int yschl = (int)(yDimMax); yschl >= yDimMin; yschl -= raster) {
        y = yMax - (double)yd * (yDimMax - yschl) / (raster);
        int usedCycles = 0;
        bool inverse = false;
        if (GData == null) {
          System.Diagnostics.Debug.WriteLine("Error: GData == null");
          return 0;
        }
        if ((GData.Picture)[pixelX, pixelY] == 0)
          usedCycles = formulas.Rechne(x, y, z, zz, zykl,
              wix, wiy, wiz,
              jx, jy, jz, jzz, formula, inverse);
        if ((GData.Picture)[pixelX, pixelY] == 2) {// Invers rechnen
          inverse = true;
          usedCycles = formulas.Rechne(x, y, z, zz, zykl,
                wix, wiy, wiz,
                jx, jy, jz, jzz, formula, inverse);
        }
        if (usedCycles == 0) {

          double fa1 = 0;
          if (inverse)
            fa1 = formulas.WinkelPerspective(zykl, x, y, z, zz,
              xd, yd, zd, zzd,
              wix, wiy, wiz,
              jx, jy, jz, jzz, formula, inverse, pixelX, pixelY, false);
          else
            fa1 = formulas.WinkelPerspective(zykl, x, y, z, zz,
              xd, yd, zd, zzd,
              wix, wiy, wiz,
              jx, jy, jz, jzz, formula, inverse, pixelX, pixelY, false);
          fa1 = (formulas.col[0] + formulas.col[1] + formulas.col[2] + formulas.col[3]) / 4.0;

          color = fa1;

          return color;

        }
      }
      return color;
    }
    */

    /*
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
          double fa2 = GData.ColorInfo2[i, j];

          p.Color = Color.FromArgb((int)fa2, (int)(fa2), (int)fa2);
          grLabel.DrawRectangle(p, (float)i, (float)j, (float)0.5, (float)0.5);
        }
      }
    }


    /// <summary>
    /// Ein Punkt wird gezeichnet.
    /// </summary>
    /// <param name="col"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    protected void DrawPoint(double col, int x, int y) {
      Pen p = new Pen(Color.FromArgb(100, 200, 50));
      p.Color = Color.FromArgb((int)col, (int)col, (int)col);
      if (mStarter == null)
        grLabel.DrawRectangle(p, x, y, (float)0.5, (float)0.5);
      if (x < width && y < height)
        GData.ColorInfo2[x, y] = col;
    }
    */

  }
}
