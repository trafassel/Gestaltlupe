using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.TomoGeometry {
    public class TomoFormulaFactory {



        public TomoFormula CreateFromString(string source) {
            TomoFormula retVal = null;
            Microsoft.CSharp.CSharpCodeProvider csProviderOnEarlyNodeCode = null;
            CompileCS(source, out csProviderOnEarlyNodeCode);

            if (csProviderOnEarlyNodeCode == null)
                return null;

            //Type scriptType;
            System.Reflection.Assembly mCAssembly = null;
            mCAssembly = results.CompiledAssembly;
            retVal = (TomoFormula)mCAssembly.CreateInstance("CSTomoFormula");

            return retVal;
        }


        System.CodeDom.Compiler.CompilerResults results = null;

        /// <summary>
        /// Compiles the CS.
        /// </summary>
        /// <param name="funcName">Name of the func.</param>
        /// <param name="code">The code.</param>
        /// <param name="provider">The provider.</param>
        protected void CompileCS(string code, out Microsoft.CSharp.CSharpCodeProvider provider) {

            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateInMemory = true;    //  'Assembly is created in memory
            parameters.TreatWarningsAsErrors = false;
            parameters.WarningLevel = 4;

            string[] refs = { "System.dll",System.Reflection.Assembly.GetExecutingAssembly().Location};
            //string[] refs = { LibPath + "X3d.dll", LibPath + "Scripting.dll", LibPath + "taraVRoptimizerLib.dll" };

            parameters.ReferencedAssemblies.AddRange(refs);
            provider = new Microsoft.CSharp.CSharpCodeProvider();
           
             //System.CodeDom.Compiler.ICodeCompiler compiler = provider.CreateCompiler();

            string tomoSource = @"
using System;
using Fractrace.TomoGeometry;
public class CSTomoFormula : TomoFormula {
public CSTomoFormula() {

    }

    " + code + @"
}
";


            try {
                //results = compiler.CompileAssemblyFromSource(parameters, tomoSource);
              results = provider.CompileAssemblyFromSource(parameters, tomoSource);
            } catch (Exception ex) {
                // TODO: Fehlerhafte Zeile markieren
                Console.WriteLine(ex.ToString());
                FormulaEditor.AddError(ex.ToString());
                return;
            }

            if (results.Errors.Count != 0) {
                foreach (System.CodeDom.Compiler.CompilerError cerror in results.Errors) {
                    if (!cerror.IsWarning) {
                        provider = null;
                        Console.WriteLine(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        FormulaEditor.AddError(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        
                        return;
                    }

                }
                //
            }

        }
    }
}
