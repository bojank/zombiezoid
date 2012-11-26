using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Zombiezoid
{
    public enum EnemyType
    {
        // no type
        Generic,
        // flies around and reacts
        Zombie
    }
    /// <summary>
    /// base class for moveable, drawable critters onscreen
    /// </summary>
    public class Enemy
    {
        public Game Game
        {
            get { return game; }
        }
        public Game game;

        public int Health
        {
            get { return Health; }
        }
        public int health;

        public bool isAlive;        

        public Vector2 position;

        public Vector2 velocity;

        public Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Animation idleAnimation;
        public Sprite sprite;
        public string spriteSet;
        public Texture2D whiteBox;

        public float MoveSpeed;
        /// </summary>
        protected Dictionary<EnemyType, Behaviors> behaviors;

        public Vector2 aiNewDir;
        public int aiNumSeen;
        /// <summary>
        /// The animal type
        /// </summary>
        public EnemyType EnemyType
        {
            get
            {
                return enemytype;
            }
        }
        protected EnemyType enemytype = EnemyType.Generic;

        /// <summary>
        /// Reaction distance
        /// </summary>
        public float ReactionDistance
        {
            get
            {
                return reactionDistance;
            }
        }
        protected float reactionDistance;

        /// <summary>
        /// Reaction location
        /// </summary>
        public Vector2 ReactionLocation
        {
            get
            {
                return reactionLocation;
            }
        }
        protected Vector2 reactionLocation;

        public bool Fleeing
        {
            get
            {
                return fleeing;
            }
            set 
            { 
                fleeing = value; 
            }
        }
        protected bool fleeing = false;

        public int BoundryWidth
        {
            get
            {
                return boundryWidth;
            }
        }
        protected int boundryWidth;

        public int BoundryHeight
        {
            get
            {
                return boundryHeight;
            }
        }
        protected int boundryHeight;

        /// <summary>
        /// Direction the animal is moving in
        /// </summary>
        public float direction;

        /// <summary>
        /// Location on screen
        /// </summary>


        #region Initialization
        /// <summary>
        /// Sets the boundries the animal can move in the texture used in Draw
        /// </summary>
        /// <param name="tex">Texture to use</param>
        /// <param name="screenSize">Size of the sample screen</param>
        /// 
        public Enemy()
        {
        }
        public Enemy(Game game, Random rnd)
        {
            
            this.game = game;
            int x = rnd.Next(1, game.map.Width - 2);
            int y = rnd.Next(1, game.map.Height - 2);
            //LoadEnemyTile(x, y, "Zombie");
            Vector2 position = RectangleExtensions.GetBottomCenter(game.map.GetBounds(x, y));
            this.position = position;
            direction =(float)rnd.NextDouble() * MathHelper.TwoPi;
            this.velocity = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
            velocity.Normalize();
            this.health = 1000;
            this.isAlive = true;
            behaviors = new Dictionary<EnemyType, Behaviors>();
            MoveSpeed = 0.1f;
            //BuildBehaviors();
            //LoadContent("Default");
        }
        public Enemy(int screenWidth, int screenHeight) 
        {
            //if (tex != null)
            //{
            //    texture = tex;
            //    textureCenter = new Vector2(texture.Width / 2, texture.Height / 2);
            //}
            boundryWidth = screenWidth;
            boundryHeight = screenHeight;
            MoveSpeed = 0.0f;

            behaviors = new Dictionary<EnemyType, Behaviors>();
        }

        public static Vector2 ChangeDirection(
            Vector2 oldDir, Vector2 newDir, float maxTurnRadians)
        {
            float oldAngle = (float)Math.Atan2(oldDir.Y, oldDir.X);
            float desiredAngle = (float)Math.Atan2(newDir.Y, newDir.X);
            float newAngle = MathHelper.Clamp(desiredAngle, WrapAngle(
                    oldAngle - maxTurnRadians), WrapAngle(oldAngle + maxTurnRadians));
            return new Vector2((float)Math.Cos(newAngle), (float)Math.Sin(newAngle));
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public void BuildBehaviors()
        {
            //Behaviors catReactions = new Behaviors();
            //catReactions.Add(new FleeBehavior(this));
            //behaviors.Add(EnemyType.Cat, catReactions);

            Behaviors birdReactions = new Behaviors();
            //birdReactions.Add(new FleeBehavior(this));
            birdReactions.Add(new AlignBehavior(this));
            birdReactions.Add(new CohesionBehavior(this));
            birdReactions.Add(new SeparationBehavior(this));
            
            behaviors.Add(EnemyType.Zombie, birdReactions);
        }

        //public void LoadContent(string spriteSet)
        //{
        //    // Load animations.
        //    spriteSet = "Sprites/" + spriteSet + "/";
        //    idleAnimation = new Animation(game.Content.Load<Texture2D>(spriteSet + "ZombieIdle"), 0.15f, true);
        //    sprite.PlayAnimation(idleAnimation);
        //    //whiteBox = new Texture2D(graphicsDevice,1,1);

        //    //whiteRectangle.SetData(new[] { Color.White });




        //    // Calculate bounds within texture size.
        //    int width = (int)(idleAnimation.FrameWidth * 0.8);
        //    int left = (idleAnimation.FrameWidth - width) / 2;
        //    int height = (int)(idleAnimation.FrameWidth * 0.8);
        //    int top = (idleAnimation.FrameHeight - height) / 2;
        //    localBounds = new Rectangle(left, top, width, height);

        //    //firstTime = true;
        //    //direction = (float)Math.Atan2(game.player.Position.Y - Position.Y, game.player.Position.X - Position.X);
        //    //velocity = new Vector2((float)Math.Cos((double)direction), (float)Math.Sin((double)direction));
        //}
        #endregion
        /// <summary>
        /// Setup the bird to figure out it's new movement direction
        /// </summary>
        /// <param name="AIparams">flock AI parameters</param>
        public void ResetThink()
        {
            Fleeing = false;
            aiNewDir = Vector2.Zero;
            aiNumSeen = 0;
            reactionDistance = 0;//localBounds.Width/2;
            reactionLocation = Vector2.Zero;
        }

        /// <summary>
        /// React to an Enemy based on it's type
        /// </summary>
        /// <param name="animal"></param>
        public void ReactTo(Enemy animal, ref AIParameters AIparams)
        {
            if (animal != null)
            {
                //setting the the reactionLocation and reactionDistance here is
                //an optimization, many of the possible reactions use the distance
                //and location of theAnimal, so we might as well figure them out
                //only once !
                Vector2 otherLocation = animal.position;
                ClosestLocation(ref position, ref otherLocation,
                    out reactionLocation);
                reactionDistance = Vector2.Distance(position, reactionLocation); //+ localBounds.Width/2;

                //we only react if theAnimal is close enough that we can see it
                if (reactionDistance < AIparams.DetectionDistance)
                {
                    Behaviors reactions = behaviors[animal.EnemyType];
                    foreach (Behavior reaction in reactions)
                    {
                        reaction.Update(animal, AIparams);
                        if (reaction.Reacted)
                        {
                            aiNewDir += reaction.Reaction;
                            aiNumSeen++;
                        }
                    }
                }
            }
        }

        public void ReactTo(Player player, ref AIParameters AIparams )
        {
            if (player != null)
            {
                
            }
        }

        //React to Tilewe
        public void ReactTo(Rectangle Tile, ref AIParameters AIparams)
        {
            Vector2 otherLocation = new Vector2(Tile.Left, Tile.Top);
            ClosestLocation(ref position, ref otherLocation,
                out reactionLocation);
            reactionDistance = Vector2.Distance(position, reactionLocation);
            Behaviors reactions = behaviors[EnemyType.Generic];
            foreach (Behavior reaction in reactions)
            {
                ((SeparationBehavior)reaction).Update(Tile, AIparams);
                if (reaction.Reacted)
                {
                    aiNewDir += reaction.Reaction;
                    aiNumSeen++;
                }
            }
        }

        public void DoDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
                isAlive = false;
        }

        /// <summary>
        /// Since we're wrapping movement around the screen, two point at extreme 
        /// sides of the screen are actually very close together, this function 
        /// figures out if destLocation is closer the srcLocation if you wrap around
        /// the screen
        /// </summary>
        /// <param name="srcLocation">screen location of src</param>
        /// <param name="destLocation">screen location of dest</param>
        /// <param name="outVector">relative location of dest to src</param>
        private void ClosestLocation(ref Vector2 srcLocation,
            ref Vector2 destLocation, out Vector2 outLocation)
        {
            outLocation = new Vector2();
            float x = destLocation.X;
            float y = destLocation.Y;
            float dX = Math.Abs(destLocation.X - srcLocation.X);
            float dY = Math.Abs(destLocation.Y - srcLocation.Y);

            // now see if the distance between birds is closer if going off one
            // side of the map and onto the other.
            if (Math.Abs(boundryWidth - destLocation.X + srcLocation.X) < dX)
            {
                dX = boundryWidth - destLocation.X + srcLocation.X;
                x = destLocation.X - boundryWidth;
            }
            if (Math.Abs(boundryWidth - srcLocation.X + destLocation.X) < dX)
            {
                dX = boundryWidth - srcLocation.X + destLocation.X;
                x = destLocation.X + boundryWidth;
            }

            if (Math.Abs(boundryHeight - destLocation.Y + srcLocation.Y) < dY)
            {
                dY = boundryHeight - destLocation.Y + srcLocation.Y;
                y = destLocation.Y - boundryHeight;
            }
            if (Math.Abs(boundryHeight - srcLocation.Y + destLocation.Y) < dY)
            {
                dY = boundryHeight - srcLocation.Y + destLocation.Y;
                y = destLocation.Y + boundryHeight;
            }
            outLocation.X = x;
            outLocation.Y = y;
        }

        #region Update and Draw

        public void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            //isOnGround = false;

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

                            if (collision == TileType.Wall)
                            {
                                // Y Axis Collision
                                if (absDepthY < absDepthX)
                                {
                                    position = new Vector2(position.X, position.Y + depth.Y);
                                    if (velocity.X > 0 && absDepthX < 44)
                                        position.X += absDepthY;
                                    else if (velocity.X < 0 && absDepthX < 44)
                                        position.X -= absDepthY;

                                }
                                //X Axis Collision
                                if (absDepthX < absDepthY)
                                {
                                    position = new Vector2(position.X + depth.X, position.Y);
                                    if (velocity.Y > 0 && absDepthY < 44)
                                        position.Y += absDepthX;
                                    else if (velocity.Y < 0 && absDepthY < 44)
                                        position.Y -= absDepthX;
                                }

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }


                            //else if (collision == TileType.Wall) // Ignore platforms.
                            //{
                            //    // Resolve the collision along the X axis.
                            //    Position = new Vector2(Position.X + depth.X, Position.Y);

                            //    // Perform further collisions with the new bounds.
                            //    bounds = BoundingRectangle;
                            //}
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            //previousBottom = bounds.Bottom;
        }
        /// <summary>
        /// Empty update function
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime, ref AIParameters aiParams)
        {
        }

        /// <summary>
        /// Draw the Enemy with the specified SpriteBatch
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //float rotation = (float)Math.Atan2(direction.Y, direction.X);            

            //spriteBatch.Draw(texture, location, null, color,
            //    rotation, textureCenter, 1.0f, SpriteEffects.None, 0.0f);
        }
        #endregion
    }
}