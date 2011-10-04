using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Fractrace.Basic {


    /// <summary>
    /// Parameters, corresponding the event ParameterDictChanged.
    /// </summary>
    public class ParameterDictChangedEventArgs {
        public ParameterDictChangedEventArgs(string name, string value) {
            Name = name;
            Value = value;
        }
        public string Name;
        public string Value;
    }


    /// <summary>
    /// An entry has changed ...
    /// </summary>
    public delegate void ParameterDictChanged(object source, ParameterDictChangedEventArgs e);


    /// <summary>
    /// Contains all project data in a hirachical way.
    /// </summary>
    class ParameterDict {

        /// <summary>
        /// Used by the singleton design pattern.
        /// </summary>
        protected static ParameterDict mExemplar = null;


        /// <summary>
        ///  Used by the singleton design pattern.
        /// </summary>
        protected static Object lockVar = new Object();


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDict"/> class.
        /// </summary>
        ParameterDict() {

        }


        /// <summary>
        /// Gets the unique static instance of this class.
        /// </summary>
        /// <value>The exemplar.</value>
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


        /// <summary>
        /// The en-US culture is used to save the float values. 
        /// </summary>
        protected static System.Globalization.CultureInfo mCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");


        /// <summary>
        /// Used in parsing of double entries ("en-US").
        /// </summary>
        /// <value>The X3D culture.</value>
        public static System.Globalization.CultureInfo Culture {
            get {
                return mCulture;
            }
        }


        /// <summary>
        /// Save all dictionary entries into a file.
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
        /// Save all dictionary entries of one of given category into a file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName, List<String> categoriesToSave) {
            XmlTextWriter tw = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            tw.Formatting = System.Xml.Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("ParameterDict");
            foreach (KeyValuePair<string, string> entry in SortedEntries) {
                bool fits = false;
                foreach (string str in categoriesToSave) {
                    if (entry.Key.StartsWith(str)) {
                        fits = true;
                        break;
                    }
                }
                if (fits) {
                    tw.WriteStartElement("Entry");
                    tw.WriteAttributeString("Key", entry.Key);
                    tw.WriteAttributeString("Value", entry.Value);
                    tw.WriteEndElement();
                }
            }
            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Close();
        }

        /// <summary>
        /// Load all dictionary entries from the given file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName) {
            mEntries.Clear();
            GlobalParameters.SetGlobalParameters();
            Append(fileName);
        }


        /// <summary>
        /// Append all dictionary entries from the given file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Append(string fileName) {
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
        /// The text 
        /// </summary>
        /// <param name="text">The text.</param>
        public void AppendFromText(string text) {
            lock (mEntries) {
                XmlDocument xdoc = new XmlDocument();
                XmlNode xnode = xdoc.CreateNode(XmlNodeType.Element, "MainNode", "");
                xnode.InnerXml = text;
                foreach (XmlNode entryNode in xnode) {
                    if (entryNode.Name == "Entry") {
                        string key = entryNode.Attributes.GetNamedItem("Key").Value;
                        string value = entryNode.Attributes.GetNamedItem("Value").Value;
                        mEntries[key] = value;
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
        /// Get the value of the entry with given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string GetValue(string name) {
            if (mEntries.ContainsKey(name))
                return mEntries[name];
            return string.Empty;
        }


        /// <summary>
        /// Set the value of the entry with given name. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void SetValue(string name, string value) {
            lock (mEntries) {
                mEntries[name] = value;
            }
            if (EventChanged != null)
                EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


        /// <summary>
        /// Set entry with the possibility not to raise a change event. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="raiseChangeEvent"></param>
        public void SetValue(string name, string value, bool raiseChangeEvent) {
            lock (mEntries) {
                mEntries[name] = value;
            }
            if (EventChanged != null && raiseChangeEvent)
                EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


        /// <summary>
        /// Removes the Entry with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveProperty(string name) {
            if (mEntries.ContainsKey(name)) {
                mEntries.Remove(name);
            }
        }


        /// <summary>
        /// Public access to the internal dictionary.
        /// </summary>
        public Dictionary<string, string> Entries {
            get {
                return mEntries;
            }
        }


        /// <summary>
        ///  Public access to the internal dictionary (sorted).
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
        /// Gets the Hash (elementName|elemenValue) of all elementes, which name starts with nodeHirarchy.
        /// </summary>
        /// <param name="nodeHirarchy">The node hirarchy.</param>
        /// <returns></returns>
        public string GetHash(string nodeHirarchy) {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in SortedEntries) {
                if (entry.Key.StartsWith(nodeHirarchy)) {
                    sb.Append("|" + entry.Key + "|" + entry.Value + "|");
                }
            }
            string bigstr = sb.ToString();
            System.Security.Cryptography.MD5 sha = System.Security.Cryptography.MD5.Create();
            byte[] buffer = new byte[Encoding.ASCII.GetByteCount(bigstr)];
            buffer = Encoding.ASCII.GetBytes(bigstr);
            byte[] buffer2 = sha.ComputeHash(buffer);

            string temp = "";
            StringBuilder retVal = new StringBuilder();
            for (int i = 0; i < buffer2.Length; i++) {
                temp = Convert.ToString(buffer2[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                retVal.Append(temp);
            }
            return retVal.ToString();
            //  return Encoding.ASCII.GetString(buffer2);
        }


        /// <summary>
        /// Gets the Hash (elementName) of all elementNames, which name starts with nodeHirarchy.
        /// </summary>
        /// <param name="nodeHirarchy">The node hirarchy.</param>
        /// <returns></returns>
        public string GetHashOfName(string nodeHirarchy) {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in SortedEntries) {
                if (entry.Key.StartsWith(nodeHirarchy)) {
                    sb.Append("|" + entry.Key + "|");
                }
            }
            string bigstr = sb.ToString();
            System.Security.Cryptography.MD5 sha = System.Security.Cryptography.MD5.Create();
            byte[] buffer = new byte[Encoding.ASCII.GetByteCount(bigstr)];
            buffer = Encoding.ASCII.GetBytes(bigstr);
            byte[] buffer2 = sha.ComputeHash(buffer);
            return Encoding.ASCII.GetString(buffer2);
        }





        /// <summary>
        /// Set double entry.
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
        /// Get double entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDouble(string key) {
            double retVal = 0;
            if (mEntries.ContainsKey(key)) {
                string var = mEntries[key];
                if (!double.TryParse(var, System.Globalization.NumberStyles.Number, Culture.NumberFormat, out retVal)) {
                    if (!double.TryParse(var, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out retVal)) {
                        System.Diagnostics.Debug.WriteLine("Error in GetDouble(" + key + ") can not convert " + var + " in double");
                            double.TryParse(var, out retVal);
                    }
                }
            }
            return retVal;
        }



        /// <summary>
        /// Set integer entry.
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
        /// Get integer entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetInt(string key) {
            int retVal = 0;
            if (mEntries.ContainsKey(key)) {
                if (!int.TryParse(mEntries[key], System.Globalization.NumberStyles.Number, Culture.NumberFormat, out retVal)) {
                    double dRetVal = 0;
                    try {
                    if(double.TryParse(mEntries[key], System.Globalization.NumberStyles.Number, Culture.NumberFormat, out dRetVal)) {
                           retVal=(int)dRetVal;
                    }
                    } catch (System.Exception) { }
                }
            } 
            return retVal;
        }


        /// <summary>
        /// Set boolean entry.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBool(string key, bool value) {
            lock (mEntries) {
                if (value)
                    mEntries[key] = "1";
                else
                    mEntries[key] = "0";
            }
            EventChanged(this, new ParameterDictChangedEventArgs(key, mEntries[key]));
        }


        /// <summary>
        /// Get boolean entry.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBool(string key) {
            if (mEntries.ContainsKey(key)) {
                string valueAsString = mEntries[key];
                if (valueAsString == "1" || valueAsString.ToLower() == "true")
                    return true;

            }
            return false;
        }


        /// <summary>
        /// Internal dictionary.
        /// </summary>
        protected Dictionary<string, string> mEntries = new Dictionary<string, string>();


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
        public event ParameterDictChanged EventChanged = null;


    }
}
