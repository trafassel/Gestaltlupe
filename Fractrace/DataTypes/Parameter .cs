using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.DataTypes {
    public class Parameter {
        public FracValues start_parameter = new FracValues();
        public FracValues end_parameter = new FracValues();
        public int number_of_frames = 0;
        public int fspin = 0;
        public int actual_frame = 0;
        public int zyklen = 0;
        public int formula = 0;
        public WinValues window_parameter = new WinValues();
        public string name = "";
        public void make_aktual_tupel(FracValues actual_values) {

        }
        public void print_parameter() { }
        public void set_parameter() {
            // TODO Parameter anpassen
        }

    }
}
