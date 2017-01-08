using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


using Fractrace.Basic;
using Fractrace.DataTypes;

namespace Fractrace
{
    public partial class RenderImage : UserControl, IAsyncComputationStarter
    {
        public RenderImage()
        {
            InitializeComponent();
            Init();
        }

        protected Graphics _graphics = null;

        /// <summary>
        /// Public access to the internal iterate object to reuse the iteration results.
        /// </summary>
        public Iterate Iterate  {  get   {  return _iterate; }  }
        protected Iterate _iterate = null;
        protected object _iterateMutex = new object();

        int _maxx = 0;

        int _maxy = 0;

        protected bool _isRightView = false;

        public bool IsRightView  {  get {   return _isRightView;  }  set  {   _isRightView = value;  } }

        protected PictureBox _pictureBox = null;

        /// <summary>
        /// Zugriff auf die Bearbeitungsparameter.
        /// </summary>
        protected FracValues _parameter = new FracValues();

        /// <summary>
        /// Gibt an, ob zur Zeit gezeichnet wird.
        /// </summary>
        protected bool _inDrawing = false;
        protected object _inDrawingMutex = new object();

        /// <summary>
        /// True, wenn von außen das Neuzeichnen aktiviert wurde.
        /// Das bedeutet, nach der aktuellen Zeichnung ist neuzuzeichnen.
        /// </summary>
        protected bool _forceRedraw = false;

        /// <summary>
        /// smallPreviewCurrentDrawStep == 0 : iter(width/2,height/2) , FastPreviewRenderer
        /// smallPreviewCurrentDrawStep == 1 : (iter running)
        /// smallPreviewCurrentDrawStep == 2 : FastPreviewRenderer
        /// smallPreviewCurrentDrawStep == 3 : FastPreviewRenderer complete
        /// </summary>
        protected int _smallPreviewCurrentDrawStep = 0;

        protected object _smallPreviewCurrentDrawStepMutex = new object();
        public int SmallPreviewCurrentDrawStep { get { return _smallPreviewCurrentDrawStep;  } }

        /// <summary>
        /// if fixedRenderer != -1 renderer to use for creating the bitmap.
        /// </summary>
        protected int _fixedRenderer = -1;

        /// <summary>
        /// Fortschritt der Berechnung in Prozent.
        /// </summary>
        protected double _progress = 0;
        public double GetProgress()
            {
                return _progress;
            }

        protected delegate void ProgressDelegate();

        public delegate void OneStepEndsDelegate();

        /// <summary>
        /// While computation this value is set to true.
        /// </summary>
        public bool InDrawing { get { return _inDrawing; } }

        /// <summary>
        /// Der Graphik-Kontext wird initialisiert.
        /// </summary>
        protected virtual void Init()
        {
            _pictureBox = new PictureBox();
            this.panel2.Controls.Add(_pictureBox);
        }


        // Kill all running renderíng and picture art processing.
        public void Abort()
        {
            lock (_iterateMutex)
            {
                if (_iterate != null)
                {
                    if (_iterate.Running)
                        _iterate.Abort();
                }
            }
        }


        protected void SetPictureBoxSize()
        {
            double widthInPixel = ParameterDict.Current.GetDouble("View.Width");
            double heightInPixel = ParameterDict.Current.GetDouble("View.Height");
            int maxSizeX = (int)(widthInPixel * ParameterDict.Current.GetDouble("View.Size"));
            int maxSizeY = (int)(heightInPixel * ParameterDict.Current.GetDouble("View.Size"));
            if (_maxx != maxSizeX || _maxy != maxSizeY)
            {
                _maxx = maxSizeX;
                _maxy = maxSizeY;
                _pictureBox.Width = _maxx;
                _pictureBox.Height = _maxy;
                Image labelImage = new Bitmap((int)(_maxx), (int)(_maxy));
                _pictureBox.Image = labelImage;
                _graphics = Graphics.FromImage(labelImage);
            }
        }


        /// <summary>
        /// Neuzeichnen.
        /// </summary>
        protected virtual void StartDrawing()
        {
            _forceRedraw = false;
            lock (_inDrawingMutex)
                _inDrawing = true;
            System.Diagnostics.Debug.WriteLine("_inDrawing = true (6)");
            SetPictureBoxSize();
            _iterate = new Iterate(_maxx, _maxy, this, IsRightView);
            AssignParameters();
            _iterate.StartAsync(_parameter,
                    ParameterDict.Current.GetInt("Formula.Static.Cycles"),
                    1,
                    ParameterDict.Current.GetBool("Formula.Static.Julia"),
                    !ParameterDict.Current.GetBool("Transformation.Camera.IsometricProjection"));
        }


        /// <summary>
        /// Parameters are set from ParameterDict.
        /// </summary>
        protected virtual void AssignParameters()
        {
            _parameter.SetFromParameterDict();
        }


        /// <summary>
        /// Create a draw image.
        /// </summary>
        public virtual void Draw()
        {
            System.Diagnostics.Debug.WriteLine("Draw() _inDrawing=" + _inDrawing.ToString());
            lock (_smallPreviewCurrentDrawStepMutex)
                _smallPreviewCurrentDrawStep = 0;
            _fixedRenderer = -1;
            if (!_inDrawing)
                StartDrawing();
            else
            {
                if (_iterate != null)
                {
                    _iterate.Abort();
                }
                _iterate = null;
                _forceRedraw = true;
            }
        }

        public void ActivatePictureArt()
        {
            try
            {
                if (_iterate != null && !_iterate.InAbort)
                {                    
                    OneStepEnds();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Paint image with fixed renderer and reuse an iterate object after computation. 
        /// </summary>
        public virtual void Redraw(Iterate otherIterate, int renderer)
        {
            _fixedRenderer = renderer;
            _iterate = otherIterate;
            OneStepEnds();
        }


        /// <summary>
        /// Fortschritt in Prozent.
        /// </summary>
        /// <param name="progressInPercent"></param>
        public virtual void Progress(double progressInPercent)
        {
            if (_progress < progressInPercent - 2 || _progress > progressInPercent)
            {
                _progress = progressInPercent;
                // TODO: Testen, ob Invoke die Ausführung verlangsamt
                this.Invoke(new ProgressDelegate(OnProgress));
            }
        }


        protected void OnProgress()
        {
            if (_progress >= 0 && _progress <= 100)
                progressBar1.Value = (int)_progress;

        }


        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        public virtual void ComputationEnds()
        {
            if(_iterate==null || !_iterate.InAbort)
              this.Invoke(new OneStepEndsDelegate(OneStepEnds));
        }


        /// <summary>
        /// Berechnung wurde beendet.
        /// </summary>
        protected virtual void OneStepEnds()
        {
            if (_iterate != null)
            {
                Fractrace.PictureArt.Renderer pArt;
                if (_fixedRenderer == -1)
                    pArt = PictureArt.PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas, ParameterDict.Current.Clone());
                else
                    pArt = new PictureArt.FrontViewRenderer(_iterate.PictureData);
                pArt.Paint(_graphics);
                Application.DoEvents();
                this.Refresh();
            }
            lock (_inDrawingMutex)
                _inDrawing = false;
            System.Diagnostics.Debug.WriteLine("_inDrawing = false (7)");
            if (_forceRedraw)
                StartDrawing();
        }


    }
}
