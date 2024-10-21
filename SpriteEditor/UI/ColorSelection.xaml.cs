using System.ComponentModel;
using System.Windows;
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

        public static readonly DependencyProperty SelectedColorProperty = 
            DependencyProperty.Register("SelectedColor", typeof(SeColor), typeof(ColorSelection), new PropertyMetadata());

        public SeColor SelectedColor
        {
            get { return (SeColor)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public ColorSelection()
        {
            InitializeComponent();
        }
    }
}
