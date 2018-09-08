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
        DataViewControlPage _lightControl = null;
        DataViewControlPage _colorControl = null;
        DataViewControlPage _backgroundControl = null;

        public MaterialControl()
        {
            InitializeComponent();

            // Test if this control is openened in Visual Studio Designer:
            if (ParameterInput.MainParameterInput != null && ParameterInput.MainParameterInput.MainDataViewControl != null)
            {
                _rendererControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _rendererControl.Dock = DockStyle.Fill;
                _rendererControl.Create(new string[]
                    {
                    "Renderer.Brightness",
                    "Renderer.Contrast",
//                    "Renderer.BrightLightLevel",
                    "Renderer.ShadowGlow",
                    "Renderer.ShadowJustify",
                    "Renderer.AmbientIntensity",
                    "Renderer.MinFieldOfView",
                    "Renderer.MaxFieldOfView",
  //                  "Renderer.Normalize",
                    }
                    );
                pnlRenderer.Controls.Add(_rendererControl);

                _lightControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _lightControl.Dock = DockStyle.Fill;
                _lightControl.Create(new string[]
                    {
    //                "Renderer.ShininessFactor",
    //                "Renderer.Shininess",
                    "Renderer.Light.X",
                    "Renderer.Light.Y",
                    "Renderer.Light.Z",
                    }
                    );
                pnlLight.Controls.Add(_lightControl);

                _colorControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _colorControl.Dock = DockStyle.Fill;
                _colorControl.Create(new string[]
                    {
                    "Renderer.ColorInside",
                    "Renderer.ColorOutside",
                    "Renderer.ColorIntensity",
                    "Renderer.ColorGreyness",
                    "Renderer.ColorFactor.Red",
                    "Renderer.ColorFactor.Green",
                    "Renderer.ColorFactor.Blue",
                    "Renderer.ColorFactor.Threshold",
                    }
                    );
                pnlColor.Controls.Add(_colorControl);

                _backgroundControl = new DataViewControlPage(ParameterInput.MainParameterInput.MainDataViewControl);
                _backgroundControl.Dock = DockStyle.Fill;
                _backgroundControl.Create(new string[]
                    {
                    "Renderer.UseDarken",
                    "Renderer.BackColor.Red",
                    "Renderer.BackColor.Green",
                    "Renderer.BackColor.Blue",
                    "Renderer.BackColor.Transparent"
                    }
                    );
                pnlBackground.Controls.Add(_backgroundControl);

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
            _lightControl.UpdateElements();
            _colorControl.UpdateElements();
            _backgroundControl.UpdateElements();
        }


    }
}
