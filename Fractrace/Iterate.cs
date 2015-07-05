using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.Geometry;

namespace Fractrace
{


    /// <summary>
    /// Hier werden die Voxelpunkte abgefahren und die Berechnungsroutine aufgerufen.
    /// </summary>
    public class Iterate
    {


        GraphicData GData = null;

        PictureData PData = null;

        protected bool mAbort = false;


        /// <summary>
        /// True while running iteration.
        /// </summary>
        protected bool mStart = false;


        protected static bool mPause = false;


        /// <summary>
        /// Globales Anhalten der Berechnung.
        /// </summary>
        public static bool Pause
        {
            get
            {
                return mPause;
            }
            set
            {
                mPause = value;
            }
        }


        /// <summary>
        /// Von Außen wurde Abbruch angestoßen. 
        /// </summary>
        public void Abort()
        {
            mAbort = true;
        }


        /// <summary>
        /// Liefert true, wenn Abort aufgerufen wurde.
        /// </summary>
        public bool InAbort
        {
            get
            {
                return mAbort;
            }
        }


        /// <summary>
        /// Liefert Informationen zu den gezeichneten Pixeln.
        /// </summary>
        public GraphicData GraphicInfo
        {
            get
            {
                return GData;
            }
        }


        /// <summary>
        /// Liefert Zusatz-Informationen zu dem gezeichneten Bild
        /// </summary>
        public PictureData PictureData
        {
            get
            {
                return PData;
            }
        }


        protected int width = 0;

        protected int height = 0;


        public Iterate()
        {

        }

        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Iterate(int width, int height)
        {
            GData = new GraphicData(width, height);
            PData = new PictureData(width, height);
            this.width = width;
            this.height = height;
        }



        protected ParameterDict mParameterDict = null;

        public Iterate(ParameterDict parameterDict, IAsyncComputationStarter starter, bool isRightView=false)
        {
            mParameterDict = parameterDict;
            mStarter = starter;
            width = parameterDict.GetWidth();
            height=parameterDict.GetHeight();
            GData = new GraphicData(width, height);
            PData = new PictureData(width, height);
            this.mIsRightView = isRightView;
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
        public Iterate(int width, int height, IAsyncComputationStarter starter, bool isRightView)
        {
            mStarter = starter;
            GData = new GraphicData(width, height);
            PData = new PictureData(width, height);
            this.width = width;
            this.height = height;
            this.mIsRightView = isRightView;
        }


        /// <summary>
        /// GraphicData of previous iteration. Used for update.
        /// </summary>
        DataTypes.GraphicData mOldData = null;


        /// <summary>
        /// PictureData of previous iteration. Used for update.
        /// </summary>
        DataTypes.PictureData mOldPictureData = null;


        /// <summary>
        /// Count the number of update steps.
        /// </summary>
        int mUpdateCount = 0;


        /// <summary>
        /// Set data of the last iteration with the same rendering parameters.
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="oldPictureData"></param>
        public void SetOldData(DataTypes.GraphicData oldData, DataTypes.PictureData oldPictureData, int updateCount)
        {
            mOldData = oldData;
            mOldPictureData = oldPictureData;
            mUpdateCount = updateCount;
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
        public double CurrentStatus
        {
            get
            {
                return percent;
            }
        }


        FracValues m_act_val = null;
        int m_zyklen = 0;
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
        /// True if oldPictureDasta has bad quality 
        /// </summary>
        protected bool mTransformUpdate = false;


        /// <summary>
        /// Start asyncron computing with parameters mParameterDict.
        /// </summary>
        public void StartAsync()
        {
            if (mParameterDict == null)
            {
                System.Diagnostics.Debug.WriteLine("Error in Iterate.StartAsync() mParameterDict is empty");
                return;
            }

            FracValues fracValues = new FracValues();
            fracValues.SetFromParameterDict();
            StartAsync(fracValues, (int)ParameterDict.Exemplar.GetDouble("Formula.Static.Cycles"),
                ParameterDict.Exemplar.GetDouble("View.Size"),
                ParameterDict.Exemplar.GetInt("Formula.Static.Formula"),
                ParameterDict.Exemplar.GetBool("View.Perspective"));

        }


        /// <summary>
        /// Wait till async computation ends. 
        /// </summary>
        public void Wait()
        {
            while (mStart && !mAbort)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }
        }


        /// <summary>
        /// Von hier aus wird die Berechnung in Einzelthreads aufgesplittet.
        /// </summary>
        /// <param name="act_val"></param>
        /// <param name="zyklen"></param>
        /// <param name="raster"></param>
        /// <param name="screensize"></param>
        /// <param name="formula"></param>
        /// <param name="perspective"></param>
        public void StartAsync(FracValues act_val, int zyklen, double screensize, int formula, bool perspective)
        {
            mStart = true;
            System.Diagnostics.Debug.WriteLine("Iter start");
            m_act_val = act_val;
            m_zyklen = zyklen;
            m_screensize = screensize;
            m_formula = formula;
            m_perspective = perspective;
            availableY = 0;

            int noOfThreads = ParameterDict.Exemplar.GetInt("Computation.NoOfThreads");
            if (noOfThreads == 1)
            {
                Generate(act_val, zyklen, screensize, formula, perspective);
                mStarter.ComputationEnds();
                return;
            }
            startCount = noOfThreads;
            PData = new PictureData(width, height);
            for (int i = 0; i < noOfThreads; i++)
            {
                System.Threading.ThreadStart tStart = new System.Threading.ThreadStart(Start);
                System.Threading.Thread thread = new System.Threading.Thread(tStart);
                thread.Start();
            }
        }


        /// <summary>
        /// call Generate(m_act_val,  m_zyklen,  m_raster,  m_screensize,  m_formula,  m_perspective) auf.
        /// </summary>
        protected void Start()
        {
            Generate(m_act_val, m_zyklen, m_screensize, m_formula, m_perspective);
            lock (startCountLock)
            {
                startCount--;
            }

            if (startCount == 0)
            {
                if (mStarter != null)
                {
                    if (mAbort)
                    {
                        if (mOldData != null)
                        {
                            GData = mOldData;
                        }
                        if (mOldPictureData != null)
                        {
                            PData = mOldPictureData;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Iter ends");
                    mStart = false;
                    mStarter.ComputationEnds();
                }
                // Endereignis aufrufen
            }
        }


        protected int availableY = 0;


        /// <summary>
        /// Return true, if corresponding image is used as small preview.
        /// </summary>
        /// <returns></returns>
        protected bool IsSmallPreview()
        {
            return (width< 150 && height < 150);
        }


        /// <summary>
        /// True, if iteration runs without updates.
        /// </summary>
        public bool OneStepProgress = false;


        /// <summary>
        /// Liefert true, wenn die angegebene z-Koordinate berechnet werden darf. Wird für die Synchronisation der asynchronen Prozesse verwendet.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool IsAvailable(int y)
        {
            if (mAbort)
                return false;
            bool retVal = true;
            lock (generateLock)
            {
                while (mPause)
                {
                    System.Threading.Thread.Sleep(10);
                    System.Windows.Forms.Application.DoEvents();
                    if (mAbort)
                        return false;
                }
                if (y < availableY)
                    retVal = false;
                else
                {
                    availableY = y + 1;
                    if (maxUpdateSteps > 0 && !OneStepProgress && !IsSmallPreview() )
                    {
                        double f = ((double)mUpdateCount) / ((double)maxUpdateSteps + 1);
                        double maxUpInvers = 1.0 / ((double)maxUpdateSteps + 2);
                        if(!mAbort)
                        mStarter.Progress(100.0 * (
                            maxUpInvers * ((double)mUpdateCount + y / (double)MAXZ_ITER))
                            );


                        /*
                        double a = (double)(mUpdateCount);
                        double b = y / (double)MAXZ_ITER;
                        double c = maxUpInvers * (a + b);

                        double progress = 100.0 * (maxUpInvers * ((double)mUpdateCount) + y / (double)MAXZ_ITER);
                         */

                    }
                    else
                    {
                        if (!mAbort)
                            mStarter.Progress(100.0 * (y / (double)MAXZ_ITER));
                    }
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
        public Formulas LastUsedFormulas
        {
            get
            {
                return mLastUsedFormulas;
            }
        }


        int maxUpdateSteps = 1;


        /// <summary>
        /// Berechung eines Einzelbildes.
        /// </summary>
        /// <param name="act_val"></param>
        /// <param name="zyklen"></param>
        /// <param name="raster"></param>
        /// <param name="screensize"></param>
        /// <param name="formula"></param>
        /// <param name="perspective"></param>
        protected void Generate(FracValues act_val, int zyklen, double screensize, int formula, bool perspective)
        {
            Random rand = new Random();
            maxUpdateSteps = ParameterDict.Exemplar.GetInt("View.UpdateSteps");
            double[] col = null;
            double xd, yd, zd, zzd;
            double x, y, z, zz;
            double dephAdd = ParameterDict.Exemplar.GetInt("View.DephAdd") * screensize;
            act_val = act_val.Clone();
            Formulas formulas = new Formulas(PData);
            mLastUsedFormulas = formulas;
            if (ParameterDict.Exemplar["Intern.Formula.Source"].Trim() == "")
            {
                formulas.InternFormula = new Fractrace.TomoGeometry.VecRotMandel2d();
            }
            else
            {
                Fractrace.TomoGeometry.TomoFormulaFactory fac = new Fractrace.TomoGeometry.TomoFormulaFactory();
                formulas.InternFormula = fac.CreateFromString(ParameterDict.Exemplar["Intern.Formula.Source"]);
            }
            if (formulas.InternFormula == null)
                return;
            formulas.InternFormula.Init();

            // Umschauen
            double centerX = (ParameterDict.Exemplar.GetDouble("Border.Max.x") + ParameterDict.Exemplar.GetDouble("Border.Min.x")) / 2.0;
            double centerY = 0.5 * (ParameterDict.Exemplar.GetDouble("Border.Max.y") + ParameterDict.Exemplar.GetDouble("Border.Min.y"));
            double centerZ = (ParameterDict.Exemplar.GetDouble("Border.Max.z") + ParameterDict.Exemplar.GetDouble("Border.Min.z")) / 2.0;

            Rotation rotView = new Rotation();
            rotView.Init(centerX, centerY, centerZ, ParameterDict.Exemplar.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Exemplar.GetDouble("Transformation.Camera.AngleY"),
                ParameterDict.Exemplar.GetDouble("Transformation.Camera.AngleZ"));
            formulas.Transforms.Add(rotView);
            // ende Umschauen

            Rotation rot = new Rotation();
            rot.Init();
            formulas.Transforms.Add(rot);

            if (mIsRightView)
            {
                RightEyeView stereoTransform = new RightEyeView();
                stereoTransform.Init();
                formulas.Transforms.Add(stereoTransform);
            }

            col = formulas.col;
            MAXX_ITER = width;
            MAXY_ITER = (int)(ParameterDict.Exemplar.GetDouble("View.Deph") * screensize);
            if(IsSmallPreview())
                MAXY_ITER = MAXX_ITER;    
            MAXZ_ITER = height;
              
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
                minCycle = zyklen;

            // Offset für den Maximalzyklus für die klassische 2D-Darstellung 
            int cycleAdd = 128;
            int colour_type = 0;

            wix = act_val.arc.x;
            wiy = act_val.arc.y;
            wiz = act_val.arc.z;

            //raster = 1;

            xd = (act_val.end_tupel.x - act_val.start_tupel.x) / (MAXX_ITER - MINX_ITER);
            yd = (act_val.end_tupel.y - act_val.start_tupel.y) / (MAXY_ITER - MINY_ITER);
            zd = (act_val.end_tupel.z - act_val.start_tupel.z) / (MAXZ_ITER - MINZ_ITER);
            zzd = (act_val.end_tupel.zz - act_val.start_tupel.zz) / (MAXZ_ITER - MINZ_ITER);

            if (mOldData != null)
            {
                yd = yd / (mUpdateCount);
                if (mUpdateCount < 5)
                {
                    MAXY_ITER *= mUpdateCount;
                }
            }
            if (mTransformUpdate)
            {
                yd *= 3.0;
            }


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

            for (zschl = (int)(MAXZ_ITER); zschl >= (MINZ_ITER); zschl -= 1)
            {

                // Nur wenn der Scheduler die Erlaubnis gibt, zschl zu benutzen,
                // die Berechnung ausführen (sonst nächste Iteration)
                if (IsAvailable(MAXZ_ITER - zschl))
                {

                    System.Windows.Forms.Application.DoEvents();
                    z = act_val.end_tupel.z - (double)zd * (MAXZ_ITER - zschl);
                    zz = act_val.end_tupel.zz - (double)zzd * (MAXZ_ITER - zschl);

                    bool minYDetected = false;
                    for (xschl = (int)(MINX_ITER); xschl <= MAXX_ITER; xschl += 1)
                    {
                        if (mAbort)
                        {
                            return;
                        }

                        x = act_val.start_tupel.x + (double)xd * xschl;
                        double miny = 0;
                        isYborder = true;

                        xx = xschl;
                        yy = MAXZ_ITER - zschl;
                        if (double.IsNaN(x) )
                            return ;

                        // Used for better start values in update iteration
                        double yAdd = rand.NextDouble() * yd;
                        // In last computation a voxel ist found at (xx,zz)
                        bool centerIsSet = false;
                        // In last computation at least on voxel ist found near (xx,zz)
                        bool areaIsSet = false;
                        double yAddCenter = 0;

                        bool needComputing = true;
                        if (mOldPictureData != null)
                        {
                            needComputing = false;
                            PixelInfo pxInfoTest = mOldPictureData.Points[xx, yy];
                            if (pxInfoTest != null && pxInfoTest.Coord != null)
                            {
                                yAddCenter = pxInfoTest.Coord.Y;
                                yAdd = yAddCenter;
                                centerIsSet = true;
                            }

                            for (int xxi = -1; xxi <= 1; xxi++)
                            {
                                for (int yyi = -1; yyi <= 1; yyi++)
                                {
                                    int xxposi = xx + xxi;
                                    int yyposi = yy + yyi;
                                    if (xxposi >= 0 && xxposi <= MAXX_ITER && yyposi >= 0 && yyposi <= MAXZ_ITER)
                                    {
                                        PixelInfo pxInfo = mOldPictureData.Points[xxposi, yyposi];
                                        if (pxInfo != null && pxInfo.Coord != null)
                                        {
                                            areaIsSet = true;
                                            double yAddTemp = pxInfo.Coord.Y;
                                            if (yAdd < yAddTemp || !centerIsSet)
                                                yAdd = yAddTemp;
                                        }
                                    }
                                }
                            }
                        }

                        if (yAdd + yd < act_val.end_tupel.y)
                        {
                            if (centerIsSet)
                            {
                                if (yAddCenter + 4.0 * yd < yAdd)
                                {
                                    needComputing = true;
                                    yAdd = yAdd - act_val.end_tupel.y + 2.0 * ((double)mUpdateCount) * yd + rand.NextDouble() * yd;
                                    GData.Picture[xx, yy] = mOldData.Picture[xx, yy];
                                }
                            }
                            else
                            {
                                if (areaIsSet)
                                {
                                    needComputing = true;
                                    yAdd = rand.NextDouble() * yd;
                                }
                            }
                        }

                        if (needComputing)
                        {
                            // yadd cannot be easy handled (because of inside rendering).
                            // yAdd = rand.NextDouble() * yd;
                            for (yschl = (int)(MAXY_ITER); yschl >= MINY_ITER - dephAdd; yschl -= 1)
                            {

                                if (mAbort)
                                {
                                    return;
                                }

                                if (xx >= 0 && xx < width && yy >= 0 && yy < height)
                                {
                                    if ((GData.Picture)[xx, yy] == 0 || (GData.Picture)[xx, yy] == 2)
                                    { // aha, noch zeichnen
                                        // Test, ob Schnitt mit Begrenzung vorliegt  
                                        y = act_val.end_tupel.y - (double)yd * (MAXY_ITER - yschl);
                                        y += yAdd;
                                        if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z))
                                            return;

                                        fa1 = 0;

                                        int usedCycles = 0;
                                        bool inverse = false;
                                        if (GData == null)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Error: GData == null");
                                            return;
                                        }
                                        if ((GData.Picture)[xx, yy] == 0)
                                            usedCycles = formulas.Rechne(x, y, z, zz, zyklen,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);

                                        if ((GData.Picture)[xx, yy] == 2)
                                        {// Invers rechnen
                                            inverse = true;
                                            usedCycles = formulas.Rechne(x, y, z, zz, minCycle,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);
                                        }

                                        if (usedCycles == 0)
                                        {
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
                                            if (minCycle != 51 && minCycle >= 0)
                                            {
                                                cycleAdd = minCycle - zyklen;
                                            }
                                            if (isYborder)
                                            { // es liegt Schnitt mit Begrenzung vor
                                                colour_type = 0; // COL; // Farbige Darstellung

                                                fa1 = formulas.Rechne(x, y, z, zz, zyklen + cycleAdd,
                                                 wix, wiy, wiz,
                                                 jx, jy, jz, jzz, formula, false);

                                                if (fa1 == 0)
                                                {
                                                    if (minCycle != 51)
                                                    {
                                                        fa1 = -1;
                                                        (GData.Picture)[xx, yy] = 2; // Punkt nicht als gesetzt markieren
                                                    }
                                                    else
                                                    {
                                                        fa1 = 255;
                                                    }
                                                }
                                                else
                                                    fa1 = 255 * fa1 / (zyklen + cycleAdd);

                                                // debug only: alle Farbwerte auf 1 setzen
                                                col[0] = col[1] = col[2] = col[3] = 255;
                                            }
                                            else
                                            {// innerer Punkt
                                                colour_type = 1; // GREY;

                                                if (inverse)
                                                {
                                                    
                                                        if (IsSmallPreview())
                                                        {
                                                            fa1 = formulas.RayCastAt(minCycle, x, y, z, zz,
                                                           xd, yd, zd, zzd,
                                                           wix, wiy, wiz,
                                                           jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                        else
                                                        {
                                                            fa1 = formulas.FixPoint(minCycle, x, y, z, zz,
                                                           xd, yd, zd, zzd,
                                                           wix, wiy, wiz,
                                                           jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                    
                                                    
                                                }
                                                else
                                                {
                                                    

                                                        if (IsSmallPreview())
                                                        {
                                                            fa1 = formulas.RayCastAt(zyklen, x, y, z, zz,
                                   xd, yd, zd, zzd,
                                   wix, wiy, wiz,
                                   jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                        else
                                                        {
                                                            fa1 = formulas.FixPoint(zyklen, x, y, z, zz,
                                     xd, yd, zd, zzd,
                                     wix, wiy, wiz,
                                     jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                            fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                                                        }

                                                  
                                                   
                                                }
                                            }

                                            
                                           
                                                GData.ColorInfo2[xx, yy] = fa1;
                                          
                                        }
                                    }
                                }
                                isYborder = false;
                            }
                            if ((GData.Picture)[xx, yy] == 0 || (GData.Picture)[xx, yy] == 2)
                            {
                                if (mOldPictureData != null)
                                {
                                    PData.Points[xx, yy] = mOldPictureData.Points[xx, yy];
                                    GData.ColorInfo2[xx, yy] = mOldData.ColorInfo2[xx, yy];
                                }
                            }


                            if (mOldData != null && mUpdateCount > 2)
                            {
                                if (mOldPictureData.Points[xx, yy] != null)
                                {
                                    if (PData.Points[xx, yy] == null)
                                    {
                                        PData.Points[xx, yy] = mOldPictureData.Points[xx, yy];
                                    }
                                    else
                                    {
                                        if (PData.Points[xx, yy].Coord.Y < mOldPictureData.Points[xx, yy].Coord.Y)
                                        {
                                            PData.Points[xx, yy] = mOldPictureData.Points[xx, yy];
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Get the old values:
                            PData.Points[xx, yy] = mOldPictureData.Points[xx, yy];
                            GData.ColorInfo2[xx, yy] = mOldData.ColorInfo2[xx, yy];
                        }
                    }
                }
            }

        }


    }
}
