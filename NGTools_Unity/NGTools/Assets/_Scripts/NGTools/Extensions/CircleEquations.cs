namespace UnityEngine
{
    public class CircleEquations
    {
        public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180F)) + origin.x;
            float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180F)) + origin.y;

            x = Mathf.Clamp(x, float.MinValue, float.MaxValue);
            y = Mathf.Clamp(y, float.MinValue, float.MaxValue);

            return new Vector2(x, y);
        }

        public static float Circumference(float radius)
        {
            if (radius >= float.MaxValue)
                return float.MaxValue;

            if (radius <= float.MinValue)
                return float.MinValue;

            float circumference = (2f * Mathf.PI * radius);
            return circumference;
        }

        public static float Radius(float circumference)
        {
            if (circumference >= float.MaxValue)
                return float.MaxValue;

            if (circumference <= float.MinValue)
                return float.MinValue;


            return (circumference / (2f * Mathf.PI));
        }
    }
}
