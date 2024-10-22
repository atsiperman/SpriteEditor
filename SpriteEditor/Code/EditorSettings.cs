using SpriteEditor.Code.Enums;
using System;
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
        IVideoMemory _videoMemory;

        #endregion

        #region Public properties

        public event EventHandler<EventArgs> Changed;

        /// <summary>
        /// Gets or sets video memory instance.
        /// </summary>
        public IVideoMemory VideoMemory 
        { 
            get {  return _videoMemory; }
            set
            {
                ReleaseVideoMemory(_videoMemory);
                _videoMemory = value;
                OnNewVideoMemory();
            }
        }

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
        public SeColor InkColor { get; set; } = new SeColor();

        /// <summary>
        /// Gets or sets selected color.
        /// </summary>
        public SeColor BackColor { get; set; } = new SeColor();

        /// <summary>
        /// Gets or sets transparent color.
        /// </summary>
        public SeColor TransparentColor { get; set; } = new SeColor();

        public string FileName
        {
            get
            {
                return string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                _hasChanges = value;
                FireDataChanged();
            }
        }

        public string FilePath { get; set; }

        public ImageType ImageType { get; set; }

        #endregion Public properties

        private void OnNewVideoMemory()
        {
            _videoMemory.Changed += Data_Changed;
        }

        private void ReleaseVideoMemory(IVideoMemory videoMemory)
        {
            if (_videoMemory != null)
                _videoMemory.Changed -= Data_Changed;
        }

        private void Data_Changed(object sender, EventArgs e)
        {
            HasChanges = true;
            FireDataChanged();
        }

        void FireDataChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
