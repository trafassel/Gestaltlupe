using Fractrace.Basic;
using Fractrace.DataTypes;
using Fractrace.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace
{
    class Iterate2d : Iterate
    {


        public Iterate2d(int width, int height, IAsyncComputationStarter starter, bool isRightView)

        : base(width, height, starter, isRightView)
        {
        }

        public Iterate2d(ParameterDict parameterDict, IAsyncComputationStarter starter, bool isRightView = false)
        {
            _parameterDict = parameterDict;
            _starter = starter;
            int quality = parameterDict.GetInt("Renderer.2D.Quality");
            _width = quality*parameterDict.GetWidth();
            _height = quality*parameterDict.GetHeight();

            _gData = new GraphicData(_width, _height);
            _pData = new PictureData(_width, _height);
            this._isRightView = isRightView;
        }

        /// <summary>
        /// Compute surface data.
        /// </summary>
        protected override void Generate(FracValues act_val, int zyklen, double screensize, int formula, bool perspective)
        {
            Random rand = new Random();
            _maxUpdateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
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

            double centerX = ParameterDict.Current.GetDouble("Scene.CenterX");
            double centerY = ParameterDict.Current.GetDouble("Scene.CenterY");
            double centerZ = ParameterDict.Current.GetDouble("Scene.CenterZ");

            Rotation rotView = new Rotation();
            rotView.Init(centerX, centerY, centerZ, ParameterDict.Current.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Current.GetDouble("Transformation.Camera.AngleY"),
                ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ"));
            formulas.Transforms.Add(rotView);


            if (_isRightView)
            {
                RightEyeView stereoTransform = new RightEyeView();
                stereoTransform.Init();
                formulas.Transforms.Add(stereoTransform);
            }

            _maxxIter = _width;
            _maxyIter = 0;
            if (IsSmallPreview() && _updateCount == 0 && _maxyIter > 1)
                _maxyIter = _maxxIter;
            _maxzIter = _height;

            int MINX_ITER = 0;
            int MINY_ITER = 0;
            int MINZ_ITER = 0;
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
            //int cycleAdd = 128;

            wix = act_val.arc.x;
            wiy = act_val.arc.y;
            wiz = act_val.arc.z;

            xd = (act_val.end_tupel.x - act_val.start_tupel.x) / (_maxxIter - MINX_ITER);
            if (_maxyIter == 0)
                yd = 0;
            else
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
            double zcenter = (act_val.start_tupel.z + act_val.end_tupel.z) / 2.0;
            bool isYborder = true;

            // Projektion initialisieren und der Berechnung zuordnen:
            // TODO: Projektion über Einstellungen abwählbar machen           
            double cameraDeph = act_val.end_tupel.y - act_val.start_tupel.y;
            cameraDeph *= ParameterDict.Current.GetDouble("Transformation.Camera.Position");
            Vec3 camera = new Vec3(xcenter, act_val.end_tupel.y + cameraDeph, zcenter);
            Vec3 viewPoint = new Vec3(xcenter, act_val.end_tupel.y, zcenter);
            Projection proj = new Projection(camera, viewPoint);
            if (!ParameterDict.Current.GetBool("Transformation.Camera.IsometricProjection"))
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

            bool computeNormals = !(IsSmallPreview() && _updateCount == 0) && _highQuality;


            for (zschl = (int)(_maxzIter); zschl >= (MINZ_ITER); zschl -= 1)
            {

                // Nur wenn der Scheduler die Erlaubnis gibt, zschl zu benutzen,
                // die Berechnung ausführen (sonst nächste Iteration)
                if (IsAvailable(_maxzIter - zschl))
                {

                    //System.Windows.Forms.Application.DoEvents();
                    z = act_val.end_tupel.z - (double)zd * (_maxzIter - zschl);

                    for (xschl = (int)(MINX_ITER); xschl <= _maxxIter; xschl += 1)
                    {
                        if (_abort)
                        {
                            return;
                        }

                        x = act_val.start_tupel.x + (double)xd * xschl;
                        isYborder = true;

                        xx = xschl;
                        yy = _maxzIter - zschl;
                        if (double.IsNaN(x))
                            break;

                        // Used for better start values in update iteration
                        double yAdd = rand.NextDouble() * yd;
                        // In last computation a voxel ist found at (xx,zz)
                        bool centerIsSet = false;
                        // In last computation at least on voxel ist found near (xx,zz)
                        bool areaIsSet = false;
                        double yAddCenter = 0;

                        bool needComputing = true;
                        

                        if (needComputing || _maxyIter == 0)
                        {
                            // yadd cannot be easy handled (because of inside rendering).
                            yschl = (int)(_maxyIter - (MINY_ITER - dephAdd) / 2);
//                            for (yschl = (int)(_maxyIter); yschl >= MINY_ITER - dephAdd; yschl -= 1)
                            {
                                if (_abort)
                                    return;

                                if (xx >= 0 && xx < _width && yy >= 0 && yy < _height)
                                {
                                    if ((_gData.Picture)[xx, yy] == 0 || (_gData.Picture)[xx, yy] == 2)
                                    { // aha, noch zeichnen
                                        // Test, ob Schnitt mit Begrenzung vorliegt  
                                        if (_maxyIter == 0)
                                            y = 0.5 * (act_val.start_tupel.y + act_val.end_tupel.y);
                                        else
                                            y = act_val.end_tupel.y - (double)yd * (_maxyIter - yschl);
                                        if (_maxyIter > 0)
                                            y += yAdd;
                                        if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z))
                                            break;

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

                                        if (usedCycles == 0)
                                        {
                                            // Iteration ist nicht abgebrochen, also weiterrechnen:
                                            //int oldPictureInfo = (_gData.Picture)[xx, yy]; // pictureInfo wird eventuell zurückgesetzt, wenn 
                                            // die Farbberechnung wiederholt wird.

                                            _gData.Picture[xx, yy] = 1; // Punkt als gesetzt markieren
                                            VoxelInfo vInfo = new VoxelInfo();
                                            _gData.PointInfo[xx, yy] = vInfo;
                                            vInfo.i = x;
                                            vInfo.j = y;
                                            vInfo.k = z;

                                            if (isYborder)
                                            {
                                                if (_maxyIter == 0)
                                                {
                                                    (_gData.Picture)[xx, yy] = 1;

                                                    _pData.Points[xx, yy] = new PixelInfo();
                                                    _pData.Points[xx, yy].Coord.X = xx;
                                                    _pData.Points[xx, yy].Coord.Y = yy;
                                                    _pData.Points[xx, yy].Coord.Z = 0;


                                                    _pData.Points[xx, yy].Coord.X = x;
                                                    _pData.Points[xx, yy].Coord.Y = y;
                                                    _pData.Points[xx, yy].Coord.Z = 0;

                                                    _pData.Points[xx, yy].AdditionalInfo = formulas.InternFormula.additionalPointInfo.Clone();
                                                }
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
        }


    }

}


