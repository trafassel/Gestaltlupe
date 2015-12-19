using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Fractrace;
using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;

namespace Fractrace.Scheduler
{
    public class PaintJob : IAsyncComputationStarter
    {

        ParameterDict mParameters = null;

        IAsyncComputationStarter mmaster = null;


        /// <summary>
        /// Number of subrendering steps 
        /// </summary>
        int mUpdateSteps = 0;

        /// <summary>
        /// Current iterate.
        /// </summary>
        Iterate mIterate = null;


        /// <summary>
        /// Iterate in last run.
        /// </summary>
        Iterate mLastIterate = null;

        GrandScheduler scheduler = GrandScheduler.Exemplar;

        Graphics mGraphics = null;

        double mCurrentProgress = 0;

        double mCurrentProgressd = 1;

        /// <summary>
        /// (master is used for progress bar).
        /// </summary>
        /// <param name="master"></param>
        public PaintJob(IAsyncComputationStarter master,Graphics graphics)
        {
            mmaster = master;
            mGraphics = graphics;
        }


        bool mAbort = false;


        /// <summary>
        /// Stop computing.
        /// </summary>
        public void Abort()
        {
            mAbort = true;
            if (mIterate != null)
                mIterate.Abort();
        }


        /// <summary>
        /// Start Computing.
        /// </summary>
        /// <param name="updateSteps"></param>
        public void Run(int updateSteps)
        {
            mCurrentProgress = 0;
            mmaster.Progress(mCurrentProgress);
            System.Diagnostics.Debug.WriteLine("PaintJob.Run " + updateSteps.ToString());
            mParameters = ParameterDict.Exemplar.Clone();
            mUpdateSteps = updateSteps;
            mCurrentProgressd = 100.0 / (double)(mUpdateSteps);
            for (int i = 0; i < mUpdateSteps; i++)
            {
               
                if (mAbort)
                    return;
                mIterate = new Iterate(mParameters, this, false);
                if (mLastIterate != null)
                {
                    mIterate.SetOldData(mLastIterate.GraphicInfo,mLastIterate.PictureData,i);
                }
                if (mAbort)
                    return;
                mIterate.StartAsync();
                mIterate.Wait();
                if (mAbort)
                    return;
                // TODO: View preview
                mLastIterate = mIterate;
                mCurrentProgress += mCurrentProgressd;
                mmaster.Progress(mCurrentProgress);
            }
            // Hint: Clone() is not neccessary.
            Renderer renderer = new PlasicRenderer(mIterate.PictureData.Clone());
            renderer.Init(mIterate.LastUsedFormulas);
            renderer.Paint(mGraphics);
            if (mAbort)
                return;
            // TODO: Show in Preview, save image and data
            System.Diagnostics.Debug.WriteLine("PaintJob.Run Ends");
            mmaster.Progress(0);
            // TODO: Draw progress bar
        }


        


        /// <summary>
        /// Is called, if computation of mIterate ends.
        /// </summary>
        public void ComputationEnds()
        {
            //scheduler.ComputationEnds();
            // Call next iteration ...
            // Start using Picture Art
        }


        /// <summary>
        /// Show progress. Valid values are [0, ..., 100].
        /// </summary>
        /// <param name="progrssInPercent"></param>
        public void Progress(double progressInPercent)
        {
           // mmaster.Progress(progressInPercent);

            double progress = mCurrentProgress + progressInPercent / (double)(mUpdateSteps );

            System.Diagnostics.Debug.WriteLine("Progress: " + progressInPercent.ToString() + " =" + progress);
            if (progress > 100)
                progress = 100;

           mmaster.Progress(progress);
             
        }


    }
}
