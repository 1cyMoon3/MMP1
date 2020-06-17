using Microsoft.Xna.Framework.Input;

namespace Glid_n_Shadow
{
    class KeyboardInput
    {
        static KeyboardState currentKeyboardState;
        static KeyboardState previousKeyboardState;

        public static KeyboardState GetInput()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            return currentKeyboardState;
        }

        public static bool IsPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public static bool HasBeenPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
        }
    }
}
