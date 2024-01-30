using SpriteEditor.Code.Enums;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpriteEditor
{
    class EditorSettings
    {
        #region Private members

        Dictionary<uint, SeColor> _paletteDictionary;
        List<SeColor> _palette;
        int _gridScale = 10;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets video memory instance.
        /// </summary>
        public IVideoMemory VideoMemory { get; set; }

        /// <summary>
        /// Gets or sets current scale.
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// Returns true if the grid must be visible.
        /// </summary>
        public bool IsGridVisible { get { return Scale >= _gridScale; } }

        /// <summary>
        /// Gets or sets selected palette.
        /// </summary>
        public List<SeColor> Palette 
        {
            get { return _palette; } 
            set
            {
                _palette = value;
                _paletteDictionary = _palette.ToDictionary(c => c.NativeColor);
            } 
        }

        /// <summary>
        /// Gets selected palette in dictionary.
        /// </summary>
        public Dictionary<uint, SeColor> PaletteDictionary { get { return _paletteDictionary; } }

        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public SeColor Color { get; set; }

        public string FileName
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        public string FilePath { get; set; }

        public ImageType ImageType { get; set; }

        #endregion Public properties
    }
}
