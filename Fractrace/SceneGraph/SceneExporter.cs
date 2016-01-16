using Fractrace.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fractrace.SceneGraph
{
    public class SceneExporter
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="X3dExporter"/> class.
        /// </summary>
        public SceneExporter(Iterate iter, PictureData pictureData)
        {
            _iterate = iter;
            _pictureData = pictureData;    
        }


        protected virtual void CreateMesh()
        {
            MeshTool _meshTool = new MeshTool(_iterate, _pictureData);
            _mesh = _meshTool.CreateMesh();
            if (!_meshTool.Valid)
                _valid = false;
        }

        protected bool _valid = true;

        /// <summary>
        /// Mesh to export.
        /// </summary>
        protected Mesh _mesh = null;

        /// <summary>
        /// PictureData of iter with surface coloring from PictureArt
        /// </summary>
        protected PictureData _pictureData = null;

        /// <summary>
        /// Used iterate
        /// </summary>
        protected Iterate _iterate = null;

        public virtual void Export(string fileName)
        {

        }


    }
}
