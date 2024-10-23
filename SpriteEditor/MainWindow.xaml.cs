using Microsoft.Win32;
using SpriteEditor.Code.Commands;
using SpriteEditor.UI;
using SpriteEditor.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpriteEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainWindowViewModel(this);

            _editorView.EditorSettings = _viewModel.EditorSettings;
            _colorPanelBack.SelectionChanged += ColorPanelBack_SelectionChanged;
            _colorPanelInk.SelectionChanged += InkColorPanel_SelectionChanged;
            _colorPanelTransparent.SelectionChanged += ColorPanelTransparent_SelectionChanged;
            _btnFill.Click += Fill_Click;
            _scaleSlider.ValueChanged += ScaleSlider_ValueChanged;
            _btnMirrorVertically.Click += MirrorVertically;
        }

        private void MirrorVertically(object sender, RoutedEventArgs e)
        {
            _viewModel.MirrorVertically();
            FireRedraw();
        }

        private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetScale((int)e.NewValue);
        }

        void Fill_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.EditorSettings.VideoMemory == null || _viewModel.EditorSettings.TransparentColor == null)
                return;

            _viewModel.EditorSettings.VideoMemory.Fill(_viewModel.EditorSettings.TransparentColor.NativeColor);
            _editorView.InvalidateVisual();
        }

        void InkColorPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_colorPanelInk.SelectedItem == null)
            {
                if (_viewModel.EditorSettings.Palette.Any())
                    _viewModel.InkColor = _viewModel.EditorSettings.Palette[0];
            }
            else
            {
                _viewModel.InkColor = (SeColor)_colorPanelInk.SelectedItem;
            }
        }

        private void ColorPanelBack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_colorPanelBack.SelectedItem == null)
            {
                if (_viewModel.EditorSettings.Palette.Any())
                    _viewModel.BackColor = _viewModel.EditorSettings.Palette[0];
            }
            else
            {
                _viewModel.BackColor = (SeColor)_colorPanelBack.SelectedItem;
            }
        }

        void ColorPanelTransparent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_colorPanelTransparent.SelectedItem == null)
            {
                if (_viewModel.EditorSettings.Palette.Any())
                    _viewModel.TransparentColor = _viewModel.EditorSettings.Palette[0];
            }
            else
            {
                _viewModel.TransparentColor = (SeColor)_colorPanelTransparent.SelectedItem;
            }
        }

        private void Init()
        {
            var dlg = new NewImageDialog();
            if (dlg.ShowDialog() != true)
                return;
            
            var vm = new KorvetVideoMemory();
            vm.SetScreenSize(dlg.ScreenWidth, dlg.ScreenHight);
            _viewModel.New(vm);
            _editorView.SetNewSize();
        }

        private void FireRedraw()
        {
            _editorView.InvalidateVisual();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void SetScale(int scale)
        {
            int oldScale = _viewModel.Scale;
            _viewModel.SetScale(scale);
            if (oldScale != _viewModel.Scale)
            {
                _editorView.SetNewSize();
            }
        }

        private void ZoomIn()
        {
            int oldScale = _viewModel.Scale;
            _viewModel.ZoomIn();
            if (oldScale != _viewModel.Scale)
            {
                _editorView.SetNewSize();
            }
        }

        private void ZoomOut()
        {
            int oldScale = _viewModel.Scale;
            _viewModel.ZoomOut();
            if (oldScale != _viewModel.Scale)
            {
                _editorView.SetNewSize();
            }
        }

        private void MenuIte_New(object sender, RoutedEventArgs e)
        {
            Init();
        }

        const string FileFilter = "Sprites (*.spr)|*.spr";

        private void MenuItem_Open(object sender, RoutedEventArgs e)
        {
            var d = new OpenFileDialog();
            d.Filter = FileFilter;
            d.FilterIndex = 0;
            var ret = d.ShowDialog(this);
            if (!(ret.HasValue && ret.Value))
            {
                return;
            }
            var newModel = _viewModel.LoadFromFile(d.FileName);
            _viewModel.New(newModel);
            FireRedraw();
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            IOCommands.SaveSprite.Execute(_viewModel);
        }

        private void MenuItem_SaveAs(object sender, RoutedEventArgs e)
        {
            IOCommands.SaveAsSprite.Execute(_viewModel);
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ImageType_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.SetImageType((string)(e.Source as RadioButton).Tag);
        }

        private void EditorView_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                return;

            if (e.Delta < 0)
            {
                ZoomOut();
                e.Handled = true;
            }
            else if (e.Delta > 0)
            {
                ZoomIn();
                e.Handled = true;
            }
        }
    }
}
