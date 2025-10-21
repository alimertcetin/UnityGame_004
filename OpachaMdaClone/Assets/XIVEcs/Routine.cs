using System.Collections;

namespace XIV.Ecs
{
    public class Routine
    {
        public IEnumerator routine;
        public CoroutineManager coroutineManager;
        
        public Routine(IEnumerator routine, CoroutineManager coroutineManager)
        {
            this.routine = routine;
            this.coroutineManager = coroutineManager;
        }
    }
}