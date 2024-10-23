using SpriteEditor.ViewModels;
using System;
using System.IO;
using System.Windows.Input;

namespace SpriteEditor.Code.Commands
{
    internal class SaveSpriteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var vm = parameter as MainWindowViewModel;
            return vm.EditorSettings?.HasChanges ?? false;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            var vm = parameter as MainWindowViewModel;

            var filePath = vm.EditorSettings?.FilePath;
            if (filePath == null || !File.Exists(filePath))
                IOCommands.SaveAsSprite.Execute(parameter);
            else
                vm.SaveToFile(filePath);
        }
    }
}
