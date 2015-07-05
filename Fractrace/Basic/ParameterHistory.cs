using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic {


  /// <summary>
  /// Managed a list of all parameter changes.
  /// </summary>
  public class ParameterHistory: ParameterDictData {


    /// <summary>
    /// Protected access to CurrentTime.
    /// </summary>
    protected int mCurrentTime = 0;


    /// <summary>
    /// Current time as event count.
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
