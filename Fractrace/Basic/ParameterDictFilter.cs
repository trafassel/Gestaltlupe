using System.Collections.Generic;

namespace Fractrace.Basic
{

    /// <summary>
    /// Filter full set of parameters.
    /// </summary>
    public class ParameterDictFilter
    {

        protected ParameterDict _dict = null;
        public ParameterDictFilter()
        {
            _dict = ParameterDict.Current.Clone();
        }


        /// <summary>
        /// Apply filter to current values.
        /// </summary>
        public virtual ParameterDict Apply()
        {
            return _dict;
        }


    }
}
