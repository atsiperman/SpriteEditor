using Microsoft.Win32;
using SpriteEditor.ViewModels;
using System;
using System.IO;
using System.Windows.Input;

namespace SpriteEditor.Code.Commands
{
    internal class SaveAsSpriteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var vm = parameter as MainWindowViewModel;
            return vm.EditorSettings?.VideoMemory != null;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            var vm = parameter as MainWindowViewModel;

            var d = new SaveFileDialog();
            d.Filter = FileFilter;
            d.FilterIndex = 0;
            d.OverwritePrompt = true;
            d.FileName = Path.GetFileName(vm.FilePath);
            var ret = d.ShowDialog(vm.Window);
            if (!(ret.HasValue && ret.Value))
                return;

            vm.SaveToFile(d.FileName);
        }

        const string FileFilter = "Sprites (*.spr)|*.spr|Sprite code (*.asm)|*.asm";
    }
}
