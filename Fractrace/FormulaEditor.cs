using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Fractrace.Basic;

namespace Fractrace {
    public partial class FormulaEditor : UserControl {
        
        
        /// <summary>
        /// 
        /// </summary>
        public FormulaEditor() {
            InitializeComponent();
            Init();
            mStaticInstance = this;
            panel1.Visible = false;
        }



        private static FormulaEditor mStaticInstance=null;

        /// <summary>
        /// Fehler beim Übersetzen des Quellcodes wird angezeigt.
        /// </summary>
        /// <param name="errorText"></param>
        public static void AddError(string errorText) {
            if(FormulaEditor.mStaticInstance!=null)
                FormulaEditor.mStaticInstance.ViewError(errorText);
        }



        public void Init() {
            tbSource.Text = ParameterDict.Exemplar["Intern.Formula.Source"];
        }


        protected string mErrorText = "";


        protected void ViewErrorInternal() {
            panel1.Visible = true;
            FormulaEditor.mStaticInstance.tbErrors.Text = mErrorText;
        
        }


        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        protected void ViewError(string errorText) {
            mErrorText = errorText;
            this.Invoke(new ShowErrorDelegate(ViewErrorInternal));
            //OneStepEnds();
        }


        delegate void ShowErrorDelegate();


        /// <summary>
        /// Der Text hat sich geändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSource_TextChanged(object sender, EventArgs e) {
            ParameterDict.Exemplar["Intern.Formula.Source"] = tbSource.Text;
        }


        /// <summary>
        /// Fehleransicht wird ausgeschaltet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShrink_Click(object sender, EventArgs e) {
            panel1.Visible = false;
        }
    }
}
