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


    /// <summary>
    /// Unique Instance of this Window.
    /// </summary>
    public static Form1 PublicForm = null;


    /// <summary>
    /// Initializes a new instance of the <see cref="Form1"/> class.
    /// </summary>
    public Form1() {
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
    /// Heigt of the Bitmap.
    /// </summary>
    int maxy = 0;


    /// <summary>
    /// Computes the surface data of the "Gestalt". 
    /// </summary>
    Iterate iter = null;


    /// <summary>
    /// Draw the "Gestalt" in a Graphics bitmap.
    /// (not tested since 01 2010).  
    /// </summary>
    Fractrace.Compability.ClassicIterate classicIter = null;



    /// <summary>
    /// Zoom Area
    /// </summary>
    int ZoomX1 = 0,ZoomX2 = 0,ZoomY1 = 0, ZoomY2 = 0;


    /// <summary>
    /// Left mouse button is pressed.
    /// </summary>
    private bool inMouseDown = false;


    /// <summary>
    /// The Hash of the Parameters of the last rendering (but without picture art settings).
    /// </summary>
    protected string oldParameterHashWithoutPictureArt = "";


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
    /// Global Variables are set.
    /// </summary>
    protected void InitGlobalVariables() {
      GlobalParameters.SetGlobalParameters();
    }

    
    /// <summary>
    /// Sets the size of the picture box and the corresponding image from settings.
    /// </summary>
    protected void SetPictureBoxSize() {
      double widthInPixel = ParameterDict.Exemplar.GetDouble("View.Width");
      double heightInPixel = ParameterDict.Exemplar.GetDouble("View.Height");
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


    /// <summary>
    /// Load bitmap from the given file.
    /// </summary>
    /// <param name="fileName"></param>
    public void ShowPictureFromFile(string fileName) {
      if (System.IO.File.Exists(fileName)) {
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
    protected string GetParameterHashWithoutPictureArt() {
      StringBuilder tempHash = new StringBuilder();
      tempHash.Append(ParameterDict.Exemplar.GetHash("View"));
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
    /// Create surface model.
    /// </summary>
    public void ComputeOneStep() {
      if (paras != null)
        paras.InComputing = true;
      this.WindowState = FormWindowState.Normal;
      if (inComputeOneStep)
        return;
      inComputeOneStep = true;
      SetPictureBoxSize();
      if (!ParameterDict.Exemplar.GetBool("View.ClassicView")) {
        string tempParameterHash = GetParameterHashWithoutPictureArt();
        if (oldParameterHashWithoutPictureArt == tempParameterHash) {
          OneStepEnds();
        } else {
          oldParameterHashWithoutPictureArt = tempParameterHash;
          classicIter = null;
          paras.Assign();
          iter = new Iterate(maxx, maxy, this, false);
          iter.StartAsync(paras.Parameter, paras.Cycles, paras.Raster, paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"));
        }
      } else {
        iter = null;
        classicIter = new Fractrace.Compability.ClassicIterate(maxx, maxy);
        classicIter.Init(grLabel);
        classicIter.frac_iterate(paras.Parameter, paras.Cycles, paras.Raster, (int)paras.ScreenSize, paras.Formula, ParameterDict.Exemplar.GetBool("View.Perspective"));
        OneStepEnds();
      }
    }


    /// <summary>
    /// Raise the event "the asynchrone computation has ended".
    /// </summary>
    public void ComputationEnds() {
      this.Invoke(new OneStepEndsDelegate(OneStepEnds));
    }


    /// <summary>
    /// Asyncrone computation is ready (but not the generation of the bitmap).
    /// </summary>
    protected void OneStepEnds() {
      Application.DoEvents();
      this.Refresh();
      ActivatePictureArt();
      string fileName = FileSystem.Exemplar.GetFileName("pic.jpg");
      this.Text = fileName;
      pictureBox1.Image.Save(fileName);
      inComputeOneStep = false;
      if (paras != null)
        paras.InComputing = false;
    }


      /// <summary>
      /// Get internal image data.
      /// </summary>
      /// <returns></returns>
    public Image GetImage() {
        return pictureBox1.Image;
    }


    /// <summary>
    /// Get true, if the surface data generation is running. 
    /// </summary>
    public bool InComputation {
      get {
        return inComputeOneStep;
      }
    }


    /// <summary>
    /// Show parameter window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button1_Click(object sender, EventArgs e) {
      try {
        paras.Show();
      }
      catch (Exception ex) {
        System.Diagnostics.Debug.WriteLine(ex.ToString());
      }
    }


    /// <summary>
    /// Zoom is activated.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button4_Click(object sender, EventArgs e) {
      Zoom();
    }



    /// <summary>
    /// Start zooming.
    /// </summary>
    private void Zoom() {
      inZoom = true;
    }



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
        // Sorry, this Rectangle is not shown
        grLabel.DrawRectangle(pen, ZoomX1, ZoomY1, ZoomX2 - ZoomX1, ZoomY2 - ZoomY1);
      }
    }


    /// <summary>
    /// Transfer of zooming parameters ends.
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
    /// Activate zooming. Warning: not working with perspective view. 
    /// </summary>
    private void SetZoom() {
      // compute Min and Max
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

      // Updating to display the new values in the parameter window.
      paras.SetGlobalParameters();
      paras.UpdateFromData();
    }


    /// <summary>
    /// Stops the surface generation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Stop() {
      if (iter != null) {
        iter.Abort();
        iter = null;
      }
      // Warning: Compution in stereo window is not stopped.
      if (classicIter != null) {
        classicIter.Abort();
      }
    }


    /// <summary>
    /// Generates the bitmap and save it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRepaint_Click(object sender, EventArgs e) {
      ActivatePictureArt();
      // Always save picture.
      string fileName = FileSystem.Exemplar.GetFileName("pic.jpg");
      this.Text = fileName;
      pictureBox1.Image.Save(fileName);
    }


    /// <summary>
    /// The surface data is analysed and the bitmap is generated.
    /// </summary>
    private void ActivatePictureArt() {
      // TODO: Run this in a own thread.
      btnRepaint.Enabled = false;
      try {
        if (iter != null) {
          Renderer pArt = PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
          pArt.Paint(grLabel);
          Application.DoEvents();
          this.Refresh();
        }
      }
      catch (System.Exception ex) {
        MessageBox.Show(ex.ToString());
      }
      btnRepaint.Enabled = true;
    }


    /// <summary>
    /// Computation progress in percent.
    /// </summary>
    /// <param name="progressInPercent"></param>
    public void Progress(double progressInPercent) {
      if (mProgress < progressInPercent - 2 || mProgress > progressInPercent) {
        mProgress = progressInPercent;
        this.Invoke(new ProgressDelegate(OnProgress));
      }
    }


    /// <summary>
    /// The value of the progressbar is updated.
    /// </summary>
    protected void OnProgress() {
      progressBar1.Value = (int)mProgress;
    }


    /// <summary>
    /// Ends this application.
    /// </summary>
    public void ForceClosing() {
      forceClosing = true;
      this.Close();
    }


    /// <summary>
    /// Dialogabfrage vor Beendigung der Anwendung.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(CancelEventArgs e) {
      if (!forceClosing) {
        if (MessageBox.Show("Close Application?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes) {
          base.OnClosing(e);
        } else e.Cancel = true;
      }
    }


    /// <summary>
    /// Handles the Click event of the button3 control.
    /// Export surface data in 3D file (only supported format is VRML95) 
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
