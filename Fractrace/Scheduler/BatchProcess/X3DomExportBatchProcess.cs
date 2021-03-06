﻿using Fractrace.Basic;
using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.Scheduler.BatchProcess
{
   
  public class X3DomExportBatchProcess : BatchProcess
    {

        /// <summary>
        /// type == 2: front, backside.
        /// type == 3: front, backside, top, bottom,right,left.
        /// type == 4: front, backside, top, bottom,right,left,top-left,top-right....
        /// </summary>
        protected int _type = 4;

        /// <summary>
        /// Number of batch steps
        /// </summary>
        protected int _steps = 0;

        /// <summary>
        /// Current batch step. (start with 1).
        /// </summary>
        protected int _currentStep = 0;

        public string ExportFile = "";

        protected List<string> _createdFiles = new List<string>();

        public override string Description() { return "X3d Export"; }


        // saved ParameterDict parameters
        double _startAngleX = 0;
        double _startAngleY = 0;
        double _startAngleZ = 0;



        public X3DomExportBatchProcess()
        {
            _type = ParameterDict.Current.GetInt("Export.X3d.BatchType");

            if (_type == 2)
            {
                _steps = 2;
            }
            if (_type == 3)
            {
                _steps = 6;
            }
            if (_type == 4)
            {
                _steps = 18;
            }
            PrepareStep();
        }


        protected void PrepareStep()
        {
            if (_currentStep == 0)
            {
                // TODO: save current ParameterDict.
            }
            _currentStep++;
            if (_type == 2)
            {
                if (_currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ",
                        ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ") + 180.0);
                }
            }

            if (_type == 3)
            {
                if (_currentStep == 1)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
                if (_currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 90);
                }
                if (_currentStep == 3)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 180);
                }
                if (_currentStep == 4)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 270);
                }
                if (_currentStep == 5)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
                if (_currentStep == 6)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
            }

            if (_type == 4)
            {
                if (_currentStep == 1)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
                if (_currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 90);
                }
                if (_currentStep == 3)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 180);
                }
                if (_currentStep == 4)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 270);
                }
                if (_currentStep == 5)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
                if (_currentStep == 6)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 0);
                }
                if (_currentStep == 7)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45);
                }
                if (_currentStep == 8)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 90);
                }
                if (_currentStep == 9)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 180);
                }
                if (_currentStep == 10)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 270);
                }
                if (_currentStep == 11)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45);
                }
                if (_currentStep == 12)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 90);
                }
                if (_currentStep == 13)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 180);
                }
                if (_currentStep == 14)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 270);
                }
                if (_currentStep == 15)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45);
                }
                if (_currentStep == 16)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 90);
                }
                if (_currentStep == 17)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 180);
                }
                if (_currentStep == 18)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX + 90 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY + 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ + 45 + 270);
                }
            }

        }


        Fractrace.SceneGraph.X3DomExporter _exporter = null;

        protected void StepEnds()
        {
            if(_exporter==null)
            {
                _exporter = new SceneGraph.X3DomExporter(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
                _exporter.Init(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
            }
            _exporter.Update(ResultImageView.PublicForm.IterateForPictureArt, ResultImageView.PublicForm.LastPicturArt.PictureData);
        }


        /// <summary>
        /// Is called after last batch step ist called.
        /// </summary>
        protected void BatchEnds()
        {
            // TODO: let exporter write result file.
            _exporter.UpdateExport(ExportFile);
            // restore current ParameterDict.
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", _startAngleX);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", _startAngleY);
            ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", _startAngleZ);

            Fractrace.Gui.ExportResultDialog exportResultDialog = new Gui.ExportResultDialog(ExportFile);
            exportResultDialog.ShowDialog();
            if (exportResultDialog.OpenInBrowser)
                System.Diagnostics.Process.Start(ExportFile);
        }


        public override void OnStart()
        {
            _startAngleX = ParameterDict.Current.GetDouble("Transformation.Camera.AngleX");
            _startAngleY = ParameterDict.Current.GetDouble("Transformation.Camera.AngleY");
            _startAngleZ = ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ");
        }


        /// <summary>
        /// Return true, if after generation a new process should be startet.
        /// </summary>
        /// <returns></returns>
        public override bool OnPictureCreated(Iterate iter, PictureData pictureData)
        {
            StepEnds();

            bool retVal = _currentStep < _steps;
            if (retVal)
                PrepareStep();
            else
                BatchEnds();
            return retVal;
        }


    }
}
