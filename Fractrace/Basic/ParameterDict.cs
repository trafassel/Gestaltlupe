using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Fractrace.Basic {


    public class ParameterDictChangedEventArgs {
        public ParameterDictChangedEventArgs(string name, string value) {
            Name = name;
            Value = value;
        }
        public string Name;
        public string Value;
    }


    public delegate void ParameterDictChanged(object source, ParameterDictChangedEventArgs e);

    /// <summary>
    /// Zur Verwaltung globaler Variablen.
    /// TODO: Speichern und Laden
    /// </summary>
    class ParameterDict {

        protected static ParameterDict mExemplar = null;

        protected static Object lockVar = new Object();


        ParameterDict() {

        }


        /// TODO: 
        /// Synchronisieren von zwei Abbildungen, 
        /// PUSH: Der gesamte Parameterinhalt 
        /// wird in eine Zwischenablade Datei geschrieben und 
        /// PULL: Sämtliche Parameter werden von den Daten der Zwischenablage-Datei
        /// (Daten müssen nicht vollständig sein, man kan einen Ast angeben,
        /// von dem ab die Daten gelesen werden), gelesen.
        /// 
        public static ParameterDict Exemplar {
            get {
                lock (lockVar) {
                    if (mExemplar == null) {
                        mExemplar = new ParameterDict();
                    }
                }
                return mExemplar;
            }
        }


        protected static System.Globalization.CultureInfo mCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        /// <summary>
        /// Liefert, die CultureInfo, die zum Parsen von Double-Einträgen verlangt wird ("en-US").
        /// </summary>
        /// <value>The X3D culture.</value>
        public static System.Globalization.CultureInfo Culture {
            get {
                return mCulture;
            }
        }


        /// <summary>
        /// Der Inhalt wird in eine Datei geschrieben.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName) {
            XmlTextWriter tw = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            tw.Formatting = System.Xml.Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("ParameterDict");
            foreach (KeyValuePair<string, string> entry in SortedEntries) {
                tw.WriteStartElement("Entry");
                tw.WriteAttributeString("Key", entry.Key);
                tw.WriteAttributeString("Value", entry.Value);
                tw.WriteEndElement();
            }

            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Close();

        }


      /// <summary>
      /// Projektdaten werden geladen.
      /// </summary>
      /// <param name="fileName"></param>
        public void Load(string fileName) {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fileName);
            foreach (XmlNode xNode in xdoc) {
                if (xNode.Name == "ParameterDict") {
                    foreach (XmlNode entryNode in xNode) {
                        if (entryNode.Name == "Entry") {
                            string key = entryNode.Attributes.GetNamedItem("Key").Value;
                            string value = entryNode.Attributes.GetNamedItem("Value").Value;
                            lock (mEntries) {
                              mEntries[key] = value;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified name.
        /// </summary>
        /// <value></value>
        public string this[string name] {
            get {
                return Exemplar.GetValue(name);
            }
            set {
                Exemplar.SetValue(name, value);
            }

        }


      /// <summary>
      /// Liefert die durch name spezifizierte Eigenschaft.
      /// </summary>
      /// <param name="name"></param>
      /// <returns></returns>
        protected string GetValue(string name) {
            if (mEntries.ContainsKey(name))
                return mEntries[name];
            return string.Empty;
        }



      /// <summary>
      /// Der Eigenschaft mit der angegeben Bezeichnung wird ein Wert zugewiesen. 
      /// </summary>
      /// <param name="name"></param>
      /// <param name="value"></param>
        protected void SetValue(string name,string value) {
          lock (mEntries) {
            mEntries[name] = value;
          }
            if(EventChanged!=null)
            EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


      /// <summary>
      /// Hier kann ein Eintrsg geändert werden, ohne einen Change-Event auszulösen. 
      /// </summary>
      /// <param name="name"></param>
      /// <param name="value"></param>
      /// <param name="raiseChangeEvent"></param>
        public void SetValue(string name, string value,bool raiseChangeEvent) {
          lock (mEntries) {
            mEntries[name] = value;
          }
          if (EventChanged != null && raiseChangeEvent)
            EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


        /// <summary>
        /// Öffentlicher Zugriff auf das interne Dictionary.
        /// </summary>
        public Dictionary<string, string> Entries {
            get {
                return mEntries;
            }
        }


        /// <summary>
        /// Öffentlicher Zugriff auf das interne Dictionary.
        /// </summary>
        public SortedDictionary<string, string> SortedEntries {
            get {
                SortedDictionary<string, string> retVal = new SortedDictionary<string, string>();
                lock (mEntries) {
                  foreach (KeyValuePair<string, string> entry in mEntries) {
                    retVal[entry.Key] = entry.Value;
                  }
                }
                return retVal;
            }
        }


        /// <summary>
        /// Setzt den Eintrag als Double
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetDouble(string key, double value) {
          lock (mEntries) {
            mEntries[key] = value.ToString(Culture.NumberFormat);
          }
            EventChanged(this, new ParameterDictChangedEventArgs(key, value.ToString()));
        }


        /// <summary>
        /// Liefert den Eintrag als Double
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(string key) {
            double retVal = 0;
            if(mEntries.ContainsKey(key)) {
                double.TryParse(mEntries[key],System.Globalization.NumberStyles.Number,Culture.NumberFormat, out retVal);
            }
           return retVal;
        }


        /// <summary>
        /// Setzt den Eintrag als Integer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetInt(string key, int value) {
          lock (mEntries) {
            mEntries[key] = value.ToString(Culture.NumberFormat);
          }
            EventChanged(this, new ParameterDictChangedEventArgs(key, value.ToString()));
        }


        /// <summary>
        /// Liefert den Eintrag als Integer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key) {
            int retVal = 0;
            if (mEntries.ContainsKey(key)) {
                int.TryParse(mEntries[key], System.Globalization.NumberStyles.Number, Culture.NumberFormat, out retVal);
            }
            return retVal;
        }

        /// <summary>
        /// Setzt den Eintrag als Integer
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBool(string key, bool value)
        {
          lock (mEntries) {
            if (value)
              mEntries[key] = "1";
            else
              mEntries[key] = "0";
          }
            EventChanged(this, new ParameterDictChangedEventArgs(key, mEntries[key]));
        }


        /// <summary>
        /// Liefert den Eintrag als Integer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBool(string key)
        {
            if (mEntries.ContainsKey(key))
            {
                string valueAsString = mEntries[key];
                if (valueAsString == "1" || valueAsString.ToLower() == "true")
                    return true;

            }
            return false;
        }

        protected Dictionary<string, string> mEntries = new Dictionary<string,string>();


        public Dictionary<string, string> Description {
            get {
                return mDescription;
            }
        }
        protected Dictionary<string, string> mDescription = new Dictionary<string, string>();


        /// <summary>
        /// Jeder Eintrag kann einer Kategorie angehören. 
        /// Verschiedene Kategorien werden z.B. bei der Parametereingabe unterschiedlich behandelt.
        /// Kategorien werden üblichweise vom Programm aus gesetzt.
        /// </summary>
        public Dictionary<string, string> Categories {
            get {
                return mCategories;
            }
        }
        protected Dictionary<string, string> mCategories = new Dictionary<string, string>();


        ///<summary>Tritt auf, wenn ein Eintrag geändert wurde.</summary>
        public event ParameterDictChanged EventChanged=null;


    }
}
