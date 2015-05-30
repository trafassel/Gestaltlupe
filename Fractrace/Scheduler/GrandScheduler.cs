using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fractrace.Basic;

namespace Fractrace.Scheduler
{


    class GrandSchedulerInstance
    {

        GrandSchedulerInstance()
        {
            mainDisplayForm = Fractrace.Form1.PublicForm;
            mainParameterInput = Fractrace.ParameterInput.MainParameterInput;
            history = mainParameterInput.History;
        }


        /// <summary>
        /// Used by the singleton design pattern.
        /// </summary>
        protected static GrandSchedulerInstance mExemplar = null;


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
        public static GrandSchedulerInstance Exemplar
        {
            get
            {
                lock (lockVar)
                {
                    if (mExemplar == null)
                    {
                        mExemplar = new GrandSchedulerInstance();
                    }
                }
                return mExemplar;
            }
        }

    }


   
}
