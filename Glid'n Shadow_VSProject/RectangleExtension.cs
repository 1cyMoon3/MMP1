//Multimediatechnology
//MMP1
//Barbara Katharina Kröll

using Microsoft.Xna.Framework;
using System;

namespace Glid_n_Shadow
{
    public static class RectangleExtension
    {
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            //Get half of the rectangles widths and heights
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            //Calculate middles
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current distance between centers of both rectangles and minimum distance for no intersection.
            float currentDistanceX = centerA.X - centerB.X;
            float currentDistanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If there's no intersection, return 0-vector
            if (Math.Abs(currentDistanceX) >= minDistanceX || Math.Abs(currentDistanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate depth of intersection(s)
            float depthX = currentDistanceX > 0 ? minDistanceX - currentDistanceX : -minDistanceX - currentDistanceX;
            float depthY = currentDistanceY > 0 ? minDistanceY - currentDistanceY : -minDistanceY - currentDistanceY;
            return new Vector2(depthX, depthY);
        }
    }
}
