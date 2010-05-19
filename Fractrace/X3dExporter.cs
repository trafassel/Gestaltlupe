using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fractrace {
  
  /// <summary>
  /// Dient dem Export der dargestellten 3D Geometrie.
  /// </summary>
  public class X3dExporter {

    /// <summary>
    /// Initializes a new instance of the <see cref="X3dExporter"/> class.
    /// </summary>
    /// <param name="iter">The iter.</param>
    public X3dExporter(Iterate iter) {
      mIterate = iter;
    }


    protected Iterate mIterate = null;


    /// <summary>
    /// Die Geometrie aus iter wird in der VRML-Datei fileName abgelegt.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void Save(string fileName) {
      if (mIterate == null)
        return;
      StreamWriter sw = new System.IO.StreamWriter(fileName, false, Encoding.GetEncoding("iso-8859-1"));
      sw.WriteLine("#VRML V2.0 utf8");

      sw.Close();
    }

  }
}
