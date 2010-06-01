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
            iter = new Iterate(maxx, maxy, this);
            iter.Init(grLabel);
            AssignParameters();
            iter.StartAsync(mParameter,
                    ParameterDict.Exemplar.GetInt("Formula.Static.Cycles"),
                    2, 
                    1,
                    ParameterDict.Exemplar.GetInt("Formula.Static.Formula"),
                    ParameterDict.Exemplar.GetBool("View.Perspective"));

        }


        protected virtual void AssignParameters() {
            mParameter.SetFromParameterDict();
            if (isRightView) {
                double eyePos = 0.2; // Das rechte Auge wird um 0.2 Einheiten nach rechts verschoben
                // Der Blickpunkt wird etwas nach rechts verschoben
                double xd = mParameter.end_tupel.x - mParameter.start_tupel.x;
                mParameter.end_tupel.x += eyePos * xd;
                mParameter.start_tupel.x += eyePos * xd;
            }
        }

       

        /// <summary>
        /// Zeichnen wird von außen angestoßen.
        /// </summary>
        public virtual void Draw() {
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
               Fractrace.PictureArt.Renderer pArt = PictureArt.PictureArtFactory.Create(iter.PictureData);
                pArt.Paint(grLabel);
                Application.DoEvents();
                this.Refresh();
                if (mPictureBox.Image.Width > 400) {
                    string fileName = FileSystem.Exemplar.GetFileName("stereo_pic_right.jpg");
                    this.Text = fileName;
                    mPictureBox.Image.Save(fileName);
                }
            }
            //btnPreview.Enabled = true;
            inDrawing = false;
            if (forceRedraw)
                StartDrawing();
        }

      


    }
}
