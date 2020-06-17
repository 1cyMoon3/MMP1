//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Glid_n_Shadow
{
    class Map
    {
        //Structure of the levelmap
        private Tile[,] tiles;
        private Texture2D[] layers;

        //Important map locations
        private Vector2 start;
        private static readonly Point InvalidPoint = new Point(-1, -1);
        private Point exit = InvalidPoint;
        private bool reachedExit;
        public bool ReachedExit
        {
            get => reachedExit;
        }
        //private SoundEffect startLevel;     //No fitting sound found
        //private SoundEffect exitReached;    //No fitting sound found

        //Player and the layer they get drawn on
        private Player player;
        public Player Player
        {
            get => player;
        }

        public Song Song;

        //Font for playerlife
        private SpriteFont levelSelectFont; //MontserratMedium40

        //Content
        private ContentManager content;
        public ContentManager Content
        {
            get => content;
        }

        //Map Width and Height
        public int Width
        {
            get => tiles.GetLength(0);
        }
        public int Height
        {
            get => tiles.GetLength(1);
        }

        //Map Scrolling
        //Vector2 offset;
        Random r = new Random();

        //Map Constructor
        public Map(ContentManager content, Stream fileStream)
        {
            this.content = content;
            LoadTiles(fileStream);

            int mapAmount = Directory.GetDirectories("Content/Maps").Length;
            int layerFolder = r.Next(1, mapAmount);
            int layerAmount = Directory.GetFiles($"Content/Maps/Map{layerFolder}").Length - 2;
            layers = new Texture2D[layerAmount];

            for(int i = 0; i < layerAmount; i++)
            {
                layers[i] = Content.Load<Texture2D>($"Maps/Map{layerFolder}/Layer_{i}");
            }

            Song = Content.Load<Song>($"Maps/Map{layerFolder}/BackgroundMusic");

            levelSelectFont = Content.Load<SpriteFont>("Fonts/MontserratMedium40");
        }
        private void LoadTiles(Stream fileStream)
        {
            int width;
            List<string> lines = new List<string>();
            //Read the file content into lines-List
            using(StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while(line != null)
                {
                    lines.Add(line);
                    if (line.Length != width) throw new Exception("The length is different to all lines before it.");
                    line = reader.ReadLine();
                }
            }            

            //Set the amount of tiles to the width and the amount of lines
            tiles = new Tile[width, lines.Count];
            //Go over every tile
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    //Set tile for position
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            if(Player == null)
            {
                throw new Exception("Map needs a starting point!");
            }
            if(exit == InvalidPoint)
            {
                throw new Exception("Map needs an exit!");
            }
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                //Air
                case '.':
                    return new Tile(null, TileCollision.Air);
                //Floating Platform
                case '~':
                    return LoadSimpleTile("Platform", TileCollision.Platform);
                //Impassable Block
                case '-':
                    return LoadSimpleTile("Ground", TileCollision.Impassable);
                case '#':
                    return LoadSimpleTile("Block", TileCollision.Impassable);
                //Starting Point
                case 'S':
                    return LoadStartTile(x,y);
                //Exit Point
                case 'E':
                    return LoadExitTile(x,y);
                case 'D':
                    return LoadSimpleTile("Death1", TileCollision.Death);
                //Not defined tile type
                default:
                    throw new NotSupportedException($"Unsupported tile type {tileType} at {x},{y}");
            }
        }

        //For loading simple Tiles (Platforms, Blocks)
        private Tile LoadSimpleTile(string textureName, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/"+textureName), collision);
        }

        //For loading the starting Tile and initialising player
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null) throw new NotSupportedException("A map can only have one starting point.");

            start = RectangleExtension.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);
            return new Tile(null, TileCollision.Air);
        }

        //For loading the exit Tile
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPoint) throw new NotSupportedException("A map can only have one exit");
            exit = GetBounds(x, y).Center;

            return LoadSimpleTile("Exit", TileCollision.Air);
        }

        //Unload level
        public void Dispose()
        {
            Content.Unload();
        }

        //Get collision type of tile at given coordinates
        public TileCollision GetCollision(int x, int y)
        {
            //Not getting to run out the sides and drop below bottom
            if (x < 0 || x >= Width || y >= Height) return TileCollision.Impassable;
            //Being able to jump above top
            if (y < 0) return TileCollision.Air;
            //Else return collision of the tile at the coordinates
            return tiles[x, y].Collision;
        }

        //Get bounding rectangle of tile in world space
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            //Pause while player is dead or in menu
            if (!Player.IsAlive)
            {
                Player.UpdatePhysics(gameTime);
            }
            //Update Player and check if exit was reached
            else
            {
                Player.Update(gameTime, keyboardState);
                //offset = new Vector2(-Player.Position.X, 0);
                if (Player.IsAlive && Player.IsOnGround && Player.BoundingRectangle.Contains(exit))
                {
                    reachedExit = true;
                }
            }
        }

        public void StartNewLife()
        {
            Player.Reset(start);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw map background
            for(int i = 0; i < layers.Length; i++)
            {
                spriteBatch.Draw(layers[i], new Rectangle(0, 0, Width * Tile.Width, Height * Tile.Height + 40), Color.White);
            }

            //Draw tiles
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    Texture2D texture = tiles[x, y].Texture;
                    if(texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.TileSize;
                        spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, Tile.Width, Tile.Height), Color.White);
                        //spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }

            //Draw player
            Player.Draw(gameTime, spriteBatch);
            DrawLife(spriteBatch);
        }

        private void DrawLife(SpriteBatch spriteBatch)
        {
            double life = Math.Round(Player.life);
            string lifeString = "Life: " + life;
            Color textColor = new Color(217, 217, 217);
            spriteBatch.DrawString(levelSelectFont, lifeString, new Vector2(5, 5), textColor);
        }
    }
}
