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

    public delegate void PictureRenderingIsReady();


    /// <summary>
    /// Control which displays the redered image.
    /// </summary>
    public class PreviewControl : RenderImage
    {

        public Button PreviewButton { get { return this.btnPreview; } }
        protected System.Windows.Forms.Button btnPreview;


        public event PictureRenderingIsReady RenderingEnds;

        /// <summary>
        /// Umschaltung, ob bei Mausklick mit den aktuellen Parametern gerechnet werden soll.
        /// </summary>
        public bool RenderOnClick { get { return _renderOnClick; } set { _renderOnClick = value; } }
        protected bool _renderOnClick = true;

        /// <summary>
        /// Fortschrittsbalken wird ein-bzw. ausgeschaltet.
        /// </summary>
        public bool ShowProgressBar { get { return panel1.Visible; } set { panel1.Visible = value; } }


        /// <summary>
        /// Der Graphik-Kontext wird initialisiert.
        /// </summary>
        protected override void Init()
        {
            this.btnPreview = new System.Windows.Forms.Button();
            this.panel2.Controls.Add(this.btnPreview);
            this.btnPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPreview.Location = new System.Drawing.Point(0, 0);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(50, 40);
            this.btnPreview.TabIndex = 1;
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.BackgroundImageLayout = ImageLayout.Stretch;
            Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
            btnPreview.BackgroundImage = labelImage;
            grLabel = Graphics.FromImage(labelImage);
        }


        /// <summary>
        /// Clear the image.
        /// </summary>
        public void Clear()
        {
            Pen p = new Pen(Color.Red);
            p.Width = 3;
            Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
            btnPreview.BackgroundImage = labelImage;
            grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
            grLabel.DrawRectangle(p, 0, 0, (float)btnPreview.Width, (float)btnPreview.Height);
            this.Refresh();
        }


        /// <summary>
        /// Redraw, forced by the user  (in this application on mouse click).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnPreview_Click(object sender, EventArgs e)
        {
            smallPreviewCurrentDrawStep = 1;
            if (_renderOnClick)
                StartDrawing();
        }


        /// <summary>
        /// Set the size of the labelImage
        /// </summary>
        public void InitLabelImage()
        {
            Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
            btnPreview.BackgroundImage = labelImage;
            grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
        }


        /// <summary>
        /// Neuzeichnen.
        /// </summary>
        protected override void StartDrawing()
        {
            Form1.PublicForm.Stop();
            forceRedraw = false;
            btnPreview.Enabled = false;
            inDrawing = true;
            if (btnPreview.Width < 1 && btnPreview.Height < 1)
            {
                Form1.PublicForm.CurrentUpdateStep = 0;
                return;
            }
            if (iter != null)
                iter.Abort();

            if (smallPreviewCurrentDrawStep == 1)
            {
                iter = new Iterate(btnPreview.Width / 2, btnPreview.Height / 2, this, false);
            }
            else
            {
                iter = new Iterate(btnPreview.Width, btnPreview.Height, this, false);
            }
            iter.OneStepProgress = false;
            AssignParameters();
            iter.StartAsync(mParameter,
                    ParameterDict.Current.GetInt("Formula.Static.Cycles"),
                    1,
                    ParameterDict.Current.GetInt("Formula.Static.Formula"),
                    ParameterDict.Current.GetBool("View.Perspective"));
            Form1.PublicForm.CurrentUpdateStep = 0;
        }


        /// <summary>
        /// Direktzugriff auf das interne Bild.
        /// </summary>
        public Image Image
        {
            get
            {
                return (Image)btnPreview.BackgroundImage.Clone();
            }
            set
            {
                btnPreview.BackgroundImage = value;
                grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
            }
        }


        /// <summary>
        /// Return true, if corresponding image is used as small preview.
        /// </summary>
        /// <returns></returns>
        protected bool IsSmallPreview()
        {
            return (Image.Width < 150 && Image.Height < 150);
        }


        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        public override void ComputationEnds()
        {
            if (iter == null || !iter.InAbort)
                this.Invoke(new OneStepEndsDelegate(OneStepEnds));
        }


        /// <summary>
        /// Berechnung wurde beendet.
        /// </summary>
        protected override void OneStepEnds()
        {
            if (iter != null)
            {
                lock (iter)
                {
                    if (iter.InAbort)
                        return;
                    try
                    {
                        Fractrace.PictureArt.Renderer pArt;
                        if (fixedRenderer == -1)
                        {
                            if (IsSmallPreview())
                            {
                                pArt = new PictureArt.FastPreviewRenderer(iter.PictureData);
                                pArt.Init(iter.LastUsedFormulas);
                            }
                            else
                            {
                                pArt = PictureArt.PictureArtFactory.Create(iter.PictureData, iter.LastUsedFormulas);
                            }
                        }
                        else
                        {
                            pArt = new PictureArt.FrontViewRenderer(iter.PictureData);
                            pArt.Init(iter.LastUsedFormulas);
                        }
                        btnPreview.BackgroundImage = new Bitmap((int)(iter.Width), (int)(iter.Height));
                        grLabel = Graphics.FromImage(btnPreview.BackgroundImage);
                        pArt.Paint(grLabel);
                        Application.DoEvents();
                        this.Refresh();
                        if (IsSmallPreview())
                        {
                            smallPreviewCurrentDrawStep++;
                            if (smallPreviewCurrentDrawStep == 2 && fixedRenderer == -1)
                            {
                                if (RenderingEnds != null)
                                    RenderingEnds();

                                // Uncomment following line for more accurate small preview rendering in next iteration. 
                                //StartDrawing();
                            }
                            else
                            {
                                if (fixedRenderer == -1)
                                {
                                    if (RenderingEnds != null)
                                        RenderingEnds();
                                }
                            }
                            if (smallPreviewCurrentDrawStep > 1)
                                smallPreviewCurrentDrawStep = 0;
                        }
                        else
                        {
                            if (RenderingEnds != null)
                                RenderingEnds();
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
            btnPreview.Enabled = true;
            inDrawing = false;
            if (ParameterDict.Current.GetBool("View.Pipeline.Preview") && this == ParameterInput.MainParameterInput.MainPreviewControl)
            {
                ParameterInput.MainParameterInput.ComputePreview();
            }
            if (forceRedraw)
                StartDrawing();
        }


        /// <summary>
        /// Called, if event PaintEnds in Renderer is raised. 
        /// </summary>
        void pArt_PaintEnds()
        {
            Application.DoEvents();
            this.Refresh();
            if (RenderingEnds != null)
                RenderingEnds();
        }


        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this.mPictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mPictureBox
            // 
            this.mPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // PreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "PreviewControl";
            ((System.ComponentModel.ISupportInitialize)(this.mPictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }


    }
}
