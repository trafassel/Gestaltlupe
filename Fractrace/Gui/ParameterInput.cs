using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.Geometry;

namespace Fractrace
{


    /// <summary>
    /// Main window (as viewed by the user), the main window of this application is Form1 (which
    /// display the rendered image).
    /// </summary>
    public partial class ParameterInput : Form
    {

        public delegate void VoidDelegate();

        /// <summary>
        /// Global instance of this unique window.
        /// </summary>
        public static ParameterInput MainParameterInput = null;


        /// <summary>
        /// used in Redraw Picture 
        /// </summary>
        protected int currentPic = 0;

        Dictionary<int, Image> _historyImages = new Dictionary<int, Image>();

        /// <summary>
        /// get small preview control.
        /// </summary>
        public Fractrace.PreviewControl MainPreviewControl
        {
            get
            {
                return this.preview2;
            }
        }

        /// <summary>
        /// Public Acces to DataViewControl.
        /// </summary>
        public DataViewControl MainDataViewControl { get { return parameterDictControl1.MainDataViewControl; } }

        /// <summary>
        /// Get in historyControl used parameter history.
        /// </summary>
        public ParameterHistory History  {  get  {  return _history;  } }
        ParameterHistory _history = new ParameterHistory();

        /// <summary>
        /// Zugriff auf die Bearbeitungsparameter.
        /// </summary>
        public FracValues Parameter { get { return _parameter; } }
        private FracValues _parameter = new FracValues();

        /// <summary>
        /// Get Formula.Static.Cycles.
        /// </summary>
        public int Cycles { get { return (int)ParameterDict.Current.GetDouble("Formula.Static.Cycles"); } }

        /// <summary>
        /// Get View.Size.
        /// </summary>
        public double ScreenSize { get { return ParameterDict.Current.GetDouble("View.Size"); } }

        /// <summary>
        /// GetFormula.Static.Formula.
        /// </summary>
        public int Formula { get {
                return ParameterDict.Current.GetBool("Formula.Static.Julia") ? -2 : -1;
            } }

        public bool AutomaticSaveInAnimation { get { return cbAutomaticSaveAnimation.Checked; } }

        public bool Changed = false;

        /// <summary>
        /// Damit wird vermieden, dass nach dem Export von 3D Daten stets beim Öffnen das Exportverzeichnis
        /// als InitialDirectory verwendet wird. 
        /// </summary>
        protected static string oldDirectory = "";

        public void DeactivatePreview() { _previewMode = false; }
        protected bool _previewMode = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInput"/> class.
        /// </summary>
        public ParameterInput()
        {
            MainParameterInput = this;
            InitializeComponent();
            ParameterDict.Current.EventChanged += new ParameterDictChanged(Exemplar_EventChanged);
            navigateControl1.Init(preview1, preview2, this);
            this.animationControl1.Init(_history);
            preview1.PreviewButton.Click += new EventHandler(PreviewButton_Click);
            preview1.ShowProgressBar = false;
            preview2.ShowProgressBar = false;
            preview1.ProgressEvent += Preview1_ProgressEvent;
            string assembyInfo = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            string[] infos = assembyInfo.Split(',');
            string version = "";
            if (infos.Length > 1)
                version = infos[1];
            this.Text = "Gestaltlupe" + version + "    [" + System.IO.Path.GetFileName(FileSystem.Exemplar.ProjectDir) + "]";
            tabControl1.SelectedIndex = 4;
            SetSmallPreviewSize();
            parameterDictControl1.SelectNode("View");
            parameterDictControl1.ElementChanged += ParameterDictControl1_ElementChanged;
            InitLastSessionsPictures();
            InitDefaultScenesPictures();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(600, 200);
        }


        private void ParameterDictControl1_ElementChanged(string name, string value)
        {
            if(GlobalParameters.IsMaterialProperty(name))
            {
                ResultImageView.PublicForm.ActivatePictureArt();
                return;
            }

            if (GlobalParameters.IsSceneProperty(name))
                DrawSmallPreview();
        }


        private void Preview1_ProgressEvent(double progress)
        {
            this.Invoke(new VoidDelegate(UpdateFrontView));
        }


        private void UpdateFrontView()
        {
            preview2.InitLabelImage();
            preview2.Redraw(preview1.Iterate, 7); // Renderer 7 is able to display a front view

        }


        /// <summary>
        /// Some parameter values has changed.
        /// </summary>
        void ParameterValuesChanged()
        {
            preview1.Draw();
            AddToHistory();
            if (tabControl1.SelectedTab == tpSource)
                formulaEditor1.Init();
        }


        /// <summary>
        /// Save current image to _historyImages.
        /// </summary>
        protected void SavePicData()
        {
            _historyImages[_history.Time] = preview1.Image;
        }


        /// <summary>
        /// Legt die aktuellen Parameter in die History ab.
        /// </summary>
        public void SaveHistory()
        {
            _history.CurrentTime = _history.Save();
        }


        /// <summary>
        /// Legt die aktuellen Parameter in die History ab.
        /// </summary>
        public void SaveHistory(string fileName)
        {
            _history.CurrentTime = _history.Save(fileName);
        }


        /// <summary>
        /// Eine globale Variable hat sich geändert.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void Exemplar_EventChanged(object source, ParameterDictChangedEventArgs e)
        {
            UpdateFromData();
        }


        /// <summary>
        /// 
        /// </summary>
        public void UpdateFromData()
        {
            parameterDictControl1.UpdateFromData();
        }


        /// <summary>
        /// Fill mParameter from global ParameterDict.
        /// </summary>
        public void Assign()
        {
            _parameter.SetFromParameterDict();
        }


        /// <summary>
        /// Neuzeichnen über das übergeordentete Control.
        /// </summary>
        private void ForceRedraw()
        {
            ResultImageView.PublicForm.ComputeOneStep();
        }


        /// <summary>
        /// Activate / deactivate some formular elements while rendering.
        /// </summary>
        public bool InComputing
        {
            set
            {
                if (value)
                {
                    btnPreview.Enabled = false;
                    btnStart.Enabled = false;
                    //btnCreatePoster.Enabled = false;
                    btnStop.Enabled = true;
                }
                else
                {
                    //btnStart.Enabled = true;
                    //btnCreatePoster.Enabled = true;
                    btnStop.Enabled = false;
                    ComputationEnds();
                }
            }
        }


        private void ComputationEnds()
        {
            if (!_previewMode || ParameterDict.Current.GetBool("View.Pipeline.UpdatePreview"))
            {
                int updateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
                if (ResultImageView.PublicForm.CurrentUpdateStep < updateSteps)
                {
                    if (_previewMode)
                        ComputePreview();
                    else
                        ResultImageView.PublicForm.ComputeOneStep();
                    return;
                }
            }

            // Use the picture in the render frame to display in preview (for history)
            Image image = ResultImageView.PublicForm.GetImage();
            int imageWidth = preview1.Width;
            int imageHeight = preview1.Height;
            Image newImage = new Bitmap(imageWidth, imageHeight);
            Graphics gr = Graphics.FromImage(newImage);
            gr.DrawImage(image, new Rectangle(0, 0, imageWidth, imageHeight));
            _historyImages[_history.Time] = newImage;
        }


        private void OK()
        {
            Changed = true;
            ResultImageView.PublicForm._inPreview = false;
            ForceRedraw();
        }


        private void tbVar1_TextChanged(object sender, EventArgs e)
        {

        }


        private void tbAngle_TextChanged(object sender, EventArgs e)
        {

        }


        private void tbVar2_TextChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Load last history entry.
        /// </summary>
        private void btnBack_Click_1(object sender, EventArgs e)
        {
            if (_history.Time >= 0)
            {
                _history.CurrentTime = 0;
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        /// <summary>
        /// Load next history entry.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            _history.CurrentTime = _history.Time;
            if (_history.CurrentTime >= 0)
            {
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        /// <summary>
        /// Activate time _history.CurrentTime.
        /// </summary>
        private void LoadFromHistory()
        {
            _history.Load(_history.CurrentTime);
            UpdateHistoryControl();
            UpdateCurrentTab();
        }


        /// <summary>
        /// Update selected tab control.
        /// </summary>
        private void UpdateCurrentTab()
        {
            switch (tabControl1.SelectedTab.Name)
            {
                case "tpNavigate":
                    navigateControl1.UpdateFromChangeProperty();
                    break;
                case "tpSource":
                    formulaEditor1.Init();
                    break;
                case "tpAnimationTop":
                    this.animationControl1.UpdateFromChangeProperty();
                    break;
                case "Data":
                    parameterDictControl1.UpdateFromData();
                    break;
                case "tpMaterial":
                    materialControl1.UpdateFromChangeProperty();
                    break;
            }
        }


        /// <summary>
        /// Redraw history control.
        /// </summary>
        protected void UpdateHistoryControl()
        {
            lblCurrentStep.Text = _history.CurrentTime.ToString();
            tbCurrentStep.Text= _history.CurrentTime.ToString();
            if (_history.CurrentTime > 0)
            {
                btnLastStep.Text = ((int)(_history.CurrentTime - 1)).ToString();
            }
            else
            {
                btnLastStep.Text = "___";
            }
            if (_history.CurrentTime < _history.Time)
            {
                btnNextStep.Text = ((int)(_history.CurrentTime + 1)).ToString();
            }
            else
            {
                btnNextStep.Text = "___";
            }

            btnNext.Text = _history.Time.ToString();
            btnBack.Text = "0";
        }


        /// <summary>
        /// Save ParameterDict.Current as *.gestalt file.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Gestalt|*.gestalt;*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                ParameterDict.Current.Save(sd.FileName);
            }
        }


        /// <summary>
        /// Load scene.
        /// </summary>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Gestalt|*.gestalt;*.xml;*.tomo;*.jpg;*.png|*.*|*.*";
            if (oldDirectory != "")
            {
                od.InitialDirectory = oldDirectory;
            }
            if (od.ShowDialog() == DialogResult.OK)
            {
                LoadScene(od.FileName);
                oldDirectory = System.IO.Path.GetDirectoryName(od.FileName);
            }
            tabControl1.SelectedIndex = 1;
        }


        /// <summary>
        /// Load Gestaltlupe project.
        /// </summary>
        private void LoadScene(string dataFileName)
        {
            if (dataFileName.ToLower().EndsWith(".jpg") || dataFileName.ToLower().EndsWith(".png"))
            {
                string testFile= System.IO.Path.GetDirectoryName(dataFileName)+"/"+ System.IO.Path.GetFileNameWithoutExtension(dataFileName) + ".gestalt";
                if (System.IO.File.Exists(testFile))
                    dataFileName = testFile;
                else
                    dataFileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + System.IO.Path.GetFileNameWithoutExtension(dataFileName) + ".gestalt";
                // Backward compatibility
                if (!System.IO.File.Exists(dataFileName))
                    dataFileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + System.IO.Path.GetFileNameWithoutExtension(dataFileName) + ".tomo";
                if (!System.IO.File.Exists(dataFileName))
                    dataFileName = dataFileName.Replace("Gestaltlupe", "Tomotrace");
            }
            ParameterDict.Current.Load(dataFileName);
            ShowPicture(dataFileName);
            Data.Update();
            parameterDictControl1.UpdateFromData();
            ParameterValuesChanged();
        }


        /// <summary>
        /// Sucht das passende Bild zur Parameterdict-Datei und zeigt es
        /// (wenn die Suche erfolgreich war) in Fenster1 an.
        /// </summary>
        private void ShowPicture(string parameterdictFile)
        {
            if (!parameterdictFile.ToLower().StartsWith(Fractrace.FileSystem.Exemplar.ExportDir.ToLower()))
                return;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(parameterdictFile);
            string tempFileName = fileName.Substring(4); // Data ist vier Zeichen lang.

            int dataPos = tempFileName.IndexOf("pic");
            int picPos = dataPos + 3;

            if (dataPos < 0)
                return;

            string gesDataString = tempFileName.Substring(0, dataPos);
            string gesPicString = tempFileName.Substring(picPos);

            string picDir = System.IO.Path.Combine(Fractrace.FileSystem.Exemplar.ExportDir, "Data" + gesDataString);
            string picFile = System.IO.Path.Combine(picDir, fileName + ".png");

            ResultImageView.PublicForm.ShowPictureFromFile(picFile);
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            StartRendering();
        }


        /// <summary>
        /// Start process of rendering.
        /// </summary>
        public void StartRendering()
        {
            _previewMode = false;
            //mPosterMode = false;
            _history.CurrentTime = _history.Save();
            ResultImageView.PublicForm._inPreview = false;
            ForceRedraw();
        }


        /// <summary>
        /// Berechnung stoppen
        /// </summary>
        private void button27_Click(object sender, EventArgs e)
        {
            Stop();
        }


        private void Stop()
        {
            Fractrace.Scheduler.GrandScheduler.Exemplar.SetBatch(null);
            ResultImageView.PublicForm.Stop();
            animationControl1.Abort();
        }


        private void btnStopAnimation_Click_1(object sender, EventArgs e)
        {
            ResultImageView.PublicForm.Stop();
        }


        /// <summary>
        /// Dialogabfrage vor Beendigung der Anwendung.
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("Close Application?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RemoveEmptyDirectory();
                base.OnClosing(e);
                ResultImageView.PublicForm.ForceClosing();
            }
            else e.Cancel = true;
        }


        /// <summary>
        /// Wenn keine Bilddateien gespeichert wurden, wird das entsprechende Hauptverzeichnis gelöscht.
        /// </summary>
        protected void RemoveEmptyDirectory()
        {
            string dir = FileSystem.Exemplar.ProjectDir;
            if (System.IO.Directory.GetFiles(dir).Length == 0)
                System.IO.Directory.Delete(dir);
        }


        /// <summary>
        /// Tab-Auswahl
        /// </summary>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurrentTab();
        }


        /// <summary>
        /// Ist die zweite Ansicht aktiviert?.
        /// </summary>
        public bool Stereo
        {
            get
            {
                return cbStereo.Checked;
            }
        }


        /// <summary>
        /// Höhe wurde verschoben.
        /// Breite der Preview Controls setzen:
        /// </summary>
        private void splitContainer1_Panel1_ClientSizeChanged(object sender, EventArgs e)
        {
            SetSmallPreviewSize();
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetSmallPreviewSize()
        {

            double winput = ParameterDict.Current.GetDouble("View.Width");
            double hinput = ParameterDict.Current.GetDouble("View.Height");
            double aspectRatio = winput / hinput;

            int width = 110;
            preview2.Width = width;
            preview1.Width = width;

            int height = (int)(width / aspectRatio) + 34;
            this.splitContainer1.SplitterDistance = height - 12;
            preview1.Height = height;
            preview2.Height = height;
        }


        /// <summary>
        /// History wird um die aktuellen Daten erweitert.
        /// </summary>
        private void btnSaveInHistory_Click(object sender, EventArgs e)
        {
            _history.CurrentTime = _history.Save();
            LoadFromHistory();
        }


        /// <summary>
        /// Die aktuellen Parameterdaten werden in die History gespeichert.
        /// </summary>
        public void AddToHistory()
        {
            _history.Save();
            UpdateHistoryControl();
        }


        /// <summary>
        /// Vorgängerschritt wurde ausgewählt.
        /// </summary>
        private void btnLastStep_Click(object sender, EventArgs e)
        {
            btnLastStep.Enabled = true;
            try
            {
                if (_history.CurrentTime > 0)
                {
                    _history.CurrentTime--;
                    preview2.Clear();
                    if (!UpdateHistoryPic())
                    {
                        preview1.Clear();
                    }
                    else
                    {
                        preview1.Visible = true;
                    }
                }
                LoadFromHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            btnLastStep.Enabled = true;
        }


        /// <summary>
        /// Load next history entry.
        /// </summary>
        private void btnNextStep_Click(object sender, EventArgs e)
        {
            btnNextStep.Enabled = false;
            try
            {
                if (_history.CurrentTime < _history.Time)
                {
                    _history.CurrentTime++;
                    preview2.Clear();
                    if (!UpdateHistoryPic())
                    {
                        preview1.Clear();
                    }
                    else
                    {
                        preview1.Visible = true;
                    }
                }
                LoadFromHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            btnNextStep.Enabled = true;
        }


        /// <summary>
        /// Load old image. Return true, if such image exists.
        /// </summary>
        private bool UpdateHistoryPic()
        {
            if (_historyImages.ContainsKey(_history.CurrentTime))
            {
                preview1.Image = _historyImages[_history.CurrentTime];
                preview1.Refresh();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Erstellung des Images im Vorschaufenster wurde beendet.
        /// </summary>
        private void preview1_RenderingEnds()
        {
            SetSmallPreviewSize();
            // Counter is set to the last time entry.
            _history.CurrentTime = _history.Time;
            UpdateHistoryControl();
            SavePicData();
            preview2.InitLabelImage();
            preview2.Redraw(preview1.Iterate, 7); // Renderer 7 is able to display a front view
        }


        /// <summary>
        /// User has click on Preview1 Control. 
        /// </summary>
        private void preview1_Clicked()
        {
            // Counter is set to the last time entry.
            _history.CurrentTime = _history.Time;
            UpdateHistoryControl();
            SavePicData();
        }


        /// <summary>
        /// Berechnung anhalten
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (btnPause.Text == "Pause")
            {
                Fractrace.Iterate.Pause = true;
                btnPause.Text = "Run";
            }
            else
            {
                Fractrace.Iterate.Pause = false;
                btnPause.Text = "Pause";
            }
        }


        /// <summary>
        /// Start rendering in the small preview control.
        /// </summary>
        public void DrawSmallPreview()
        {
            preview1.Draw();
            AddToHistory();
        }


        /// <summary>
        /// Handles the Click event of the btnLoadLast control.
        /// Das letzte Projekt wird geladen.
        /// </summary>
        private void btnLoadLast_Click(object sender, EventArgs e)
        {
            string exportDir = FileSystem.Exemplar.ExportDir;
            exportDir = System.IO.Path.Combine(exportDir, "data");
            exportDir = System.IO.Path.Combine(exportDir, "parameters");
            if (System.IO.Directory.Exists(exportDir))
            {
                DateTime maxDateTime = DateTime.MinValue;
                string fileName = "";
                foreach (string file in System.IO.Directory.GetFiles(exportDir))
                {
                    DateTime dt = System.IO.File.GetCreationTime(file);
                    if (dt > maxDateTime)
                    {
                        maxDateTime = dt;
                        fileName = file;
                    }
                }
                if (fileName != "")
                {
                    LoadConfiguration(fileName);
                }
            }
            tabControl1.SelectedIndex = 1;
        }


        /// <summary>
        /// Projektdatei wird geladen und (falls ein Bild existiert) angezeigt.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void LoadConfiguration(string fileName)
        {
            ParameterDict.Current.Load(fileName);
            ShowPicture(fileName);
            Data.Update();
            parameterDictControl1.UpdateFromData();
            ParameterValuesChanged();
            oldDirectory = System.IO.Path.GetDirectoryName(fileName);
        }


        /// <summary>
        /// Stop the to the preview control assigned iter
        /// </summary>
        public void AbortPreview()
        {
            ResultImageView.PublicForm.Stop();
        }


        public void ComputePreview()
        {
            _previewMode = true;
            {
                _history.CurrentTime = _history.Save();
                // Size und Raster festlegen
                string sizeStr = ParameterDict.Current["View.Size"];
                ParameterDict.Current["View.Size"] = "0.2";
                ResultImageView.PublicForm._inPreview = true;
                ForceRedraw();
                ResultImageView.PublicForm._inPreview = false;
                // Size und Raster auf die Ursprungswerte setzen
                ParameterDict.Current["View.Size"] = sizeStr;
            }
        }


        /// <summary>
        /// Compute preview.
        /// </summary>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            ComputePreview();
        }


        /// <summary>
        /// Handles the Click event of the btnPred control.
        /// Go to the last entry with generated bitmap.
        /// </summary>
        private void btnPred_Click(object sender, EventArgs e)
        {
            btnPred.Enabled = true;
            preview1.Visible = true;
            try
            {
                while (_history.CurrentTime > 0)
                {
                    _history.CurrentTime--;
                    if (UpdateHistoryPic())
                        break;
                }
                LoadFromHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            btnPred.Enabled = true;
        }


        /// <summary>
        /// Activate next history entry.
        /// </summary>
        private void button1_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = false;
            preview1.Visible = true;
            try
            {
                while (_history.CurrentTime < _history.Time)
                {
                    _history.CurrentTime++;
                    if (UpdateHistoryPic())
                        break;
                }
                LoadFromHistory();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            button1.Enabled = true;
        }


        /// <summary>
        /// Handles the Click event of the preview1 control.
        /// </summary>
        private void PreviewButton_Click(object sender, EventArgs e)
        {
            _history.CurrentTime = _history.Save();
            SetSmallPreviewSize();
            preview1.btnPreview_Click(sender, e);
        }


        /// <summary>
        /// Add file data to the current data. 
        /// </summary>
        private void btnAppend_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Gestalt|*.gestalt;*.xml;*.tomo|*.*|*.*";
            if (oldDirectory != "")
            {
                od.InitialDirectory = oldDirectory;
            }
            if (od.ShowDialog() == DialogResult.OK)
            {
                ParameterDict.Current.Append(od.FileName);
                ShowPicture(od.FileName);
                Data.Update();
                parameterDictControl1.UpdateFromData();
                ParameterValuesChanged();
                oldDirectory = System.IO.Path.GetDirectoryName(od.FileName);
            }
        }


        /// <summary>
        /// Save only Gestalt parameters. 
        /// </summary>
        private void btnSaveFormula_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.gestalt|*.gestalt;*.xml;*.tomo|*.tomo|*.tomo|*.*|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                List<string> formulaSettingCategories = new List<string>();
                formulaSettingCategories.Add("Scene");
                formulaSettingCategories.Add("Transformation.Camera");
                formulaSettingCategories.Add("Transformation.Perspective");
                formulaSettingCategories.Add("Formula");
                formulaSettingCategories.Add("Intern.Formula");
                formulaSettingCategories.Add("Intern.Version");
                formulaSettingCategories.Add("Renderer.Color");
                ParameterDict.Current.Save(sd.FileName, formulaSettingCategories);
            } 
        }


        /// <summary>
        /// Handles the Click event of the btBulk control.
        /// The formula parameters and the formula itself are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        private void btBulk_Click(object sender, EventArgs e)
        {
            tbInfoOutput.Text = InfoGenerator.GenerateCompressedFormula();
        }


        /// <summary>
        /// Display the documentation.
        /// </summary>
        private void btnShowDocumentation_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Gestaltlupe.chm"));
        }


        /// <summary>
        /// The formula parameters, the formula itself and diplay settings are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        private void btnCreateCompleteBulk_Click(object sender, EventArgs e)
        {
            tbInfoOutput.Text = InfoGenerator.GenerateCompressedFormulaAndViewSettings();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Animation.AnimationControl.MainAnimationControl.AddCurrentHistoryEntry();
        }


        /// <summary>
        /// Buttons are activated/deactivated as if start where running.
        /// </summary>
        public void SetButtonsToStart()
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPause.Enabled = true;
            btnCreatePoster.Enabled = false;
            btnPreview.Enabled = false;
        }


        /// <summary>
        /// Buttons are activated/deactivated as if start has just ended.
        /// </summary>
        public void SetButtonsToStop()
        {
            //btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnPause.Enabled = true;
            btnCreatePoster.Enabled = true;
            btnPreview.Enabled = true;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            ResultImageView.PublicForm.ActivatePictureArt();
        }


        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            try
            {
                Application.DoEvents();
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "VRML|*.wrl|Web|*.xhtml|*.*|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    if (ResultImageView.PublicForm.LastPicturArt == null)
                    {
                        MessageBox.Show("No Surface Data available.");
                        btnExport.Enabled = true;
                        return;
                    }
                    if(sd.FileName.ToLower().EndsWith(".html"))
                    {
                        Fractrace.SceneGraph.WebGlExporter exporter = new SceneGraph.WebGlExporter(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                        exporter.Export(sd.FileName);
                    }
                    else
                    if (sd.FileName.ToLower().EndsWith(".xhtml"))
                    {
                        Fractrace.SceneGraph.X3DomExporter exporter = new SceneGraph.X3DomExporter(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                        exporter.Init(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                        exporter.Update(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                        exporter.Export(sd.FileName);
                    }
                    else
                    {
                        Fractrace.SceneGraph.VrmlSceneExporter exporter = new SceneGraph.VrmlSceneExporter(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                    exporter.Export(sd.FileName);
                    }
                    Fractrace.Gui.ExportResultDialog exportResultDialog = new Gui.ExportResultDialog(sd.FileName);
                    exportResultDialog.ShowDialog();

                    if(exportResultDialog.OpenInBrowser)
                    {
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            btnExport.Enabled = true;
        }


        private void tbCurrentStep_TextChanged(object sender, EventArgs e)
        {
            // TODO: Load editet entry.
            int currentStep = 0;
            if(int.TryParse(tbCurrentStep.Text,out currentStep))
            {
                if (_history.CurrentTime != currentStep)
                {
                    _history.CurrentTime = currentStep;
                    UpdateHistoryPic();
                    LoadFromHistory();
                }
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (_history.Time >= 0)
            {
                _history.CurrentTime = 0;
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            _history.CurrentTime = _history.Time;
            if (_history.CurrentTime >= 0)
            {
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }

        public void EnableRepaint(bool enable)
        {
            button4.Enabled = enable;
            Application.DoEvents();
        }

        public void EnableStartButton(bool enable)
        {
            btnPreview.Enabled = enable;
            btnStart.Enabled = enable;
            Application.DoEvents();
        }


        private void btnLoadExamples_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "Gestalt|*.gestalt;*.xml;*.tomo;*.jpg;*.png|*.*|*.*";     
            od.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Scenes");     
            if (od.ShowDialog() == DialogResult.OK)
            {
                LoadScene(od.FileName);
                tabControl1.SelectedIndex = 1;
            }    
        }


        private void btnBatchExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.wrl|*.wrl|Web|*.xhtml";
            if (sd.ShowDialog() == DialogResult.OK)
            {

                if (sd.FileName.ToLower().EndsWith(".xhtml"))
                {
                    Fractrace.Scheduler.BatchProcess.X3DomExportBatchProcess x3DomExportBatchProcess = new Scheduler.BatchProcess.X3DomExportBatchProcess();
                    x3DomExportBatchProcess.ExportFile = sd.FileName;
                    Fractrace.Scheduler.GrandScheduler.Exemplar.SetBatch(x3DomExportBatchProcess);
                    StartRendering();
                }
                else if (sd.FileName.ToLower().EndsWith(".wrl"))
                {
                    Fractrace.Scheduler.BatchProcess.X3dExportBatchProcess x3dExportBatchProcess = new Scheduler.BatchProcess.X3dExportBatchProcess();
                    x3dExportBatchProcess.ExportFile = sd.FileName;
                    Fractrace.Scheduler.GrandScheduler.Exemplar.SetBatch(x3dExportBatchProcess);
                    StartRendering();
                }
                else
                {
                    MessageBox.Show("Unknown Fileformat.");
                }
            }
        }


        private void btnAddToAnimation_Click(object sender, EventArgs e)
        {
            this.animationControl1.AddToAnimation();
        }


        /// <summary>
        /// Show images of last Sessions.
        /// </summary>
        private void InitLastSessionsPictures()
        {
            PictureBox pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Left;
            this.panel33.Controls.Add(pictureBox);
            
            string exportDir = FileSystem.Exemplar.ExportDir;
            exportDir = System.IO.Path.Combine(exportDir, "data");
            exportDir = System.IO.Path.Combine(exportDir, "parameters");
            if (System.IO.Directory.Exists(exportDir))
            {
                DateTime maxDateTime = DateTime.MinValue;
                string fileName = "";
                foreach (string file in System.IO.Directory.GetFiles(exportDir))
                {
                    DateTime dt = System.IO.File.GetCreationTime(file);
                    if (dt > maxDateTime)
                    {
                        maxDateTime = dt;
                        fileName = file;
                    }
                }
                if (fileName != "")
                {
                    string imageFileId = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    int did = imageFileId.IndexOf("pic");
                    if(did>0)
                    { 
                    string fileDir = imageFileId.Substring(0, did);
                        string imageFile = fileName.Replace("\\data\\parameters\\", "\\"+fileDir+"\\");
                        imageFile = imageFile.Replace(".gestalt", ".png");
                        if(System.IO.File.Exists(imageFile))
                        {
                            Image image = Image.FromFile(imageFile);
                            pictureBox.Width = 100 * image.Width / image.Height;
                            Size size = new Size(pictureBox.Width, 100);
                            pictureBox.Image = (Image)(new Bitmap(image, size)); // TODO: Consider aspect ratio
                            pictureBox.Tag = fileName;
                            Graphics graphics = Graphics.FromImage(pictureBox.Image);
                            
                            this.Refresh();
                            this.WindowState = FormWindowState.Normal;
                        }
                    }
                }
            }
            pictureBox.Click += PictureBox_Click;
        }


        /// <summary>
        /// Gernerate buttons to load some default scenes.
        /// </summary>
        private void InitDefaultScenesPictures()
        {
            List<string> currentDirs = new List<string>();
            Dictionary<string, string> gestaltFiles = new Dictionary<string, string>();
            currentDirs.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Scenes"));

            int currentXpos = 0;
            int currentYpos = 6;
            int bordersize = 6;
            int maxXpos = 480;
            int maxYpos = 160;

            Random rand = new Random();
            SortedDictionary<double, string> files = new SortedDictionary<double, string>();
            while (currentDirs.Count > 0)
            {
                List<string> subDirs = new List<string>();
                foreach (string currentDir in currentDirs)
                {
                    if (System.IO.Directory.Exists(currentDir))
                    {
                        foreach (string subDirectory in System.IO.Directory.GetDirectories(currentDir))
                        {
                            subDirs.Add(subDirectory);
                        }
                        foreach (string imageFile in System.IO.Directory.GetFiles(currentDir, "*.png"))
                        {
                            files[rand.NextDouble()] = imageFile;
                            gestaltFiles[imageFile] = currentDir + "/" + System.IO.Path.GetFileNameWithoutExtension(imageFile) + ".gestalt";
                        }
                    }
                }
                currentDirs = subDirs;
            }
            foreach(KeyValuePair<double,string> fileEntry in files)
            {
                string imageFile = fileEntry.Value;
                System.Diagnostics.Debug.WriteLine(imageFile);
                string gestaltFile = gestaltFiles[imageFile];

                PictureBox pictureBox = new PictureBox();
                pictureBox.Left = currentXpos;
                pictureBox.Top = currentYpos;
                this.panel30.Controls.Add(pictureBox);

                Image image = Image.FromFile(imageFile);
                pictureBox.Width = 100 * image.Width / image.Height;
                pictureBox.Height = 100;
                Size size = new Size(pictureBox.Width, 100);
                pictureBox.Image = (Image)(new Bitmap(image, size)); // TODO: Consider aspect ratio
                pictureBox.Tag = gestaltFile;
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                pictureBox.Click += PictureBox_Click;

                currentXpos += pictureBox.Width + bordersize;
                if (currentXpos > maxXpos)
                {
                    currentXpos = 0;
                    currentYpos += 100 + bordersize;
                    if (currentYpos > maxYpos)
                        return;
                }
            }
        }


        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            LoadScene(pictureBox.Tag.ToString());
        }


        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "Image|*.png";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                ResultImageView.PublicForm.GetImage().Save(sd.FileName);
            }
        }

        /// <summary>
        /// Show navigation tab.
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            tabControl2.SelectedIndex = 0;
        }

        /// <summary>
        /// Show material tab.
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            tabControl2.SelectedIndex = 0;
        }

        /// <summary>
        /// Show formula parameters.
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            parameterDictControl1.SelectTreeNode("Formula");
            tabControl2.SelectedIndex = 0;
        }

        /// <summary>
        /// Show view parameters.
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            parameterDictControl1.SelectTreeNode("View");
            tabControl2.SelectedIndex = 0;
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ;


           // TabPage tabPage = (TabPage)sender;
            switch(tabControl2.SelectedTab.Name)
            {

                case "topTbFiles":
                    tabControl1.SelectedIndex = 4;
                    break;

                case "topTbFormula":
                    tabControl1.SelectedIndex = 3;
                    break;

                case "tpAnimation":
                    //tabControl1.SelectedIndex = 5;
                    break;

                case "tpRender":
                    //tabControl1.SelectedIndex = 0;
                    //parameterDictControl1.SelectTreeNode("Renderer");
                    break;

                case "tpSettings":
                    tabControl1.SelectedIndex = 0;
                    break;

                case "tpSpecial":
                    tabControl1.SelectedIndex = 0;
                    parameterDictControl1.SelectTreeNode("Export");
                    break;

            }




        }

        private void btnDocumentation_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
        }

        private void btnExtras_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            this.formulaEditor1.Format();
        }

        private void btnAnimation_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }
    }
}
