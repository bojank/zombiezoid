using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Zombiezoid
{
    public class Player
    {
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation shootAnimation;
        private Animation dieAnimation;
        private SpriteEffects effects = SpriteEffects.None;
        private Sprite sprite;
        private Random rand = new Random();

        private SoundEffect killedSound;
        private SoundEffect gunShot1;

        public int Health
        {
            get { return health; }
        }
        public int health;

        public Game Game
        {
            get { return game; }
        }
        Game game;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public float Angle
        {
            get { return angle; }
        }
        float angle;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        private const float WalkSpeed = 150.0f;
        private const float RunSpeed = 250.0f;

        private bool isShooting;
        private float shootWait;
        
        
        private bool isRunning;

        private float movementRight;
        private float movementForward;


        public Vector2 MousePosition
        {
            get { return mousePosition; }
        }
        public Vector2 mousePosition;

        private Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Player(Game game)//Map map, Vector2 position)
        {
            this.game = game;

            LoadContent();
            Reset();
        }

        public void LoadContent()
        {
            // Load animated textures.
            idleAnimation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/PlayerIdle"), 0.1f, true);
            runAnimation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/PlayerIdle"), 0.1f, true);//new Animation(map.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            shootAnimation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/PlayerShoot"), 0.2f, false);// new Animation(map.Content.Load<Texture2D>("Sprites/Player/Shoot"), 0.1f, false);
            dieAnimation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/PlayerIdle"), 0.1f, true);//new Animation(map.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);aw
            gunShot1 = game.Content.Load<SoundEffect>("Sounds/GunShot1");

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.8);// (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = (idleAnimation.FrameHeight - height) / 2;
            localBounds = new Rectangle(left, top, width, height);

            // Load sounds.            
            //killedSound = map.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            //shootSound = map.Content.Load<SoundEffect>("Sounds/PlayerShoot");
        }

        public void Reset()
        {
            int x = rand.Next(1, game.map.Width - 2);
            int y = rand.Next(1, game.map.Height - 2);
            position = RectangleExtensions.GetBottomCenter(game.map.GetBounds(x, y));
            //position = new Vector2(100, 100);
            health = 200;
            shootWait = 0.0f;
            isShooting = false;
            isRunning = false;
            //Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            
            //mousePosition = position;
            Mouse.SetPosition((int)position.X, (int)position.Y);
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            MouseState mouseState,
            Vector2 screenCenter)
            //TouchCollection touchState, 
            //AccelerometerState accelState,
            //DisplayOrientation orientation)
        {

            GetInput(keyboardState, mouseState, gameTime, screenCenter);//orientation,

            ApplyPhysics(gameTime, mouseState, screenCenter);
            if (isShooting) {
                sprite.PlayAnimation(shootAnimation);
            }
            else if (IsAlive)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0 || Math.Abs(Velocity.Y) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimation);
                }
            }

            movementRight = 0.0f;
            movementForward = 0.0f;
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(
            KeyboardState keyboardState,
            MouseState mouseState,
            //DisplayOrientation orientation,
            GameTime gameTime,
            Vector2 screenCenter)
        {
           
            
            if (keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movementRight = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movementRight = 1.0f;
            }
            else
            {
                movementRight = 0.0f;
            }

            if (keyboardState.IsKeyDown(Keys.Down) ||
                keyboardState.IsKeyDown(Keys.S))
            {
                movementForward = -1.0f;
            }
            else if (keyboardState.IsKeyDown(Keys.Up) ||
                     keyboardState.IsKeyDown(Keys.W))
            {
                movementForward = 1.0f;
            }
            else
            {
                movementForward = 0.0f;
            }


            //THIS 640 and 360 hare hard coded! FIX ASAP
            //this.mousePosition.X += mousePosition.X;
            //this.mousePosition.Y += mousePosition.Y;
            //mousePosition.X = Mouse.GetState().X;//mouseState.X - screenCenter.X;
            //mousePosition.Y = Mouse.GetState().Y;//mouseState.Y - screenCenter.Y;

            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                isShooting = true;
            }

            if (keyboardState.IsKeyDown(Keys.LeftShift) && !isRunning ||
                keyboardState.IsKeyDown(Keys.RightShift) && !isRunning)
            {
                isRunning = true;
            }
            else if (keyboardState.IsKeyUp(Keys.LeftShift) && isRunning ||
                     keyboardState.IsKeyUp(Keys.RightShift) && isRunning)
            {
                isRunning = false;
            }

        }

        protected void Shoot(GameTime gameTime)
        {
            
            if (isShooting)
            {
                if (shootWait == 0.0f)
                {
                    sprite.PlayAnimation(shootAnimation);
                    //gunShot1.Play();
                    shootWait = 0.2f;
                    game.projectileManager.CreateProjectile(position,angle,this);
                }
                else
                {
                    shootWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (shootWait < 0.0f || shootWait == 0.0f)
                {
                    isShooting = false;
                    shootWait = 0.0f;
                }
            }

        }

        public void ApplyPhysics(GameTime gameTime, MouseState mouseState, Vector2 screenCenter)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            angle = (float)Math.Atan2(mousePosition.Y - Position.Y, mousePosition.X - Position.X);
            Vector2 previousPosition = Position;

            Shoot(gameTime);

            velocity.X = movementRight;
            velocity.Y = -movementForward;
            //if(velocity != Vector2.Zero)
            //    velocity.Normalize();
            //velocity *= (isRunning ? RunSpeed*elapsed : WalkSpeed*elapsed);
            //velocity = Vector2.Zero;

            //Weird Controls ?
            //if (movementForward > 0.0f)
            //{
            //    velocity = new Vector2((float)Math.Cos((double)angle), (float)Math.Sin((double)angle));
            //}
            //else if (movementForward < 0.0f)
            //{
            //    velocity = new Vector2((float)Math.Cos((double)angle + (float)Math.PI), (float)Math.Sin((double)angle + (float)Math.PI)); //new Vector2(angle + (float)Math.PI);
            //}

            //if (movementRight > 0.0f)
            //{
            //    velocity = Vector2.Add(velocity,new Vector2((float)Math.Cos((double)angle + (float)(Math.PI / 2)), (float)Math.Sin((double)angle + (float)(Math.PI / 2))));//Vector2.Add(velocity, new Vector2(angle - (float)(Math.PI/2)));
            //}
            //else if (movementRight < 0.0f)
            //{
            //    velocity = Vector2.Add(velocity,new Vector2((float)Math.Cos((double)angle + (float)(Math.PI / 2) + (float)(Math.PI)), (float)Math.Sin((double)angle + (float)(Math.PI / 2) + (float)(Math.PI))));//Vector2.Add(velocity,new Vector2(angle - (float)(Math.PI/2)+ (float)(Math.PI)));
            //}
            if (velocity != Vector2.Zero)
                    velocity.Normalize();
            velocity *= (isRunning ? RunSpeed * elapsed : WalkSpeed * elapsed);

            Position += velocity ;
            //mousePosition = new Vector2(mouseState.X, mouseState.Y);// - velocity; 

            HandleCollisions();
            if (Position.X.Equals(previousPosition.X))
                velocity.X = 0;
            if (Position.Y.Equals(previousPosition.Y))
                velocity.Y = 0;

            mousePosition = position + new Vector2(mouseState.X, mouseState.Y) - screenCenter ; 
            
        }

        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;


            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileType collision = game.map.GetCollision(x, y);
                    if (collision != TileType.Ground)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = game.map.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (collision == TileType.Wall)
                            {
                                if (absDepthY < absDepthX)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);
                                    velocity.Y += depth.Y;
                                    mousePosition.Y += depth.Y;
                                }
                                if (absDepthX < absDepthY)
                                {
                                    Position = new Vector2(Position.X + depth.X, Position.Y);
                                    velocity.X += depth.X;
                                    mousePosition.X += depth.X;
                                }

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
        }

        public void DoDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                
                
                Reset();
            }
        }

        public void OnKilled(Zombie killedBy)
        {
            isAlive = false;
            killedSound.Play();
            sprite.PlayAnimation(dieAnimation);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.Draw(gameTime, spriteBatch, Position, angle);
        }

    }
}
