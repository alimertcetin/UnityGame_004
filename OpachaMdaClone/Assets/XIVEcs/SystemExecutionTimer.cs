using System.Collections.Generic;
using System.Diagnostics;

namespace XIV.Ecs
{
    public class SystemExecutionTimer
    {
        public Dictionary<System, long[]> systemExeTimeDic = new Dictionary<System, long[]>();
        readonly Stopwatch watch = new Stopwatch();

        public void StartWatch()
        {
            watch.Restart();
        }

        public void StopWatch(System system, MethodType methodType)
        {
            watch.Stop();

            if (systemExeTimeDic.TryGetValue(system,out var durations))
            {
                durations[(int)methodType] = watch.ElapsedMilliseconds;
            }
            else
            {
                durations = new long[(int) MethodType.NumberOfMethods];
                durations[(int) methodType] = watch.ElapsedMilliseconds;
                systemExeTimeDic.Add(system, durations);
            }
        }

        public enum MethodType
        {
            PreAwake = 0,
            Awake,
            Start,
            PreUpdate,
            Update,
            FixedUpdate,
            LateUpdate,
            NumberOfMethods
        }
    }
}