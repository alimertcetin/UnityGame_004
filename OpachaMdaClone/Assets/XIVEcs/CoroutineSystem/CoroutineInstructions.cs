using System.Collections;
using UnityEngine;

namespace DalakLib.Coroutines
{
    public static class Wait
    {
        public delegate bool ComparisonDelegate();
        
        public static IEnumerator Seconds(float time)
        {
            float timer = 0;
            while (timer < time)
            {
                timer += Time.deltaTime;
                yield return 0;
            }
        }

        public static IEnumerator Until(ComparisonDelegate comparison)
        {
            while (!comparison())
            {
                yield return 0;
            }
        }
        
        public static IEnumerator EndOfFrame(int frameCount)
        {
            int counter = 0;
            
            while (counter < frameCount)
            {
                counter++;
                yield return 0;
            }
        }
        
        public static IEnumerator MouseInputDown()
        {
            while (!Input.GetMouseButtonDown(0))
            {
                yield return 0;
            }
        }
        
    }

    
}


