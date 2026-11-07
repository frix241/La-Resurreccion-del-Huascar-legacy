using UnityEngine;

namespace LRH.Runtime
{
    public static class UiTheme
    {
        private static Sprite surface, normal, hover, pressed, disabled, track, health, gold, danger, knob;
        public static Sprite Surface => surface ?? (surface = Skin("panel"));
        public static Sprite Normal => normal ?? (normal = Skin("normal"));
        public static Sprite Hover => hover ?? (hover = Skin("hover"));
        public static Sprite Pressed => pressed ?? (pressed = Skin("pressed"));
        public static Sprite Disabled => disabled ?? (disabled = Skin("disabled"));
        public static Sprite Track => track ?? (track = Gradient(new Color(.008f,.025f,.04f),new Color(.025f,.065f,.08f),false));
        public static Sprite Health => health ?? (health = Gradient(new Color(.10f,.48f,.39f),new Color(.40f,.83f,.64f),false));
        public static Sprite Gold => gold ?? (gold = Gradient(new Color(.58f,.35f,.12f),new Color(.96f,.79f,.40f),false));
        public static Sprite Danger => danger ?? (danger = Gradient(new Color(.48f,.12f,.13f),new Color(.94f,.38f,.29f),false));
        public static Sprite Knob => knob ?? (knob = Gradient(new Color(.66f,.48f,.22f),new Color(.99f,.89f,.63f),true));

        private static Sprite Skin(string name)
        {
            var texture=Resources.Load<Texture2D>("Retro/"+name);
            if(texture==null)throw new System.InvalidOperationException("Falta UI Artstyle/"+name);
            texture.filterMode=FilterMode.Bilinear;
            return Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),new Vector2(.5f,.5f),100f,
                0,SpriteMeshType.FullRect,new Vector4(24,24,24,24),false);
        }

        private static Sprite Gradient(Color bottom,Color top,bool border)
        {
            const int size=64;
            var texture=new Texture2D(size,size,TextureFormat.RGBA32,false);
            texture.filterMode=FilterMode.Bilinear;texture.wrapMode=TextureWrapMode.Clamp;
            var pixels=new Color[size*size];
            for(int y=0;y<size;++y)
            for(int x=0;x<size;++x)
            {
                float t=y/(float)(size-1);
                var color=Color.Lerp(bottom,top,t);
                if(border && (x<2 || x>=size-2 || y<2 || y>=size-2))color=new Color(.72f,.56f,.31f);
                const int radius=10;
                bool cut=(x<radius && y<radius && (x-radius)*(x-radius)+(y-radius)*(y-radius)>radius*radius) ||
                    (x>=size-radius && y<radius && (x-(size-radius-1))*(x-(size-radius-1))+(y-radius)*(y-radius)>radius*radius) ||
                    (x<radius && y>=size-radius && (x-radius)*(x-radius)+(y-(size-radius-1))*(y-(size-radius-1))>radius*radius) ||
                    (x>=size-radius && y>=size-radius && (x-(size-radius-1))*(x-(size-radius-1))+(y-(size-radius-1))*(y-(size-radius-1))>radius*radius);
                if(cut)color.a=0;
                pixels[y*size+x]=color;
            }
            texture.SetPixels(pixels);texture.Apply(false,true);
            return Sprite.Create(texture,new Rect(0,0,size,size),new Vector2(.5f,.5f),100f,0,
                SpriteMeshType.FullRect,new Vector4(12,12,12,12),false);
        }
    }
}
