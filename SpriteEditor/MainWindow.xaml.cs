using Microsoft.Win32;
using SpriteEditor.UI;
using SpriteEditor.ViewModels;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
            this.DataContext = _viewModel = new MainWindowViewModel();

            _editorView.EditorSettings = _viewModel.EditorSettings;
            _colorPanel.SelectionChanged += ColorPanel_SelectionChanged;
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
            if (_viewModel.EditorSettings.VideoMemory == null || _viewModel.EditorSettings.Color == null)
                return;

            _viewModel.EditorSettings.VideoMemory.Fill(_viewModel.EditorSettings.Color.NativeColor);
            _editorView.InvalidateVisual();
        }

        void ColorPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_colorPanel.SelectedItem == null)
            {
                if (_viewModel.EditorSettings.Palette.Any())
                    _viewModel.SelectedColor = _viewModel.EditorSettings.Palette[0];
            }
            else
            {
                _viewModel.SelectedColor = (SeColor)_colorPanel.SelectedItem;
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

        const string FileFilter = "Sprites (*.spr)|*.spr|Sprite data (*.dat)|*.dat";

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
            if (string.IsNullOrEmpty(_viewModel.FilePath))
            {
                MenuItem_SaveAs(sender, e);
            }
            else
            {
                _viewModel.SaveToFile(_viewModel.FilePath);
            }
        }

        private void MenuItem_SaveAs(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();            
            d.Filter = FileFilter;
            d.FilterIndex = 0;
            d.OverwritePrompt = true;
            d.FileName = Path.GetFileName(_viewModel.FilePath);
            var ret = d.ShowDialog(this);
            if (!(ret.HasValue && ret.Value))
            {
                return;
            }
            _viewModel.SaveToFile(d.FileName);
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ImageType_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.SetImageType((string)(e.Source as RadioButton).Tag);
        }
    }
}
