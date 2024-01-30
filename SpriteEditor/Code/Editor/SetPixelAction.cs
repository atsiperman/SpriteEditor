namespace SpriteEditor.Editor
{
    class SetPixelAction : IEditAction
    {
        #region Private members

        uint _nativeColorOld;
        uint _nativeColorNew;
        int _x;
        int _y;

        #endregion Private members

        #region Constructors

        public SetPixelAction(uint nativeColorOld, uint nativeColorNew, int x, int y)
        {
            _nativeColorOld = nativeColorOld;
            _nativeColorNew = nativeColorNew;
            _x = x;
            _y = y;
        }

        #endregion Constructors

        #region Public methods

        public void Redo(IVideoMemory videoMemory)
        {
            videoMemory.SetPixel(_nativeColorNew, _x, _y);
        }

        public void Undo(IVideoMemory videoMemory)
        {
            videoMemory.SetPixel(_nativeColorOld, _x, _y);
        }

        #endregion Public methods
    }
}
