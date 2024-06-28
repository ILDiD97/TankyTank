using UnityEngine;

namespace TrueRandomInteger
{
    public class TrueRandomInteger : MonoBehaviour
    {
        //General Function For Random Generation, the parameter number need return of Count function of Lists
        public static int GetRandomIntgerForList(int number)
        {
            if (number > 1)
            {

                int randomInt;
                float randomRange = Random.Range(0, 100);
                float randomFloat = randomRange / 100;
                randomInt = (int)(randomFloat * number);

                return Mathf.Clamp(randomInt, 0, number - 1);
            }
            else
            {
                return number - 1;
            }
        }
    }
}
