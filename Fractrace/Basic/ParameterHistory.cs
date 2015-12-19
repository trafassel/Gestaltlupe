namespace Fractrace.Basic
{


    /// <summary>
    /// Managed a list of all parameter changes.
    /// </summary>
    public class ParameterHistory : ParameterDictData
    {


        /// <summary>
        /// Current time as event count.
        /// </summary>
        public int CurrentTime { get { return _currentTime; } set { _currentTime = value; } }
        /// <summary> Protected access to CurrentTime.</summary>
        protected int _currentTime = 0;

    }

}
