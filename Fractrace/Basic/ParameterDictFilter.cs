using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.Basic
{

    /// <summary>
    /// Filter full set of parameters.
    /// </summary>
    public class ParameterDictFilter
    {

        public ParameterDictFilter()
        {
        }


        /// <summary>
        /// Saved dict which will be activated after using this filter.
        /// </summary>
        Dictionary<string, string> _savedDict = new Dictionary<string, string>();


        /// <summary>
        /// Contails all changed parameters in filter.
        /// </summary>
        Dictionary<string, bool> _changedParameters = new Dictionary<string, bool>();


        /// <summary>
        /// Current values are saved and filter is applied to current values.
        /// </summary>
        public void Apply()
        {
            try
            {
                foreach (KeyValuePair<string, string> entry in ParameterDict.Current.Entries)
                {
                    _savedDict[entry.Key] = entry.Value;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ParameterDict.Exemplar.Entries are updated while saving. Filter is not applyed.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                _savedDict.Clear();
                return;
            }
            Filter();
            foreach (KeyValuePair<string, string> entry in ParameterDict.Current.Entries)
            {
                if (_savedDict[entry.Key] != entry.Value)
                    _changedParameters[entry.Key] = true;
            }
        }


        /// <summary>
        /// Apply filter to current values.
        /// </summary>
        protected virtual void Filter()
        {

        }


        /// <summary>
        /// All saved values are restored.
        /// </summary>
        public void Restore()
        {
            foreach (KeyValuePair<string, string> entry in _savedDict)
            {
                if( _changedParameters.ContainsKey(entry.Key) )
                  ParameterDict.Current.SetValue(entry.Key, entry.Value, false);
            }
            ParameterDict.Current.SetValue("Intern.Filter", "", false);
        }


    }
}
