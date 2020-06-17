//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glid_n_Shadow
{
    class Button
    {
        public const int Width = 390;
        public const int Height = 50;
        private Texture2D texture;
        public Texture2D Texture
        {
            get => texture;
            set => texture = value;
        }
        private Texture2D arrow;
        private Color textColor;
        public Vector2 buttonSize;
        private Vector2 buttonCenter;
        private Vector2 position;

        public Button(Texture2D texture, Texture2D arrow, Color textColor, Vector2 position)
        {
            this.texture = texture;
            this.textColor = textColor;
            this.position = position;
            this.arrow = arrow;
            buttonSize = new Vector2(Width, Height);
            buttonCenter = new Vector2(this.position.X + Width / 2, this.position.Y + Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, string text)
        {
            //draw button
            spriteBatch.Draw(texture, destinationRectangle: new Rectangle((int)position.X, (int)position.Y, Width, Height), Color.White);

            float textHeight = font.MeasureString(text).Y;
            float textWidth = font.MeasureString(text).X;

            Vector2 textPosition = new Vector2(buttonCenter.X - textWidth / 2, buttonCenter.Y - textHeight / 2);
            //draw text in button
            spriteBatch.DrawString(font, text, textPosition, textColor);
        }

        public void DrawArrowButton(SpriteBatch spriteBatch, SpriteFont font, string text)
        {
            Draw(spriteBatch, font, text);
            int arrowHeight = 21;
            int arrowWidth = 14;
            int arrowYPosition = (int)position.Y + Height / 2 - arrowHeight / 2;
            SpriteEffects left = SpriteEffects.FlipHorizontally;
            //Right Arrow
            spriteBatch.Draw(arrow, destinationRectangle: new Rectangle((int)position.X + Width + 30, arrowYPosition, arrowWidth, arrowHeight), Color.White);
            //Left Arrow
            spriteBatch.Draw(arrow, destinationRectangle: new Rectangle((int)position.X - 30 - arrowWidth, arrowYPosition, arrowWidth, arrowHeight), effects: left);
        }
    }
}
