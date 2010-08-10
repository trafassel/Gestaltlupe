using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BitmapPostProcessing {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }


        /// <summary>
        /// Rechts- links- Einzelbilder werden zu einem CrossEye-Stereobild zusammengesetzt. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e) {
            // Eine beliebige Datei wird ausgewählt
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string inputFile = ofd.FileName;
                if (inputFile.Contains("pic")) {
                    string dirName = System.IO.Path.GetDirectoryName(inputFile);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(inputFile);
                    string extension = System.IO.Path.GetExtension(inputFile);
                    fileName = fileName.Substring(0,fileName.IndexOf("pic"));
                    string rightPicSubstring="pic";
                    string leftPicSubString="stereo_pic_right";
                    int maxgap = 250;
                    int currentGap = 0;
                    int i = 10000;
                    while (currentGap < maxgap) {
                        string currentRightPicFileName = fileName +rightPicSubstring+ i.ToString()+extension;
                        string currentLeftPicFileName = fileName +leftPicSubString+ i.ToString()+extension;
                        if (System.IO.File.Exists(System.IO.Path.Combine(dirName,  currentRightPicFileName)) && System.IO.File.Exists(System.IO.Path.Combine(dirName, currentLeftPicFileName))) {
                            // TODO: Kombinieren und speichern.
                        } else {
                            currentGap++;
                        }
                        i++;
                    }

                }

            }
        }
    }
}
