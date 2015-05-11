﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fractrace.DataTypes;
using Fractrace.Basic;

namespace Fractrace
{


    /// <summary>
    /// Main window (as viewed by the user), the main window of this application is Form1 (which
    /// display the rendered image).
    /// </summary>
    public partial class ParameterInput : Form
    {


        /// <summary>
        /// Global instance of this unique window.
        /// </summary>
        public static ParameterInput MainParameterInput = null;


        /// <summary>
        /// used in Redraw Picture 
        /// </summary>
        protected int currentPic = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInput"/> class.
        /// </summary>
        public ParameterInput()
        {
            MainParameterInput = this;
            InitializeComponent();
            ParameterDict.Exemplar.EventChanged += new ParameterDictChanged(Exemplar_EventChanged);
            navigateControl1.Init(preview1, preview2, this);
            this.animationControl1.Init(mHistory);
            preview1.PreviewButton.Click += new EventHandler(PreviewButton_Click);
            string assembyInfo = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            string[] infos = assembyInfo.Split(',');
            string version = "";
            if (infos.Length > 1)
                version = infos[1];
            this.Text = "Gestaltlupe" + version + "    [" + System.IO.Path.GetFileName(FileSystem.Exemplar.ProjectDir) + "]";
            tabControl1.SelectedIndex = 1;
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
            Console.WriteLine("Save Pic to Time " + mHistory.Time.ToString());
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


        protected bool inSetParameters = false;


        /// <summary>
        /// 
        /// </summary>
        public void UpdateFromData()
        {
            parameterDictControl1.UpdateFromData();
        }


        /// <summary>
        /// Das Feld Parameter wird aus dem ParameterDict gelesen.
        /// </summary>
        public void Assign()
        {
            mParameter.start_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            mParameter.start_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            mParameter.start_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            mParameter.start_tupel.zz = ParameterDict.Exemplar.GetDouble("Border.Min.zz");
            mParameter.end_tupel.x = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            mParameter.end_tupel.y = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            mParameter.end_tupel.z = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            mParameter.start_tupel.zz = ParameterDict.Exemplar.GetDouble("Border.Max.zz");
            mParameter.arc.x = ParameterDict.Exemplar.GetDouble("Transformation.AngleX");
            mParameter.arc.y = ParameterDict.Exemplar.GetDouble("Transformation.AngleY");
            mParameter.arc.z = ParameterDict.Exemplar.GetDouble("Transformation.AngleZ");
        }


        /// <summary>
        /// Umkehrung von Assign: Der Inhalt von Parameter wird in das ParameterDict geschrieben.
        /// </summary>
        public void SetGlobalParameters()
        {
            ParameterDict.Exemplar.SetDouble("Border.Min.x", mParameter.start_tupel.x);
            ParameterDict.Exemplar.SetDouble("Border.Min.y", mParameter.start_tupel.y);
            ParameterDict.Exemplar.SetDouble("Border.Min.z", mParameter.start_tupel.z);
            ParameterDict.Exemplar.SetDouble("Border.Max.x", mParameter.end_tupel.x);
            ParameterDict.Exemplar.SetDouble("Border.Max.y", mParameter.end_tupel.y);
            ParameterDict.Exemplar.SetDouble("Border.Max.z", mParameter.end_tupel.z);
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
                return (int)ParameterDict.Exemplar.GetDouble("Formula.Static.Cycles");
            }
        }


        /// <summary>
        /// Pixelgröße
        /// </summary>
        public int Raster
        {
            get
            {
                return (int)ParameterDict.Exemplar.GetDouble("View.Raster");
            }
        }


        /// <summary>
        /// Faktor der Fenstergröße.
        /// </summary>
        public double ScreenSize
        {
            get
            {
                return ParameterDict.Exemplar.GetDouble("View.Size");
            }
        }


        /// <summary>
        /// Index der zu berechnenden Formel.
        /// </summary>
        public int Formula
        {
            get
            {
                return (int)ParameterDict.Exemplar.GetDouble("Formula.Static.Formula");
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
            if (Form1.PublicForm.dontActivateRender)
                return;

            if (!mPreviewMode || ParameterDict.Exemplar.GetBool("View.Pipeline.UpdatePreview"))
            {
                int updateSteps = ParameterDict.Exemplar.GetInt("View.UpdateSteps");
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
            Form1.PublicForm.inPreview = false;
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
            mHistory.CurrentTime = 0;
            UpdateHistoryPic();
            LoadFromHistory();
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
            UpdateHistoryPic();
            LoadFromHistory();
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
            UpdateFromData();
            formulaEditor1.Init();
            // TODO: Bild aktualisieren

        }


        /// <summary>
        /// Das Control zur Übersicht der historischen Daten wird neu geladen.
        /// </summary>
        protected void UpdateHistoryControl()
        {
            lblCurrentStep.Text = mHistory.CurrentTime.ToString();
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
                ParameterDict.Exemplar.Save(sd.FileName);
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
            od.Filter = "*.xml|*.xml;*.tomo|*.*|*.*";
            if (oldDirectory != "")
            {
                od.InitialDirectory = oldDirectory;
            }
            if (od.ShowDialog() == DialogResult.OK)
            {
                ParameterDict.Exemplar.Load(od.FileName);
                ShowPicture(od.FileName);
                Data.Update();
                parameterDictControl1.UpdateFromData();
                ParameterValuesChanged();
                oldDirectory = System.IO.Path.GetDirectoryName(od.FileName);
            }
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

            //gesData=int.Parse(


            int dataPos = tempFileName.IndexOf("pic");
            int picPos = dataPos + 3;

            if (dataPos < 0)
                return;

            string gesDataString = tempFileName.Substring(0, dataPos);
            string gesPicString = tempFileName.Substring(picPos);
            //	string str=fileName.Split([p])[0]);
            //gesPic=int.Parse(fileName.Split("pic")[1]);

            string picDir = System.IO.Path.Combine(Fractrace.FileSystem.Exemplar.ExportDir, "Data" + gesDataString);
            string picFile = System.IO.Path.Combine(picDir, fileName + ".png");
            //         string  gesPicFileName="|gesPic"+gesPic+"|"+"|gesData"+gesData+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|";

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
            Form1.PublicForm.inPreview = false;
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
            if (tabControl1.SelectedTab == Data)
            {
                parameterDictControl1.UpdateFromData();
            }
            if (tabControl1.SelectedTab == tpSource)
                formulaEditor1.Init();
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
            preview1.Width = preview1.Height - 10;
            preview2.Width = preview2.Height - 10;
        }


        /// <summary>
        /// History wird um die aktuellen Daten erweitert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveInHistory_Click(object sender, EventArgs e)
        {
            //SavePicData(); 
            mHistory.CurrentTime = mHistory.Save();
            //mHistory.Save();
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
                    ParameterDict.Exemplar.SetInt("View.PosterX", 0);
                    ParameterDict.Exemplar.SetInt("View.PosterZ", 0);
                    return;
            }

            ParameterDict.Exemplar.SetInt("View.PosterX", xi);
            ParameterDict.Exemplar.SetInt("View.PosterZ", yi);
            Form1.PublicForm.inPreview = false;
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
            ParameterDict.Exemplar.Load(fileName);
            ShowPicture(fileName);
            Data.Update();
            parameterDictControl1.UpdateFromData();
            ParameterValuesChanged();
            oldDirectory = System.IO.Path.GetDirectoryName(fileName);
        }


        private bool inPreview = false;


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
                string sizeStr = ParameterDict.Exemplar["View.Size"];
                string rasterStr = ParameterDict.Exemplar["View.Raster"];
                ParameterDict.Exemplar["View.Size"] = "0.2";
                ParameterDict.Exemplar["View.Raster"] = "2";
                Form1.PublicForm.inPreview = true;
                ForceRedraw();
                Form1.PublicForm.inPreview = false;
                // Size und Raster auf die Ursprungswerte setzen
                ParameterDict.Exemplar["View.Size"] = sizeStr;
                ParameterDict.Exemplar["View.Raster"] = rasterStr;

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
            LoadFromHistory();
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
                ParameterDict.Exemplar.Append(od.FileName);
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
                ParameterDict.Exemplar.Save(sd.FileName, formulaSettingCategories);
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



    }
}
