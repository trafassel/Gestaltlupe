using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fractrace.Basic;
using Fractrace.DataTypes;

namespace Fractrace.Scheduler
{


    class GrandScheduler
    {

        GrandScheduler()
        {
            _mainDisplayForm = Fractrace.ResultImageView.PublicForm;
        }


        protected static GrandScheduler _exemplar = null;
        protected static Object _lockVar = new Object();


        /// <summary>
        /// Runing jobs.
        /// </summary>
        protected List<PaintJob> _jobs = new List<PaintJob>();


        /// <summary>
        /// Reference to the Form which display Rendered image.
        /// </summary>
        Fractrace.ResultImageView _mainDisplayForm = null;

        /// <summary>
        /// Gets the unique static instance of this class.
        /// </summary>
        public static GrandScheduler Exemplar
        {
            get
            {
                lock (_lockVar)
                {
                    if (_exemplar == null)
                    {
                        _exemplar = new GrandScheduler();
                    }
                }
                return _exemplar;
            }
        }


        /// <summary>
        /// True, if current input values are freezed.
        /// </summary>
        protected bool _historyFreezed = false;

        protected object _historyFreezedObj = new object();


        /// <summary>
        ///  Contains list of all running threads.
        /// </summary>
        protected System.Collections.Generic.List<System.Threading.Thread> threads=new List<System.Threading.Thread>();


        /// <summary>
        /// Add given thread to threads. Previously closed threads are removed from threads.
        /// </summary>
        public void AddThread(System.Threading.Thread thread)
        {
            lock (threads)
            {
                System.Collections.Generic.List<System.Threading.Thread> toDel = new List<System.Threading.Thread>();
                foreach (System.Threading.Thread oldThread in threads)
                {
                    if (!oldThread.IsAlive)
                    {
                        toDel.Add(oldThread);
                    }
                }
                foreach (System.Threading.Thread oldThread in toDel)
                    threads.Remove(oldThread);
                if (threads.Count > 0)
                {
                    // If there is only one thread left, evythhing should be ok, because in redraw new pixel art ist startet before old pixel art stopped.
                    System.Diagnostics.Debug.WriteLine("Error in GrandSchedulerInstance.AddThread(): Some old threads still running. (Count:" + threads.Count);
                }
                threads.Add(thread);
            }
        }


        public bool DontActivateRender = false;


        /// <summary>
        /// Indicates, that the application is computing a bitmap of the "Gestalt". 
        /// </summary>
        public bool inComputeOneStep = false;


        /// <summary>
        /// Is called, if mainDisplayForm.ComputeOneStep ends.
        /// </summary>
        public void ComputeOneStepEnds()
        {
            if (!DontActivateRender)
                _mainDisplayForm.ComputeOneStepEnds();
            inComputeOneStep = false;
        }


        /// <summary>
        /// Is called, if drawing of image ist ready.
        /// </summary>
        protected void PictureArtEnds()
        {         

        }


        /// <summary>
        /// Is called, if result picture is created.
        /// Return true if new rendering should be startet.
        /// </summary>
        public bool PictureIsCreated(Iterate iter, PictureData pictureData)
        {
            if (_batchProcess!=null)
            {
                bool retVal= _batchProcess.OnPictureCreated( iter,  pictureData);
                if (!retVal)
                    _batchProcess = null;
                return retVal;
            }
            return false;
        }


        BatchProcess.BatchProcess _batchProcess = null;


        public void SetBatch(BatchProcess.BatchProcess batchProcess)
        {
            if(_batchProcess==null)
            {
                System.Diagnostics.Debug.WriteLine("Error in GrandScheduler.SetBatch() old batchProcess is still running.");
            }
            _batchProcess = batchProcess;
        }


    }   
}
