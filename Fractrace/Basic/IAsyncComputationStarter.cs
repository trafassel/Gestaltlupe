

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
        void Progress(double progressInPercent);


    }
}
