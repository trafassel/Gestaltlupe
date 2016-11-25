using System;
using System.Drawing;
using System.Windows.Forms;

using Fractrace.Basic;

namespace Fractrace
{

    public delegate void PictureRenderingIsReady();

    public delegate void Progress(double progress);

    /// <summary>
    /// Control which displays the redered image.
    /// </summary>
    public class PreviewControl : RenderImage
    {

        public PreviewControl(int updateSteps)
        {
            _updateSteps = updateSteps;
        }

        // number of additional steps to increase render quality.
        int _updateSteps = 0;

        public Button PreviewButton
        {
            get
            {
                lock (_smallPreviewCurrentDrawStepMutex)
                { return this.btnPreview; }
            }
        }
        protected System.Windows.Forms.Button btnPreview;

        public event PictureRenderingIsReady RenderingEnds;

        public event Progress ProgressEvent;

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
            this.btnPreview.FlatStyle = FlatStyle.Flat;
            this.btnPreview.FlatAppearance.BorderSize = 0;
            Image labelImage = new Bitmap((int)(btnPreview.Width), (int)(btnPreview.Height));
            btnPreview.BackgroundImage = labelImage;
            _graphics = Graphics.FromImage(labelImage);
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
            _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
            _graphics.DrawRectangle(p, 0, 0, (float)btnPreview.Width, (float)btnPreview.Height);
            this.Refresh();
        }


        /// <summary>
        /// Redraw, forced by the user  (in this application on mouse click).
        /// </summary>
        public void btnPreview_Click(object sender, EventArgs e)
        {
            lock (_smallPreviewCurrentDrawStepMutex)
            {
                _smallPreviewCurrentDrawStep = 1;
            }
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
            _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
        }


        /// <summary>
        /// Neuzeichnen.
        /// </summary>
        protected override void StartDrawing()
        {
            ResultImageView.PublicForm.Stop();
            _forceRedraw = false;
               
            if (btnPreview.Width < 1 && btnPreview.Height < 1)
            {
                ResultImageView.PublicForm.CurrentUpdateStep = 0;
                return;
            }
            lock (_inDrawingMutex)
                _inDrawing = true;
            System.Diagnostics.Debug.WriteLine("_inDrawing = true (1)");
            if (_iterate != null)
            {
                if(_iterate.Running)
                  _iterate.Abort();
            }

            lock (_smallPreviewCurrentDrawStepMutex)
            {
                btnPreview.Enabled = false;
                if (_smallPreviewCurrentDrawStep == 0 || _smallPreviewCurrentDrawStep == 1 || _smallPreviewCurrentDrawStep == 2 || _smallPreviewCurrentDrawStep == 4)
                {
                    _iterate = new Iterate(btnPreview.Width / 2, btnPreview.Height / 2, this, false);
                    _smallPreviewCurrentDrawStep = 2;
                }

                else if (_smallPreviewCurrentDrawStep == 3)
                {
                    _iterate = new Iterate(btnPreview.Width, btnPreview.Height, this, false);
                    _iterate.SetOldData(null, null, 2);
                    _smallPreviewCurrentDrawStep = 4;
                }
                else
                {
                    if (_iterate != null)
                        if (_iterate.Running)
                            _iterate.Abort();
                    _iterate = null;
                    lock (_inDrawingMutex)
                        _inDrawing = false;
                    _smallPreviewCurrentDrawStep = 1;
                }

            }
            if (_iterate != null)
            {
                _iterate._oneStepProgress = false;
                AssignParameters();
                _iterate.StartAsync(_parameter,
                        ParameterDict.Current.GetInt("Formula.Static.Cycles"),
                        1,
                        ParameterDict.Current.GetBool("Formula.Static.Julia"),
                        !ParameterDict.Current.GetBool("Transformation.Camera.IsometricProjection"));
            }
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
                if(btnPreview.BackgroundImage!=null)
                   _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
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
            if (_iterate == null || !_iterate.InAbort)
                this.Invoke(new OneStepEndsDelegate(OneStepEnds));
            else
            {
                lock (_inDrawingMutex)
                    _inDrawing = false;
            }
            System.Diagnostics.Debug.WriteLine("_inDrawing = false (2)");
            InitBaseImage();
        }


        /// <summary>
        /// Berechnung wurde beendet.
        /// </summary>
        protected override void OneStepEnds()
        {
            if (_iterate != null)
            {
                lock (_iterate)
                {
                    if (_iterate.InAbort)
                        return;
                    try
                    {
                        Fractrace.PictureArt.Renderer pArt;
                        if (_fixedRenderer == -1)
                        {
                            if (IsSmallPreview())
                            {
                                lock (_smallPreviewCurrentDrawStepMutex)
                                {
                                   
                                    if (_smallPreviewCurrentDrawStep == 1 || _smallPreviewCurrentDrawStep == 2)
                                    {
                                        if (_iterate.Height < 60)
                                        {
                                            pArt = new PictureArt.FastPreviewRenderer(_iterate.PictureData);
                                            pArt.Init(_iterate.LastUsedFormulas);
                                        }
                                        else
                                        {
                                            pArt = PictureArt.PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas);
                                        }
                                        
                                    }

                                    else if (_smallPreviewCurrentDrawStep == 4 || _smallPreviewCurrentDrawStep == 3)
                                    {
                                        pArt = PictureArt.PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas);
                                        _smallPreviewCurrentDrawStep = 5;
                                    }
                                    else
                                    {
                                        if (_smallPreviewCurrentDrawStep == 5)
                                            _smallPreviewCurrentDrawStep = 1;
                                        pArt = null;
                                    }
                                }
                                   
                            }
                            else
                            {
                                pArt = PictureArt.PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas);
                            }
                            btnPreview.BackgroundImage = new Bitmap((int)(_iterate.Width), (int)(_iterate.Height));
                            _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
                            pArt.Paint(_graphics);
                            Application.DoEvents();
                            this.Refresh();
                        }
                        else
                        {
                            DrawFromView();
                        }
                       
                        if (IsSmallPreview())
                        {
                            lock (_smallPreviewCurrentDrawStepMutex)
                            {
                                // _smallPreviewCurrentDrawStep++;

                                if (_smallPreviewCurrentDrawStep == 5)
{
                                    _smallPreviewCurrentDrawStep = 1;

                                }
                                else if ( (_smallPreviewCurrentDrawStep == 1 || _smallPreviewCurrentDrawStep == 2) && _fixedRenderer == -1)

                                //                  if (_smallPreviewCurrentDrawStep == 2 && _fixedRenderer == -1)
                                {
                                    if (RenderingEnds != null)
                                        RenderingEnds();

                                    // Uncomment following line for more accurate small preview rendering in next iteration. 
                                    if (_smallPreviewCurrentDrawStep == 2 && _updateSteps>0)
                                    {
                                        if (!_iterate.Running)
                                        {
                                            _smallPreviewCurrentDrawStep = 3;
                                            lock (_inDrawingMutex)
                                                _inDrawing = false;
                                            System.Diagnostics.Debug.WriteLine("_inDrawing = false (3)");

                                            StartDrawing();
                                        }
                                    }
                                }
                                else
                                {
                                    if (_fixedRenderer == -1)
                                    {
                                        if (RenderingEnds != null)
                                            RenderingEnds();
                                    }
                                }
                               // if (_smallPreviewCurrentDrawStep > 1)
                               //     _smallPreviewCurrentDrawStep = 0;
                            }
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
            lock (_inDrawingMutex)
                _inDrawing = false;
            System.Diagnostics.Debug.WriteLine("_inDrawing = false (4)");
            if (ParameterDict.Current.GetBool("View.Pipeline.Preview") && this == ParameterInput.MainParameterInput.MainPreviewControl)
            {
                ParameterInput.MainParameterInput.ComputePreview();
            }
            if (_forceRedraw)
                StartDrawing();
        }

        /// <summary>
        /// Fortschritt in Prozent.
        /// </summary>
        /// <param name="progressInPercent"></param>
        public override void Progress(double progressInPercent)
        {
            base.Progress(progressInPercent);
            if(ProgressEvent!=null)
                ProgressEvent(progressInPercent);
        }


        private void DrawFromView()
        {
            Fractrace.PictureArt.Renderer pArt;
            pArt = new PictureArt.FrontViewRenderer(_iterate.PictureData);
            pArt.Init(_iterate.LastUsedFormulas);
            btnPreview.BackgroundImage = new Bitmap((int)(_iterate.Width), (int)(_iterate.Height));
            _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
            pArt.Paint(_graphics);
            Application.DoEvents();
            this.Refresh();
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
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mPictureBox
            // 
            this._pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            // 
            // PreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "PreviewControl";
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        System.Drawing.Bitmap _baseBitmap = null;

        /// <summary>
        /// Internal reference bitmap is set (as copy of current bitmap).
        /// </summary>
        public void InitBaseImage()
        {
            try
            {
                _baseBitmap = (System.Drawing.Bitmap)btnPreview.BackgroundImage.Clone();
            }
            catch (Exception ex)
            { System.Diagnostics.Debug.WriteLine(ex.ToString()); }
        }


        public void MoveBitmap(int x, int y)

        {
            System.Diagnostics.Debug.WriteLine("MoveBitmap " + x.ToString() + " " + y.ToString());
            if (_baseBitmap == null)
            {
                return;
            }
            _graphics = Graphics.FromImage(btnPreview.BackgroundImage);
            _graphics.Clear(Color.White);
            if(btnPreview.BackgroundImage.Width<100)
               _graphics.DrawImage(_baseBitmap, x/2, y/2);
            else
                _graphics.DrawImage(_baseBitmap, x, y);
            Application.DoEvents();
            this.Update();
            this.Refresh();
            btnPreview.Refresh();
            btnPreview.Focus();
        }


    }
}
