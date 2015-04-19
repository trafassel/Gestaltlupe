
using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic
{
    class DataViewHeadlineElement : DataViewElement
    {
        protected override void PreInit()
        {
            lblName.Font = new System.Drawing.Font(lblName.Font, System.Drawing.FontStyle.Bold);
        }

    }
}

