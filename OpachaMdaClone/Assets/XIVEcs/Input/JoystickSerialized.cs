using UnityEngine;
using XIV.UnityEngineIntegration;

namespace XIV.Ecs
{
    public struct JoystickInputData
    {
        public bool inputStarted;
        public bool inputStartedThisFrame;
        public bool inputEndedThisFrame;
        public bool inputEndedByFingerUp;

        public float inputAngle; // Angle towards to up vector from joystick direction
        public Vector2 inputDirection; // Normalized vector from joystick knob position respect to knob container

        /// <summary>
        /// calculated direction based on perceivedForward(where player looks) and input
        /// </summary>
        /// <param name="perceivedForward">
        /// perceivedForward = new Vector3(camera.forward.x,0,camera.forward.z).normalized
        /// </param>
        /// <returns></returns>
        public Vector3 MovementDir(Vector3 perceivedForward)
        {
            float magnitude = inputDirection.magnitude;
            var dir = Quaternion.Euler(0, inputAngle, 0) * perceivedForward;
            return dir * magnitude;
        }

        /// <summary>
        /// calculated direction based on perceivedForward(where player looks) and input
        /// returns inputMagnitude <= 0 ? Vector3.zero or normalizedDirection
        /// </summary>
        /// <param name="perceivedForward">
        /// perceivedForward = new Vector3(camera.forward.x,0,camera.forward.z).normalized
        /// </param>
        /// <returns></returns>
        public Vector3 MovementDirNormalized(Vector3 perceivedForward)
        {
            float magnitude = inputDirection.magnitude;
            var dir = Quaternion.Euler(0, inputAngle, 0) * perceivedForward;
            return magnitude <= Mathf.Epsilon ? Vector3.zero : dir.normalized;
        }
    }
    public struct JoystickComp : IComponent
    {
        public enum JoystickType
        {
            Static = 0,
            Dynamic = 1,
            DynamicAndFloating = 2
        }
        
        public RectTransform knobContainerContainer;
        public RectTransform knobContainer;
        public RectTransform knob;
        public JoystickType joystickType;
        public bool screenSpaceOverlay;

        public JoystickInputData joystickInputData;
    }
    
    public class JoystickSerialized : SerializedComponent<JoystickComp>
    {
        public RectTransform knobContainerContainer;
        public RectTransform knobContainer;
        public RectTransform knob;
        public JoystickComp.JoystickType joystickType;
        
        public override void AddComponentForEntity(Entity entity)
        {
            if (joystickType != JoystickComp.JoystickType.Static)
            {
                knobContainer.gameObject.SetActive(false);
            }
            entity.AddComponent(new JoystickComp
            {
                knobContainerContainer = knobContainerContainer,
                knobContainer = knobContainer,
                knob = knob,
                joystickType = joystickType,
                screenSpaceOverlay = knobContainerContainer.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay
            });
        }

        [Button]
        void AutoName()
        {
            knobContainerContainer.name = "KnobContainerContainer";
            knobContainer.name = "KnobContainer";
            knob.name = "Knob";
        }
    }
}