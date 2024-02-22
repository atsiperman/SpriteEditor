using System.Collections.Generic;

namespace SpriteEditor.Code.Sprites
{
    public class SpriteMap
    {
        private List<Sprite> _sprites = new List<Sprite>();

        public int Width { get; set; }
        public int Height { get; set; }

        public IReadOnlyList<Sprite> Sprites => _sprites;        

        public SpriteMap(int w, int h) 
        { 
            Width = w;
            Height = h;
        }

        public void AddSprite(Sprite sprite)
        {
            _sprites.Add(sprite);
        }
    }
}
