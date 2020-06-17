//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Glid_n_Shadow
{
    class Player
    {


        //Soundeffects
        private SoundEffect jumpSound;
        private SoundEffect deathSound;

        //Character Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation fallAnimation;     //Whenever fall reaches damaging speed
        private Animation glideAnimation;
        private Animation dieAnimation;
        private AnimationPlayer sprite;
        private SpriteEffects flip = SpriteEffects.None;    //Flips the sprite in the direction it's running as needed

        //The map the player is generated in
        private Map map;
        public Map Map
        {
            get => map;
        }
        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        Vector2 speed;

        // Constants for controling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 3000.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -1500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 850.0f;
        private const float JumpControlPower = 0.14f;

        private bool isGliding = false;
        private float MaxGlideSpeed = 50.0f;
        public float life = 100.0f;
        private float damagePerSpeedOver = 0.5f;
        private float damageAtSpeed = MaxFallSpeed - 50;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;

        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        //Constructor for player
        public Player(Map map, Vector2 position)
        {
            this.map = map;
            life = 100.0f;
            LoadContent();
            Reset(position);
        }

        public void LoadContent()
        {
            //Sounds
            jumpSound = Map.Content.Load<SoundEffect>("Player/Sounds/jump");
            deathSound = Map.Content.Load<SoundEffect>("Player/Sounds/death");

            //Animations
            idleAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/idle"), 0.1f, true);        
            runAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/run"), 0.1f, true);          
            jumpAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/jump"), 0.1f, false);       
            fallAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/fall"), 0.1f, false);       
            glideAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/glide"), 0.1f, true);      
            dieAnimation = new Animation(Map.Content.Load<Texture2D>("Player/Sprite/die"), 0.1f, false);

            //Bounding box according to texture
            int width = (int)(idleAnimation.FrameWidth * 0.7);
            int height = (int)(idleAnimation.FrameHeight * 0.8);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }
         public void Reset(Vector2 position)
        {
            Position = position;
            Speed = Vector2.Zero;
            isAlive = true;
            sprite.RunAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            GetInput(keyboardState);
            UpdatePhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                if (Math.Abs(Speed.X) - 0.02f > 0)
                {
                    sprite.RunAnimation(runAnimation);
                }
                else
                {
                    sprite.RunAnimation(idleAnimation);
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
        }

        private void GetInput(KeyboardState keyboardState)
        {
            //Moving left
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            //Moving right
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
            }

            // Check for jump
            isJumping =
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);
            //Check for Glide
            isGliding = Speed.Y >= MaxFallSpeed / 6 && (keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up));
            if (isGliding) isJumping = false;
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            //Get elapsed time for change to speed
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            speed.X += movement * MoveAcceleration * elapsed;
            speed.Y = MathHelper.Clamp(speed.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            speed.Y = Jump(speed.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                speed.X *= GroundDragFactor;
            else
                speed.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            speed.X = MathHelper.Clamp(speed.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += speed * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                speed.X = 0;

            if (Position.Y == previousPosition.Y)
                speed.Y = 0;
        }

        private float Jump(float speedY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.RunAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    speedY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    jumpTime = 0.0f;
                }
            }
            else if (isGliding)
            {
                speedY = MaxGlideSpeed;
                sprite.RunAnimation(glideAnimation);
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return speedY;
        }

        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Map.GetCollision(x, y);
                    if (collision != TileCollision.Air)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Map.GetBounds(x, y);
                        Vector2 depth = RectangleExtension.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    isOnGround = true;
                                    if(speed.Y > damageAtSpeed && speed.Y >= 0)
                                    {
                                        life -= MathHelper.Clamp(speed.Y - damageAtSpeed, 0, MaxFallSpeed - damageAtSpeed) * damagePerSpeedOver;
                                        if(life <= 0)
                                        {
                                            life = 0;
                                            OnKilled();
                                        }
                                    }
                                }

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                                if (collision == TileCollision.Death) OnKilled();
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        public void OnKilled()
        {
            isAlive = false;

            deathSound.Play();

            sprite.RunAnimation(dieAnimation);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the sprite to face the way we are moving.
            if (Speed.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Speed.X > 0)
                flip = SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}
