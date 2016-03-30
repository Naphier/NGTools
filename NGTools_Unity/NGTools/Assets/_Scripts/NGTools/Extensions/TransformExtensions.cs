namespace UnityEngine
{
    /// <summary>
    /// Extansioon methods for Vector3 modification and easy transform vector3 modifications.
    /// Each method has optional parameters so you can call them in your preferred order and/or
    /// omit parameters you don't want to use. For example, vector3.SetValues(z:10) will set 
    /// only the z value on the returned Vector3.
    /// </summary>
    public static class TransformExtensions
    {
        public static Vector3 SetValues(this Vector3 vector3,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            if (!float.IsNaN(x))
                vector3.x = x;

            if (!float.IsNaN(y))
                vector3.y = y;

            if (!float.IsNaN(z))
                vector3.z = z;

            return vector3;
        }

        public static Vector3 AddValues(this Vector3 vector3,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            if (!float.IsNaN(x))
                vector3.x += x;

            if (!float.IsNaN(y))
                vector3.y += y;

            if (!float.IsNaN(z))
                vector3.z += z;

            return vector3;
        }

        public static void SetEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.eulerAngles = transform.eulerAngles.SetValues(x, y, z);
        }

        public static void AddEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.eulerAngles = transform.eulerAngles.AddValues(x, y, z);
        }

        public static void SetLocalEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localEulerAngles = transform.localEulerAngles.SetValues(x, y, z);
        }

        public static void AddToLocalEulerAngles(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localEulerAngles = transform.localEulerAngles.AddValues(x, y, z);
        }

        public static void SetLocalPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localPosition = transform.localPosition.SetValues(x, y, z);
        }

        public static void AddToLocalPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localPosition = transform.localPosition.AddValues(x, y, z);
        }

        public static void SetLocalScale(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localScale = transform.localScale.SetValues(x, y, z);
        }

        public static void AddToLocalScale(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localScale = transform.localScale.AddValues(x, y, z);
        }

        public static void SetPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.position = transform.position.SetValues(x, y, z);
        }

        public static void AddToPosition(this Transform transform,
            float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.position = transform.position.AddValues(x, y, z);
        }
    }
}