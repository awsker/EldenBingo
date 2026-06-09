namespace EldenBingo.Util
{
    internal record struct RectDir(Rectangle Rect, int Dir);

    internal static class WindowHelper
    {
        public static Tuple<Point, Point> GetLocationAndSizeOffsets(Point location, Size size, bool changeSize, int margin)
        {
            // Clip new position and size with all screens and ensure that nothing clips outside
            var rect = new Rectangle(new Point(location.X - margin, location.Y - margin), new Size(size.Width + margin * 2, size.Height + margin * 2));
            if (!RectIntersectsAnyScreen(rect))
            {
                var closestScreen = GetClosestScreenBounds(rect);
                if (closestScreen.IsEmpty)
                    return new(Point.Empty, Point.Empty);
                var moveX = 0;
                var moveY = 0;

                // Calculate the distance to move rect so it fits fully inside closestScreen
                if (rect.Left < closestScreen.Left)
                    moveX = closestScreen.Left - rect.Left;
                else if (rect.Right > closestScreen.Right)
                    moveX = closestScreen.Right - rect.Right;

                if (rect.Top < closestScreen.Top)
                    moveY = closestScreen.Top - rect.Top;
                else if (rect.Bottom > closestScreen.Bottom)
                    moveY = closestScreen.Bottom - rect.Bottom;

                return new(new Point(moveX, moveY), Point.Empty);
            }
            var remaining = new List<RectDir> { new RectDir(rect, 0) };
            for (int i = 0; i < remaining.Count;)
            {
                var r = remaining[i];
                bool removed = false;
                foreach (var scr in Screen.AllScreens)
                {
                    // Get all rectangles outside the current screen
                    var clippedRects = clipRectangle(r, scr.Bounds).ToList();
                    // If any rectangles outside the screen
                    if (!(clippedRects.Count == 1 && clippedRects[0].Rect == r.Rect))
                    {
                        remaining.RemoveAt(i);
                        removed = true;
                        remaining.AddRange(clippedRects);
                        break;
                    }
                }
                if (!removed) ++i;
            }

            int locationChangeX = 0, locationChangeY = 0, sizeChangeX = 0, sizeChangeY = 0;
            if (remaining.Count > 0)
            {
                var groups = remaining.GroupBy(r => r.Dir);
                foreach (var group in groups)
                {
                    var maxWidth = group.Max(r => r.Rect.Width);
                    var maxHeight = group.Max(r => r.Rect.Height);
                    switch (group.Key)
                    {
                        case 1:
                            if (changeSize)
                                sizeChangeX -= maxWidth;
                            locationChangeX += maxWidth;
                            break;
                        case 2:
                            if (changeSize)
                                sizeChangeX -= maxWidth;
                            else
                                locationChangeX -= maxWidth;
                            break;
                        case 3: 
                            if (changeSize)
                                sizeChangeY -= maxHeight;
                            locationChangeY += maxHeight;
                            break;
                        case 4:
                            if (changeSize)
                                sizeChangeY -= maxHeight;
                            else
                                locationChangeY -= maxHeight;
                            break;
                    }
                }
            }
            return new Tuple<Point, Point>(new Point(locationChangeX, locationChangeY), new Point(sizeChangeX, sizeChangeY));
        }

        public static bool RectIntersectsAnyScreen(Rectangle rect)
        {
            return Screen.AllScreens.Any(scr => rect.IntersectsWith(scr.Bounds));
        }

        public static Rectangle GetClosestScreenBounds(Rectangle rect)
        {
            var midPointFunc = (Rectangle rect) => new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            var rectMidPoint = midPointFunc(rect);
            var distSqr = (Point p1, Point p2) =>
            {
                var dx = (p2.X - p1.X);
                var dy = (p2.Y - p1.Y);
                return dx * dx + dy * dy;
            };
            if (Screen.AllScreens.Length == 0)
                return Rectangle.Empty;
            try
            {
                var minRect = Screen.AllScreens.ElementWithMinValue(scr => distSqr(rectMidPoint, midPointFunc(scr.Bounds)));
                if (minRect != null)
                {
                    return minRect.Bounds;
                }
            } catch { /* Ignore error getting closest screen */ }
            return Rectangle.Empty;
        }

        private static IEnumerable<RectDir> clipRectangle(RectDir rect, Rectangle clipRectangle)
        {
            // Returns rectangles representing areas of 'rect' that are outside 'clipRectangle'
            // Maximum of 4 rectangles (left, right, top, bottom)
            if (clipRectangle.Contains(rect.Rect))
            {
                yield break;
            }

            var r = rect.Rect;

            Rectangle intersection = Rectangle.Intersect(rect.Rect, clipRectangle);

            if (intersection.IsEmpty || intersection.Width == 0 || intersection.Height == 0)
            {
                // No overlap, entire rect is outside
                yield return rect;
                yield break;
            }

            // Left side
            if (intersection.Left > r.Left)
            {
                yield return new RectDir(
                    new Rectangle(
                    r.Left,
                    r.Top,
                    intersection.Left - r.Left,
                    r.Height)
                , 1);
            }

            // Right side
            if (intersection.Right < r.Right)
            {
                yield return new RectDir(
                    new Rectangle(
                    intersection.Right,
                    r.Top,
                    r.Right - intersection.Right,
                    r.Height)
                , 2);
            }

            // Top side
            if (intersection.Top > r.Top)
            {
                yield return new RectDir(
                    new Rectangle(
                    Math.Max(intersection.Left, r.Left),
                    r.Top,
                    Math.Min(intersection.Width, r.Width - (intersection.Left - r.Left)),
                    intersection.Top - r.Top)
                , 3);
            }

            // Bottom side
            if (intersection.Bottom < r.Bottom)
            {
                yield return new RectDir(
                    new Rectangle(
                    Math.Max(intersection.Left, r.Left),
                    intersection.Bottom,
                    Math.Min(intersection.Width, r.Width - (intersection.Left - r.Left)),
                    r.Bottom - intersection.Bottom)
                , 4);
            }
        }
    }
}
