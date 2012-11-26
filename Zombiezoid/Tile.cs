using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Zombiezoid
{

    public enum TileType
    {
        Ground = 0,
        Wall = 1,
        Water = 2,
        Snow = 3,
        Pain = 8,
        Death = 9
    }

    struct Tile
    {
        public Texture2D Texture;
        public TileType Type;

        public const int Width = 48;
        public const int Height = 48;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileType type)
        {
            Texture = texture;
            Type = type;
        }
    }
}