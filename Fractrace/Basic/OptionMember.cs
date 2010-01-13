namespace Fractrace.Basic {

    public class OptionMember {
        private string mName = "";
        private string mValue = "";


        public OptionMember(string name, string value) {
            this.mName = name;
            this.mValue = value;
        }


        /// <summary>
        /// Ein eindeutiger Name
        /// </summary>
        public string Name {
            get {
                return mName;
            }
        }

        /// <summary>
        /// Wert
        /// </summary>
        public string Value {
            get {
                return mValue;
            }
            set {
                mValue = value;
            }

        }
    }
}