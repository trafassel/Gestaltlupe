namespace Fractrace.Basic
{

    /// <summary>
    /// A Parameterdict Entry. 
    /// </summary>
    public class OptionMember
    {


        public OptionMember(string name, string value)
        {
            this._name = name;
            this._value = value;
        }


        /// <summary>
        /// Unique name of this entry.
        /// </summary>
        public string Name { get { return _name; } }
        private string _name = "";

        /// <summary>
        /// Corresponding value as string.
        /// </summary>
        public string Value { get { return _value; } set { _value = value; } }
        private string _value = "";


    }
}
