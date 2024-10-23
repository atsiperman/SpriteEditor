using SpriteEditor.Code.Storage;
using SpriteEditor.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SpriteEditor.Code.Commands
{
    internal class ExportSpriteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var vm = parameter as MainWindowViewModel;
            var filePath = MakeFilePath(vm.EditorSettings?.FilePath);
            return filePath != null && File.Exists(filePath);
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            var es = ((MainWindowViewModel)parameter).EditorSettings;
            var filePath = MakeFilePath(es.FilePath);
            ExportData.Save(filePath, es.VideoMemory, es.InkColor.NativeColor, es.BackColor.NativeColor, es.TransparentColor.NativeColor, Enums.ImageType.Sprite);
            MessageBox.Show("Export done.", filePath);
        }

        private string MakeFilePath(string filePath)
        {
            if (filePath == null)
                return null;

            var path = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);            
            return Path.Combine(path, $"{fileName}.asm");
        }
    }
}
