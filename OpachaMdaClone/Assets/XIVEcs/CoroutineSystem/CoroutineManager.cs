using System.Collections;
using System.Collections.Generic;

namespace DalakLib.Coroutines
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


    public class CoroutineManager
    {
        public List<Routine> routines = new List<Routine>();

        public Routine StartCoroutine(IEnumerator routine)
        {
            var customCoroutine = new Routine(routine,this);
            routines.Add(customCoroutine);
            return customCoroutine;
        }

        public void StopCoroutine(Routine routine)
        {
            routines.Remove(routine);
        }

        public void StopAll()
        {
            routines.Clear();
        }

        public void Update()
        {
            for (int i = 0; i < routines.Count; i++)
            {
                if (routines[i].routine.Current is IEnumerator)
                    if (MoveNext((IEnumerator) routines[i].routine.Current))
                        continue;


                if (!routines[i].routine.MoveNext())
                {
                    routines.RemoveAt(i--);
                }
            }
        }

        bool MoveNext(IEnumerator routine)
        {
            if (routine.Current is IEnumerator)
            {
                if (MoveNext((IEnumerator) routine.Current))
                    return true;
            }

            return routine.MoveNext();
        }

        public int Count => routines.Count;

        public bool Running => routines.Count > 0;
    }
}