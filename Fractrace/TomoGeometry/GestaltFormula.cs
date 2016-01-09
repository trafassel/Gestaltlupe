using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fractrace.Basic;

namespace Fractrace.TomoGeometry
{
    public class GestaltFormula : TomoFormula
    {
        protected double _jx = 0;
        protected double _jy = 0;
        protected double _jz = 0;
        protected int _cycles = 10;
        protected bool _isJulia=false;

        public override void Init()
        {
            base.Init();
            _jx = ParameterDict.Current.GetDouble("Formula.Static.jx");
            _jy = ParameterDict.Current.GetDouble("Formula.Static.jy");
            _jz = ParameterDict.Current.GetDouble("Formula.Static.jz");
            _cycles = ParameterDict.Current.GetInt("Formula.Static.Cycles");
            if (ParameterDict.Current.GetInt("Formula.Static.Formula") == -2)
                _isJulia = true;
        }


        /// <summary>
        /// Return true if given point belongs to set.
        /// </summary>
        public virtual bool GetBool(double x, double y, double z)
        {
            return false;
        }


    }

}
