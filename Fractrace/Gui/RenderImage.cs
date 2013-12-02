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

        protected Image labelImage = null;

        protected Graphics grLabel = null;


        /// <summary>
        /// Berechnung wird abgebrochen:
        /// </summary>
        public void Abort() {
          if (iter != null) {
            iter.Abort();
          }
        }


        /// <summary>
        /// Public access to the internal iterate object to reuse the iteration results.
        /// </summary>
        public Iterate Iterate {
            get {
                return iter;
            }
        }

        protected Iterate iter = null;

        int maxx = 0;

        int maxy = 0;

        protected bool isRightView = false;

        public bool IsRightView {
            get {
                return isRightView;
            }

            set {
                isRightView = value;
            }
        }

        /// <summary>
        /// Der Graphik-Kontext wird initialisiert.
        /// </summary>
        protected virtual void Init() {
            mPictureBox = new PictureBox();
            this.panel2.Controls.Add(mPictureBox);
        }



        protected PictureBox mPictureBox = null;


        protected void SetPictureBoxSize() {
          double widthInPixel = ParameterDict.Exemplar.GetDouble("View.Width");
          double heightInPixel = ParameterDict.Exemplar.GetDouble("View.Height");
          //ParameterDict.Exemplar["View.Deph"] = "800";


          int maxSizeX = (int)(widthInPixel * ParameterDict.Exemplar.GetDouble("View.Size"));
          int maxSizeY = (int)(heightInPixel * ParameterDict.Exemplar.GetDouble("View.Size"));
          if (maxx != maxSizeX || maxy != maxSizeY) {
            maxx = maxSizeX;
            maxy = maxSizeY;
            mPictureBox.Width = maxx;
            mPictureBox.Height = maxy;
            Image labelImage = new Bitmap((int)(maxx), (int)(maxy));
            mPictureBox.Image = labelImage;
            grLabel = Graphics.FromImage(labelImage);
          }

        }


        /// <summary>
        /// Zugriff auf die Bearbeitungsparameter.
        /// </summary>
        protected FracValues mParameter = new FracValues();

        /// <summary>
        /// Gibt an, ob zur Zeit gezeichnet wird.
        /// </summary>
        protected bool inDrawing = false;

        /// <summary>
        /// True, wenn von außen das Neuzeichnen aktiviert wurde.
        /// Das bedeutet, nach der aktuellen Zeichnung ist neuzuzeichnen.
        /// </summary>
        protected bool forceRedraw = false;

        /// <summary>
        /// Neuzeichnen.
        /// </summary>
        protected virtual void StartDrawing() {
            forceRedraw = false;
            inDrawing = true;
            SetPictureBoxSize();
            iter = new Iterate(maxx, maxy, this,IsRightView);
            AssignParameters();
            iter.StartAsync(mParameter,
                    ParameterDict.Exemplar.GetInt("Formula.Static.Cycles"),
                    2, 
                    1,
                    ParameterDict.Exemplar.GetInt("Formula.Static.Formula"),
                    ParameterDict.Exemplar.GetBool("View.Perspective"),false);

        }


        /// <summary>
        /// Parameters are set from ParameterDict.
        /// </summary>
        protected virtual void AssignParameters() {
            mParameter.SetFromParameterDict();
        }


        /// <summary>
        /// Create an draw image.
        /// </summary>
        public virtual void Draw() {
            fixedRenderer = -1;
            if (!inDrawing)
                StartDrawing();
            else {
                if (iter != null) {
                    iter.Abort();
                }
                iter = null;
                forceRedraw = true;
            }
        }


        /// <summary>
        /// if fixedRenderer != -1 renderer to use for creating the bitmap.
        /// </summary>
        protected int fixedRenderer = -1;


        /*
        /// <summary>
        /// Paint image with fixed renderer
        /// </summary>
        public virtual void Draw(int renderer) {
            fixedRenderer = renderer;
            if (!inDrawing)
                StartDrawing();
            else {
                if (iter != null) {
                    iter.Abort();
                }
                iter = null;
                forceRedraw = true;
            }
        }
         */


        /// <summary>
        /// Paint image with fixed renderer and reuse an iterate object after computation. 
        /// </summary>
        /// <param name="otherIterate"></param>
        /// <param name="renderer"></param>
        public virtual void Redraw(Iterate otherIterate, int renderer) {
            fixedRenderer = renderer;
            iter = otherIterate;
            OneStepEnds();
        }


        /// <summary>
        /// Fortschritt in Prozent.
        /// </summary>
        /// <param name="progressInPercent"></param>
        public void Progress(double progressInPercent) {
          if (mProgress < progressInPercent - 2 || mProgress > progressInPercent) {
            mProgress = progressInPercent;
            // TODO: Testen, ob Invoke die Ausführung verlangsamt
            this.Invoke(new ProgressDelegate(OnProgress));
          }
        }


        protected delegate void ProgressDelegate();

        
        protected void OnProgress() {
          if(mProgress>=0 && mProgress<=100)
            progressBar1.Value = (int)mProgress;

        }

        /// <summary>
        /// Fortschritt der Berechnung in Prozent.
        /// </summary>
        protected double mProgress = 0;



        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        public void ComputationEnds() {
            this.Invoke(new OneStepEndsDelegate(OneStepEnds));
        }


        delegate void OneStepEndsDelegate();


        /// <summary>
        /// Berechnung wurde beendet.
        /// </summary>
        protected virtual void OneStepEnds() {
            if (iter != null) {
                Fractrace.PictureArt.Renderer pArt;
                if (fixedRenderer == -1)
                    pArt = PictureArt.PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
                else
                    pArt = new PictureArt.FrontViewRenderer(iter.PictureData);
                pArt.Paint(grLabel);
                Application.DoEvents();
                this.Refresh();
              // In instance of RenderImage is used in the big stereo
              // and in the small preview display. Which variant is used is (clumsy) detected
              // by the picture size.
                if (mPictureBox.Image.Width > 400) { 
                    string fileName = FileSystem.Exemplar.GetFileName("stereo_pic_right.jpg");
                    this.Text = fileName;
                    mPictureBox.Image.Save(fileName);
                }
            }
            inDrawing = false;
            if (forceRedraw)
                StartDrawing();
        }


        /// <summary>
        /// While computation this value is set to true.
        /// </summary>
        public bool InDrawing {
            get {
                return inDrawing;
            }
        }


    }
}
