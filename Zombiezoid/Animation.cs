using System;
using Microsoft.Xna.Framework.Graphics;

namespace Zombiezoid
{
    public class Animation
    {
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        public int FrameWidth
        {
            get { return Texture.Height; }
        }

        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        public Animation(Texture2D texture, float animationTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = animationTime / FrameCount;
            this.isLooping = isLooping;
        }
    }
}
