using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace
{

    /// <summary>
    /// Control which displays the rendered image and one additional tool button to redraw the picture.
    /// </summary>
    public partial class Form1 : Form, IAsyncComputationStarter
    {


        /// <summary>
        /// The input control.
        /// </summary>
        private ParameterInput paras = null;


        /// <summary>
        /// Unique Instance of this Window.
        /// </summary>
        public static Form1 PublicForm = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            object o = FileSystem.Exemplar;
            InitGlobalVariables();
            paras = new ParameterInput();
            paras.Show();
            PublicForm = this;
        }


        /// <summary>
        /// Indicates, that the application is computing a bitmap of the "Gestalt". 
        /// </summary>
        bool inComputeOneStep = false;


        /// <summary>
        /// This graphic is used to generate the bitmap. 
        /// </summary>
        Graphics grLabel = null;


        /// <summary>
        /// With of the bitmap.
        /// </summary>
        int maxx = 0;


        /// <summary>
        /// Heigth of the Bitmap.
        /// </summary>
        int maxy = 0;


        /// <summary>
        /// Computes the surface data of the "Gestalt". 
        /// </summary>
        Iterate iter = null;


        /// <summary>
        /// Zoom Area
        /// </summary>
        int ZoomX1 = 0, ZoomX2 = 0, ZoomY1 = 0, ZoomY2 = 0;


        /// <summary>
        /// Left mouse button is pressed.
        /// </summary>
        private bool inMouseDown = false;


        /// <summary>
        /// The Hash of the Parameters of the last rendering (but without picture art settings).
        /// </summary>
        protected string oldParameterHashWithoutPictureArt = "";


        /// <summary>
        /// The Hash of the Parameters of the last rendering (but without picture art settings and navigation).
        /// </summary>
        protected string oldParameterHashWithoutPictureArtAndNavigation = "";


        /// <summary>
        /// Delegate for the OneStepEnds event.
        /// </summary>
        delegate void OneStepEndsDelegate();


        /// <summary>
        /// Delegate for updating the progress bar.
        /// </summary>
        protected delegate void ProgressDelegate();


        /// <summary>
        /// If false, a warning message is shown before closing the application.
        /// </summary>
        protected bool forceClosing = false;


        /// <summary>
        /// Indicates, that zooming is enabled.
        /// </summary>
        private bool inZoom = false;


        /// <summary>
        /// Progress of surface computation in percent.
        /// </summary>
        protected double mProgress = 0;


        /// <summary>
        /// Progress of picture art in percent.
        /// </summary>
        protected double mSubProgress = 0;


        /// <summary>
        /// Used to fix inPaint while updating.
        /// </summary>
        object paintMutex = new object();


        /// <summary>
        /// True while method Paint() runs.
        /// </summary>
        bool inPaint = false;


        /// <summary>
        /// True, if after end of DrawPicture() a new paint request should be startet. 
        /// </summary>
        bool repaintRequested = false;


        /// <summary>
        /// Global Variables are set.
        /// </summary>
        protected void InitGlobalVariables()
        {
            GlobalParameters.SetGlobalParameters();
        }


        /// <summary>
        /// Sets the size of the picture box and the corresponding image from settings.
        /// </summary>
        protected void SetPictureBoxSize()
        {
            double widthInPixel = ParameterDict.Exemplar.GetDouble("View.Width");
            double heightInPixel = ParameterDict.Exemplar.GetDouble("View.Height");
            int maxSizeX = (int)(widthInPixel * paras.ScreenSize);
            int maxSizeY = (int)(heightInPixel * paras.ScreenSize);
            if (maxx != maxSizeX || maxy != maxSizeY)
            {
                maxx = maxSizeX;
                maxy = maxSizeY;
                pictureBox1.Width = maxx;
                pictureBox1.Height = maxy;
                Image labelImage = new Bitmap((int)(maxx), (int)(maxy));
                pictureBox1.Image = labelImage;
                grLabel = Graphics.FromImage(labelImage);
            }
        }


        /// <summary>
        /// Load bitmap from the given file.
        /// </summary>
        /// <param name="fileName"></param>
        public void ShowPictureFromFile(string fileName)
        {
            if (System.IO.File.Exists(fileName))
            {
                SetPictureBoxSize();
                pictureBox1.Image = Image.FromFile(fileName);
                grLabel = Graphics.FromImage(pictureBox1.Image);
                this.Refresh();
                this.WindowState = FormWindowState.Normal;
            }
        }


        /// <summary>
        /// Computes the hash of all Parameter entries which are used in 
        /// rendering, but not in picture art.
        /// </summary>
        /// <returns></returns>
        protected string GetParameterHashWithoutPictureArt()
        {
            StringBuilder tempHash = new StringBuilder();

            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Raster"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Size"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Perspective"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Width"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Height"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Deph"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.DephAdd"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.PosterX"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.PosterZ"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Border"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Transformation"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Formula"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Intern.Formula"));
            // The following categories are not used 
            // Composite
            // Computation.NoOfThreads
            return tempHash.ToString();
        }


        /// <summary>
        /// Parameterhash ohne PictureArt und ohne Navigationsänderung
        /// </summary>
        /// <returns></returns>
        protected string GetParameterHashWithoutPictureArtAndNavigation()
        {
            StringBuilder tempHash = new StringBuilder();

            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Raster"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Size"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Perspective"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Width"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Height"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.Deph"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.DephAdd"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.PosterX"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("View.PosterZ"));
            //          tempHash.Append(ParameterDict.Exemplar.GetHash("Border"));
            //tempHash.Append(ParameterDict.Exemplar.GetHash("Transformation"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Formula"));
            tempHash.Append(ParameterDict.Exemplar.GetHash("Intern.Formula"));
            // The following categories are not used 
            // Composite
            // Computation.NoOfThreads
            return tempHash.ToString();
        }


        /// <summary>
        /// Count the number of update steps
        /// </summary>
        int mUpdateCount = 0;


        /// <summary>
        /// Get the number of pictures, which was updated on the same dataset.
        /// </summary>
        public int CurrentUpdateStep
        {

            set
            {
                mCurrentUpdateStep = value;
            }

            get
            {
                return mCurrentUpdateStep;
            }

        }


        /// <summary>
        /// Count the number of pictures, wich was updated on the same dataset.
        /// </summary>
        int mCurrentUpdateStep = 0;


        /// <summary>
        /// Different handling of Progress Bar while in preview.
        /// </summary>
        public bool inPreview = false;


        /// <summary>
        /// Create surface model.
        /// </summary>
        public void ComputeOneStep()
        {
            if (paras != null)
                paras.InComputing = true;
            this.WindowState = FormWindowState.Normal;
            if (inComputeOneStep)
                return;
            inComputeOneStep = true;
            SetPictureBoxSize();
            string tempParameterHash = GetParameterHashWithoutPictureArt();
            paras.Assign();

            if (oldParameterHashWithoutPictureArt == tempParameterHash)
            {
                // Update last render for better quality
                mCurrentUpdateStep++;
                oldParameterHashWithoutPictureArt = tempParameterHash;
                DataTypes.GraphicData oldData = null;
                DataTypes.PictureData oldPictureData = null;
                if (iter != null && !iter.InAbort)
                {
                    oldData = iter.GraphicInfo;
                    oldPictureData = iter.PictureData;
                }
                iter = new Iterate(maxx, maxy, this, false);
                mUpdateCount++;
                iter.SetOldData(oldData, oldPictureData, mUpdateCount);
                if (!ParameterDict.Exemplar.GetBool("View.Pipeline.UpdatePreview"))
                    iter.OneStepProgress = inPreview;
                else
                    iter.OneStepProgress = false;
                if (mUpdateCount > ParameterDict.Exemplar.GetDouble("View.UpdateSteps") + 1)
                    iter.OneStepProgress = true;
                iter.StartAsync(paras.Parameter, paras.Cycles, paras.Raster, paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"), false);
            }
            else
            {
                {
                    mCurrentUpdateStep = 0;
                    oldParameterHashWithoutPictureArt = tempParameterHash;
                    paras.Assign();
                    mUpdateCount = 1;
                    iter = new Iterate(maxx, maxy, this, false);
                    //   iter.OneStepProgress = inPreview;
                    iter.OneStepProgress = false;
                    iter.StartAsync(paras.Parameter, paras.Cycles, paras.Raster, paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"), false);
                }
            }
        }


        /// <summary>
        /// Raise the event "the asynchrone computation has ended".
        /// </summary>
        public void ComputationEnds()
        {
//            this.Invoke(new OneStepEndsDelegate(OneStepEnds));
            OneStepEnds();
        }


        /// <summary>
        /// Use by animation control to deactivate rendering outputs in low quality.
        /// </summary>
        public bool dontActivateRender = false;


        /// <summary>
        /// Asyncrone computation is ready (but not the generation of the bitmap).
        /// </summary>
        protected void OneStepEnds()
        {
            if (!dontActivateRender)
            {
                ActivatePictureArt();
                /*
                string fileName = FileSystem.Exemplar.GetFileName("pic.png");
                this.Text = fileName;
                pictureBox1.Image.Save(fileName);
                 */
            }
            inComputeOneStep = false;
            //if (paras != null)
            //    paras.InComputing = false;
        }


        /// <summary>
        /// Get internal image data.
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return pictureBox1.Image;
        }


        /// <summary>
        /// Get true, if the surface data generation is running. 
        /// </summary>
        public bool InComputation
        {
            get
            {
                return inComputeOneStep;
            }
        }


        /// <summary>
        /// Show parameter window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                paras.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Zoom is activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Zoom();
        }



        /// <summary>
        /// Start zooming.
        /// </summary>
        private void Zoom()
        {
            inZoom = true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (inZoom)
            {
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
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (inMouseDown)
            {
                ZoomX2 = e.X;
                ZoomY2 = e.Y;
                Pen pen = new Pen(Color.Black);
                // Sorry, this Rectangle is not shown
                grLabel.DrawRectangle(pen, ZoomX1, ZoomY1, ZoomX2 - ZoomX1, ZoomY2 - ZoomY1);
            }
        }


        /// <summary>
        /// Transfer of zooming parameters ends.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (inMouseDown)
            {
                ZoomX2 = e.X;
                ZoomY2 = e.Y;
                inZoom = false;
                inMouseDown = false;
                SetZoom();
            }
        }


        /// <summary>
        /// Activate zooming.
        /// </summary>
        private void SetZoom()
        {
            if (iter == null)
            {
                return;
            }
            // compute Min and Max
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            double minZZ = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double maxZ = double.MinValue;
            double maxZZ = double.MinValue;

            if (ZoomX2 - ZoomX1 < 4)
            {
                ZoomX1 -= maxx / 10;
                ZoomX2 += maxx / 10;
                if (ZoomX1 < 0)
                    ZoomX1 = 0;
                if (ZoomX2 >= maxx)
                    ZoomX2 = maxx - 1;
                ZoomY1 -= maxy / 10;
                ZoomY2 += maxy / 10;
                if (ZoomY1 < 0)
                    ZoomY1 = 0;
                if (ZoomY2 >= maxy)
                    ZoomY2 = maxy - 1;
            }

            //  iter.PictureData.Points
            double x, y, z, zz;
            for (int i = ZoomX1; i <= ZoomX2; i++)
            {
                for (int j = ZoomY1; j <= ZoomY2; j++)
                {
                    if (iter.GraphicInfo.PointInfo[i, j] != null)
                    {
                        x = iter.GraphicInfo.PointInfo[i, j].i;
                        y = iter.GraphicInfo.PointInfo[i, j].j;
                        z = iter.GraphicInfo.PointInfo[i, j].k;
                        zz = iter.GraphicInfo.PointInfo[i, j].l;

                        Geometry.Vec3 trans2 = iter.LastUsedFormulas.GetTransform(x, y, z);
                        x = trans2.X;
                        y = trans2.Y;
                        z = trans2.Z;

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

            // Set parameters:
            paras.Parameter.start_tupel.x = minX;
            paras.Parameter.start_tupel.y = minY;
            paras.Parameter.start_tupel.z = minZ;
            paras.Parameter.start_tupel.zz = minZZ;

            paras.Parameter.end_tupel.x = maxX;
            paras.Parameter.end_tupel.y = maxY;
            paras.Parameter.end_tupel.z = maxZ;
            paras.Parameter.end_tupel.zz = maxZZ;

            // Updating to display the new values in the parameter window.
            paras.SetGlobalParameters();
            paras.UpdateFromData();
            Fractrace.Geometry.Navigator.SetAspectRatio();

            // TODO: draw small Preview 

            ParameterInput.MainParameterInput.DrawSmallPreview();
        }


        /// <summary>
        /// Stops the surface generation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Stop()
        {

            mCurrentUpdateStep = ParameterDict.Exemplar.GetInt("View.UpdateSteps") + 1;
            if (iter != null)
            {
                iter.Abort();
                //iter = null;
            }
            // Warning: Compution in stereo window is not stopped.
        }


        /// <summary>
        /// Generates the bitmap and save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRepaint_Click(object sender, EventArgs e)
        {
            ActivatePictureArt();
        }


        /// <summary>
        /// The surface data is analysed. The generation of the corresponding bitmap starts here .
        /// </summary>
        private void ActivatePictureArt()
        {
            try
            {
                if (iter != null && !iter.InAbort)
                {
                    System.Threading.ThreadStart tStart = new System.Threading.ThreadStart(DrawPicture);
                    System.Threading.Thread thread = new System.Threading.Thread(tStart);
                    thread.Start();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        Renderer currentPicturArt = null;


        /// <summary>
        /// Create PictureArt and use it to paint the current picture according to iter.
        /// </summary>
        void DrawPicture()
        {
            lock (paintMutex)
            {
                if (inPaint)
                {
                    if (currentPicturArt == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Error in DrawPicture() currentPicturArt == null");
                    }
                    currentPicturArt.StopAndWait();
                    currentPicturArt = null;
                }
                inPaint = true;                
            }

            try
            {
                if (currentPicturArt != null)
                {
                    System.Diagnostics.Debug.WriteLine("Error in DrawPicture() currentPicturArt != null");
                }
                currentPicturArt = PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
                currentPicturArt.Paint(grLabel);
                while (repaintRequested)
                {
                    currentPicturArt = PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
                    currentPicturArt.Paint(grLabel);
                }
                currentPicturArt = null;
                repaintRequested = false;
                drawEnds();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            lock (paintMutex)
            {
                inPaint = false;
            }
        }


        private void drawEnds()
        {
            needUpdate = true;
        }


        /// <summary>
        /// Computation progress in percent.
        /// </summary>
        /// <param name="progressInPercent"></param>
        public void Progress(double progressInPercent)
        {
            if (progressInPercent > 0 && progressInPercent < 100)
            {
                mProgress = progressInPercent;
                this.Invoke(new ProgressDelegate(OnProgress));
            }
        }


        /// <summary>
        /// The value of the progressbar is updated.
        /// </summary>
        protected void OnProgress()
        {
            progressBar1.Value = (int)mProgress;
        }


        /// <summary>
        /// Ends this application.
        /// </summary>
        public void ForceClosing()
        {
            forceClosing = true;
            this.Close();
        }


        /// <summary>
        /// Dialogabfrage vor Beendigung der Anwendung.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!forceClosing)
            {
                if (MessageBox.Show("Close Application?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    base.OnClosing(e);
                }
                else e.Cancel = true;
            }
        }


        /// <summary>
        /// Handles the Click event of the button3 control.
        /// Export surface data in 3D file (only supported format is VRML95) 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.wrl|*.wrl|*.*|all";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                X3dExporter export = new X3dExporter(iter);
                export.Save(sd.FileName);
            }
        }

        private bool needUpdate = false;



        /// <summary>
        /// Try to update in background (still with some sync problems).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(inPaint)
                btnRepaint.Enabled = false;

            if (needUpdate)
            {
                needUpdate = false;
                this.Refresh();
                string fileName = FileSystem.Exemplar.GetFileName("pic.png");
                this.Text = fileName;
                pictureBox1.Image.Save(fileName);
                btnRepaint.Enabled = true;
                if (paras != null)
                  paras.InComputing = false;
            }
        }


    


    }
}
