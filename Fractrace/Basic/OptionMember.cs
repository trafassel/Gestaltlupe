namespace Fractrace.Basic
{

    /// <summary>
    /// Entry as 
    /// </summary>
    public class OptionMember
    {
        private string mName = "";
        private string mValue = "";


        public OptionMember(string name, string value)
        {
            this.mName = name;
            this.mValue = value;
        }


        /// <summary>
        /// Unique name of this entry.
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
        }


        /// <summary>
        /// Corresponding value as string.
        /// </summary>
        public string Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
            }

        }
    }
}