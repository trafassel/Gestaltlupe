using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.Basic {


    /// <summary>
    /// Klassen, die eine asynchrone Berechnung starten, können sich über dieses
    /// Interface von der Beendigung der Berechnung informieren lassen.
    /// </summary>
    public interface IAsyncComputationStarter {


        /// <summary>
        /// Wird aufgerufen, wenn die asynchrone Berechnung bendet wurde.
        /// </summary>
        void ComputationEnds();


        /// <summary>
        /// Hier wird der Bearbeitungsfortschritt angezeigt.
        /// </summary>
        /// <param name="progrssInPercent"></param>
        void Progress(double progressInPercent);
    }
}
