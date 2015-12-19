
namespace Fractrace.Geometry
{

    /// <summary>
    /// Base class for used transformations.
    /// </summary>
    public class Transform3D
    {


        public Transform3D()
        {
        }


        /// <summary>
        /// Initialisation from current global parameters.
        /// </summary>
        public virtual void Init()
        {
        }



        /// <summary>
        /// Apply transformation.
        /// </summary>
        public virtual Vec3 Transform(Vec3 input)
        {
            Vec3 p1 = new Vec3(input);
            return (p1);
        }

    }
}
