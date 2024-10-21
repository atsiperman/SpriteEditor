using System.ComponentModel;
using System.Windows.Controls;

namespace SpriteEditor.UI
{
    /// <summary>
    /// Interaction logic for ColorSelection.xaml
    /// </summary>
    public partial class ColorSelection : UserControl
    {
        [Category("Behavior")]
        public event SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                _colorPanel.SelectionChanged += value;
            }
            remove
            {
                _colorPanel.SelectionChanged -= value;
            }
        }

        public object SelectedItem => _colorPanel.SelectedItem;

        public ColorSelection()
        {
            InitializeComponent();
        }
    }
}
