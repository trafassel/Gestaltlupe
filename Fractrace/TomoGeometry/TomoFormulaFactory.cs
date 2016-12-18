using Fractrace.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fractrace.TomoGeometry
{
    public class TomoFormulaFactory
    {



        protected bool IsCsharpCode(string source)
        {
            if (source.Contains("long InSet"))
                return true;

            if (source.Contains("Overrides "))
                return false;

            if (source.Contains(" Function"))
                return false;

            // Default: Assume C# Code 
            return true;
        }

        public TomoFormula CreateFromString(string source)
        {
            TomoFormula retVal = null;
            bool isCsharpCode = IsCsharpCode(source);

            if (isCsharpCode)
            { // Expected Language: C#
                Microsoft.CSharp.CSharpCodeProvider csProviderOnEarlyNodeCode = null;
                CompileCS(source, out csProviderOnEarlyNodeCode);
                if (csProviderOnEarlyNodeCode == null)
                    return null;
                System.Reflection.Assembly mCAssembly = null;
                mCAssembly = results.CompiledAssembly;
                retVal = (TomoFormula)mCAssembly.CreateInstance("CSTomoFormula");
            }
            else
            { // Exprected Language VB
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
        /// Compiles the Visual Basic code in assembly.
        /// </summary>
        protected void CompileVB(string code, out Microsoft.VisualBasic.VBCodeProvider provider)
        {

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

            try
            {
                results = provider.CompileAssemblyFromSource(parameters, tomoSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FormulaEditor.AddError(ex.ToString(), 0, 0);
                return;
            }

            if (results.Errors.Count != 0)
            {
                foreach (System.CodeDom.Compiler.CompilerError cerror in results.Errors)
                {
                    if (!cerror.IsWarning)
                    {
                        provider = null;
                        Console.WriteLine(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        FormulaEditor.AddError(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"", cerror.Line, cerror.Column);
                        return;
                    }
                }
            }
        }




        // Create interface parameter definition for public variables from formula source code.
        protected List<Tuple<string, string, string>> ParseInterfaceVariables(string code)
        {
            List<Tuple<string, string, string>> interfaceVariableDefinition = new List<Tuple<string, string, string>>();
            string[] codeLines = code.Split('\n');
            foreach (string line in codeLines)
            {
                if (line.TrimStart().StartsWith("public "))
                {
                    string codeLine = line.Trim();
                    codeLine = codeLine.Replace("=", " = ");
                    codeLine = codeLine.Replace(";", " ; ");
                    string[] lineElements = codeLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = "";
                    string parameterType = "";
                    string defaultValue = "";
                    if (lineElements.Length >= 3 && lineElements[1] != "override")
                    {
                        name = lineElements[2];
                        parameterType = lineElements[1];
                    }
                    if (lineElements.Length >= 6 && lineElements[3] == "=")
                    {
                        defaultValue = lineElements[4];
                    }
                    if (name != "")
                        interfaceVariableDefinition.Add(new Tuple<string, string, string>(name, parameterType, defaultValue));
                }
            }
            return interfaceVariableDefinition;
        }


        /// <summary>
        /// Compiles the code in C# assembly.
        /// </summary>
        protected void CompileCS(string code, out Microsoft.CSharp.CSharpCodeProvider provider)
        {

            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateInMemory = true;    //  'Assembly is created in memory
            parameters.TreatWarningsAsErrors = false;
            parameters.WarningLevel = 4;

            string[] refs = { "System.dll", System.Reflection.Assembly.GetExecutingAssembly().Location };

            parameters.ReferencedAssemblies.AddRange(refs);
            provider = new Microsoft.CSharp.CSharpCodeProvider();

            List<Tuple<string, string, string>> interfaceVariableDefinition = ParseInterfaceVariables(code);

            StringBuilder initialisiationCode = new StringBuilder();
            foreach (Tuple<string, string, string> parameterInfo in interfaceVariableDefinition)
            {
                switch (parameterInfo.Item2)
                {
                    case "double":
                        if (parameterInfo.Item3 != "")
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=GetOrSetDouble(\"" + parameterInfo.Item1 + "\"," + parameterInfo.Item3 + ");");
                        else
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=GetOrSetDouble(\"" + parameterInfo.Item1 + "\");");
                        break;
                    case "bool":
                        if (parameterInfo.Item3 != "")
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=GetOrSetBool(\"" + parameterInfo.Item1 + "\"," + parameterInfo.Item3 + ");");
                        else
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=GetOrSetBool(\"" + parameterInfo.Item1 + "\");");
                        break;
                    case "int":
                        if (parameterInfo.Item3 != "")
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=(int)GetOrSetDouble(\"" + parameterInfo.Item1 + "\"," + parameterInfo.Item3 + ");");
                        else
                            initialisiationCode.AppendLine(parameterInfo.Item1 + "=(int)GetOrSetDouble(\"" + parameterInfo.Item1 + "\");");
                        break;
                }

            }
            
            string tomoSource = @"
using System;
using Fractrace;
using Fractrace.TomoGeometry;
using Fractrace.Geometry;

public class CSTomoFormula : " + GuessFormulaClass(code) + @" {

    " + code + @"
public CSTomoFormula() {
" + initialisiationCode.ToString()
+
@"
    }

}
";

            try
            {
                results = provider.CompileAssemblyFromSource(parameters, tomoSource);
                RemoveUnusedParameters(tomoSource);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FormulaEditor.AddError(ex.ToString(), 0, 0);
                return;
            }
            if (results.Errors.Count != 0)
            {
                foreach (System.CodeDom.Compiler.CompilerError cerror in results.Errors)
                {
                    if (!cerror.IsWarning)
                    {
                        provider = null;
                        Console.WriteLine(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"");
                        FormulaEditor.AddError(cerror.ErrorText + "Line " + cerror.Line + " " + cerror.Column + " in \"" + tomoSource + "\"", cerror.Line, cerror.Column);
                        return;
                    }
                }
            }
        }

        static object lockRemoveUnusedParameters = new object();

        void RemoveUnusedParameters(string tomoSource)
        {
            lock (lockRemoveUnusedParameters)
            {
                try
                {
                    if (tomoSource.Contains("SetParameterBulk"))
                        return;
                    Dictionary<string, bool> usedParameters = UsedParameters(tomoSource);
                    List<string> entriesToDelete = new List<string>();
                    foreach (KeyValuePair<string, string> entry in ParameterDict.Current.Entries)
                    {
                        if (entry.Key.StartsWith("Formula.Parameters."))
                        {
                            if (!ParameterDict.IsAdditionalInfo(entry.Key))
                            {
                                string parameterName = entry.Key.Substring("Formula.Parameters.".Length);
                                if (!usedParameters.ContainsKey(parameterName))
                                {
                                    entriesToDelete.Add(parameterName);
                                }
                            }
                        }
                    }
                    foreach (string entryToDelete in entriesToDelete)
                    {
                        ParameterDict.Current.RemoveProperty("Formula.Parameters." + entryToDelete);
                    }
                } catch(System.InvalidOperationException)
                {
                    // throwed if enumeration is not possible because ParameterDict.Current.Entries has changed in other thread.
                }
            }
        }

        Dictionary<string,bool> UsedParameters(string code)
        {
            Dictionary<string, bool> usedParameters = new Dictionary<string, bool>();
            if (code.Trim() == "")
                return usedParameters;

            int i = 0;
            while(i!=-1)
            {
                i = code.IndexOf("GetOrSet", i);
                if(i!=-1)
                {
                    i = code.IndexOf("\"", i)+1;
                    int nameStart = i;
                    i = code.IndexOf("\"", i);
                    int nameEnd = i;
                    if(nameStart!=-1 && nameEnd!=-1)
                    {
                        string parameterName = code.Substring(nameStart, nameEnd - nameStart);
                        if(parameterName.StartsWith("Formula.Parameters."))
                            parameterName = parameterName.Substring("Formula.Parameters.".Length);
                        usedParameters[parameterName] = true;
                    }
                }
            }
            return usedParameters;
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
