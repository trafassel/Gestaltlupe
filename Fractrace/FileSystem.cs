using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Fractrace
{
    public class FileSystem
    {

        /// <summary>
        /// Lock access to Exemplar.
        /// </summary>
        protected static Object _lockVar = new Object();

        /// <summary>
        /// Current File count (start with 10000 for easy sorting in video processing software)
        /// </summary>
        protected int _fileCount = 10000;

        protected static FileSystem _exemplar = null;
        public static FileSystem Exemplar
        {
            get
            {
                lock (_lockVar)
                {
                    if (_exemplar == null)
                        _exemplar = new FileSystem();
                    return _exemplar;

                }
            }
        }

        /// <summary>
        /// Return common diroctory of all with GetFileName() cretated Filenames in this program instance.
        /// </summary>
        protected string _projectDir = "";
        public string ProjectDir { get { return _projectDir; } }

        /// <summary>
        /// Get the directory where the created bitmaps and the corresponding settings are stored.
        /// </summary>
        public string ExportDir
        {
            get
            {
                string exportDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                exportDir = Path.Combine(exportDir, "Gestaltlupe");
                if (!Directory.Exists(exportDir))
                    Directory.CreateDirectory(exportDir);
                return exportDir;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystem"/> class.
        /// Warning: Each time a new instance of the class is created, a 
        /// new directory in the user home path is created. 
        /// </summary>
        protected FileSystem()
        {
            int i = 0;
            foreach (string str in Directory.GetDirectories(ExportDir))
            {
                string pattern = str.Substring(4);
                int maxTest = 0;
                int.TryParse(pattern, out maxTest);
                if (maxTest > i)
                    i = maxTest;
            }
            i = i + 1;

            do
            {
                _projectDir = Path.Combine(ExportDir, "Data" + i.ToString());
                i++;
            } while (Directory.Exists(_projectDir));
            Directory.CreateDirectory(_projectDir);
        }


        /// <summary>
        /// Get full path of next available Filename. The result has the same extension as template extension. Gestaltlupe 1.3. template: "pic.png". 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public string GetFileName(string template)
        {
            string retVal = "";
            string file = Path.GetFileName(ProjectDir) + Path.GetFileNameWithoutExtension(template);

            retVal = Path.Combine(ProjectDir, file + _fileCount.ToString() + Path.GetExtension(template));
            // Darf eigentlich nicht vorkommen wenn mit leeren Verzeichnis begonnen wird.
            while (File.Exists(retVal))
            {
                _fileCount++;
                retVal = Path.Combine(ProjectDir, file + _fileCount.ToString() + Path.GetExtension(template));
            }
            // Die dazu passenden Parameter werden unter
            // data/parameters/ gespeichert.
            string infoFilePath = Path.Combine(ExportDir, "data");
            if (!Directory.Exists(infoFilePath))
                Directory.CreateDirectory(infoFilePath);
            infoFilePath = Path.Combine(infoFilePath, "parameters");
            if (!Directory.Exists(infoFilePath))
                Directory.CreateDirectory(infoFilePath);
            Basic.ParameterDict.Current.Save(Path.Combine(infoFilePath, Path.GetFileNameWithoutExtension(retVal) + ".gestalt"));

            string exportDir = Path.Combine(ExportDir, "TomoTrace");
            if (!Directory.Exists(exportDir))
                Directory.CreateDirectory(exportDir);

            return retVal;
        }



    }
}
