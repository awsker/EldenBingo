namespace EldenBingo.UI
{
    internal static class UserControlExtension
    {
        public static SizeF DefaultScaleDimensions(this UserControl control)
        {
            return new SizeF(96f, 96f);
        }

        public static SizeF DefaultScaleFactors(this UserControl control)
        {
            var sc = control.DefaultScaleDimensions();
            return new SizeF(control.CurrentAutoScaleDimensions.Width / sc.Width, control.CurrentAutoScaleDimensions.Height / sc.Height);
        }
    }
}
