using TMPro;
using XIV.Ecs;
using XIV.UnityEngineIntegration;

namespace TheGame
{

    public enum NumericTextDisplayType
    {
        Exact = 0,
        Percent,
        Normalized01,
    }
    
    [System.Serializable]
    public struct NumericTextComp : IComponent
    {
        public TMP_Text txt;
        public NumericTextDisplayType displayType;
    }
    
    public class SerializedNumericTextComp : SerializedComponent<NumericTextComp>
    {
        void OnValidate()
        {
            component.txt ??= GetComponentInChildren<TMP_Text>();
        }
    }
}