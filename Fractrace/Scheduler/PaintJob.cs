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

        ParameterDict _parameters = null;

        IAsyncComputationStarter _master = null;

        /// <summary>
        /// Number of subrendering steps 
        /// </summary>
        int _updateSteps = 0;

        /// <summary>
        /// Current iterate.
        /// </summary>
        Iterate _iterate = null;

        /// <summary>
        /// Iterate in last run.
        /// </summary>
        Iterate _lastIterate = null;


        Graphics _graphics = null;

        double _currentProgress = 0;

        double _currentProgressd = 1;

        bool _abort = false;


        /// <summary>
        /// (master is used for progress bar).
        /// </summary>
        public PaintJob(IAsyncComputationStarter master,Graphics graphics)
        {
            _master = master;
            _graphics = graphics;
        }


        /// <summary>
        /// Stop computing.
        /// </summary>
        public void Abort()
        {
            _abort = true;
            if (_iterate != null)
                _iterate.Abort();
        }


        /// <summary>
        /// Start Computing. Is called while rendering an animation. 
        /// </summary>
        public void Run(int updateSteps)
        {
            _currentProgress = 0;
            _master.Progress(_currentProgress);
            System.Diagnostics.Debug.WriteLine("PaintJob.Run " + updateSteps.ToString());
            _parameters = ParameterDict.Current.Clone();
            _updateSteps = updateSteps;
            if (_updateSteps == 0)
                _updateSteps = 1;
                _currentProgressd = 100.0 / (double)(_updateSteps);
            for (int i = 0; i < _updateSteps; i++)
            {               
                if (_abort)
                    return;
                if (_parameters["View.Renderer"] == "2d")
                {
                    _iterate = new Iterate2d(_parameters, this, false);
                }
                else
                _iterate = new Iterate(_parameters, this, false);
                if (_lastIterate != null)
                {
                    _iterate.SetOldData(_lastIterate.GraphicInfo,_lastIterate.PictureData,i);
                }
                if (_abort)
                    return;
                _iterate.StartAsync();
                _iterate.Wait();
                if (_abort)
                    return;
                _lastIterate = _iterate;
                _currentProgress += _currentProgressd;
                _master.Progress(_currentProgress);
            }
            if (_iterate ==null)
            {
            }
else
            {

            Renderer renderer = PictureArtFactory.Create(_iterate.PictureData, _iterate.LastUsedFormulas, ParameterDict.Current.Clone());
            renderer.Paint(_graphics);
            if (_abort)
                return;
            _master.Progress(0);
            }

        }


        /// <summary>
        /// Is called, if computation of mIterate ends.
        /// </summary>
        public void ComputationEnds()
        {
        }


        /// <summary>
        /// Show progress. Valid values are [0, ..., 100].
        /// </summary>
        public void Progress(double progressInPercent)
        {
            double progress = _currentProgress + progressInPercent / (double)(_updateSteps );
            System.Diagnostics.Debug.WriteLine("Progress: " + progressInPercent.ToString() + " =" + progress);
            if (progress > 100)
                progress = 100;
           _master.Progress(progress);
        }


    }
}
