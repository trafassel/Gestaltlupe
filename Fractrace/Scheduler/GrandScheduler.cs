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
            mainDisplayForm = Fractrace.ResultImageView.PublicForm;
            mainParameterInput = Fractrace.ParameterInput.MainParameterInput;
            history = mainParameterInput.History;
        }



        /// <summary>
        /// Used by the singleton design pattern.
        /// </summary>
        protected static GrandScheduler mExemplar = null;


        /// <summary>
        ///  Used by the singleton design pattern.
        /// </summary>
        protected static Object lockVar = new Object();


        /// <summary>
        /// Runing jobs.
        /// </summary>
        protected List<PaintJob> jobs = new List<PaintJob>();


        /// <summary>
        /// Reference to the Form which display Rendered image.
        /// </summary>
        Fractrace.ResultImageView mainDisplayForm = null;


        /// <summary>
        /// Reference to the parameter input control.
        /// </summary>
        Fractrace.ParameterInput mainParameterInput = null;


        /// <summary>
        ///  Reference to history entries, used in history control and animations.
        /// </summary>
        ParameterHistory history = null;


        /// <summary>
        /// 
        /// </summary>
        protected long time = 0;


        /// <summary>
        /// Gets the unique static instance of this class.
        /// </summary>
        /// <value>The exemplar.</value>
        public static GrandScheduler Exemplar
        {
            get
            {
                lock (lockVar)
                {
                    if (mExemplar == null)
                    {
                        mExemplar = new GrandScheduler();
                    }
                }
                return mExemplar;
            }
        }

        


        /// <summary>
        /// True, if current input values are freezed.
        /// </summary>
        protected bool historyFreezed = false;

        protected object historyFreezedObj = new object();


        /// <summary>
        ///  Contains list of all running threads.
        /// </summary>
        protected System.Collections.Generic.List<System.Threading.Thread> threads=new List<System.Threading.Thread>();


        /// <summary>
        /// Add given thread to threads. Previously closed threads are removed from threads.
        /// </summary>
        /// <param name="thread"></param>
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


        /// <summary>
        /// Starts computing surface model.
        /// </summary>
        public void ComputeOneStep()
        {
            System.Diagnostics.Debug.WriteLine("GrandScheduler.ComputeOneStep");
            lock (historyFreezedObj)
            {
                historyFreezed = true;
                mainDisplayForm.ComputeOneStep();
            }
        }


        public bool dontActivateRender = false;


        /// <summary>
        /// Indicates, that the application is computing a bitmap of the "Gestalt". 
        /// </summary>
        public bool inComputeOneStep = false;



        /// <summary>
        /// Is called, if mainDisplayForm.ComputeOneStep ends.
        /// </summary>
        public void ComputeOneStepEnds()
        {
            if (!dontActivateRender)
                mainDisplayForm.ActivatePictureArt();
            inComputeOneStep = false;
        }


        /// <summary>
        /// Is called, if drawing of image ist ready.
        /// </summary>
        protected void PictureArtEnds()
        {

         

            // TODO: Save image
            // TODO: Save in history
            // TODO: Start waiting threads
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
