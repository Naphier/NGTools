using UnityEngine;

/// <summary>
/// Gets a weighted random number
/// Usage
/// float y = WeightedRandom.GetRandomValue(
///                 new WeightedRandom.RandomSelection(100f, 80f , 0.20f),
///                 new WeightedRandom.RandomSelection(80f, 60f, 0.40f),
///                 new WeightedRandom.RandomSelection(60f , 30f, 0.30f),
///                 new WeightedRandom.RandomSelection(30f , 0f , 0.10f)
/// );
/// </summary>
public class WeightedRandom
{
    public static float GetRandomValue(params RandomSelection[] selections)
    {
        float rand = Random.value;
        float currentProb = 0;
        for (int i = 0; i < selections.Length; i++)
        {
            currentProb += selections[i].probability;
            if (rand <= currentProb)
                return selections[i].GetValue();
        }

        //will happen if the input's probabilities sums to less than 1
        //throw error here if that's appropriate
        return -1;
    }

    public struct RandomSelection
    {
        private float minValue;
        private float maxValue;
        public float probability;

        public RandomSelection(float minValue, float maxValue, float probability)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.probability = probability;
        }

        public float GetValue() { return Random.Range(minValue, maxValue + 1f); }
    }
}

