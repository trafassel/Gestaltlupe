

namespace Fractrace.Basic
{

    /// <summary>
    /// In DataView page used headline.
    /// </summary>
    class DataViewHeadlineElement : DataViewElement
    {

        protected override void PreInit()
        {
            float currentSize = lblName.Font.Size;
            currentSize += 2.0F;

            //   lblName.Font = new System.Drawing.Font(lblName.Font.Name, currentSize, System.Drawing.FontStyle.Bold, lblName.Font.Unit);
            lblName.Font = new System.Drawing.Font(lblName.Font.Name, currentSize, lblName.Font.Style, lblName.Font.Unit);

            this.Height =(int)( 1.5 * this.Height);
            panel1.Width = 70;
        }


    }
}

