#region File Description
//-----------------------------------------------------------------------------
// Flock.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Zombiezoid
{
    /// <summary>
    /// This class manages all the birds in the flock and handles 
    /// their update and draw
    /// </summary>
    public class Hoard
    {
        #region Constants
        //Number of FLock members
        const int hoardSize = 100;
        #endregion

        #region Fields

        //birds that fly out of the boundry(screen) will wrap around to 
        //the other side
        int boundryWidth;
        int boundryHeight;

        /// <summary>
        /// Tecture used to draw the Flock
        /// </summary>
        Texture2D birdTexture;

        /// <summary>
        /// List of Flock Members
        /// </summary>
        public List<Enemy> hoard;

        private Map map;
        /// <summary>
        /// Parameters flock members use to move and think
        /// </summary>
        public AIParameters HoardParams
        {
            get
            {
                return HoardParams;
            }

            set
            {
                hoardParams = value;
            }           
        }
        protected AIParameters hoardParams;
        

        #endregion

        #region Initialization

        /// <summary>
        /// Setup the flock boundaries and generate individual members of the flock
        /// </summary>
        /// <param name="tex"> The texture to be used by the birds</param>
        /// <param name="screenWidth">Width of the screen</param>
        /// <param name="screenHeight">Height of the screen</param>
        /// <param name="hoardParameters">Behavior of the flock</param>
        public Hoard( Game game, Random rnd,
            AIParameters hoardParameters)
        {
            boundryWidth = game.map.Width;
            boundryHeight = game.map.Height;

            map = game.map;
            //birdTexture = tex;

            hoard = new List<Enemy>();
            hoardParams = hoardParameters;

            ResetHoard(game, rnd);
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update each flock member, Each bird want to fly with or flee from everything
        /// it sees depending on what type it is
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="cat"></param>
        public void Update(GameTime gameTime)
        {
           
            foreach (var enemy in hoard)
            {
                enemy.ResetThink();

                foreach (var otherEnemy in hoard)
                {
                    //this check is so we don't try to fly to ourself!
                    if (enemy != otherEnemy)
                    {
                        enemy.ReactTo(otherEnemy, ref hoardParams);
                    }
                }

                //Map
                Rectangle bounds = enemy.BoundingRectangle;
                int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width)-1;
                int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) + 1;
                int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height)-1;
                int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) + 1;

                // Reset flag to search for ground collision.
                //isOnGround = false;

                // For each potentially colliding tile,
                for (int y = topTile; y <= bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        // If this tile is collidable,
                        TileType collision = map.GetCollision(x, y);
                        if (collision != TileType.Ground)
                        {
                            // Determine collision depth (with direction) and magnitude.
                            enemy.ReactTo(map.GetBounds(x, y), ref hoardParams);
                        }
                    }
                }
                //Look for the cat
                //thisBird.ReactTo(cat, ref flockParams);

                enemy.Update(gameTime, ref hoardParams);
            }
        }

        /// <summary>
        /// Calls Draw on every member of the Flock
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var enemy in hoard)
            {
                enemy.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the current flock if it exists and randomly generate a new one
        /// </summary>
        public void ResetHoard(Game game, Random rnd)
        {
            hoard.Clear();
            hoard.Capacity = hoardSize;

            Zombie tempBird;
//            Vector2 tempDir;
//            Vector2 tempLoc;

            Random random = new Random();

            for (int i = 0; i < hoardSize; i++)
            {
//                tempLoc = new Vector2((float)
//                    random.Next(boundryWidth), (float)random.Next(boundryHeight));
//                tempDir = new Vector2((float)
//                    random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
//                tempDir.Normalize();

                tempBird = new Zombie( game, rnd);
                hoard.Add(tempBird);
            }
        }
        #endregion
    }
}
