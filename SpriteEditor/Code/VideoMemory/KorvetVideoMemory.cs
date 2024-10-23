using System;
using System.Collections.Generic;

namespace SpriteEditor
{
    public class KorvetVideoMemory : IVideoMemory
    {
        #region Private members

        int _videoWidth;
        int _videoHeight;
        int _bytesPerLine;

        List<SeColor> _palette;
        List<byte[]> _planes;

        #endregion Private members

        #region Public properties     

        public event EventHandler<EventArgs> Changed;

        public string Description { get { return "Korvet"; } }

        public int ScreenWidth { get { return _videoWidth; } }
        public int ScreenHeight { get { return _videoHeight; } }

        public List<SeColor> Palette { get { return _palette; } }

        #endregion Public properties

        public KorvetVideoMemory()
        {
            _planes = new List<byte[]>(3) { null, null, null };

            _palette = new List<SeColor>() 
                { 
                    new SeColor() { NativeColor = 0, A = 0xFF, R = 0,    G = 0,     B = 0, Label = "C_BLACK" },
                    new SeColor() { NativeColor = 1, A = 0xFF, R = 0,    G = 0xFF,  B = 0, Label = "C_GREEN" },
                    new SeColor() { NativeColor = 2, A = 0xFF, R = 0xFF, G = 0,     B = 0, Label = "C_RED" },
                    new SeColor() { NativeColor = 3, A = 0xFF, R = 0,    G = 0,     B = 0xFF, Label = "C_BLUE" },
                    new SeColor() { NativeColor = 4, A = 0xFF, R = 0,    G = 0xFF,  B = 0xFF, Label = "C_CYAN" },
                    new SeColor() { NativeColor = 5, A = 0xFF, R = 0xFF, G = 0xFF,  B = 0, Label = "C_YELLOW" },
                    new SeColor() { NativeColor = 6, A = 0xFF, R = 0xFF, G = 0,     B = 0xFF, Label = "C_PINK" },
                    new SeColor() { NativeColor = 7, A = 0xFF, R = 0xFF, G = 0xFF,  B = 0xFF, Label = "C_WHITE" },
                    //new SeColor() { NativeColor = 1024, A = 0xFF, R = 0xBF, G = 0xFF,  B = 0xD1 },
                };
        }

        #region Public methods

        public uint GetPixel(int x, int y)
        {
            int idx = GetIndexOfByte(x, y);
            int bit = 1 << (x % 8);
            uint bit0 = (uint)((_planes[0][idx] & bit) == 0 ? 0 : 1);
            uint bit1 = (uint)((_planes[1][idx] & bit) == 0 ? 0 : 1);
            uint bit2 = (uint)((_planes[2][idx] & bit) == 0 ? 0 : 1);

            uint color = (uint)(bit2 << 2) | (uint)(bit1 << 1) | (uint)bit0;

            return color;
        }

        public void SetPixel(uint color, int x, int y)
        {
            int idx = GetIndexOfByte(x, y);
            byte bit = (byte)(1 << (x % 8));

            for (int k = 0; k < _planes.Count; k++)
            {
                byte mask = _planes[k][idx];
                uint newColor = color & (uint)(1 << k);

                if (newColor > 0)
                    mask |= bit;
                else
                    mask &= (byte)~bit;

                _planes[k][idx] = mask;
            }
            FireChangedEvent();
        }

        public void Fill(uint nativeColor)
        {
            for (int k = 0; k < _planes.Count; k++)
            {
                byte newColor = (byte)((nativeColor & (uint)(1 << k)) == 0 ? 0 : 0xFF);

                for (int idx = 0; idx < _planes[k].Length; idx++)
                {
                    _planes[k][idx] = newColor;
                }
            }
            FireChangedEvent();
        }

        public void SetScreenSize(int width, int height)
        {
            // FIXME: add error checking

            _videoWidth = width;
            _videoHeight = height;
            _bytesPerLine = width / 8;
            for (int k = 0; k < _planes.Count; k++)
            {
                _planes[k] = new byte[_bytesPerLine * height];
            }
        }

        public IVideoMemory Clone()
        {
            var clone = new KorvetVideoMemory();
            clone.CopyFrom(this);
            return clone;
        }

        public void CopyFrom(IVideoMemory clone)
        {
            //FIXME:
            var vm = (clone as KorvetVideoMemory);
            this.SetScreenSize(clone.ScreenWidth, clone.ScreenHeight);
            for (int k = 0; k < vm._planes.Count; k++)
            {
                vm._planes[k].CopyTo(_planes[k], 0);
            }
        }

        public byte GetMaskOfByte(int x, int y, uint inkColor)
        {
            byte result = 0;
            for (int w = 0; w < 8; w++)
            {
                byte pixel = GetPixel(x * 8  + 7 - w, y) == inkColor ? (byte)1 : (byte)0;
                result |= (byte)(pixel << w);
            }
            return result;
        }

        public string GetStringMaskOfByte(int x, int y, uint inkColor)
        {
            string result = "";
            for (int w = 0; w < 8; w++)
            {
                byte pixel = GetPixel(x * 8 + w, y) == inkColor ? (byte)1 : (byte)0;
                result += pixel;
            }
            return result;
        }

        #endregion Public methods

        #region Private methods

        int GetIndexOfByte(int x, int y)
        {
            return y * _bytesPerLine + x / 8;
        }

        void FireChangedEvent()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #endregion Private methods
    }
}
