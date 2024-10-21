using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpriteEditor.UI
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        #region Private members

        #endregion Private members

        #region Public properties
        
        internal EditorSettings EditorSettings { get; set; }

        #endregion Public properties

        #region Constructors

        public EditorView()
        {
            InitializeComponent();

            this.MouseLeftButtonUp += EditorView_MouseLeftButtonUp;
            this.MouseMove += EditorView_MouseMove;
        }

        #endregion Constructors

        #region Private methods

        void EditorView_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton & MouseButtonState.Pressed) == 0 ||
                EditorSettings == null || EditorSettings.VideoMemory == null || EditorSettings.Color == null)
            {
                return;
            }
            var point = e.GetPosition(this);
            SetPixel(point);
            this.InvalidateVisual();
        }

        void EditorView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EditorSettings == null || EditorSettings.VideoMemory == null || EditorSettings.Color == null)
                return;

            var point = e.GetPosition(this);
            SetPixel(point);
            this.InvalidateVisual();
        }

        void SetPixel(Point point)
        {
            var dotW = EditorSettings.IsGridVisible ? EditorSettings.Scale + 1 : EditorSettings.Scale;
            int x = (int)(point.X - OffsetX) / dotW;
            int y = (int)(point.Y - OffsetY) / dotW;

            EditorSettings.VideoMemory.SetPixel(EditorSettings.Color.NativeColor, x, y);
        }

        private int OffsetX => EditorSettings.IsGridVisible ? EditorSettings.Scale + 1 : 0;
        private int OffsetY => EditorSettings.IsGridVisible ? EditorSettings.Scale + 1 : 0;

        protected override void OnRender(DrawingContext drawingContext)
        {
            //base.OnRender(drawingContext);

            drawingContext.DrawRectangle(new SolidColorBrush(Colors.LightGray), null, new Rect(0, 0, this.Width, this.Height));

            if (EditorSettings == null || EditorSettings.VideoMemory == null || EditorSettings.PaletteDictionary == null)
                return;

            int scale = EditorSettings.Scale;
            int offset = !EditorSettings.IsGridVisible ? scale : scale + 1;
            double yStart = 0, xStart = OffsetX;

            if (EditorSettings.IsGridVisible)
            {
                Point textPos = new Point(0, offset);
                for (int y = 0; y < EditorSettings.VideoMemory.ScreenHeight; y++)
                {
                    var text = new FormattedText($"{y}", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black, 1);
                    drawingContext.DrawText(text, textPos);
                    textPos.Offset(0, offset);
                }
            }

            Rect rect = new Rect(xStart, yStart, scale, scale);

            for (int x = 0; x < EditorSettings.VideoMemory.ScreenWidth; x++)
            {
                if (EditorSettings.IsGridVisible)
                {
                    var text = new FormattedText($"{x}", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black, 1);
                    drawingContext.DrawText(text, rect.TopLeft);
                    rect.Offset(0, offset);
                }

                for (int y = 0; y < EditorSettings.VideoMemory.ScreenHeight; y++)
                {
                    var cidx = EditorSettings.VideoMemory.GetPixel(x, y);
                    if (!EditorSettings.PaletteDictionary.ContainsKey(cidx))
                        continue;

                    var color = EditorSettings.PaletteDictionary[cidx];
                    var brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

                    drawingContext.DrawRectangle(brush, null, rect);
                    rect.Offset(0, offset);
                }
                rect.Y = yStart;
                rect.Offset(offset, 0);
            }
        }

        #endregion Private methods

        #region Public methods

        public void SetNewSize()
        {
            if (EditorSettings == null || EditorSettings.VideoMemory == null)
                return;

            int scale = EditorSettings.Scale;
            int screenW = EditorSettings.VideoMemory.ScreenWidth;
            int screenH = EditorSettings.VideoMemory.ScreenHeight;
            int w, h;

            if (!EditorSettings.IsGridVisible)
            {
                w = screenW * scale;
                h = screenH  * scale;
            }
            else
            {
                w = (screenW + 1) * (scale + 1) - 1;
                h = (screenH + 1) * (scale + 1) - 1;
            }
            
            this.Width = w;
            this.Height = h;
        }

        #endregion Public methods
    }
}
