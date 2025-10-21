using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DalakLib.Coroutines
{
    public static class RoutineExtensions
    {
        public static void Stop(this Routine routine)
        {
            routine.coroutineManager.StopCoroutine(routine);
        }

        public static bool IsRunning(this Routine routine)
        {
            return routine.coroutineManager.routines.Contains(routine);
        }
    }
}