//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glid_n_Shadow
{        
    //Differing Collision detection and response
    enum TileCollision
    {
        Air = 0,
        Platform = 1,
        Impassable = 2,
        Death = 3
    }

    struct Tile
    {
        //Tile Appearance and Type
        public Texture2D Texture;
        public TileCollision Collision;

        //Fixed tile size
        public const int Width = 48;
        public const int Height = 16;
        public static readonly Vector2 TileSize = new Vector2(Width, Height);

        //Tile Constructor
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
