#region File Description
//-----------------------------------------------------------------------------
// SeparationBehavior.cs
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
    /// SeparationBehavior is a Behavior that will make an Enemy move away from
    /// another if it's too close for comfort
    /// </summary>
    class SeparationBehavior : Behavior
    {
        #region Initialization
        public SeparationBehavior(Enemy enemy)
            : base(enemy)
        {
        }
        #endregion

        #region Update

        /// <summary>
        /// separationBehavior.Update infuences the owning Enemy to move away from
        /// the otherAnimal is it’s too close, in this case if it’s inside 
        /// AIParameters.separationDistance.
        /// </summary>
        /// <param name="otherAnimal">the Enemy to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public override void Update(Enemy otherAnimal, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pushDirection = Vector2.Zero;
            float weight = aiParams.PerMemberWeight;

            if (Enemy.ReactionDistance > 0.0f && 
                Enemy.ReactionDistance <= aiParams.SeparationDistance)
            {
                //The otherAnimal is too close so we figure out a pushDirection 
                //vector in the opposite direction of the otherAnimal and then weight
                //that reaction based on how close it is vs. our separationDistance

                pushDirection = Enemy.position - Enemy.ReactionLocation;
                Vector2.Normalize(ref pushDirection, out pushDirection);

                //push away
                weight *= (1 - 
                    (float)Enemy.ReactionDistance / aiParams.SeparationDistance);

                pushDirection *= weight;

                reacted = true;
                reaction += pushDirection;
            }
        }

        public void Update(Rectangle rectangle, AIParameters aiParams)
        {
            base.ResetReaction();

            Vector2 pushDirection = Vector2.Zero;
            float weight = aiParams.PerDangerWeight;

            if (Enemy.ReactionDistance > 0.0f &&
                Enemy.ReactionDistance <= aiParams.SeparationDistance)
            {
                //The otherAnimal is too close so we figure out a pushDirection 
                //vector in the opposite direction of the otherAnimal and then weight
                //that reaction based on how close it is vs. our separationDistance

                pushDirection = Enemy.position - Enemy.ReactionLocation;
                Vector2.Normalize(ref pushDirection, out pushDirection);

                //push away
                weight *= (1 -
                    (float)Enemy.ReactionDistance / aiParams.SeparationDistance);

                pushDirection *= weight;

                reacted = true;
                reaction += pushDirection;
            }
        }
        #endregion
    }
}
