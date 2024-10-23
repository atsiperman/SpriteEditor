using Newtonsoft.Json;
using SpriteEditor.Code.Enums;
using SpriteEditor.Code.Storage;
using System.Collections.Generic;
using System.IO;

namespace SpriteEditor.Code
{
    static class FileSaver
    {
        class NativeEditorFileFormat
        {
            public List<SeColor> Palette { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public uint[] Data { get; set; }
            public uint[] Mask { get; set; }

            public uint InkColorNative { get; set; }
            public uint BackColorNative { get; set; }
            public uint TransparentColorNative { get; set; }
        }

        public static void Save(EditorSettings editor, string path, uint inkNativeColor, uint backNativeColor, uint maskNativeColor, ImageType imageType)
        {
            editor.FilePath = path;
            var ext = Path.GetExtension(path);
            bool hasChanges = editor.HasChanges;
            switch (ext)
            {
                case ".spr":
                    SaveNative(editor, path);
                    hasChanges = false;
                    break;

                case ".asm":
                    ExportData.Save(path, editor.VideoMemory, inkNativeColor, backNativeColor, maskNativeColor, imageType);
                    hasChanges = false;
                    break;

                default:
                    break;
            }
            editor.HasChanges = hasChanges;
        }

        private static void SaveNative(EditorSettings editor, string path)
        {
            var data = new NativeEditorFileFormat()
            {
                Palette = editor.Palette,
                Width = editor.VideoMemory.ScreenWidth,
                Height = editor.VideoMemory.ScreenHeight,
                TransparentColorNative = editor.TransparentColor.NativeColor,
                InkColorNative = editor.InkColor.NativeColor,
                BackColorNative = editor.BackColor.NativeColor,
            };
            data.Data = new uint[data.Width * data.Height];
            for (int h = 0; h < data.Height; h++)
            {
                for (int w = 0; w < data.Width; w++)
                {
                    var b = editor.VideoMemory.GetPixel(w, h);
                    data.Data[h * data.Width + w] = b;
                }
            }
            using (var writer = new StreamWriter(path))
            {
                var str = JsonConvert.SerializeObject(data);
                writer.WriteLine(str);
            }
        }

        public static EditorSettings Read(string path)
        {
            var json = string.Empty;
            using (var reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();    
            }
            var data = (NativeEditorFileFormat)JsonConvert.DeserializeObject(json, typeof(NativeEditorFileFormat));
            var editor = new EditorSettings();
            editor.Palette = data.Palette;
            editor.InkColor = data.Palette.Find(x => x.NativeColor == data.InkColorNative);
            editor.BackColor = data.Palette.Find(x => x.NativeColor == data.BackColorNative);
            editor.TransparentColor = data.Palette.Find(x => x.NativeColor == data.TransparentColorNative);
            editor.VideoMemory = new KorvetVideoMemory();
            editor.VideoMemory.SetScreenSize(data.Width, data.Height);

            for (int h = 0; h < data.Height; h++)
            {
                for (int w = 0; w < data.Width; w++)
                {
                    editor.VideoMemory.SetPixel(data.Data[h * data.Width + w], w, h);
                }
            }

            editor.FilePath = path;
            return editor;
        }
    }
}
