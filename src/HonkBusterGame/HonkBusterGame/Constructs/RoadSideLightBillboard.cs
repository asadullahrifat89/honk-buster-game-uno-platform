namespace HonkBusterGame
{
    public partial class RoadSideLightBillboard : MovableConstruct
    {
        #region Fields

        private readonly ImageElement _content_image;
        private readonly Uri[] _tree_uris;

        #endregion

        #region Ctor

        public RoadSideLightBillboard(
            Action<Construct> animateAction,
            Action<Construct> recycleAction)
        {
            ConstructType = ConstructType.ROAD_SIDE_LIGHT_BILLBOARD;

            AnimateAction = animateAction;
            RecycleAction = recycleAction;

            _tree_uris = Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.ROAD_SIDE_LIGHT_BILLBOARD).Select(x => x.Uri).ToArray();

            SetConstructSize(ConstructType);

            var uri = ConstructExtensions.GetRandomContentUri(_tree_uris);
            _content_image = new(uri: uri, width: this.Width, height: this.Height);

            SetChild(_content_image);

            Speed = Constants.DEFAULT_CONSTRUCT_SPEED;
            IsometricDisplacement = Constants.DEFAULT_ISOMETRIC_DISPLACEMENT;
            DropShadowDistance = -5;
        }

        #endregion
    }
}
