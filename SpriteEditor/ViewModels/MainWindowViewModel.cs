﻿using SpriteEditor.Code;
using SpriteEditor.Code.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SpriteEditor.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        #region Private members 

        List<SeColor> _colors;
        EditorSettings _editorSettings;

        #endregion Private members

        #region Public properties

        public bool IsViewEnabled { get { return EditorSettings != null && EditorSettings.VideoMemory != null; } }

        public EditorSettings EditorSettings 
        { 
            get { return _editorSettings; }
            private set
            {
                _editorSettings = value;
                OnNewEditorSettings();
                FireRenew();
            }
        }

        private void OnNewEditorSettings()
        {
            _editorSettings.Changed += EditorSettings_Changed;
        }

        private void ReleaseEditorSettings(EditorSettings editorSettings)
        {
            if (EditorSettings != null)
                editorSettings.Changed -= EditorSettings_Changed;
        }
        private void EditorSettings_Changed(object sender, EventArgs e)
        {
            FirePropertyChanged(nameof(Title));
        }

        public int MaxScale { get { return 32; } }

        public int Scale { get { return EditorSettings.Scale; } }

        public string ScaleText { get { return string.Format("x{0}", EditorSettings.Scale); } }
         
        public bool ZoomInEnabled
        {
            get { return EditorSettings.VideoMemory != null && EditorSettings.Scale < MaxScale; }
        }

        public bool ZoomOutEnabled
        { 
            get { return EditorSettings.VideoMemory != null && EditorSettings.Scale > 1; }
        }

        public string Title
        {
            get
            {
                return string.Format("{0}{1}{2}{3}", "Sprite Editor", string.IsNullOrEmpty(FilePath) ? string.Empty : " : ", FilePath, EditorSettings?.HasChanges == true ? "*" : "");
            }
        }

        public string FilePath
        {
            get { return EditorSettings.FilePath; }
        }

        public List<SeColor> Colors
        {
            get { return _colors; }
            set
            {
                _colors = value;
                FirePropertyChanged(nameof(Colors));
            }
        }

        public SeColor InkColor
        {
            get { return EditorSettings.InkColor; }
            set
            {
                EditorSettings.InkColor = value;
                FirePropertyChanged(nameof(InkColor));
            }
        }

        public SeColor BackColor
        {
            get { return EditorSettings.BackColor; }
            set
            {
                EditorSettings.BackColor = value;
                FirePropertyChanged(nameof(BackColor));
            }
        }

        public SeColor TransparentColor
        {
            get { return EditorSettings.TransparentColor; }
            set
            {
                EditorSettings.TransparentColor = value;
                FirePropertyChanged(nameof(TransparentColor));
            }
        }

        public ImageType ImageType
        {
            get { return EditorSettings.ImageType; }
            set
            {
                EditorSettings.ImageType = value;
                FirePropertyChanged(nameof(ImageType));
            }
        }

        public Window Window { get; private set; }

        #endregion Public properties

        #region Constructors

        public MainWindowViewModel(Window window)
        {
            EditorSettings = new EditorSettings();
            Window = window;
        }

        #endregion Constructors

        #region Private methods

        private void OnScaleChanged()
        {
            FirePropertyChanged(nameof(ZoomInEnabled));
            FirePropertyChanged(nameof(ZoomOutEnabled));
            FirePropertyChanged(nameof(Scale));
            FirePropertyChanged(nameof(ScaleText));
        }

        private void FireRenew()
        {
            FirePropertyChanged(nameof(InkColor));
            FirePropertyChanged(nameof(BackColor));
            FirePropertyChanged(nameof(TransparentColor));
            FirePropertyChanged(nameof(IsViewEnabled));
            FirePropertyChanged(nameof(Title));
        }

        #endregion Private methods

        #region Public methods

        public void New(EditorSettings settings)
        {
            NewHelper(settings.VideoMemory);
            EditorSettings.FilePath = settings.FilePath;
            EditorSettings.InkColor = settings.InkColor;
            EditorSettings.BackColor = settings.BackColor;
            EditorSettings.TransparentColor = settings.TransparentColor;
            FireRenew();
        }

        public void New(IVideoMemory videoMemory)
        {
            NewHelper(videoMemory);
            FireRenew();
        }

        private void NewHelper(IVideoMemory videoMemory)
        {
            EditorSettings.FilePath = "New Object";
            EditorSettings.VideoMemory = videoMemory;
            EditorSettings.Scale = 10;
            EditorSettings.Palette = videoMemory.Palette;

            if (EditorSettings.Palette.Any())
                InkColor = EditorSettings.Palette.First();

            Colors = videoMemory.Palette;
            EditorSettings.HasChanges = false;
            OnScaleChanged();
        }

        public void SetScale(int scale)
        {
            if (scale > 0 && scale <= MaxScale)
            {
                EditorSettings.Scale = scale;
                OnScaleChanged();
            }
        }

        public void ZoomIn()
        {
            if (EditorSettings.Scale < MaxScale)
            {
                EditorSettings.Scale++;
                OnScaleChanged();
            }
        }

        public void ZoomOut()
        {
            if (EditorSettings.Scale > 1)
            {
                EditorSettings.Scale--;
                OnScaleChanged();
            }
        }

        public void SaveToFile(string file)
        {
            FileSaver.Save(EditorSettings, file, InkColor.NativeColor, BackColor.NativeColor, TransparentColor.NativeColor, ImageType);
            FirePropertyChanged(nameof(Title));
        }

        public EditorSettings LoadFromFile(string file)
        {
            return FileSaver.Read(file);
        }

        public void MirrorVertically()
        {
            var vm = EditorSettings.VideoMemory;
            for (var y = 0; y < vm.ScreenHeight; y++)
            {
                for (var x = 0; x < vm.ScreenWidth / 2; x++)
                {
                    var rightIndex = vm.ScreenWidth - 1 - x;
                    var a = vm.GetPixel(x, y);
                    var b = vm.GetPixel(rightIndex, y);
                    vm.SetPixel(b, x, y);
                    vm.SetPixel(a, rightIndex, y);
                }
            }
        }

        public void SetImageType(string imageType)
        {
            var imType = (ImageType)Enum.Parse(typeof(ImageType), imageType);
            ImageType = imType;
        }

        #endregion Public methods
    }
}
