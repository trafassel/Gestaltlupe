using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.TomoGeometry {
    public class TomoFormulaFactory {



        protected bool IsCsharpCode(string source) {
            if (source.Contains("long InSet"))
                return true;

            if (source.Contains("Overrides "))
                return false;


            if (source.Contains(" Function"))
                return false;

            // Default: Assume C# Code 
            return true;
        }

        public TomoFormula CreateFromString(string source) {
            TomoFormula retVal = null;
            bool isCsharpCode = IsCsharpCode(source);

            if (isCsharpCode) { // Expected Language: C#
                Microsoft.CSharp.CSharpCodeProvider csProviderOnEarlyNodeCode = null;
                CompileCS(source, out csProviderOnEarlyNodeCode);
                if (csProviderOnEarlyNodeCode == null)
                    return null;
                System.Reflection.Assembly mCAssembly = null;
                mCAssembly = results.CompiledAssembly;
                retVal = (TomoFormula)mCAssembly.CreateInstance("CSTomoFormula");
            } else { // Exprected Language VB
                Microsoft.VisualBasic.VBCodeProvider vbProviderOnEarlyNodeCode = null;
                CompileVB(source, out vbProviderOnEarlyNodeCode);
                if (vbProviderOnEarlyNodeCode == null)
                    return null;
                System.Reflection.Assembly mCAssembly = null;
                mCAssembly = results.CompiledAssembly;
                retVal = (TomoFormula)mCAssembly.CreateInstance("VBTomoFormula");


            }


            return retVal;
        }


        System.CodeDom.Compiler.CompilerResults results = null;



        

        /// <summary>
        /// Compiles the code in C# assembly.
        /// </summary>
        /// <param name="funcName">Name of the func.</param>
        /// <param name="code">The code.</param>
        /// <param name="provider">The provider.</param>
        protected void CompileVB(string code, out Microsoft.VisualBasic.VBCodeProvider provider) {

            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateInMemory = true;    //  'Assembly is created in memory
            parameters.TreatWarningsAsErrors = false;
            parameters.WarningLevel = 4;

            string[] refs = { "System.dll", System.Reflection.Assembly.GetExecutingAssembly().Location };

            parameters.ReferencedAssemblies.AddRange(refs);
            provider = new Microsoft.VisualBasic.VBCodeProvider();
           
            string tomoSource = @"
Imports System
Imports Fractrace.TomoGeometry


Public Class VBTomoFormula 
Inherits TomoFormula

" + code + @"

End Class
";

            try {
                results = provider.CompileAssemblyFromSource(parameters, tomoSource);
            } catch (Exception ex) {
                // TODO: Fehlerhafte Zeile markieren
                Console.WriteLine(ex.ToString());
                FormulaEditor.AddError(ex.ToString(),0,0);
                return;
            }

            if (results.Errors.Count != 0) {
                foreach (System.CodeDom.Compiler.CompilerError cerror in results.Errors) {
                    if (!cerror.IsWarning) {
                        provider = null;
                        Console.WriteLine(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        FormulaEditor.AddError(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"",cerror.Line,cerror.Column);

                        return;
                    }

                }
                //
            }
        }


        
        /// <summary>
        /// Compiles the code in C# assembly.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="provider">The provider.</param>
        protected void CompileCS(string code, out Microsoft.CSharp.CSharpCodeProvider provider) {

            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateInMemory = true;    //  'Assembly is created in memory
            parameters.TreatWarningsAsErrors = false;
            parameters.WarningLevel = 4;

            string[] refs = { "System.dll",System.Reflection.Assembly.GetExecutingAssembly().Location};

            parameters.ReferencedAssemblies.AddRange(refs);
            provider = new Microsoft.CSharp.CSharpCodeProvider();
          
            string tomoSource = @"
using System;
using Fractrace.TomoGeometry;
public class CSTomoFormula : "+ GuessFormulaClass(code) + @" {
public CSTomoFormula() {

    }

    " + code + @"
}
";

            try {
              results = provider.CompileAssemblyFromSource(parameters, tomoSource);
            } catch (Exception ex) {
                // TODO: Fehlerhafte Zeile markieren
                Console.WriteLine(ex.ToString());
                FormulaEditor.AddError(ex.ToString(),0,0);
                return;
            }

            if (results.Errors.Count != 0) {
                foreach (System.CodeDom.Compiler.CompilerError cerror in results.Errors) {
                    if (!cerror.IsWarning) {
                        provider = null;
                        Console.WriteLine(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        FormulaEditor.AddError(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"", cerror.Line, cerror.Column);
                        
                        return;
                    }

                }
                //
            }

        }


        /// <summary>
        /// Guess type of base formula ( TomoFormula or GestaltFormula )
        /// </summary>
        /// <param name="formulaSource"></param>
        /// <returns></returns>
        protected string GuessFormulaClass(string formulaSource)
        {
            if (formulaSource.Contains("GetBool"))
                return "GestaltFormula";
            return "TomoFormula";
        }


    }
}
