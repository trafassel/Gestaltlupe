using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;

using Fractrace.Basic;

namespace Fractrace.Animation {
    public partial class AnimationControl : UserControl {


        public AnimationControl() {
            InitializeComponent();
            dataGridView1.DataSource = mAnimationSteps.Steps;
        }


        /// <summary>
        /// Verweis auf die global verwaltete Historie.
        /// </summary>
        ParameterHistory dataPerTime = null;


        /// <summary>
        /// Initialisierung.
        /// </summary>
        public void Init(ParameterHistory data) {
            dataPerTime = data;
        }


        private AnimationSteps mAnimationSteps = new AnimationSteps();

      


        /// <summary>
        /// Zeile wird an der aktuellen Position eingefügt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRow_Click(object sender, EventArgs e) {
            AnimationPoint point = new AnimationPoint();
            point.Time = dataPerTime.CurrentTime;
          if(mAnimationSteps.Steps.Count>0)
            point.Steps = ParameterDict.Exemplar.GetInt("Animation.Steps");
          int row = 0;
          if (dataGridView1.CurrentCell != null) {
            row = dataGridView1.CurrentCell.RowIndex;
          }
            if (row > 0 && row < mAnimationSteps.Steps.Count) {
              mAnimationSteps.Steps.Insert(row+1, point);
            } else {
              mAnimationSteps.Steps.Add(point);
            }
            UpdateFromAnimationSteps();
        }


      /// <summary>
        /// Der Inhalt der Tabelle wird vom Feld mAnimationSteps ausgehend aufgebaut.
      /// </summary>
        private void UpdateFromAnimationSteps() {
          // Alte Auswahlposition speichern:
          int currentRow = 0;
          if(dataGridView1.CurrentCell!=null)
            currentRow=dataGridView1.CurrentCell.RowIndex;

          dataGridView1.DataSource = null;
          dataGridView1.DataSource = mAnimationSteps.Steps.ToList();
         // dataGridView1.DataSource = mAnimationSteps.Steps;
          if (currentRow >= 0 && currentRow < mAnimationSteps.Steps.Count) {
            //dataGridView1.CurrentCell = DataGridViewCell.;
            dataGridView1.Rows[currentRow].Selected=true;
            dataGridView1.CurrentCell = dataGridView1.Rows[currentRow].Cells[0];
            //.CurrentCell.RowIndex = currentRow;
          }
        }



        protected double mPictureSize = 1;

      /// <summary>
      /// Enthält die Formel des ersten Eintrages
      /// </summary>
        protected string mFormula = "";

        /// <summary>
        /// Start der Animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e) {
          if (mAnimationSteps.Steps.Count == 0)
            return;
          btnStart.Enabled = false;
          btnStop.Enabled = true;
          btnStop.Visible = true;
          animationAbort = false;
          lblAnimationProgress.Text = "startet...";
          
     //     AnimationPoint apStart = mAnimationSteps.Steps[0];
     //     dataPerTime.Load(apStart.Time);
          mFormula = ParameterDict.Exemplar["Intern.Formula.Source"];
          for (int i = 1; i < mAnimationSteps.Steps.Count; i++) {
            if (animationAbort)
              break;
            AnimationPoint ap1=mAnimationSteps.Steps[i-1];
            AnimationPoint ap2=mAnimationSteps.Steps[i];
            ComputeAnimationPart(ap1.Time, ap2.Time, ap2.Steps);
          }
          btnStop.Visible = false;
          btnStart.Enabled = true;
          lblAnimationProgress.Text = "ready";
          animationAbort = false;
        }


      /// <summary>
      /// Berechnet einen Animationsteil, bestehend aus steps Einzelschritten.
      /// </summary>
      /// <param name="from"></param>
      /// <param name="to"></param>
      /// <param name="steps"></param>
        private void ComputeAnimationPart(int from, int to, int steps) {
          ParameterHistory animationHistory = new ParameterHistory();
          dataPerTime.Load(from);
          // Größe festlegen:
          ParameterDict.Exemplar.SetDouble("View.Size", mPictureSize);
          ParameterDict.Exemplar["Intern.Formula.Source"]=mFormula;
          animationHistory.Save();
          dataPerTime.Load(to);
          ParameterDict.Exemplar.SetDouble("View.Size", mPictureSize);
          ParameterDict.Exemplar["Intern.Formula.Source"] = mFormula;
          animationHistory.Save();
          lblAnimationProgress.Text = "compute: "+from.ToString()+" "+to.ToString();
          for (int i = 0; i < steps && !animationAbort; i++) {
            double r = 1.0 / steps * (double)i;
            Application.DoEvents();
            animationHistory.Load(r);
            Form1.PublicForm.ComputeOneStep();
            lblAnimationProgress.Text = "compute: " + from.ToString() + " " + to.ToString()+ " Step " + i.ToString() + " (from " + steps.ToString() + ")";
            // Auf Beendigung der Berechnung warten.
            if (animationAbort)
              break;
            while (Form1.PublicForm.InComputation) {
           //   System.Threading.Thread.Sleep(199);
              if (animationAbort)
                break;
              Application.DoEvents();
              if (animationAbort)
                break;
            }
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
        private void btnStop_Click(object sender, EventArgs e) {
          animationAbort = true;
          btnStop.Enabled = false;
        }


      /// <summary>
      /// Der ausgewählte Eintrag wird aus der Liste entfernt.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e) {
            try
            {
                int row = dataGridView1.CurrentCell.RowIndex;
                if (row >= 0 && row < mAnimationSteps.Steps.Count)
                {
                    mAnimationSteps.Steps.RemoveAt(row);
                    UpdateFromAnimationSteps();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }


      /// <summary>
      /// Nutzereingabe zur Größe der Ausgabefläche.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
        private void tbSize_TextChanged(object sender, EventArgs e) {
            if (double.TryParse(tbSize.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mPictureSize))
                tbSize.ForeColor = Color.Black;
            else
                tbSize.ForeColor = Color.Red;
        }


      /// <summary>
      /// Die Einzelereignisse der Animation werden gezeigt.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
        private void btnPreview_Click(object sender, EventArgs e) {
          RenderPreview();
        }


        protected bool inRenderingPreview=false;


        private void RenderPreview() {
          pnlPreview.Controls.Clear();
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
        void mPreview1_RenderingEnds() {
          if (currentPreviewStep >= mAnimationSteps.Steps.Count) {
            inRenderingPreview = false;
            currentPreviewStep = 0;
            return;
          }
          if (!inRenderingPreview)
            return;
          // Die Daten von currentPreviewStep laden:
          AnimationPoint ap = mAnimationSteps.Steps[currentPreviewStep];
          ParameterHistory animationHistory = new ParameterHistory();
          dataPerTime.Load(ap.Time);
          animationHistory.Save();
          /* das nur für Zwischenschritte */
          /*
          dataPerTime.Load(to);
          ParameterDict.Exemplar.SetDouble("View.Size", mPictureSize);
          animationHistory.Save();
          */

          PreviewControl mPreview1 = new Fractrace.PreviewControl();
          mPreview1.Width = 50;
          mPreview1.Height = 50;
          mPreview1.Location = new System.Drawing.Point(50 * currentPreviewStep, 0);
          pnlPreview.Controls.Add(mPreview1);
          mPreview1.ShowProgressBar = false;
          mPreview1.RenderOnClick = false;

          currentPreviewStep++;
          mPreview1.RenderingEnds += new PictureRenderingIsReady(mPreview1_RenderingEnds);
          mPreview1.Draw();
        }


    }
}
