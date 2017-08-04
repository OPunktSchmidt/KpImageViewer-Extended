# KpImageViewer-Extended
Customized and extended version of an image viewer control found on CodeProject. All credits for the initial work go to Jordy from CodeProject (https://www.codeproject.com/script/Membership/View.aspx?mid=4545084) and his Image Viewer Control (https://www.codeproject.com/Articles/71225/Image-Viewer-UserControl).

This is a customized version for my needs. Check out the following changelog:

- The method FitToScreen() can now be called directly from KpImageViewer. So for example you can fit the image to the screen when the window size changes
- Provides the new public method Open(byte[] imageContent). It's a very simple method with just one line of code but with this method you don't need to care about assigning a correctly assembled System.Drawing.Bitmap object to the Image property. 
- Using System.Drawing.Image.RawFormat and System.Drawing.Imaging.ImageFormat to determine the correct image format for displaying TIFF and GIF files. Before this change, many unecessary exceptions were thrown and some other bugs prevented displaying TIFF and GIF files correctly (especially when loaded from a byte array).
