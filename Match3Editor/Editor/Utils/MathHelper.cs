using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Editor.Utils
{
  public class MathHelper
  {
    /// <summary>
    ///   <para>Clamps value between 0 and 1 and returns value.</para>
    /// </summary>
    /// <param name="value"></param>
    public static double Clamp01(double value)
    {
      if ((double)value < 0.0)
        return 0.0f;
      if ((double)value > 1.0)
        return 1f;
      return value;
    }

    /// <summary>
    ///   <para>Linearly interpolates between a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static double Lerp(double a, double b, double t)
    {
      return a + (b - a) * Clamp01(t);
    }

    /// <summary>
    ///   <para>Linearly interpolates between a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static double LerpUnclamped(double a, double b, double t)
    {
      return a + (b - a) * t;
    }
  }
}
