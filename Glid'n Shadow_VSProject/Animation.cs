//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework.Graphics;

namespace Glid_n_Shadow
{
    //Assume a single animation in Texture2D (placed vertically)
    class Animation
    {
        //All frames
        private Texture2D texture;
        public Texture2D Texture
        {
            get => texture;
        }

        //Time each frame shows
        private float timePerFrame;
        public float TimePerFrame
        {
            get => timePerFrame;
        }

        //Is it a looping animation? (Run vs. Spawn)
        private bool isLooping;
        public bool IsLooping
        {
            get => isLooping;
        }

        //Frame dimensions
        public int FrameWidth
        {
            get => Texture.Width;
        }
        //Assume 3 frames per animation
        public int FrameHeight
        {
            get => 32;
        }

        //Amount of frames in texture
        public int FrameCount
        {
            get => Texture.Height / FrameHeight;
        }

        public Animation(Texture2D texture, float timePerFrame, bool isLooping)
        {
            this.texture = texture;
            this.timePerFrame = timePerFrame;
            this.isLooping = isLooping;
        }
    }
}
