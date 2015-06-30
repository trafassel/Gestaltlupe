using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fractrace.Basic;

namespace Fractrace.Scheduler
{


    class GrandScheduler
    {

        GrandScheduler()
        {
            mainDisplayForm = Fractrace.Form1.PublicForm;
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
        /// Reference to the Form which display Rendered image.
        /// </summary>
        Fractrace.Form1 mainDisplayForm = null;


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
        protected System.Collections.Generic.List<System.Threading.Thread> threads;


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
                    System.Diagnostics.Debug.WriteLine("Error in GrandSchedulerInstance.AddThread(): Some old threads still running.");
                }
                threads.Add(thread);
            }
        }


        /// <summary>
        /// Starts computing surface model.
        /// </summary>
        public void ComputeOneStep()
        {
            lock (historyFreezedObj)
            {
                historyFreezed = true;
                mainDisplayForm.ComputeOneStep();
            }
        }


        public void ComputeOneStepEnds()
        {

        }



    }


   
}
