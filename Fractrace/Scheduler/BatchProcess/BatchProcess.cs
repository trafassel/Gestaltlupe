using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.Scheduler.BatchProcess
{
    public class BatchProcess
    {


        public BatchProcess()
        {

        }

        public virtual string Description()
        {
            return "BatchProcess";
        }


        public virtual void OnStart()
        {

        }


        /// <summary>
        /// Return true, if after generation a new process should be startet.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnPictureCreated(Iterate iter, PictureData pictureData)
        {
            return false;
        }


    }
}
