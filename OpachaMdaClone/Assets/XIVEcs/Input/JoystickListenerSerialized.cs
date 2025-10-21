using UnityEngine;
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    public struct JoystickListenerComp : IComponent
    {
        public Entity[] joystickEntities;
        public JoystickInputData[] inputs;
    }
    
    public class JoystickListenerSerialized : SerializedComponent<JoystickListenerComp>
    {
        [Header("You can leave it empty if you have a single joystick in the scene")]
        public JoystickSerialized[] joysticks; 
        
        public override object GetComponentData(World world)
        {
            return new JoystickListenerComp
            {
                joystickEntities = (joysticks == null || joysticks.Length == 0) 
                    ? null 
                    : joysticks.DalToEntities(),
                inputs = (joysticks == null || joysticks.Length == 0) ?
                    null :
                    new JoystickInputData[joysticks.Length]
            };
        }

        [Button]
        void AutoFill()
        {
            var joystickSerializedList = FindObjectsOfType<JoystickSerialized>();
            if (joysticks == null || joysticks.Length == 0)
            {
                joysticks = new JoystickSerialized[1];
            }
            
            int i = 0;
            for (; i < joystickSerializedList.Length; i++)
            {
                if (joystickSerializedList[i] == joysticks[0])
                {
                    break;
                }   
            }
            
            joysticks[0] = joystickSerializedList[(i + 1) % joystickSerializedList.Length]; 
        }

    }
}