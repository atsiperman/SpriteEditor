using SpriteEditor.Code.Enums;
using System.IO;

namespace SpriteEditor.Code.Storage
{
    static class ExportData
    {
        public static void Save(string path, IVideoMemory vm, uint inkColor, uint backColor, uint maskColor, ImageType imageType)
        {
            switch (imageType)
            {
                case ImageType.Sprite:
                    SaveSprite(path, vm, inkColor, backColor);
                    break;

                case ImageType.BackgroundTile:
                    SaveBackgroundTile(path, vm, inkColor);
                    break;

                case ImageType.StaticObject:
                    StaticObject(path, vm, inkColor, 3);
                    break;

                case ImageType.Text:
                    SaveText(path, vm, inkColor);
                    break;

                default:
                    break;
            }
        }

        private static void SaveText(string path, IVideoMemory vm, uint inkColor)
        {
            using (var stream = new StreamWriter(path))
            {
                for (int xPos = 0; xPos < vm.ScreenWidth / 8; xPos++)
                {
                    stream.Write("db");
                    for (var yPos = 0; yPos < vm.ScreenHeight; yPos++)
                    {
                        byte data = vm.GetMaskOfByte(xPos, yPos, inkColor);
                        stream.Write("{0} {1}", yPos == 0 ? "" : ",", data);
                    }
                    stream.WriteLine();
                }
            }
        }

        private static void SaveSprite(string path, IVideoMemory vm, uint inkColor, uint backColor)
        {
            using (var stream = new StreamWriter(path))
            {
                SaveSpritePlane(vm, inkColor, stream);
                SaveSpritePlane(vm, backColor, stream);
            }
        }

        private static void SaveSpritePlane(IVideoMemory vm, uint color, StreamWriter stream)
        {
            int[] idxSeq =   { 0, 4, 5, 1, 2, 6, 7, 3 };

            var width = vm.ScreenWidth / 8;
            var height = vm.ScreenHeight / 8;
            stream.WriteLine();
            for (var tileLine = 0; tileLine < height; tileLine++)
            {
                var startY = tileLine * 8;
                for (var k = 0; k < 8; k++)
                {
                    stream.Write("\tdb");
                    var firstByte = true;

                    var h = startY + idxSeq[k];
                    bool forward = k % 2 == 0;
                    int w = forward ? 0 : width - 1;
                    for (;;)
                    {
                        var data = vm.GetStringMaskOfByte(w, h, color) + "b";
                        w = forward ? w + 1 : w - 1;

                        stream.Write("{0}{1}", firstByte ? " " : ",", data);
                        firstByte = false;
                        
                        if (forward && w == width || !forward && w < 0)
                            break;
                    }
                    stream.WriteLine();
                }                
            }
        }

        private static void SaveBackgroundTile(string path, IVideoMemory vm, uint inkColor)
        {
            using (var stream = new StreamWriter(path))
            {
                stream.Write("db ");
                var firstByte = true;
                for (int w = 0; w < vm.ScreenWidth / 8; w++)
                {
                    for (var h = 0; h < 8; h++)
                    {
                        byte data = vm.GetMaskOfByte(w, h, inkColor);
                        stream.Write("{0}{1} ", firstByte ? " " : ", ", data);
                        firstByte = false;
                    }
                }

                stream.WriteLine();
            }
        }

        private static void StaticObject(string path, IVideoMemory vm, uint inkColor, uint paperColor)
        {
            using (var stream = new StreamWriter(path))
            {
                var width = vm.ScreenWidth / 8;
                var height = vm.ScreenHeight;
                stream.WriteLine("db {0},{1}", width, height);
                for (var line = 0; line < vm.ScreenHeight; line++)
                {
                    stream.Write($"db {inkColor}");
                    for (int w = 0; w < width; w++)
                    {
                        byte data = vm.GetMaskOfByte(w, line, inkColor);
                        stream.Write($", {data}");
                    }

                    stream.WriteLine($", {paperColor} ");
                }
            }
        }
    }
}
