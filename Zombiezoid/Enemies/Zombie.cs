using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zombiezoid
{
    public class Zombie : Enemy
    {
        protected Random random;
        //Vector2 aiNewDir;
        //int aiNumSeen;

        private bool isAttacking;
        private float attackWait;
        
        public Animation runAnimation;
        public Animation attackAnimation;
        public Animation dieAnimation;

        //private float waitTime;

        //private const float MaxWaitTime = 0.5f;

        public Zombie(Game game, Random rnd) 
            : base(game ,rnd )
        {
            enemytype = EnemyType.Zombie;
            random = rnd;
            isAttacking = false;
            BuildBehaviors();
            LoadContent("Zombie");
        }

        public void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            idleAnimation = new Animation(game.Content.Load<Texture2D>(spriteSet + "ZombieIdle"), 0.15f, true);
            runAnimation = new Animation(game.Content.Load<Texture2D>(spriteSet + "ZombieIdle"), 0.1f, true);
            attackAnimation = new Animation(game.Content.Load<Texture2D>(spriteSet + "ZombieAttack"), 0.3f, false);
            dieAnimation = new Animation(game.Content.Load<Texture2D>(spriteSet + "ZombieIdle"), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);
            //whiteBox = new Texture2D(graphicsDevice,1,1);

            //whiteRectangle.SetData(new[] { Color.White });




            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.8);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = (idleAnimation.FrameHeight - height) / 2;
            localBounds = new Rectangle(left, top, width, height);

            //firstTime = true;
            //direction = (float)Math.Atan2(game.player.Position.Y - Position.Y, game.player.Position.X - Position.X);
            //velocity = new Vector2((float)Math.Cos((double)direction), (float)Math.Sin((double)direction));
        }

        public new void BuildBehaviors()
        {
            Behaviors enemyReaction = new Behaviors();
            //enemyReaction.Add(new FleeBehavior(this));
            enemyReaction.Add(new AlignBehavior(this));
            //enemyReaction.Add(new CohesionBehavior(this));
            enemyReaction.Add(new SeparationBehavior(this));
            behaviors.Add(EnemyType.Zombie, enemyReaction);

            Behaviors tileReaction = new Behaviors();
            //enemyReaction.Add(new FleeBehavior(this));
            //enemyReaction.Add(new AlignBehavior(this));
            //enemyReaction.Add(new CohesionBehavior(this));
            tileReaction.Add(new SeparationBehavior(this));
            behaviors.Add(EnemyType.Generic, tileReaction);
        }

        public override void Update(GameTime gameTime, ref AIParameters aiParams)
        {

            //Vector2 previousPosition = position;
            //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //if(velocity != Vector2.Zero)
            //    velocity.Normalize();
            //velocity = velocity * MoveSpeed * elapsed;
            
            velocity = new Vector2((float)Math.Cos((double)direction), (float)Math.Sin((double)direction));
            //velocity.Normalize();
            ////velocity.
            //position = position + velocity;

            //this.HandleCollisions();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 randomDir = Vector2.Zero;

            randomDir.X = (float)random.NextDouble() - 0.5f;
            randomDir.Y = (float)random.NextDouble() - 0.5f;
            Vector2.Normalize(ref randomDir, out randomDir);

            if (aiNumSeen > 0)
            {
                aiNewDir = (velocity * aiParams.MoveInOldDirectionInfluence) +
                    (aiNewDir * (aiParams.MoveInFlockDirectionInfluence /
                    (float)aiNumSeen));
            }
            else
            {
                aiNewDir = velocity * aiParams.MoveInOldDirectionInfluence;
             }

            aiNewDir += (randomDir * aiParams.MoveInRandomDirectionInfluence);
            Vector2.Normalize(ref aiNewDir, out aiNewDir);
            aiNewDir = ChangeDirection(velocity, aiNewDir,
                aiParams.MaxTurnRadians * elapsedTime);
            velocity = aiNewDir;
            //velocity.Normalize();
            //velocity *= MoveSpeed * elapsedTime;
            direction = (float)Math.Atan2(aiNewDir.Y,aiNewDir.X);

            position = position + velocity;

           // AI(gameTime);

            this.HandleCollisions();
            //if (new Vector2(direction).LengthSquared() > .01f)
            //{
            //    Vector2 moveAmount = new Vector2(direction) * MoveSpeed * elapsedTime;
            //    location = location + moveAmount;

            //}

        }

        

        

        public bool canSee(Player player)
        {
            //Rectangle view = new Rectangle()
            //if()
            return true;
        }

        
        //AI
        public void Attack(GameTime gameTime)
        {
            //if (isAttacking)
            //{
                if (attackWait == 0.0f)
                {
                    sprite.PlayAnimation(attackAnimation);
                    //soundZombieAttack.Play();
                    attackWait = 0.3f;
                    //map.ProjectileManager.CreateProjectile(position, angle, this);
                    game.player.DoDamage(50);
                    isAttacking = true;
                }
                else
                {
                    attackWait -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (attackWait <= 0.0f)
                {
                    sprite.PlayAnimation(idleAnimation);
                    isAttacking = false;
                    attackWait = 0.0f;
                }
            //}


        }

        private bool canSeePlayer(){
            //if()
            //Rectangle bounds = new Rectangle(this.Position.X,this.position.Y,);
                //bounds.Intersects();
            return true;
        }



        private void AI(GameTime gameTime)
        {
            float playerDistance = Vector2.Distance(this.position, game.player.Position);
            //if (canSeePlayer) {
            if (playerDistance > 20 && playerDistance < 150 )//&& canSeePlayer())
                Chase();
                
            if (playerDistance < 40)
                Attack(gameTime);
            else
                return;


            return;
        }

        public void Chase()
        {
            direction = (float)Math.Atan2(game.player.Position.Y - position.Y, game.player.Position.X - position.X);
            velocity = new Vector2((float)Math.Cos((double)direction), (float)Math.Sin((double)direction));
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Stop running when the game is paused or before turning around.
            //sprite.PlayAnimation(runAnimation);



            // Draw facing the way the enemy is moving.
            sprite.Draw(gameTime, spriteBatch, position, direction);
            //riteBatch.Draw(dummyTexture, BoundingRectangle, Color.DeepPink);
            //spriteBatch.
        }

        public bool firstTime { get; set; }
    }
}
