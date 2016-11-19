using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fractrace.Basic;

namespace Fractrace.Gui
{
    public partial class MaterialControl : UserControl
    {

        DataViewControlPage _rendererControl = null;
        public MaterialControl()
        {
            InitializeComponent();

            // test is for designer only
            if (ParameterInput.MainParameterInput != null && ParameterInput.MainParameterInput.MainDataViewControl != null)
            {
                _rendererControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _rendererControl.Dock = DockStyle.Fill;
                _rendererControl.Create(new string[]
                    {
                    "Renderer.ColorInside",
                    "Renderer.ColorOutside",
                    "Renderer.ColorIntensity",
                    "Renderer.ColorGreyness",
                    "Renderer.Brightness",
                    "Renderer.Contrast",
                    "Renderer.LightIntensity",
                    "Renderer.BrightLightLevel",
                    "Renderer.ShininessFactor",
                    "Renderer.Shininess",
                    "Renderer.ShadowJustify"
                    }
                    );
                pnlRenderer.Controls.Add(_rendererControl);
            }

        }


        /// <summary>
        /// Set some default rendering parameters.
        /// </summary>
        private void SetDefault()
        {
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShadowGlow", 0.94);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.UseLight", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0.2);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShadowJustify", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShininessFactor", 0.5);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Shininess", 28);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 1.05);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Contrast", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.SmoothNormalLevel", 8);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.LightIntensity", 0);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDouble("Renderer.BrightLightLevel", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 1.65);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShininessFactor", 0.8);
            ParameterDict.Current.SetDouble("Renderer.BrightLightLevel", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 3);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDouble("Renderer.BrightLightLevel", 0.4);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button16_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShadowGlow", 0.961);
            ParameterDict.Current.SetDouble("Renderer.ShadowJustify", 0.2);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0.8);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 3.5);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShadowGlow", 0.971);
            ParameterDict.Current.SetDouble("Renderer.ShadowJustify", 0.1);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Shininess", 28);
            ParameterDict.Current.SetDouble("Renderer.ShininessFactor", 0.8);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDouble("Renderer.LightIntensity", 0);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button6_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0);
            ParameterDict.Current.SetDouble("Renderer.LightIntensity", 0);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button17_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDouble("Renderer.LightIntensity", 1);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDouble("Renderer.ShadowJustify", 0);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button8_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0.3);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 1.2);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Shininess", 14);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShininessFactor", 0.3);
            ParameterDict.Current.SetDouble("Renderer.ShadowJustify", 1.5);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button9_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0.8);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 5);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Shininess", 14);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.ShininessFactor", 0.8);
            ParameterDict.Current.SetDouble("Renderer.ShadowJustify", 0.2);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }


        private void button18_Click(object sender, EventArgs e)
        {
            SetDefault();
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.BrightLightLevel", 0.94);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Brightness", 6.5);
            ParameterDict.Current.SetDoubleWithoutRaiseChange("Renderer.Shininess", 9);
            ParameterDict.Current.SetDouble("Renderer.ShininessFactor", 1);
            ResultImageView.PublicForm.ActivatePictureArt();
            ParameterInput.MainParameterInput.UpdatePictureArtInSmallPreview();
            UpdateFromChangeProperty();
        }

        /// <summary>
        /// Is called, if some properties changed.
        /// </summary>
        public void UpdateFromChangeProperty()
        {
            _rendererControl.UpdateElements();
        }


    }
}
