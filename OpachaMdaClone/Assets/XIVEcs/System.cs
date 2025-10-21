using System.Collections;
using System.Collections.Generic;

namespace XIV.Ecs
{
    public abstract class System
    {
        public World world;
        public SystemManager manager;
        public CoroutineManager coroutineManager = new CoroutineManager();
        public bool active = true;

        public virtual void PreAwake() { }
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void FixedUpdate() { }
        public virtual void PreUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void OnDestroy() { }

        public Routine StartCoroutine(IEnumerator routine) => coroutineManager.StartCoroutine(routine);
        public void StopCoroutine(Routine routine) => coroutineManager.StopCoroutine(routine);
        public void StopAllCoroutines() => coroutineManager.StopAll();
    }
}