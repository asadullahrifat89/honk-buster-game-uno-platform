using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace HonkBusterGame
{
    public partial class ImageElement : Image
    {
        private readonly BitmapImage _bitmapImage;

        private Uri _uri;

        public ImageElement(Uri uri, double width, double height)
        {
            _uri = uri;
            _bitmapImage = new BitmapImage(uriSource: _uri);
            Source = _bitmapImage;
            Width = width;
            Height = height;
            CanDrag = false;
        }

        public void SetSource(Uri uri)
        {
            _uri = uri;
            _bitmapImage.UriSource = _uri;
        }

        public Uri GetSourceUri()
        {
            return _uri;
        }
    }
}
