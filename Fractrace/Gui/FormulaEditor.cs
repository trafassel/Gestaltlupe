using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;

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
            tbSource.Text = ParameterDict.Exemplar["Intern.Formula.Source"];
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSource_TextChanged(object sender, EventArgs e) {
            ParameterDict.Exemplar["Intern.Formula.Source"] = tbSource.Text;
        }


        /// <summary>
        /// Close error view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShrink_Click(object sender, EventArgs e) {
            panel1.Visible = false;
        }
    }
}
