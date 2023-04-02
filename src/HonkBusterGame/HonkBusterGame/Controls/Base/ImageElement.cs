using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace HonkBusterGame
{
    public partial class ImageElement : Image
    {
        private readonly BitmapImage _bitmapImage;

        public ImageElement(Uri uri, double width, double height)
        {
            _bitmapImage = new BitmapImage(uriSource: uri);
            Source = _bitmapImage;
            Width = width;
            Height = height;
            CanDrag = false;
        }

        public void SetSource(Uri uri)
        {
            _bitmapImage.UriSource = uri;
        }

        public Uri GetSourceUri()
        {
            return _bitmapImage.UriSource;
        }
    }
}
