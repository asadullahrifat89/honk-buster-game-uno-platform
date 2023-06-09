﻿namespace HonkBusterGame
{
    public partial class HoveringTitleScreen : AnimableBase
    {
        #region Fields

        //private readonly Storyboard _opacity_storyboard;
        //private readonly DoubleAnimation _doubleAnimation;

        #endregion

        #region Ctor

        public HoveringTitleScreen()
        {
            #region Opacity Animation

            //_doubleAnimation = new DoubleAnimation()
            //{
            //    Duration = new Duration(TimeSpan.FromMilliseconds(700)),
            //    From = 0,
            //    To = 1,
            //};

            //Storyboard.SetTarget(_doubleAnimation, this.Content);
            //Storyboard.SetTargetProperty(_doubleAnimation, "Opacity");

            //_opacity_storyboard = new Storyboard();
            //_opacity_storyboard.Children.Add(_doubleAnimation);
            //_opacity_storyboard.Completed += (s, e) =>
            //{
            //    _opacity_storyboard.Stop();

            //    LoggingExtensions.Log($"HoveringTitleScreen: _opacity_storyboard -> Completed");
            //}; 

            #endregion
        }

        #endregion

        #region Methods

        public void Reset()
        {
            //_opacity_storyboard.Begin();
        }

        public void Reposition()
        {
            SetPosition(
                left: (((GameView.Width / 4) * 2) - Width / 2),
                top: ((GameView.Height / 2) - Height / 2));
        }

        #endregion
    }
}
