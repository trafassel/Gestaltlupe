

namespace Fractrace.Basic
{

    /// <summary>
    /// In DataView page used headline.
    /// </summary>
    class DataViewHeadlineElement : DataViewElement
    {

        protected override void PreInit()
        {
            lblName.Font = new System.Drawing.Font(lblName.Font, System.Drawing.FontStyle.Bold);
        }


    }
}

