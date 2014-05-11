using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Fractrace
{
    public class FileSystem
    {

        protected static Object lockVar = new Object();

        protected static FileSystem mExemplar = null;
        public static FileSystem Exemplar
        {
            get
            {
                lock (lockVar)
                {
                    if (mExemplar == null)
                        mExemplar = new FileSystem();
                    return mExemplar;

                }

            }

        }


        /// <summary>
        /// Get the directory where the created bitmaps and the corresponding settings are stored.
        /// </summary>
        public string ExportDir
        {
            get
            {
                string exportDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                exportDir = System.IO.Path.Combine(exportDir, "TomoTrace");
                if (!System.IO.Directory.Exists(exportDir))
                    System.IO.Directory.CreateDirectory(exportDir);
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
            foreach (string str in System.IO.Directory.GetDirectories(ExportDir))
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
                mProjectDir = System.IO.Path.Combine(ExportDir, "Data" + i.ToString());
                i++;
            } while (System.IO.Directory.Exists(mProjectDir));

            System.IO.Directory.CreateDirectory(mProjectDir);
        }


        /// <summary>
        /// Current File count (start with 10000 for easy sorting in video processing software)
        /// </summary>
        protected int fileCount = 10000;


        /// <summary>
        /// Liefert einen passenden Dateinamen.
        /// </summary>
        /// <param name="shortFileName"></param>
        /// <returns></returns>
        public string GetFileName(string shortFileName)
        {

            string retVal = "";
            string file = Path.GetFileName(ProjectDir) + Path.GetFileNameWithoutExtension(shortFileName);

            retVal = System.IO.Path.Combine(ProjectDir, file + fileCount.ToString() + Path.GetExtension(shortFileName));
            // Darf eigentlich nicht vorkommen wenn mit leeren Verzeichnis begonnen wird.
            while (File.Exists(retVal))
            {
                fileCount++;
                retVal = System.IO.Path.Combine(ProjectDir, file + fileCount.ToString() + Path.GetExtension(shortFileName));
            }
            // Die dazu passenden Parameter werden unter
            // data/parameters/ gespeichert.
            string infoFilePath = System.IO.Path.Combine(ExportDir, "data");
            if (!System.IO.Directory.Exists(infoFilePath))
                System.IO.Directory.CreateDirectory(infoFilePath);
            infoFilePath = System.IO.Path.Combine(infoFilePath, "parameters");
            if (!System.IO.Directory.Exists(infoFilePath))
                System.IO.Directory.CreateDirectory(infoFilePath);
            Fractrace.Basic.ParameterDict.Exemplar.Save(System.IO.Path.Combine(infoFilePath, System.IO.Path.GetFileNameWithoutExtension(retVal) + ".tomo"));

            string exportDir = System.IO.Path.Combine(ExportDir, "TomoTrace");
            if (!System.IO.Directory.Exists(exportDir))
                System.IO.Directory.CreateDirectory(exportDir);

            return retVal;
        }

        protected string mProjectDir = "";
        public string ProjectDir
        {
            get
            {
                return mProjectDir;
            }
        }

    }
}
