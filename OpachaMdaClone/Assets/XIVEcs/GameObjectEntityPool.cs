using System.Collections.Generic;
using UnityEngine;

namespace XIV.Ecs
{
    public class GameObjectEntityPool
    {
        readonly GameObject prefab;
        readonly Transform container;
    
        readonly List<GameObjectEntity> availableViews = new List<GameObjectEntity>(32);
    
        public GameObjectEntityPool(GameObject prefab)
        {
            this.prefab = Object.Instantiate(prefab);
            this.prefab.SetActive(false);
        }

        public GameObjectEntity GetView()
        {
            GameObjectEntity view = null;
            if (availableViews.Count != 0)
            {
                view = availableViews[^1];
                availableViews.RemoveAt(availableViews.Count - 1);
            }
            else
            {
                view = Object.Instantiate(prefab, container).GetComponent<GameObjectEntity>();
            }

            view.owner = this;
            return view;
        }

        public void ReleaseView(GameObjectEntity view)
        {
            view.entity = Entity.Invalid;
            view.gameObject.SetActive(false);
            availableViews.Add(view);
        }

    }
}
