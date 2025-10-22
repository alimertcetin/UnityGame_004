using System;
using TMPro;
using XIV.Ecs;

namespace TheGame
{
    [Serializable]
    public struct TextComp : IComponent
    {
        public TMP_Text txt;
    }

    public class TextCompSerialized : SerializedComponent<TextComp>
    {
        
    }
}