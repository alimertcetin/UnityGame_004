using UnityEngine;
using UnityEngine.UI;

namespace XIV.Ecs
{
    public struct ButtonComp : IComponent
    {
        public ButtonMono buttonMono;
    }
    public struct ButtonClickedTag : ITag
    {
        
    }
    
    public class ButtonSerialized : SerializedComponent<ButtonComp>
    {
        public Button[] buttons;

        public override void AddComponentForEntity(Entity entity)
        {
            var buttonMono = gameObject.AddComponent<ButtonMono>();
            foreach (var button in buttons)
            {
                button.onClick.AddListener(() => buttonMono.clicked = true);
            }
            
            entity.AddComponent(new ButtonComp
            {
                buttonMono = buttonMono
            });
        }

        void OnValidate()
        {
            if (buttons == null || buttons.Length == 0)
            {
                buttons = GetComponents<Button>();
            }
        }
    }

    public class ButtonMono : MonoBehaviour
    {
        public bool clicked = false;
    }
    
    
}