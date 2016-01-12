using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.Scheduler.BatchProcess
{
    public class X3dExportBatchProcess : BatchProcess
    {

        public override string Description()
        {
            return "BatchProcess";
        }


        public string ExportFile = "";

        public override void OnStart()
        {

        }


        /// <summary>
        /// Return true, if after generation a new process should be startet.
        /// </summary>
        /// <returns></returns>
        public override bool OnPictureCreated(Iterate iter, PictureData pictureData)
        {
            X3dExporter export = new X3dExporter(iter, pictureData);
            export.Save(ExportFile);
            return false;
        }


    }
}
