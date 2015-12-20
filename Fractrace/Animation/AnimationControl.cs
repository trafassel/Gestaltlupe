using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
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
        /// Constructer.
        /// </summary>
        public AnimationControl()
        {
            InitializeComponent();
            MainAnimationControl = this;
        }


        public void Abort()
        {
            if (currentPaintJob != null)
                currentPaintJob.Abort();
        }


        PaintJob currentPaintJob = null;

        /// <summary>
        /// Global instance of this unique window.
        /// </summary>
        public static AnimationControl MainAnimationControl = null;


        /// <summary>
        /// Append Entry to Animation
        /// </summary>
        public void AddCurrentHistoryEntry()
        {
            AnimationPoint point = new AnimationPoint();
            point.Time = dataPerTime.CurrentTime;
            point.Steps = ParameterDict.Current.GetInt("Animation.Steps");
            string comment = "";
            try
            {
                string file = dataPerTime.Get(point.Time)["Intern.FileName"];
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
        /// Remov entry with given time from timeline.
        /// </summary>
        /// <param name="time"></param>
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
        /// Return true, if animation process is running.
        /// </summary>
        public static bool InAnimation
        {
            get
            {
                return inAnimation;
            }
        }


        static bool inAnimation = false; 


        /// <summary>
        /// Verweis auf die global verwaltete Historie.
        /// </summary>
        ParameterHistory dataPerTime = null;


        /// <summary>
        /// Initialisierung.
        /// </summary>
        public void Init(ParameterHistory data)
        {
            dataPerTime = data;
        }


        /// <summary>
        /// The Timeline.
        /// </summary>
        private AnimationSteps mAnimationSteps = new AnimationSteps();


        /// <summary>
        /// Zeile wird an der aktuellen Position eingefügt.
        /// </summary>
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddCurrentHistoryEntry();
        }


        /// <summary>
        /// Size of the picture in each frame.
        /// </summary>
        protected double mPictureSize = 1;


        /// <summary>
        /// Enthält die Formel des ersten Eintrages
        /// </summary>
        protected string mFormula = "";


        /// <summary>
        /// Aus dem eingegebenen Text wird die Animation erzeugt.
        /// </summary>
        private void CreateAnimationSteps(string animationDescription)
        {
            mAnimationSteps.Steps.Clear();

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
                            mAnimationSteps.Steps.Add(currentAp);
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
                mAnimationSteps.Steps.Add(currentAp);
        }


        /// <summary>
        /// Start der Animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            inAnimation = true;
            ParameterInput.MainParameterInput.SetButtonsToStart();
            CreateAnimationSteps(tbAnimationDescription.Text);
            if (mAnimationSteps.Steps.Count == 0)
                return;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnStop.Visible = true;
            animationAbort = false;
            lblAnimationProgress.Text = "run ...";

            // Prepare AnimationHistory
            ParameterHistory animationHistory = new ParameterHistory();
            for (int i = 0; i < mAnimationSteps.Steps.Count; i++)
            {
                AnimationPoint ap = mAnimationSteps.Steps[i];
                dataPerTime.Load(ap.Time);
                ParameterDict.Current.SetDouble("View.Size", mPictureSize);
                animationHistory.Save();
            }
            // Compute each Animation frame.
            for (int i = 1; i < mAnimationSteps.Steps.Count; i++)
            {

                AnimationPoint ap1 = mAnimationSteps.Steps[i - 1];
                AnimationPoint ap2 = mAnimationSteps.Steps[i];
                ComputeAnimationPart(ap1.Time, ap2.Time, ap2.Steps, animationHistory,i-1);
                if (animationAbort)
                    break;
            }
            if (mAnimationSteps.Steps.Count > 0)
            {
                ComputeAnimationPart(mAnimationSteps.Steps[mAnimationSteps.Steps.Count - 1].Time, mAnimationSteps.Steps[mAnimationSteps.Steps.Count - 1].Time, 1, animationHistory, mAnimationSteps.Steps.Count-1);
            }

            btnStop.Visible = false;
            btnStart.Enabled = true;
            lblAnimationProgress.Text = "ready";
            animationAbort = false;
            inAnimation = false;
            ParameterInput.MainParameterInput.SetButtonsToStop();
        }



        /// <summary>
        /// Berechnet einen Animationsteil, bestehend aus steps Einzelschritten.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="steps"></param>
        private void ComputeAnimationPart(int from, int to, int steps, ParameterHistory animationHistory, int historyIndex)
        {
            lblAnimationProgress.Text = "compute: " + from.ToString() + " " + to.ToString();
            for (int i = 0; i < steps && !animationAbort; i++)
            {
                lblAnimationProgress.Text = "compute: " + from.ToString() + " " + to.ToString() + " Step " + i.ToString() + " (from " + steps.ToString() + ")";
                double r = 1.0 / steps * (double)i;
                Application.DoEvents();
                if (cbSmooth.Checked)
                    animationHistory.LoadSmoothed(r + historyIndex);
                else
                    animationHistory.Load(r + historyIndex);

                int updateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
                if (updateSteps <= 0)
                    updateSteps = 0;
                if (updateSteps > 1)
                    ParameterDict.Current.SetInt("View.UpdateSteps", updateSteps - 1);

                Form1.PublicForm.SetPictureBoxSize();
                Fractrace.Scheduler.PaintJob paintJob = new Scheduler.PaintJob(Form1.PublicForm, Form1.PublicForm.GestaltPicture);
                currentPaintJob = paintJob;
                paintJob.Run(updateSteps);
                Form1.PublicForm.CallDrawImage();
                if (StepPreviewControls.ContainsKey(from))
                    StepPreviewControls[from].UpdateComputedStep(i);

            }
        } 


        /// <summary>
        /// Wird gesetzt, wenn der Nutzer die Berechnung der Animation abgebrochen hat.
        /// </summary>
        private bool animationAbort = false;


        /// <summary>
        /// Animation anhalten.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            animationAbort = true;
            btnStop.Enabled = false;
        }


        /// <summary>
        /// Die Einzelereignisse der Animation werden gezeigt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            btnPreview.Enabled = false;
            RenderPreview();
        }


        protected bool inRenderingPreview = false;


        private void RenderPreview()
        {
            int height = 0;
            if (int.TryParse(tbPreviewSize.Text, out height))
            {
                if (height > 10 && height < 256)
                    previewHeight = height;
            }
            int width = 0;
            if (int.TryParse(tbPreviewSize.Text, out width))
            {
                if (width > 10 && width < 256)
                    previewWidth = width;
            }

            CreateAnimationSteps(tbAnimationDescription.Text);
            pnlPreview.Controls.Clear();
            StepPreviewControls.Clear();
            inRenderingPreview = true;
            currentPreviewStep = 0;
            mPreview1_RenderingEnds();
        }

        protected int currentPreviewStep = 0;

        /// <summary>
        /// Teilschritte, wenn nicht nur die Eckdaten geladen werden sollen.
        /// </summary>
        protected double currentPreviewSubStep = 0;


        /// <summary>
        /// Ein Einzelschritt der Voransichts-Animation wurde geladen.
        /// </summary>
        void mPreview1_RenderingEnds()
        {
            if (currentPreviewStep >= mAnimationSteps.Steps.Count)
            {
                inRenderingPreview = false;
                currentPreviewStep = 0;
                btnPreview.Enabled = true;
                return;
            }
            if (!inRenderingPreview)
                return;
            // Load data of currentPreviewStep:
            AnimationPoint ap = mAnimationSteps.Steps[currentPreviewStep];
            ParameterHistory animationHistory = new ParameterHistory();
            dataPerTime.Load(ap.Time);
            animationHistory.Save();

            PreviewControl mPreview1 = new Fractrace.PreviewControl();
            mPreview1.Width = previewWidth ;
            mPreview1.Height = previewHeight;
            mPreview1.Location = new System.Drawing.Point(previewWidth * currentPreviewStep, 0);
            pnlPreview.Controls.Add(mPreview1);
            mPreview1.ShowProgressBar = false;
            mPreview1.RenderOnClick = false;

            AnimationStepPreview stepInfo = new AnimationStepPreview();
            stepInfo.Width = previewWidth;
            stepInfo.Height = previewHeight;
            stepInfo.Location = new System.Drawing.Point(previewWidth * currentPreviewStep, previewHeight);
            pnlPreview.Controls.Add(stepInfo);
            int steps=0;
            if ( mAnimationSteps.Steps.Count>currentPreviewStep+1 )
              steps = mAnimationSteps.Steps[currentPreviewStep+1].Steps;
            stepInfo.Init(ap.Time, steps);
            StepPreviewControls[ap.Time] = stepInfo;
            currentPreviewStep++;
            mPreview1.RenderingEnds += new PictureRenderingIsReady(mPreview1_RenderingEnds);
            mPreview1.Draw();
        }


        Dictionary<int, AnimationStepPreview> StepPreviewControls = new Dictionary<int, AnimationStepPreview>();


        int previewWidth = 50;


        int previewHeight = 50;


        private void tbSize_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(tbSize.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mPictureSize))
            {
                tbSize.ForeColor = Color.Black;
            }
            else
                tbSize.ForeColor = Color.Red;


        }


        /// <summary>
        /// Load Animation file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                for (int i = 0; i < mAnimationSteps.Steps.Count; i++)
                {
                    AnimationPoint ap = mAnimationSteps.Steps[i];
                    // load file
                    if (ap.fileName != "")
                    {
                        string dir = ap.fileName.Substring(0, ap.fileName.IndexOf("pic"));
                        //picture filenename
                        string picFileName = System.IO.Path.Combine(System.IO.Path.Combine(FileSystem.Exemplar.ExportDir, dir), ap.fileName);
                        string fileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + ap.fileName + ".tomo";

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


    }
}
