using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
//using System.Data;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;
using System.IO;

namespace Fractrace {
    public partial class FormulaEditor : UserControl {
        
        
        /// <summary>
        /// Constructer.
        /// </summary>
        public FormulaEditor() {
            InitializeComponent();
            Init();
            mStaticInstance = this;
            panel1.Visible = false;
        }


        /// <summary>
        /// Global instance of this class.
        /// </summary>
        private static FormulaEditor mStaticInstance=null;


        /// <summary>
        /// Fehler beim Übersetzen des Quellcodes wird angezeigt.
        /// </summary>
        /// <param name="errorText"></param>
        public static void AddError(string errorText,int line,int column) {
            if (FormulaEditor.mStaticInstance != null) {
                FormulaEditor.mStaticInstance.ViewError(errorText);
                FormulaEditor.mStaticInstance.SelectLine(line-9);
            }
        }


        /// <summary>
        /// Initialize text.
        /// </summary>
        public void Init() {
            tbSource.Text = ParameterDict.Current["Intern.Formula.Source"];
        }


        protected string mErrorText = "";


        /// <summary>
        /// Show error as text in panel1.
        /// </summary>
        protected void ViewErrorInternal() {
            panel1.Visible = true;
            FormulaEditor.mStaticInstance.tbErrors.Text = mErrorText;
        }


        /// <summary>
        ///Show error as text and mark error line red.
        /// </summary>
        protected void ViewError(string errorText) {
            mErrorText = errorText;
            this.Invoke(new ShowErrorDelegate(ViewErrorInternal));
        }


        /// <summary>
        /// Given line is marked red.
        /// </summary>
        /// <param name="line"></param>
        protected void SelectLine(int line) {
            if (this.InvokeRequired) {
                this.Invoke(new SelectLineDelegate(SelectLine),line);
                return;
            }

            // find the position of the line in the text.
            int startpos=0, endpos=0;
            int currentpos=0;
            int currentline=0;
            foreach(char c in tbSource.Text.ToCharArray()) {
                currentpos++;
                if(c=='\n') {
                   currentline++;
                    if(currentline==line) {
                        startpos=currentpos;
                    }
                    if(currentline==line+1) {
                        endpos=currentpos;
                    }
                }
            }
            if (startpos < endpos && endpos < tbSource.Text.Length) {
                this.tbSource.Select(startpos, endpos - startpos);
                this.tbSource.SelectionColor = Color.Red;
            }
        }


        delegate void ShowErrorDelegate();

        delegate void SelectLineDelegate(int line);


        /// <summary>
        /// Is called, each time, the text has changed. Set "Intern.Formula.Source to source text.
        /// </summary>
        private void tbSource_TextChanged(object sender, EventArgs e) {
            ParameterDict.Current["Intern.Formula.Source"] = tbSource.Text;
        }


        /// <summary>
        /// Close error view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShrink_Click(object sender, EventArgs e) {
            panel1.Visible = false;
        }


        /// <summary>
        /// Format 
        /// </summary>
        public void Format()
        {
            string inputText = tbSource.Text;
            inputText=inputText.Replace("{", System.Environment.NewLine + "{" + System.Environment.NewLine);
            inputText = inputText.Replace("}", System.Environment.NewLine + "}" + System.Environment.NewLine);
            inputText = inputText.Replace(";", ";" + System.Environment.NewLine);
            inputText = inputText.Replace(System.Environment.NewLine+";", ";");

            StringBuilder formatedSource = new StringBuilder();
            using (StringReader sr = new StringReader(inputText))
            {
                int indent = 0;
                string line;
                // dont use newline while in for(int i=0;i...)
                bool inFor = false;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (inFor && line.Contains(")"))
                        inFor = false;

                    if (line.StartsWith("for"))
                        inFor = true;
                    if (line.Contains("}"))
                        indent--;
                    if (line != "")
                    {
                        if (inFor)
                        {
                            if (line.Contains("for"))
                                formatedSource.Append(Indent(indent));
                            formatedSource.Append(line);
                        }
                        else
                        {
                            formatedSource.AppendLine(Indent(indent) + line);
                        }
                    }
                    if (line.Contains("{"))
                        indent++;
                }
            }
            tbSource.Text = formatedSource.ToString();
        }

        string Indent(int indent)
        {
            StringBuilder retVal = new StringBuilder();
            for (int i = 0; i < indent; i++)
                retVal.Append("  ");
            return retVal.ToString();
        }


    }
}
