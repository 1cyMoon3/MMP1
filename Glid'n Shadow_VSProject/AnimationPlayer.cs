//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Glid_n_Shadow
{
    struct AnimationPlayer
    {
        //Current animation
        private Animation animation;
        public Animation Animation
        {
            get => animation;
        }

        //Get the current frame index
        private int currentFrame;
        public int CurrentFrame
        {
            get => currentFrame;
        }

        //How long the frame is already being shown
        private float shownTime;

        //Origin for frame in texture (bottom center of frame)
        public Vector2 Origin
        {
            get => new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight);
        }

        //Start new animation or continue current animation
        public void RunAnimation(Animation animation)
        {
            //Doesn't restart animation if it's already running
            if (Animation == animation) return;

            //Run new animation
            this.animation = animation;
            this.currentFrame = 0;
            this.shownTime = 0.0f;
        }

        //Draws current frame of animation
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Animation == null) throw new NotSupportedException("No animation currently running");

            //Adds passed gametime to the time the current frame was shown
            shownTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

            //If the time the frame was shown is already longer then the allowed time per frame, advance index and reduce the showntime by the time allowed per frame
            while(shownTime > Animation.TimePerFrame)
            {
                shownTime -= Animation.TimePerFrame;
                //Advances frame index according to the isLooping from the running animation (looping or stopping at the end)
                if (Animation.IsLooping)
                {
                    currentFrame = (currentFrame + 1) % Animation.FrameCount;
                }
                else
                {
                    currentFrame = Math.Min(currentFrame + 1, Animation.FrameCount - 1);
                }

            }

            //Create the rectangle for current frame
            Rectangle frame = new Rectangle(0, currentFrame * Animation.FrameHeight, Animation.FrameWidth, Animation.FrameHeight);

            //Draw the current frame
            spriteBatch.Draw(Animation.Texture, position, frame, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
