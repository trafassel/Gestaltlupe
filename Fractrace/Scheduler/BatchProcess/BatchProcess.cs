using Fractrace.DataTypes;

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
        public virtual bool OnPictureCreated(Iterate iter, PictureData pictureData)
        {
            return false;
        }


    }
}
