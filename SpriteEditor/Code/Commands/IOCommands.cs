namespace SpriteEditor.Code.Commands
{
    internal static class IOCommands
    {
        public static ExportSpriteCommand ExportSprite { get; } = new ExportSpriteCommand();
        public static SaveAsSpriteCommand SaveAsSprite { get; } = new SaveAsSpriteCommand();
        public static SaveSpriteCommand SaveSprite { get; } = new SaveSpriteCommand();
    }
}
