namespace SpriteEditor.Editor
{
    class FillAllAction : IEditAction
    {
        #region Private members

        uint _nativeColorNew;
        IVideoMemory _copyOfOldMemory;

        #endregion Private members

        #region Constructors

        public FillAllAction(IVideoMemory copyOfOldMemory, uint nativeColorNew)
        {
            _copyOfOldMemory = copyOfOldMemory;
            _nativeColorNew = nativeColorNew;
        }

        #endregion Constructors

        #region Private methods
        #endregion Private methods

        #region Public methods

        public void Redo(IVideoMemory videoMemory)
        {
            videoMemory.Fill(_nativeColorNew);
        }

        public void Undo(IVideoMemory videoMemory)
        {
            videoMemory.CopyFrom(_copyOfOldMemory);
        }

        #endregion Public methods
    }
}
