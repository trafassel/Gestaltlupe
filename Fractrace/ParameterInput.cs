using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fractrace.DataTypes;
using Fractrace.Basic;

namespace Fractrace {
    public partial class ParameterInput : Form {


        /// <summary>
        /// Initialisierung
        /// </summary>
        public ParameterInput() {
            InitializeComponent();
            mParameter.SetToDefault();
            ParameterDict.Exemplar.EventChanged += new ParameterDictChanged(Exemplar_EventChanged);
            // Das zweite PreviewControl ist für die Stereosicht zuständig.
            preview2.IsRightView = true; 
            navigateControl1.Init(preview1,preview2,this);
            parameterDictControl1.InternDataGridView.CellValueChanged += new DataGridViewCellEventHandler(InternDataGridView_CellValueChanged);
            this.animationControl1.Init(mHistory);
        }


        /// <summary>
        /// Wenn sich Einträge im Datagrid geändert haben, wird Neuzeichnen angestoßen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InternDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            ParameterValuesChanged();
        }


        /// <summary>
        /// Irgendwelche Werte haben sich geändert.
        /// </summary>
        void ParameterValuesChanged() {
            preview1.Draw();
            if (cbAutomaticSave.Checked) {
                AddToHistory();
            }
          // Hier auch den Quelltext aktualisieren
            if (tabControl1.SelectedTab == tpSource)
              formulaEditor1.Init();
        }

      


        /// <summary>
        /// Das berechnete Bild wird für die spätere Verwendung gespeichert.
        /// </summary>
        protected void SavePicData() {
            mHistoryImages[mHistory.Time] = preview1.Image;
            Console.WriteLine("Save Pic to Time " + mHistory.Time.ToString());

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
        void Exemplar_EventChanged(object source, ParameterDictChangedEventArgs e) {
        
            UpdateFromData();
        }


        protected bool inSetParameters = false;


      /// <summary>
      /// 
      /// </summary>
        public void UpdateFromData() {
          parameterDictControl1.UpdateFromData();
        }


        /// <summary>
        /// Das Feld Parameter wird aus dem ParameterDict gelesen.
        /// </summary>
        public void Assign() {

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
        public void SetGlobalParameters() {
           ParameterDict.Exemplar.SetDouble("Border.Min.x",mParameter.start_tupel.x);
           ParameterDict.Exemplar.SetDouble("Border.Min.y", mParameter.start_tupel.y);
           ParameterDict.Exemplar.SetDouble("Border.Min.z", mParameter.start_tupel.z);
           ParameterDict.Exemplar.SetDouble("Border.Min.zz", mParameter.start_tupel.zz);
           ParameterDict.Exemplar.SetDouble("Border.Max.x", mParameter.end_tupel.x);
           ParameterDict.Exemplar.SetDouble("Border.Max.y", mParameter.end_tupel.y);
           ParameterDict.Exemplar.SetDouble("Border.Max.z", mParameter.end_tupel.z);
           ParameterDict.Exemplar.SetDouble("Border.Max.zz", mParameter.end_tupel.zz);

        }

       

        /// <summary>
        /// Zugriff auf die Bearbeitungsparameter.
        /// </summary>
        private FracValues mParameter = new FracValues();

        public FracValues Parameter {
            get {
                return mParameter;
            }
        }


        /// <summary>
        /// Iterationstiefe
        /// </summary>
        public int Cycles {
            get {
                return (int) ParameterDict.Exemplar.GetDouble("Formula.Static.Cycles");
            }
        }


        /// <summary>
        /// Pixelgröße
        /// </summary>
        public int Raster {
            get {
                return (int )ParameterDict.Exemplar.GetDouble("View.Raster");
            }
        }


        /// <summary>
        /// Faktor der Fenstergröße.
        /// </summary>
        public double ScreenSize {
            get {
                return ParameterDict.Exemplar.GetDouble("View.Size");
            }
        }


        /// <summary>
        /// Index der zu berechnenden Formel.
        /// </summary>
        public int Formula {
            get {
                return (int)ParameterDict.Exemplar.GetDouble("Formula.Static.Formula");
            }
        }

      




        /// <summary>
        /// Neuzeichnen über das übergeordentete Control.
        /// </summary>
        private void ForceRedraw() {
            Form1.PublicForm.ComputeOneStep();
            if(Stereo)
                DrawStereo();
        }


      /// <summary>
      /// Wird von außen gesetzt, um Steuerelemente während der Berechung zu deaktivieren.
      /// </summary>
        public bool InComputing {
          set {
            if (value) {
              btnStart.Enabled = false;
              btnCreatePoster.Enabled = false;
              btnStop.Enabled = true;
            } else {
              btnStart.Enabled = true;
              btnCreatePoster.Enabled = true;
              btnStop.Enabled = false;
              ComputationEnds();
            }
          }
        }


        private void ComputationEnds() {
          if (mPosterMode) {
            DrawNextPosterPart();
          }
        }

        private void DrawStereo() {
            if (mStereoForm == null) {
                mStereoForm = new StereoForm();
                mStereoForm.Show();
            }
            mStereoForm.ImageRenderer.Draw();
        }


        private StereoForm mStereoForm = null;

        /// <summary>
        /// 
        /// </summary>
        public bool Changed = false;



       private void OK() {
           Changed = true;
           ForceRedraw();
       }


       private void tbVar1_TextChanged(object sender, EventArgs e) {

       }

       private void tbAngle_TextChanged(object sender, EventArgs e) {

       }

       private void tbVar2_TextChanged(object sender, EventArgs e) {

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
       private void btnBack_Click_1(object sender, EventArgs e) {
               mHistory.CurrentTime=0;
               UpdateHistoryPic();
               LoadFromHistory();
       }


        /// <summary>
       /// Der nächste Eintrag der History wird geladen (wenn möglich)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void button1_Click(object sender, EventArgs e) {
               //SavePicData(); 
         mHistory.CurrentTime = mHistory.Time;
               UpdateHistoryPic();
               LoadFromHistory();
       }


        /// <summary>
       /// Die Parameterdaten zum Zeitpunkt mHistoryTime werden geladen und dargestellt.
        /// </summary>
       private void LoadFromHistory() {
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
       protected void UpdateHistoryControl() {
         this.Text = "Parameters " + mHistory.CurrentTime.ToString();
         lblCurrentStep.Text = mHistory.CurrentTime.ToString();
           if (mHistory.CurrentTime > 0) {
             btnLastStep.Text = ((int)(mHistory.CurrentTime - 1)).ToString();
           }
           else {
               btnLastStep.Text = "___";
           }
           if (mHistory.CurrentTime < mHistory.Time) {
             btnNextStep.Text = ((int)(mHistory.CurrentTime + 1)).ToString();
           }
           else {
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
       private void btnSave_Click(object sender, EventArgs e) {
           SaveFileDialog sd = new SaveFileDialog();
           sd.Filter="*.xml|*.xml|*.*|all";
           if(sd.ShowDialog()==DialogResult.OK) {
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
       private void btnLoad_Click(object sender, EventArgs e) {
           OpenFileDialog od = new OpenFileDialog();
           od.Filter = "*.xml|*.xml;*.tomo|*.*|*.*";
           if (oldDirectory != "") {
             od.InitialDirectory = oldDirectory;
           }
           if (od.ShowDialog() == DialogResult.OK) {
               ParameterDict.Exemplar.Load(od.FileName);
               ShowPicture(od.FileName);
               Data.Update();
               parameterDictControl1.UpdateFromData();
               ParameterValuesChanged();
              // preview1.Draw();
               oldDirectory = System.IO.Path.GetDirectoryName(od.FileName);
           }
       }


        /// <summary>
        /// Sucht das passende Bild zur Parameterdict-Datei und zeigt es
        /// (wenn die Suche erfolgreich war) in Fenster1 an.
        /// 
        /// </summary>
        /// <param name="parameterdictFilen"></param>
       private void ShowPicture(string parameterdictFile) {
           if(!parameterdictFile.ToLower().StartsWith(Fractrace.FileSystem.Exemplar.ExportDir.ToLower()))
               return;
           string fileName = System.IO.Path.GetFileNameWithoutExtension(parameterdictFile);
           // Data128pic8
           int gesData=0;
           int gesPic=0;
           gesData=4;
           string tempFileName=fileName.Substring(4); // Data ist vier Zeichen lang.
           
           //gesData=int.Parse(


           int dataPos = tempFileName.IndexOf("pic");
           int picPos=dataPos+3;

           if (dataPos < 0)
               return;

           string gesDataString = tempFileName.Substring(0, dataPos);
           string gesPicString = tempFileName.Substring(picPos);
           //	string str=fileName.Split([p])[0]);
           //gesPic=int.Parse(fileName.Split("pic")[1]);

           string picDir = System.IO.Path.Combine(Fractrace.FileSystem.Exemplar.ExportDir, "Data" + gesDataString);
           string picFile = System.IO.Path.Combine(picDir, fileName + ".jpg");
//         string  gesPicFileName="|gesPic"+gesPic+"|"+"|gesData"+gesData+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|"+"|";

           Form1.PublicForm.ShowPictureFromFile(picFile);
		
          
       }



       private bool animationAbort = false;

    


       /// <summary>
       /// Animation anhalten
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
       private void btnStopAnimation_Click(object sender, EventArgs e) {
           animationAbort = true;
       }


   

       private void btnStart_Click(object sender, EventArgs e)
       {
         mPosterMode = false;
           // Todo: Bild nur speichern, wenn der Haken gesetzt ist
           if (cbSaveHistory.Checked)
           {
             mHistory.CurrentTime = mHistory.Save();
             this.Text = "Parameters " + mHistory.CurrentTime.ToString();
           }
           ForceRedraw();

       }


        /// <summary>
        /// Berechnung stoppen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void button27_Click(object sender, EventArgs e) {
           mPosterMode = false;
           Form1.PublicForm.Stop();
       }

       private void navigateControl1_Load(object sender, EventArgs e) {

       }

        /// <summary>
       /// Zwischen dem aktuellen und dem nächsten Schritt werden "Animation.Steps" Zwischenschritte berechnet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void btnStartAnimation_Click_1(object sender, EventArgs e) {
           btnStartAnimation.Enabled = false;
           btnStopAnimation.Visible = true;
           Application.DoEvents();
           animationAbort = false;
           int steps = (int)ParameterDict.Exemplar.GetDouble("Animation.Steps");
           lblAnimationSteps.Text = "Step 0 (from " + steps.ToString() + ")";
           for (int i = 0; i < steps&&!animationAbort; i++) {
               double r = 1.0 / steps * (double)i;
               Application.DoEvents();
               mHistory.Load((double)mHistory.CurrentTime + r);
               ForceRedraw();
               lblAnimationSteps.Text = "Step " + i.ToString() + " (from " + steps.ToString() + ")";
               // Auf Beendigung der Berechnung warten.
               while (Form1.PublicForm.InComputation) {
                   System.Threading.Thread.Sleep(199);
                   Application.DoEvents();
               }
           }
           btnStartAnimation.Enabled = true;
           btnStopAnimation.Visible = true;
       }

       private void btnStopAnimation_Click_1(object sender, EventArgs e) {
           animationAbort = true;
           Form1.PublicForm.Stop();
           animationAbort = true;
       }


        /// <summary>
        /// Dialogabfrage vor Beendigung der Anwendung.
        /// </summary>
        /// <param name="e"></param>
       protected override void OnClosing(CancelEventArgs e) {
           if (MessageBox.Show("Exit", "Really?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
               RemoveEmptyDirectory();
               base.OnClosing(e);
               Form1.PublicForm.ForceClosing();
           }
           else e.Cancel = true;
       }


       /// <summary>
       /// Wenn keine Bilddateien gespeichert wurden, wird das entsprechende Hauptverzeichnis gelöscht.
       /// </summary>
       protected void RemoveEmptyDirectory() {
           string dir=FileSystem.Exemplar.ProjectDir;
           if (System.IO.Directory.GetFiles(dir).Length == 0)
               System.IO.Directory.Delete(dir);
       }


        /// <summary>
        /// Tab-Auswahl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
           if (tabControl1.SelectedTab == Data) {
             parameterDictControl1.UpdateFromData();
           }
            if (tabControl1.SelectedTab == tpSource)
               formulaEditor1.Init();
       }


        /// <summary>
        /// Ist die zweite Ansicht aktiviert?.
        /// </summary>
       public bool Stereo {
           get {
               return cbStereo.Checked;
           }
       }


        /// <summary>
        /// Höhe wurde verschoben.
        /// Breite der Preview Controls setzen:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void splitContainer1_Panel1_ClientSizeChanged(object sender, EventArgs e) {
           preview1.Width = preview1.Height - 10;
           preview2.Width = preview2.Height - 10;
       }


        /// <summary>
        /// History wird um die aktuellen Daten erweitert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void btnSaveInHistory_Click(object sender, EventArgs e) {
           //SavePicData(); 
         mHistory.CurrentTime = mHistory.Save();
           //mHistory.Save();
           LoadFromHistory();

       }


        /// <summary>
        /// Die aktuellen Parameterdaten werden in die History gespeichert.
        /// </summary>
       public void AddToHistory() {
           mHistory.Save();
           UpdateHistoryControl();
       }

        /// <summary>
        /// Vorgängerschritt wurde ausgewählt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void btnLastStep_Click(object sender, EventArgs e) {
           //SavePicData(); 
         if (mHistory.CurrentTime > 0) {
           mHistory.CurrentTime--;
               UpdateHistoryPic();
           }
           LoadFromHistory();
       }


       Dictionary<int, Image> mHistoryImages = new Dictionary<int, Image>();


        /// <summary>
        /// Nächster History-Eintrag wird geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void btnNextStep_Click(object sender, EventArgs e) {

         if (mHistory.CurrentTime < mHistory.Time) {
           mHistory.CurrentTime++;
               UpdateHistoryPic();
           }
           LoadFromHistory();
       }


        /// <summary>
        /// Ein gespeichertes Bild wird geladen.
        /// </summary>
       private void UpdateHistoryPic() {
         if (mHistoryImages.ContainsKey(mHistory.CurrentTime)) {
           preview1.Image = mHistoryImages[mHistory.CurrentTime];
               preview1.Refresh();
           }
       }

        /// <summary>
        /// Erstellung des Images im Vorschaufenster wurde beendet.
        /// </summary>
       private void preview1_RenderingEnds() {
           // Der Zähler wird auf den aktuellen Endzeitpunkt gestellt.
         mHistory.CurrentTime = mHistory.Time;
           UpdateHistoryControl();
           SavePicData(); 
       }


      /// <summary>
      /// Ein anderer Karteikastenreiter wurde ausgewählt.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
       private void tabControl2_SelectedIndexChanged(object sender, EventArgs e) {
         if (tabControl2.SelectedTab == tpRender) {
           // View-Eigenschaften bei den globalen Einstellungen auswählen.
           this.parameterDictControl1.SelectNode("View");
         }

       }


       protected int mPosterStep = 0;

        protected bool mPosterMode=false;


      /// <summary>
      /// Erstellung eines Posters wurde angeklickt
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
       private void btnCreatePoster_Click(object sender, EventArgs e) {
         mPosterStep = 0;
         mPosterMode = true;
         DrawNextPosterPart();
       }

      /// <summary>
      /// Erstellt das nächste Einzelbild des Posters.
      /// </summary>
       private void DrawNextPosterPart() {
         if (!mPosterMode)
           return;
         int xi = 0;
         int yi = 0;
         switch (mPosterStep) {
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
             return;
         }

         ParameterDict.Exemplar.SetInt("View.PosterX",xi);
         ParameterDict.Exemplar.SetInt("View.PosterZ", yi);
         ForceRedraw();
         mPosterStep++;
       }

       private void navigateControl1_Load_1(object sender, EventArgs e) {

       }

      /// <summary>
      /// Berechnung anhalten
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
       private void btnPause_Click(object sender, EventArgs e) {
         if (btnPause.Text == "Pause") {
           Fractrace.Iterate.Pause = true;
           btnPause.Text = "Run";
         } else {
           Fractrace.Iterate.Pause = false;
           btnPause.Text = "Pause";
         }
       }

       private void button2_Click(object sender, EventArgs e) {

       }

       private void panel12_Paint(object sender, PaintEventArgs e) {

       }

       /// <summary>
       /// Handles the Click event of the btnLoadLast control.
       /// Das letzte Projekt wird geladen.
       /// </summary>
       /// <param name="sender">The source of the event.</param>
       /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
       private void btnLoadLast_Click(object sender, EventArgs e) {

       }


       /// <summary>
       /// Projektdatei wird geladen und (falls ein Bild existiert) angezeigt.
       /// </summary>
       /// <param name="fileName">Name of the file.</param>
       private void LoadConfiguration(string fileName) {
         ParameterDict.Exemplar.Load(fileName);
         ShowPicture(fileName);
         Data.Update();
         parameterDictControl1.UpdateFromData();
         ParameterValuesChanged();
         oldDirectory = System.IO.Path.GetDirectoryName(fileName);
       }


    }
}
