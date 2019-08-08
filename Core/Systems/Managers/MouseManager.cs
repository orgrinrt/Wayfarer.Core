using System;
using Godot;

namespace Wayfarer.Core.Systems.Managers
{
    public class MouseManager : Manager
    {
        [Signal] public delegate void StoppedDragging(Node draggedNode);
        [Signal] public delegate void StartedDragging(Node draggedNode);
        
        [Export()] public Resource Cursor;
        [Export()] public AtlasTexture GlyphUse;
        [Export()] public AtlasTexture GlyphTalk;
        [Export()] public AtlasTexture GlyphMore;
        [Export()] public AtlasTexture GlyphLeave;
        [Export()] public AtlasTexture GlyphInspect;
        [Export()] public Vector2 GlyphOffset; // 28x28 looks good

        private CanvasLayer _canvas;
        private Sprite _glyph;
        private Node _draggedNode;
        private bool _isDragging;
        private Vector2 _dragAnchor = Vector2.Zero;

        public Node DraggedNode => _draggedNode;
        public bool IsDragging => _isDragging;
        public Vector2 DragAnchor => _dragAnchor;
        
        public override void _Ready()
        {
            base._Ready();

            _canvas = GetNode<CanvasLayer>("./CanvasLayer");
            if (!IsInstanceValid(_canvas) || _canvas == null)
            {
                _canvas = new CanvasLayer() { Name = "CanvasLayer" };
                AddChild(_canvas);
            }

            _glyph = _canvas.GetNode<Sprite>("./Glyph");
            if (!IsInstanceValid(_glyph) || _glyph == null)
            {
                _glyph = new Sprite() {Name = "Glyph"};
                AddChild(_glyph);
            }

            if (IsInstanceValid(Cursor))
            {
                Input.SetCustomMouseCursor(Cursor);
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (!Input.IsMouseButtonPressed((int) ButtonList.Left) && IsDragging)
            {
                _isDragging = false;
                EmitSignal(nameof(StoppedDragging), _draggedNode);
                _draggedNode = null; // this may cause the above be null as well, so if problems, check this first
            }
            else if (IsDragging)
            {
                if (DraggedNode is Control control)
                {
                    control.SetGlobalPosition(control.GetGlobalMousePosition() - DragAnchor);
                }
                else if (DraggedNode is Node2D node2D)
                {
                    node2D.SetGlobalPosition(node2D.GetGlobalMousePosition() - DragAnchor);
                }
            }
            
            _glyph.SetGlobalPosition(_glyph.GetGlobalMousePosition() + GlyphOffset);
        }

        public void StartDragging(Node node, Vector2 dragAnchor)
        {
            _draggedNode = node;
            _dragAnchor = dragAnchor;
            _isDragging = true;
            EmitSignal(nameof(StartedDragging), _draggedNode);
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