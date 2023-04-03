using Windows.Foundation;

namespace HonkBusterGame
{
    public static class ConstructExtensions
    {
        private static readonly Random _random = new Random();

        #region Methods

        public static Uri GetContentUri(this GameObject construct)
        {
            if (construct.Content is not null && construct.Content is ImageElement image)
            {
                return image.GetSourceUri();
            }
            else
            {
                return null;
            }
        }

        public static Uri GetRandomContentUri(Uri[] uris)
        {
            return uris[_random.Next(uris.Length)];
        }

        /// <summary>
        /// Checks if a two rects intersect.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IntersectsWith(this Rect source, Rect target)
        {
            var targetX = target.X;
            var targetY = target.Y;
            var sourceX = source.X;
            var sourceY = source.Y;

            var sourceWidth = source.Width;
            var sourceHeight = source.Height;

            var targetWidth = target.Width;
            var targetHeight = target.Height;

            if (source.Width >= 0.0 && target.Width >= 0.0
                && targetX <= sourceX + sourceWidth && targetX + targetWidth >= sourceX
                && targetY <= sourceY + sourceHeight)
            {
                return targetY + targetHeight >= sourceY;
            }

            return false;
        }

        public static Rect GetHitBox(this GameObject Construct)
        {
            var rect = new Rect(
              x: Construct.GetLeft(),
              y: Construct.GetTop(),
              width: Construct.Width,
              height: Construct.Height);

            //Construct.SetHitBoxBorder(rect);

            return rect;
        }

        public static Rect GetCloseHitBox(this GameObject Construct)
        {
            var diviWidth = Construct.Width / 4;
            var diviHeight = Construct.Height / 4;

            var rect = new Rect(
                x: Construct.GetLeft() + diviWidth,
                y: Construct.GetTop() + diviHeight,
                width: Construct.Width - diviWidth,
                height: Construct.Height - diviHeight);

            //Construct.SetHitBoxBorder(rect);

            return rect;
        }

        //public static Rect GetHorizontalHitBox(this Construct Construct)
        //{
        //    /*
        //     *   __________
        //     *   |        |
        //     *  =============
        //     * ||     x     ||
        //     *  =============
        //     *   |        |
        //     *   ----------
        //     */

        //    var diviWidth = Construct.Width / 4;
        //    var diviHeight = Construct.Height / 5;

        //    var rect = new Rect(
        //        x: Construct.GetLeft() - diviWidth,
        //        y: Construct.GetTop(),
        //        width: Construct.Width + diviWidth,
        //        height: Construct.Height);

        //    //Construct.SetHitBoxBorder(rect);

        //    return rect;
        //}      

        public static Rect GetDistantHitBox(this GameObject Construct)
        {
            var multiWidth = (Construct.Width * 3);
            var multiHeight = (Construct.Height * 3);

            return new Rect(
                x: Construct.GetLeft() - multiWidth,
                y: Construct.GetTop() - multiHeight,
                width: Construct.Width + multiWidth,
                height: Construct.Height + multiHeight);
        }

        #endregion
    }
}
