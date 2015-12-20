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

        /// <summary>
        /// True, wenn von außen das Neuzeichnen aktiviert wurde.
        /// Das bedeutet, nach der aktuellen Zeichnung ist neuzuzeichnen.
        /// </summary>
        protected bool _forceRedraw = false;

        /// <summary>
        /// smallPreviewCurrentDrawStep == 0 : iter(width/2,height/2) , FastPreviewRenderer
        /// smallPreviewCurrentDrawStep == 1 : FastPreviewRenderer  
        /// </summary>
        protected int _smallPreviewCurrentDrawStep = 0;

        /// <summary>
        /// if fixedRenderer != -1 renderer to use for creating the bitmap.
        /// </summary>
        protected int _fixedRenderer = -1;

        /// <summary>
        /// Fortschritt der Berechnung in Prozent.
        /// </summary>
        protected double _progress = 0;

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


        /// <summary>
        /// Berechnung wird abgebrochen:
        /// </summary>
        public void Abort()
        {
            if (_iterate != null)
            {
                _iterate.Abort();
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
            _inDrawing = true;
            SetPictureBoxSize();
            _iterate = new Iterate(_maxx, _maxy, this, IsRightView);
            AssignParameters();
            _iterate.StartAsync(_parameter,
                    ParameterDict.Current.GetInt("Formula.Static.Cycles"),
                    1,
                    ParameterDict.Current.GetInt("Formula.Static.Formula"),
                    ParameterDict.Current.GetBool("View.Perspective"));
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
            _smallPreviewCurrentDrawStep = 1;
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


        /// <summary>
        /// Paint image with fixed renderer and reuse an iterate object after computation. 
        /// </summary>
        /// <param name="otherIterate"></param>
        /// <param name="renderer"></param>
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
                    pArt = PictureArt.PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas);
                else
                    pArt = new PictureArt.FrontViewRenderer(_iterate.PictureData);
                pArt.Paint(_graphics);
                Application.DoEvents();
                this.Refresh();
                // In instance of RenderImage is used in the big stereo
                // and in the small preview display. Which variant is used is (clumsy) detected
                // by the picture size.
                if (_pictureBox.Image.Width > 400)
                {
                    string fileName = FileSystem.Exemplar.GetFileName("stereo_pic_right.png");
                    this.Text = fileName;
                    _pictureBox.Image.Save(fileName);
                }
            }
            _inDrawing = false;
            if (_forceRedraw)
                StartDrawing();
        }


    }
}
