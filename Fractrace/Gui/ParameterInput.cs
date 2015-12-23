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
        /// Initializes a new instance of the <see cref="ParameterInput"/> class.
        /// </summary>
        public ParameterInput()
        {
            MainParameterInput = this;
            InitializeComponent();
            ParameterDict.Current.EventChanged += new ParameterDictChanged(Exemplar_EventChanged);
            navigateControl1.Init(preview1, preview2, this);
            this.animationControl1.Init(mHistory);
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
            tabControl1.SelectedIndex = 1;
            SetSmallPreviewSize();
            parameterDictControl1.SelectNode("View");
            parameterDictControl1.ElementChanged += ParameterDictControl1_ElementChanged;
        }


        private void ParameterDictControl1_ElementChanged(string name, string value)
        {

            if(GlobalParameters.IsMaterialProperty(name))
            {
                Form1.PublicForm.ActivatePictureArt();
                return;
            }

            if (GlobalParameters.NeedRecalculateAspect(name))
              Navigator.SetAspectRatio();

            if (GlobalParameters.IsSceneProperty(name))
                DrawSmallPreview();

        }

        /// <summary>
        /// Public Acces to DataViewControl.
        /// </summary>
        public DataViewControl MainDataViewControl { get { return parameterDictControl1.MainDataViewControl; } }


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
            if (cbAutomaticSave.Checked)
            {
                // TODO: only add, if picture in preview1 exists
                AddToHistory();
            }
            // Update the source code.
            if (tabControl1.SelectedTab == tpSource)
                formulaEditor1.Init();
        }


        /// <summary>
        /// Das berechnete Bild wird für die spätere Verwendung gespeichert.
        /// </summary>
        protected void SavePicData()
        {
            mHistoryImages[mHistory.Time] = preview1.Image;
        }


        /// <summary>
        /// Legt die aktuellen Parameter in die History ab.
        /// </summary>
        public void SaveHistory()
        {
            mHistory.CurrentTime = mHistory.Save();
        }


        /// <summary>
        /// Legt die aktuellen Parameter in die History ab.
        /// </summary>
        public void SaveHistory(string fileName)
        {
            mHistory.CurrentTime = mHistory.Save(fileName);
        }


        /// <summary>
        /// Get in historyControl used parameter history.
        /// </summary>
        public ParameterHistory History
        {
            get
            {
                return mHistory;
            }
        }


        /// <summary>
        /// Enthält die History der letzten Parameter
        /// </summary>
        ParameterHistory mHistory = new ParameterHistory();


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
            mParameter.start_tupel.x = ParameterDict.Current.GetDouble("Border.Min.x");
            mParameter.start_tupel.y = ParameterDict.Current.GetDouble("Border.Min.y");
            mParameter.start_tupel.z = ParameterDict.Current.GetDouble("Border.Min.z");
            mParameter.end_tupel.x = ParameterDict.Current.GetDouble("Border.Max.x");
            mParameter.end_tupel.y = ParameterDict.Current.GetDouble("Border.Max.y");
            mParameter.end_tupel.z = ParameterDict.Current.GetDouble("Border.Max.z");
            mParameter.arc.x = ParameterDict.Current.GetDouble("Transformation.AngleX");
            mParameter.arc.y = ParameterDict.Current.GetDouble("Transformation.AngleY");
            mParameter.arc.z = ParameterDict.Current.GetDouble("Transformation.AngleZ");
        }


        /// <summary>
        /// Zugriff auf die Bearbeitungsparameter.
        /// </summary>
        private FracValues mParameter = new FracValues();

        public FracValues Parameter
        {
            get
            {
                return mParameter;
            }
        }


        /// <summary>
        /// Iterationstiefe
        /// </summary>
        public int Cycles
        {
            get
            {
                return (int)ParameterDict.Current.GetDouble("Formula.Static.Cycles");
            }
        }


        /// <summary>
        /// Pixelgröße
        /// </summary>
        public int Raster
        {
            get
            {
                return (int)ParameterDict.Current.GetDouble("View.Raster");
            }
        }


        /// <summary>
        /// Faktor der Fenstergröße.
        /// </summary>
        public double ScreenSize
        {
            get
            {
                return ParameterDict.Current.GetDouble("View.Size");
            }
        }


        /// <summary>
        /// Index der zu berechnenden Formel.
        /// </summary>
        public int Formula
        {
            get
            {
                return (int)ParameterDict.Current.GetDouble("Formula.Static.Formula");
            }
        }


        public bool AutomaticSaveInAnimation
        {
            get
            {
                return cbAutomaticSaveAnimation.Checked;
            }
        }


        /// <summary>
        /// Neuzeichnen über das übergeordentete Control.
        /// </summary>
        private void ForceRedraw()
        {
            Form1.PublicForm.ComputeOneStep();
            if (Stereo)
                DrawStereo();
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
                    btnStart.Enabled = false;
                    btnCreatePoster.Enabled = false;
                    btnStop.Enabled = true;
                }
                else
                {
                    btnStart.Enabled = true;
                    btnCreatePoster.Enabled = true;
                    btnStop.Enabled = false;
                    ComputationEnds();
                }
            }
        }


        private void ComputationEnds()
        {

            if (!mPreviewMode || ParameterDict.Current.GetBool("View.Pipeline.UpdatePreview"))
            {
                int updateSteps = ParameterDict.Current.GetInt("View.UpdateSteps");
                if (Form1.PublicForm.CurrentUpdateStep < updateSteps)
                {
                    if (mPreviewMode)
                        ComputePreview();
                    else
                        Form1.PublicForm.ComputeOneStep();
                    return;
                }
            }

            if (mPosterMode)
            {
                DrawNextPosterPart();
            }
            else
            {
                // Use the picture in the render frame to display in preview (for history)
                Image image = Form1.PublicForm.GetImage();
                int imageWidth = preview1.Width;
                int imageHeight = preview1.Height;
                Image newImage = new Bitmap(imageWidth, imageHeight);
                Graphics gr = Graphics.FromImage(newImage);
                gr.DrawImage(image, new Rectangle(0, 0, imageWidth, imageHeight));
                mHistoryImages[mHistory.Time] = newImage;
            }
        }

        public void DrawStereo()
        {
            if (mStereoForm == null)
            {
                mStereoForm = new StereoForm();
                mStereoForm.Show();
            }
            mStereoForm.ImageRenderer.Draw();
        }


        public StereoForm StereoForm
        {
            get
            {
                return mStereoForm;
            }
        }

        private StereoForm mStereoForm = null;

        /// <summary>
        /// 
        /// </summary>
        public bool Changed = false;



        private void OK()
        {
            Changed = true;
            Form1.PublicForm._inPreview = false;
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
        /// Verweis auf den Bezug zu Data
        /// </summary>
        //   private int mHistoryTime = 0;


        /// <summary>
        /// Der letzte Eintrag der History wird geladen (wenn möglich)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click_1(object sender, EventArgs e)
        {
            if (mHistory.Time >= 0)
            {
                mHistory.CurrentTime = 0;
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        /// <summary>
        /// Der nächste Eintrag der History wird geladen (wenn möglich)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //SavePicData(); 
            mHistory.CurrentTime = mHistory.Time;
            if (mHistory.CurrentTime >= 0)
            {
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        /// <summary>
        /// Die Parameterdaten zum Zeitpunkt mHistoryTime werden geladen und dargestellt.
        /// </summary>
        private void LoadFromHistory()
        {
            //ParameterValuesChanged(); // Bild retten
            //SavePicData();  // Bild retten
            mHistory.Load(mHistory.CurrentTime);
            UpdateHistoryControl();
            //  UpdateFromData();


            UpdateCurrentTab();

            // TODO: Bild aktualisieren

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
                case "Data":
                    parameterDictControl1.UpdateFromData();
                    break;
            }
        }


        /// <summary>
        /// Das Control zur Übersicht der historischen Daten wird neu geladen.
        /// </summary>
        protected void UpdateHistoryControl()
        {
            lblCurrentStep.Text = mHistory.CurrentTime.ToString();
            tbCurrentStep.Text= mHistory.CurrentTime.ToString();
            if (mHistory.CurrentTime > 0)
            {
                btnLastStep.Text = ((int)(mHistory.CurrentTime - 1)).ToString();
            }
            else
            {
                btnLastStep.Text = "___";
            }
            if (mHistory.CurrentTime < mHistory.Time)
            {
                btnNextStep.Text = ((int)(mHistory.CurrentTime + 1)).ToString();
            }
            else
            {
                btnNextStep.Text = "___";
            }

            btnNext.Text = mHistory.Time.ToString();
            btnBack.Text = "0";
        }


        /// <summary>
        /// Der Inhalt des Parameterdict wird gespeichert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.xml|*.xml;*.tomo|*.tomo|*.tomo|*.*|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                ParameterDict.Current.Save(sd.FileName);
            }
        }


        /// <summary>
        /// Damit wird vermieden, dass nach dem Export von 3D Daten stets beim Öffnen das Exportverzeichnis
        /// als InitialDirectory verwendet wird. 
        /// </summary>
        protected static string oldDirectory = "";


        /// <summary>
        /// Konfiguration öffnen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "tomofile|*.xml;*.tomo;*.jpg;*.png|*.*|*.*";
            if (oldDirectory != "")
            {
                od.InitialDirectory = oldDirectory;
            }
            if (od.ShowDialog() == DialogResult.OK)
            {
                LoadScene(od.FileName);
                oldDirectory = System.IO.Path.GetDirectoryName(od.FileName);
            }
        }


        /// <summary>
        /// Load Gestaltlupe project.
        /// </summary>
        private void LoadScene(string dataFileName)
        {
            if (dataFileName.ToLower().EndsWith(".jpg") || dataFileName.ToLower().EndsWith(".png"))
            {
                dataFileName = FileSystem.Exemplar.ExportDir + "/data/parameters/" + System.IO.Path.GetFileNameWithoutExtension(dataFileName) + ".tomo";
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
        /// 
        /// </summary>
        /// <param name="parameterdictFilen"></param>
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

            Form1.PublicForm.ShowPictureFromFile(picFile);


        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            mPreviewMode = false;
            mPosterMode = false;
            // Todo: Bild nur speichern, wenn der Haken gesetzt ist
            if (cbSaveHistory.Checked)
            {
                mHistory.CurrentTime = mHistory.Save();
            }
            Form1.PublicForm._inPreview = false;
            ForceRedraw();

        }


        /// <summary>
        /// Berechnung stoppen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27_Click(object sender, EventArgs e)
        {
            mPosterMode = false;
            Form1.PublicForm.Stop();
            if (mStereoForm != null)
            {
                mStereoForm.Abort();
            }
            animationControl1.Abort();
        }



        private void btnStopAnimation_Click_1(object sender, EventArgs e)
        {
            Form1.PublicForm.Stop();
        }


        /// <summary>
        /// Dialogabfrage vor Beendigung der Anwendung.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("Close Application?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RemoveEmptyDirectory();
                base.OnClosing(e);
                Form1.PublicForm.ForceClosing();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurrentTab();

            /*
            if (tabControl1.SelectedTab == Data)
            {
                parameterDictControl1.UpdateFromData();
            }
            if (tabControl1.SelectedTab == tpSource)
                formulaEditor1.Init();

            switch (tabControl1.SelectedTab.Name)
            {
                case "tpNavigate":
                    navigateControl1.UpdateFromChangeProperty();
                    break;
            }
            */

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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveInHistory_Click(object sender, EventArgs e)
        {
            mHistory.CurrentTime = mHistory.Save();
            LoadFromHistory();
        }


        /// <summary>
        /// Die aktuellen Parameterdaten werden in die History gespeichert.
        /// </summary>
        public void AddToHistory()
        {
            mHistory.Save();
            UpdateHistoryControl();
        }

        /// <summary>
        /// Vorgängerschritt wurde ausgewählt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLastStep_Click(object sender, EventArgs e)
        {
            btnLastStep.Enabled = true;
            try
            {
                if (mHistory.CurrentTime > 0)
                {
                    mHistory.CurrentTime--;
                    preview2.Clear();
                    if (!UpdateHistoryPic())
                    {
                        // Not working:
                        //Graphics gr = Graphics.FromImage(preview1.Image);
                        //gr.DrawRectangle(new System.Drawing.Pen(Color.Gray, 4), 10, 10, 20, 20);
                        //preview1.Refresh();
                        preview1.Clear();
                        //preview1.Visible = false;
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


        Dictionary<int, Image> mHistoryImages = new Dictionary<int, Image>();


        /// <summary>
        /// Nächster History-Eintrag wird geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextStep_Click(object sender, EventArgs e)
        {
            btnNextStep.Enabled = false;
            try
            {
                if (mHistory.CurrentTime < mHistory.Time)
                {
                    mHistory.CurrentTime++;
                    preview2.Clear();
                    if (!UpdateHistoryPic())
                    {
                        preview1.Clear();
                        //preview1.Visible = false;
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
            if (mHistoryImages.ContainsKey(mHistory.CurrentTime))
            {
                preview1.Image = mHistoryImages[mHistory.CurrentTime];
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
            mHistory.CurrentTime = mHistory.Time;
            UpdateHistoryControl();
            SavePicData();
            //preview2.Draw();
            preview2.InitLabelImage();
            preview2.Redraw(preview1.Iterate, 7); // Renderer 7 is able to display a front view
        }


        /// <summary>
        /// User has click on Preview1 Control. 
        /// </summary>
        private void preview1_Clicked()
        {
            // Counter is set to the last time entry.
            mHistory.CurrentTime = mHistory.Time;
            UpdateHistoryControl();
            SavePicData();
        }


        /// <summary>
        /// Ein anderer Karteikastenreiter wurde ausgewählt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab == tpRender)
            {
                // View-Eigenschaften bei den globalen Einstellungen auswählen.
                this.parameterDictControl1.SelectNode("View");
            }

        }


        protected int mPosterStep = 0;

        protected bool mPosterMode = false;


        public void DeactivatePreview()
        {
            mPreviewMode = false;
        }

        protected bool mPreviewMode = false;


        /// <summary>
        /// Erstellung eines Posters wurde angeklickt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreatePoster_Click(object sender, EventArgs e)
        {
            mPosterStep = 0;
            mPosterMode = true;
            DrawNextPosterPart();
        }

        /// <summary>
        /// Erstellt das nächste Einzelbild des Posters.
        /// </summary>
        private void DrawNextPosterPart()
        {
            if (!mPosterMode)
                return;
            int xi = 0;
            int yi = 0;
            switch (mPosterStep)
            {
                case 0:
                    xi = -1;
                    yi = -1;
                    break;
                case 1:
                    xi = 0;
                    yi = -1;
                    break;
                case 2:
                    xi = 1;
                    yi = -1;
                    break;
                case 3:
                    xi = -1;
                    yi = 0;
                    break;
                case 4:
                    xi = 0;
                    yi = 0;
                    break;
                case 5:
                    xi = 1;
                    yi = 0;
                    break;
                case 6:
                    xi = -1;
                    yi = 1;
                    break;
                case 7:
                    xi = 0;
                    yi = 1;
                    break;
                case 8:
                    xi = 1;
                    yi = 1;
                    break;
                case 9:
                    // Ende
                    mPosterStep = 0;
                    mPosterMode = false;
                    ParameterDict.Current.SetInt("View.PosterX", 0);
                    ParameterDict.Current.SetInt("View.PosterZ", 0);
                    return;
            }

            ParameterDict.Current.SetInt("View.PosterX", xi);
            ParameterDict.Current.SetInt("View.PosterZ", yi);
            Form1.PublicForm._inPreview = false;
            ForceRedraw();
            mPosterStep++;
        }

        private void navigateControl1_Load_1(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Berechnung anhalten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
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
            Form1.PublicForm.Stop();
        }



        public void ComputePreview()
        {
            mPreviewMode = true;
            {
                mPosterMode = false;
                // Todo: Bild nur speichern, wenn der Haken gesetzt ist
                //if (cbSaveHistory.Checked)
                {
                    mHistory.CurrentTime = mHistory.Save();
                }
                // Size und Raster festlegen
                string sizeStr = ParameterDict.Current["View.Size"];
                ParameterDict.Current["View.Size"] = "0.2";
                Form1.PublicForm._inPreview = true;
                ForceRedraw();
                Form1.PublicForm._inPreview = false;
                // Size und Raster auf die Ursprungswerte setzen
                ParameterDict.Current["View.Size"] = sizeStr;
            }
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            ComputePreview();
        }



        /// <summary>
        /// Handles the Click event of the btnPred control.
        /// Go to the last entry with generated bitmap.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnPred_Click(object sender, EventArgs e)
        {
            btnPred.Enabled = true;
            preview1.Visible = true;
            try
            {
                while (mHistory.CurrentTime > 0)
                {
                    mHistory.CurrentTime--;
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
        /// Handles the 1 event of the button1_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = false;
            preview1.Visible = true;
            try
            {
                while (mHistory.CurrentTime < mHistory.Time)
                {
                    mHistory.CurrentTime++;
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PreviewButton_Click(object sender, EventArgs e)
        {
            mHistory.CurrentTime = mHistory.Save();
            SetSmallPreviewSize();
            preview1.btnPreview_Click(sender, e);
        }


        /// <summary>
        /// Add file data to the current data. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAppend_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "*.xml|*.xml;*.tomo|*.*|*.*";
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveFormula_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.xml|*.xml;*.tomo|*.tomo|*.tomo|*.*|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                List<string> formulaSettingCategories = new List<string>();
                formulaSettingCategories.Add("Border");
                // The both following parameters has always to be saved if Border is saved.
                formulaSettingCategories.Add("View.Width");
                formulaSettingCategories.Add("View.Height");
                formulaSettingCategories.Add("Border");
                formulaSettingCategories.Add("View.Perspective");
                formulaSettingCategories.Add("Border");
                formulaSettingCategories.Add("Transformation");
                formulaSettingCategories.Add("Formula");
                formulaSettingCategories.Add("Intern.Formula");
                ParameterDict.Current.Save(sd.FileName, formulaSettingCategories);
            }
        }


        /// <summary>
        /// Handles the Click event of the btBulk control.
        /// The formula parameters and the formula itself are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btBulk_Click(object sender, EventArgs e)
        {
            tbInfoOutput.Text = InfoGenerator.GenerateCompressedFormula();
        }


        /// <summary>
        /// Display the documentation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowDocumentation_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(this, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Gestaltlupe.chm"));
        }


        /// <summary>
        /// The formula parameters, the formula itself and diplay settings are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnPause.Enabled = true;
            btnCreatePoster.Enabled = true;
            btnPreview.Enabled = true;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Form1.PublicForm.ActivatePictureArt();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            btnExport.Enabled = false;
            Application.DoEvents();
            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "*.wrl|*.wrl|*.*|all";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                X3dExporter export = new X3dExporter(Form1.PublicForm.Iterate);
                export.Save(sd.FileName);
            }
            btnExport.Enabled = true;
        }


        private void tbCurrentStep_TextChanged(object sender, EventArgs e)
        {
            // TODO: Load editet entry.
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (mHistory.Time >= 0)
            {
                mHistory.CurrentTime = 0;
                UpdateHistoryPic();
                LoadFromHistory();
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            mHistory.CurrentTime = mHistory.Time;
            if (mHistory.CurrentTime >= 0)
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

        private void btnLoadExamples_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "tomofile|*.xml;*.tomo;*.jpg;*.png|*.*|*.*";     
            od.InitialDirectory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Scenes");
          
            if (od.ShowDialog() == DialogResult.OK)
            {
                LoadScene(od.FileName);
            }
        }
    }
}
