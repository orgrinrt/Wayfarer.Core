using Godot;

namespace Wayfarer.Core.Systems.Managers
{
    public class MouseManager : Manager
    {
        [Export()] public Resource Cursor;
        [Export()] public AtlasTexture GlyphUse;
        [Export()] public AtlasTexture GlyphTalk;
        [Export()] public AtlasTexture GlyphMore;
        [Export()] public AtlasTexture GlyphLeave;
        [Export()] public AtlasTexture GlyphInspect;
        [Export()] public Vector2 GlyphOffset; // 28x28 looks good

        private CanvasLayer _canvas;
        private Sprite _glyph;
        
        public override void _Ready()
        {
            base._Ready();

            _canvas = GetNode<CanvasLayer>("./CanvasLayer");
            _glyph = _canvas.GetNode<Sprite>("./Glyph");
            
            Input.SetCustomMouseCursor(Cursor);
            
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            
            _glyph.SetGlobalPosition(_glyph.GetGlobalMousePosition() + GlyphOffset);
        }

        public void SetCursorGlyph(CursorGlyph targetGlyph)
        {
            switch (targetGlyph)
            {
                case CursorGlyph.None:
                    DropGlyph();
                    break;
                case CursorGlyph.Use:
                    _glyph.Texture = GlyphUse;
                    break;
                case CursorGlyph.More:
                    _glyph.Texture = GlyphMore;
                    break;
                case CursorGlyph.Talk:
                    _glyph.Texture = GlyphTalk;
                    break;
                case CursorGlyph.Leave:
                    _glyph.Texture = GlyphLeave;
                    break;
                case CursorGlyph.Inspect:
                    _glyph.Texture = GlyphInspect;
                    break;
            }
        }

        private void DropGlyph()
        {
            _glyph.Texture = null;
        }
    }
}