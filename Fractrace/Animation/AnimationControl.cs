using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Fractrace.Basic;
using Fractrace.Scheduler;

namespace Fractrace.Animation
{
    public partial class AnimationControl : UserControl
    {

        /// <summary>
        /// Control for "Animation" Parameters.
        /// </summary>
        DataViewControlPage _propertyControl;

        /// <summary>
        /// Constructer.
        /// </summary>
        public AnimationControl()
        {
            InitializeComponent();
            _mainAnimationControl = this;
            if (ParameterInput.MainParameterInput != null && ParameterInput.MainParameterInput.MainDataViewControl != null)
            {
                _propertyControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _propertyControl.Dock = DockStyle.Fill;
                _propertyControl.Create("Animation");
                panel3.Controls.Add(_propertyControl);
                this.cbSmooth.Visible = false;
                this.tbSize.Visible = false;
                this.label1.Visible = false;
                this.tbAnimationDescription.ScrollBars = ScrollBars.None;
                this.tbAnimationDescription.BorderStyle = BorderStyle.None;
//                this.tbAnimationDescription.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            }

        }


        PaintJob _currentPaintJob = null;

        /// <summary>
        /// Global instance of this unique window.
        /// </summary>
        public static AnimationControl MainAnimationControl { get { return _mainAnimationControl;  }  }
        private static AnimationControl _mainAnimationControl = null;

        /// <summary>
        /// Return true, if animation process is running.
        /// </summary>
        public static bool InAnimation { get { return _inAnimation; } }
        static bool _inAnimation = false;

        /// <summary>
        /// Verweis auf die global verwaltete Historie.
        /// </summary>
        ParameterHistory _dataPerTime = null;

        /// <summary>
        /// The Timeline.
        /// </summary>
        private AnimationSteps _animationSteps = new AnimationSteps();   

        /// <summary>
        /// Size of the picture in each frame.
        /// </summary>
        protected double _pictureSize = 1;

        /// <summary>
        /// Enthält die Formel des ersten Eintrages
        /// </summary>
        protected string _formula = "";

        /// <summary>
        /// Wird gesetzt, wenn der Nutzer die Berechnung der Animation abgebrochen hat.
        /// </summary>
        private bool _animationAbort = false;
  
        // Preview parameters:

        /// <summary>
        /// True while rendering of preview images. 
        /// </summary>
        protected bool _inRenderingPreview = false;

        protected int _currentPreviewStep = 0;

        /// <summary>
        /// Teilschritte, wenn nicht nur die Eckdaten geladen werden sollen.
        /// </summary>
        protected double _currentPreviewSubStep = 0;

        Dictionary<int, AnimationStepPreview> _stepPreviewControls = new Dictionary<int, AnimationStepPreview>();

        int _previewWidth = 50;

        int _previewHeight = 50;

        /// <summary>
        /// Initialisierung.
        /// </summary>
        public void Init(ParameterHistory data)
        {
            _dataPerTime = data;
        }


        /// <summary>
        /// Zeile wird an der aktuellen Position eingefügt.
        /// </summary>
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddToAnimation();
        }


        public void AddToAnimation()
        {
            string fileName = FileSystem.Exemplar.GetFileName("pic.png");
            ParameterInput.MainParameterInput.SaveHistory(fileName);
            ParameterInput.MainParameterInput.MainPreviewControl.Image.Save(fileName);
            AddCurrentHistoryEntry();
        }


        /// <summary>
        /// Append Entry to Animation
        /// </summary>
        public void AddCurrentHistoryEntry()
        {
            AnimationPoint point = new AnimationPoint();
            point.Time = _dataPerTime.CurrentTime;
            point.Steps = ParameterDict.Current.GetInt("Animation.Steps");
            string comment = "";
            try
            {
                string file = _dataPerTime.Get(point.Time)["Intern.FileName"];
                if (file != "")
                {
                    file = System.IO.Path.GetFileNameWithoutExtension(file);
                    comment = "         # File " + file;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            tbAnimationDescription.Text = tbAnimationDescription.Text + System.Environment.NewLine + "Run Steps " + point.Steps.ToString() + " Time " + point.Time.ToString() + comment ;
        }


        /// <summary>
        /// Remove entry with given time from timeline.
        /// </summary>
        public void RemoveStep(int time)
        {
            // Remove Entry with given time from Text.
            string animtext = tbAnimationDescription.Text;
            string[] stringSeparators = new string[] { System.Environment.NewLine };
            string[] lines = animtext.Split(stringSeparators, StringSplitOptions.None);
            StringBuilder updatedText =new StringBuilder();
            foreach (string line in lines)
            {
                if (!line.Contains(" Time " + time.ToString() + " "))
                    updatedText.AppendLine(line);
            }
            tbAnimationDescription.Text = updatedText.ToString();
        }

      
        /// <summary>
        /// Create animation info from string.
        /// </summary>
        private void CreateAnimationSteps(string animationDescription)
        {
            _animationSteps.Steps.Clear();
            string tempstr = animationDescription.Replace(System.Environment.NewLine, " ");
            string[] entries = tempstr.Split(' ');
            AnimationPoint currentAp = null;
            string lastEntry = "";
            foreach (string str in entries)
            {
                switch (str.ToLower())
                {
                    case "run":
                        if (currentAp != null)
                            _animationSteps.Steps.Add(currentAp);
                        currentAp = new AnimationPoint();
                        break;
                }
                switch (lastEntry.ToLower())
                {
                    case "steps":
                        if (currentAp != null)
                            currentAp.Steps = int.Parse(str);
                        break;

                    case "time":
                        if (currentAp != null)
                            currentAp.Time = int.Parse(str);
                        break;

                    case "file":
                        if (currentAp != null)
                            currentAp.fileName = str;
                        break;
                }
                lastEntry = str;
            }
            if (currentAp != null)
                _animationSteps.Steps.Add(currentAp);
        }


        bool _animationSmooth = ParameterDict.Current.GetBool("Animation.Smooth");

        /// <summary>
        /// Start rendering full Animation.
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            _pictureSize = ParameterDict.Current.GetDouble("Animation.Size");
            _animationSmooth = ParameterDict.Current.GetBool("Animation.Smooth");
            _inAnimation = true;
            ParameterInput.MainParameterInput.SetButtonsToStart();
            CreateAnimationSteps(tbAnimationDescription.Text);
            if (_animationSteps.Steps.Count == 0)
                return;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnStop.Visible = true;
            _animationAbort = false;
            lblAnimationProgress.Text = "run ...";
            // Prepare AnimationHistory
            ParameterHistory animationHistory = new ParameterHistory();
            for (int i = 0; i < _animationSteps.Steps.Count; i++)
            {
                AnimationPoint ap = _animationSteps.Steps[i];
                _dataPerTime.Load(ap.Time);
                ParameterDict.Current.SetDouble("View.Size", _pictureSize);
                animationHistory.Save();
            }
            // Compute each Animation frame.
            for (int i = 1; i < _animationSteps.Steps.Count; i++)
            {
                AnimationPoint ap1 = _animationSteps.Steps[i - 1];
                AnimationPoint ap2 = _animationSteps.Steps[i];
                ComputeAnimationPart(ap1.Time, ap2.Time, ap2.Steps, animationHistory,i-1);
                if (_animationAbort)
                    break;
            }
            if (_animationSteps.Steps.Count > 0)
            {
                ComputeAnimationPart(_animationSteps.Steps[_animationSteps.Steps.Count - 1].Time, _animationSteps.Steps[_animationSteps.Steps.Count - 1].Time, 1, animationHistory, _animationSteps.Steps.Count-1);
            }
            btnStop.Visible = false;
            btnStart.Enabled = true;
            lblAnimationProgress.Text = "ready";
            _animationAbort = false;
            _inAnimation = false;
            ParameterInput.MainParameterInput.SetButtonsToStop();
        }


        /// <summary>
        /// Compute part of animation.
        /// </summary>
        private void ComputeAnimationPart(int from, int to, int steps, ParameterHistory animationHistory, int historyIndex)
        {
            lblAnimationProgress.Text = "compute: " + from.ToString() + " " + to.ToString();
            for (int i = 0; i < steps && !_animationAbort; i++)
            {
                lblAnimationProgress.Text = "compute: " + from.ToString() + " " + to.ToString() + " Step " + i.ToString() + " (from " + steps.ToString() + ")";
                double r = 1.0 / steps * (double)i;
                Application.DoEvents();
                if (_animationSmooth)
                    animationHistory.LoadSmoothed(r + historyIndex);
                else
                    animationHistory.Load(r + historyIndex);
                int updateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
                if (updateSteps <= 0)
                    updateSteps = 0;
                if (updateSteps > 1)
                    ParameterDict.Current.SetInt("View.UpdateSteps", updateSteps - 1);
                ResultImageView.PublicForm.SetPictureBoxSize();
                Fractrace.Scheduler.PaintJob paintJob = new Scheduler.PaintJob(ResultImageView.PublicForm, ResultImageView.PublicForm.GestaltPicture);
                _currentPaintJob = paintJob;
                paintJob.Run(updateSteps);
                ResultImageView.PublicForm.CallDrawImage();
                if (_stepPreviewControls.ContainsKey(from))
                    _stepPreviewControls[from].UpdateComputedStep(i);
            }
        } 


        /// <summary>
        /// Stop Animation. Creation of current image is not stopped.
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            _animationAbort = true;
            btnStop.Enabled = false;
        }


        /// <summary>
        /// Diplay some animation steps as preview images.
        /// </summary>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            RenderPreview();
        }


        /// <summary>
        /// Draws all preview images.
        /// </summary>
        private void RenderPreview()
        {
            _previewHeight = 100;
            _previewWidth = 100;

            CreateAnimationSteps(tbAnimationDescription.Text);
            pnlPreview.Controls.Clear();
            _stepPreviewControls.Clear();
            _inRenderingPreview = true;
            _currentPreviewStep = 0;
            mPreview1_RenderingEnds();
        }


        /// <summary>
        /// Rendering of one preview image is ready.
        /// </summary>

        void mPreview1_RenderingEnds()
        {
            if (_currentPreviewStep >= _animationSteps.Steps.Count)
            {
                _inRenderingPreview = false;
                _currentPreviewStep = 0;
                btnPreview.Enabled = true;
                return;
            }
            if (!_inRenderingPreview)
                return;
            // Load data of currentPreviewStep:
            AnimationPoint ap = _animationSteps.Steps[_currentPreviewStep];
            ParameterHistory animationHistory = new ParameterHistory();
            _dataPerTime.Load(ap.Time);
            animationHistory.Save();

            PreviewControl mPreview1 = new Fractrace.PreviewControl(0);
            mPreview1.Width = _previewWidth ;
            mPreview1.Height = _previewHeight;
            mPreview1.Location = new System.Drawing.Point(_previewWidth * _currentPreviewStep, 0);
            pnlPreview.Controls.Add(mPreview1);
            mPreview1.ShowProgressBar = false;
            mPreview1.RenderOnClick = false;

            AnimationStepPreview stepInfo = new AnimationStepPreview();
            stepInfo.Width = _previewWidth;
            stepInfo.Height = 30; // _previewHeight;
            stepInfo.Location = new System.Drawing.Point(_previewWidth * _currentPreviewStep, _previewHeight);
            pnlPreview.Controls.Add(stepInfo);
            int steps=0;
            if ( _animationSteps.Steps.Count>_currentPreviewStep+1 )
              steps = _animationSteps.Steps[_currentPreviewStep+1].Steps;
            stepInfo.Init(ap.Time, steps);
            _stepPreviewControls[ap.Time] = stepInfo;
            _currentPreviewStep++;
            mPreview1.RenderingEnds += new PictureRenderingIsReady(mPreview1_RenderingEnds);
            mPreview1.Draw();
        }
        

        /// <summary>
        /// Load Animation file.
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "*.franim|*.franim";
            if (od.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(od.FileName, Encoding.GetEncoding("iso-8859-1"));
                String animstring = sr.ReadToEnd();
                sr.Close();

                // Load scenes given in comment
                CreateAnimationSteps(animstring);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _animationSteps.Steps.Count; i++)
                {
                    AnimationPoint ap = _animationSteps.Steps[i];
                    // load file
                    if (ap.fileName != "")
                    {
                        string dir = ap.fileName.Substring(0, ap.fileName.IndexOf("pic"));
                        //picture filenename
                        string picFileName = System.IO.Path.Combine(System.IO.Path.Combine(FileSystem.Exemplar.ExportDir, dir), ap.fileName);
                        string fileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + ap.fileName + ".gestalt";
                        if (!System.IO.File.Exists(fileName))
                            fileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + ap.fileName + ".tomo";
                        if (!System.IO.File.Exists(fileName))
                            fileName = fileName.Replace("Gestaltlupe", "Tomotrace");

                        ParameterDict.Current.Load(fileName);
                        ParameterInput.MainParameterInput.SaveHistory(picFileName);
                        // save in history
                        sb.AppendLine("Run Steps " + ap.Steps.ToString() + " Time " + ParameterInput.MainParameterInput.History.CurrentTime.ToString() + "      # File " + ap.fileName);
                    }
                }
                tbAnimationDescription.Text = sb.ToString();
            }
        }


        /// <summary>
        /// Saved animation file.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.franim|*.franim";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new System.IO.StreamWriter(sd.FileName, false, Encoding.GetEncoding("iso-8859-1"));
                sw.Write(tbAnimationDescription.Text);
                sw.Close();
            }
        }


        public void Abort()
        {
            if (_currentPaintJob != null)
                _currentPaintJob.Abort();
        }


        /// <summary>
        /// Is called if some properties changed.
        /// </summary>
        public void UpdateFromChangeProperty()
        {
            _propertyControl.UpdateElements();
        }


    }
}
