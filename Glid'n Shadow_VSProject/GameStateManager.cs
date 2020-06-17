//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Glid_n_Shadow
{
    class GameStateManager : IDisposable
    {
        private enum GameState
        {
            LandingScreen,
            LevelSelection,
            Gameplay
        }
        public bool ExitGame;

        private ContentManager content;
        private ContentManager mapContent;
        private SpriteBatch spriteBatch;
        private MenuScreens menuScreens;
        private GameState state;
        private GameState lastState;

        //Fonts
        private SpriteFont titleFont;       //Aclonica160
        private SpriteFont smallTitleFont;  //Aclonica80
        private SpriteFont buttonFont;      //MontserratMedium60
        private SpriteFont levelSelectFont; //MontserratMedium40
        private SpriteFont exitButtonFont;  //MontserratMedium30
        private SpriteFont smallFont;       //MontserratRegular30

        //Screen
        Texture2D menuBackground;
        Vector2 screenSize;
        Song menuSong;

        //Input
        KeyboardState keyboardState;

        //Map
        private int lvlNr;
        private Map map;
        private bool isLvlLoaded;

        public GameStateManager(ContentManager content, SpriteBatch spriteBatch, Vector2 screenSize, ContentManager mapContent)
        {
            this.content = content;
            this.spriteBatch = spriteBatch;
            this.screenSize = screenSize;
            this.mapContent = mapContent;


            //Load fonts & textures & song
            titleFont = content.Load<SpriteFont>("Fonts/Aclonica160"); 
            smallTitleFont = content.Load<SpriteFont>("Fonts/Aclonica80");
            buttonFont = content.Load<SpriteFont>("Fonts/MontserratMedium60");
            levelSelectFont = content.Load<SpriteFont>("Fonts/MontserratMedium40");
            exitButtonFont = content.Load<SpriteFont>("Fonts/MontserratMedium30");
            smallFont = content.Load<SpriteFont>("Fonts/MontserratRegular30");
            Texture2D buttonTexture = content.Load<Texture2D>("Button");
            Texture2D arrowTexture = content.Load<Texture2D>("Arrow");
            menuBackground = content.Load<Texture2D>("menuBackground");
            menuSong = content.Load<Song>("MenuMusic");

            menuScreens = new MenuScreens(screenSize, titleFont, smallTitleFont, buttonFont, levelSelectFont, exitButtonFont, smallFont, buttonTexture, arrowTexture);

            state = GameState.LandingScreen;
            ExitGame = false;
            isLvlLoaded = true;
        }

        public void UpdateGameState(GameTime time)
        {
            //Get Inputs
            keyboardState = Keyboard.GetState();
            KeyboardInput.GetInput();

            //Check which game state
            if (menuScreens.showMainMenu) state = GameState.LandingScreen;
            if (menuScreens.showLevelSelectMenu) state = GameState.LevelSelection;
            if (menuScreens.showMap) state = GameState.Gameplay;

            //Update according to game state
            switch (state)
            {
                case GameState.LandingScreen:
                    UpdateLandingScreen();
                break;
                case GameState.LevelSelection:
                    UpdateLevelSelection();
                break;
                case GameState.Gameplay:
                    UpdateMap(time);
                break;
            }
            lvlNr = menuScreens.mapIndex;

            //Update last state (for background music)
            lastState = state;
        }

        public void DrawGameState(GameTime time)
        {
            //Draw according to game state
            switch (state)
            {
                case GameState.LandingScreen:
                    DrawLandingScreen();
                    break;
                case GameState.LevelSelection:
                    DrawLevelSelection();
                    break;
                case GameState.Gameplay:
                    DrawMap(time);
                    break;
            }
        }

        private void UpdateLandingScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) ExitGame = true;
            //Start new song if game state (/the needed backgroundmusic) changed 
            if (state != lastState && lastState != GameState.LevelSelection)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(menuSong);
            }
            menuScreens.UpdateMainMenu();
        }

        private void UpdateLevelSelection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) ExitGame = true;
            //Start new song if game state (/the needed backgroundmusic) changed 
            if (state != lastState && lastState != GameState.LandingScreen)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(menuSong);
            }
            menuScreens.UpdateLevelSelect();
        }

        private void UpdateMap(GameTime time)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) ExitGame = true;

            //Checking if new map gets opened
            if (isLvlLoaded)
            {
                LoadLevel(lvlNr);
                isLvlLoaded = false;
            }

            map.Update(time, keyboardState);

            //Start new song if game state changed 
            if (state != lastState)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(map.Song);
            }

            //Leaving Level-GameState and going back to Level Selection
            if (map.ReachedExit || KeyboardInput.HasBeenPressed(Keys.Back) || !map.Player.IsAlive)
            {
                menuScreens.showMap = false;
                menuScreens.showLevelSelectMenu = true;
                isLvlLoaded = true;
            }
        }

        private void DrawLandingScreen()
        {
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
            menuScreens.DrawMainMenu(spriteBatch);
        }

        private void DrawLevelSelection()
        {
            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
            menuScreens.DrawLevelSelection(spriteBatch);
        }

        private void DrawMap(GameTime time)
        {
            map.Draw(time, spriteBatch);
        }

        private void LoadLevel(int levelnr)
        {
            if (map != null)
            {
                map.Dispose();
            }

            string path = $"Content/MapContent/{levelnr}.txt";
            using (Stream stream = TitleContainer.OpenStream(path))
            {
                map = new Map(mapContent, stream);
                Console.WriteLine(path);
            }
        }

        public void Dispose()
        {
            content.Unload();
        }
    }
}
