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


        GraphicData _gData = null;

        PictureData _pData = null;

        protected bool _abort = false;


        /// <summary>
        /// True while running iteration.
        /// </summary>
        protected bool _start = false;


        /// <summary>
        /// Globales Anhalten der Berechnung.
        /// </summary>
        public static bool Pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
            }
        }
        protected static bool _pause = false;


        /// <summary>
        /// Von Außen wurde Abbruch angestoßen. 
        /// </summary>
        public void Abort()
        {
            _abort = true;
        }


        /// <summary>
        /// Liefert true, wenn Abort aufgerufen wurde.
        /// </summary>
        public bool InAbort
        {
            get
            {
                return _abort;
            }
        }


        /// <summary>
        /// Liefert Informationen zu den gezeichneten Pixeln.
        /// </summary>
        public GraphicData GraphicInfo
        {
            get
            {
                return _gData;
            }
        }


        /// <summary>
        /// Liefert Zusatz-Informationen zu dem gezeichneten Bild
        /// </summary>
        public PictureData PictureData
        {
            get
            {
                return _pData;
            }
        }





        public int Width
        {
            get
            {
                return _width;
            }
        }
        protected int _width = 0;




        public int Height {  get  {  return _height; } }
        protected int _height = 0;


        public Iterate()
        {

        }

        /// <summary>
        /// Initialisation
        /// </summary>
        public Iterate(int width, int height)
        {
            _gData = new GraphicData(width, height);
            _pData = new PictureData(width, height);
            this._width = width;
            this._height = height;
        }



        protected ParameterDict _parameterDict = null;

        public Iterate(ParameterDict parameterDict, IAsyncComputationStarter starter, bool isRightView=false)
        {
            _parameterDict = parameterDict;
            _starter = starter;
            _width = parameterDict.GetWidth();
            _height=parameterDict.GetHeight();
            _gData = new GraphicData(_width, _height);
            _pData = new PictureData(_width, _height);
            this._isRightView = isRightView;
        }


        /// <summary>
        /// Control which startet this iteration.
        /// </summary>
        IAsyncComputationStarter _starter = null;


        /// <summary>
        /// Initialisation
        /// </summary>
        public Iterate(int width, int height, IAsyncComputationStarter starter, bool isRightView)
        {
            _starter = starter;
            _gData = new GraphicData(width, height);
            _pData = new PictureData(width, height);
            this._width = width;
            this._height = height;
            this._isRightView = isRightView;
        }


        /// <summary>
        /// GraphicData of previous iteration. Used for update.
        /// </summary>
        DataTypes.GraphicData _oldData = null;


        /// <summary>
        /// PictureData of previous iteration. Used for update.
        /// </summary>
        DataTypes.PictureData _oldPictureData = null;


        /// <summary>
        /// Count the number of update steps.
        /// </summary>
        int _updateCount = 0;

        protected int _availableY = 0;

        /// <summary>
        /// Set data of the last iteration with the same rendering parameters.
        /// </summary>
        /// <param name="oldData"></param>
        /// <param name="oldPictureData"></param>
        public void SetOldData(DataTypes.GraphicData oldData, DataTypes.PictureData oldPictureData, int updateCount)
        {
            _oldData = oldData;
            _oldPictureData = oldPictureData;
            _updateCount = updateCount;
        }


        /// <summary>
        /// Wird bei der Stereoansicht verwendet. Hier wird unterschieden, ob
        /// es sich um eine Sicht vom rechten Auge handelt. 
        /// </summary>
        protected bool _isRightView = false;



        /// <summary>
        /// Liefert den Status der Berechnung (von 0-100)
        /// </summary>
        double _percent = 0;


        /// <summary>
        /// Liefert den Status der Berechnung (von 0-100)
        /// </summary>
        /// 
        public double CurrentStatus
        {
            get
            {
                return _percent;
            }
        }


        FracValues _actVal = null;
        int _cycles = 0;
        double _screensize = 0;
        int _formula = 0;
        bool _perspective = true;


        /// <summary>
        /// Lock IsAvailable().
        /// </summary>
        protected object _generateLock = new object();

        protected object _startCountLock = new object();

        protected int _startCount = 0;

        /// <summary>
        /// True if oldPictureDasta has bad quality 
        /// </summary>
        protected bool _transformUpdate = false;

        /// <summary>
        /// True, if iteration runs without updates.
        /// </summary>
        public bool _oneStepProgress = false;

        protected int _maxxIter = 0;
        protected int _maxyIter = 0;
        protected int _maxzIter = 0;

        /// <summary>
        /// Die im zuletzt gestarteten Thread verwendete Formula-Klasse kann benutzt werden, um aus den x,y,z-Raumkoordinaten
        /// die benutzten Koordinaten der mathematischen Menge zu ermitteln.
        /// </summary>
        public Formulas LastUsedFormulas { get { return _lastUsedFormulas; } }
        protected Formulas _lastUsedFormulas = null;

        int _maxUpdateSteps = 1;


        /// <summary>
        /// Start asyncron computing with parameters mParameterDict.
        /// </summary>
        public void StartAsync()
        {
            if (_parameterDict == null)
            {
                System.Diagnostics.Debug.WriteLine("Error in Iterate.StartAsync() mParameterDict is empty");
                return;
            }

            FracValues fracValues = new FracValues();
            fracValues.SetFromParameterDict();
            StartAsync(fracValues, (int)ParameterDict.Current.GetDouble("Formula.Static.Cycles"),
                ParameterDict.Current.GetDouble("View.Size"),
                ParameterDict.Current.GetInt("Formula.Static.Formula"),
                ParameterDict.Current.GetBool("View.Perspective"));
        }


        /// <summary>
        /// Wait till async computation ends. 
        /// </summary>
        public void Wait()
        {
            while (_start && !_abort)
            {
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }
        }


        /// <summary>
        /// Split computing in threads.
        /// </summary>
        public void StartAsync(FracValues act_val, int zyklen, double screensize, int formula, bool perspective)
        {
            _start = true;
            System.Diagnostics.Debug.WriteLine("Iter start");
            _actVal = act_val;
            _cycles = zyklen;
            _screensize = screensize;
            _formula = formula;
            _perspective = perspective;
            _availableY = 0;

            int noOfThreads = ParameterDict.Current.GetInt("Computation.NoOfThreads");
            if (noOfThreads == 1)
            {
                Generate(act_val, zyklen, screensize, formula, perspective);
                _starter.ComputationEnds();
                return;
            }
            _startCount = noOfThreads;
            _pData = new PictureData(_width, _height);
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
            Generate(_actVal, _cycles, _screensize, _formula, _perspective);
            lock (_startCountLock)
            {
                _startCount--;
            }

            if (_startCount == 0)
            {
                if (_starter != null)
                {
                    if (_abort)
                    {
                        if (_oldData != null)
                        {
                            _gData = _oldData;
                        }
                        if (_oldPictureData != null)
                        {
                            _pData = _oldPictureData;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Iter ends");
                    _start = false;
                    _starter.ComputationEnds();
                }
            }
        }


        /// <summary>
        /// Return true, if corresponding image is used as small preview.
        /// </summary>
        /// <returns></returns>
        protected bool IsSmallPreview()
        {
            return (_width< 150 && _height < 150);
        }


        /// <summary>
        /// Liefert true, wenn die angegebene z-Koordinate berechnet werden darf. Wird für die Synchronisation der asynchronen Prozesse verwendet.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        protected bool IsAvailable(int y)
        {
            if (_abort)
                return false;
            bool retVal = true;
            lock (_generateLock)
            {
                while (_pause)
                {
                    System.Threading.Thread.Sleep(10);
                    System.Windows.Forms.Application.DoEvents();
                    if (_abort)
                        return false;
                }
                if (y < _availableY)
                    retVal = false;
                else
                {
                    _availableY = y + 1;
                    if (_maxUpdateSteps > 0 && !_oneStepProgress && !IsSmallPreview() )
                    {
                        double f = ((double)_updateCount) / ((double)_maxUpdateSteps + 1);
                        double maxUpInvers = 1.0 / ((double)_maxUpdateSteps + 2);
                        if(!_abort)
                        _starter.Progress(100.0 * (
                            maxUpInvers * ((double)_updateCount + y / (double)_maxzIter))
                            );
                    }
                    else
                    {
                        if (!_abort)
                            _starter.Progress(100.0 * (y / (double)_maxzIter));
                    }
                    retVal = true;
                }
            }
            return retVal;
        }


        /// <summary>
        /// Compute surface data.
        /// </summary>
        protected void Generate(FracValues act_val, int zyklen, double screensize, int formula, bool perspective)
        {
            Random rand = new Random();
            _maxUpdateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
            double[] col = null;
            double xd, yd, zd;
            double x, y, z;
            double dephAdd = ParameterDict.Current.GetInt("View.DephAdd") * screensize;
            act_val = act_val.Clone();
            Formulas formulas = new Formulas(_pData);
            _lastUsedFormulas = formulas;
            if (ParameterDict.Current["Intern.Formula.Source"].Trim() == "")
            {
                formulas.InternFormula = new Fractrace.TomoGeometry.VecRotMandel2d();
            }
            else
            {
                Fractrace.TomoGeometry.TomoFormulaFactory fac = new Fractrace.TomoGeometry.TomoFormulaFactory();
                formulas.InternFormula = fac.CreateFromString(ParameterDict.Current["Intern.Formula.Source"]);
            }
            if (formulas.InternFormula == null)
                return;
            formulas.InternFormula.Init();

            // Umschauen
            double centerX = (ParameterDict.Current.GetDouble("Border.Max.x") + ParameterDict.Current.GetDouble("Border.Min.x")) / 2.0;
            double centerY = 0.5 * (ParameterDict.Current.GetDouble("Border.Max.y") + ParameterDict.Current.GetDouble("Border.Min.y"));
            double centerZ = (ParameterDict.Current.GetDouble("Border.Max.z") + ParameterDict.Current.GetDouble("Border.Min.z")) / 2.0;

            Rotation rotView = new Rotation();
            rotView.Init(centerX, centerY, centerZ, ParameterDict.Current.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Current.GetDouble("Transformation.Camera.AngleY"),
                ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ"));
            formulas.Transforms.Add(rotView);
            // ende Umschauen

            // TODO: only use in compatibility mode.
            Rotation rot = new Rotation();
            rot.Init();
            formulas.Transforms.Add(rot);

            if (_isRightView)
            {
                RightEyeView stereoTransform = new RightEyeView();
                stereoTransform.Init();
                formulas.Transforms.Add(stereoTransform);
            }

            col = formulas.col;
            _maxxIter = _width;
            _maxyIter = (int)(ParameterDict.Current.GetDouble("View.Deph") * screensize);
            if(IsSmallPreview())
                _maxyIter = _maxxIter;    
            _maxzIter = _height;
              
            int MINX_ITER = 0;
            int MINY_ITER = 0;
            int MINZ_ITER = 0;
            double fa1;
            int xschl = 0, yschl = 0, zschl = 0, xx = 0, yy = 0;
            double wix = 0, wiy = 0, wiz = 0;
            double jx = 0, jy = 0, jz = 0, jzz = 0;

            jx = ParameterDict.Current.GetDouble("Formula.Static.jx");
            jy = ParameterDict.Current.GetDouble("Formula.Static.jy");
            jz = ParameterDict.Current.GetDouble("Formula.Static.jz");
            jzz = ParameterDict.Current.GetDouble("Formula.Static.jzz");

            // Innenbereich
            int minCycle = (int)ParameterDict.Current.GetDouble("Formula.Static.MinCycle");
            if (minCycle == 0)
                minCycle = zyklen;

            // Offset für den Maximalzyklus für die klassische 2D-Darstellung 
            int cycleAdd = 128;

            wix = act_val.arc.x;
            wiy = act_val.arc.y;
            wiz = act_val.arc.z;

            xd = (act_val.end_tupel.x - act_val.start_tupel.x) / (_maxxIter - MINX_ITER);
            yd = (act_val.end_tupel.y - act_val.start_tupel.y) / (_maxyIter - MINY_ITER);
            zd = (act_val.end_tupel.z - act_val.start_tupel.z) / (_maxzIter - MINZ_ITER);
           
            if (_oldData != null)
            {
                yd = yd / (_updateCount);
                if (_updateCount < 5)
                {
                    _maxyIter *= _updateCount;
                }
            }
            if (_transformUpdate)
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
            cameraDeph *= ParameterDict.Current.GetDouble("Transformation.Perspective.Cameraposition");
            Vec3 camera = new Vec3(xcenter, act_val.end_tupel.y + cameraDeph, zcenter);
            Vec3 viewPoint = new Vec3(xcenter, act_val.end_tupel.y, zcenter);
            Projection proj = new Projection(camera, viewPoint);
            if (ParameterDict.Current.GetBool("View.Perspective"))
                formulas.Projection = proj;

            // Bei der Postererstellung werden die Parameter der räumlichen Projektion auf das mittlere Bild 
            // ausgerichtet und anschließend die Grenzen verschoben
            double xPoster = ParameterDict.Current.GetInt("View.PosterX");
            double zPoster = ParameterDict.Current.GetInt("View.PosterZ");

            double xDiff = act_val.end_tupel.x - act_val.start_tupel.x;
            double zDiff = act_val.end_tupel.z - act_val.start_tupel.z;
            act_val.end_tupel.x += xDiff * xPoster;
            act_val.start_tupel.x += xDiff * xPoster;
            act_val.end_tupel.z += zDiff * zPoster;
            act_val.start_tupel.z += zDiff * zPoster;

            // Start der Iteration in der Reihenfolge: z,x,y (y entspricht der Tiefe)
            z = act_val.end_tupel.z + zd;

            for (zschl = (int)(_maxzIter); zschl >= (MINZ_ITER); zschl -= 1)
            {

                // Nur wenn der Scheduler die Erlaubnis gibt, zschl zu benutzen,
                // die Berechnung ausführen (sonst nächste Iteration)
                if (IsAvailable(_maxzIter - zschl))
                {

                    System.Windows.Forms.Application.DoEvents();
                    z = act_val.end_tupel.z - (double)zd * (_maxzIter - zschl);

                    bool minYDetected = false;
                    for (xschl = (int)(MINX_ITER); xschl <= _maxxIter; xschl += 1)
                    {
                        if (_abort)
                        {
                            return;
                        }

                        x = act_val.start_tupel.x + (double)xd * xschl;
                        double miny = 0;
                        isYborder = true;

                        xx = xschl;
                        yy = _maxzIter - zschl;
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
                        if (_oldPictureData != null)
                        {
                            needComputing = false;
                            PixelInfo pxInfoTest = _oldPictureData.Points[xx, yy];
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
                                    if (xxposi >= 0 && xxposi <= _maxxIter && yyposi >= 0 && yyposi <= _maxzIter)
                                    {
                                        PixelInfo pxInfo = _oldPictureData.Points[xxposi, yyposi];
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
                                    yAdd = yAdd - act_val.end_tupel.y + 2.0 * ((double)_updateCount) * yd + rand.NextDouble() * yd;
                                    _gData.Picture[xx, yy] = _oldData.Picture[xx, yy];
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
                            for (yschl = (int)(_maxyIter); yschl >= MINY_ITER - dephAdd; yschl -= 1)
                            {
                                if (_abort)
                                    return;

                                if (xx >= 0 && xx < _width && yy >= 0 && yy < _height)
                                {
                                    if ((_gData.Picture)[xx, yy] == 0 || (_gData.Picture)[xx, yy] == 2)
                                    { // aha, noch zeichnen
                                        // Test, ob Schnitt mit Begrenzung vorliegt  
                                        y = act_val.end_tupel.y - (double)yd * (_maxyIter - yschl);
                                        y += yAdd;
                                        if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z))
                                            return;

                                        fa1 = 0;

                                        int usedCycles = 0;
                                        bool inverse = false;
                                        if (_gData == null)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Error: GData == null");
                                            return;
                                        }
                                        if ((_gData.Picture)[xx, yy] == 0)
                                            usedCycles = formulas.Rechne(x, y, z, 0, zyklen,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);

                                        if ((_gData.Picture)[xx, yy] == 2)
                                        {// Inverse computing
                                            inverse = true;
                                            usedCycles = formulas.Rechne(x, y, z, 0, minCycle,
                                                  wix, wiy, wiz,
                                                  jx, jy, jz, jzz, formula, inverse);
                                        }

                                        if (usedCycles == 0)
                                        {
                                            if (!minYDetected)
                                                miny = yschl;
                                            minYDetected = true;
                                            // Iteration ist nicht abgebrochen, also weiterrechnen:
                                            int oldPictureInfo = (_gData.Picture)[xx, yy]; // pictureInfo wird eventuell zurückgesetzt, wenn 
                                            // die Farbberechnung wiederholt wird.
                                            (_gData.Picture)[xx, yy] = 1; // Punkt als gesetzt markieren
                                            VoxelInfo vInfo = new VoxelInfo();
                                            _gData.PointInfo[xx, yy] = vInfo;
                                            vInfo.i = x;
                                            vInfo.j = y;
                                            vInfo.k = z;

                                            cycleAdd = 1024;
                                            if (minCycle >= 0)
                                            {
                                                cycleAdd = minCycle - zyklen;
                                            }
                                            if (isYborder)
                                            { // es liegt Schnitt mit Begrenzung vor

                                                fa1 = formulas.Rechne(x, y, z, 0, zyklen + cycleAdd,
                                                 wix, wiy, wiz,
                                                 jx, jy, jz, jzz, formula, false);

                                                if (fa1 == 0)
                                                {
                                                    fa1 = -1;
                                                    (_gData.Picture)[xx, yy] = 2; // Punkt nicht als gesetzt markieren
                                                }
                                                else
                                                    fa1 = 255 * fa1 / (zyklen + cycleAdd);

                                                // debug only: alle Farbwerte auf 1 setzen
                                                col[0] = col[1] = col[2] = col[3] = 255;
                                            }
                                            else
                                            {// innerer Punkt

                                                if (inverse)
                                                {
                                                    
                                                        if (IsSmallPreview())
                                                        {
                                                            fa1 = formulas.RayCastAt(minCycle, x, y, z, 0,
                                                           xd, yd, zd, 0,
                                                           wix, wiy, wiz,
                                                           jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                        else
                                                        {
                                                            fa1 = formulas.FixPoint(minCycle, x, y, z, 0,
                                                           xd, yd, zd, 0,
                                                           wix, wiy, wiz,
                                                           jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                }
                                                else
                                                {
                                                        if (IsSmallPreview())
                                                        {
                                                            fa1 = formulas.RayCastAt(zyklen, x, y, z, 0,
                                   xd, yd, zd, 0,
                                   wix, wiy, wiz,
                                   jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                        }
                                                        else
                                                        {
                                                            fa1 = formulas.FixPoint(zyklen, x, y, z, 0,
                                     xd, yd, zd, 0,
                                     wix, wiy, wiz,
                                     jx, jy, jz, jzz, formula, inverse, xx, yy, true);
                                                            fa1 = (col[0] + col[1] + col[2] + col[3]) / 4.0;
                                                        } 
                                                }
                                            }
                                        }
                                    }
                                }
                                isYborder = false;
                            }
                            if ((_gData.Picture)[xx, yy] == 0 || (_gData.Picture)[xx, yy] == 2)
                            {
                                if (_oldPictureData != null)
                                {
                                    _pData.Points[xx, yy] = _oldPictureData.Points[xx, yy];
                                }
                            }

                            if (_oldData != null && _updateCount > 2)
                            {
                                if (_oldPictureData.Points[xx, yy] != null)
                                {
                                    if (_pData.Points[xx, yy] == null)
                                    {
                                        _pData.Points[xx, yy] = _oldPictureData.Points[xx, yy];
                                    }
                                    else
                                    {
                                        if (_pData.Points[xx, yy].Coord.Y < _oldPictureData.Points[xx, yy].Coord.Y)
                                        {
                                            _pData.Points[xx, yy] = _oldPictureData.Points[xx, yy];
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Get the old values:
                            _pData.Points[xx, yy] = _oldPictureData.Points[xx, yy];
                        }
                    }
                }
            }
        }


    }
}
