#region File Description
//-----------------------------------------------------------------------------
// Behavior.cs
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
    #region FlockingAIParameters
    public struct AIParameters
    {
        /// <summary>
        /// how far away the animals see each other
        /// </summary>
        public float DetectionDistance;
        /// <summary>
        /// seperate from animals inside this distance
        /// </summary>
        public float SeparationDistance;
        /// <summary>
        /// how much the Enemy tends to move in it's previous direction
        /// </summary>
        public float MoveInOldDirectionInfluence;
        /// <summary>
        /// how much the Enemy tends to move with animals in it's detection distance
        /// </summary>
        public float MoveInFlockDirectionInfluence;
        /// <summary>
        /// how much the Enemy tends to move randomly
        /// </summary>
        public float MoveInRandomDirectionInfluence;
        /// <summary>
        /// how quickly the Enemy can turn
        /// </summary>
        public float MaxTurnRadians;
        /// <summary>
        /// how much each nearby Enemy influences it's behavior
        /// </summary>
        public float PerMemberWeight;
        /// <summary>
        /// how much dangerous animals influence it's behavior
        /// </summary>
        public float PerDangerWeight;
    }
    #endregion
    /// <summary>
    /// Behavior is the base class for the four flock behaviors in this sample: 
    /// aligning, cohesion, separation and fleeing. It is an abstract class, 
    /// leaving the implementation of Update up to its subclasses. Enemy objects 
    /// can have an arbitrary number of behaviors, after the entity calls Update 
    /// on the behavior the reaction results are stored in reaction so the owner 
    /// can query it.
    /// </summary>
    public abstract class Behavior
    {
        #region Fields
        /// <summary>
        /// Keep track of the Enemy that this behavior belongs to.
        /// </summary>
        public Enemy Enemy
        {
            get { return enemy; }
            set { enemy = value; }
        }
        private Enemy enemy;

        /// <summary>
        /// Store the behavior reaction here.
        /// </summary>
        public Vector2 Reaction
        {
            get { return reaction; }
        }
        protected Vector2 reaction;

        /// <summary>
        /// Store if the behavior has reaction results here.
        /// </summary>
        public bool Reacted
        {
            get { return reacted; }
        }
        protected bool reacted;
        #endregion

        #region Initialization
        protected Behavior(Enemy enemy)
        {
            this.enemy = enemy;
        }
        #endregion

        #region Update
        /// <summary>
        /// Abstract function that the subclass must impliment. Figure out the 
        /// Behavior reaction here.
        /// </summary>
        /// <param name="otherAnimal">the Enemy to react to</param>
        /// <param name="aiParams">the Behaviors' parameters</param>
        public abstract void Update(Enemy otherAnimal, AIParameters aiParams);
        //public abstract void Update(Rectangle rectangle, AIParameters aiParams);

        /// <summary>
        /// Reset the behavior reactions from the last Update
        /// </summary>
        protected void ResetReaction()
        {
            reacted = false;
            reaction = Vector2.Zero;
        }
        #endregion
    }
}
