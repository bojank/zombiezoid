using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace Zombiezoid
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private KeyboardState oldState;
        private SpriteBatch spriteBatch;
        private SpriteFont hudFont;
        public Map map;
        private Texture2D crosshair;

        public GamePadState gamePadState;
        private KeyboardState keyboardState;
        private MouseState mouseState;
        private Random random;


        const float detectionDefault = 500.0f;
        const float separationDefault = 80.0f;
        const float moveInOldDirInfluenceDefault = 1f;
        const float moveInFlockDirInfluenceDefault = 1f;
        const float moveInRandomDirInfluenceDefault = 0.5f;
        const float maxTurnRadiansDefault = 6.0f;
        const float perMemberWeightDefault = 2.0f;
        const float perDangerWeightDefault = 50.0f;

        public Hoard hoard;
        //private Vector2 mousePosition;
        AIParameters hoardParams;

        //public ProjectileManager ProjectileManager
        //{
        //    get { return projectileManager; }
        //}
        public ProjectileManager projectileManager;

        private const int EntityLayer = 2;

        //public Player Player
        //{
        //    get { return player; }
        //}
        public Player player;


        //public List<Zombie> Zombies
        //{
        //    get { return zombies; }
        //}
        //public List<Zombie> zombies;

        private Vector2 screenCenter;
        //private int cx;
        //private int cy;

        public Game()
        {
            
            
            graphics = new GraphicsDeviceManager(this);
            //graphics.ToggleFullScreen();
            //graphics.IsFullScreen = true;

            graphics.PreferredBackBufferHeight = 720;//1080;// 600;//
            graphics.PreferredBackBufferWidth = 1280;//1920;// 800;//
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            hoard = null;

            hoardParams = new AIParameters();
            ResetAIParams();
        }

        protected override void Initialize()
        {
            oldState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            //this.Content.Load<Texture2D>("Sprites/Zombie/ZombieIdle");
            //this.Content.Load<Texture2D>("Sprites/Zombie/ZombieAttack");
            //this.Content.Load<Texture2D>("Sprites/Player/PlayerIdle");
            //this.Content.Load<Texture2D>("Sprites/Player/PlayerShoot");
            this.Content.Load<Texture2D>("Sprites/Projectiles/Default");
            this.Content.Load<Texture2D>("Sprites/Other/Crosshair");
            this.Content.Load<Texture2D>("Tiles/Ground");
            this.Content.Load<Texture2D>("Tiles/Wall");
            //this.Content.Load<SoundEffect>;

            screenCenter.X = graphics.GraphicsDevice.Viewport.Width / 2;
            screenCenter.Y = graphics.GraphicsDevice.Viewport.Height / 2;

            LoadMap("test");
            player = new Player(this);
            //zombies = new List<Zombie>();
            projectileManager = new ProjectileManager(this);
            random = new Random();
            //for (int zombie = 0; zombie < 10; zombie++)
            //{

            //    zombies.Add(new Zombie(this, random));
            //}
            
            crosshair = map.Content.Load<Texture2D>("Sprites/Other/Crosshair");
        }

        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //mousePosition = map.Player.mousePosition;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateInput();
            //mousePosition.X = mouseState.X - screenCenter.X;
            //mousePosition.Y = mouseState.Y - screenCenter.Y;
            player.Update(gameTime, keyboardState, mouseState, screenCenter);//orientation);
            if (hoard != null)
            {
                hoard.Update(gameTime);
            }
            else
            {
                SpawnHoard();
            }
            projectileManager.Update(gameTime);
            //UpdateEnemies(gameTime);
            

            map.Update(gameTime, keyboardState, mouseState, screenCenter);//, Window.CurrentOrientation);
            
            base.Update(gameTime);
        }

        //private void UpdateEnemies(GameTime gameTime)
        //{
        //    foreach (Zombie enemy in zombies)
        //    {
        //        enemy.Update(gameTime);
        //    }
        //}

        private void UpdateInput()
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);
            mouseState = Mouse.GetState();
            
            //might need this.
            //if (keyboardState.IsKeyDown(Keys.Space))
            //{
            //    if (!oldState.IsKeyDown(Keys.Space))
            //    {
            //        //Pressed
            //    }
            //}
            //else if (oldState.IsKeyDown(Keys.Space))
            //{
            //    //Released
            //}

            if (keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            oldState = keyboardState;
        }

        private void LoadMap(string mapName)
        {

            // Unloads the content for the current level before loading the next one.
            if (map != null)
                map.Dispose();

            // Load the level.

            string levelPath = string.Format("Content/Maps/{0}.txt", mapName);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                map = new Map(Services, fileStream);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            //spriteBatch.Begin();
            Matrix transform = Matrix.Identity *
                        Matrix.CreateTranslation(-player.Position.X, -player.Position.Y, 0) *
                        Matrix.CreateRotationZ(0.0f) *
                        Matrix.CreateTranslation(graphics.PreferredBackBufferWidth / 2.0f, graphics.PreferredBackBufferHeight / 2.0f, 0);
                

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null, null, transform);
            map.Draw(gameTime, spriteBatch);
            projectileManager.Draw(gameTime, spriteBatch);
            player.Draw(gameTime, spriteBatch);

            //foreach (Zombie enemy in zombies)
            //    enemy.Draw(gameTime, spriteBatch);
            if (hoard != null)
            {
                hoard.Draw(spriteBatch, gameTime);
            }

            //mousePosition += map.Player.Velocity;
            //Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
            DrawCrosshair();
            
            DrawHud();
            

            //spriteBatch.End();
            
            

            //spriteBatch.Draw(_heliTexture, _heliPosition, heliSourceRectangle, Color.White, 0.0f, new Vector2(0, 0), 0.5f, SpriteEffects.FlipHorizontally, 0.0f);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawCrosshair()
        {
            //Adjust crosshair position
            Vector2 crosshairPosition = new Vector2(player.MousePosition.X - crosshair.Width / 2, player.MousePosition.Y - crosshair.Height / 2);
            spriteBatch.Draw(crosshair, crosshairPosition, Color.White);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);
            //DrawLine(spriteBatch, 1, Color.Black, map.Player.Position, map.Player.mousePosition);

            DrawShadowedString(hudFont, player.mousePosition.X + " : " + player.mousePosition.Y + " | ", hudLocation, Color.Black);

        }

        //public void DrawRectamgle
        //{

        //}
        protected void SpawnHoard()
        {
            if (hoard == null)
            {
                hoard = new Hoard(this, random, hoardParams);
            }
        }
       public void DrawLine(SpriteBatch batch, float width, Color color, Vector2 point1, Vector2 point2)
        {
            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });

            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       player.Angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        public void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }

        private void ResetAIParams()
        {
            hoardParams.DetectionDistance = detectionDefault;
            hoardParams.SeparationDistance = separationDefault;
            hoardParams.MoveInOldDirectionInfluence = moveInOldDirInfluenceDefault;
            hoardParams.MoveInFlockDirectionInfluence = moveInFlockDirInfluenceDefault;
            hoardParams.MoveInRandomDirectionInfluence = moveInRandomDirInfluenceDefault;
            hoardParams.MaxTurnRadians = maxTurnRadiansDefault;
            hoardParams.PerMemberWeight = perMemberWeightDefault;
            hoardParams.PerDangerWeight = perDangerWeightDefault;
        }
    }
}