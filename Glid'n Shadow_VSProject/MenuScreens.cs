//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Glid_n_Shadow
{
    public class MenuScreens
    {
        //Screen size
        private Vector2 screenSize;

        //Main menu
        private const string titleText = "Glid'n Shadow";
        private const string continueText = "Press any key to continue";
        private const string impressumText1 = "Multimediatechnology";
        private const string impressumText2 = "MMP1";
        private const string impressumText3 = "Barbara Katharina Kroell";
        private Color titleColor = new Color(164, 80, 139);
        private Color normalTextColor = new Color(217, 217, 217);
        Vector2 titlePosition;
        Vector2 continuePosition;
        Vector2 impressum1Position;
        Vector2 impressum2Position;
        Vector2 impressum3Position;

        //Level selection
        Button levelSelect;
        string close = "X";
        Texture2D buttonTexture;
        Texture2D arrowTexture;
        Vector2 smallTitlePosition;
        Vector2 levelSelectPosition;
        Vector2 closePosition;
        public int mapAmount;
        public int mapIndex = 1;
        public bool IsClosed = false;

        //Pause Screen
        //Button continueButton;
        //Button levelSelectionButton;
        //Button exitButton;

        //Fonts
        private SpriteFont titleFont;       //Aclonica160
        private SpriteFont smallTitleFont;  //Aclonica80
        private SpriteFont buttonFont;      //MontserratMedium60
        private SpriteFont levelSelectFont; //MontserratMedium40
        private SpriteFont exitButtonFont;  //MontserratMedium30
        private SpriteFont smallFont;       //MontserratRegular30

        public bool showMainMenu = true;
        public bool showLevelSelectMenu = false;
        public bool showMap = false;

        //Inputs
        private KeyboardState keyboardState;
        //private MouseState mouseState;        //didn't work out

        public MenuScreens(Vector2 screenSize, 
            SpriteFont titleFont, SpriteFont smallTitleFont, SpriteFont buttonFont, SpriteFont levelSelectFont, SpriteFont exitButtonFont,
            SpriteFont smallFont, Texture2D buttonTexture, Texture2D arrowTexture)
        {
            this.screenSize = screenSize;
            //Load fonts
            this.titleFont = titleFont; ;
            this.smallTitleFont = smallTitleFont;
            this.buttonFont = buttonFont;
            this.levelSelectFont = levelSelectFont;
            this.exitButtonFont = exitButtonFont;
            this.smallFont = smallFont;
            this.buttonTexture = buttonTexture;
            this.arrowTexture = arrowTexture;

            this.mapAmount = Directory.GetFiles("Content/MapContent").Length;

            CalculateMainScreen();
            CalculateLevelSelectScreen(buttonTexture, arrowTexture);
        }

        private void CalculateMainScreen()
        {
            float titleTextHeight = titleFont.MeasureString(titleText).Y;
            float titleTextWidth = titleFont.MeasureString(titleText).X;

            float continueTextHeight = buttonFont.MeasureString(continueText).Y;
            float continueTextWidth = buttonFont.MeasureString(continueText).X;

            float impressum1Height = smallFont.MeasureString(impressumText1).Y;
            float impressum1Width = smallFont.MeasureString(impressumText1).X;

            float impressum2Height = smallFont.MeasureString(impressumText2).Y;
            float impressum2Width = smallFont.MeasureString(impressumText2).X;

            float impressum3Height = smallFont.MeasureString(impressumText3).Y;
            float impressum3Width = smallFont.MeasureString(impressumText3).X;

            titlePosition = new Vector2((screenSize.X / 2) - (titleTextWidth / 2), Vector2.Zero.Y + 30);
            continuePosition = new Vector2((screenSize.X / 2) - (continueTextWidth / 2), screenSize.Y - continueTextHeight - 30);
            impressum1Position = new Vector2(screenSize.X - 10 - impressum1Width, screenSize.Y - impressum1Height - impressum2Height - impressum3Height - 6);
            impressum2Position = new Vector2(screenSize.X - 10 - impressum2Width, impressum1Position.Y + 2 + impressum1Height);
            impressum3Position = new Vector2(screenSize.X - 10 - impressum3Width, impressum2Position.Y + 2 + impressum2Height);
        }

        private void CalculateLevelSelectScreen(Texture2D buttonTexture, Texture2D arrowTexture)
        {
            Vector2 screenCenter = new Vector2(screenSize.X / 2, screenSize.Y / 2);
            levelSelectPosition = new Vector2(screenCenter.X - Button.Width / 2, screenCenter.Y - Button.Height / 2);
            levelSelect = new Button(buttonTexture, arrowTexture, normalTextColor, levelSelectPosition);

            float smallTitleWidth = smallTitleFont.MeasureString(titleText).X;
            smallTitlePosition = new Vector2(screenCenter.X - smallTitleWidth / 2, 10);
            float closeWidth = buttonFont.MeasureString(close).X;
            closePosition = new Vector2(screenSize.X - 20 - closeWidth, 10);
        }

        public void UpdateMainMenu()
        {
            keyboardState = Keyboard.GetState();
            if(keyboardState.GetPressedKeys().Length > 0 && !keyboardState.IsKeyDown(Keys.Escape) && !keyboardState.IsKeyDown(Keys.Back))
            {
                showMainMenu = false;
                showLevelSelectMenu = true;
            }
        }

        public void UpdateLevelSelect()
        {
            if (KeyboardInput.HasBeenPressed(Keys.Left))
            {
                if(mapIndex > 1)
                {
                    mapIndex--;
                }
                else if(mapIndex == 1)
                {
                    mapIndex = mapAmount;
                }
            }
            if (KeyboardInput.HasBeenPressed(Keys.Right))
            {
                if(mapIndex <= mapAmount)
                {
                    mapIndex++;
                }
                if(mapIndex == mapAmount + 1)
                {
                    mapIndex = 1;
                }
            }
            if (KeyboardInput.HasBeenPressed(Keys.Enter))
            {
                showLevelSelectMenu = false;
                showMap = true;
            }
            if (KeyboardInput.HasBeenPressed(Keys.Back))
            {
                showLevelSelectMenu = false;
                showMainMenu = true;
            }
        }

        public Rectangle CloseRectangle()
        {
            int closeWidth = (int)buttonFont.MeasureString(close).X * 2;
            int closeHeight = (int)buttonFont.MeasureString(close).Y * 2;
            Rectangle closeRectangle = new Rectangle((int)screenSize.X - closeWidth, 0, closeWidth, closeHeight);
            return closeRectangle;
        }

        public Rectangle LevelRectangle(Vector2 position)
        {
            Rectangle levelRectangle = new Rectangle((int)position.X - Button.Width / 2, (int)(position.Y + Button.Height * 3), Button.Width, (int)(Button.Height * 1.3));
            return levelRectangle;
        }

        public void DrawMainMenu(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(titleFont, titleText, titlePosition, titleColor);
            spriteBatch.DrawString(buttonFont, continueText, continuePosition, normalTextColor);
            spriteBatch.DrawString(smallFont, impressumText1, impressum1Position, normalTextColor);
            spriteBatch.DrawString(smallFont, impressumText2, impressum2Position, normalTextColor);
            spriteBatch.DrawString(smallFont, impressumText3, impressum3Position, normalTextColor);
        }


        public void DrawLevelSelection(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(smallTitleFont, titleText, smallTitlePosition, titleColor);
            levelSelect.DrawArrowButton(spriteBatch, buttonFont, $"Level: {mapIndex}");
            DrawControls(spriteBatch);
        }

        private void DrawControls(SpriteBatch spriteBatch)
        {
            string left = "Run Left: A / Left";
            string right = "Run Right: D / Right";
            string jump = "Jump/Glide: Space / Up";
            string leave = "Leave Lvl: Back";
            string exit = "Exit Game: Esc";
            string enter = "Enter selected Level: Enter";
            int margin = 20;
            Color textColor = new Color(217, 217, 217);

            Vector2 centerLeft = new Vector2(0, screenSize.Y / 2);
            Vector2 centerRight = new Vector2(screenSize.X, screenSize.Y / 2);
            Vector2 center = new Vector2(screenSize.X / 2, screenSize.Y / 2);
            float textHeight = levelSelectFont.MeasureString(left).Y;
            float leaveWidth = levelSelectFont.MeasureString(leave).X;
            float exitWidth = levelSelectFont.MeasureString(exit).X;
            float enterWidth = levelSelectFont.MeasureString(enter).X;
            float enterHeight = levelSelectFont.MeasureString(enter).Y;

            //Linke Seite
            spriteBatch.DrawString(levelSelectFont, left, new Vector2(centerLeft.X + margin, centerLeft.Y - textHeight * 2), textColor);
            spriteBatch.DrawString(levelSelectFont, right, new Vector2(centerLeft.X + margin, centerLeft.Y + textHeight * 2), textColor);
            spriteBatch.DrawString(levelSelectFont, jump, new Vector2(centerLeft.X + margin, centerLeft.Y), textColor);

            //Rechte Seite
            spriteBatch.DrawString(levelSelectFont, leave, new Vector2(centerRight.X - leaveWidth - margin, centerRight.Y - textHeight), textColor);
            spriteBatch.DrawString(levelSelectFont, exit, new Vector2(centerRight.X - exitWidth - margin, centerRight.Y + textHeight), textColor);

            //Center
            spriteBatch.DrawString(levelSelectFont, enter, new Vector2(center.X - enterWidth / 2, center.Y + margin + enterHeight), textColor);
        }

        //public void DrawPauseScreen(SpriteBatch spriteBatch, Vector2 screenSize)
        //{

        //}
    }
}
