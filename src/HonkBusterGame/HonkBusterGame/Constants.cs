namespace HonkBusterGame
{
    static class Constants
    {
        public const double DEFAULT_FRAME_TIME = 19;

        public const int DEFAULT_CONSTRUCT_SPEED = 6;

        public const double DEFAULT_SLOW_MOTION_REDUCTION_FACTOR = 4;
        public const double DEFAULT_ISOMETRIC_DISPLACEMENT = 0.5;

        public const double DEFAULT_DROP_SHADOW_DISTANCE = 65;

        public const double DEFAULT_CONTROLLER_KEY_SIZE = 70;
        public const double DEFAULT_CONTROLLER_KEY_CORNER_RADIUS = 30;
        public const double DEFAULT_CONTROLLER_KEY_BORDER_THICKNESS = 4;
        public const double DEFAULT_CONTROLLER_DIRECTION_KEYS_MARGIN = 6;

        public const double DEFAULT_GUI_FONT_SIZE = 30;

        public const double DEFAULT_SCENE_WIDTH = 1900;
        public const double DEFAULT_SCENE_HEIGHT = 940;

        public const double DEFAULT_BLAST_RING_CORNER_RADIUS = 100;
        public const double DEFAULT_BLAST_RING_BORDER_THICKNESS = 10;
        public const double DEFAULT_BLAST_SHRINK_SCALE = 0.8; // 0.4;
        public const double DEFAULT_BLAST_FADE_SCALE = 0.02;

        public const double DEFAULT_ROCKET_SIZE = 85;

        public const double DEFAULT_HOVERING_SCREEN_OPACITY = 0.9;

        public static (ConstructType ConstructType, double Width, double Height)[] CONSTRUCT_SIZES = new (ConstructType, double, double)[]
        {
            new (ConstructType.PLAYER_BALLOON, 165, 165),
            new (ConstructType.PLAYER_ROCKET, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),
            new (ConstructType.PLAYER_HONK_BOMB, DEFAULT_ROCKET_SIZE - 25, DEFAULT_ROCKET_SIZE - 25),
            new (ConstructType.PLAYER_ROCKET_SEEKING, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),
            new (ConstructType.PLAYER_ROCKET_BULLS_EYE, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),

            new (ConstructType.UFO_BOSS, 180, 180),
            new (ConstructType.UFO_BOSS_ROCKET, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),
            new (ConstructType.UFO_BOSS_ROCKET_SEEKING, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),

            new (ConstructType.MAFIA_BOSS, 180, 180),
            new (ConstructType.MAFIA_BOSS_ROCKET, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),
            new (ConstructType.MAFIA_BOSS_ROCKET_BULLS_EYE, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),

            new (ConstructType.ZOMBIE_BOSS, 180, 180),
            new (ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK, 200, 200),

            new (ConstructType.UFO_ENEMY, 160, 160),
            new (ConstructType.UFO_ENEMY_ROCKET, DEFAULT_ROCKET_SIZE, DEFAULT_ROCKET_SIZE),

            new (ConstructType.VEHICLE_ENEMY_SMALL, 230, 230),
            new (ConstructType.VEHICLE_ENEMY_LARGE, 260, 260),

            new (ConstructType.VEHICLE_BOSS, 260, 260),
            new (ConstructType.VEHICLE_BOSS_ROCKET, DEFAULT_ROCKET_SIZE + 15, DEFAULT_ROCKET_SIZE + 15),

            new (ConstructType.CLOUD, 260, 260),
            new (ConstructType.HONK, 90, 90),
            new (ConstructType.FLOATING_NUMBER, 50, 50),

            new (ConstructType.ROAD_MARK, 512, 512),

            new (ConstructType.ROAD_SIDE_TREE, 470, 470),
            new (ConstructType.ROAD_SIDE_HEDGE, 470, 470),

            new (ConstructType.ROAD_SIDE_WALK, 300, 300),
            new (ConstructType.ROAD_SIDE_LAMP, 150, 150),
            new (ConstructType.ROAD_SIDE_LIGHT_BILLBOARD, 150, 150),
            new (ConstructType.ROAD_SIDE_BILLBOARD, 300, 300),

            new (ConstructType.DROP_SHADOW, 60, 25),

            new (ConstructType.HEALTH_PICKUP, 100, 100),
            new (ConstructType.POWERUP_PICKUP, 100, 100),

            new (ConstructType.TITLE_SCREEN, DEFAULT_CONTROLLER_KEY_SIZE * 10, 400),
            new (ConstructType.INTERIM_SCREEN, DEFAULT_CONTROLLER_KEY_SIZE * 10, 400),
        };

        public static (ConstructType ConstructType, Uri Uri)[] CONSTRUCT_TEMPLATES = new (ConstructType, Uri)[]
        {
            new (ConstructType.ROAD_MARK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_marks.png")),

            new (ConstructType.ROAD_SIDE_TREE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/tree_1.png")),
            new (ConstructType.ROAD_SIDE_TREE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/tree_2.png")),

            new (ConstructType.ROAD_SIDE_LAMP, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_lamp_1.png")),
            new (ConstructType.ROAD_SIDE_LAMP, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_lamp_2.png")),

            new (ConstructType.ROAD_SIDE_LIGHT_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_light_billboard_1.png")),
            new (ConstructType.ROAD_SIDE_LIGHT_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_light_billboard_2.png")),
            new (ConstructType.ROAD_SIDE_LIGHT_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_light_billboard_3.png")),

            new (ConstructType.ROAD_SIDE_HEDGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_hedge_1.png")),

            new (ConstructType.ROAD_SIDE_WALK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/road_side_walk_1.png")),

            new (ConstructType.ROAD_SIDE_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/billboard_1.png")),
            new (ConstructType.ROAD_SIDE_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/billboard_2.png")),
            new (ConstructType.ROAD_SIDE_BILLBOARD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/billboard_3.png")),

            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_1.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_2.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_3.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_4.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_5.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_6.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_7.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_8.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_9.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_10.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_11.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_12.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_13.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_14.png")),
            new (ConstructType.VEHICLE_ENEMY_SMALL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_small_15.png")),

            new (ConstructType.VEHICLE_ENEMY_LARGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_large_1.png")),
            new (ConstructType.VEHICLE_ENEMY_LARGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_large_2.png")),
            new (ConstructType.VEHICLE_ENEMY_LARGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_large_3.png")),
            new (ConstructType.VEHICLE_ENEMY_LARGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_large_4.png")),
            new (ConstructType.VEHICLE_ENEMY_LARGE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_large_5.png")),

            new (ConstructType.VEHICLE_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/vehicle_boss_rocket.png")),

            new (ConstructType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/honk_1.png")),
            new (ConstructType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/honk_2.png")),
            new (ConstructType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/honk_3.png")),

            new (ConstructType.HONK_BUSTER, new Uri("ms-appx:///HonkBusterGame/Assets/Images/honk_buster_1.png")),
            new (ConstructType.HONK_BUSTER, new Uri("ms-appx:///HonkBusterGame/Assets/Images/honk_buster_2.png")),

            new (ConstructType.PLAYER_BALLOON, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_1_character.png")),
            new (ConstructType.PLAYER_BALLOON_IDLE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_1_idle.png")),
            new (ConstructType.PLAYER_BALLOON_ATTACK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_1_attack.png")),
            new (ConstructType.PLAYER_BALLOON_WIN, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_1_win.png")),
            new (ConstructType.PLAYER_BALLOON_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_1_hit.png")),

            new (ConstructType.PLAYER_BALLOON, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_2_character.png")),
            new (ConstructType.PLAYER_BALLOON_IDLE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_2_idle.png")),
            new (ConstructType.PLAYER_BALLOON_ATTACK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_2_attack.png")),
            new (ConstructType.PLAYER_BALLOON_WIN, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_2_win.png")),
            new (ConstructType.PLAYER_BALLOON_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_balloon_2_hit.png")),

            new (ConstructType.PLAYER_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_bomb_1.png")),
            new (ConstructType.PLAYER_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_bomb_2.png")),
            new (ConstructType.PLAYER_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_bomb_3.png")),

            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cracker_1.png")),
            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cracker_2.png")),
            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cracker_3.png")),

            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/trash_1.png")),
            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/trash_2.png")),
            new (ConstructType.PLAYER_HONK_BOMB, new Uri("ms-appx:///HonkBusterGame/Assets/Images/trash_3.png")),

            new (ConstructType.PLAYER_ROCKET_SEEKING, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_rocket_seeking_1.png")),
            new (ConstructType.PLAYER_ROCKET_SEEKING, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_rocket_seeking_2.png")),

            new (ConstructType.PLAYER_ROCKET_BULLS_EYE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/player_rocket_bulls_eye_1.png")),

            new (ConstructType.BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Images/blast_1.png")),
            new (ConstructType.BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Images/blast_2.png")),

            new (ConstructType.BANG, new Uri("ms-appx:///HonkBusterGame/Assets/Images/bang_1.png")),
            new (ConstructType.BANG, new Uri("ms-appx:///HonkBusterGame/Assets/Images/bang_2.png")),

            new (ConstructType.CLOUD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cloud_1.png")),
            new (ConstructType.CLOUD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cloud_2.png")),
            new (ConstructType.CLOUD, new Uri("ms-appx:///HonkBusterGame/Assets/Images/cloud_3.png")),

            new (ConstructType.UFO_BOSS_IDLE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_1_idle.png")),
            new (ConstructType.UFO_BOSS_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_1_hit.png")),
            new (ConstructType.UFO_BOSS_WIN, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_1_win.png")),

            new (ConstructType.MAFIA_BOSS_IDLE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_1_idle.png")),
            new (ConstructType.MAFIA_BOSS_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_1_hit.png")),
            new (ConstructType.MAFIA_BOSS_WIN, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_1_win.png")),

            new (ConstructType.ZOMBIE_BOSS_IDLE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_1_idle.png")),
            new (ConstructType.ZOMBIE_BOSS_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_1_hit.png")),
            new (ConstructType.ZOMBIE_BOSS_WIN, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_1_win.png")),

            new (ConstructType.UFO_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_rocket_1.png")),
            new (ConstructType.UFO_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_rocket_2.png")),
            new (ConstructType.UFO_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_rocket_3.png")),

            new (ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_cube_1.png")),
            new (ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_cube_2.png")),
            new (ConstructType.ZOMBIE_BOSS_ROCKET_BLOCK, new Uri("ms-appx:///HonkBusterGame/Assets/Images/zombie_boss_cube_3.png")),

            new (ConstructType.UFO_BOSS_ROCKET_SEEKING, new Uri("ms-appx:///HonkBusterGame/Assets/Images/ufo_boss_rocket_seeking.png")),
            new (ConstructType.MAFIA_BOSS_ROCKET_BULLS_EYE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_rocket_bulls_eye.png")),

            new (ConstructType.UFO_ENEMY, new Uri("ms-appx:///HonkBusterGame/Assets/Images/enemy_1.png")),
            new (ConstructType.UFO_ENEMY, new Uri("ms-appx:///HonkBusterGame/Assets/Images/enemy_2.png")),
            new (ConstructType.UFO_ENEMY, new Uri("ms-appx:///HonkBusterGame/Assets/Images/enemy_3.png")),
            new (ConstructType.UFO_ENEMY, new Uri("ms-appx:///HonkBusterGame/Assets/Images/enemy_4.png")),

            new (ConstructType.UFO_ENEMY_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/enemy_bomb.png")),

            new (ConstructType.HEALTH_PICKUP, new Uri("ms-appx:///HonkBusterGame/Assets/Images/health_pickup.png")),
            new (ConstructType.POWERUP_PICKUP_ARMOR, new Uri("ms-appx:///HonkBusterGame/Assets/Images/powerup_pickup_armor.png")),
            new (ConstructType.POWERUP_PICKUP_BULLS_EYE, new Uri("ms-appx:///HonkBusterGame/Assets/Images/powerup_pickup_bulls_eye.png")),
            new (ConstructType.POWERUP_PICKUP_SEEKING_SNITCH, new Uri("ms-appx:///HonkBusterGame/Assets/Images/powerup_pickup_seeking_snitch.png")),

            new (ConstructType.MAFIA_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_rocket_1.png")),
            new (ConstructType.MAFIA_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_rocket_2.png")),
            new (ConstructType.MAFIA_BOSS_ROCKET, new Uri("ms-appx:///HonkBusterGame/Assets/Images/mafia_boss_rocket_3.png")),

            new (ConstructType.MENU_PANEL, new Uri("ms-appx:///HonkBusterGame/Assets/Images/blue_panel.png")),
        };

        public static (SoundType SoundType, Uri Uri)[] SOUND_TEMPLATES = new (SoundType, Uri)[]
        {
            new (SoundType.CRACKER_DROP, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_drop_1.mp3")),
            new (SoundType.CRACKER_DROP, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_drop_2.mp3")),

            new (SoundType.CRACKER_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_blast_1.mp3")),
            new (SoundType.CRACKER_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_blast_2.mp3")),
            new (SoundType.CRACKER_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_blast_3.mp3")),
            new (SoundType.CRACKER_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/cracker_blast_4.mp3")),

            new (SoundType.TRASH_CAN_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/trashcan_hit_1.mp3")),
            new (SoundType.TRASH_CAN_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/trashcan_hit_2.mp3")),
            new (SoundType.TRASH_CAN_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/trashcan_hit_3.mp3")),
            new (SoundType.TRASH_CAN_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/trashcan_hit_4.mp3")),
            new (SoundType.TRASH_CAN_HIT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/trashcan_hit_5.mp3")),

            new (SoundType.ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_launch_1.mp3")),
            new (SoundType.ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_launch_2.mp3")),
            new (SoundType.ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_launch_3.mp3")),

            new (SoundType.ROCKET_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_blast_1.mp3")),
            new (SoundType.ROCKET_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_blast_2.mp3")),
            new (SoundType.ROCKET_BLAST, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/rocket_blast_3.mp3")),

            new (SoundType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/car_honk_1.mp3")),
            new (SoundType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/car_honk_2.mp3")),
            new (SoundType.HONK, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/car_honk_3.mp3")),

            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_1.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_2.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_3.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_4.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_5.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_6.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_7.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_8.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_9.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_10.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_11.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_12.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_13.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_14.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_15.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_16.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_17.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_18.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_19.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_20.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_21.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_22.mp3")),
            new (SoundType.HONK_BUST_REACTION, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/honk_bust_reaction_23.mp3")),

            new (SoundType.SEEKER_ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/seeker_rocket_launch_1.mp3")),
            new (SoundType.SEEKER_ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/seeker_rocket_launch_2.mp3")),

            new (SoundType.BULLS_EYE_ROCKET_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/bulls_eye_rocket_launch_1.mp3")),

            new (SoundType.AMBIENCE, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ambience_1.mp3")),
            new (SoundType.AMBIENCE, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ambience_2.mp3")),
            new (SoundType.AMBIENCE, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ambience_3.mp3")),

            new (SoundType.UFO_HOVERING, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_hovering_1.mp3")),
            new (SoundType.UFO_HOVERING, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_hovering_2.mp3")),
            new (SoundType.UFO_HOVERING, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_hovering_3.mp3")),

            new (SoundType.UFO_BOSS_ENTRY, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_entry_1.mp3")),
            new (SoundType.UFO_BOSS_ENTRY, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_entry_2.mp3")),

            new (SoundType.UFO_BOSS_DEAD, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_dead_1.mp3")),
            new (SoundType.UFO_BOSS_DEAD, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_boss_dead_2.mp3")),

            new (SoundType.POWERUP_PICKUP, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/power_up_pickup_1.mp3")),
            new (SoundType.HEALTH_PICKUP, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/health_pickup_1.mp3")),

            new (SoundType.PLAYER_HEALTH_LOSS, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/player_health_loss_1.mp3")),

            new (SoundType.UFO_ENEMY_ENTRY, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_enemy_entry_1.mp3")),
            new (SoundType.UFO_ENEMY_ENTRY, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/ufo_enemy_entry_2.mp3")),

            new (SoundType.GAME_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_background_music_1.mp3")),
            new (SoundType.GAME_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_background_music_2.mp3")),
            new (SoundType.GAME_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_background_music_3.mp3")),
            new (SoundType.GAME_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_background_music_4.mp3")),

            new (SoundType.BOSS_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/boss_background_music_1.mp3")),
            new (SoundType.BOSS_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/boss_background_music_2.mp3")),
            new (SoundType.BOSS_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/boss_background_music_3.mp3")),
            new (SoundType.BOSS_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/boss_background_music_4.mp3")),
            new (SoundType.BOSS_BACKGROUND_MUSIC, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/boss_background_music_5.mp3")),

            new (SoundType.GAME_START, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_start.mp3")),
            new (SoundType.GAME_PAUSE, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_pause.mp3")),
            new (SoundType.GAME_OVER, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/game_over.mp3")),

            new (SoundType.ORB_LAUNCH, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/orb_launch.mp3")),

            new (SoundType.LEVEL_UP, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/level_up.mp3")),
            new (SoundType.OPTION_SELECT, new Uri("ms-appx:///HonkBusterGame/Assets/Sounds/option_select.mp3")),
        };
    }

    public enum ConstructType
    {
        NONE,

        PLAYER_BALLOON,
        PLAYER_BALLOON_IDLE,
        PLAYER_BALLOON_ATTACK,
        PLAYER_BALLOON_WIN,
        PLAYER_BALLOON_HIT,

        PLAYER_ROCKET,
        PLAYER_ROCKET_SEEKING,
        PLAYER_ROCKET_BULLS_EYE,
        PLAYER_HONK_BOMB,

        VEHICLE_ENEMY_SMALL,
        VEHICLE_ENEMY_LARGE,
        VEHICLE_BOSS,

        VEHICLE_BOSS_ROCKET,

        ROAD_MARK,
        ROAD_SIDE_WALK,

        ROAD_SIDE_TREE,
        ROAD_SIDE_HEDGE,
        ROAD_SIDE_LAMP,
        ROAD_SIDE_LIGHT_BILLBOARD,
        ROAD_SIDE_BILLBOARD,

        HONK,
        HONK_BUSTER,

        CLOUD,

        BLAST,
        BANG,

        DROP_SHADOW,

        UFO_BOSS,
        UFO_BOSS_IDLE,
        UFO_BOSS_HIT,
        UFO_BOSS_WIN,

        MAFIA_BOSS,
        MAFIA_BOSS_IDLE,
        MAFIA_BOSS_HIT,
        MAFIA_BOSS_WIN,

        ZOMBIE_BOSS,
        ZOMBIE_BOSS_IDLE,
        ZOMBIE_BOSS_HIT,
        ZOMBIE_BOSS_WIN,

        UFO_BOSS_ROCKET,
        UFO_BOSS_ROCKET_SEEKING,

        MAFIA_BOSS_ROCKET,
        MAFIA_BOSS_ROCKET_BULLS_EYE,

        ZOMBIE_BOSS_ROCKET_BLOCK,

        UFO_ENEMY,
        UFO_ENEMY_ROCKET,

        HEALTH_PICKUP,

        POWERUP_PICKUP,
        POWERUP_PICKUP_SEEKING_SNITCH,
        POWERUP_PICKUP_ARMOR,
        POWERUP_PICKUP_BULLS_EYE,

        COLLECTABLE_PICKUP,
        FLOATING_NUMBER,

        TITLE_SCREEN,
        INTERIM_SCREEN,
        MENU_PANEL,
    }

    public enum SoundType
    {
        GAME_START,
        GAME_PAUSE,
        GAME_OVER,

        CRACKER_DROP,
        CRACKER_BLAST,

        TRASH_CAN_HIT,

        ROCKET_LAUNCH,
        ROCKET_BLAST,

        HONK,
        HONK_BUST_REACTION,

        SEEKER_ROCKET_LAUNCH,
        BULLS_EYE_ROCKET_LAUNCH,

        AMBIENCE,

        UFO_BOSS_ENTRY,
        UFO_HOVERING,
        UFO_BOSS_DEAD,

        POWERUP_PICKUP,
        HEALTH_PICKUP,

        PLAYER_HEALTH_LOSS,

        UFO_ENEMY_ENTRY,

        GAME_BACKGROUND_MUSIC,
        BOSS_BACKGROUND_MUSIC,

        ORB_LAUNCH,

        LEVEL_UP,
        OPTION_SELECT
    }
}
