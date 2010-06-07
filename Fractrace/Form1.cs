using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace {


    public partial class Form1 : Form, IAsyncComputationStarter {

        private ParameterInput paras = null;

        //   private string exportPath = "C:\\Users\\Per\\Documents\\Pic\\Frac\\";

        public static Form1 PublicForm = null;

        public Form1() {
            InitializeComponent();
            object o = FileSystem.Exemplar;
            InitGlobalVariables();
            paras = new ParameterInput();
            paras.Show();
            PublicForm = this;
        }


        /// <summary>
        /// Globale Variablen werden gesetzt (wird später aus Datei geladen)
        /// </summary>
        protected void InitGlobalVariables() {
            // ParameterDict.Exemplar["ExportPath"] = exportPath;
            ParameterDict.Exemplar["test2"] = "df";
            GlobalParameters.SetGlobalParameters();

        }


        Image labelImage = null;

        Graphics grLabel = null;

        int maxx = 0;

        int maxy = 0;

        Iterate iter = null;

        Fractrace.Compability.ClassicIterate classicIter = null;



        protected void SetPictureBoxSize() {
            //double sizeWeight = 700;

            double widthInPixel= ParameterDict.Exemplar.GetDouble("View.Width");
            double heightInPixel=  ParameterDict.Exemplar.GetDouble("View.Height");
            //ParameterDict.Exemplar["View.Deph"] = "800";


            int maxSizeX = (int)(widthInPixel * paras.ScreenSize);
            int maxSizeY = (int)(heightInPixel * paras.ScreenSize);
            if (maxx != maxSizeX || maxy != maxSizeY) {
              maxx = maxSizeX;
              maxy = maxSizeY;
                pictureBox1.Width = maxx;
                pictureBox1.Height = maxy;
                Image labelImage = new Bitmap((int)(maxx), (int)(maxy));
                pictureBox1.Image = labelImage;
                grLabel = Graphics.FromImage(labelImage);
            }
        }


        bool inComputeOneStep = false;


        /// <summary>
        /// Zeigt das angegebene Bild an.
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void ShowPictureFromFile(string fileName) {
            if (System.IO.File.Exists(fileName)) {
                SetPictureBoxSize();
                pictureBox1.Image=Image.FromFile(fileName);
                grLabel = Graphics.FromImage(pictureBox1.Image);
                //grLabel.DrawImage(
                this.Refresh();
                this.WindowState=FormWindowState.Normal;
            }
        }



        /// <summary>
        /// Erzeugt eine Bilddarstellung.
        /// </summary>
        public void ComputeOneStep() {
          if(paras!=null)
             paras.InComputing = true;
            this.WindowState = FormWindowState.Normal;
            if (inComputeOneStep)
                return;
            inComputeOneStep = true;
            SetPictureBoxSize();
            if (!ParameterDict.Exemplar.GetBool("View.ClassicView")) {
                classicIter = null;
                paras.Assign();
                iter = new Iterate(maxx, maxy, this,false);
                //Iterate.DEPHFACTOR = paras.DephFactor;
                iter.Init(grLabel);
                iter.StartAsync(paras.Parameter, paras.Cycles, paras.Raster, paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"));
            }
            else {
                iter = null;
                classicIter = new Fractrace.Compability.ClassicIterate(maxx, maxy);
                // Fractrace.Compability.ClassicIterate.DEPHFACTOR = paras.DephFactor;
                classicIter.Init(grLabel);
                classicIter.frac_iterate(paras.Parameter, paras.Cycles, paras.Raster, (int)paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"));
                OneStepEnds();
            }


        }


        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        public void ComputationEnds() {
            this.Invoke(new OneStepEndsDelegate(OneStepEnds));
            //OneStepEnds();
        }


        delegate void OneStepEndsDelegate();

        protected void OneStepEnds() {

            Application.DoEvents();
            this.Refresh();

            ActivatePictureArt();
            string fileName = FileSystem.Exemplar.GetFileName("pic.jpg");
            this.Text = fileName;
            pictureBox1.Image.Save(fileName);
            fileCount++;
            inComputeOneStep = false;
            if (paras != null)
              paras.InComputing = false;
        }


        /// <summary>
        /// Liefert true, wenn ein Berechnungsschritt durchgeführt wird. 
        /// </summary>
        public bool InComputation {
            get {
                return inComputeOneStep;
            }
        }

        private int fileCount = 10000;


        /// <summary>
        /// Anzeige und Erstellung der Parameter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            try {
                paras.Show();
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Zoom wurde aktiviert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e) {
            Zoom();
        }


        private bool inZoom = false;

        /// <summary>
        /// Zoomen wird gestartet.
        /// </summary>
        private void Zoom() {
            inZoom = true;
        }

        int ZoomX1 = 0;
        int ZoomX2 = 0;
        int ZoomY1 = 0;
        int ZoomY2 = 0;

        private bool inMouseDown = false;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            if (inZoom) {
                ZoomX1 = e.X;
                ZoomY1 = e.Y;
                inMouseDown = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) {
            if (inMouseDown) {
                ZoomX2 = e.X;
                ZoomY2 = e.Y;
                Pen pen = new Pen(Color.Black);
                grLabel.DrawRectangle(pen, ZoomX1, ZoomY1, ZoomX2 - ZoomX1, ZoomY2 - ZoomY1);
            }
        }


        /// <summary>
        /// Die Parameter der Zoomfensters wurden übermittelt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e) {
            if (inMouseDown) {
                ZoomX2 = e.X;
                ZoomY2 = e.Y;
                inZoom = false;
                inMouseDown = false;
                SetZoom();
            }
        }


        /// <summary>
        /// Zoom wird aktiviert.
        /// </summary>
        private void SetZoom() {
            // Minimum und Maximum bestimmen:
            double minX = 1000;
            double minY = 1000;
            double minZ = 1000;
            double minZZ = 1000;
            double maxX = -1000;
            double maxY = -1000;
            double maxZ = -1000;
            double maxZZ = -1000;

            double x, y, z, zz;
            for (int i = ZoomX1; i <= ZoomX2; i++) {
                for (int j = ZoomY1; j <= ZoomY2; j++) {
                    if (iter.GraphicInfo.PointInfo[i, j] != null) {
                        x = iter.GraphicInfo.PointInfo[i, j].i;
                        y = iter.GraphicInfo.PointInfo[i, j].j;
                        z = iter.GraphicInfo.PointInfo[i, j].k;
                        zz = iter.GraphicInfo.PointInfo[i, j].l;
                        if (minX > x)
                            minX = x;
                        if (maxX < x)
                            maxX = x;
                        if (minY > y)
                            minY = y;
                        if (maxY < y)
                            maxY = y;
                        if (minZ > z)
                            minZ = z;
                        if (maxZ < z)
                            maxZ = z;
                        if (minZZ > zz)
                            minZZ = zz;
                        if (maxZZ < zz)
                            maxZZ = zz;
                    }
                }
            }
            // Parameter befüllen:
            paras.Parameter.start_tupel.x = minX;
            paras.Parameter.start_tupel.y = minY;
            paras.Parameter.start_tupel.z = minZ;
            paras.Parameter.start_tupel.zz = minZZ;

            paras.Parameter.end_tupel.x = maxX;
            paras.Parameter.end_tupel.y = maxY;
            paras.Parameter.end_tupel.z = maxZ;
            paras.Parameter.end_tupel.zz = maxZZ;
            paras.SetGlobalParameters();
            paras.UpdateFromData();

        }

        private bool abortAni = false;


        /// <summary>
        /// Stop wurde gewählt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Stop() {
            if (iter != null) {
                iter.Abort();
                iter = null;
            }

         
          // Stereo fehlt noch
            if (classicIter != null) {
                classicIter.Abort();
            }

            abortAni = true;
        }


        /// <summary>
        /// Das Ergebnis wird nachgebessert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRepaint_Click(object sender, EventArgs e) {
            ActivatePictureArt();
          // Speichern
            string fileName = FileSystem.Exemplar.GetFileName("pic.jpg");
            this.Text = fileName;
            pictureBox1.Image.Save(fileName);
            fileCount++;
        }


        /// <summary>
        /// Das Ergebnis wird nachgebessert.
        /// </summary>
        private void ActivatePictureArt() {
            btnRepaint.Enabled = false;
            try {
                if (iter != null) {
                    Renderer pArt = PictureArtFactory.Create(iter.PictureData);
                    pArt.Paint(grLabel);
                    Application.DoEvents();
                    this.Refresh();
                }
            } catch (System.Exception ex) {
                MessageBox.Show(ex.ToString());
            }
            btnRepaint.Enabled = true;
        }


        protected delegate void ProgressDelegate();


        /// <summary>
        /// Fortschritt in Prozent.
        /// </summary>
        /// <param name="progressInPercent"></param>
        public void Progress(double progressInPercent) {
          if (mProgress < progressInPercent - 2 || mProgress > progressInPercent) {
            mProgress = progressInPercent;
            this.Invoke(new ProgressDelegate(OnProgress));
          }
        }


        /// <summary>
        /// Berechnungsfortschritt
        /// </summary>
        protected void OnProgress() {
            progressBar1.Value = (int)mProgress;
        }

        /// <summary>
        /// Fortschritt der Berechnung in Prozent.
        /// </summary>
        protected double mProgress = 0;


        /// <summary>
        /// Beenden ohne Nutzerabfrage.
        /// </summary>
        public void ForceClosing() {
            forceClosing = true;
            this.Close();
        }

        protected bool forceClosing = false;


        /// <summary>
        /// Dialogabfrage vor Beendigung der Anwendung.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e) {
            if (!forceClosing) {
                if (MessageBox.Show("Exit", "Really?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    base.OnClosing(e);
                }
                else e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the button3 control.
        /// Exportieren wurde ausgewählt. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button3_Click(object sender, EventArgs e) {
          SaveFileDialog sd = new SaveFileDialog();
          sd.Filter = "*.wrl|*.wrl|*.*|all";
          if (sd.ShowDialog() == DialogResult.OK) {
            X3dExporter export = new X3dExporter(iter);
            export.Save(sd.FileName);
          }
        }


    }
}
