using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fractrace.Basic {
  public class ParameterHistory: ParameterDictData {


    protected int mCurrentTime = 0;

    /// <summary>
    /// Der aktuelle Zeitpunkt
    /// </summary>
    public int CurrentTime {
      get {
        return mCurrentTime;
      }

      set {
        mCurrentTime = value;
      }
    }
  }
}
