using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Fractrace.Basic
{


    /// <summary>
    /// Parameters, corresponding the event ParameterDictChanged.
    /// </summary>
    public class ParameterDictChangedEventArgs
    {
        public ParameterDictChangedEventArgs(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name;
        public string Value;
    }


    /// <summary>
    /// An entry has changed.
    /// </summary>
    public delegate void ParameterDictChanged(object source, ParameterDictChangedEventArgs e);


    /// <summary>
    /// Contains all project data in a hirachical way.
    /// </summary>
    public class ParameterDict
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterDict"/> class.
        /// </summary>
        protected ParameterDict()
        {

        }

        /// <summary>
        /// Used by the singleton design pattern.
        /// </summary>
        protected static ParameterDict _exemplar = null;

        /// <summary>
        ///  Used by the singleton design pattern.
        /// </summary>
        protected static Object _lockVar = new Object();

        /// <summary>
        /// Short documentation of the entry (in UK-english).
        /// </summary>
        public Dictionary<string, string> Description { get { return _description; } }
        protected Dictionary<string, string> _description = new Dictionary<string, string>();

        /// <summary>
        /// Jeder Eintrag kann einer Kategorie angehören. 
        /// Verschiedene Kategorien werden z.B. bei der Parametereingabe unterschiedlich behandelt.
        /// Kategorien werden üblichweise vom Programm aus gesetzt.
        /// </summary>
        public Dictionary<string, string> Categories { get { return _categories; } }
        protected Dictionary<string, string> _categories = new Dictionary<string, string>();

        /// <summary>
        /// Used in parsing of double entries ("en-US").
        /// </summary>
        /// <value>The X3D culture.</value>
        public static System.Globalization.CultureInfo Culture { get { return mCulture; } }
        /// <summary>en-US culture is used to save the float and double values.</summary>
        protected static System.Globalization.CultureInfo mCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

        ///<summary>Raised, if the value of an entry has changed.</summary>
        public event ParameterDictChanged EventChanged = null;

        /// <summary>
        /// Public access to the internal dictionary.
        /// </summary>
        public Dictionary<string, string> Entries { get { return _entries; } }
        /// <summary>Internal dictionary.</summary>
        protected Dictionary<string, string> _entries = new Dictionary<string, string>();

        /// <summary>
        /// If true, no outgoing update events are raised.
        /// </summary>
        private bool _noUpdateEvents = false;

        /// <summary>
        /// Gets the unique static instance of this class.
        /// </summary>
        public static ParameterDict Current
        {
            get
            {
                lock (_lockVar)
                {
                    if (_exemplar == null)
                    {
                        _exemplar = new ParameterDict();
                    }
                }
                return _exemplar;
            }
        }


        /// <summary>
        /// Save all dictionary entries into a file.
        /// </summary>
        public void Save(string fileName)
        {
            XmlTextWriter tw = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            tw.Formatting = System.Xml.Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("ParameterDict");
            foreach (KeyValuePair<string, string> entry in SortedEntries)
            {
                if (!ParameterDict.IsAdditionalInfo(entry.Key) && !ParameterDict.IsUserSetting(entry.Key))
                {
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
        /// Save all dictionary entries of given category into a file.
        /// </summary>
        public void Save(string fileName, List<String> categoriesToSave)
        {
            XmlTextWriter tw = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);
            tw.Formatting = System.Xml.Formatting.Indented;
            tw.WriteStartDocument();
            tw.WriteStartElement("ParameterDict");
            foreach (KeyValuePair<string, string> entry in SortedEntries)
            {
                if (!ParameterDict.IsAdditionalInfo(entry.Key))
                {
                    bool fits = false;
                    foreach (string str in categoriesToSave)
                    {
                        if (entry.Key.StartsWith(str))
                        {
                            fits = true;
                            break;
                        }
                    }
                    if (fits)
                    {
                        tw.WriteStartElement("Entry");
                        tw.WriteAttributeString("Key", entry.Key);
                        tw.WriteAttributeString("Value", entry.Value);
                        tw.WriteEndElement();
                    }
                }
            }
            tw.WriteEndElement();
            tw.WriteEndDocument();
            tw.Close();
        }


        /// <summary>
        /// Load all dictionary entries from the given file.
        /// </summary>
        public void Load(string fileName)
        {
            _entries.Clear();
            _noUpdateEvents = true;
            GlobalParameters.SetGlobalParameters();
            Append(fileName);
            ParameterUpdater.Update();
            _noUpdateEvents = false;
        }


        /// <summary>
        /// Append all dictionary entries from the given file.
        /// </summary>
        public void Append(string fileName)
        {
            XmlDocument xdoc = new XmlDocument();
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }
            xdoc.Load(fileName);
            foreach (XmlNode xNode in xdoc)
            {
                if (xNode.Name == "ParameterDict")
                {
                    foreach (XmlNode entryNode in xNode)
                    {
                        if (entryNode.Name == "Entry")
                        {
                            string key = entryNode.Attributes.GetNamedItem("Key").Value;
                            string value = entryNode.Attributes.GetNamedItem("Value").Value;
                            lock (_entries)
                            {
                                _entries[key] = value;
                            }
                        }
                    }
                }
            }
        }


        string Category(string entryname)
        {
            int lastPPos = entryname.LastIndexOf('.');
            if (lastPPos == -1 )
                return entryname;
            return entryname.Substring(0,lastPPos);
        }

        string ParameterName(string entryname)
        {
            int lastPPos = entryname.LastIndexOf('.');
            if (lastPPos == -1)
                return entryname;
            return entryname.Substring(lastPPos+1);
        }

        public string CreateBulk(string[] parameters,string[] parametersToExclude)
        {
            
            Dictionary<string, bool> parameterFilter = new Dictionary<string, bool>();
            foreach(string str in parametersToExclude)
            {
                parameterFilter[str] = true;
            }
            
            StringBuilder bulkString = new StringBuilder();

            string oldCategory = "";
            foreach (KeyValuePair<string, string> entry in SortedEntries)
            {
                if (!IsAdditionalInfo(entry.Key) &&!parameterFilter.ContainsKey(entry.Key))
                {
                    bool useEntry = false;
                    foreach (string str in parameters)
                    {
                        if (entry.Key.StartsWith(str))
                        {
                            useEntry = true;
                            break;
                        }
                    }
                    if (useEntry)
                    {
                        string category = Category(entry.Key);
                        if (oldCategory != category)
                            bulkString.Append(category + ": ");
                        string parameterName = ParameterName(entry.Key);
                        bulkString.Append(parameterName + "=" + entry.Value + " ");
                        oldCategory = category;
                    }
                }
            }
            return bulkString.ToString();
        }


        /// <summary>
        /// The text 
        /// </summary>
        public void AppendFromText(string text)
        {
            lock (_entries)
            {
                text = text.Replace(System.Environment.NewLine, " ");
                text = text.Replace("\n", " ");

                if (text.StartsWith("<"))
                {
                    XmlDocument xdoc = new XmlDocument();
                    XmlNode xnode = xdoc.CreateNode(XmlNodeType.Element, "MainNode", "");
                    xnode.InnerXml = text;
                    foreach (XmlNode entryNode in xnode)
                    {
                        if (entryNode.Name == "Entry")
                        {
                            string key = entryNode.Attributes.GetNamedItem("Key").Value;
                            string value = entryNode.Attributes.GetNamedItem("Value").Value;
                            _entries[key] = value;
                        }
                    }
                }
                else
                {
                    string currentCategory = "";
                    string[] entries = text.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
                    foreach(string entry in entries)
                    {
                        if (entry.EndsWith(":"))
                            currentCategory = entry.Substring(0,entry.Length-1);
                        else
                        {
                            string[] split = entry.Split('=');
                            string entryName = currentCategory+"."+split[0];
                            string value = split[1];
                            _entries[entryName] = value;
                        }
                    }

                }
            }
        }


        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified name.
        /// </summary>
        public string this[string name]
        {
            get
            {
                return Current.GetValue(name);
            }
            set
            {
                Current.SetValue(name, value);
            }
        }


        /// <summary>
        /// Get the value of the entry with given name.
        /// </summary>
        protected string GetValue(string name)
        {
            if (_entries.ContainsKey(name))
                return _entries[name];
            return string.Empty;
        }


        public bool Exists(string name)
        {
            return _entries.ContainsKey(name);
        }

        /// <summary>
        /// Set the value of the entry with given name. 
        /// </summary>
        protected void SetValue(string name, string value)
        {
            lock (_entries)
            {
                _entries[name] = value;
            }
            if (EventChanged != null && !_noUpdateEvents)
                EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


        /// <summary>
        /// Set entry with the possibility not to raise a change event. 
        /// </summary>
        public void SetValue(string name, string value, bool raiseChangeEvent)
        {
            lock (_entries)
            {
                _entries[name] = value;
            }
            if (EventChanged != null && raiseChangeEvent)
                EventChanged(this, new ParameterDictChangedEventArgs(name, value));
        }


        /// <summary>
        /// Removes the Entry with the given name.
        /// </summary>
        public void RemoveProperty(string name)
        {
            if (_entries.ContainsKey(name))
            {
                _entries.Remove(name);
            }
        }


        /// <summary>
        ///  Public access to the internal dictionary (sorted).
        /// </summary>
        public SortedDictionary<string, string> SortedEntries
        {
            get
            {
                SortedDictionary<string, string> retVal = new SortedDictionary<string, string>();
                lock (_entries)
                {
                    try
                    {
                        foreach (KeyValuePair<string, string> entry in _entries)
                        {
                            try
                            {
                                retVal[entry.Key] = entry.Value;
                            }
                            catch { }
                        }
                    }
                    catch { } // this could cause problems later (enumeration has changed).
                }
                return retVal;
            }
        }


        /// <summary>
        /// Gets the Hash (elementName|elemenValue) of all elementes, which name starts with nodeHirarchy.
        /// </summary>
        public string GetHash(string nodeHirarchy)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in SortedEntries)
            {
                if (entry.Key.StartsWith(nodeHirarchy))
                {
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
            for (int i = 0; i < buffer2.Length; i++)
            {
                temp = Convert.ToString(buffer2[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                retVal.Append(temp);
            }
            return retVal.ToString();
        }


        /// <summary>
        /// Gets the Hash (elementName) of all elementNames, which name starts with nodeHirarchy.
        /// </summary>
        public string GetHashOfName(string nodeHirarchy)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in SortedEntries)
            {
                if (entry.Key.StartsWith(nodeHirarchy))
                {
                    sb.Append("|" + entry.Key + "|");
                }
            }
            string bigstr = sb.ToString();
            System.Security.Cryptography.MD5 sha = System.Security.Cryptography.MD5.Create();
            byte[] buffer = new byte[Encoding.ASCII.GetByteCount(bigstr)];
            buffer = Encoding.ASCII.GetBytes(bigstr);
            byte[] buffer2 = sha.ComputeHash(buffer);

            string temp = "";
            StringBuilder retVal = new StringBuilder();
            for (int i = 0; i < buffer2.Length; i++)
            {
                temp = Convert.ToString(buffer2[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                retVal.Append(temp);
            }
            return retVal.ToString();
        }


        /// <summary>
        /// Set double entry.
        /// </summary>
        public void SetDouble(string key, double value)
        {
            lock (_entries)
            {
                _entries[key] = value.ToString(Culture.NumberFormat);
            }
            EventChanged(this, new ParameterDictChangedEventArgs(key, value.ToString()));
        }


        /// <summary>
        /// Set double entry without raising change event.
        /// </summary>
        public void SetDoubleWithoutRaiseChange(string key, double value)
        {
            lock (_entries)
            {
                _entries[key] = value.ToString(Culture.NumberFormat);
            }
        }


        /// <summary>
        /// Get double entry.
        /// </summary>
        public double GetDouble(string key)
        {
            double retVal = 0;
            if (_entries.ContainsKey(key))
            {
                string var = _entries[key];
                if (!double.TryParse(var, System.Globalization.NumberStyles.Number, Culture.NumberFormat, out retVal))
                {
                    if (!double.TryParse(var, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out retVal))
                    {
                        System.Diagnostics.Debug.WriteLine("Error in GetDouble(" + key + ") can not convert " + var + " in double");
                        double.TryParse(var, out retVal);
                    }
                }
            }
            return retVal;
        }


        /// <summary>
        /// Get double entry. Set entry, if not exists.
        /// </summary>
        public double GetOrSetDouble(string key, double defaultValue = 0, string description = "", bool addDefaultButtons = false)
        {
            if (!_entries.ContainsKey(key))
            {
                ParameterDict.Current[key] = defaultValue.ToString(mCulture);
            }
            if (description != null && description != "")
            {
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.Description", description, false);
            }
            if (addDefaultButtons)
            {
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.FixedButtons", "0", false);
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.PlusButton", "0.1", false);
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.PlusPlusButton", "0.001", false);
            }
            return GetDouble(key);
        }

        /// <summary>
        /// Get bool entry. Set entry, if not exists.
        /// </summary>
        public bool GetOrSetBool(string key, bool defaultValue = false, string description = "", bool addDefaultButtons = false)
        {
            if (!_entries.ContainsKey(key))
            {
                ParameterDict.Current[key] = defaultValue.ToString(mCulture);
            }
            if (description != null && description != "")
            {
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.Description", description, false);
            }
            ParameterDict.Current.SetValue(key + ".PARAMETERINFO.Datatype", "Bool", false);
            if (addDefaultButtons)
            {
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.FixedButtons", "0", false);
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.PlusButton", "0.1", false);
                ParameterDict.Current.SetValue(key + ".PARAMETERINFO.VIEW.PlusPlusButton", "0.001", false);
            }
            return GetBool(key);
        }


        /// <summary>
        /// Return View.Size * View.Width.
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return (int)(GetDouble("View.Width") * GetDouble("View.Size"));
        }


        /// <summary>
        /// Return View.Size * View.Height.
        /// </summary>
        public int GetHeight()
        {
            return (int)(GetDouble("View.Height") * GetDouble("View.Size"));
        }


        /// <summary>
        /// Set integer entry.
        /// </summary>
        public void SetInt(string key, int value)
        {
            lock (_entries)
            {
                _entries[key] = value.ToString(Culture.NumberFormat);
            }
            EventChanged(this, new ParameterDictChangedEventArgs(key, value.ToString()));
        }


        /// <summary>
        /// Get integer entry.
        /// </summary>
        public int GetInt(string key)
        {
            int retVal = 0;
            if (_entries.ContainsKey(key))
            {
                if (!int.TryParse(_entries[key], System.Globalization.NumberStyles.Number, Culture.NumberFormat, out retVal))
                {
                    double dRetVal = 0;
                    try
                    {
                        if (double.TryParse(_entries[key], System.Globalization.NumberStyles.Number, Culture.NumberFormat, out dRetVal))
                        {
                            retVal = (int)dRetVal;
                        }
                    }
                    catch (System.Exception) { }
                }
            }
            return retVal;
        }


        /// <summary>
        /// Set boolean entry.
        /// </summary>
        public void SetBool(string key, bool value)
        {
            lock (_entries)
            {
                if (value)
                    _entries[key] = "1";
                else
                    _entries[key] = "0";
            }
            EventChanged(this, new ParameterDictChangedEventArgs(key, _entries[key]));
        }


        /// <summary>
        /// Get boolean entry.
        /// </summary>
        public bool GetBool(string key)
        {
            if (_entries.ContainsKey(key))
            {
                string valueAsString = _entries[key];
                if (valueAsString == "1" || valueAsString.ToLower() == "true")
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Return a clone of this instance.
        /// </summary>
        public ParameterDict Clone()
        {
            ParameterDict retVal = new ParameterDict();
            foreach (KeyValuePair<string, string> entry in _entries)
            {
                retVal._entries[entry.Key] = entry.Value;
            }
            return retVal;
        }


        /// <summary>
        /// Return Datatype of given parameter. If Datatype is unknown an empty string is returned.
        /// </summary>
        public string GetDatatype(string parameterName)
        {
            return GetValue(parameterName + ".PARAMETERINFO.Datatype");
        }


        /// <summary>
        /// Return description of given parameter.
        /// </summary>
        public string GetDescription(string parameterName)
        {
            return GetValue(parameterName + ".PARAMETERINFO.Description");
        }


        /// <summary>
        /// Return true, if corresponding parameter entry is for info only and should not generate any user control.
        /// </summary>
        public static bool IsAdditionalInfo(string parameterName)
        {
            if (parameterName.Contains(".PARAMETERINFO"))
                return true;
            return false;
        }


        /// <summary>
        /// Return true, if corresponding parameter entry is user setting and does not belongs to .
        /// </summary>
        public static bool IsUserSetting(string parameterName)
        {
            if (parameterName=="Intern.Filter")
                return true;
            if (parameterName == "Computation.NoOfThreads")
                return true;
            if (parameterName == "View.PosterX")
                return true;
            if (parameterName == "View.PosterZ")
                return true;
            if (parameterName == "View.Raster")
                return true;
          //  if (parameterName == "View.Size")
         //       return true;
            if (parameterName == "View.Pipeline.Preview")
                return true;
            if (parameterName == "View.Pipeline.UpdatePreview")
                return true;
            if (parameterName == "Animation.Steps")
                return true;
            if (parameterName.StartsWith("Export."))
                return true;
            return false;
        }


        /// <summary>
        /// Return true, if the corresponding control should be generated.
        /// </summary>
        public static bool HasControl(string parameterName)
        {
            return !ParameterDict.Current.GetBool(parameterName + ".PARAMETERINFO.VIEW.Invisible");
        }


    }
}
