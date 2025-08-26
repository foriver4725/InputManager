namespace foriver4725.InputManager
{
    internal static partial class InputManager
    {
        internal static InputInfo Click { get; private set; }
        internal static InputInfo Hold { get; private set; }
        internal static InputInfo Value0 { get; private set; }
        internal static InputInfo Value1 { get; private set; }
        internal static InputInfo Value2 { get; private set; }
        internal static InputInfo Value3 { get; private set; }

        private static void Bind()
        {
            Click = Create(source.Main.Click, InputType.Click);
            Hold = Create(source.Main.Hold, InputType.Hold);
            Value0 = Create(source.Main.Value0, InputType.Value0);
            Value1 = Create(source.Main.Value1, InputType.Value1);
            Value2 = Create(source.Main.Value2, InputType.Value2);
            Value3 = Create(source.Main.Value3, InputType.Value3);
        }
    }
}
