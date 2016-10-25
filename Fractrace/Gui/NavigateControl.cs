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
            panel2.Visible = false;

            UpdateMoveButtonAppearance();
            UpdateMoveAngleButtonAppearance();

            // This prevents the designer to read this block.
            if (ParameterInput.MainParameterInput != null && ParameterInput.MainParameterInput.MainDataViewControl != null)
            {
                _propertyControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _propertyControl.Dock = DockStyle.Fill;
                _propertyControl.Create("Transformation");
                pnlProperties.Controls.Add(_propertyControl);
                _propertyControlBbox = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _propertyControlBbox.Dock = DockStyle.Fill;
                _propertyControlBbox.Create("Scene");
                pnlBorderProperties.Controls.Add(_propertyControlBbox);
            }
            mZoomFactor = 1.2;
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
        PreviewControl _preview = null;

        /// <summary>
        /// The "right eye"-view of the control.
        /// </summary>
        PreviewControl _previewStereo = null;

        /// <summary>
        /// Parent control.
        /// </summary>
        ParameterInput _parent = null;

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
            _preview = preview;
            _previewStereo = preview2;
            _parent = parent;
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

            double centerX = ParameterDict.Current.GetDouble("Scene.CenterX");
            double centerY = ParameterDict.Current.GetDouble("Scene.CenterY");
            double centerZ = ParameterDict.Current.GetDouble("Scene.CenterZ");

            //Rotation rotView = null;
            double centerXChange = centerX + 1;
            double centerYChange = centerY + 1;
            double centerZChange = centerZ + 1;

            // For Zerotest
            double minDoubleVal = 0.0000000000000001;

            // This does not work for angle combinations:

            Rotation rotView = new Rotation();
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
        /// Move Scene.CenterX.
        /// </summary>
        private void SlideX(double xdiff)
        {
            double centerX = ParameterDict.Current.GetDouble("Scene.CenterX");
            double radius = ParameterDict.Current.GetDouble("Scene.Radius");
            ParameterDict.Current.SetDouble("Scene.CenterX", centerX + xdiff * radius);
        }


        /// <summary>
        /// Move Scene.CenterY.
        /// </summary>
        private void SlideY(double ydiff)
        {
            double centerY = ParameterDict.Current.GetDouble("Scene.CenterY");
            double radius = ParameterDict.Current.GetDouble("Scene.Radius");
            ParameterDict.Current.SetDouble("Scene.CenterY", centerY + ydiff * radius);
        }


        /// <summary>
        /// Move Scene.CenterZ.
        /// </summary>
        private void SlideZ(double zdiff)
        {
            double centerZ = ParameterDict.Current.GetDouble("Scene.CenterZ");
            double radius = ParameterDict.Current.GetDouble("Scene.Radius");
            ParameterDict.Current.SetDouble("Scene.CenterZ", centerZ + zdiff * radius);
        }


        /// <summary>
        /// Move right.
        /// </summary>
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
        /// Factor used in move.
        /// </summary>
        protected double mFactor = 12;

        private void button4_Click(object sender, EventArgs e)
        {
            RotateX(-Math.PI * mAngle / 180.0);
            DrawAndWriteInHistory();
        }


        protected double mZoomFactor = 1.2;

        protected double mAngle = 10;


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
            _parent.AddToHistory();
            UpdateFromChangeProperty();
        }


        public void DrawPreview()
        {
            System.Diagnostics.Debug.WriteLine("DrawPreview");
            ResultImageView.PublicForm.Stop();
            _preview.Draw();
            System.Diagnostics.Debug.WriteLine("DrawPreview Ends");
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
        /// Zoom in.
        /// </summary>
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            double radius = ParameterDict.Current.GetDouble("Scene.Radius");
            ParameterDict.Current.SetDouble("Scene.Radius", radius / mZoomFactor);
            DrawAndWriteInHistory();
        }


        /// <summary>
        /// Zoom out.
        /// </summary>
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            double radius = ParameterDict.Current.GetDouble("Scene.Radius");
            ParameterDict.Current.SetDouble("Scene.Radius", radius * mZoomFactor);
            DrawAndWriteInHistory();
        }


        private void btnMoveFast_Click(object sender, EventArgs e)
        {
            mFactor = 4;
            mZoomFactor = 1.2;
            UpdateMoveButtonAppearance();
        }


        private void UpdateMoveButtonAppearance()
        {
            btnMoveFast.Font = new Font(btnMoveFast.Font.Name, btnMoveFast.Font.Size,
                                        FontStyle.Regular, btnMoveFast.Font.Unit);
            btnMoveNormal.Font = new Font(btnMoveNormal.Font.Name, btnMoveNormal.Font.Size,
                                        FontStyle.Regular, btnMoveNormal.Font.Unit);
            btnMoveSlow.Font = new Font(btnMoveSlow.Font.Name, btnMoveSlow.Font.Size,
                                        FontStyle.Regular, btnMoveFine.Font.Unit);
            btnMoveFine.Font = new Font(btnMoveFine.Font.Name, btnMoveFine.Font.Size,
                                        FontStyle.Regular, btnMoveFine.Font.Unit);

            if (mFactor == 4)
                btnMoveFast.Font = new Font(btnMoveFast.Font.Name, btnMoveFast.Font.Size,
               FontStyle.Underline, btnMoveFast.Font.Unit);
            if (mFactor == 12)
                btnMoveNormal.Font = new Font(btnMoveNormal.Font.Name, btnMoveNormal.Font.Size,
              FontStyle.Underline, btnMoveNormal.Font.Unit);
            if (mFactor == 32)
                btnMoveSlow.Font = new Font(btnMoveSlow.Font.Name, btnMoveSlow.Font.Size,
             FontStyle.Underline, btnMoveSlow.Font.Unit);
            if (mFactor == 128)
                btnMoveFine.Font = new Font(btnMoveFine.Font.Name, btnMoveFine.Font.Size,
             FontStyle.Underline, btnMoveFine.Font.Unit);
        }


        private void btnMoveNormal_Click(object sender, EventArgs e)
        {
            mFactor = 12;
            mZoomFactor = 1.03;
            UpdateMoveButtonAppearance();
        }


        private void btnMoveSlow_Click(object sender, EventArgs e)
        {
            mFactor = 32;
            mZoomFactor = 1.01;
            UpdateMoveButtonAppearance();
        }


        private void btnMoveFine_Click(object sender, EventArgs e)
        {
            mFactor = 128;
            mZoomFactor = 1.005;
            UpdateMoveButtonAppearance();
        }


        private void btnMoveAngleFast_Click(object sender, EventArgs e)
        {
            mAngle = 10;
            UpdateMoveAngleButtonAppearance();
        }


        private void btnMoveAngleNormal_Click(object sender, EventArgs e)
        {
            mAngle = 2;
            UpdateMoveAngleButtonAppearance();
        }


        private void btnMoveAngleFine_Click(object sender, EventArgs e)
        {
            mAngle = 0.1;
            UpdateMoveAngleButtonAppearance();
        }


        private void UpdateMoveAngleButtonAppearance()
        {
            btnMoveAngleFast.Font = new Font(btnMoveAngleFast.Font.Name, btnMoveAngleFast.Font.Size,
                          FontStyle.Regular, btnMoveAngleFast.Font.Unit);
            btnMoveAngleNormal.Font = new Font(btnMoveAngleNormal.Font.Name, btnMoveAngleNormal.Font.Size,
                          FontStyle.Regular, btnMoveAngleNormal.Font.Unit);
            btnMoveAngleFine.Font = new Font(btnMoveAngleFine.Font.Name, btnMoveAngleFine.Font.Size,
                          FontStyle.Regular, btnMoveAngleFine.Font.Unit);
            if (mAngle == 10)
                btnMoveAngleFast.Font = new Font(btnMoveAngleFast.Font.Name, btnMoveAngleFast.Font.Size,
                                                 FontStyle.Underline, btnMoveAngleFast.Font.Unit);
            if (mAngle == 2)
                btnMoveAngleNormal.Font = new Font(btnMoveAngleNormal.Font.Name, btnMoveAngleNormal.Font.Size,
                                                 FontStyle.Underline, btnMoveAngleNormal.Font.Unit);
            if (mAngle == 0.1)
                btnMoveAngleFine.Font = new Font(btnMoveAngleFine.Font.Name, btnMoveAngleFine.Font.Size,
                                                 FontStyle.Underline, btnMoveAngleFine.Font.Unit);
        }


        public void ZoomIn()
        {
            btnZoomIn_Click(null, null);
        }



        public void RotateSceneBottomView(int x, int y)
        {
            if (x != 0 || y != 0)
            {
                //                RotateX((double)y / -1.0 / 180.0);
                RotateY((double)y / -1.0 / 180.0);
                RotateZ((double)x / 1.0 / 180.0);
            }
        }


        /// <summary>
        /// Rotate scene. Center is scene center. Input is from bootom view mouse move event or fron view mouse move event.
        /// </summary>
        public void RotateScene(int x, int y)
        {
            if (x != 0 || y != 0)
            {
                RotateX((double)y / -1.0 / 180.0);
                RotateZ((double)x / 1.0 / 180.0);
            }
        }

        /// <summary>
        /// Slide scene. Input is from bootom view mouse move event.
        /// </summary>
        public void MoveSceneFromBottomView(int x, int y)
        {
            if (x != 0 || y != 0)
            {
                System.Diagnostics.Debug.WriteLine("MoveScene: " + x.ToString() + " " + y.ToString());

                {
                    UpdateCenterDiff();
                    Vec3 trans = SolveEqusyst(new Vec3(((double)x) / -10, 0, 0), centerDiffX, centerDiffY, centerDiffZ);
                    if (trans.X != 0)
                        SlideX(trans.X / mFactor);
                    if (trans.Y != 0)
                        SlideY(trans.Y / mFactor);
                    if (trans.Z != 0)
                        SlideZ(trans.Z / mFactor);
                }


                
                {
                    UpdateCenterDiff();
                    Vec3 trans = SolveEqusyst(new Vec3(0, ((double)y) / -10, 0), centerDiffX, centerDiffY, centerDiffZ);
                    if (trans.X != 0)
                        SlideX(trans.X / mFactor);
                    if (trans.Y != 0)
                        SlideY(trans.Y / mFactor);
                    if (trans.Z != 0)
                        SlideZ(trans.Z / mFactor);
                }
                


            }

        }


        public void MoveScene(int x,int y)
        {
            if(x!=0 || y !=0)
            {
                System.Diagnostics.Debug.WriteLine("MoveScene: " + x.ToString() + " " + y.ToString());

                {
                    UpdateCenterDiff();
                    Vec3 trans = SolveEqusyst(new Vec3(((double)x) / -10, 0, 0), centerDiffX, centerDiffY, centerDiffZ);
                    if (trans.X != 0)
                        SlideX(trans.X / mFactor);
                    if (trans.Y != 0)
                        SlideY(trans.Y / mFactor);
                    if (trans.Z != 0)
                        SlideZ(trans.Z / mFactor);
                }

               
                {
                    UpdateCenterDiff();
                    Vec3 trans = SolveEqusyst(new Vec3(0, 0, ((double)y) / -10), centerDiffX, centerDiffY, centerDiffZ);
                    if (trans.X != 0)
                        SlideX(trans.X / mFactor);
                    if (trans.Y != 0)
                        SlideY(trans.Y / mFactor);
                    if (trans.Z != 0)
                        SlideZ(trans.Z / mFactor);
                }
               

              //  DrawAndWriteInHistory();
            }
        }
        /*
        public void Zoom(double factor)
        {
            ParameterDict.Current.SetDouble("Scene.Radius", ParameterDict.Current.GetDouble("Scene.Radius") * factor);
            // if (mZoomFactor>1.1)
            {
                //                btnBackwards_Click(null, null);

                if (mFactor == 0)
                    return;
                if (factor >= 1)
                {
                    UpdateCenterDiff();
                    Vec3 trans = SolveEqusyst(new Vec3(0, -0.3, 0), centerDiffX, centerDiffY, centerDiffZ);
                    if (trans.X != 0)
                        SlideX(trans.X / mFactor);
                    if (trans.Y != 0)
                        SlideY(trans.Y / mFactor);
                    if (trans.Z != 0)
                        SlideZ(trans.Z / mFactor);
                    DrawAndWriteInHistory();

                }

                //     btnBackwards_Click(null, null);
                //     btnBackwards_Click(null, null);
            }
            DrawAndWriteInHistory();
        }
        */


        public void ZoomOut()
        {
            btnZoomOut_Click(null, null);
        }

        public void MoveLeftRightUpOrDown(int diffx, int diffy)
        {

            Vec3 trans = SolveEqusyst(new Vec3(-1, 0, 0), centerDiffX, centerDiffY, centerDiffZ);
            if (trans.X != 0)
                SlideX(trans.X / mFactor);
            if (trans.Y != 0)
                SlideY(trans.Y / mFactor);
            if (trans.Z != 0)
                SlideZ(trans.Z / mFactor);

            /*
            if (diffx < -2)
                navigateControl1.MoveLeft();
            if (diffx > 2)
                navigateControl1.MoveLeft();
            if (diffy < -2)
                navigateControl1.MoveUp();
            if (diffy > 2)
                navigateControl1.MoveDown();
                */
        }

    }
}
