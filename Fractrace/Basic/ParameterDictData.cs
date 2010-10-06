using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic {
    
    public class ParameterDictData {


        /// <summary>
        /// Speichert den aktuellen Inhalt des ParameterDicts und liefert den Referenzzähler. 
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
              refCount++;
              mHistory.Add(dict);
              return Time;
            } catch (System.Exception ex) {
              System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
          }
          return 0;
        }

        System.Collections.Generic.List<Dictionary<string,string>> mHistory = new List<Dictionary<string,string>>();

   
        /// <summary>
        /// Überträgt den Inhalt des Eintrages index der Liste in die Programm-Registry 
        /// </summary>
        /// <param name="Läd"></param>
        public void Load(int index) {
            if (index < 0 | index > Time)
                throw new ArgumentException("Index out of range");
            foreach (KeyValuePair<string, string> entry in mHistory[index]) {
                ParameterDict.Exemplar[entry.Key] = entry.Value;
            }
        }


        /// <summary>
        /// Liefert die Anzahl der gespeicherten History-Einträge.
        /// </summary>
        public int Time {
            get {
                return mHistory.Count-1;
            }
    }


        /// <summary>
        /// Überträgt den Inhalt des Eintrages index der Liste in die Programm-Registry. Wird keine ganze
        /// Zahl angegeben, wird der gewichtete Mittelwert zwischen index und index-1 ausgegeben.
        /// </summary>
        /// <param name="Läd"></param>
        public void Load(double index) {
            foreach (KeyValuePair<string, string> entry in mHistory[(int)index]) {
                double doubleVal = 0;
                if (double.TryParse(mHistory[(int)index][entry.Key],out doubleVal)) {
                    double firstDouble = GetDouble((int)index,entry.Key);
                    double lastDouble = firstDouble;
                    if(index<Time)
                        lastDouble = GetDouble((int)index + 1,entry.Key);
                    double r=(index-(int)index);
                    ParameterDict.Exemplar.SetDouble(entry.Key, (r * lastDouble + (1 - r) * firstDouble));

                } else { // den ersten Eintrag liefern
                    ParameterDict.Exemplar[entry.Key] = entry.Value;
                }

            }
        }


        /// <summary>
        /// Liefert den Zahlenwert key zum Zeitpunkt index
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





        protected static object refCountLock=new object();

        protected static int refCount = 0;

    }

}
