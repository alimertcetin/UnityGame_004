
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    public struct InputListenerComp : IComponent
    {
        public SingleInputData singleInput;
        public MultiInputData multiInput;
    }
    
    public class InputListenerSerialized : SerializedComponent<InputListenerComp>
    {
        
    }
}