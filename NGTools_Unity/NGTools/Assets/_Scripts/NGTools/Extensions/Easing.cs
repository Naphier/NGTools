using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NG.Easing
{
    public static class Easing
    {
        // Adapted from source : http://www.robertpenner.com/easing/
        // Reference curves: DocumentationResources/easing.png

        #region Basic Easing
        public static float Ease(double linearStep, float acceleration, EasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                              acceleration < 0 ? EaseOut(linearStep, type) :
                              (float)linearStep;

            return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
        }

        public static float EaseIn(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseIn(linearStep);
                case EasingType.Quadratic: return Power.EaseIn(linearStep, 2);
                case EasingType.Cubic: return Power.EaseIn(linearStep, 3);
                case EasingType.Quartic: return Power.EaseIn(linearStep, 4);
                case EasingType.Quintic: return Power.EaseIn(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseOut(linearStep);
                case EasingType.Quadratic: return Power.EaseOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }
        public static float EaseInOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseInOut(linearStep);
                case EasingType.Quadratic: return Power.EaseInOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseInOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseInOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseInOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        static class Sine
        {
            public static float EaseIn(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }
            public static float EaseOut(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi);
            }
            public static float EaseInOut(double s)
            {
                return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }

        static class Power
        {
            public static float EaseIn(double s, int power)
            {
                return (float)Math.Pow(s, power);
            }
            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }
            public static float EaseInOut(double s, int power)
            {
                s *= 2;
                if (s < 1) return EaseIn(s, power) / 2;
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
        #endregion

        #region Sigmoid
        public static float SigmoidEaseInOut(float k, float t)
        {
            float correction = 0.5f / SigmoidBase(k, 1);
            t = SigmoidClamp(t, 0, 1);
            return correction * SigmoidBase(k, (2 * t - 1)) + 0.5f;
        }

        public static float SigmoidEaseOut(float k, float t)
        {
            k = (k == 0) ? 1e-7f : k;

            t = SigmoidClamp(t, 0, 1);
            return SigmoidBase(k, t) / SigmoidBase(k, 1);
        }

        public static float SigmoidEaseIn(float k, float t)
        {
            k = (k == 0) ? 1e-7f : k;            
            t = SigmoidClamp(t, 0, 1);
            
            return SigmoidBase(k, t - 0.5f) / SigmoidBase(k, 1) + 1;
        }

        static float SigmoidBase(float k, float t)
        {
            return (1f / (1f + (float)Math.Exp(-k * t))) - 0.5f;
        }

        static float SigmoidClamp(float value, float lower, float upper)
        {
            return Math.Max(Math.Min(value, upper), lower);
        }
        #endregion

        #region Elastic
        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: 
        /// decelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseOut(float t, float b, float c, float d)
        {
            if ((t /= d) == 1)
                return b + c;

            float p = d * .3f;
            float s = p / 4.0f;

            return (c * (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: 
        /// accelerating from zero velocity.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseIn(float t, float b, float c, float d)
        {
            if ((t /= d) == 1)
                return b + c;

            float p = d * .3f;
            float s = p / 4;

            return -(c * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: 
        /// acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) == 2)
                return b + c;

            float p = d * (.3f * 1.5f);
            float s = p / 4;

            if (t < 1)
                return -.5f * (c * (float)Math.Pow(2, 10 * (t -= 1)) * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b;
            return c * (float)Math.Pow(2, -10 * (t -= 1)) * (float)Math.Sin((t * d - s) * (2 * Math.PI) / p) * .5f + c + b;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out/in: 
        /// deceleration until halfway, then acceleration.
        /// </summary>
        /// <param name="t">Current time in seconds.</param>
        /// <param name="b">Starting value.</param>
        /// <param name="c">Final value.</param>
        /// <param name="d">Duration of animation.</param>
        /// <returns>The correct value.</returns>
        public static float ElasticEaseOutIn(float t, float b, float c, float d)
        {
            if (t < d / 2)
                return ElasticEaseOut(t * 2, b, c / 2, d);
            return ElasticEaseIn((t * 2) - d, b + c / 2, c / 2, d);
        }
        #endregion

        public class Elastic
        {
            public static float In(float k)
            {
                if (k <= 0) return 0;
                if (k >= 1) return 1;
                return (float)(-Math.Pow(2f, 10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f));
            }

            public static float Out(float k)
            {
                if (k <= 0) return 0;
                if (k >= 1) return 1;
                return (float)(Math.Pow(2f, -10f * k) * Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f) + 1f);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f)
                    return -0.5f * (float)(Math.Pow(2f, 10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f));
                return (float)(Math.Pow(2f, -10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * Math.PI) / 0.4f) * 0.5f + 1f);
            }
        };
    }

    public enum EasingType
    {
        Step,
        Linear,
        Sine,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
    }

    public static class MathHelper
    {
        public const float Pi = (float)Math.PI;
        public const float HalfPi = (float)(Math.PI / 2);

        public static float Lerp(double from, double to, double step)
        {
            return (float)((to - from) * step + from);
        }
    }
}
