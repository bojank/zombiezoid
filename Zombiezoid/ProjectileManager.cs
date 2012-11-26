using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zombiezoid
{
    public class ProjectileManager
    {

        private List<Projectile> projectiles;
        private Game game;
        public ProjectileManager(Game game)
        {
            this.game = game;
            projectiles = new List<Projectile>();
        }

        public void CreateProjectile(Vector2 position, float angle,Player player)
        {
            var projectile = new Projectile(game, player, position, angle);
            projectiles.Add(projectile);
        }

        public  void ClearAll()
        {
            projectiles.Clear();
        }

        public void Update(GameTime gameTime)
        {

            
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = projectiles[i];
                projectile.Update(gameTime);
                if (projectile.IsStopped)
                    projectiles.Remove(projectile);
                //projectile.Draw(gameTime, spriteBatch);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                Projectile projectile = projectiles[i];
                projectile.Draw(gameTime,spriteBatch);
            }

        }

    }
}