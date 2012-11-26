#region File Description
//-----------------------------------------------------------------------------
// FleeBehavior.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace Zombiezoid
{
    /// <summary>
    /// FleeBehavior is a Behavior that makes an Enemy run from another
    /// </summary>
    public class FleeBehavior : Behavior
    {
        #region Initialization
        public FleeBehavior(Enemy enemy)
            : base(enemy)
        {
        }
        #endregion

        #region Update
        public override void Update(Enemy otherAnimal, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 dangerDirection = Vector2.Zero;

            //Vector2.Dot will return a negative result in this case if the 
            //otherAnimal is behind the Enemy, in that case we don’t have to 
            //worry about it beceause we’re already moving away from it.
            if (Vector2.Dot(
                Enemy.position, Enemy.ReactionLocation) >= -(Math.PI / 2))
            {
                //set the Enemy to fleeing so that it flashes red
                Enemy.Fleeing = true;
                reacted = true;

                dangerDirection = Enemy.position - Enemy.ReactionLocation;
                Vector2.Normalize(ref dangerDirection, out dangerDirection);
                
                reaction = (aiParams.PerDangerWeight * dangerDirection);
            }
        }
        #endregion
    }
}
