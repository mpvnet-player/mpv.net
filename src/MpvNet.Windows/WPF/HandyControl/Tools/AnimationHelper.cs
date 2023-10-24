
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

using HandyControl.Tools.Extension;

namespace HandyControl.Tools
{
    public class AnimationHelper
    {
        public static ThicknessAnimation CreateAnimation(Thickness thickness = default, double milliseconds = 200)
        {
            return new ThicknessAnimation(thickness, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }

        public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200)
        {
            return new DoubleAnimation(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }

        internal static void DecomposeGeometryStr(string geometryStr, out double[] arr)
        {
            var collection = Regex.Matches(geometryStr, RegexPatterns.DigitsPattern);
            arr = new double[collection.Count];
            for (var i = 0; i < collection.Count; i++)
            {
                arr[i] = collection[i].Value.Value<double>();
            }
        }

        internal static Geometry ComposeGeometry(string[] strings, double[] arr)
        {
            var builder = new StringBuilder(strings[0]);
            for (var i = 0; i < arr.Length; i++)
            {
                var s = strings[i + 1];
                var n = arr[i];
                if (!double.IsNaN(n))
                {
                    builder.Append(n).Append(s);
                }
            }

            return Geometry.Parse(builder.ToString());
        }

        internal static Geometry InterpolateGeometry(double[] from, double[] to, double progress, string[] strings)
        {
            var accumulated = new double[to.Length];
            for (var i = 0; i < to.Length; i++)
            {
                var fromValue = from[i];
                accumulated[i] = fromValue + (to[i] - fromValue) * progress;
            }

            return ComposeGeometry(strings, accumulated);
        }

        internal static double[] InterpolateGeometryValue(double[] from, double[] to, double progress)
        {
            var accumulated = new double[to.Length];
            for (var i = 0; i < to.Length; i++)
            {
                var fromValue = from[i];
                accumulated[i] = fromValue + (to[i] - fromValue) * progress;
            }

            return accumulated;
        }
    }
}
