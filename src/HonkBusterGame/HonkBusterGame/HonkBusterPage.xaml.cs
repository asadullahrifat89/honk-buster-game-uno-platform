using Microsoft.UI;
using Windows.Graphics.Display;

namespace HonkBusterGame
{
    public sealed partial class HonkBusterPage : Page
    {
        #region Fields

        private readonly Random _random;

        private readonly GameView _gameView;
        private readonly GameView _mainMenuScene;
        private readonly GameController _gameController;

        private readonly HealthBar _playerHealthBar;

        private readonly HealthBar _ufoBossHealthBar;
        private readonly HealthBar _vehicleBossHealthBar;
        private readonly HealthBar _zombieBossHealthBar;
        private readonly HealthBar _mafiaBossHealthBar;

        private readonly HealthBar _powerUpMeter;
        private readonly HealthBar _soundPollutionMeter;

        private readonly ScoreBar _gameScoreBar;
        private readonly StackPanel _healthBars;

        private readonly GameCheckpoint _ufoBossCheckpoint;
        private readonly GameCheckpoint _vehicleBossCheckpoint;
        private readonly GameCheckpoint _zombieBossCheckpoint;
        private readonly GameCheckpoint _mafiaBossCheckpoint;

        private readonly GameCheckpoint _ufoEnemyCheckpoint;

        private readonly double _soundPollutionMaxLimit = 6; // max 3 vehicles or ufos honking to trigger sound pollution limit

        //TODO: set defaults _vehicleReleasePoint = 25
        private readonly double _vehicleBossReleasePoint = 25; // first appearance
        private readonly double _vehicleBossReleasePoint_increase = 15;

        //TODO: set defaults _ufoBossReleasePoint = 50
        private readonly double _ufoBossReleasePoint = 50; // first appearance
        private readonly double _ufoBossReleasePoint_increase = 15;

        //TODO: set defaults _zombieBossReleasePoint = 75
        private readonly double _zombieBossReleasePoint = 75; // first appearance
        private readonly double _zombieBossReleasePoint_increase = 15;

        //TODO: set defaults _mafiaBossReleasePoint = 100
        private readonly double _mafiaBossReleasePoint = 100; // first appearance
        private readonly double _mafiaBossReleasePoint_increase = 15;

        //TODO: set defaults _ufoEnemyReleasePoint = 35
        private readonly double _ufoEnemyReleasePoint = 35; // first appearance
        private readonly double _ufoEnemyReleasePoint_increase = 5;

        private double _ufoEnemyKillCount;
        private readonly double _ufoEnemyKillCount_limit = 20;

        private bool _ufoEnemyFleetAppeared;

        private PlayerBalloon _player;
        private PlayerBalloonTemplate _selectedPlayerTemplate;
        private PlayerHonkBombTemplate _selectedPlayerHonkBombTemplate;
        private int _gameLevel;

        private readonly AudioStub _audioStub;

        #endregion

        #region Ctor

        public HonkBusterPage()
        {
            this.InitializeComponent();

            _gameView = this.GameScene;
            _mainMenuScene = this.MainMenuScene;

            _playerHealthBar = this.PlayerHealthBar;

            _ufoBossHealthBar = this.UfoBossHealthBar;
            _zombieBossHealthBar = this.ZombieBossHealthBar;
            _vehicleBossHealthBar = this.VehicleBossHealthBar;
            _mafiaBossHealthBar = this.MafiaBossHealthBar;

            _powerUpMeter = this.PowerUpHealthBar;
            _soundPollutionMeter = this.SoundPollutionBar;

            _gameController = this.GameController;

            _gameScoreBar = this.GameScoreBar;
            _healthBars = this.HealthBars;

            _ufoBossCheckpoint = new GameCheckpoint(_ufoBossReleasePoint);
            _zombieBossCheckpoint = new GameCheckpoint(_zombieBossReleasePoint);
            _vehicleBossCheckpoint = new GameCheckpoint(_vehicleBossReleasePoint);
            _mafiaBossCheckpoint = new GameCheckpoint(_mafiaBossReleasePoint);

            _ufoEnemyCheckpoint = new GameCheckpoint(_ufoEnemyReleasePoint);

            ToggleHudVisibility(Visibility.Collapsed);

            _random = new Random();

            _audioStub = new AudioStub(
                (SoundType.GAME_BACKGROUND_MUSIC, 0.5, true),
                (SoundType.BOSS_BACKGROUND_MUSIC, 0.5, true),
                (SoundType.AMBIENCE, 0.6, true),
                (SoundType.GAME_START, 1, false),
                (SoundType.GAME_PAUSE, 1, false),
                (SoundType.GAME_OVER, 1, false),
                (SoundType.UFO_ENEMY_ENTRY, 1, false));

            ScreenExtensions.Width = Constants.DEFAULT_SCENE_WIDTH;
            ScreenExtensions.Height = Constants.DEFAULT_SCENE_HEIGHT;

            _mainMenuScene.SetRenderTransformOrigin(0.5);

            SetSceneScaling();

            Loaded += HonkBusterPage_Loaded;
            Unloaded += HonkBusterPage_Unloaded;
        }

        #endregion

        #region Methods

        #region Game

        private bool PauseGame()
        {
            _audioStub.Play(SoundType.GAME_PAUSE);

            _audioStub.Pause(SoundType.AMBIENCE);

            if (AnyBossExists())
            {
                _audioStub.Pause(SoundType.BOSS_BACKGROUND_MUSIC);
            }
            else
            {
                _audioStub.Pause(SoundType.GAME_BACKGROUND_MUSIC);
            }

            ToggleHudVisibility(Visibility.Collapsed);

            _gameView.Pause();
            _mainMenuScene.Play();

            _gameController.DeactivateGyrometerReading();
            _gameController.SetDefaultThumbstickPosition();

            GenerateGameStartScreen(title: "Game Paused", subTitle: "-Taking a break-");

            return true;
        }

        private void ResumeGame()
        {
            _audioStub.Resume(SoundType.AMBIENCE);

            if (AnyBossExists())
            {
                _audioStub.Resume(SoundType.BOSS_BACKGROUND_MUSIC);
            }
            else
            {
                _audioStub.Resume(SoundType.GAME_BACKGROUND_MUSIC);
            }

            ToggleHudVisibility(Visibility.Visible);

            _gameView.Play();
            _mainMenuScene.Pause();

            _gameController.ActivateGyrometerReading();
            _gameController.FocusAttackButton();
        }

        private void NewGame()
        {
            LoggingExtensions.Log("New game dtarted.");

            if (_gameView.IsNightModeActivated)
                ToggleNightMode(false);

            _gameLevel = 0;

            _audioStub.Play(SoundType.AMBIENCE, SoundType.GAME_BACKGROUND_MUSIC);

            _gameController.Reset();

            _gameScoreBar.Reset();
            _powerUpMeter.Reset();

            _ufoBossHealthBar.Reset();
            _vehicleBossHealthBar.Reset();
            _zombieBossHealthBar.Reset();
            _mafiaBossHealthBar.Reset();

            _soundPollutionMeter.Reset();
            _soundPollutionMeter.SetMaxiumHealth(_soundPollutionMaxLimit);
            _soundPollutionMeter.SetIcon(Constants.CONSTRUCT_TEMPLATES.FirstOrDefault(x => x.ConstructType == ConstructType.HONK).Uri);
            _soundPollutionMeter.SetBarColor(color: Colors.Purple);

            _ufoBossCheckpoint.Reset(_ufoBossReleasePoint);
            _vehicleBossCheckpoint.Reset(_vehicleBossReleasePoint);
            _zombieBossCheckpoint.Reset(_zombieBossReleasePoint);
            _mafiaBossCheckpoint.Reset(_mafiaBossReleasePoint);

            _ufoEnemyCheckpoint.Reset(_ufoEnemyReleasePoint);
            _ufoEnemyKillCount = 0;
            _ufoEnemyFleetAppeared = false;

            SetupSetPlayerBalloon();

            GeneratePlayerBalloon();
            RepositionConstructs();

            _gameView.GameViewState = GameViewState.GAME_RUNNING;
            _gameView.Play();

            _mainMenuScene.Pause();

            ToggleHudVisibility(Visibility.Visible);

            _gameController.FocusAttackButton();
            _gameController.SetDefaultThumbstickPosition();
            _gameController.ActivateGyrometerReading();
        }

        private void SetupSetPlayerBalloon()
        {
            _player.SetPlayerTemplate(_selectedPlayerTemplate); // change player template

            foreach (var honkBomb in _gameView.GameObjects.OfType<PlayerHonkBomb>()) // change player honk bomb template
            {
                honkBomb.SetHonkBombTemplate(_selectedPlayerHonkBombTemplate);
            }
        }

        private void GameOver()
        {
            // if player is dead game keeps playing in the background but scene state goes to game over
            if (_player.IsDead)
            {
                _audioStub.Stop(SoundType.AMBIENCE, SoundType.GAME_BACKGROUND_MUSIC, SoundType.BOSS_BACKGROUND_MUSIC);

                if (_gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating) is UfoBoss ufoBoss)
                {
                    ufoBoss.SetWinStance();
                    ufoBoss.StopSoundLoop();
                }

                _audioStub.Play(SoundType.GAME_OVER);

                _mainMenuScene.Play();
                _gameView.GameViewState = GameViewState.GAME_STOPPED;

                ToggleHudVisibility(Visibility.Collapsed);
                GenerateGameStartScreen(title: "Game Over", subTitle: $"-Score: {_gameScoreBar.GetScore():0000} Level: {_gameLevel}-");

                _gameController.DeactivateGyrometerReading();
            }
        }

        private void RepositionConstructs()
        {
            foreach (var construct in _gameView.GameObjects.OfType<GameObject>()
                .Where(x => x.ConstructType is
                ConstructType.VEHICLE_ENEMY_LARGE or
                ConstructType.VEHICLE_ENEMY_SMALL or
                ConstructType.VEHICLE_BOSS or
                ConstructType.UFO_BOSS or
                ConstructType.ZOMBIE_BOSS or
                ConstructType.MAFIA_BOSS or
                ConstructType.HONK or
                ConstructType.PLAYER_ROCKET or
                ConstructType.PLAYER_ROCKET_SEEKING or
                ConstructType.PLAYER_HONK_BOMB or
                ConstructType.UFO_BOSS_ROCKET or
                ConstructType.UFO_BOSS_ROCKET_SEEKING or
                ConstructType.UFO_ENEMY or
                ConstructType.UFO_ENEMY_ROCKET or
                ConstructType.VEHICLE_BOSS_ROCKET or
                ConstructType.MAFIA_BOSS_ROCKET or
                ConstructType.MAFIA_BOSS_ROCKET_BULLS_EYE or
                ConstructType.POWERUP_PICKUP or
                ConstructType.HEALTH_PICKUP or
                ConstructType.FLOATING_NUMBER))
            {
                construct.IsAnimating = false;

                if (construct is VehicleBoss vehicleboss)
                {
                    vehicleboss.IsAttacking = false;
                    vehicleboss.Health = 0;
                }

                if (construct is UfoBoss ufoBoss)
                {
                    ufoBoss.IsAttacking = false;
                    ufoBoss.Health = 0;
                }

                if (construct is ZombieBoss zombieBoss)
                {
                    zombieBoss.IsAttacking = false;
                    zombieBoss.Health = 0;
                }

                if (construct is MafiaBoss mafiaBoss)
                {
                    mafiaBoss.IsAttacking = false;
                    mafiaBoss.Health = 0;
                }
            }
        }

        private void RepositionHoveringTitleScreens()
        {
            foreach (var screen in _mainMenuScene.GameObjects.OfType<HoveringTitleScreen>().Where(x => x.IsAnimating))
            {
                screen.Reposition();
            }
        }

        private void LevelUp()
        {
            _gameLevel++;
            GenerateInterimScreen($"LEVEL {_gameLevel} COMPLETE");
        }

        private void ToggleNightMode(bool isNightMode)
        {
            _gameView.ToggleNightMode(isNightMode);

            if (_gameView.IsNightModeActivated)
            {
                this.NightToDayStoryboard.Stop();
                this.DayToNightStoryboard.Begin();
            }
            else
            {
                this.DayToNightStoryboard.Stop();
                this.NightToDayStoryboard.Begin();
            }
        }

        private async Task OpenGame()
        {
            _gameView.Play();
            _mainMenuScene.Play();

            ToggleNightMode(false);

            await Task.Delay(500);

            GenerateGameStartScreen(title: "Honk Buster", subTitle: "-Stop Honkers, Save The City-");

            _audioStub.Play(SoundType.GAME_BACKGROUND_MUSIC);
        }

        #endregion

        #region Screens

        #region PromptOrientationChangeScreen

        private void SpawnPromptOrientationChangeScreen()
        {
            PromptOrientationChangeScreen promptOrientationChangeScreen = null;

            promptOrientationChangeScreen = new(
                animateAction: AnimatePromptOrientationChangeScreen,
                recycleAction: (se) => { });

            promptOrientationChangeScreen.SetZ(z: 10);
            promptOrientationChangeScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(promptOrientationChangeScreen);
        }

        private void GeneratePromptOrientationChangeScreen()
        {
            if (_mainMenuScene.GameObjects.OfType<PromptOrientationChangeScreen>().FirstOrDefault(x => x.IsAnimating == false) is PromptOrientationChangeScreen promptOrientationChangeScreen)
            {
                promptOrientationChangeScreen.Reposition();
                promptOrientationChangeScreen.IsAnimating = true;
            }
        }

        private void AnimatePromptOrientationChangeScreen(GameObject promptOrientationChangeScreen)
        {
            PromptOrientationChangeScreen screen1 = promptOrientationChangeScreen as PromptOrientationChangeScreen;
            screen1.Hover();
        }

        private void RecyclePromptOrientationChangeScreen(PromptOrientationChangeScreen promptOrientationChangeScreen)
        {
            promptOrientationChangeScreen.IsAnimating = false;
        }

        #endregion

        #region AssetsLoadingScreen

        private void SpawnAssetsLoadingScreen()
        {
            AssetsLoadingScreen assetsLoadingScreen = null;

            assetsLoadingScreen = new(
                animateAction: AnimateAssetsLoadingScreen,
                recycleAction: (se) => { });

            assetsLoadingScreen.SetZ(z: 10);
            assetsLoadingScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(assetsLoadingScreen);
        }

        private void GenerateAssetsLoadingScreen()
        {
            if (_mainMenuScene.GameObjects.OfType<AssetsLoadingScreen>().FirstOrDefault(x => x.IsAnimating == false) is AssetsLoadingScreen assetsLoadingScreen)
            {
                assetsLoadingScreen.Reposition();
                assetsLoadingScreen.SetSubTitle($"... Loading Assets ...");
                assetsLoadingScreen.IsAnimating = true;

                _ = assetsLoadingScreen.PreloadAssets(async () =>
                {
                    RecycleAssetsLoadingScreen(assetsLoadingScreen);

                    if (ScreenExtensions.IsScreenInRequiredOrientation())
                    {
                        if (!_gameView.GameObjectGeneratorsAdded)
                        {
                            PrepareGameView();
                            await Task.Delay(500);
                            await OpenGame();
                        }
                        else
                        {
                            await OpenGame();
                        }
                    }
                    else
                    {
                        GeneratePromptOrientationChangeScreen();
                    }
                });
            }
        }

        private void AnimateAssetsLoadingScreen(GameObject assetsLoadingScreen)
        {
            AssetsLoadingScreen screen1 = assetsLoadingScreen as AssetsLoadingScreen;
            screen1.Hover();
        }

        private void RecycleAssetsLoadingScreen(AssetsLoadingScreen assetsLoadingScreen)
        {
            assetsLoadingScreen.IsAnimating = false;
        }

        #endregion

        #region GameStartScreen

        private void SpawnGameStartScreen()
        {
            GameStartScreen gameStartScreen = null;

            gameStartScreen = new(
                animateAction: AnimateGameStartScreen,
                recycleAction: (se) => { },
                playAction: () =>
                {
                    if (_gameView.GameViewState == GameViewState.GAME_STOPPED)
                    {
                        if (ScreenExtensions.IsScreenInRequiredOrientation())
                        {
                            RecycleGameStartScreen(gameStartScreen);
                            GeneratePlayerCharacterSelectionScreen();
                            ScreenExtensions.EnterFullScreen(true);
                        }
                        else
                        {
                            ScreenExtensions.ChangeDisplayOrientationAsRequired();
                        }
                    }
                    else
                    {
                        if (!_gameView.IsAnimating)
                        {
                            if (ScreenExtensions.IsScreenInRequiredOrientation())
                            {
                                ResumeGame();
                                RecycleGameStartScreen(gameStartScreen);
                            }
                            else
                            {
                                ScreenExtensions.ChangeDisplayOrientationAsRequired();
                            }
                        }
                    }
                });

            gameStartScreen.SetZ(z: 10);
            gameStartScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(gameStartScreen);
        }

        private void GenerateGameStartScreen(string title, string subTitle = "")
        {
            if (_mainMenuScene.GameObjects.OfType<GameStartScreen>().FirstOrDefault(x => x.IsAnimating == false) is GameStartScreen gameStartScreen)
            {
                gameStartScreen.SetTitle(title);
                gameStartScreen.SetSubTitle(subTitle);
                gameStartScreen.Reposition();
                gameStartScreen.Reset();
                gameStartScreen.IsAnimating = true;
                gameStartScreen.SetContent(ConstructExtensions.GetRandomContentUri(Constants.CONSTRUCT_TEMPLATES.Where(x => x.ConstructType == ConstructType.PLAYER_BALLOON).Select(x => x.Uri).ToArray()));
            }
        }

        private void AnimateGameStartScreen(GameObject gameStartScreen)
        {
            GameStartScreen screen1 = gameStartScreen as GameStartScreen;
            screen1.Hover();
        }

        private void RecycleGameStartScreen(GameStartScreen gameStartScreen)
        {
            gameStartScreen.IsAnimating = false;
        }

        #endregion

        #region PlayerCharacterSelectionScreen

        private void SpawnPlayerCharacterSelectionScreen()
        {
            PlayerCharacterSelectionScreen playerCharacterSelectionScreen = null;

            playerCharacterSelectionScreen = new(
                animateAction: AnimatePlayerCharacterSelectionScreen,
                recycleAction: (se) => { },
                playAction: (int playerTemplate) =>
                {
                    _selectedPlayerTemplate = (PlayerBalloonTemplate)playerTemplate;

                    RecyclePlayerCharacterSelectionScreen(playerCharacterSelectionScreen);
                    GeneratePlayerHonkBombSelectionScreen();
                },
                backAction: () =>
                {
                    RecyclePlayerCharacterSelectionScreen(playerCharacterSelectionScreen);
                    GenerateGameStartScreen(title: "Honk Buster", subTitle: "-Stop Honkers, Save The City-");
                });

            playerCharacterSelectionScreen.SetZ(z: 10);
            playerCharacterSelectionScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(playerCharacterSelectionScreen);
        }

        private void GeneratePlayerCharacterSelectionScreen()
        {
            if (_mainMenuScene.GameObjects.OfType<PlayerCharacterSelectionScreen>().FirstOrDefault(x => x.IsAnimating == false) is PlayerCharacterSelectionScreen playerCharacterSelectionScreen)
            {
                //playerCharacterSelectionScreen.Reset();
                playerCharacterSelectionScreen.Reposition();
                playerCharacterSelectionScreen.IsAnimating = true;
            }
        }

        private void AnimatePlayerCharacterSelectionScreen(GameObject playerCharacterSelectionScreen)
        {
            PlayerCharacterSelectionScreen screen1 = playerCharacterSelectionScreen as PlayerCharacterSelectionScreen;
            screen1.Hover();
        }

        private void RecyclePlayerCharacterSelectionScreen(PlayerCharacterSelectionScreen playerCharacterSelectionScreen)
        {
            playerCharacterSelectionScreen.IsAnimating = false;
        }

        #endregion

        #region PlayerHonkBombSelectionScreen

        private void SpawnPlayerHonkBombSelectionScreen()
        {
            PlayerHonkBombSelectionScreen playerHonkBombSelectionScreen = null;

            playerHonkBombSelectionScreen = new(
                animateAction: AnimatePlayerHonkBombSelectionScreen,
                recycleAction: (se) => { },
                playAction: (int playerTemplate) =>
                {
                    _selectedPlayerHonkBombTemplate = (PlayerHonkBombTemplate)playerTemplate;

                    if (_gameView.GameViewState == GameViewState.GAME_STOPPED)
                    {
                        RecyclePlayerHonkBombSelectionScreen(playerHonkBombSelectionScreen);
                        NewGame();
                    }
                },
                backAction: () =>
                {
                    RecyclePlayerHonkBombSelectionScreen(playerHonkBombSelectionScreen);
                    GeneratePlayerCharacterSelectionScreen();
                });

            playerHonkBombSelectionScreen.SetZ(z: 10);
            playerHonkBombSelectionScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(playerHonkBombSelectionScreen);
        }

        private void GeneratePlayerHonkBombSelectionScreen()
        {
            if (_mainMenuScene.GameObjects.OfType<PlayerHonkBombSelectionScreen>().FirstOrDefault(x => x.IsAnimating == false) is PlayerHonkBombSelectionScreen playerHonkBombSelectionScreen)
            {
                //playerHonkBombSelectionScreen.Reset();
                playerHonkBombSelectionScreen.Reposition();
                playerHonkBombSelectionScreen.IsAnimating = true;
            }
        }

        private void AnimatePlayerHonkBombSelectionScreen(GameObject playerHonkBombSelectionScreen)
        {
            PlayerHonkBombSelectionScreen screen1 = playerHonkBombSelectionScreen as PlayerHonkBombSelectionScreen;
            screen1.Hover();
        }

        private void RecyclePlayerHonkBombSelectionScreen(PlayerHonkBombSelectionScreen playerHonkBombSelectionScreen)
        {
            playerHonkBombSelectionScreen.IsAnimating = false;
        }

        #endregion

        #region InterimScreen

        private void SpawnInterimScreen()
        {
            InterimScreen interimScreen = null;

            interimScreen = new(
                animateAction: AnimateInterimScreen,
                recycleAction: RecycleInterimScreen);

            interimScreen.SetZ(z: 10);
            interimScreen.MoveOutOfSight();

            _mainMenuScene.AddToView(interimScreen);
        }

        private void GenerateInterimScreen(string title)
        {
            if (_mainMenuScene.GameObjects.OfType<InterimScreen>().FirstOrDefault(x => x.IsAnimating == false) is InterimScreen interimScreen)
            {
                interimScreen.SetTitle(title);
                interimScreen.Reset();
                interimScreen.Reposition();
                interimScreen.IsAnimating = true;

                _mainMenuScene.Play();
            }
        }

        private void AnimateInterimScreen(GameObject interimScreen)
        {
            InterimScreen screen1 = interimScreen as InterimScreen;
            screen1.Hover();
            screen1.DepleteOnScreenDelay();
        }

        private void RecycleInterimScreen(GameObject interimScreen)
        {
            if (interimScreen is InterimScreen interimScreen1 && interimScreen1.IsDepleted)
            {
                interimScreen.IsAnimating = false;
                _mainMenuScene.Pause();
            }
        }

        #endregion

        #endregion

        #region GameContainers

        #region RoadMarks

        public void SpawnRoadMarksContainer()
        {
            var roadMarkSize = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.ROAD_MARK);
            int numberOfRoadMarks = 5;

            for (int j = 0; j < 2; j++)
            {
                GameObjectContainer roadMarkContainer = new(
                    animateAction: AnimateRoadMarksContainer,
                    recycleAction: RecycleRoadMarksContainer)
                {
                    Speed = Constants.DEFAULT_CONSTRUCT_SPEED,
                    ConstructType = ConstructType.ROAD_MARK,
                };

                roadMarkContainer.SetSize(
                    width: roadMarkSize.Width * numberOfRoadMarks,
                    height: (roadMarkSize.Height / 2) * numberOfRoadMarks);

                for (int i = 0; i < numberOfRoadMarks; i++)
                {
                    RoadMark roadMark = new(
                        animateAction: (roadMark) => { },
                        recycleAction: (roadMark) => { });

                    roadMark.SetPosition(
                      left: (roadMarkSize.Width * i),
                      top: ((roadMarkSize.Height / 2) * i));

                    roadMark.Render();

                    roadMarkContainer.AddChild(roadMark);
                }

                roadMarkContainer.MoveOutOfSight();
                _gameView.AddToView(roadMarkContainer);
            }
        }

        public void GenerateRoadMarksContainer()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_MARK) is GameObjectContainer roadMarkContainer)
            {
                roadMarkContainer.SetPosition(
                  left: roadMarkContainer.Width * -1,
                  top: roadMarkContainer.Height * -1.1);

                roadMarkContainer.IsAnimating = true;
            }
        }

        private void AnimateRoadMarksContainer(GameObjectContainer roadMarkContainer)
        {
            var speed = roadMarkContainer.Speed;
            roadMarkContainer.MoveDownRight(speed);
        }

        private void RecycleRoadMarksContainer(GameObjectContainer roadMarkContainer)
        {
            var hitBox = roadMarkContainer.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadMarkContainer.IsAnimating = false;
            }
        }

        #endregion

        #region RoadSideWalks

        public void SpawnRoadSideWalksContainer()
        {
            var roadSideWalkSize = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.ROAD_SIDE_WALK);
            int numberOfRoadSideWalks = 5;
            double xyAdjustment = 33;

            for (int j = 0; j < 5; j++)
            {
                GameObjectContainer roadSideWalkContainer = new(
                    animateAction: AnimateRoadSideWalksContainer,
                    recycleAction: RecycleRoadSideWalksContainer)
                {
                    Speed = Constants.DEFAULT_CONSTRUCT_SPEED,
                    ConstructType = ConstructType.ROAD_SIDE_WALK,
                };

                roadSideWalkContainer.SetSize(
                    width: roadSideWalkSize.Width * numberOfRoadSideWalks,
                    height: (roadSideWalkSize.Height / 2) * numberOfRoadSideWalks);

                for (int i = 0; i < numberOfRoadSideWalks; i++)
                {
                    RoadSideWalk roadSideWalk = new(
                        animateAction: (roadSideWalk) => { },
                        recycleAction: (roadSideWalk) => { });

                    roadSideWalk.SetPosition(
                      left: (roadSideWalkSize.Width * i - (xyAdjustment * i)),
                      top: ((roadSideWalkSize.Height / 2) * i - ((xyAdjustment / 2) * i)));

                    roadSideWalk.Render();
                    roadSideWalkContainer.AddChild(roadSideWalk);
                }

                roadSideWalkContainer.MoveOutOfSight();
                _gameView.AddToView(roadSideWalkContainer);
            }
        }

        public void GenerateRoadSideWalksContainerTop()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_WALK) is GameObjectContainer roadSideWalkContainerTop)
            {
                roadSideWalkContainerTop.SetPosition(
                  left: ((Constants.DEFAULT_SCENE_WIDTH / 5) * -1) - 300,
                  top: (roadSideWalkContainerTop.Height * -1) - 150,
                  z: 0);

                roadSideWalkContainerTop.IsAnimating = true;
            }
        }

        public void GenerateRoadSideWalksContainerBottom()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_WALK) is GameObjectContainer roadSideWalkContainerBottom)
            {
                roadSideWalkContainerBottom.SetPosition(
                  left: (roadSideWalkContainerBottom.Width * -1.1),
                  top: (Constants.DEFAULT_SCENE_HEIGHT / 2.8) * -1,
                  z: 0);

                roadSideWalkContainerBottom.IsAnimating = true;
            }
        }

        private void AnimateRoadSideWalksContainer(GameObjectContainer roadContainer)
        {
            var speed = roadContainer.Speed;
            roadContainer.MoveDownRight(speed);
        }

        private void RecycleRoadSideWalksContainer(GameObjectContainer roadContainer)
        {
            var hitBox = roadContainer.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - 150 > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadContainer.IsAnimating = false;
            }
        }

        #endregion

        #region RoadSideTrees

        public void SpawnRoadSideTreesContainer()
        {
            var roadSideTreeSize = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.ROAD_SIDE_TREE);
            int numberOfRoadSideTrees = 5;
            double xyAdjustment = 31.5;

            for (int j = 0; j < 5; j++)
            {
                GameObjectContainer roadSideTreeContainer = new(
                    animateAction: AnimateRoadSideTreesContainer,
                    recycleAction: RecycleRoadSideTreesContainer)
                {
                    Speed = Constants.DEFAULT_CONSTRUCT_SPEED,
                    ConstructType = ConstructType.ROAD_SIDE_TREE,
                };

                roadSideTreeContainer.SetSize(
                    width: roadSideTreeSize.Width * numberOfRoadSideTrees,
                    height: (roadSideTreeSize.Height / 2) * numberOfRoadSideTrees);

                for (int i = 0; i < numberOfRoadSideTrees; i++)
                {
                    RoadSideTree roadSideTree = new(
                        animateAction: (roadSideTree) => { },
                        recycleAction: (roadSideTree) => { });

                    roadSideTree.SetPosition(
                      left: (roadSideTreeSize.Width * i - (xyAdjustment * i)),
                      top: ((roadSideTreeSize.Height / 2) * i - ((xyAdjustment / 2) * i)));

                    roadSideTree.Render();
                    roadSideTreeContainer.AddChild(roadSideTree);
                }

                roadSideTreeContainer.MoveOutOfSight();
                _gameView.AddToView(roadSideTreeContainer);
            }
        }

        public void GenerateRoadSideTreesContainerTop()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_TREE) is GameObjectContainer roadSideTreeContainerTop)
            {
                roadSideTreeContainerTop.SetPosition(
                  left: ((Constants.DEFAULT_SCENE_WIDTH / 2.0) * -1) - 300,
                  top: (roadSideTreeContainerTop.Height * -1) - 150,
                  z: 3);

                roadSideTreeContainerTop.IsAnimating = true;
            }
        }

        public void GenerateRoadSideTreesContainerBottom()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_TREE) is GameObjectContainer roadSideTreeContainerBottom)
            {
                roadSideTreeContainerBottom.SetPosition(
                  left: (roadSideTreeContainerBottom.Width * -1.1),
                  top: ((Constants.DEFAULT_SCENE_HEIGHT) * -1),
                  z: 7);

                roadSideTreeContainerBottom.IsAnimating = true;
            }
        }

        private void AnimateRoadSideTreesContainer(GameObjectContainer roadContainer)
        {
            var speed = roadContainer.Speed;
            roadContainer.MoveDownRight(speed);
        }

        private void RecycleRoadSideTreesContainer(GameObjectContainer roadContainer)
        {
            var hitBox = roadContainer.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - 150 > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadContainer.IsAnimating = false;
            }
        }

        #endregion

        #region RoadSideHedges

        public void SpawnRoadSideHedgesContainer()
        {
            var roadSideHedgeSize = Constants.CONSTRUCT_SIZES.FirstOrDefault(x => x.ConstructType == ConstructType.ROAD_SIDE_HEDGE);
            int numberOfRoadSideHedges = 5;
            double xyAdjustment = 31.5;

            for (int j = 0; j < 5; j++)
            {
                GameObjectContainer roadSideHedgeContainer = new(
                    animateAction: AnimateRoadSideHedgesContainer,
                    recycleAction: RecycleRoadSideHedgesContainer)
                {
                    Speed = Constants.DEFAULT_CONSTRUCT_SPEED,
                    ConstructType = ConstructType.ROAD_SIDE_HEDGE,
                };

                roadSideHedgeContainer.SetSize(
                    width: roadSideHedgeSize.Width * numberOfRoadSideHedges,
                    height: (roadSideHedgeSize.Height / 2) * numberOfRoadSideHedges);

                for (int i = 0; i < numberOfRoadSideHedges; i++)
                {
                    RoadSideHedge roadSideHedge = new(
                        animateAction: (roadSideHedge) => { },
                        recycleAction: (roadSideHedge) => { });

                    roadSideHedge.SetPosition(
                      left: (roadSideHedgeSize.Width * i - (xyAdjustment * i)),
                      top: ((roadSideHedgeSize.Height / 2) * i - ((xyAdjustment / 2) * i)));

                    roadSideHedge.Render();
                    roadSideHedgeContainer.AddChild(roadSideHedge);
                }

                roadSideHedgeContainer.MoveOutOfSight();
                _gameView.AddToView(roadSideHedgeContainer);
            }
        }

        public void GenerateRoadSideHedgesContainerTop()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_HEDGE) is GameObjectContainer roadSideHedgeContainerTop)
            {
                roadSideHedgeContainerTop.SetPosition(
                  left: ((Constants.DEFAULT_SCENE_WIDTH / 1.5) * -1) - 270,
                  top: (roadSideHedgeContainerTop.Height * -1) - 150,
                  z: 3);

                roadSideHedgeContainerTop.IsAnimating = true;
            }
        }

        public void GenerateRoadSideHedgesContainerBottom()
        {
            if (_gameView.GameObjectContainers.OfType<GameObjectContainer>().FirstOrDefault(x => x.IsAnimating == false && x.ConstructType == ConstructType.ROAD_SIDE_HEDGE) is GameObjectContainer roadSideHedgeContainerBottom)
            {
                roadSideHedgeContainerBottom.SetPosition(
                  left: (roadSideHedgeContainerBottom.Width * -1.1) - 30,
                  top: ((Constants.DEFAULT_SCENE_HEIGHT) * -1),
                  z: 4);

                roadSideHedgeContainerBottom.IsAnimating = true;
            }
        }

        private void AnimateRoadSideHedgesContainer(GameObjectContainer roadContainer)
        {
            var speed = roadContainer.Speed;
            roadContainer.MoveDownRight(speed);
        }

        private void RecycleRoadSideHedgesContainer(GameObjectContainer roadContainer)
        {
            var hitBox = roadContainer.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - 150 > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadContainer.IsAnimating = false;
            }
        }

        #endregion

        #endregion

        #region GameObjects

        #region Player

        #region PlayerBalloon

        private void SpawnPlayerBalloon()
        {
            var playerTemplate = _random.Next(1, 3);

            _player = new(
                animateAction: AnimatePlayerBalloon,
                recycleAction: (_player) => { });

            _player.SetZ(z: 7);
            _player.MoveOutOfSight();

            SpawnDropShadow(source: _player);

            _gameView.AddToView(_player);

            LoggingExtensions.Log($"Player Template: {playerTemplate}");
        }

        private void GeneratePlayerBalloon()
        {
            _player.Reset();
            _player.Reposition();
            _player.IsAnimating = true;

            switch (_selectedPlayerTemplate)
            {
                case PlayerBalloonTemplate.Blue:
                    {
                        _gameController.SetAttackButtonColor(Application.Current.Resources["PlayerBlueAccentColor"] as SolidColorBrush);
                        _gameController.SetThumbstickThumbColor(Application.Current.Resources["PlayerBlueAccentColor"] as SolidColorBrush);
                    }
                    break;
                case PlayerBalloonTemplate.Red:
                    {
                        _gameController.SetAttackButtonColor(Application.Current.Resources["PlayerRedAccentColor"] as SolidColorBrush);
                        _gameController.SetThumbstickThumbColor(Application.Current.Resources["PlayerRedAccentColor"] as SolidColorBrush);
                    }
                    break;
                default:
                    break;
            }

            GenerateDropShadow(source: _player);
            SetPlayerHealthBar();
        }

        private void SetPlayerHealthBar()
        {
            _playerHealthBar.SetMaxiumHealth(_player.Health);
            _playerHealthBar.SetValue(_player.Health);
            _playerHealthBar.SetBarColor(color: Colors.Crimson);
        }

        private void AnimatePlayerBalloon(GameObject player)
        {
            _player.Pop();
            _player.Hover();
            _player.DepleteAttackStance();
            _player.DepleteWinStance();
            _player.DepleteHitStance();
            _player.RecoverFromHealthLoss();

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
            {
                if (_gameController.IsPausing)
                {
                    PauseGame();
                    _gameController.IsPausing = false;
                }
                else
                {
                    var count = _gameView.GameObjects.OfType<VehicleEnemy>().Count(x => x.IsAnimating && x.WillHonk) + _gameView.GameObjects.OfType<UfoEnemy>().Count(x => x.IsAnimating && x.WillHonk);
                    _soundPollutionMeter.SetValue(count * 2);

                    if (_soundPollutionMeter.GetValue() >= _soundPollutionMeter.GetMaxiumHealth()) // loose score slowly if sound pollution has reached the limit
                    {
                        _gameScoreBar.LooseScore(0.01);
                    }

                    var scaling = ScreenExtensions.GetScreenSpaceScaling();
                    var speed = _player.Speed;

                    _player.Move(
                        speed: speed,
                        sceneWidth: Constants.DEFAULT_SCENE_WIDTH * scaling,
                        sceneHeight: Constants.DEFAULT_SCENE_HEIGHT * scaling,
                        controller: _gameController);

                    if (_gameController.IsAttacking)
                    {
                        if (UfoEnemyExists() || AnyInAirBossExists())
                        {
                            if (_powerUpMeter.HasHealth)
                            {
                                switch ((PowerUpType)_powerUpMeter.Tag)
                                {
                                    case PowerUpType.SEEKING_SNITCH:
                                        {
                                            GeneratePlayerRocketSeeking();
                                        }
                                        break;
                                    case PowerUpType.BULLS_EYE:
                                        {
                                            GeneratePlayerRocketBullsEye();
                                        }
                                        break;
                                    case PowerUpType.ARMOR:
                                        {
                                            GeneratePlayerRocket();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                GeneratePlayerRocket();
                            }
                        }
                        else
                        {
                            GeneratePlayerHonkBomb();
                        }

                        _gameController.DeactivateAttack();
                    }
                }
            }
        }

        private void LoosePlayerHealth()
        {
            _player.SetPopping();

            if (_powerUpMeter.HasHealth && (PowerUpType)_powerUpMeter.Tag == PowerUpType.ARMOR)
            {
                DepletePowerUp();
                GenerateFloatingNumber(_player);
            }
            else
            {
                _player.LooseHealth();
                _player.SetHitStance();

                GenerateFloatingNumber(_player);

                _playerHealthBar.SetValue(_player.Health);

                if (_gameView.GameObjects.OfType<GameObject>().FirstOrDefault(x => x.IsAnimating &&
                    (x.ConstructType == ConstructType.UFO_BOSS || x.ConstructType == ConstructType.ZOMBIE_BOSS || x.ConstructType == ConstructType.MAFIA_BOSS)) is GameObject boss)
                {
                    if (boss is UfoBoss ufo)
                    {
                        ufo.SetWinStance();
                    }
                    else if (boss is ZombieBoss zombie)
                    {
                        zombie.SetWinStance();
                    }
                    else if (boss is MafiaBoss mafia)
                    {
                        mafia.SetWinStance();
                    }
                }

                GameOver();
            }
        }

        #endregion

        #region PlayerHonkBomb

        private void SpawnPlayerHonkBombs()
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerHonkBomb playerHonkBomb = new(
                    animateAction: AnimatePlayerHonkBomb,
                    recycleAction: RecyclePlayerHonkBomb);

                playerHonkBomb.SetZ(z: 7);
                playerHonkBomb.MoveOutOfSight();

                _gameView.AddToView(playerHonkBomb);

                SpawnDropShadow(source: playerHonkBomb);
            }
        }

        private void GeneratePlayerHonkBomb()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !_gameView.IsSlowMotionActivated)
            {
                if ((VehicleBossExists() || _gameView.GameObjects.OfType<VehicleEnemy>().Any(x => x.IsAnimating)) &&
                    _gameView.GameObjects.OfType<PlayerHonkBomb>().FirstOrDefault(x => x.IsAnimating == false) is PlayerHonkBomb playerHonkBomb)
                {
                    _player.SetAttackStance();

                    playerHonkBomb.Reset();
                    playerHonkBomb.IsGravitatingDownwards = true;
                    playerHonkBomb.Reposition(player: _player);
                    playerHonkBomb.IsAnimating = true;

                    GenerateDropShadow(source: playerHonkBomb);
                }
                else
                {
                    _player.SetWinStance();
                }
            }
        }

        private void AnimatePlayerHonkBomb(GameObject playerHonkBomb)
        {
            PlayerHonkBomb playerHonkBomb1 = playerHonkBomb as PlayerHonkBomb;

            var speed = playerHonkBomb1.Speed;

            if (playerHonkBomb1.IsBlasting)
            {
                playerHonkBomb.Expand();
                playerHonkBomb.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
                playerHonkBomb1.MoveDownRight(speed);
            }
            else
            {
                //playerHonkBomb.Pop();
                playerHonkBomb.SetLeft(playerHonkBomb.GetLeft() + speed);
                playerHonkBomb.SetTop(playerHonkBomb.GetTop() + speed * 1.2);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    DropShadow dropShadow = _gameView.GameObjects.OfType<DropShadow>().First(x => x.Id == playerHonkBomb.Id);

                    var drpShdwHitBox = dropShadow.GetCloseHitBox();
                    var fireCrackerHitBox = playerHonkBomb.GetCloseHitBox();

                    if (drpShdwHitBox.IntersectsWith(fireCrackerHitBox) && playerHonkBomb.GetBottom() > dropShadow.GetBottom())  // start blast animation when the bomb touches it's shadow
                    {
                        if (_gameView.GameObjects.OfType<VehicleEnemy>()
                            .Where(x => x.IsAnimating /*&& x.WillHonk*/)
                            .FirstOrDefault(x => x.GetCloseHitBox().IntersectsWith(fireCrackerHitBox)) is VehicleEnemy vehicleEnemy) // while in blast check if it intersects with any vehicle, if it does then the vehicle stops honking and slows down
                        {
                            LooseVehicleEnemyHealth(vehicleEnemy);
                        }

                        if (_gameView.GameObjects.OfType<VehicleBoss>()
                            .FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is VehicleBoss vehicleBoss && vehicleBoss.GetCloseHitBox().IntersectsWith(fireCrackerHitBox)) // if a vechile boss is in place then boss looses health
                        {
                            LooseVehicleBossHealth(vehicleBoss);
                        }

                        playerHonkBomb1.SetBlast();

                        dropShadow.IsAnimating = false;
                        dropShadow.SetPosition(-3000, -3000);
                    }
                }
            }
        }

        private void RecyclePlayerHonkBomb(GameObject playerHonkBomb)
        {
            if (playerHonkBomb.IsFadingComplete)
            {
                playerHonkBomb.IsAnimating = false;
                playerHonkBomb.IsGravitatingDownwards = false;
                playerHonkBomb.SetPosition(left: -3000, top: -3000);
            }
        }

        #endregion

        #region PlayerRocket

        private void SpawnPlayerRockets()
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerRocket playerRocket = new(
                    animateAction: AnimatePlayerRocket,
                    recycleAction: RecyclePlayerRocket);

                playerRocket.SetZ(z: 8);
                playerRocket.MoveOutOfSight();

                _gameView.AddToView(playerRocket);

                SpawnDropShadow(source: playerRocket);
            }
        }

        private void GeneratePlayerRocket()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !_gameView.IsSlowMotionActivated &&
                _gameView.GameObjects.OfType<PlayerRocket>().FirstOrDefault(x => x.IsAnimating == false) is PlayerRocket playerRocket)
            {
                _player.SetAttackStance();

                playerRocket.Reset();
                playerRocket.SetPopping();
                playerRocket.Reposition(player: _player);
                playerRocket.IsAnimating = true;

                GenerateDropShadow(source: playerRocket);

                var playerDistantHitBox = _player.GetDistantHitBox();

                // get closest possible target
                UfoBossRocketSeeking ufoBossRocketSeeking = _gameView.GameObjects.OfType<UfoBossRocketSeeking>()?.FirstOrDefault(x => x.IsAnimating && !x.IsBlasting && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                UfoBoss ufoBoss = _gameView.GameObjects.OfType<UfoBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));
                ZombieBoss zombieBoss = _gameView.GameObjects.OfType<ZombieBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));
                MafiaBoss mafiaBoss = _gameView.GameObjects.OfType<MafiaBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                UfoEnemy ufoEnemy = _gameView.GameObjects.OfType<UfoEnemy>()?.FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                // if not found then find random target
                ufoBossRocketSeeking ??= _gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting);
                ufoBoss ??= _gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);
                zombieBoss ??= _gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);
                mafiaBoss ??= _gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);

                ufoEnemy ??= _gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete);

                if (ufoEnemy is not null)
                {
                    SetPlayerRocketDirection(source: _player, rocket: playerRocket, rocketTarget: ufoEnemy);
                }
                else if (ufoBoss is not null)
                {
                    SetPlayerRocketDirection(source: _player, rocket: playerRocket, rocketTarget: ufoBoss);
                }
                else if (ufoBossRocketSeeking is not null)
                {
                    SetPlayerRocketDirection(source: _player, rocket: playerRocket, rocketTarget: ufoBossRocketSeeking);
                }
                else if (zombieBoss is not null)
                {
                    SetPlayerRocketDirection(source: _player, rocket: playerRocket, rocketTarget: zombieBoss);
                }
                else if (mafiaBoss is not null)
                {
                    SetPlayerRocketDirection(source: _player, rocket: playerRocket, rocketTarget: mafiaBoss);
                }
            }
        }

        private void AnimatePlayerRocket(GameObject playerRocket)
        {
            PlayerRocket playerRocket1 = playerRocket as PlayerRocket;

            var speed = playerRocket1.Speed;

            if (playerRocket1.AwaitMoveDownLeft)
            {
                playerRocket1.MoveDownLeft(speed);
            }
            else if (playerRocket1.AwaitMoveUpRight)
            {
                playerRocket1.MoveUpRight(speed);
            }
            else if (playerRocket1.AwaitMoveUpLeft)
            {
                playerRocket1.MoveUpLeft(speed);
            }
            else if (playerRocket1.AwaitMoveDownRight)
            {
                playerRocket1.MoveDownRight(speed);
            }

            if (playerRocket1.IsBlasting)
            {
                playerRocket.Expand();
                playerRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                playerRocket.Pop();
                playerRocket1.Hover();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var hitBox = playerRocket.GetCloseHitBox();

                    if (_gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting && x.GetCloseHitBox().IntersectsWith(hitBox)) is UfoBossRocketSeeking ufoBossRocketSeeking) // if player bomb touches UfoBoss's seeking bomb, it blasts
                    {
                        playerRocket1.SetBlast();
                        ufoBossRocketSeeking.SetBlast();
                    }
                    else if (_gameView.GameObjects.OfType<ZombieBossRocketBlock>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting && x.GetCloseHitBox().IntersectsWith(hitBox)) is ZombieBossRocketBlock zombieBossRocket) // if player bomb touches ZombieBoss's seeking bomb, it blasts
                    {
                        playerRocket1.SetBlast();
                        LooseZombieBossRocketBlockHealth(zombieBossRocket);
                    }
                    else if (_gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitBox)) is UfoBoss ufoBoss) // if player bomb touches UfoBoss, it blasts, UfoBoss looses health
                    {
                        playerRocket1.SetBlast();
                        LooseUfoBossHealth(ufoBoss);
                    }
                    else if (_gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitBox)) is ZombieBoss zombieBoss) // if player bomb touches ZombieBoss, it blasts, ZombieBoss looses health
                    {
                        playerRocket1.SetBlast();
                        LooseZombieBossHealth(zombieBoss);
                    }
                    else if (_gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitBox)) is MafiaBoss mafiaBoss) // if player bomb touches MafiaBoss, it blasts, MafiaBoss looses health
                    {
                        playerRocket1.SetBlast();
                        LooseMafiaBossHealth(mafiaBoss);
                    }
                    else if (_gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating && !x.IsDead && x.GetCloseHitBox().IntersectsWith(hitBox)) is UfoEnemy ufoEnemy) // if player bomb touches enemy, it blasts, enemy looses health
                    {
                        playerRocket1.SetBlast();
                        LooseUfoEnemyHealth(ufoEnemy);
                    }

                    if (playerRocket1.AutoBlast())
                        playerRocket1.SetBlast();
                }
            }
        }

        private void RecyclePlayerRocket(GameObject playerRocket)
        {
            var hitbox = playerRocket.GetHitBox();

            // if bomb is blasted or goes out of scene bounds
            if (playerRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT)
            {
                playerRocket.IsAnimating = false;
            }
        }

        #endregion

        #region PlayerRocketSeeking

        private void SpawnPlayerRocketSeekings()
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerRocketSeeking playerRocketSeeking = new(
                    animateAction: AnimatePlayerRocketSeeking,
                    recycleAction: RecyclePlayerRocketSeeking);

                playerRocketSeeking.SetZ(z: 7);
                playerRocketSeeking.MoveOutOfSight();

                _gameView.AddToView(playerRocketSeeking);

                SpawnDropShadow(source: playerRocketSeeking);
            }
        }

        private void GeneratePlayerRocketSeeking()
        {
            // generate a seeking bomb if one is not in scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !_gameView.IsSlowMotionActivated &&
                _gameView.GameObjects.OfType<PlayerRocketSeeking>().FirstOrDefault(x => x.IsAnimating == false) is PlayerRocketSeeking playerRocketSeeking)
            {
                _player.SetAttackStance();

                playerRocketSeeking.Reset();
                playerRocketSeeking.SetPopping();
                playerRocketSeeking.Reposition(player: _player);
                playerRocketSeeking.IsAnimating = true;

                GenerateDropShadow(source: playerRocketSeeking);

                if (_powerUpMeter.HasHealth && (PowerUpType)_powerUpMeter.Tag == PowerUpType.SEEKING_SNITCH)
                    DepletePowerUp();
            }
        }

        private void AnimatePlayerRocketSeeking(GameObject playerRocketSeeking)
        {
            PlayerRocketSeeking playerRocketSeeking1 = playerRocketSeeking as PlayerRocketSeeking;

            if (playerRocketSeeking1.IsBlasting)
            {
                var speed = playerRocketSeeking1.Speed;
                playerRocketSeeking1.MoveDownRight(speed);
                playerRocketSeeking.Expand();
                playerRocketSeeking.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                playerRocketSeeking.Pop();
                playerRocketSeeking.Rotate(rotationSpeed: 3.5);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (_gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting) is UfoBossRocketSeeking ufoBossRocketSeeking) // target UfoBossRocketSeeking
                    {
                        playerRocketSeeking1.Seek(ufoBossRocketSeeking.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(ufoBossRocketSeeking.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            ufoBossRocketSeeking.SetBlast();
                        }
                    }
                    else if (_gameView.GameObjects.OfType<ZombieBossRocketBlock>().FirstOrDefault(x => x.IsAnimating) is ZombieBossRocketBlock zombieBossRocket) // target ZombieBossRocketBlock
                    {
                        playerRocketSeeking1.Seek(zombieBossRocket.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(zombieBossRocket.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            zombieBossRocket.LooseHealth();
                        }
                    }
                    else if (_gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is UfoBoss ufoBoss) // target UfoBoss
                    {
                        playerRocketSeeking1.Seek(ufoBoss.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(ufoBoss.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            LooseUfoBossHealth(ufoBoss);
                        }
                    }
                    else if (_gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is ZombieBoss ZombieBoss) // target ZombieBoss
                    {
                        playerRocketSeeking1.Seek(ZombieBoss.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(ZombieBoss.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            LooseZombieBossHealth(ZombieBoss);
                        }
                    }
                    else if (_gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is MafiaBoss MafiaBoss) // target MafiaBoss
                    {
                        playerRocketSeeking1.Seek(MafiaBoss.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(MafiaBoss.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            LooseMafiaBossHealth(MafiaBoss);
                        }
                    }
                    else if (_gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete) is UfoEnemy enemy) // target UfoEnemy
                    {
                        playerRocketSeeking1.Seek(enemy.GetCloseHitBox());

                        if (playerRocketSeeking1.GetCloseHitBox().IntersectsWith(enemy.GetCloseHitBox()))
                        {
                            playerRocketSeeking1.SetBlast();
                            LooseUfoEnemyHealth(enemy);
                        }
                    }

                    if (playerRocketSeeking1.AutoBlast())
                        playerRocketSeeking1.SetBlast();
                }
            }
        }

        private void RecyclePlayerRocketSeeking(GameObject playerRocketSeeking)
        {
            var hitbox = playerRocketSeeking.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (playerRocketSeeking.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Bottom > Constants.DEFAULT_SCENE_HEIGHT)
            {
                playerRocketSeeking.IsAnimating = false;
            }
        }

        #endregion

        #region PlayerRocketBullsEye

        private void SpawnPlayerRocketBullsEyes()
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerRocketBullsEye playerRocketBullsEye = new(
                    animateAction: AnimatePlayerRocketBullsEye,
                    recycleAction: RecyclePlayerRocketBullsEye);

                playerRocketBullsEye.SetZ(z: 7);
                playerRocketBullsEye.MoveOutOfSight();

                _gameView.AddToView(playerRocketBullsEye);

                SpawnDropShadow(source: playerRocketBullsEye);
            }
        }

        private void GeneratePlayerRocketBullsEye()
        {
            // generate a bulls eye bomb if one is not in scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !_gameView.IsSlowMotionActivated &&
                _gameView.GameObjects.OfType<PlayerRocketBullsEye>().FirstOrDefault(x => x.IsAnimating == false) is PlayerRocketBullsEye playerRocketBullsEye)
            {
                _player.SetAttackStance();

                playerRocketBullsEye.Reset();
                playerRocketBullsEye.SetPopping();
                playerRocketBullsEye.Reposition(player: _player);
                playerRocketBullsEye.IsAnimating = true;

                GenerateDropShadow(source: playerRocketBullsEye);

                var playerDistantHitBox = _player.GetDistantHitBox();

                // get closest possible target
                UfoBossRocketSeeking ufoBossRocketSeeking = _gameView.GameObjects.OfType<UfoBossRocketSeeking>()?.FirstOrDefault(x => x.IsAnimating && !x.IsBlasting && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                UfoBoss ufoBoss = _gameView.GameObjects.OfType<UfoBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));
                ZombieBoss zombieBoss = _gameView.GameObjects.OfType<ZombieBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));
                MafiaBoss mafiaBoss = _gameView.GameObjects.OfType<MafiaBoss>()?.FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                UfoEnemy ufoEnemy = _gameView.GameObjects.OfType<UfoEnemy>()?.FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete && x.GetHitBox().IntersectsWith(playerDistantHitBox));

                // if not found then find random target
                ufoBossRocketSeeking ??= _gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting);

                ufoBoss ??= _gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);
                zombieBoss ??= _gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);
                mafiaBoss ??= _gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking);

                ufoEnemy ??= _gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete);

                if (ufoEnemy is not null)
                {
                    playerRocketBullsEye.SetTarget(ufoEnemy.GetCloseHitBox());
                }
                else if (ufoBoss is not null)
                {
                    playerRocketBullsEye.SetTarget(ufoBoss.GetCloseHitBox());
                }
                else if (ufoBossRocketSeeking is not null)
                {
                    playerRocketBullsEye.SetTarget(ufoBossRocketSeeking.GetCloseHitBox());
                }
                else if (zombieBoss is not null)
                {
                    playerRocketBullsEye.SetTarget(zombieBoss.GetCloseHitBox());
                }
                else if (mafiaBoss is not null)
                {
                    playerRocketBullsEye.SetTarget(mafiaBoss.GetCloseHitBox());
                }

                if (_powerUpMeter.HasHealth && (PowerUpType)_powerUpMeter.Tag == PowerUpType.BULLS_EYE)
                    DepletePowerUp();
            }
        }

        private void AnimatePlayerRocketBullsEye(GameObject playerRocketBullsEye)
        {
            PlayerRocketBullsEye playerRocketBullsEye1 = playerRocketBullsEye as PlayerRocketBullsEye;

            if (playerRocketBullsEye1.IsBlasting)
            {
                var speed = playerRocketBullsEye1.Speed;
                playerRocketBullsEye1.MoveDownRight(speed);
                playerRocketBullsEye.Expand();
                playerRocketBullsEye.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                playerRocketBullsEye.Pop();
                playerRocketBullsEye.Rotate(rotationSpeed: 3.5);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING) // check if the rocket intersects with any target on its path
                {
                    var speed = playerRocketBullsEye1.Speed;
                    playerRocketBullsEye1.Move();

                    var hitbox = playerRocketBullsEye1.GetCloseHitBox();

                    if (_gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating && !x.IsBlasting && x.GetCloseHitBox().IntersectsWith(hitbox)) is UfoBossRocketSeeking ufoBossRocketSeeking) // target UfoBossRocketSeeking
                    {
                        playerRocketBullsEye1.SetBlast();
                        ufoBossRocketSeeking.SetBlast();

                    }
                    else if (_gameView.GameObjects.OfType<ZombieBossRocketBlock>().FirstOrDefault(x => x.IsAnimating && x.GetCloseHitBox().IntersectsWith(hitbox)) is ZombieBossRocketBlock zombieBossRocket) // target ZombieBossRocketBlock
                    {
                        playerRocketBullsEye1.SetBlast();
                        zombieBossRocket.LooseHealth();
                    }
                    else if (_gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitbox)) is UfoBoss ufoBoss) // target UfoBoss
                    {
                        playerRocketBullsEye1.SetBlast();
                        LooseUfoBossHealth(ufoBoss);
                    }
                    else if (_gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitbox)) is ZombieBoss zombieBoss) // target ZombieBoss
                    {
                        playerRocketBullsEye1.SetBlast();
                        LooseZombieBossHealth(zombieBoss);
                    }
                    else if (_gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking && x.GetCloseHitBox().IntersectsWith(hitbox)) is MafiaBoss mafiaBoss) // target MafiaBoss
                    {
                        playerRocketBullsEye1.SetBlast();
                        LooseMafiaBossHealth(mafiaBoss);
                    }
                    else if (_gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating && !x.IsFadingComplete && x.GetCloseHitBox().IntersectsWith(hitbox)) is UfoEnemy enemy) // target UfoEnemy
                    {
                        playerRocketBullsEye1.SetBlast();
                        LooseUfoEnemyHealth(enemy);
                    }

                    if (playerRocketBullsEye1.AutoBlast())
                        playerRocketBullsEye1.SetBlast();
                }
            }
        }

        private void RecyclePlayerRocketBullsEye(GameObject playerRocketBullsEye)
        {
            var hitbox = playerRocketBullsEye.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (playerRocketBullsEye.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Bottom > Constants.DEFAULT_SCENE_HEIGHT)
            {
                playerRocketBullsEye.IsAnimating = false;
            }
        }

        #endregion

        #endregion

        #region Road

        #region RoadMark

        //private void SpawnRoadMarks()
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        RoadMark roadMark = new(
        //            animateAction: AnimateRoadMark,
        //            recycleAction: RecycleRoadMark);

        //        roadMark.SetZ(z: 0);
        //        roadMark.MoveOutOfSight();

        //        _gameView.AddToView(roadMark);
        //    }
        //}

        //private void GenerateRoadMark()
        //{
        //    if (_gameView.GameObjects.OfType<RoadMark>().FirstOrDefault(x => x.IsAnimating == false) is RoadMark roadMark)
        //    {
        //        roadMark.Reset();
        //        roadMark.SetPosition(
        //          left: roadMark.Height * -1.5,
        //          top: roadMark.Height * -1);
        //        roadMark.IsAnimating = true;
        //    }
        //}

        //private void AnimateRoadMark(GameObject roadMark)
        //{
        //    RoadMark roadMark1 = roadMark as RoadMark;
        //    var speed = roadMark1.Speed;
        //    roadMark1.MoveDownRight(speed);
        //}

        //private void RecycleRoadMark(GameObject roadMark)
        //{
        //    var hitBox = roadMark.GetHitBox();

        //    if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left > Constants.DEFAULT_SCENE_WIDTH)
        //    {
        //        roadMark.IsAnimating = false;
        //    }
        //}

        #endregion

        #region RoadSideWalk

        //private void SpawnRoadSideWalks()
        //{
        //    for (int i = 0; i < 15; i++)
        //    {
        //        RoadSideWalk roadSideWalk = new(
        //        animateAction: AnimateRoadSideWalk,
        //        recycleAction: RecycleRoadSideWalk);

        //        roadSideWalk.MoveOutOfSight();

        //        _gameView.AddToView(roadSideWalk);
        //    }
        //}

        //private void GenerateRoadSideWalk()
        //{
        //    if (_gameView.GameObjects.OfType<RoadSideWalk>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideWalk roadSideWalkTop)
        //    {
        //        roadSideWalkTop.Reset();
        //        roadSideWalkTop.SetPosition(
        //            left: (Constants.DEFAULT_SCENE_WIDTH / 2.25 - roadSideWalkTop.Width),
        //            top: roadSideWalkTop.Height * -1);
        //        roadSideWalkTop.IsAnimating = true;
        //    }

        //    if (_gameView.GameObjects.OfType<RoadSideWalk>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideWalk roadSideWalkBottom)
        //    {
        //        roadSideWalkBottom.Reset();
        //        roadSideWalkBottom.SetPosition(
        //            left: (roadSideWalkBottom.Height * -1.5) - 30,
        //            top: (Constants.DEFAULT_SCENE_HEIGHT / 5 + roadSideWalkBottom.Height / 2) - 90);
        //        roadSideWalkBottom.IsAnimating = true;
        //    }
        //}

        //private void AnimateRoadSideWalk(GameObject roadSideWalk)
        //{
        //    RoadSideWalk roadSideWalk1 = roadSideWalk as RoadSideWalk;
        //    var speed = roadSideWalk1.Speed;
        //    roadSideWalk1.MoveDownRight(speed: speed);
        //}

        //private void RecycleRoadSideWalk(GameObject roadSideWalk)
        //{
        //    var hitBox = roadSideWalk.GetHitBox();

        //    if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideWalk.Width > Constants.DEFAULT_SCENE_WIDTH)
        //    {
        //        roadSideWalk.IsAnimating = false;
        //    }
        //}

        #endregion

        #region RoadSideTree

        //private void SpawnRoadSideTrees()
        //{
        //    for (int i = 0; i < 11; i++)
        //    {
        //        RoadSideTree roadSideTree = new(
        //            animateAction: AnimateRoadSideTree,
        //            recycleAction: RecycleRoadSideTree);

        //        roadSideTree.MoveOutOfSight();

        //        _gameView.AddToView(roadSideTree);
        //    }
        //}

        //private void GenerateRoadSideTree()
        //{
        //    if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideTree>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideTree roadSideTreeTop)
        //    {
        //        roadSideTreeTop.Reset();
        //        roadSideTreeTop.SetPosition(
        //          left: (Constants.DEFAULT_SCENE_WIDTH / 5),
        //          top: (roadSideTreeTop.Height * -1.1),
        //          z: 3);
        //        roadSideTreeTop.IsAnimating = true;


        //        //GenerateDropShadow(source: roadSideTreeTop);
        //    }

        //    if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideTree>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideTree roadSideTreeBottom)
        //    {
        //        roadSideTreeBottom.Reset();
        //        roadSideTreeBottom.SetPosition(
        //          left: (roadSideTreeBottom.Width * -1.1),
        //          top: (Constants.DEFAULT_SCENE_HEIGHT / 7.8),
        //          z: 7);
        //        roadSideTreeBottom.IsAnimating = true;

        //        //GenerateDropShadow(source: roadSideTreeBottom);
        //    }
        //}

        //private void AnimateRoadSideTree(GameObject roadSideTree)
        //{
        //    RoadSideTree roadSideTree1 = roadSideTree as RoadSideTree;
        //    var speed = roadSideTree1.Speed;
        //    roadSideTree1.MoveDownRight(speed);
        //}

        //private void RecycleRoadSideTree(GameObject roadSideTree)
        //{
        //    var hitBox = roadSideTree.GetHitBox();

        //    if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideTree.Width > Constants.DEFAULT_SCENE_WIDTH)
        //    {
        //        roadSideTree.IsAnimating = false;
        //    }
        //}

        #endregion

        #region RoadSideHedge

        //private void SpawnRoadSideHedges()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        RoadSideHedge roadSideHedge = new(
        //            animateAction: AnimateRoadSideHedge,
        //            recycleAction: RecycleRoadSideHedge);

        //        roadSideHedge.MoveOutOfSight();

        //        _gameView.AddToView(roadSideHedge);
        //    }
        //}

        //private void GenerateRoadSideHedge()
        //{
        //    if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideHedge>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideHedge roadSideHedgeTop)
        //    {
        //        roadSideHedgeTop.Reset();
        //        roadSideHedgeTop.SetPosition(
        //          left: (Constants.DEFAULT_SCENE_WIDTH / 20),
        //          top: (roadSideHedgeTop.Height * -1.1),
        //          z: 3);
        //        roadSideHedgeTop.IsAnimating = true;
        //    }

        //    if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideHedge>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideHedge roadSideHedgeBottom)
        //    {
        //        roadSideHedgeBottom.Reset();
        //        roadSideHedgeBottom.SetPosition(
        //          left: (roadSideHedgeBottom.Width * -1.1),
        //          top: (Constants.DEFAULT_SCENE_HEIGHT / 7.9),
        //          z: 4);
        //        roadSideHedgeBottom.IsAnimating = true;
        //    }
        //}

        //private void AnimateRoadSideHedge(GameObject roadSideHedge)
        //{
        //    RoadSideHedge roadSideHedge1 = roadSideHedge as RoadSideHedge;
        //    var speed = roadSideHedge1.Speed;
        //    roadSideHedge1.MoveDownRight(speed);
        //}

        //private void RecycleRoadSideHedge(GameObject roadSideHedge)
        //{
        //    var hitBox = roadSideHedge.GetHitBox();

        //    if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideHedge.Width > Constants.DEFAULT_SCENE_WIDTH)
        //    {
        //        roadSideHedge.IsAnimating = false;
        //    }
        //}

        #endregion

        #region RoadSideLamp

        private void SpawnRoadSideLamps()
        {
            for (int i = 0; i < 7; i++)
            {
                RoadSideLamp roadSideLamp = new(
                    animateAction: AnimateRoadSideLamp,
                    recycleAction: RecycleRoadSideLamp);

                roadSideLamp.MoveOutOfSight();

                _gameView.AddToView(roadSideLamp);
            }
        }

        private void GenerateRoadSideLamp()
        {
            if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideLamp>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideLamp roadSideLampTop)
            {
                roadSideLampTop.Reset();
                roadSideLampTop.SetPosition(
                  left: (Constants.DEFAULT_SCENE_WIDTH / 3 - roadSideLampTop.Width) - 100,
                  top: ((roadSideLampTop.Height * 1.5) * -1) - 50,
                  z: 4);
                roadSideLampTop.IsAnimating = true;
            }

            if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideLamp>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideLamp roadSideLampBottom)
            {
                roadSideLampBottom.Reset();
                roadSideLampBottom.SetPosition(
                  left: (-1.9 * roadSideLampBottom.Width),
                  top: (Constants.DEFAULT_SCENE_HEIGHT / 4.3),
                  z: 6);
                roadSideLampBottom.IsAnimating = true;
            }
        }

        private void AnimateRoadSideLamp(GameObject roadSideLamp)
        {
            RoadSideLamp roadSideLamp1 = roadSideLamp as RoadSideLamp;
            var speed = roadSideLamp1.Speed;
            roadSideLamp1.MoveDownRight(speed);
        }

        private void RecycleRoadSideLamp(GameObject roadSideLamp)
        {
            var hitBox = roadSideLamp.GetHitBox();

            if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideLamp.Width > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadSideLamp.IsAnimating = false;
            }
        }

        #endregion

        #region RoadSideBillboard

        private void SpawnRoadSideBillboards()
        {
            for (int i = 0; i < 3; i++)
            {
                RoadSideBillboard roadSideBillboard = new(
                    animateAction: AnimateRoadSideBillboard,
                    recycleAction: RecycleRoadSideBillboard);

                roadSideBillboard.SetZ(z: 4);
                roadSideBillboard.MoveOutOfSight();

                _gameView.AddToView(roadSideBillboard);
            }
        }

        private void GenerateRoadSideBillboard()
        {
            if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideBillboard>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideBillboard roadSideBillboardTop)
            {
                roadSideBillboardTop.Reset();
                roadSideBillboardTop.SetPosition(
                  left: (Constants.DEFAULT_SCENE_WIDTH / 3.1),
                  top: (roadSideBillboardTop.Height * -1.1));
                roadSideBillboardTop.IsAnimating = true;
            }
        }

        private void AnimateRoadSideBillboard(GameObject roadSideBillboard)
        {
            RoadSideBillboard roadSideBillboard1 = roadSideBillboard as RoadSideBillboard;
            var speed = roadSideBillboard1.Speed;
            roadSideBillboard1.MoveDownRight(speed);
        }

        private void RecycleRoadSideBillboard(GameObject roadSideBillboard)
        {
            var hitBox = roadSideBillboard.GetHitBox();

            if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideBillboard.Width > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadSideBillboard.IsAnimating = false;
            }
        }

        #endregion      

        #region RoadSideLightBillboard

        private void SpawnRoadSideLightBillboards()
        {
            for (int i = 0; i < 4; i++)
            {
                RoadSideLightBillboard roadSideLight = new(
                    animateAction: AnimateRoadSideLightBillboard,
                    recycleAction: RecycleRoadSideLightBillboard);

                roadSideLight.MoveOutOfSight();

                _gameView.AddToView(roadSideLight);
            }
        }

        private void GenerateRoadSideLightBillboard()
        {
            if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<RoadSideLightBillboard>().FirstOrDefault(x => x.IsAnimating == false) is RoadSideLightBillboard roadSideLight)
            {
                roadSideLight.Reset();
                roadSideLight.SetPosition(
                  left: (-3.5 * roadSideLight.Width) + 10,
                  top: (Constants.DEFAULT_SCENE_HEIGHT / 5.2) + 10,
                  z: 5);
                roadSideLight.IsAnimating = true;
            }
        }

        private void AnimateRoadSideLightBillboard(GameObject roadSideLight)
        {
            RoadSideLightBillboard roadSideLight1 = roadSideLight as RoadSideLightBillboard;
            var speed = roadSideLight1.Speed;
            roadSideLight1.MoveDownRight(speed);
        }

        private void RecycleRoadSideLightBillboard(GameObject roadSideLight)
        {
            var hitBox = roadSideLight.GetHitBox();

            if (hitBox.Top - 45 > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - roadSideLight.Width > Constants.DEFAULT_SCENE_WIDTH)
            {
                roadSideLight.IsAnimating = false;
            }
        }

        #endregion

        #endregion

        #region UfoBoss

        #region UfoBoss

        private void SpawnUfoBosses()
        {
            UfoBoss ufoBoss = new(
                animateAction: AnimateUfoBoss,
                recycleAction: RecycleUfoBoss);

            ufoBoss.SetZ(z: 8);
            ufoBoss.MoveOutOfSight();

            _gameView.AddToView(ufoBoss);

            SpawnDropShadow(source: ufoBoss);
        }

        private void GenerateUfoBoss()
        {
            // if scene doesn't contain a UfoBoss then pick a UfoBoss and add to scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _ufoBossCheckpoint.ShouldRelease(_gameScoreBar.GetScore()) && !UfoBossExists() &&
                _gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating == false) is UfoBoss ufoBoss)
            {
                _audioStub.Stop(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.2);

                ufoBoss.Reset();
                ufoBoss.SetPosition(
                    left: 0,
                    top: ufoBoss.Height * -1);
                ufoBoss.IsAnimating = true;

                GenerateDropShadow(source: ufoBoss);

                // set UfoBoss health
                ufoBoss.Health = _ufoBossCheckpoint.GetReleasePointDifference() * 1.5;

                _ufoBossCheckpoint.IncreaseThreasholdLimit(increment: _ufoBossReleasePoint_increase, currentPoint: _gameScoreBar.GetScore());

                _ufoBossHealthBar.SetMaxiumHealth(ufoBoss.Health);
                _ufoBossHealthBar.SetValue(ufoBoss.Health);
                _ufoBossHealthBar.SetIcon(ufoBoss.GetContentUri());
                _ufoBossHealthBar.SetBarColor(color: Colors.Crimson);

                _gameView.ActivateSlowMotion();

                GenerateInterimScreen("Beware of Scarlet Saucer");

                ToggleNightMode(true);
            }
        }

        private void AnimateUfoBoss(GameObject ufoBoss)
        {
            UfoBoss ufoBoss1 = ufoBoss as UfoBoss;

            if (ufoBoss1.IsDead)
            {
                ufoBoss.Shrink();
            }
            else
            {
                ufoBoss.Pop();

                ufoBoss1.Hover();
                ufoBoss1.DepleteHitStance();
                ufoBoss1.DepleteWinStance();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var speed = ufoBoss1.Speed;
                    var scaling = ScreenExtensions.GetScreenSpaceScaling();

                    if (ufoBoss1.IsAttacking)
                    {
                        ufoBoss1.Move(
                            speed: speed,
                            sceneWidth: Constants.DEFAULT_SCENE_WIDTH * scaling,
                            sceneHeight: Constants.DEFAULT_SCENE_HEIGHT * scaling,
                            playerPoint: _player.GetCloseHitBox());


                        if (ufoBoss1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                        {
                            LoosePlayerHealth();
                        }
                    }
                    else
                    {
                        ufoBoss1.MoveDownRight(speed);

                        if (ufoBoss.GetLeft() > (Constants.DEFAULT_SCENE_WIDTH * scaling / 3)) // bring UfoBoss to a suitable distance from player and then start attacking
                        {
                            ufoBoss1.IsAttacking = true;
                        }
                    }
                }
            }
        }

        private void RecycleUfoBoss(GameObject ufoBoss)
        {
            if (ufoBoss.IsShrinkingComplete)
            {
                ufoBoss.IsAnimating = false;
            }
        }

        private void LooseUfoBossHealth(UfoBoss ufoBoss)
        {
            ufoBoss.SetPopping();
            ufoBoss.LooseHealth();
            ufoBoss.SetHitStance();

            GenerateFloatingNumber(ufoBoss);

            _ufoBossHealthBar.SetValue(ufoBoss.Health);

            if (ufoBoss.IsDead)
            {
                _audioStub.Stop(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.6);

                _player.SetWinStance();
                _gameScoreBar.GainScore(3);

                LevelUp();

                _gameView.ActivateSlowMotion();
                ToggleNightMode(false);
            }
        }

        private bool UfoBossExists()
        {
            return _gameView.GameObjects.OfType<UfoBoss>().Any(x => x.IsAnimating);
        }

        #endregion

        #region UfoBossRocket

        private void SpawnUfoBossRockets()
        {
            for (int i = 0; i < 4; i++)
            {
                UfoBossRocket ufoBossRocket = new(
                    animateAction: AnimateUfoBossRocket,
                    recycleAction: RecycleUfoBossRocket);

                ufoBossRocket.SetZ(z: 7);
                ufoBossRocket.MoveOutOfSight();

                _gameView.AddToView(ufoBossRocket);

                SpawnDropShadow(source: ufoBossRocket);
            }
        }

        private void GenerateUfoBossRocket()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is UfoBoss ufoBoss &&
                _gameView.GameObjects.OfType<UfoBossRocket>().FirstOrDefault(x => x.IsAnimating == false) is UfoBossRocket ufoBossRocket)
            {
                ufoBossRocket.Reset();
                ufoBossRocket.SetPopping();
                ufoBossRocket.Reposition(ufoBoss: ufoBoss);
                ufoBossRocket.IsAnimating = true;

                GenerateDropShadow(source: ufoBossRocket);
                SetBossRocketDirection(source: ufoBoss, rocket: ufoBossRocket, rocketTarget: _player);
            }
        }

        private void AnimateUfoBossRocket(GameObject ufoBossRocket)
        {
            UfoBossRocket ufoBossRocket1 = ufoBossRocket as UfoBossRocket;

            var speed = ufoBossRocket1.Speed;

            if (ufoBossRocket1.AwaitMoveDownLeft)
            {
                ufoBossRocket1.MoveDownLeft(speed);
            }
            else if (ufoBossRocket1.AwaitMoveUpRight)
            {
                ufoBossRocket1.MoveUpRight(speed);
            }
            else if (ufoBossRocket1.AwaitMoveUpLeft)
            {
                ufoBossRocket1.MoveUpLeft(speed);
            }
            else if (ufoBossRocket1.AwaitMoveDownRight)
            {
                ufoBossRocket1.MoveDownRight(speed);
            }

            if (ufoBossRocket1.IsBlasting)
            {
                ufoBossRocket.Expand();
                ufoBossRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                ufoBossRocket.Pop();
                ufoBossRocket1.Hover();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (ufoBossRocket.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                    {
                        ufoBossRocket1.SetBlast();
                        LoosePlayerHealth();
                    }

                    if (ufoBossRocket1.AutoBlast())
                        ufoBossRocket1.SetBlast();
                }
            }
        }

        private void RecycleUfoBossRocket(GameObject ufoBossRocket)
        {
            var hitbox = ufoBossRocket.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (ufoBossRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT)
            {
                ufoBossRocket.IsAnimating = false;
            }
        }

        #endregion                

        #region UfoBossRocketSeeking

        private void SpawnUfoBossRocketSeekings()
        {
            for (int i = 0; i < 2; i++)
            {
                UfoBossRocketSeeking ufoBossRocketSeeking = new(
                    animateAction: AnimateUfoBossRocketSeeking,
                    recycleAction: RecycleUfoBossRocketSeeking);

                ufoBossRocketSeeking.SetZ(z: 7);
                ufoBossRocketSeeking.MoveOutOfSight();

                _gameView.AddToView(ufoBossRocketSeeking);

                SpawnDropShadow(source: ufoBossRocketSeeking);
            }
        }

        private void GenerateUfoBossRocketSeeking()
        {
            // generate a seeking bomb if one is not in scene
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<UfoBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is UfoBoss ufoBoss &&
                !_gameView.GameObjects.OfType<UfoBossRocketSeeking>().Any(x => x.IsAnimating) &&
                _gameView.GameObjects.OfType<UfoBossRocketSeeking>().FirstOrDefault(x => x.IsAnimating == false) is UfoBossRocketSeeking ufoBossRocketSeeking)
            {
                ufoBossRocketSeeking.Reset();
                ufoBossRocketSeeking.SetPopping();
                ufoBossRocketSeeking.Reposition(UfoBoss: ufoBoss);
                ufoBossRocketSeeking.IsAnimating = true;

                GenerateDropShadow(source: ufoBossRocketSeeking);
            }
        }

        private void AnimateUfoBossRocketSeeking(GameObject ufoBossRocketSeeking)
        {
            UfoBossRocketSeeking ufoBossRocketSeeking1 = ufoBossRocketSeeking as UfoBossRocketSeeking;

            var speed = ufoBossRocketSeeking1.Speed;

            if (ufoBossRocketSeeking1.IsBlasting)
            {
                ufoBossRocketSeeking.Expand();
                ufoBossRocketSeeking.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
                ufoBossRocketSeeking1.MoveDownRight(speed);
            }
            else
            {
                ufoBossRocketSeeking.Pop();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (_gameView.GameObjects.OfType<UfoBoss>().Any(x => x.IsAnimating && x.IsAttacking))
                    {
                        ufoBossRocketSeeking1.Seek(_player.GetCloseHitBox());

                        if (ufoBossRocketSeeking1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                        {
                            ufoBossRocketSeeking1.SetBlast();
                            LoosePlayerHealth();
                        }
                        else
                        {
                            if (ufoBossRocketSeeking1.AutoBlast())
                                ufoBossRocketSeeking1.SetBlast();
                        }
                    }
                    else
                    {
                        ufoBossRocketSeeking1.SetBlast();
                    }
                }
            }
        }

        private void RecycleUfoBossRocketSeeking(GameObject ufoBossRocketSeeking)
        {
            var hitbox = ufoBossRocketSeeking.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (ufoBossRocketSeeking.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Bottom > Constants.DEFAULT_SCENE_HEIGHT)
            {
                ufoBossRocketSeeking.IsAnimating = false;
            }
        }

        #endregion

        #endregion

        #region UfoEnemy

        #region UfoEnemy

        private void SpawnUfoEnemys()
        {
            for (int i = 0; i < 7; i++)
            {
                UfoEnemy ufoEnemy = new(
                    animateAction: AnimateUfoEnemy,
                    recycleAction: RecycleUfoEnemy);

                ufoEnemy.SetZ(z: 8);
                ufoEnemy.MoveOutOfSight();

                _gameView.AddToView(ufoEnemy);

                SpawnDropShadow(source: ufoEnemy);
            }
        }

        private void GenerateUfoEnemy()
        {
            if (!AnyBossExists() &&
                _ufoEnemyCheckpoint.ShouldRelease(_gameScoreBar.GetScore()) &&
                _gameView.GameObjects.OfType<UfoEnemy>().FirstOrDefault(x => x.IsAnimating == false) is UfoEnemy ufoEnemy)
            {
                ufoEnemy.Reset();
                ufoEnemy.Reposition();
                ufoEnemy.IsAnimating = true;

                GenerateDropShadow(source: ufoEnemy);

                if (!_ufoEnemyFleetAppeared)
                {
                    _audioStub.Play(SoundType.UFO_ENEMY_ENTRY);

                    GenerateInterimScreen("Beware of UFO Fleet");
                    _gameView.ActivateSlowMotion();
                    _ufoEnemyFleetAppeared = true;
                }
            }
        }

        private void AnimateUfoEnemy(GameObject ufoEnemy)
        {
            UfoEnemy ufoEnemy1 = ufoEnemy as UfoEnemy;

            if (ufoEnemy1.IsDead)
            {
                ufoEnemy1.Shrink();
            }
            else
            {
                ufoEnemy1.Hover();
                ufoEnemy1.Pop();

                var speed = ufoEnemy1.Speed;

                ufoEnemy1.MoveDownRight(speed);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (ufoEnemy1.Honk())
                        GenerateUfoEnemyHonk(ufoEnemy1);

                    if (ufoEnemy1.Attack())
                        GenerateUfoEnemyRocket(ufoEnemy1);
                }
            }
        }

        private void RecycleUfoEnemy(GameObject ufoEnemy)
        {
            var hitbox = ufoEnemy.GetHitBox();

            if (ufoEnemy.IsShrinkingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT) // enemy is dead or goes out of bottom right corner
            {
                ufoEnemy.IsAnimating = false;
            }
        }

        private void LooseUfoEnemyHealth(UfoEnemy ufoEnemy)
        {
            ufoEnemy.SetPopping();
            ufoEnemy.LooseHealth();

            GenerateFloatingNumber(ufoEnemy);

            if (ufoEnemy.IsDead)
            {
                _gameScoreBar.GainScore(2);

                _ufoEnemyKillCount++;

                if (_ufoEnemyKillCount > _ufoEnemyKillCount_limit) // after killing limited enemies increase the threadhold limit
                {
                    _ufoEnemyCheckpoint.IncreaseThreasholdLimit(increment: _ufoEnemyReleasePoint_increase, currentPoint: _gameScoreBar.GetScore());
                    _ufoEnemyKillCount = 0;
                    _ufoEnemyFleetAppeared = false;

                    LevelUp();

                    _gameView.ActivateSlowMotion();
                }
            }
        }

        private bool UfoEnemyExists()
        {
            return _gameView.GameObjects.OfType<UfoEnemy>().Any(x => x.IsAnimating);
        }

        #endregion

        #region UfoEnemyRocket

        private void SpawnUfoEnemyRockets()
        {
            for (int i = 0; i < 7; i++)
            {
                UfoEnemyRocket ufoEnemyRocket = new(
                    animateAction: AnimateUfoEnemyRocket,
                    recycleAction: RecycleUfoEnemyRocket);

                ufoEnemyRocket.SetZ(z: 8);
                ufoEnemyRocket.MoveOutOfSight();

                _gameView.AddToView(ufoEnemyRocket);

                SpawnDropShadow(source: ufoEnemyRocket);
            }
        }

        private void GenerateUfoEnemyRocket(UfoEnemy ufoEnemy)
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<UfoEnemyRocket>().FirstOrDefault(x => x.IsAnimating == false) is UfoEnemyRocket ufoEnemyRocket)
            {
                ufoEnemyRocket.Reset();
                ufoEnemyRocket.SetPopping();
                ufoEnemyRocket.Reposition(ufoEnemy: ufoEnemy);
                ufoEnemyRocket.IsAnimating = true;

                GenerateDropShadow(source: ufoEnemyRocket);
            }
        }

        private void AnimateUfoEnemyRocket(GameObject ufoEnemyRocket)
        {
            UfoEnemyRocket ufoEnemyRocket1 = ufoEnemyRocket as UfoEnemyRocket;

            var speed = ufoEnemyRocket1.Speed;
            ufoEnemyRocket1.MoveDownRight(speed);

            if (ufoEnemyRocket1.IsBlasting)
            {
                ufoEnemyRocket.Expand();
                ufoEnemyRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                ufoEnemyRocket.Pop();
                ufoEnemyRocket1.Hover();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (ufoEnemyRocket1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                    {
                        ufoEnemyRocket1.SetBlast();
                        LoosePlayerHealth();
                    }

                    if (ufoEnemyRocket1.AutoBlast())
                        ufoEnemyRocket1.SetBlast();
                }
            }
        }

        private void RecycleUfoEnemyRocket(GameObject ufoEnemyRocket)
        {
            var hitbox = ufoEnemyRocket.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (ufoEnemyRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Bottom > Constants.DEFAULT_SCENE_HEIGHT)
            {
                ufoEnemyRocket.IsAnimating = false;
            }
        }

        #endregion 

        #endregion

        #region VehicleEnemy

        private void SpawnVehicleEnemys()
        {
            for (int i = 0; i < 7; i++)
            {
                VehicleEnemy vehicleEnemy = new(
                    animateAction: AnimateVehicleEnemy,
                    recycleAction: RecycleVehicleEnemy);

                vehicleEnemy.SetZ(z: 5);
                vehicleEnemy.MoveOutOfSight();

                _gameView.AddToView(vehicleEnemy);
            }
        }

        private void GenerateVehicleEnemy()
        {
            if (!AnyBossExists() && !_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<VehicleEnemy>().FirstOrDefault(x => x.IsAnimating == false) is VehicleEnemy vehicleEnemy)
            {
                vehicleEnemy.Reset();
                vehicleEnemy.Reposition();
                vehicleEnemy.IsAnimating = true;
            }
        }

        private void AnimateVehicleEnemy(GameObject vehicleEnemy)
        {
            VehicleEnemy vehicleEnemy1 = vehicleEnemy as VehicleEnemy;

            vehicleEnemy.Pop();
            vehicleEnemy1.Vibrate();

            var speed = vehicleEnemy1.Speed;
            vehicleEnemy1.MoveDownRight(speed);

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
            {
                if (vehicleEnemy1.Honk())
                    GenerateVehicleEnemyHonk(vehicleEnemy1);
            }

            PreventVehicleEnemyOverlapping(vehicleEnemy);
        }

        private void RecycleVehicleEnemy(GameObject vehicleEnemy)
        {
            var hitBox = vehicleEnemy.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left > Constants.DEFAULT_SCENE_WIDTH)
            {
                vehicleEnemy.IsAnimating = false;
            }
        }

        private void PreventVehicleEnemyOverlapping(GameObject vehicleEnemy)
        {
            if (_gameView.GameObjects.OfType<VehicleEnemy>().FirstOrDefault(x => x.IsAnimating && x.GetHitBox().IntersectsWith(vehicleEnemy.GetHitBox())) is GameObject collidingVehicleEnemy)
            {
                var hitBox = vehicleEnemy.GetHitBox();

                if (collidingVehicleEnemy.Speed > vehicleEnemy.Speed) // colliding vehicleEnemy is faster
                {
                    vehicleEnemy.Speed = collidingVehicleEnemy.Speed;
                }
                else if (vehicleEnemy.Speed > collidingVehicleEnemy.Speed) // vehicleEnemy is faster
                {
                    collidingVehicleEnemy.Speed = vehicleEnemy.Speed;
                }
            }
        }

        private void LooseVehicleEnemyHealth(VehicleEnemy vehicleEnemy)
        {
            vehicleEnemy.SetPopping();
            vehicleEnemy.LooseHealth();

            if (vehicleEnemy.WillHonk)
            {
                GenerateFloatingNumber(vehicleEnemy);

                if (vehicleEnemy.IsDead)
                {
                    vehicleEnemy.SetBlast();
                    _gameScoreBar.GainScore(2);
                }
            }
        }

        #endregion

        #region VehicleBoss

        #region VehicleBoss

        private void SpawnVehicleBosses()
        {
            VehicleBoss vehicleBoss = new(
                animateAction: AnimateVehicleBoss,
                recycleAction: RecycleVehicleBoss);

            vehicleBoss.SetZ(z: 5);
            vehicleBoss.MoveOutOfSight();

            _gameView.AddToView(vehicleBoss);
        }

        private void GenerateVehicleBoss()
        {
            // if scene doesn't contain a VehicleBoss then pick a random VehicleBoss and add to scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _vehicleBossCheckpoint.ShouldRelease(_gameScoreBar.GetScore()) && !VehicleBossExists())
            {
                if (_gameView.GameObjects.OfType<VehicleBoss>().FirstOrDefault(x => x.IsAnimating == false) is VehicleBoss vehicleBoss)
                {
                    _audioStub.Stop(SoundType.GAME_BACKGROUND_MUSIC);
                    _audioStub.Play(SoundType.BOSS_BACKGROUND_MUSIC);
                    _audioStub.SetVolume(SoundType.AMBIENCE, 0.4);

                    vehicleBoss.Reset();
                    vehicleBoss.Reposition();
                    vehicleBoss.IsAnimating = true;
                    // set VehicleBoss health
                    vehicleBoss.Health = _vehicleBossCheckpoint.GetReleasePointDifference() * 1.5;

                    _vehicleBossCheckpoint.IncreaseThreasholdLimit(increment: _vehicleBossReleasePoint_increase, currentPoint: _gameScoreBar.GetScore());

                    _vehicleBossHealthBar.SetMaxiumHealth(vehicleBoss.Health);
                    _vehicleBossHealthBar.SetValue(vehicleBoss.Health);
                    _vehicleBossHealthBar.SetIcon(vehicleBoss.GetContentUri());
                    _vehicleBossHealthBar.SetBarColor(color: Colors.Crimson);

                    GenerateInterimScreen("Crazy Honker Arrived");
                    _gameView.ActivateSlowMotion();

                    //ToggleNightMode(true); 
                }
            }
        }

        private void AnimateVehicleBoss(GameObject vehicleBoss)
        {
            VehicleBoss vehicleBoss1 = vehicleBoss as VehicleBoss;

            var speed = vehicleBoss1.Speed;

            if (vehicleBoss1.IsDead)
            {
                vehicleBoss1.MoveDownRight(speed);
            }
            else
            {
                vehicleBoss.Pop();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var scaling = ScreenExtensions.GetScreenSpaceScaling();

                    if (vehicleBoss1.IsAttacking)
                    {
                        vehicleBoss1.Move(
                            speed: speed,
                            sceneWidth: Constants.DEFAULT_SCENE_WIDTH * scaling,
                            sceneHeight: Constants.DEFAULT_SCENE_HEIGHT * scaling);

                        if (vehicleBoss1.Honk())
                            GenerateVehicleBossHonk(vehicleBoss1);
                    }
                    else
                    {
                        if (_gameView.GameObjects.OfType<VehicleEnemy>().All(x => !x.IsAnimating)
                            || _gameView.GameObjects.OfType<VehicleEnemy>().Where(x => x.IsAnimating).All(x => x.GetLeft() > Constants.DEFAULT_SCENE_WIDTH * scaling / 2)) // only bring the boss in view when all other vechiles are gone
                        {
                            vehicleBoss1.MoveDownRight(speed);

                            if (vehicleBoss1.GetLeft() > (Constants.DEFAULT_SCENE_WIDTH * scaling / 3)) // bring boss to a suitable distance from player and then start attacking
                            {
                                vehicleBoss1.IsAttacking = true;
                            }
                        }
                    }
                }
            }
        }

        private void RecycleVehicleBoss(GameObject vehicleBoss)
        {
            var hitBox = vehicleBoss.GetHitBox();

            VehicleBoss vehicleBoss1 = vehicleBoss as VehicleBoss;

            if (vehicleBoss1.IsDead && hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left > Constants.DEFAULT_SCENE_WIDTH)
            {
                vehicleBoss.IsAnimating = false;
            }
        }

        private void LooseVehicleBossHealth(VehicleBoss vehicleBoss)
        {
            vehicleBoss.SetPopping();
            vehicleBoss.LooseHealth();

            GenerateFloatingNumber(vehicleBoss);

            _vehicleBossHealthBar.SetValue(vehicleBoss.Health);

            if (vehicleBoss.IsDead)
            {
                _audioStub.Stop(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.6);

                _player.SetWinStance();
                _gameScoreBar.GainScore(3);

                LevelUp();

                _gameView.ActivateSlowMotion();

                //ToggleNightMode(false);
            }
        }

        private bool VehicleBossExists()
        {
            return _gameView.GameObjects.OfType<VehicleBoss>().Any(x => x.IsAnimating);
        }

        #endregion

        #region VehicleBossRocket

        private void SpawnVehicleBossRockets()
        {
            for (int i = 0; i < 4; i++)
            {
                VehicleBossRocket vehicleBossRocket = new(
                    animateAction: AnimateVehicleBossRocket,
                    recycleAction: RecycleVehicleBossRocket);

                vehicleBossRocket.SetZ(z: 7);
                vehicleBossRocket.MoveOutOfSight();

                _gameView.AddToView(vehicleBossRocket);

                SpawnDropShadow(source: vehicleBossRocket);
            }
        }

        private void GenerateVehicleBossRocket()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<VehicleBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is VehicleBoss vehicleBoss &&
                _gameView.GameObjects.OfType<VehicleBossRocket>().FirstOrDefault(x => x.IsAnimating == false) is VehicleBossRocket vehicleBossRocket)
            {
                vehicleBossRocket.Reset();
                vehicleBossRocket.Reposition(vehicleBoss: vehicleBoss);
                vehicleBossRocket.SetPopping();
                vehicleBossRocket.IsGravitatingUpwards = true;
                vehicleBossRocket.AwaitMoveUpRight = true;
                vehicleBossRocket.IsAnimating = true;

                GenerateDropShadow(source: vehicleBossRocket);
            }
        }

        private void AnimateVehicleBossRocket(GameObject vehicleBossRocket)
        {
            VehicleBossRocket vehicleBossRocket1 = vehicleBossRocket as VehicleBossRocket;

            var speed = vehicleBossRocket1.Speed;

            if (vehicleBossRocket1.AwaitMoveUpRight)
            {
                vehicleBossRocket1.MoveUpRight(speed);
            }

            if (vehicleBossRocket1.IsBlasting)
            {
                vehicleBossRocket.Expand();
                vehicleBossRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                vehicleBossRocket.Pop();
                vehicleBossRocket1.DillyDally();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (vehicleBossRocket.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                    {
                        vehicleBossRocket1.SetBlast();
                        LoosePlayerHealth();
                    }

                    if (vehicleBossRocket1.AutoBlast())
                        vehicleBossRocket1.SetBlast();
                }
            }
        }

        private void RecycleVehicleBossRocket(GameObject vehicleBossRocket)
        {
            var hitbox = vehicleBossRocket.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (vehicleBossRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT)
            {
                vehicleBossRocket.IsAnimating = false;
                vehicleBossRocket.IsGravitatingUpwards = false;
            }
        }

        #endregion

        #endregion

        #region ZombieBoss

        #region ZombieBoss

        private void SpawnZombieBosses()
        {
            ZombieBoss zombieBoss = new(
                animateAction: AnimateZombieBoss,
                recycleAction: RecycleZombieBoss);

            zombieBoss.SetZ(z: 8);
            zombieBoss.MoveOutOfSight();

            _gameView.AddToView(zombieBoss);

            SpawnDropShadow(source: zombieBoss);
        }

        private void GenerateZombieBoss()
        {
            // if scene doesn't contain a ZombieBoss then pick a ZombieBoss and add to scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _zombieBossCheckpoint.ShouldRelease(_gameScoreBar.GetScore()) && !ZombieBossExists() &&
                _gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating == false) is ZombieBoss zombieBoss)
            {
                _audioStub.Stop(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.2);

                zombieBoss.Reset();
                zombieBoss.SetPosition(
                    left: 0,
                    top: zombieBoss.Height * -1);
                zombieBoss.IsAnimating = true;

                GenerateDropShadow(source: zombieBoss);

                // set ZombieBoss health
                zombieBoss.Health = _zombieBossCheckpoint.GetReleasePointDifference() * 1.5;

                _zombieBossCheckpoint.IncreaseThreasholdLimit(increment: _zombieBossReleasePoint_increase, currentPoint: _gameScoreBar.GetScore());

                _zombieBossHealthBar.SetMaxiumHealth(zombieBoss.Health);
                _zombieBossHealthBar.SetValue(zombieBoss.Health);
                _zombieBossHealthBar.SetIcon(zombieBoss.GetContentUri());
                _zombieBossHealthBar.SetBarColor(color: Colors.Crimson);

                _gameView.ActivateSlowMotion();
                ToggleNightMode(true);

                GenerateInterimScreen("Beware of Blocks Zombie");
            }
        }

        private void AnimateZombieBoss(GameObject zombieBoss)
        {
            ZombieBoss zombieBoss1 = zombieBoss as ZombieBoss;

            if (zombieBoss1.IsDead)
            {
                zombieBoss.Shrink();
            }
            else
            {
                zombieBoss.Pop();

                zombieBoss1.Hover();
                zombieBoss1.DepleteHitStance();
                zombieBoss1.DepleteWinStance();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var speed = zombieBoss1.Speed;
                    var scaling = ScreenExtensions.GetScreenSpaceScaling();

                    if (zombieBoss1.IsAttacking)
                    {
                        zombieBoss1.Move(
                            speed: speed,
                            sceneWidth: Constants.DEFAULT_SCENE_WIDTH * scaling,
                            sceneHeight: Constants.DEFAULT_SCENE_HEIGHT * scaling);

                        if (zombieBoss1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                        {
                            LoosePlayerHealth();
                        }
                    }
                    else
                    {
                        zombieBoss1.MoveDownRight(speed);

                        if (zombieBoss.GetLeft() > (Constants.DEFAULT_SCENE_WIDTH * scaling / 3)) // bring ZombieBoss to a suitable distance from player and then start attacking
                        {
                            zombieBoss1.IsAttacking = true;
                        }
                    }
                }
            }
        }

        private void RecycleZombieBoss(GameObject zombieBoss)
        {
            if (zombieBoss.IsShrinkingComplete)
            {
                zombieBoss.IsAnimating = false;
            }
        }

        private void LooseZombieBossHealth(ZombieBoss zombieBoss)
        {
            zombieBoss.SetPopping();
            zombieBoss.LooseHealth();
            zombieBoss.SetHitStance();

            GenerateFloatingNumber(zombieBoss);

            _zombieBossHealthBar.SetValue(zombieBoss.Health);

            if (zombieBoss.IsDead)
            {
                _audioStub.Stop(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.6);

                _player.SetWinStance();
                _gameScoreBar.GainScore(3);

                LevelUp();

                _gameView.ActivateSlowMotion();
                ToggleNightMode(false);
            }
        }

        private bool ZombieBossExists()
        {
            return _gameView.GameObjects.OfType<ZombieBoss>().Any(x => x.IsAnimating);
        }

        #endregion

        #region ZombieBossRocketBlock

        private void SpawnZombieBossRocketBlocks()
        {
            for (int i = 0; i < 5; i++)
            {
                ZombieBossRocketBlock zombieBossRocket = new(
                    animateAction: AnimateZombieBossRocketBlock,
                    recycleAction: RecycleZombieBossRocketBlock);

                zombieBossRocket.SetZ(z: 7);
                zombieBossRocket.MoveOutOfSight();

                _gameView.AddToView(zombieBossRocket);

                SpawnDropShadow(source: zombieBossRocket);
            }
        }

        private void GenerateZombieBossRocketBlock()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<ZombieBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is ZombieBoss zombieBoss &&
                _gameView.GameObjects.OfType<ZombieBossRocketBlock>().FirstOrDefault(x => x.IsAnimating == false) is ZombieBossRocketBlock zombieBossRocket)
            {
                zombieBossRocket.Reset();
                zombieBossRocket.SetPopping();
                zombieBossRocket.Reposition();
                zombieBossRocket.IsAnimating = true;

                GenerateDropShadow(source: zombieBossRocket);
            }
        }

        private void AnimateZombieBossRocketBlock(GameObject zombieBossRocket)
        {
            ZombieBossRocketBlock zombieBossRocket1 = zombieBossRocket as ZombieBossRocketBlock;

            var speed = zombieBossRocket1.Speed;

            zombieBossRocket1.MoveDownRight(speed);

            if (zombieBossRocket1.IsBlasting)
            {
                zombieBossRocket.Expand();
                zombieBossRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                zombieBossRocket.Pop();
                zombieBossRocket1.Hover();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (zombieBossRocket.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                    {
                        zombieBossRocket1.SetBlast();
                        LoosePlayerHealth();
                    }

                    if (zombieBossRocket1.AutoBlast())
                        zombieBossRocket1.SetBlast();
                }
            }
        }

        private void RecycleZombieBossRocketBlock(GameObject zombieBossRocket)
        {
            var hitbox = zombieBossRocket.GetHitBox();

            var scaling = ScreenExtensions.GetScreenSpaceScaling();

            // if bomb is blasted and faed or goes out of scene bounds
            if (zombieBossRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH * scaling || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT * scaling)
            {
                zombieBossRocket.IsAnimating = false;
            }
        }

        private void LooseZombieBossRocketBlockHealth(ZombieBossRocketBlock zombieBossRocket)
        {
            zombieBossRocket.LooseHealth();
            GenerateFloatingNumber(zombieBossRocket);
        }

        #endregion

        #endregion

        #region MafiaBoss

        #region MafiaBoss

        private void SpawnMafiaBosses()
        {
            MafiaBoss mafiaBoss = new(
                animateAction: AnimateMafiaBoss,
                recycleAction: RecycleMafiaBoss);

            mafiaBoss.SetZ(z: 8);
            mafiaBoss.MoveOutOfSight();

            _gameView.AddToView(mafiaBoss);

            SpawnDropShadow(source: mafiaBoss);
        }

        private void GenerateMafiaBoss()
        {
            // if scene doesn't contain a MafiaBoss then pick a MafiaBoss and add to scene

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _mafiaBossCheckpoint.ShouldRelease(_gameScoreBar.GetScore()) && !MafiaBossExists() &&
                _gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating == false) is MafiaBoss mafiaBoss)
            {
                _audioStub.Stop(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.2);

                mafiaBoss.Reset();
                mafiaBoss.SetPosition(
                    left: 0,
                    top: mafiaBoss.Height * -1);
                mafiaBoss.IsAnimating = true;

                GenerateDropShadow(source: mafiaBoss);

                // set MafiaBoss health
                mafiaBoss.Health = _mafiaBossCheckpoint.GetReleasePointDifference() * 1.5;

                _mafiaBossCheckpoint.IncreaseThreasholdLimit(increment: _mafiaBossReleasePoint_increase, currentPoint: _gameScoreBar.GetScore());

                _mafiaBossHealthBar.SetMaxiumHealth(mafiaBoss.Health);
                _mafiaBossHealthBar.SetValue(mafiaBoss.Health);
                _mafiaBossHealthBar.SetIcon(mafiaBoss.GetContentUri());
                _mafiaBossHealthBar.SetBarColor(color: Colors.Crimson);

                _gameView.ActivateSlowMotion();

                GenerateInterimScreen("Beware of Crimson Mafia");

                ToggleNightMode(true);
            }
        }

        private void AnimateMafiaBoss(GameObject mafiaBoss)
        {
            MafiaBoss mafiaBoss1 = mafiaBoss as MafiaBoss;

            if (mafiaBoss1.IsDead)
            {
                mafiaBoss.Shrink();
            }
            else
            {
                mafiaBoss.Pop();

                mafiaBoss1.Hover();
                mafiaBoss1.DepleteHitStance();
                mafiaBoss1.DepleteWinStance();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var speed = mafiaBoss1.Speed;
                    var scaling = ScreenExtensions.GetScreenSpaceScaling();

                    if (mafiaBoss1.IsAttacking)
                    {
                        mafiaBoss1.Move(
                            speed: speed,
                            sceneWidth: Constants.DEFAULT_SCENE_WIDTH * scaling,
                            sceneHeight: Constants.DEFAULT_SCENE_HEIGHT * scaling,
                            playerPoint: _player.GetCloseHitBox());


                        if (mafiaBoss1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                        {
                            LoosePlayerHealth();
                        }
                    }
                    else
                    {
                        mafiaBoss1.MoveDownRight(speed);

                        if (mafiaBoss.GetLeft() > (Constants.DEFAULT_SCENE_WIDTH * scaling / 3)) // bring MafiaBoss to a suitable distance from player and then start attacking
                        {
                            mafiaBoss1.IsAttacking = true;
                        }
                    }
                }
            }
        }

        private void RecycleMafiaBoss(GameObject mafiaBoss)
        {
            if (mafiaBoss.IsShrinkingComplete)
            {
                mafiaBoss.IsAnimating = false;
            }
        }

        private void LooseMafiaBossHealth(MafiaBoss mafiaBoss)
        {
            mafiaBoss.SetPopping();
            mafiaBoss.LooseHealth();
            mafiaBoss.SetHitStance();

            GenerateFloatingNumber(mafiaBoss);

            _mafiaBossHealthBar.SetValue(mafiaBoss.Health);

            if (mafiaBoss.IsDead)
            {
                _audioStub.Stop(SoundType.BOSS_BACKGROUND_MUSIC);
                _audioStub.Play(SoundType.GAME_BACKGROUND_MUSIC);
                _audioStub.SetVolume(SoundType.AMBIENCE, 0.6);

                _player.SetWinStance();
                _gameScoreBar.GainScore(3);

                LevelUp();

                _gameView.ActivateSlowMotion();
                ToggleNightMode(false);
            }
        }

        private bool MafiaBossExists()
        {
            return _gameView.GameObjects.OfType<MafiaBoss>().Any(x => x.IsAnimating);
        }

        #endregion

        #region MafiaBossRocket

        private void SpawnMafiaBossRockets()
        {
            for (int i = 0; i < 4; i++)
            {
                MafiaBossRocket mafiaBossRocket = new(
                    animateAction: AnimateMafiaBossRocket,
                    recycleAction: RecycleMafiaBossRocket);

                mafiaBossRocket.SetZ(z: 7);
                mafiaBossRocket.MoveOutOfSight();

                _gameView.AddToView(mafiaBossRocket);

                SpawnDropShadow(source: mafiaBossRocket);
            }
        }

        private void GenerateMafiaBossRocket()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is MafiaBoss mafiaBoss &&
                _gameView.GameObjects.OfType<MafiaBossRocket>().FirstOrDefault(x => x.IsAnimating == false) is MafiaBossRocket mafiaBossRocket)
            {
                mafiaBossRocket.Reset();
                mafiaBossRocket.SetPopping();
                mafiaBossRocket.Reposition(mafiaBoss: mafiaBoss);
                mafiaBossRocket.IsAnimating = true;

                GenerateDropShadow(source: mafiaBossRocket);
                SetBossRocketDirection(source: mafiaBoss, rocket: mafiaBossRocket, rocketTarget: _player);
            }
        }

        private void AnimateMafiaBossRocket(GameObject mafiaBossRocket)
        {
            MafiaBossRocket mafiaBossRocket1 = mafiaBossRocket as MafiaBossRocket;

            var speed = mafiaBossRocket1.Speed;

            if (mafiaBossRocket1.AwaitMoveDownLeft)
            {
                mafiaBossRocket1.MoveDownLeft(speed);
            }
            else if (mafiaBossRocket1.AwaitMoveUpRight)
            {
                mafiaBossRocket1.MoveUpRight(speed);
            }
            else if (mafiaBossRocket1.AwaitMoveUpLeft)
            {
                mafiaBossRocket1.MoveUpLeft(speed);
            }
            else if (mafiaBossRocket1.AwaitMoveDownRight)
            {
                mafiaBossRocket1.MoveDownRight(speed);
            }

            if (mafiaBossRocket1.IsBlasting)
            {
                mafiaBossRocket.Expand();
                mafiaBossRocket.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
            }
            else
            {
                mafiaBossRocket.Pop();
                mafiaBossRocket1.Hover();

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (mafiaBossRocket.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                    {
                        mafiaBossRocket1.SetBlast();
                        LoosePlayerHealth();
                    }

                    if (mafiaBossRocket1.AutoBlast())
                        mafiaBossRocket1.SetBlast();
                }
            }
        }

        private void RecycleMafiaBossRocket(GameObject mafiaBossRocket)
        {
            var hitbox = mafiaBossRocket.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (mafiaBossRocket.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Top > Constants.DEFAULT_SCENE_HEIGHT)
            {
                mafiaBossRocket.IsAnimating = false;
            }
        }

        #endregion                

        #region MafiaBossRocketBullsEye

        private void SpawnMafiaBossRocketBullsEyes()
        {
            for (int i = 0; i < 3; i++)
            {
                MafiaBossRocketBullsEye mafiaBossRocketBullsEye = new(
                    animateAction: AnimateMafiaBossRocketBullsEye,
                    recycleAction: RecycleMafiaBossRocketBullsEye);

                mafiaBossRocketBullsEye.SetZ(z: 7);
                mafiaBossRocketBullsEye.MoveOutOfSight();

                _gameView.AddToView(mafiaBossRocketBullsEye);

                SpawnDropShadow(source: mafiaBossRocketBullsEye);
            }
        }

        private void GenerateMafiaBossRocketBullsEye()
        {
            // generate a seeking bomb if one is not in scene
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING &&
                _gameView.GameObjects.OfType<MafiaBoss>().FirstOrDefault(x => x.IsAnimating && x.IsAttacking) is MafiaBoss mafiaBoss &&
                _gameView.GameObjects.OfType<MafiaBossRocketBullsEye>().FirstOrDefault(x => x.IsAnimating == false) is MafiaBossRocketBullsEye mafiaBossRocketBullsEye)
            {
                mafiaBossRocketBullsEye.Reset();
                mafiaBossRocketBullsEye.SetPopping();
                mafiaBossRocketBullsEye.Reposition(mafiaBoss: mafiaBoss);
                mafiaBossRocketBullsEye.SetTarget(_player.GetCloseHitBox());
                mafiaBossRocketBullsEye.IsAnimating = true;

                GenerateDropShadow(source: mafiaBossRocketBullsEye);
            }
        }

        private void AnimateMafiaBossRocketBullsEye(GameObject mafiaBossRocketBullsEye)
        {
            MafiaBossRocketBullsEye mafiaBossRocketBullsEye1 = mafiaBossRocketBullsEye as MafiaBossRocketBullsEye;

            var speed = mafiaBossRocketBullsEye1.Speed;

            if (mafiaBossRocketBullsEye1.IsBlasting)
            {
                mafiaBossRocketBullsEye.Expand();
                mafiaBossRocketBullsEye.Fade(Constants.DEFAULT_BLAST_FADE_SCALE);
                mafiaBossRocketBullsEye1.MoveDownRight(speed);
            }
            else
            {
                mafiaBossRocketBullsEye.Pop();
                mafiaBossRocketBullsEye.Rotate(rotationSpeed: 2.5);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    if (_gameView.GameObjects.OfType<MafiaBoss>().Any(x => x.IsAnimating && x.IsAttacking))
                    {
                        mafiaBossRocketBullsEye1.Move();

                        if (mafiaBossRocketBullsEye1.GetCloseHitBox().IntersectsWith(_player.GetCloseHitBox()))
                        {
                            mafiaBossRocketBullsEye1.SetBlast();
                            LoosePlayerHealth();
                        }
                        else
                        {
                            if (mafiaBossRocketBullsEye1.AutoBlast())
                                mafiaBossRocketBullsEye1.SetBlast();
                        }
                    }
                    else
                    {
                        mafiaBossRocketBullsEye1.SetBlast();
                    }
                }
            }
        }

        private void RecycleMafiaBossRocketBullsEye(GameObject mafiaBossRocketBullsEye)
        {
            var hitbox = mafiaBossRocketBullsEye.GetHitBox();

            // if bomb is blasted and faed or goes out of scene bounds
            if (mafiaBossRocketBullsEye.IsFadingComplete || hitbox.Left > Constants.DEFAULT_SCENE_WIDTH || hitbox.Right < 0 || hitbox.Bottom < 0 || hitbox.Bottom > Constants.DEFAULT_SCENE_HEIGHT)
            {
                mafiaBossRocketBullsEye.IsAnimating = false;
            }
        }

        #endregion 

        #endregion

        #region Honk

        private void SpawnHonks()
        {
            for (int i = 0; i < 10; i++)
            {
                Honk honk = new(
                    animateAction: AnimateHonk,
                    recycleAction: RecycleHonk);

                honk.SetZ(z: 5);
                honk.MoveOutOfSight();

                _gameView.AddToView(honk);
            }
        }

        private void GenerateHonk(GameObject source)
        {
            if (_gameView.GameObjects.OfType<Honk>().FirstOrDefault(x => x.IsAnimating == false) is Honk honk)
            {
                honk.SetPopping();

                honk.Reset();

                var hitBox = source.GetCloseHitBox();

                honk.Reposition(source: source);
                honk.SetRotation(_random.Next(-30, 30));
                honk.SetZ(source.GetZ() + 1);

                source.SetPopping();
                honk.IsAnimating = true;
            }
        }

        private void AnimateHonk(GameObject honk)
        {
            honk.Pop();
            honk.Fade(0.06);
        }

        private void RecycleHonk(GameObject honk)
        {
            if (honk.IsFadingComplete)
            {
                honk.IsAnimating = false;
            }
        }

        private void GenerateVehicleBossHonk(VehicleBoss source)
        {
            // if there are no UfoBosses or enemies in the scene the vehicles will honk

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !UfoBossExists())
            {
                GenerateHonk(source);
            }
        }

        private void GenerateVehicleEnemyHonk(VehicleEnemy source)
        {
            // if there are no UfoBosses or enemies in the scene the vehicles will honk

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !UfoEnemyExists() && !AnyBossExists())
            {
                GenerateHonk(source);
            }
        }

        private void GenerateUfoEnemyHonk(UfoEnemy source)
        {
            // if there are no UfoBosses in the scene the vehicles will honk

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && !UfoBossExists())
            {
                GenerateHonk(source);
            }
        }

        #endregion

        #region Cloud

        private void SpawnClouds()
        {
            for (int i = 0; i < 3; i++)
            {
                Cloud cloud = new(
                    animateAction: AnimateCloud,
                    recycleAction: RecycleCloud);

                cloud.SetZ(z: 9);
                cloud.MoveOutOfSight();

                _gameView.AddToView(cloud);
            }
        }

        private void GenerateCloud()
        {
            if (!AnyBossExists() && _gameView.GameObjects.OfType<Cloud>().FirstOrDefault(x => x.IsAnimating == false) is Cloud cloud)
            {
                cloud.Reset();

                var topOrLeft = _random.Next(2);

                var lane = _random.Next(2);

                switch (topOrLeft)
                {
                    case 0:
                        {
                            var xLaneWidth = Constants.DEFAULT_SCENE_WIDTH / 4;
                            cloud.SetPosition(
                                left: _random.Next(Convert.ToInt32(xLaneWidth - cloud.Width)),
                                top: cloud.Height * -1);
                        }
                        break;
                    case 1:
                        {
                            var yLaneWidth = (Constants.DEFAULT_SCENE_HEIGHT / 2) / 2;
                            cloud.SetPosition(
                                left: cloud.Width * -1,
                                top: _random.Next(Convert.ToInt32(yLaneWidth)));
                        }
                        break;
                    default:
                        break;
                }
                cloud.IsAnimating = true;
            }
        }

        private void AnimateCloud(GameObject cloud)
        {
            Cloud cloud1 = cloud as Cloud;
            cloud1.Hover();

            var speed = cloud1.Speed;
            cloud1.MoveDownRight(speed);
        }

        private void RecycleCloud(GameObject cloud)
        {
            var hitBox = cloud.GetHitBox();

            if (hitBox.Top > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left > Constants.DEFAULT_SCENE_WIDTH)
            {
                cloud.IsAnimating = false;
            }
        }

        #endregion

        #region DropShadow

        private void SpawnDropShadow(GameObject source)
        {
            DropShadow dropShadow = new(
                animateAction: AnimateDropShadow,
                recycleAction: RecycleDropShadow);

            _gameView.AddToView(dropShadow);

            dropShadow.SetParent(construct: source);
            dropShadow.Move();
            dropShadow.SetZ(source.GetZ() - 1);
        }

        private void AnimateDropShadow(GameObject construct)
        {
            DropShadow dropShadow = construct as DropShadow;
            dropShadow.Move();
        }

        private void RecycleDropShadow(GameObject dropShadow)
        {
            DropShadow dropShadow1 = dropShadow as DropShadow;

            if (!dropShadow1.IsParentConstructAnimating())
            {
                dropShadow.IsAnimating = false;
            }
        }

        private void GenerateDropShadow(GameObject source)
        {
            if (_gameView.GameObjects.OfType<DropShadow>().FirstOrDefault(x => x.Id == source.Id) is DropShadow dropShadow)
            {
                dropShadow.SetZ(source.GetZ() - 2);
                dropShadow.Reset();
                dropShadow.IsAnimating = true;
            }
        }

        #endregion

        #region Pickup

        #region HealthPickup

        private void SpawnHealthPickups()
        {
            for (int i = 0; i < 3; i++)
            {
                HealthPickup healthPickup = new(
                    animateAction: AnimateHealthPickup,
                    recycleAction: RecycleHealthPickup);

                healthPickup.SetZ(z: 6);
                healthPickup.MoveOutOfSight();

                _gameView.AddToView(healthPickup);
            }
        }

        private void GenerateHealthPickups()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING && HealthPickup.ShouldGenerate(_player.Health) &&
                _gameView.GameObjects.OfType<HealthPickup>().FirstOrDefault(x => x.IsAnimating == false) is HealthPickup healthPickup)
            {
                healthPickup.Reset();

                var topOrLeft = _random.Next(2);
                var lane = _random.Next(2);

                switch (topOrLeft)
                {
                    case 0:
                        {
                            var xLaneWidth = Constants.DEFAULT_SCENE_WIDTH / 4;
                            healthPickup.SetPosition(
                                left: _random.Next(Convert.ToInt32(xLaneWidth - healthPickup.Width)),
                                top: healthPickup.Height * -1);
                        }
                        break;
                    case 1:
                        {
                            var yLaneWidth = (Constants.DEFAULT_SCENE_HEIGHT / 2) / 2;
                            healthPickup.SetPosition(
                                left: healthPickup.Width * -1,
                                top: _random.Next(Convert.ToInt32(yLaneWidth)));
                        }
                        break;
                    default:
                        break;
                }

                healthPickup.IsAnimating = true;
            }
        }

        private void AnimateHealthPickup(GameObject healthPickup)
        {
            HealthPickup healthPickup1 = healthPickup as HealthPickup;

            var speed = healthPickup1.Speed;

            if (healthPickup1.IsPickedUp)
            {
                healthPickup1.Shrink();
            }
            else
            {
                healthPickup1.MoveDownRight(speed);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var hitbox = healthPickup.GetCloseHitBox();

                    if (_player.GetCloseHitBox().IntersectsWith(hitbox))
                    {
                        healthPickup1.PickedUp();

                        _player.GainHealth();
                        _playerHealthBar.SetValue(_player.Health);
                    }
                }
            }
        }

        private void RecycleHealthPickup(GameObject healthPickup)
        {
            var hitBox = healthPickup.GetHitBox();

            if (healthPickup.IsShrinkingComplete || hitBox.Top - healthPickup.Height > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - healthPickup.Width > Constants.DEFAULT_SCENE_WIDTH) // if object is out side of bottom right corner
            {
                healthPickup.IsAnimating = false;
            }
        }

        #endregion

        #region PowerUpPickup

        private void SpawnPowerUpPickups()
        {
            for (int i = 0; i < 3; i++)
            {
                PowerUpPickup powerUpPickup = new(
                    animateAction: AnimatePowerUpPickup,
                    recycleAction: RecyclePowerUpPickup);

                powerUpPickup.SetZ(z: 6);
                powerUpPickup.MoveOutOfSight();

                _gameView.AddToView(powerUpPickup);
            }
        }

        private void GeneratePowerUpPickup()
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
            {
                if ((AnyInAirBossExists() || UfoEnemyExists()) && !_powerUpMeter.HasHealth) // if any in air boss or enemy exists and currently player has no other power up
                {
                    if (_gameView.GameObjects.OfType<PowerUpPickup>().FirstOrDefault(x => x.IsAnimating == false) is PowerUpPickup powerUpPickup)
                    {
                        powerUpPickup.Reset();

                        var topOrLeft = _random.Next(2);

                        var lane = _random.Next(2);

                        switch (topOrLeft)
                        {
                            case 0:
                                {
                                    var xLaneWidth = Constants.DEFAULT_SCENE_WIDTH / 4;

                                    powerUpPickup.SetPosition(
                                        left: _random.Next(Convert.ToInt32(xLaneWidth - powerUpPickup.Width)),
                                        top: powerUpPickup.Height * -1);
                                }
                                break;
                            case 1:
                                {
                                    var yLaneWidth = (Constants.DEFAULT_SCENE_HEIGHT / 2) / 2;

                                    powerUpPickup.SetPosition(
                                        left: powerUpPickup.Width * -1,
                                        top: _random.Next(Convert.ToInt32(yLaneWidth)));
                                }
                                break;
                            default:
                                break;
                        }

                        powerUpPickup.IsAnimating = true;
                    }
                }
            }
        }

        private void AnimatePowerUpPickup(GameObject powerUpPickup)
        {
            PowerUpPickup powerUpPickup1 = powerUpPickup as PowerUpPickup;

            var speed = powerUpPickup1.Speed;

            if (powerUpPickup1.IsPickedUp)
            {
                powerUpPickup1.Shrink();
            }
            else
            {
                powerUpPickup1.MoveDownRight(speed);

                if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
                {
                    var hitbox = powerUpPickup.GetCloseHitBox();

                    if (_player.GetCloseHitBox().IntersectsWith(hitbox))
                    {
                        powerUpPickup1.PickedUp();

                        _powerUpMeter.Tag = powerUpPickup1.PowerUpType;

                        switch (powerUpPickup1.PowerUpType)
                        {
                            case PowerUpType.SEEKING_SNITCH: // if seeking snitch powerup, allow using a burst of 3 seeking bombs 3 times
                                {
                                    _powerUpMeter.SetMaxiumHealth(9);
                                    _powerUpMeter.SetValue(9);

                                    GenerateInterimScreen("Seeking Snitch +9");
                                }
                                break;
                            case PowerUpType.BULLS_EYE: // if bulls eye powerup, allow using a single shot of 20 bombs
                                {
                                    _powerUpMeter.SetMaxiumHealth(20);
                                    _powerUpMeter.SetValue(20);

                                    GenerateInterimScreen("Bylls Eye +20");
                                }
                                break;
                            case PowerUpType.ARMOR:
                                {
                                    _powerUpMeter.SetMaxiumHealth(10); // if armor powerup then take additional 10 hits
                                    _powerUpMeter.SetValue(10);

                                    GenerateInterimScreen("Armor +10");
                                }
                                break;
                            default:
                                break;
                        }

                        _powerUpMeter.SetIcon(powerUpPickup1.GetContentUri());
                        _powerUpMeter.SetBarColor(color: Colors.Green);
                    }
                }
            }
        }

        private void RecyclePowerUpPickup(GameObject powerUpPickup)
        {
            var hitBox = powerUpPickup.GetHitBox();

            if (hitBox.Top - powerUpPickup.Height > Constants.DEFAULT_SCENE_HEIGHT || hitBox.Left - powerUpPickup.Width > Constants.DEFAULT_SCENE_WIDTH || powerUpPickup.IsShrinkingComplete)
            {
                powerUpPickup.IsAnimating = false;
            }
        }

        private void DepletePowerUp()
        {
            // use up the power up
            if (_powerUpMeter.HasHealth)
                _powerUpMeter.SetValue(_powerUpMeter.GetValue() - 1);
        }

        #endregion      

        #endregion

        #region FloatingNumber

        private void SpawnFloatingNumbers()
        {
            for (int i = 0; i < 5; i++)
            {
                FloatingNumber floatingNumber = new(
                    animateAction: AnimateFloatingNumber,
                    recycleAction: RecycleFloatingNumber);

                floatingNumber.SetZ(z: 10);
                floatingNumber.MoveOutOfSight();

                _gameView.AddToView(floatingNumber);
            }
        }

        private void GenerateFloatingNumber(AnimableHealthyBase source)
        {
            if (!_gameView.IsSlowMotionActivated && _gameView.GameObjects.OfType<FloatingNumber>().FirstOrDefault(x => x.IsAnimating == false) is FloatingNumber floatingNumberTop)
            {
                floatingNumberTop.Reset(source.HitPoint);
                floatingNumberTop.Reposition(source);
                floatingNumberTop.IsAnimating = true;
            }
        }

        private void AnimateFloatingNumber(GameObject floatingNumber)
        {
            FloatingNumber floatingNumber1 = floatingNumber as FloatingNumber;
            floatingNumber1.Move();
            floatingNumber1.DepleteOnScreenDelay();
        }

        private void RecycleFloatingNumber(GameObject floatingNumber)
        {
            FloatingNumber floatingNumber1 = floatingNumber as FloatingNumber;

            if (floatingNumber1.IsDepleted)
            {
                floatingNumber.IsAnimating = false;
            }
        }

        #endregion

        #region Rocket

        private void SetPlayerRocketDirection(GameObject source, AnimableBase rocket, GameObject rocketTarget)
        {
            // rocket target is on the bottom right side of the UfoBoss
            if (rocketTarget.GetTop() > source.GetTop() && rocketTarget.GetLeft() > source.GetLeft())
            {
                rocket.AwaitMoveDownRight = true;
                rocket.SetRotation(33);
            }
            // rocket target is on the bottom left side of the UfoBoss
            else if (rocketTarget.GetTop() > source.GetTop() && rocketTarget.GetLeft() < source.GetLeft())
            {
                rocket.AwaitMoveDownLeft = true;
                rocket.SetRotation(-213);
            }
            // if rocket target is on the top left side of the UfoBoss
            else if (rocketTarget.GetTop() < source.GetTop() && rocketTarget.GetLeft() < source.GetLeft())
            {
                rocket.AwaitMoveUpLeft = true;
                rocket.SetRotation(213);
            }
            // if rocket target is on the top right side of the UfoBoss
            else if (rocketTarget.GetTop() < source.GetTop() && rocketTarget.GetLeft() > source.GetLeft())
            {
                rocket.AwaitMoveUpRight = true;
                rocket.SetRotation(-33);
            }
            else
            {
                rocket.AwaitMoveUpLeft = true;
                rocket.SetRotation(213);
            }
        }

        private void SetBossRocketDirection(GameObject source, AnimableBase rocket, GameObject rocketTarget)
        {
            // rocket target is on the bottom right side of the UfoBoss
            if (rocketTarget.GetTop() > source.GetTop() && rocketTarget.GetLeft() > source.GetLeft())
            {
                rocket.AwaitMoveDownRight = true;
                rocket.SetRotation(33);
            }
            // rocket target is on the bottom left side of the UfoBoss
            else if (rocketTarget.GetTop() > source.GetTop() && rocketTarget.GetLeft() < source.GetLeft())
            {
                rocket.AwaitMoveDownLeft = true;
                rocket.SetRotation(-213);
            }
            // if rocket target is on the top left side of the UfoBoss
            else if (rocketTarget.GetTop() < source.GetTop() && rocketTarget.GetLeft() < source.GetLeft())
            {
                rocket.AwaitMoveUpLeft = true;
                rocket.SetRotation(213);
            }
            // if rocket target is on the top right side of the UfoBoss
            else if (rocketTarget.GetTop() < source.GetTop() && rocketTarget.GetLeft() > source.GetLeft())
            {
                rocket.AwaitMoveUpRight = true;
                rocket.SetRotation(-33);
            }
            else
            {
                rocket.AwaitMoveDownRight = true;
                rocket.SetRotation(33);
            }
        }

        #endregion

        #region Boss

        private bool AnyBossExists()
        {
            return (UfoBossExists() || VehicleBossExists() || ZombieBossExists() || MafiaBossExists());
        }

        private bool AnyInAirBossExists()
        {
            return (UfoBossExists() || ZombieBossExists() || MafiaBossExists());
        }

        #endregion 

        #endregion

        #region Controller

        private void SetController()
        {
            _gameController.SetScene(_gameView);
            _gameController.SetGyrometer();
        }

        private void UnsetController()
        {
            _gameController.SetScene(null);
            _gameController.UnsetGyrometer();
        }

        private void ToggleHudVisibility(Visibility visibility)
        {
            _gameController.Visibility = visibility;
            _gameScoreBar.Visibility = visibility;
            _healthBars.Visibility = visibility;
            _soundPollutionMeter.Visibility = visibility;
        }

        #endregion

        #region GameViews

        private void PrepareGameView()
        {
            _gameView.Clear();

            #region Player

            SpawnPlayerBalloon();
            SpawnPlayerRockets();
            SpawnPlayerHonkBombs();
            SpawnPlayerRocketSeekings();
            SpawnPlayerRocketBullsEyes();

            #endregion

            #region Road

            SpawnRoadMarksContainer();
            SpawnRoadSideWalksContainer();
            SpawnRoadSideTreesContainer();
            SpawnRoadSideHedgesContainer();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 213,
                elaspedAction: GenerateRoadMarksContainer),

            new GameObjectGenerator(
                delay: 110,
                elaspedAction: GenerateRoadSideWalksContainerTop),

            new GameObjectGenerator(
                delay: 110,
                elaspedAction: GenerateRoadSideWalksContainerBottom),

            new GameObjectGenerator(
                delay: 178,
                elaspedAction: GenerateRoadSideTreesContainerTop),

            new GameObjectGenerator(
                delay: 178,
                elaspedAction: GenerateRoadSideTreesContainerBottom),

            new GameObjectGenerator(
                delay: 182,
                elaspedAction: GenerateRoadSideHedgesContainerTop),

            new GameObjectGenerator(
                delay: 182,
                elaspedAction: GenerateRoadSideHedgesContainerBottom)
                );

            //SpawnRoadMarks();
            //SpawnRoadSideWalks();
            //SpawnRoadSideTrees();
            //SpawnRoadSideHedges();
            SpawnRoadSideLamps();
            SpawnRoadSideBillboards();
            SpawnRoadSideLightBillboards();

            _gameView.AddToView(

            //new GameObjectGenerator(
            //    delay: 38,
            //    elaspedAction: GenerateRoadMark),

            //new GameObjectGenerator(
            //    delay: 18,
            //    elaspedAction: GenerateRoadSideWalk),


            //new GameObjectGenerator(
            //    delay: 30,
            //    elaspedAction: GenerateRoadSideTree),

            //new GameObjectGenerator(
            //    delay: 38,
            //    elaspedAction: GenerateRoadSideHedge),

            new GameObjectGenerator(
                delay: 36,
                elaspedAction: GenerateRoadSideLamp),

            new GameObjectGenerator(
                delay: 72,
                elaspedAction: GenerateRoadSideBillboard),          

            new GameObjectGenerator(
                delay: 36,
                elaspedAction: GenerateRoadSideLightBillboard)
           );

            #endregion

            #region UfoEnemy

            SpawnUfoEnemys();
            SpawnUfoEnemyRockets();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 180,
                elaspedAction: GenerateUfoEnemy,
                scramble: true));

            #endregion

            #region UfoBoss

            SpawnUfoBosses();
            SpawnUfoBossRockets();
            SpawnUfoBossRocketSeekings();

            _gameView.AddToView(

            new GameObjectGenerator(
                delay: 10,
                elaspedAction: GenerateUfoBoss),

            new GameObjectGenerator(
                delay: 40,
                elaspedAction: GenerateUfoBossRocket,
                scramble: true),

            new GameObjectGenerator(
                delay: 200,
                elaspedAction: GenerateUfoBossRocketSeeking,
                scramble: true)
                );

            #endregion

            #region FloatingNumber

            SpawnFloatingNumbers();

            #endregion

            #region Pickup

            SpawnHealthPickups();
            SpawnPowerUpPickups();

            _gameView.AddToView(

            new GameObjectGenerator(
                delay: 800,
                elaspedAction: GenerateHealthPickups),

            new GameObjectGenerator(
                delay: 800,
                elaspedAction: GeneratePowerUpPickup));

            #endregion            

            #region Cloud

            SpawnClouds();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 300,
                elaspedAction: GenerateCloud,
                scramble: true));

            #endregion

            #region VehicleEnemy

            SpawnVehicleEnemys();
            SpawnHonks();

            _gameView.AddToView(new GameObjectGenerator(
                delay: 95,
                elaspedAction: GenerateVehicleEnemy));

            #endregion

            #region VehicleBoss

            SpawnVehicleBosses();
            SpawnVehicleBossRockets();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 10,
                elaspedAction: GenerateVehicleBoss),

            new GameObjectGenerator(
                delay: 50,
                elaspedAction: GenerateVehicleBossRocket,
                scramble: true));

            #endregion

            #region ZombieBoss

            SpawnZombieBosses();
            SpawnZombieBossRocketBlocks();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 10,
                elaspedAction: GenerateZombieBoss),

            new GameObjectGenerator(
                delay: 30,
                elaspedAction: GenerateZombieBossRocketBlock)
            );

            #endregion

            #region MafiaBoss

            SpawnMafiaBosses();
            SpawnMafiaBossRockets();
            SpawnMafiaBossRocketBullsEyes();

            _gameView.AddToView(
            new GameObjectGenerator(
                delay: 10,
                elaspedAction: GenerateMafiaBoss),

            new GameObjectGenerator(
                delay: 75,
                elaspedAction: GenerateMafiaBossRocket),

            new GameObjectGenerator(
                delay: 45,
                elaspedAction: GenerateMafiaBossRocketBullsEye));

            #endregion
        }

        private void PrepareMainMenuView()
        {
            _mainMenuScene.Clear();

            SpawnAssetsLoadingScreen();
            SpawnInterimScreen();
            SpawnGameStartScreen();
            SpawnPlayerCharacterSelectionScreen();
            SpawnPlayerHonkBombSelectionScreen();
            SpawnPromptOrientationChangeScreen();
        }

        private void SetSceneScaling()
        {
            var scaling = ScreenExtensions.GetScreenSpaceScaling();

            LoggingExtensions.Log($"ScreenSpaceScaling: {scaling}");

            // resize the game scene
            _gameView.Width = ScreenExtensions.Width;
            _gameView.Height = ScreenExtensions.Height;

            // resize the main menu
            _mainMenuScene.Width = ScreenExtensions.Width;
            _mainMenuScene.Height = ScreenExtensions.Height;

            // scale the scenes
            _gameView.SetScaleTransform(scaling);
            _mainMenuScene.SetScaleTransform(scaling);
        }

        #endregion

        #endregion

        #region Events

        #region Load

        private void HonkBusterPage_Loaded(object sender, RoutedEventArgs e)
        {
            ScreenExtensions.DisplayInformation.OrientationChanged += DisplayInformation_OrientationChanged;
            ScreenExtensions.SetRequiredDisplayOrientations(DisplayOrientations.Landscape, DisplayOrientations.LandscapeFlipped);

            // set display orientation to required orientation
            if (!ScreenExtensions.IsScreenInRequiredOrientation())
                ScreenExtensions.ChangeDisplayOrientationAsRequired();

            SetController();
            PrepareMainMenuView();
            _mainMenuScene.Play();

            SizeChanged += HonkBusterPage_SizeChanged;

            if (ScreenExtensions.IsScreenInRequiredOrientation()) // if the screen is in desired orientation the show asset loading screen
            {
                ScreenExtensions.EnterFullScreen(true);
                GenerateAssetsLoadingScreen(); // if generators are not added to game scene, show the assets loading screen               
            }
            else
            {
                GeneratePromptOrientationChangeScreen();
            }
        }

        private void HonkBusterPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= HonkBusterPage_SizeChanged;
            ScreenExtensions.DisplayInformation.OrientationChanged -= DisplayInformation_OrientationChanged;
            UnsetController();
        }

        #endregion

        #region Size

        private void HonkBusterPage_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            ScreenExtensions.Width = args.NewSize.Width <= Constants.DEFAULT_SCENE_WIDTH ? args.NewSize.Width : Constants.DEFAULT_SCENE_WIDTH;
            ScreenExtensions.Height = args.NewSize.Height <= Constants.DEFAULT_SCENE_HEIGHT ? args.NewSize.Height : Constants.DEFAULT_SCENE_HEIGHT;

            SetSceneScaling();

            if (_gameView.GameViewState == GameViewState.GAME_RUNNING)
            {
                _player.Reposition();
                GenerateDropShadow(source: _player);
            }

            RepositionHoveringTitleScreens();
            LoggingExtensions.Log($"Width: {ScreenExtensions.Width} x Height: {ScreenExtensions.Height}");
        }

        #endregion

        #region Orientation

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            if (_gameView.GameViewState == GameViewState.GAME_RUNNING) // if screen orientation is changed while game is running, pause the game
            {
                PauseGame();
            }
            else
            {
                ScreenExtensions.EnterFullScreen(true);

                if (ScreenExtensions.IsScreenInRequiredOrientation())
                {
                    if (_mainMenuScene.GameObjects.OfType<PromptOrientationChangeScreen>().FirstOrDefault(x => x.IsAnimating) is PromptOrientationChangeScreen promptOrientationChangeScreen)
                    {
                        RecyclePromptOrientationChangeScreen(promptOrientationChangeScreen);
                        GenerateAssetsLoadingScreen();
                    }
                }
                else // ask to change orientation
                {
                    _gameView.Pause();
                    _mainMenuScene.Pause();

                    _audioStub.Pause(SoundType.GAME_BACKGROUND_MUSIC);

                    foreach (var hoveringTitleScreen in _mainMenuScene.GameObjects.OfType<HoveringTitleScreen>().Where(x => x.IsAnimating))
                    {
                        hoveringTitleScreen.IsAnimating = false;
                    }

                    foreach (var construct in _gameView.GameObjects.OfType<GameObject>())
                    {
                        construct.IsAnimating = false;
                    }

                    GeneratePromptOrientationChangeScreen();
                }
            }

            LoggingExtensions.Log($"CurrentOrientation: {sender.CurrentOrientation}");
        }

        #endregion

        #endregion
    }
}
