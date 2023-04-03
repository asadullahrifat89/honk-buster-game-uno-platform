﻿namespace HonkBusterGame
{
    public partial class RoadMark : MovableBase
    {
        #region Fields

        private readonly ImageContainer _imageContainer;
        private readonly Uri[] _tree_uris;

        #endregion

        #region Ctor

        public RoadMark(
            Action<GameObject> animateAction,
            Action<GameObject> recycleAction)
        {
            ConstructType = ConstructType.ROAD_MARK;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _tree_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_MARK).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_tree_uris);
            _imageContainer = new(uri: uri, width: this.Width, height: this.Height);
            //_imageContainer.SetDropShadow(offsetX: 0, offsetY: 0, blurRadius: 5);
            SetContent(_imageContainer);

            SetSkewY(36);
            SetRotation(-63.5);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
        }

        #endregion

        public void Reset()
        {
            _imageContainer.SetBrighness(GameView.IsNightModeActivated ? 50 : 100);
        }
    }
}
