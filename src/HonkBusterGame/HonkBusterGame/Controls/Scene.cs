using Microsoft.UI;
using System.Diagnostics;
using Windows.Foundation;

namespace HonkBusterGame
{
    public partial class Scene : Border
    {
        #region Fields

        private readonly Canvas _canvas;

        //private readonly Storyboard _opacity_storyboard;
        //private readonly DoubleAnimation _doubleAnimation;

        private readonly CompositeTransform _transform = new()
        {
            CenterX = 0.5,
            CenterY = 0.5,
            Rotation = 0,
            ScaleX = 1,
            ScaleY = 1,
        };

        private PeriodicTimer _gameViewTimer;
        private readonly TimeSpan _frameTime = TimeSpan.FromMilliseconds(Constants.DEFAULT_FRAME_TIME);

        private readonly List<Construct> _destroyables = new();
        private readonly List<Generator> _generators = new();

        private double _slowMotionDelay;
        private readonly double _slowMotionDelayDefault = 160;

#if DEBUG
        private Stopwatch _stopwatch;
        private TimeSpan _lastElapsed;
        private int _famesCount;
#endif
        #endregion

        #region Ctor

        public Scene()
        {
            #region Opacity Animation

            //_doubleAnimation = new DoubleAnimation()
            //{
            //    Duration = new Duration(TimeSpan.FromSeconds(4)),
            //    From = 0,
            //    To = 1,
            //};

            //Storyboard.SetTarget(_doubleAnimation, this);
            //Storyboard.SetTargetProperty(_doubleAnimation, "Opacity");

            //_opacity_storyboard = new Storyboard();
            //_opacity_storyboard.Children.Add(_doubleAnimation);
            //_opacity_storyboard.Completed += (s, e) =>
            //{
            //    _opacity_storyboard.Stop();
            //    LoggingExtensions.Log($"Scene: _opacity_storyboard -> Completed");
            //};

            #endregion

            CanDrag = false;
            Children = new List<Construct>();

            _canvas = new()
            {
                RenderTransformOrigin = new Point(0, 0),
                RenderTransform = _transform,
                Background = new SolidColorBrush(Colors.Transparent),
            };

            SceneState = SceneState.GAME_STOPPED;

            //Loaded += Scene_Loaded;
            Unloaded += Scene_Unloaded;

            this.Child = _canvas;
        }

        #endregion

        #region Properties

        public bool IsAnimating { get; set; }

        public bool IsInNightMode { get; set; }

        public SceneState SceneState { get; set; }

        public List<Construct> Children { get; set; }

        public bool IsSlowMotionActivated => _slowMotionDelay > 0;

        public bool GeneratorsExist => _generators.Any();

        #endregion

        #region Events

        //private void Scene_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //_opacity_storyboard.Begin();
        //}

        private void Scene_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        #endregion

        #region Methods

        public void ToggleNightMode(bool isNightMode)
        {
            IsInNightMode = isNightMode;
        }

        public void SetRenderTransformOrigin(double xy)
        {
            _canvas.RenderTransformOrigin = new Point(xy, xy);
        }

        public void SetScaleTransform(double scaleXY)
        {
            _transform.ScaleX = scaleXY;
            _transform.ScaleY = scaleXY;
        }

        public void AddToScene(params Construct[] constructs)
        {
            if (constructs is not null)
            {
                foreach (var construct in constructs)
                {
                    construct.Scene = this;
                    //_canvas.Children.Add(construct.Child);
                    Children.Add(construct);
                }
            }
        }

        public void AddToScene(params Generator[] generators)
        {
            if (generators is not null)
            {
                foreach (var generator in generators)
                {
                    generator.Scene = this;
                    _generators.Add(generator);
                }
            }
        }

        public void DisposeFromScene(Construct construct)
        {
            _destroyables.Add(construct);
        }

        public async void Play()
        {
            if (!IsAnimating)
            {
                IsAnimating = true;
#if DEBUG
                _stopwatch = Stopwatch.StartNew();
                _famesCount = 0;
                _lastElapsed = TimeSpan.FromSeconds(0);
#endif
                _gameViewTimer = new PeriodicTimer(_frameTime);

                Children = Children.OrderBy(x => x.Z).ToList();

                while (await _gameViewTimer.WaitForNextTickAsync())
                {
                    UpdateFrame();
                }
            }
        }

        public void Pause()
        {
            if (IsAnimating)
            {
                IsAnimating = false;
#if DEBUG
                _stopwatch?.Reset();
#endif
                _gameViewTimer?.Dispose();
            }
        }

        public void SetState(SceneState sceneState)
        {
            SceneState = sceneState;
        }

        public void Stop()
        {
            IsAnimating = false;
#if DEBUG
            _stopwatch?.Stop();
#endif
            _gameViewTimer?.Dispose();

            Clear();
        }

        public void Clear()
        {
            Children.Clear();

            _canvas.Children.Clear();

            _generators.Clear();
            _destroyables.Clear();

#if DEBUG
            _stopwatch?.Stop();
#endif
            _gameViewTimer?.Dispose();
        }

        private void UpdateFrame()
        {
            // generate new constructs in scene from generators
            foreach (Generator generator in _generators)
            {
                generator.Generate();
            }

            foreach (Construct construct in Children.Where(x => x.IsAnimating))
            {
                if (!_canvas.Children.Contains(construct.Content))
                {
                    _canvas.Children.Add(construct.Content);
                }

                construct.Animate();
                construct.Update();
                construct.Recycle();

                //if (!construct.IsAnimating)
                //{
                //    DisposeFromScene(construct);
                //}
            }

            // remove the destroyables from the scene
            foreach (Construct destroyable in _destroyables)
            {
                _canvas.Children.Remove(destroyable.Content);
            }

            _destroyables.Clear();

            DepleteSlowMotion();

#if DEBUG
            _famesCount++;

            if (_stopwatch.Elapsed - _lastElapsed > TimeSpan.FromSeconds(2))
            {
                _lastElapsed = _stopwatch.Elapsed;

                var fps = _famesCount / 2;

                LoggingExtensions.Log($"Scene: {Name} ~ Generators: {_generators.Count} ~ Animating Objects: {Children.Count(x => x.IsAnimating)} \n Rendered Objects: {_canvas.Children.Count} \n Total Objects: {Children.Count} \n FPS: {fps}");

                _famesCount = 0;
            }
#endif
        }

        public void ActivateSlowMotion()
        {
            if (!IsSlowMotionActivated)
            {
                _slowMotionDelay = _slowMotionDelayDefault;
            }
        }

        private void DepleteSlowMotion()
        {
            if (_slowMotionDelay > 0)
            {
                _slowMotionDelay--;
            }
        }

        #endregion
    }

    public enum SceneState
    {
        GAME_STOPPED, GAME_RUNNING,
    }
}
