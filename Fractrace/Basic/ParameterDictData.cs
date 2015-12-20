using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic
{


    /// <summary>
    /// Managed a list of ParameterEntries. In Gestaltlupe this class is used for parameterHistory.
    /// </summary>
    public class ParameterDictData
    {

        /// <summary>
        /// Number of history entries.
        /// </summary>
        public int Time { get { return _history.Count - 1; } }

        /// <summary>
        /// Return parameter history at given time.
        /// </summary>
        public Dictionary<string, string> Get(int time) { return _history[time]; }
        /// <summary>Parameter History.</summary>
        List<Dictionary<string, string>> _history = new List<Dictionary<string, string>>();

        /// <summary>
        /// Used for lock other threads while Save().
        /// </summary>
        protected static object _refCountLock = new object();

        /// <summary>
        /// Save ParameterDict and return new time (as event count). 
        /// </summary>
        public int Save(string fileName)
        {
            lock (_refCountLock)
            {
                try
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> entry in ParameterDict.Current.Entries)
                    {
                        dict[entry.Key] = entry.Value;

                    }
                    if (fileName != "")
                        dict["Intern.FileName"] = fileName;
                    _history.Add(dict);
                    return Time;
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
            return 0;
        }


        /// <summary>
        /// Save ParameterDict and return new time (as event count). 
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public int Save()
        {
            return Save("");
        }


        /// <summary>
        /// Entry at position index is moved to the global ParameterDict instance.
        /// </summary>
        /// <param name="Läd"></param>
        public void Load(int index)
        {
            if (index < 0 | index > Time)
                throw new ArgumentException("Index out of range");
            foreach (KeyValuePair<string, string> entry in _history[index])
            {
                // Use the following, if no update events should be raised.
                // ParameterDict.Exemplar.SetValue(entry.Key, entry.Value, false);
                ParameterDict.Current[entry.Key] = entry.Value;
            }
        }


        /// <summary>
        /// Entry at position index is moved to the global ParameterDict instance.
        /// If index is no integer and the corresponding value is a number, the values at Round(index) and Round(index)+1 
        /// are weighted and combined. 
        /// </summary>
        /// <param name="Läd"></param>
        public void Load(double index)
        {
            foreach (KeyValuePair<string, string> entry in _history[(int)index])
            {
                double doubleVal = 0;
                string var = _history[(int)index][entry.Key];
                bool isDouble = true;
                if (!double.TryParse(var, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out doubleVal))
                {
                    if (!double.TryParse(var, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out doubleVal))
                    {
                        if (!double.TryParse(var, out doubleVal))
                            isDouble = false;
                    }
                }
                if (isDouble)
                {
                    double firstDouble = GetDouble((int)index, entry.Key);
                    double lastDouble = firstDouble;
                    if (index < Time)
                        lastDouble = GetDouble((int)index + 1, entry.Key);
                    double r = (index - (int)index);
                    ParameterDict.Current.SetDouble(entry.Key, (r * lastDouble + (1 - r) * firstDouble));
                }
                else
                { // use first entry
                    ParameterDict.Current[entry.Key] = entry.Value;
                }
            }
        }


        /// <summary>
        /// Entry at position index is moved to the global ParameterDict instance.
        /// If index is no integer and the corresponding value is a number, the value of a bezier courve at point index is returned.
        /// </summary>
        public void LoadSmoothed(double index)
        {
            foreach (KeyValuePair<string, string> entry in _history[(int)index])
            {
                double doubleVal = 0;
                string var = _history[(int)index][entry.Key];
                bool isDouble = true;
                if (!double.TryParse(var, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out doubleVal))
                {
                    if (!double.TryParse(var, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out doubleVal))
                    {
                        if (!double.TryParse(var, out doubleVal))
                            isDouble = false;
                    }
                }

                if (isDouble)
                {
                    int indexAsInt = (int)index;
                    double firstDouble = GetDouble(indexAsInt, entry.Key);
                    double firstfirstDouble = firstDouble;
                    if (indexAsInt > 0)
                        firstfirstDouble = GetDouble(indexAsInt - 1, entry.Key);
                    double lastDouble = firstDouble;
                    if (indexAsInt < Time)
                        lastDouble = GetDouble(indexAsInt + 1, entry.Key);
                    double lastlastDouble = lastDouble;
                    if (indexAsInt < Time - 1)
                        lastlastDouble = GetDouble(indexAsInt + 2, entry.Key);
                    double t = (index - indexAsInt);
                    double pdiff = lastDouble - firstDouble;
                    double p0 = firstDouble;
                    double p1 = firstDouble + ((firstDouble - firstfirstDouble) + pdiff) / 6.0;
                    double p2 = lastDouble - ((lastlastDouble - lastDouble) +pdiff) / 6.0;
                    double p3 = lastDouble;
                    double tm = 1 - t;
                    double tm2 = tm * tm;
                    double t2 = t * t;
                    double val = tm2 * tm * p0 + 3.0 * tm2 * t * p1 + 3.0 * tm * t2 * p2 + t2 * t * p3;
                    ParameterDict.Current.SetDouble(entry.Key, val);
                }
                else
                { // use first entry
                    ParameterDict.Current[entry.Key] = entry.Value;
                }
            }
        }


        /// <summary>
        /// Get the value of entry key at time index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected double GetDouble(int index, string key)
        {
            string valuesAsString = _history[index][key];
            double retVal = 0;
            if (!double.TryParse(valuesAsString, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out retVal))
            {
                if (!double.TryParse(valuesAsString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out retVal))
                {
                    double.TryParse(valuesAsString, out retVal);
                }
            }
            return retVal;
        }


    }
}
