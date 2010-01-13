using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;

namespace Fractrace {
    public partial class NavigateControl : UserControl {
        public NavigateControl() {
            InitializeComponent();
        }


        PreviewControl mPreview = null;

        PreviewControl mPreviewStereo = null;

        ParameterInput mParent = null;


        /// <summary>
        /// Initialisierung.
        /// </summary>
        /// <param name="preview"></param>
        public void Init(PreviewControl preview, PreviewControl preview2,ParameterInput parent) {
            mPreview = preview;
            mPreviewStereo = preview2;
            mParent = parent;
        }


        /// <summary>
        /// Bewegung nach links.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e) {
            double endX = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            double startX = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX -= ddiff / mFactor;
            startX -= ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.x", endX);
            ParameterDict.Exemplar.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Bewegung nach rechts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e) {
            double endX = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            double startX = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX += ddiff / mFactor;
            startX += ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.x", endX);
            ParameterDict.Exemplar.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Bewegung nach oben
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTop_Click(object sender, EventArgs e) {
            double endZ = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ += ddiff / mFactor;
            startZ += ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.z", endZ);
            ParameterDict.Exemplar.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Bewegung nach unten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e) {
            double endZ = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ -= ddiff / mFactor;
            startZ -= ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.z", endZ);
            ParameterDict.Exemplar.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Vorwärts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForward_Click(object sender, EventArgs e) {
            double endY = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            double startY = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY -= ddiff / mFactor;
            startY -= ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.y", endY);
            ParameterDict.Exemplar.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();
        }

        /// <summary>
        /// Rückwärts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackwards_Click(object sender, EventArgs e) {
            double endY = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            double startY = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY += ddiff / mFactor;
            startY += ddiff / mFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.y", endY);
            ParameterDict.Exemplar.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// X Zoomen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomX_Click(object sender, EventArgs e) {
            double endX = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            double startX = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX -= ddiff / mZoomFactor;
            startX += ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.x", endX);
            ParameterDict.Exemplar.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
        }

        private void btnZoomY_Click(object sender, EventArgs e) {
            double endY = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            double startY = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY -= ddiff / mZoomFactor;
            startY += ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.y", endY);
            ParameterDict.Exemplar.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();
       
        }

        private void btnZoomZ_Click(object sender, EventArgs e) {
            double endZ = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ -= ddiff / mZoomFactor;
            startZ += ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.z", endZ);
            ParameterDict.Exemplar.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
     
        }

        private void button1_Click(object sender, EventArgs e) {
            double endX = ParameterDict.Exemplar.GetDouble("Border.Max.x");
            double startX = ParameterDict.Exemplar.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX += ddiff / mZoomFactor;
            startX -= ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.x", endX);
            ParameterDict.Exemplar.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
 
        }

        private void btnZoomYout_Click(object sender, EventArgs e) {
            double endY = ParameterDict.Exemplar.GetDouble("Border.Max.y");
            double startY = ParameterDict.Exemplar.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY += ddiff / mZoomFactor;
            startY -= ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.y", endY);
            ParameterDict.Exemplar.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();

        }

        private void btnZoomZout_Click(object sender, EventArgs e) {
            double endZ = ParameterDict.Exemplar.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Exemplar.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ += ddiff / mZoomFactor;
            startZ -= ddiff / mZoomFactor;
            ParameterDict.Exemplar.SetDouble("Border.Max.z", endZ);
            ParameterDict.Exemplar.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
   
        }


        protected double mFactor = 6;

        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (double.TryParse(textBox1.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mFactor))
                textBox1.ForeColor = Color.Black;
            else
                textBox1.ForeColor = Color.Red;

        }

        private void button4_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleX = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleX");
            angleX -= mAngle;
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleX", angleX);
            DrawAndWriteInHistory();
      
        }


        protected double mZoomFactor = 6;


        /// <summary>
        /// Zoomfaktor wird geparst
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbZoomFactor_TextChanged(object sender, EventArgs e) {
            if (double.TryParse(tbZoomFactor.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mZoomFactor))
                tbZoomFactor.ForeColor = Color.Black;
            else
                tbZoomFactor.ForeColor = Color.Red;

            if (mZoomFactor <= 2) {
                mZoomFactor = 2.0001;
                tbZoomFactor.Text = "2.0001";
            }

        }


        protected double mAngle = 6;


        /// <summary>
        /// Rotationswinkel wird geparst
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAngle_TextChanged(object sender, EventArgs e) {
            if (double.TryParse(tbAngle.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mAngle))
                 tbAngle.ForeColor = Color.Black;
            else
                tbAngle.ForeColor = Color.Red;

        }

        private void btnRotX_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleX = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleX");
            angleX += mAngle;    
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleX",angleX);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Der Mittelpunkt der Rotation wird auf den (virtuellen) Bildschirmmittelpunkt gesetzt.
        /// (Etwas näher an Maxy als an der Mittes des Voxelraumes).
        /// </summary>
        private void SetRotationCenter() {
            double centerX = (ParameterDict.Exemplar.GetDouble("Border.Max.x") + ParameterDict.Exemplar.GetDouble("Border.Min.x")) / 2.0;
            double centerY = 0.15 * (ParameterDict.Exemplar.GetDouble("Border.Max.y") - ParameterDict.Exemplar.GetDouble("Border.Min.y"))  
                + ParameterDict.Exemplar.GetDouble("Border.Max.y");
            double centerZ = (ParameterDict.Exemplar.GetDouble("Border.Max.z") + ParameterDict.Exemplar.GetDouble("Border.Min.z")) / 2.0;
            ParameterDict.Exemplar.SetDouble("Transformation.3.CenterX", centerX);
            ParameterDict.Exemplar.SetDouble("Transformation.3.CenterY", centerY);
            ParameterDict.Exemplar.SetDouble("Transformation.3.CenterZ", centerZ);
        }


        private void btnRotY_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleY = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleY");
            angleY += mAngle;    
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleY",angleY);
            DrawAndWriteInHistory();
        }



        private void btnRotZ_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleZ = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleZ");
            angleZ += mAngle;
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleZ", angleZ);
            DrawAndWriteInHistory();

        }

        private void btnRotYneg_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleY = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleY");
            angleY -= mAngle;
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleY", angleY);
            DrawAndWriteInHistory();
        }

        private void btnRotZneg_Click(object sender, EventArgs e) {
            SetRotationCenter();
            double angleZ = ParameterDict.Exemplar.GetDouble("Transformation.3.AngleZ");
            angleZ -= mAngle;
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleZ", angleZ);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Alle Betrachtungswinkel werden auf null gesetzt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAllAngles0_Click(object sender, EventArgs e) {
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleX", 0);
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleY", 0);
            ParameterDict.Exemplar.SetDouble("Transformation.3.AngleZ", 0);
        }


        /// <summary>
        /// Zeichnet und aktualisiert die History.
        /// </summary>
        private void DrawAndWriteInHistory() {
            mPreview.Draw();
            mParent.AddToHistory();
          
        }



    }
}
