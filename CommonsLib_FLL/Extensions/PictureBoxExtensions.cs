using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CommonsLib_FLL.Extensions
{
    /// <summary>
    /// PictureBox Extensions.
    /// </summary>
    public static class PictureBoxExtensions
    {

        /// <summary>
        /// Loads an image into a PictureBox without holding it in filesystem.
        /// </summary>
        /// <param name="self">PictureBox itself.</param>
        /// <param name="imagePath">Path to the image to be loaded.</param>
        public static void LoadImageFromPath(this PictureBox self, string imagePath)
        {
            self.ReleaseLoadedImage();
            if (!File.Exists(imagePath)) return;
            using (var tmpImage = Image.FromFile(imagePath))
            {
                // tmpImage is holding FS image, so let's create new bitmap from it, 
                // and FS image will be released as soon as flow goes out of using statement.
                self.Image = new Bitmap(tmpImage);
            }
        }

        /// <summary>
        /// Cleans PictureBox Image, and disposes it if it exists.
        /// Extension method for PictureBox.
        /// </summary>
        /// <param name="self">PictureBox itself.</param>
        public static void ReleaseLoadedImage(this PictureBox self)
        {
            var loadedBmp = self.Image;
            self.Image = null;
            loadedBmp?.Dispose();
        }
    }
}