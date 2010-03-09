using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using Fractrace.Basic;
using Fractrace.DataTypes;

namespace Fractrace {

  public delegate void PictureRenderingIsReady();

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
      this.btnPreview.Size = new System.Drawing.Size(150, 140);
      this.btnPreview.TabIndex = 1;
      this.btnPreview.UseVisualStyleBackColor = true;
      this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);


      Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
      btnPreview.BackgroundImage = labelImage;
      grLabel = Graphics.FromImage(labelImage);
    }



    /// <summary>
    /// Neuzeichnen wurde ausgewählt
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPreview_Click(object sender, EventArgs e) {
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
    /// Um das schnelle Zeichnen von Objekten aus den Preview Control zu aktivieren, 
    /// </summary>
    string oldRenderer = "";
    /// <summary>
    /// Neuzeichnen.
    /// </summary>
    protected override void StartDrawing() {

      forceRedraw = false;
      btnPreview.Enabled = false;
      inDrawing = true;
      iter = new Iterate(btnPreview.Width, btnPreview.Height, this);
      iter.Init(grLabel);
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
            string oldRenderer = ParameterDict.Exemplar["Composite.Renderer"];
            ParameterDict.Exemplar["Composite.Renderer"] = "FastScienceRenderer";

            Fractrace.PictureArt.Renderer pArt = PictureArt.PictureArtFactory.Create(iter.PictureData);
            // Hier wird kurz der Renderer ausgetauscht
            ParameterDict.Exemplar["Composite.Renderer"] = oldRenderer;

            pArt.Paint(grLabel);
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
