using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace HonkBusterGame
{
    public partial class HoveringTitleScreen : AnimableConstruct
    {
        #region Fields

        private readonly Storyboard _opacity_storyboard;
        private readonly DoubleAnimation _doubleAnimation;

        #endregion

        #region Ctor

        public HoveringTitleScreen()
        {
            _doubleAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(700)),
                From = 0,
                To = 1,
            };

            Storyboard.SetTarget(_doubleAnimation, this);
            Storyboard.SetTargetProperty(_doubleAnimation, "Opacity");

            _opacity_storyboard = new Storyboard();
            _opacity_storyboard.Children.Add(_doubleAnimation);
            _opacity_storyboard.Completed += (s, e) =>
            {
                _opacity_storyboard.Stop();

                LoggingExtensions.Log($"HoveringTitleScreen: _opacity_storyboard -> Completed");
            };
        }

        #endregion

        #region Methods

        public void Reset()
        {
            _opacity_storyboard.Begin();
        }

        public void Reposition()
        {
            SetPosition(
                left: (((Scene.Width / 4) * 2) - Width / 2),
                top: ((Scene.Height / 2) - Height / 2));
        }

        #endregion
    }
}
