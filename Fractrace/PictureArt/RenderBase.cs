using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Fractrace.DataTypes;
using Fractrace.Basic;
using Fractrace.PictureArt;
using Fractrace.Geometry;

namespace Fractrace.PictureArt
{
    public class RenderBase : Renderer
    {


        /// <summary>
        /// Initialisation.
        /// </summary>
        /// <param name="pData"></param>
        public RenderBase(PictureData pData)
            : base(pData)
        {

        }


        /// <summary>(
        /// Create RGB Picture.
        /// </summary>
        /// <param name="grLabel"></param>
        public override void Paint(Graphics grLabel)
        {
            if (formula == null)
            {
                System.Diagnostics.Debug.WriteLine("Warning in RenderBase.Paint() formula==null");
                return;
            }
            _width = pData.Width;
            _height = pData.Height;
            PreCalculate();
            grLabel.Clear(Color.FromArgb(0, 0, 0, 0));
            if (!_stopRequest)
            {
                for (int i = 0; i < _width; i++)
                {
                    for (int j = 0; j < _height; j++)
                    {
                        Pen p = new Pen(GetColor(i, j));
                        // Hier Exceeption: Das Objekt wird bereits an anderer Stelle verwendet
                        grLabel.DrawRectangle(p, i, j, (float)0.5, (float)0.5);
                    }
                }
            }
            CallPaintEnds();
        }


        /// <summary>
        /// Allgemeine Informationen werden erzeugt
        /// </summary>
        protected virtual void PreCalculate()
        {


        }


        /// <summary>
        /// Return rgb value at (x,y)
        /// </summary>
        protected virtual Color GetColor(int x, int y)
        {
            Vec3 col = GetRgbAt(x, y);
            if (col.X < 0)
                col.X = 0;
            if (col.Y < 0)
                col.Y = 0;
            if (col.Z < 0)
                col.Z = 0;
            if (col.X > 1)
                col.X = 1;
            if (col.Y > 1)
                col.Y = 1;
            if (col.Z > 1)
                col.Z = 1;

            try
            {
                if (double.IsNaN(col.X))
                    return Color.Red;
                return Color.FromArgb((int)(255.0 * col.X), (int)(255.0 * col.Y), (int)(255.0 * col.Z));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Color.Black;
        }


        protected virtual Vec3 GetRgbAt(int x, int y)
        {
            Vec3 retVal = new Vec3(1, 0, 0); // rot
            return retVal;
        }


    }
}
