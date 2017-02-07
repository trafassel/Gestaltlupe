using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class AvailableExporters
    {

        List<SceneExporter> _exporters = new List<SceneExporter>();

        public AvailableExporters()
        {
            _exporters.Add(new X3DomExporter(null, null));
            _exporters.Add(new ObjFileExporter(null, null));
            _exporters.Add(new VrmlSceneExporter(null, null));
#if DEBUG
            _exporters.Add(new WebGlExporter(null, null));
#endif
        }

        /// <summary>
        /// Create instance of SceneExporter wth type as template.
        /// </summary>
        SceneExporter CreateExporter(SceneExporter template, Iterate iter, PictureData pictureData)
        {
            if (template is ObjFileExporter)
                return new ObjFileExporter(iter, pictureData);
            if (template is VrmlSceneExporter)
                return new VrmlSceneExporter(iter, pictureData);
            if (template is WebGlExporter)
                return new WebGlExporter(iter, pictureData);
            if (template is X3DomExporter)
                return new X3DomExporter(iter, pictureData);
            return null;
        }

        /// <summary>
        /// Return in SaveFileDialog used filter string for all availables exporters.
        /// </summary>
        public string FiledialogFilter
        {
            get
            {
                StringBuilder retVal = new StringBuilder();
                bool first = true;
                foreach (SceneExporter sceneExporter in _exporters)
                {
                    if (!first) retVal.Append("|");
                    retVal.Append(sceneExporter.FileDescription);
                    first = false;
                }
                return retVal.ToString();
            }
        }

        public bool Export(string fileName, Iterate iter, PictureData pictureData)
        {
            foreach (SceneExporter sceneExporter in _exporters)
            {

                if(sceneExporter.FileTypeIsSupported(fileName))
                {
                    SceneExporter exporter = CreateExporter(sceneExporter, iter, pictureData);
                    exporter.Export(fileName);
                    return true;
                }
            }
            return false;
        }

    }
}
