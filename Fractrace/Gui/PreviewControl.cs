using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;
using Fractrace.DataTypes;

namespace Fractrace {

  public delegate void PictureRenderingIsReady();


    /// <summary>
    /// Control which displays the redered image.
    /// </summary>
  public class PreviewControl : RenderImage {


    protected System.Windows.Forms.Button btnPreview;


    /// <summary>
    /// Der Graphik-Kontext wird initialisiert.
    /// </summary>
    protected override void Init() {
      this.btnPreview = new System.Windows.Forms.Button();
      this.panel2.Controls.Add(this.btnPreview);
      this.btnPreview.Dock = System.Windows.Forms.DockStyle.Fill;
      this.btnPreview.Location = new System.Drawing.Point(0, 0);
      this.btnPreview.Name = "btnPreview";
      this.btnPreview.Size = new System.Drawing.Size(50, 40);
      this.btnPreview.TabIndex = 1;
      this.btnPreview.UseVisualStyleBackColor = true;
      //this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
      Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
      btnPreview.BackgroundImage = labelImage;
      grLabel = Graphics.FromImage(labelImage);
    }

    public Button PreviewButton {
      get {
        return this.btnPreview;
      }
    }


    /// <summary>
    /// Clear the image.
    /// </summary>
    public void Clear() {


        /*
        if (mPictureBox == null) {
            this.BackColor = Color.Red;
            return;
        }


         //   Init();
         */




        //Image labelImage = new Bitmap((int)(Width), (int)(Height));
        //mPictureBox.Image = labelImage;
        Pen p = new Pen(Color.Red);
        p.Width = 3;


        Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
        btnPreview.BackgroundImage = labelImage;
        grLabel = Graphics.FromImage(btnPreview.BackgroundImage);

        //btnPreview.BackColor = Color.Red;
        /*
        Graphics grLabel = Graphics.FromImage(btnPreview.Image);
        grLabel.DrawRectangle(p, 2, 2, (float)20, (float)20);
        Application.DoEvents();
         * */
        grLabel.DrawRectangle(p, 0, 0, (float)btnPreview.Width, (float)btnPreview.Height);
        this.Refresh();
        // Redraw();
    }



    /// <summary>
    /// Redraw, forced by the user  (in this application on mouse click).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void btnPreview_Click(object sender, EventArgs e) {
      if(mRenderOnClick)
        StartDrawing();
    }

    protected bool mRenderOnClick = true;



    /// <summary>
    /// Umschaltung, ob bei Mausklick mit den aktuellen Parametern gerechnet werden soll.
    /// </summary>
    public bool RenderOnClick {
      get {
        return mRenderOnClick;
      }
      set {
        mRenderOnClick = value;
      }
    }


      /// <summary>
    /// Set the size of the labelImage
      /// </summary>
    public void InitLabelImage() {
        Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
        btnPreview.BackgroundImage = labelImage;
        grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
    }

    /// <summary>
    /// Neuzeichnen.
    /// </summary>
    protected override void StartDrawing() {
      forceRedraw = false;
      btnPreview.Enabled = false;
      inDrawing = true;
      Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
      btnPreview.BackgroundImage = labelImage;
      grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
      iter = new Iterate(btnPreview.Width, btnPreview.Height, this, false);
      iter.OneStepProgress=true;
      AssignParameters();
      iter.StartAsync(mParameter,
              ParameterDict.Exemplar.GetInt("Formula.Static.Cycles"),
              2,
              1,
              ParameterDict.Exemplar.GetInt("Formula.Static.Formula"),
              ParameterDict.Exemplar.GetBool("View.Perspective"));
    }


    /// <summary>
    /// Direktzugriff auf das interne Bild.
    /// </summary>
    public Image Image {
      get {
        return (Image)btnPreview.BackgroundImage.Clone();
      }
      set {
        btnPreview.BackgroundImage = value;
        grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
      }
    }


    /// <summary>
    /// Berechnung wurde beendet.
    /// </summary>
    protected override void OneStepEnds() {
      if (iter != null) {
        lock (iter) {
          try {
              Fractrace.PictureArt.Renderer pArt;
              if (fixedRenderer == -1)
                  pArt = PictureArt.PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
              else
                  pArt = new PictureArt.FrontViewRenderer(iter.PictureData);
            //  btnPreview
            pArt.Paint(grLabel);
     
             // btnPreview.Size.Width
            Application.DoEvents();
            this.Refresh();
            if (RenderingEnds != null)
              RenderingEnds();
          } catch (Exception ex) {
            // tritt auf, wenn iter null ist
            System.Diagnostics.Debug.WriteLine(ex.ToString());
          }
        }
      }
      btnPreview.Enabled = true;
      inDrawing = false;
      if (forceRedraw)
        StartDrawing();
    }


    /// <summary>
    /// Fortschrittsbalken wird ein-bzw. ausgeschaltet.
    /// </summary>
    public bool ShowProgressBar {
      get {
        return panel1.Visible;
      }
      set {
            panel1.Visible = value;
      }
    }


    public event PictureRenderingIsReady RenderingEnds;

  }
}
