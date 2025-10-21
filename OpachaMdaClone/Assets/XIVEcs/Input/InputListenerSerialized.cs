
namespace XIV.Ecs
{
    public struct InputListenerComp : IComponent
    {
        public InputData input;
    }
    
    public class InputListenerSerialized : SerializedComponent<InputListenerComp>
    {
        
    }
}