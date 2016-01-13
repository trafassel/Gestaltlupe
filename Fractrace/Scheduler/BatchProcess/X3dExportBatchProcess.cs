using Fractrace.Basic;
using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.Scheduler.BatchProcess
{
    public class X3dExportBatchProcess : BatchProcess
    {


        public X3dExportBatchProcess()
        {
            if (type == 2)
            {
                steps = 2;
            }
            if (type == 3)
            {
                steps = 6;
            }
            if (type == 4)
            {
                steps = 14;
            }
            PrepareStep();
        }


        public override string Description()
        {
            return "BatchProcess";
        }


        protected List<string> _createdFiles = new List<string>();

        protected void PrepareStep()
        {
            if (currentStep == 0)
            {
                // TODO: save current ParameterDict.
            }
            currentStep++;
            if (type == 2)
            {
                if (currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ",
                        ParameterDict.Current.GetDouble("Transformation.Camera.AngleZ") + 180.0);
                }
            }

            if (type == 3)
            {
                if (currentStep == 1)
                {
                    ParameterDict.Current.SetBool("Export.X3d.ClosedSurface", false);
                  //  ParameterDict.Current.SetDouble("Export.X3d.ClosedSurfaceDist", 1.3);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
                if (currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 90);
                }
                if (currentStep == 3)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 180);
                }
                if (currentStep == 4)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 270);
                }
                if (currentStep == 5)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 90);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
                if (currentStep == 6)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
            }

            if (type == 4)
            {
                if (currentStep == 1)
                {
                    ParameterDict.Current.SetBool("Export.X3d.ClosedSurface", false);
                   // ParameterDict.Current.SetDouble("Export.X3d.ClosedSurfaceDist", 1.3);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
                if (currentStep == 2)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 90);
                }
                if (currentStep == 3)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 180);
                }
                if (currentStep == 4)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 270);
                }
                if (currentStep == 5)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 90);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
                if (currentStep == 6)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 0);
                }
                if (currentStep == 7)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45);
                }
                if (currentStep == 8)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45+90);
                }
                if (currentStep == 9)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45+180);
                }
                if (currentStep == 10)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45+270);
                }
                if (currentStep == 11)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270-45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45);
                }
                if (currentStep == 12)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45+90);
                }
                if (currentStep == 13)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45+180);
                }
                if (currentStep == 14)
                {
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleX", 270 - 45);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleY", 0);
                    ParameterDict.Current.SetDouble("Transformation.Camera.AngleZ", 45 + 270);
                }
            }

        }

        protected void StepEnds()
        {
            X3dExporter export = new X3dExporter(_iter, _pictureData);
            string filename = ExportFile;
            filename = filename.Replace(".wrl", "_temp"+currentStep.ToString() + ".wrl");
            export.Save(filename);
            _createdFiles.Add(filename);
        }


        /// <summary>
        /// Is called after last batch step ist called.
        /// </summary>
        protected void BatchEnds()
        {
            // combine created files
            FileStream resultFile = File.Open(ExportFile, FileMode.Create);
            foreach (string file in _createdFiles)
            {
                FileStream fileStream = File.Open(file, FileMode.Open);
                byte[] fileContent = new byte[fileStream.Length];
                fileStream.Read(fileContent, 0, (int)fileStream.Length);
                resultFile.Write(fileContent, 0, (int)fileStream.Length);
                fileStream.Close();
            }
            resultFile.Close();
            foreach (string file in _createdFiles)
            {
                System.IO.File.Delete(file);
            }
            System.Windows.Forms.MessageBox.Show("File " + ExportFile + " created.");
            // TODO: restore current ParameterDict.
        }


        /// <summary>
        /// type == 2: front, backside.
        /// type == 3: front, backside, top, bottom,right,left.
        /// type == 4: front, backside, top, bottom,right,left,top-left,top-right....
        /// </summary>
        protected int type = 4;

        /// <summary>
        /// Number of batch steps
        /// </summary>
        protected int steps = 0;

        /// <summary>
        /// Current batch step. (start with 1).
        /// </summary>
        protected int currentStep = 0;

        public string ExportFile = "";

        public override void OnStart()
        {

        }

        Iterate _iter = null;
        PictureData _pictureData = null;


        /// <summary>
        /// Return true, if after generation a new process should be startet.
        /// </summary>
        /// <returns></returns>
        public override bool OnPictureCreated(Iterate iter, PictureData pictureData)
        {
            _iter = iter;
            _pictureData = pictureData;
            StepEnds();

            bool retVal = currentStep < steps;
            if (retVal)
                PrepareStep();
            else
                BatchEnds();
            return retVal;
        }


    }
}
