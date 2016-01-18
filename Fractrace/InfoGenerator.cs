using System;
using System.Collections.Generic;
using System.Text;

using Fractrace.Basic;

namespace Fractrace
{


    /// <summary>
    /// Helper Class, witch generates useful informations mostly in text form.
    /// </summary>
    public class InfoGenerator
    {


        /// <summary>
        /// Generates the compressed formula.
        /// The formula parameters and the formula itself are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        /// <returns></returns>
        public static string GenerateCompressedFormula()
        {
            //return "// sorry not implemnted";
            string formula = ParameterDict.Current["Intern.Formula.Source"];

            List<string> formulaSettingCategories = new List<string>();
            formulaSettingCategories.Add("Scene");
            //formulaSettingCategories.Add("View.Width");
            //formulaSettingCategories.Add("View.Height");
            //formulaSettingCategories.Add("Border");
            formulaSettingCategories.Add("View.Perspective");
            //formulaSettingCategories.Add("Border");
            formulaSettingCategories.Add("Transformation.Camera");
            formulaSettingCategories.Add("Transformation.Perspectice");
            formulaSettingCategories.Add("Formula");

            // To make the new settings unique

            ParameterDict.Current["intern.Formula.TempUpdateVal"] = "vv";
            string testHash = ParameterDict.Current.GetHash("");

            string insertSettingsStringHere = "base.Init();";
            if (formula.Contains(insertSettingsStringHere))
            {

                StringBuilder settingsString = new StringBuilder();
                settingsString.Append("if(GetString(\"intern.Formula.TempUpdateVal\")!=\"" + testHash + "\"){");
                settingsString.Append("SetParameterBulk(\"");
                foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
                {
                    bool isInCategorie = false;
                    foreach (string testCategorie in formulaSettingCategories)
                    {
                        if (entry.Key.StartsWith(testCategorie))
                        {
                            isInCategorie = true;
                            break;
                        }
                    }
                    if (isInCategorie)
                    {
                        if(!ParameterDict.IsAdditionalInfo(entry.Key))
                          settingsString.Append("<Entry Key='" + entry.Key + "' Value='" + entry.Value + "' />");
                    }
                }

                // fix this formula to testHash
                settingsString.Append("<Entry Key='intern.Formula.TempUpdateVal' Value='" + testHash + "' />");
                settingsString.Append("\");");
                settingsString.Append("}");
                formula = formula.Replace(insertSettingsStringHere, insertSettingsStringHere + settingsString.ToString());
            }

            StringBuilder retVal = new StringBuilder();
            retVal.Append(CompressFormula(formula));
            return retVal.ToString();
        }


        /// <summary>
        /// Generates the compressed formula.
        /// The formula parameters and the formula itself are combined in a compact text,
        /// which can later be copied into the formula text window to get the current
        /// formula configuration.
        /// </summary>
        /// <returns></returns>
        public static string GenerateCompressedFormulaAndViewSettings()
        {
            //return "// sorry not implemnted";
            string formula = ParameterDict.Current["Intern.Formula.Source"];

            List<string> formulaSettingCategories = new List<string>();
            //formulaSettingCategories.Add("Border");
            //formulaSettingCategories.Add("View.Width");
            //formulaSettingCategories.Add("View.Height");
            formulaSettingCategories.Add("Scene");
            formulaSettingCategories.Add("View.Perspective");
            formulaSettingCategories.Add("Transformation.Camera");
            formulaSettingCategories.Add("Transformation.Perspectice");
            formulaSettingCategories.Add("Formula");
            formulaSettingCategories.Add("Renderer");
            formulaSettingCategories.Add("Renderer.BackColor");
            formulaSettingCategories.Add("Renderer.ColorFactor");
            formulaSettingCategories.Add("Renderer.Light");
            //formulaSettingCategories.Add("Intern.Formula");

            // To make the new settings unique

            ParameterDict.Current["intern.Formula.TempUpdateVal"] = "vv";
            string testHash = ParameterDict.Current.GetHash("");

            string insertSettingsStringHere = "base.Init();";
            if (formula.Contains(insertSettingsStringHere))
            {

                StringBuilder settingsString = new StringBuilder();
                settingsString.Append("if(GetString(\"intern.Formula.TempUpdateVal\")!=\"" + testHash + "\"){");
                settingsString.Append("SetParameterBulk(\"");
                foreach (KeyValuePair<string, string> entry in ParameterDict.Current.SortedEntries)
                {
                    bool isInCategorie = false;
                    foreach (string testCategorie in formulaSettingCategories)
                    {
                        if (entry.Key.StartsWith(testCategorie))
                        {
                            isInCategorie = true;
                            break;
                        }
                    }
                    if (isInCategorie)
                    {
                        if (!ParameterDict.IsAdditionalInfo(entry.Key))
                            settingsString.Append("<Entry Key='" + entry.Key + "' Value='" + entry.Value + "' />");
                    }
                }

                // fix this formula to testHash
                settingsString.Append("<Entry Key='intern.Formula.TempUpdateVal' Value='" + testHash + "' />");
                settingsString.Append("\");");
                settingsString.Append("}");
                formula = formula.Replace(insertSettingsStringHere, insertSettingsStringHere + settingsString.ToString());
            }

            StringBuilder retVal = new StringBuilder();
            retVal.Append(CompressFormula(formula));
            return retVal.ToString();
        }


        /// <summary>
        /// Compress the formula (remove big spaces, newlines, comments.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        protected static string CompressFormula(string source)
        {
            string[] lines = source.Split('\n');
            StringBuilder sourceBlock = new StringBuilder();
            foreach (string line in lines)
            {
                string lineToAdd = line;
                if (line.Contains("//"))
                {
                    int index = line.IndexOf("//");
                    lineToAdd = line.Substring(0, index);
                }
                lineToAdd.Trim();
                if (lineToAdd != "")
                    sourceBlock.Append(lineToAdd);
            }

            source = sourceBlock.ToString();
            source = source.Replace("\n", " ");
            source = source.Replace("\t", " ");
            source = source.Replace("\r", " ");
            source = source.Replace("  ", " ");
            source = source.Replace("  ", " ");
            source = source.Replace("  ", " ");
            source = source.Replace("  ", " ");
            source = source.Replace("  ", " ");

            return source;
        }


    }
}
