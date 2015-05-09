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
        Dictionary<string, string> mSavedDict = new Dictionary<string, string>();


        /// <summary>
        /// Current values are saved and filter is applied to current values.
        /// </summary>
        public void Apply()
        {
            try
            {
                foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.Entries)
                {
                    mSavedDict[entry.Key] = entry.Value;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ParameterDict.Exemplar.Entries are updated while saving. Filter is not applyed.");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                mSavedDict.Clear();
                return;
            }
            Filter();

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
            foreach (KeyValuePair<string, string> entry in mSavedDict)
            {
                ParameterDict.Exemplar.SetValue(entry.Key, entry.Value, false);
            }
            ParameterDict.Exemplar.SetValue("Intern.Filter", "", false);

        }


    }
}
