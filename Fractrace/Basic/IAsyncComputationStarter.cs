using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic
{


    /// <summary>
    /// Classes, which are able to use asynchron computations can use this interface to get an event if the computation ends.
    /// </summary>
    public interface IAsyncComputationStarter
    {


        /// <summary>
        /// Is called if computation is ready.
        /// </summary>
        void ComputationEnds();


        /// <summary>
        /// Show progress. Valid values are [0, ..., 100].
        /// </summary>
        /// <param name="progrssInPercent"></param>
        void Progress(double progressInPercent);


    }
}
