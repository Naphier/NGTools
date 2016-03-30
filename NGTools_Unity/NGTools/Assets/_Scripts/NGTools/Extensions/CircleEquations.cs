namespace UnityEngine
{
    public class CircleEquations
    {
        public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180F)) + origin.x;
            float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180F)) + origin.y;

            return new Vector2(x, y);
        }

        public static float Circumference(float radius)
        {
            return (2f * Mathf.PI * radius);
        }

        public static float Radius(float circumference)
        {
            return (circumference / (2f * Mathf.PI));
        }
    }
}
