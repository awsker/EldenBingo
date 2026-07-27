namespace EldenBingo.UI
{
    internal static class UserControlExtension
    {
        public static SizeF DefaultScaleDimensions(this ContainerControl control)
        {
            return new SizeF(96f, 96f);
        }

        public static SizeF DefaultScaleFactors(this ContainerControl control)
        {
            return new SizeF(1f, 1f);
            //var sc = control.DefaultScaleDimensions();

            //return new SizeF(Math.Max(control.CurrentAutoScaleDimensions.Width, 1) / Math.Max(sc.Width, 1), Math.Max(1, control.CurrentAutoScaleDimensions.Height) / Math.Max(sc.Height, 1));
        }
    }
}
