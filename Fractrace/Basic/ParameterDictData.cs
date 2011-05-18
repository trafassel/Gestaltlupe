﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic {


  /// <summary>
  /// Managed a list of ParameterEntries. In Gestaltlupe this class is used for parameterHistory.
  /// </summary>
  public class ParameterDictData {


    /// <summary>
    /// Save ParameterDict and return new time (as event count). 
    /// </summary>
    /// <param name="dict"></param>
    /// <returns></returns>
    public int Save() {
      lock (refCountLock) {
        try {
          Dictionary<string, string> dict = new Dictionary<string, string>();
          foreach (KeyValuePair<string, string> entry in ParameterDict.Exemplar.Entries) {
            dict[entry.Key] = entry.Value;
          }
          mHistory.Add(dict);
          return Time;
        }
        catch (System.Exception ex) {
          System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
      }
      return 0;
    }


    /// <summary>
    /// Parameter History.
    /// </summary>
    System.Collections.Generic.List<Dictionary<string, string>> mHistory = new List<Dictionary<string, string>>();


    /// <summary>
    /// Entry at position index is moved to the global ParameterDict instance.
    /// </summary>
    /// <param name="Läd"></param>
    public void Load(int index) {
      if (index < 0 | index > Time)
        throw new ArgumentException("Index out of range");
      foreach (KeyValuePair<string, string> entry in mHistory[index]) {
        // Use the following, if no update events should be raised.
        // ParameterDict.Exemplar.SetValue(entry.Key, entry.Value, false);
        ParameterDict.Exemplar[entry.Key] = entry.Value;
      }
    }


    /// <summary>
    /// Number of history entries.
    /// </summary>
    public int Time {
      get {
        return mHistory.Count - 1;
      }
    }


    /// <summary>
    /// Entry at position index is moved to the global ParameterDict instance.
    /// If index is no integer and the corresponding value is a number, the values at Round(index) and Round(index)+1 
    /// are weighted and combined. 
    /// </summary>
    /// <param name="Läd"></param>
    public void Load(double index) {
      foreach (KeyValuePair<string, string> entry in mHistory[(int)index]) {
        double doubleVal = 0;
        if (double.TryParse(mHistory[(int)index][entry.Key], out doubleVal)) {
          double firstDouble = GetDouble((int)index, entry.Key);
          double lastDouble = firstDouble;
          if (index < Time)
            lastDouble = GetDouble((int)index + 1, entry.Key);
          double r = (index - (int)index);
          ParameterDict.Exemplar.SetDouble(entry.Key, (r * lastDouble + (1 - r) * firstDouble));
        } else { // use first entry
          ParameterDict.Exemplar[entry.Key] = entry.Value;
        }
      }
    }


    /// <summary>
    /// Get the value of entry key at time index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    protected double GetDouble(int index, string key) {
      string valuesAsString = mHistory[index][key];
      double retVal = 0;
      double.TryParse(valuesAsString, System.Globalization.NumberStyles.Number, ParameterDict.Culture.NumberFormat, out retVal);
      return retVal;
    }


    /// <summary>
    /// Used for lock other threads while Save().
    /// </summary>
    protected static object refCountLock = new object();



  }

}
