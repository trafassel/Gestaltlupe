using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BitmapPostProcessing {
    public class BitmapCombine {

        System.Drawing.Image leftImage = null;
        System.Drawing.Image rightImage = null;

        Image outputImage = null;
        Graphics outputGraphics = null;
              
        /// <summary>
        /// Fügt linkes und rechtes Bild zusammen und speichert die Composition unter outputFile.
        /// </summary>
        /// <param name="leftInputFile"></param>
        /// <param name="rightInputFile"></param>
        /// <param name="outputFile"></param>
        public void Combine(string leftInputFile, string rightInputFile, string outputFile) {

            if (leftImage != null)
                leftImage.Dispose();
              leftImage = Image.FromFile(leftInputFile);
              if (rightImage != null)
                  rightImage.Dispose();
              rightImage = Image.FromFile(rightInputFile);
             // Graphics leftGraphics = Graphics.FromImage(leftImage);
             // Graphics rightGraphics = Graphics.FromImage(rightImage);
              int width = leftImage.Width * 2;
              int height = rightImage.Height;
              if (outputImage == null) {
                  outputImage = new Bitmap(width, height);
                  outputGraphics = Graphics.FromImage(outputImage);
              }
              outputGraphics.DrawImageUnscaled(leftImage, 0, 0);
              outputGraphics.DrawImageUnscaled(rightImage, leftImage.Width, 0);

            /*
              for (int i = 0; i < leftImage.Width; i++) {
                  for (int j = 0; j < height; j++) {

                      // Left pic:
                      leftGraphics.
                      Pen p = new Pen(GetColor(i, j));
                      grLabel.DrawRectangle(p, i, j, (float)0.5, (float)0.5);
                  }
              }
            
            */
              if (System.IO.File.Exists(outputFile))
                  System.IO.File.Delete(outputFile);
              outputImage.Save(outputFile);
            //  outputImage.Dispose();
        }
    }
}
