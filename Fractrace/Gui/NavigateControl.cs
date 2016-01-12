using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;
using Fractrace.Geometry;

namespace Fractrace
{


    /// <summary>
    /// A Control with some basic buttons to navigate in the virtual object space.
    /// </summary>
    public partial class NavigateControl : UserControl
    {


        /// <summary>
        /// Contructer.
        /// </summary>
        public NavigateControl()
        {
            InitializeComponent();

            // disable some problemtic buttons
            btnZoomX.Visible = false;
            button1.Visible = false;
            btnZoomY.Visible = false;
            btnZoomYout.Visible = false;
            btnZoomZ.Visible = false;
            btnZoomZout.Visible = false;

            tbAngle_TextChanged(null, null);

            label1.Visible = false;
            tbMove.Visible = false;
            label3.Visible = false;
            tbAngle.Visible = false;
            label2.Visible = false;
            tbZoomFactor.Visible = false;
            // btnAspect.Visible = false;
            panel2.Visible = false;

            UpdateMoveButtonAppearance();
            UpdateMoveAngleButtonAppearance();

            // test is for designer only
            if (ParameterInput.MainParameterInput != null && ParameterInput.MainParameterInput.MainDataViewControl != null)
            {
                _propertyControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _propertyControl.Dock = DockStyle.Fill;
                _propertyControl.Create("Transformation");
                pnlProperties.Controls.Add(_propertyControl);
                _propertyControlBbox = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _propertyControlBbox.Dock = DockStyle.Fill;
                _propertyControlBbox.Create("Border");
                pnlBorderProperties.Controls.Add(_propertyControlBbox);
            }

        }

        /// <summary>
        /// Control for "Transform" Parameters.
        /// </summary>
        DataViewControlPage _propertyControl;

        /// <summary>
        /// Control for "Border" Parameters.
        /// </summary>
        DataViewControlPage _propertyControlBbox;

        /// <summary>
        /// A small control to display the preview of the rendered scene.
        /// </summary>
        PreviewControl mPreview = null;

        /// <summary>
        /// The "right eye"-view of the control.
        /// </summary>
        PreviewControl mPreviewStereo = null;

        /// <summary>
        /// Parent control.
        /// </summary>
        ParameterInput mParent = null;

        /// <summary>
        /// Difference in the center position, if x-Border ist changed by (1,0,0)
        /// </summary>
        Vec3 centerDiffX = new Vec3();

        /// <summary>
        /// Difference in the center position, if y-Border ist changed by (0,1,0)
        /// </summary>
        Vec3 centerDiffY = new Vec3();

        /// <summary>
        /// Difference in the center position, if z-Border ist canged by (0,0,1)
        /// </summary>
        Vec3 centerDiffZ = new Vec3();


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="preview"></param>
        public void Init(PreviewControl preview, PreviewControl preview2, ParameterInput parent)
        {
            mPreview = preview;
            mPreviewStereo = preview2;
            mParent = parent;
        }


        /// <summary>
        /// Move left.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(-1, 0, 0), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Set centerDiffX, centerDiffY and centerDiffZ.
        /// </summary>
        public void UpdateCenterDiff()
        {
            double centerX = (ParameterDict.Current.GetDouble("Border.Max.x") + ParameterDict.Current.GetDouble("Border.Min.x")) / 2.0;
            double centerY = 0.5 * (ParameterDict.Current.GetDouble("Border.Max.y") + ParameterDict.Current.GetDouble("Border.Min.y"));
            double centerZ = (ParameterDict.Current.GetDouble("Border.Max.z") + ParameterDict.Current.GetDouble("Border.Min.z")) / 2.0;
            Rotation rotView = null;
            double centerXChange = centerX + 1;
            double centerYChange = centerY + 1;
            double centerZChange = centerZ + 1;

            // For Zerotest
            double minDoubleVal = 0.0000000000000001;

            // This does not work for angle combinations:
            rotView = new Rotation();
            rotView.Init(0, 0, 0, -ParameterDict.Current.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Current.GetDouble("Transformation.Camera.AngleY"),
                  ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ"));
            centerDiffX = rotView.TransformForNavigation(new Vec3(1, 0, 0));

            rotView = new Rotation();
            rotView.Init(0, 0, 0, -ParameterDict.Current.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Current.GetDouble("Transformation.Camera.AngleY"),
                  ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ"));
            centerDiffY = rotView.TransformForNavigation(new Vec3(0, -1, 0));

            rotView = new Rotation();
            rotView.Init(0, 0, 0, -ParameterDict.Current.GetDouble("Transformation.Camera.AngleX"), ParameterDict.Current.GetDouble("Transformation.Camera.AngleY"),
                  ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ"));
            centerDiffZ = rotView.TransformForNavigation(new Vec3(0, 0, -1));

            // Set 0-Entries
            if (centerDiffX.X > -minDoubleVal && centerDiffX.X < minDoubleVal)
                centerDiffX.X = 0;
            if (centerDiffX.Y > -minDoubleVal && centerDiffX.Y < minDoubleVal)
                centerDiffX.Y = 0;
            if (centerDiffX.Z > -minDoubleVal && centerDiffX.Z < minDoubleVal)
                centerDiffX.Z = 0;

            if (centerDiffY.X > -minDoubleVal && centerDiffY.X < minDoubleVal)
                centerDiffY.X = 0;
            if (centerDiffY.Y > -minDoubleVal && centerDiffY.Y < minDoubleVal)
                centerDiffY.Y = 0;
            if (centerDiffY.Z > -minDoubleVal && centerDiffY.Z < minDoubleVal)
                centerDiffY.Z = 0;

            if (centerDiffZ.X > -minDoubleVal && centerDiffZ.X < minDoubleVal)
                centerDiffZ.X = 0;
            if (centerDiffZ.Y > -minDoubleVal && centerDiffZ.Y < minDoubleVal)
                centerDiffZ.Y = 0;
            if (centerDiffZ.Z > -minDoubleVal && centerDiffZ.Z < minDoubleVal)
                centerDiffZ.Z = 0;
        }


        /// <summary>
        /// Solve the equation system a*pp1+b*pp2+c*pp3=pp0 
        /// </summary>
        /// <param name="pp0"></param>
        /// <param name="pp1"></param>
        /// <param name="pp2"></param>
        /// <param name="pp3"></param>
        /// <returns></returns>
        Vec3 SolveEqusyst(Vec3 pp0, Vec3 pp1, Vec3 pp2, Vec3 pp3)
        {
            double s = 0, t = 0, u = 0;

            // Solve:
            // a1*s+a2*t+a3*u=a0;
            // b1*s+b2*t+b3*u=b0;
            // c1*s+c2*t+c3*u=c0;
            double a0 = pp0.X, b0 = pp0.Y, c0 = pp0.Z;
            double a1 = pp1.X, b1 = pp1.Y, c1 = pp1.Z;
            double a2 = pp2.X, b2 = pp2.Y, c2 = pp2.Z;
            double a3 = pp3.X, b3 = pp3.Y, c3 = pp3.Z;

            // Gauss transformation to get:
            // x11*s + x12*t + x13*u = x14
            //         x22*t + x23*u = x24
            //                 x33*u = x34
            double x11, x12, x13, x14;
            double x22, x23, x24;
            double x33, x34;
            if (a1 != 0)
            {
                x11 = a1; x12 = a2; x13 = a3; x14 = a0;
                if (a1 * b2 - b1 * a2 != 0)
                {
                    x22 = a1 * b2 - b1 * a2; x23 = a1 * b3 - b1 * a3; x24 = a1 * b0 - b1 * a0;
                    x33 = (a1 * b2 - b1 * a2) * (a1 * c3 - c1 * a3) - (a1 * c2 - c1 * a2) * (a1 * b3 - b1 * a3);
                    x34 = (a1 * b2 - b1 * a2) * (a1 * c0 - c1 * a0) - (a1 * c2 - c1 * a2) * (a1 * b0 - b1 * a0);
                }
                else
                { // Dritte mit zweiter Zeile tauschen
                    x22 = a1 * c2 - c1 * a2; x23 = a1 * c3 - c1 * a3; x24 = a1 * c0 - c1 * a0;
                    x33 = a1 * b3 - b1 * a3; x34 = a1 * b0 - b1 * a0;
                }
            }
            else
            {
                if (b1 != 0)
                { // Erste mit zweiter Zeile vertauschen
                    x11 = b1; x12 = b2; x13 = b3; x14 = b0;
                    if (a2 != 0)
                    {
                        x22 = a2; x23 = a3; x24 = a0;
                        x33 = a2 * b1 * c3 - a2 * c1 * b3 - a3 * b1 * c2 + a3 * c1 * b2;
                        x34 = a2 * b1 * c0 - a2 * c1 * b0 - a0 * b1 * c2 + a0 * c1 * b2;
                    }
                    else
                    { // Dritte mit zweiter Zeile vertauschen
                        x22 = b1 * c2 - c1 * b2; x23 = b1 * c3 - c1 * b3; x24 = b1 * c0 - c1 * b0;
                        x33 = a3; x34 = a0;
                    }
                }
                else
                { // Erste mit dritter Zeile vertauschen
                    x11 = c1; x12 = c2; x13 = c3; x14 = c0;
                    if (a2 != 0)
                    {
                        x22 = a2; x23 = a3; x24 = a0;
                        x33 = a2 * b3 - b2 * a3; x34 = a2 * b0 - b2 * a0;
                    }
                    else
                    { // Zweite mit dritter Zeile vertauschen
                        x22 = b2; x23 = b3; x24 = b0;
                        x33 = a3; x34 = a0;
                    }
                }
            }

            if (x33 == 0)
            {
                if (x34 != 0)
                {
                    return new Vec3(0, 0, 0);
                }
                else
                {
                    u = 0; // u beliebig wählbar, bei u=0 liegt Punkt auf der Fläche
                    if (x22 == 0)
                        t = 1; // t beliebig wählbar
                    else
                        t = x24 / x22;
                    if (x11 == 0)
                        s = 1; // s beliebig wählbar
                    else
                        s = (x14 - x12 * t) / x11;
                }
            }
            else
            { // x33!=0
                u = x34 / x33;
                if (x22 == 0)
                    t = 1; // t beliebig wählbar
                else
                    t = (x24 - x23 * u) / x22;
                if (x11 == 0)
                    s = 1; // s beliebig wählbar
                else
                    s = (x14 - x12 * t - x13 * u) / x11;
            }
            return new Vec3(s, t, u);
        }


        /// <summary>
        /// Slide X-border (not used anymore).
        /// </summary>
        /// <param name="xdiff"></param>
        private void SlideX(double xdiff)
        {
            double endX = ParameterDict.Current.GetDouble("Border.Max.x");
            double startX = ParameterDict.Current.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX += xdiff * ddiff;
            startX += xdiff * ddiff;
            ParameterDict.Current.SetDouble("Border.Max.x", endX);
            ParameterDict.Current.SetDouble("Border.Min.x", startX);
        }


        /// <summary>
        /// Slide Y-border (not used anymore).
        /// </summary>
        private void SlideY(double ydiff)
        {
            double endY = ParameterDict.Current.GetDouble("Border.Max.y");
            double startY = ParameterDict.Current.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY += ydiff * ddiff;
            startY += ydiff * ddiff;
            ParameterDict.Current.SetDouble("Border.Max.y", endY);
            ParameterDict.Current.SetDouble("Border.Min.y", startY);
        }


        /// <summary>
        /// Slide Z-border (not used anymore).
        /// </summary>
        /// <param name="zdiff"></param>
        private void SlideZ(double zdiff)
        {
            double endZ = ParameterDict.Current.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Current.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ += zdiff * ddiff;
            startZ += zdiff * ddiff;
            ParameterDict.Current.SetDouble("Border.Max.z", endZ);
            ParameterDict.Current.SetDouble("Border.Min.z", startZ);
        }


        /// <summary>
        /// Move right.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(1, 0, 0), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Move up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTop_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(0, 0, -1), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Move down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(0, 0, 1), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Move forward.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnForward_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(0, 1, 0), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Move backwards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackwards_Click(object sender, EventArgs e)
        {
            if (mFactor == 0)
                return;
            UpdateCenterDiff();
            Vec3 trans = SolveEqusyst(new Vec3(0, -1, 0), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// X Zoom (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomX_Click(object sender, EventArgs e)
        {
            double endX = ParameterDict.Current.GetDouble("Border.Max.x");
            double startX = ParameterDict.Current.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX -= ddiff / mZoomFactor;
            startX += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.x", endX);
            ParameterDict.Current.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Y-Zoom (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomY_Click(object sender, EventArgs e)
        {
            double endY = ParameterDict.Current.GetDouble("Border.Max.y");
            double startY = ParameterDict.Current.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY -= ddiff / mZoomFactor;
            startY += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.y", endY);
            ParameterDict.Current.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Z-Zoom (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomZ_Click(object sender, EventArgs e)
        {
            double endZ = ParameterDict.Current.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Current.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ -= ddiff / mZoomFactor;
            startZ += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.z", endZ);
            ParameterDict.Current.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Zoom out X (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            double endX = ParameterDict.Current.GetDouble("Border.Max.x");
            double startX = ParameterDict.Current.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX += ddiff / mZoomFactor;
            startX -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.x", endX);
            ParameterDict.Current.SetDouble("Border.Min.x", startX);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Zoom out Y (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomYout_Click(object sender, EventArgs e)
        {
            double endY = ParameterDict.Current.GetDouble("Border.Max.y");
            double startY = ParameterDict.Current.GetDouble("Border.Min.y");
            double ddiff = endY - startY;
            endY += ddiff / mZoomFactor;
            startY -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.y", endY);
            ParameterDict.Current.SetDouble("Border.Min.y", startY);
            DrawAndWriteInHistory();

        }


        /// <summary>
        /// Zoom aout Z (not used).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZoomZout_Click(object sender, EventArgs e)
        {
            double endZ = ParameterDict.Current.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Current.GetDouble("Border.Min.z");
            double ddiff = endZ - startZ;
            endZ += ddiff / mZoomFactor;
            startZ -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.z", endZ);
            ParameterDict.Current.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        protected double mFactor = 6;

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(tbMove.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mFactor))
                tbMove.ForeColor = Color.Black;
            else
                tbMove.ForeColor = Color.Red;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            RotateX(-Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }


        protected double mZoomFactor = 6;


        /// <summary>
        /// Parse Zoomfactor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbZoomFactor_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(tbZoomFactor.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mZoomFactor))
                tbZoomFactor.ForeColor = Color.Black;
            else
                tbZoomFactor.ForeColor = Color.Red;
            if (mZoomFactor <= 2)
            {
                mZoomFactor = 2.0001;
                tbZoomFactor.Text = "2.0001";
            }
        }


        protected double mAngle = 10;


        /// <summary>
        /// Rotationswinkel wird geparst
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAngle_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(tbAngle.Text, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out mAngle))
                tbAngle.ForeColor = Color.Black;
            else
                tbAngle.ForeColor = Color.Red;

        }


        void RotateX(double angle)
        {

            Fractrace.Geometry.VecRotation rotation = new VecRotation();

            rotation.FromEuler(Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleX") / 180.0,
                -Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleY") / 180.0,
                Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ") / 180.0);

            rotation.Normalize();
            rotation.combine(-angle, 0, 0);

            double ax = 0, ay = 0, az = 0;
            rotation.toEuler(ref ax, ref ay, ref az);

            ax = 180 * ax / Math.PI;
            ay = 180 * ay / Math.PI;
            az = 180 * az / Math.PI;

            ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", ax);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", -ay);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", az);
        }


        void RotateY(double angle)
        {

            Fractrace.Geometry.VecRotation rotation = new VecRotation();

            rotation.FromEuler(
                Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleX") / 180.0,
               -Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleY") / 180.0,
                Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ") / 180.0
                );

            rotation.Normalize();
            rotation.combine(0, angle, 0);

            double ax = 0, ay = 0, az = 0;
            rotation.toEuler(ref ax, ref ay, ref az);

            ax = 180 * ax / Math.PI;
            ay = 180 * ay / Math.PI;
            az = 180 * az / Math.PI;

            ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", ax);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", -ay);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", az);
        }


        void RotateZ(double angle)
        {

            Fractrace.Geometry.VecRotation rotation = new VecRotation();
            rotation.FromEuler(Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleX") / 180.0,
                Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleY") / 180.0,
                Math.PI * ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ") / 180.0);

            rotation.Normalize();
            rotation.combine(0, 0, angle);

            double ax = 0, ay = 0, az = 0;
            rotation.toEuler(ref ax, ref ay, ref az);

            ax = 180 * ax / Math.PI;
            ay = 180 * ay / Math.PI;
            az = 180 * az / Math.PI;

            ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", ax);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", ay);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", az);
        }


        private void btnRotX_Click(object sender, EventArgs e)
        {
            RotateX(Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }


        private void btnRotY_Click(object sender, EventArgs e)
        {
            RotateY(Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }



        private void btnRotZ_Click(object sender, EventArgs e)
        {
            RotateZ(Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();

        }

        private void btnRotYneg_Click(object sender, EventArgs e)
        {
            RotateY(-Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }

        private void btnRotZneg_Click(object sender, EventArgs e)
        {
            RotateZ(-Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Zeichnet und aktualisiert die History.
        /// </summary>
        private void DrawAndWriteInHistory()
        {
            DrawPreview();
            mParent.AddToHistory();
            UpdateFromChangeProperty();
        }


        private void DrawPreview()
        {
            ResultImageView.PublicForm.Stop();
            if (ParameterInput.MainParameterInput.StereoForm != null)     
                 ParameterInput.MainParameterInput.StereoForm.Abort();
            mPreview.Draw();
        }

        /// <summary>
        /// Is called, if some properties changed.
        /// </summary>
        public void UpdateFromChangeProperty()
        {
            _propertyControl.UpdateElements();
            _propertyControlBbox.UpdateElements();
        }


        /// <summary>
        /// Handles the Click event of the btnZoomIn control.
        /// Hineinzoomen
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnZoomIn_Click(object sender, EventArgs e)
        {

            double endX = ParameterDict.Current.GetDouble("Border.Max.x");
            double startX = ParameterDict.Current.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX -= ddiff / mZoomFactor;
            startX += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.x", endX);
            ParameterDict.Current.SetDouble("Border.Min.x", startX);
       
            double endY = ParameterDict.Current.GetDouble("Border.Max.y");
            double startY = ParameterDict.Current.GetDouble("Border.Min.y");
            ddiff = endY - startY;
            endY -= ddiff / mZoomFactor;
            startY += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.y", endY);
            ParameterDict.Current.SetDouble("Border.Min.y", startY);
       
            double endZ = ParameterDict.Current.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Current.GetDouble("Border.Min.z");
            ddiff = endZ - startZ;
            endZ -= ddiff / mZoomFactor;
            startZ += ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.z", endZ);
            ParameterDict.Current.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();

        }


        private void btnZoomOut_Click(object sender, EventArgs e)
        {

            double endX = ParameterDict.Current.GetDouble("Border.Max.x");
            double startX = ParameterDict.Current.GetDouble("Border.Min.x");
            double ddiff = endX - startX;
            endX += ddiff / mZoomFactor;
            startX -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.x", endX);
            ParameterDict.Current.SetDouble("Border.Min.x", startX);
  
            double endY = ParameterDict.Current.GetDouble("Border.Max.y");
            double startY = ParameterDict.Current.GetDouble("Border.Min.y");
            ddiff = endY - startY;
            endY += ddiff / mZoomFactor;
            startY -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.y", endY);
            ParameterDict.Current.SetDouble("Border.Min.y", startY);
    
            double endZ = ParameterDict.Current.GetDouble("Border.Max.z");
            double startZ = ParameterDict.Current.GetDouble("Border.Min.z");
            ddiff = endZ - startZ;
            endZ += ddiff / mZoomFactor;
            startZ -= ddiff / mZoomFactor;
            ParameterDict.Current.SetDouble("Border.Max.z", endZ);
            ParameterDict.Current.SetDouble("Border.Min.z", startZ);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Create an automatic aspect corresponding the screen size. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Click(object sender, EventArgs e)
        {
            Navigator.SetAspectRatio();
        }

        private void btnMoveFast_Click(object sender, EventArgs e)
        {
            tbMove.Text = "2";
            tbZoomFactor.Text = "5";
            UpdateMoveButtonAppearance();
        }

        private void UpdateMoveButtonAppearance()
        {
            btnMoveFast.FlatStyle = FlatStyle.Flat;
            btnMoveNormal.FlatStyle = FlatStyle.Flat;
            btnMoveSlow.FlatStyle = FlatStyle.Flat;
            btnMoveFine.FlatStyle = FlatStyle.Flat;
            if (tbMove.Text == "2")
                btnMoveFast.FlatStyle = FlatStyle.Standard;
            if (tbMove.Text == "6")
                btnMoveNormal.FlatStyle = FlatStyle.Standard;
            if (tbMove.Text == "16")
                btnMoveSlow.FlatStyle = FlatStyle.Standard;
            if (tbMove.Text == "64")
                btnMoveFine.FlatStyle = FlatStyle.Standard;
        }


        private void btnMoveNormal_Click(object sender, EventArgs e)
        {
            tbMove.Text = "6";
            tbZoomFactor.Text = "7";
            UpdateMoveButtonAppearance();
        }


        private void btnMoveSlow_Click(object sender, EventArgs e)
        {
            tbMove.Text = "16";
            tbZoomFactor.Text = "16";
            UpdateMoveButtonAppearance();
        }


        private void btnMoveFine_Click(object sender, EventArgs e)
        {
            tbMove.Text = "64";
            tbZoomFactor.Text = "64";
            UpdateMoveButtonAppearance();
        }


        private void btnMoveAngleFast_Click(object sender, EventArgs e)
        {
            tbAngle.Text = "10";
            UpdateMoveAngleButtonAppearance();
        }


        private void btnMoveAngleNormal_Click(object sender, EventArgs e)
        {
            tbAngle.Text = "2";
            UpdateMoveAngleButtonAppearance();
        }


        private void btnMoveAngleFine_Click(object sender, EventArgs e)
        {
            tbAngle.Text = "0.1";
            UpdateMoveAngleButtonAppearance();
        }


        private void UpdateMoveAngleButtonAppearance()
        {
            btnMoveAngleFast.FlatStyle = FlatStyle.Flat;
            btnMoveAngleNormal.FlatStyle = FlatStyle.Flat;
            btnMoveAngleFine.FlatStyle = FlatStyle.Flat;
            if (tbAngle.Text == "10")
                btnMoveAngleFast.FlatStyle = FlatStyle.Standard;
            if (tbAngle.Text == "2")
                btnMoveAngleNormal.FlatStyle = FlatStyle.Standard;
            if (tbAngle.Text == "0.1")
                btnMoveAngleFine.FlatStyle = FlatStyle.Standard;
        }

       
    }
}
