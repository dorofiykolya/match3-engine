using System;

namespace Match3.Engine.Utils
{
  public class EngineRandom
  {
    private const int MBIG = Int32.MaxValue;
    private const int MSEED = 161803398;
    private const int MZ = 0;

    private int _inext;
    private int _inextp;
    private readonly int[] _seedArray = new int[56];

    public EngineRandom(int seed)
    {
      var mj = MSEED - Math.Abs(seed);
      _seedArray[55] = mj;
      var mk = 1;
      for (int i = 1; i < 55; i++)
      {
        var ii = (21 * i) % 55;
        _seedArray[ii] = mk;
        mk = mj - mk;
        if (mk < 0) mk += MBIG;
        mj = _seedArray[ii];
      }
      for (int k = 1; k < 5; k++)
      {
        for (int i = 1; i < 56; i++)
        {
          _seedArray[i] -= _seedArray[1 + (i + 30) % 55];
          if (_seedArray[i] < 0) _seedArray[i] += MBIG;
        }
      }
      _inext = 0;
      _inextp = 21;
    }

    /// <returns>Return a new random number [0..1) and reSeed the Seed array.</returns>
    protected virtual double Sample()
    {
      return (InternalSample() * (1.0 / MBIG));
    }

    private int InternalSample()
    {
      int locINext = _inext;
      int locINextp = _inextp;

      if (++locINext >= 56) locINext = 1;
      if (++locINextp >= 56) locINextp = 1;

      var retVal = _seedArray[locINext] - _seedArray[locINextp];

      if (retVal < 0) retVal += MBIG;

      _seedArray[locINext] = retVal;

      _inext = locINext;
      _inextp = locINextp;

      return retVal;
    }

    /// <returns>[0..Int32.MaxValue)</returns>
    public virtual int Next()
    {
      return InternalSample();
    }

    private double GetSampleForLargeRange()
    {
      int result = InternalSample();
      bool negative = (InternalSample() % 2 == 0);
      if (negative)
      {
        result = -result;
      }
      double d = result;
      d += Int32.MaxValue - 1;
      d /= 2 * (uint)Int32.MaxValue - 1;
      return d;
    }

    /// <param name="minValue">the least legal value for the Random number</param>
    /// <param name="maxValue">One greater than the greatest legal return value</param>
    /// <returns>[minvalue..maxvalue) </returns>
    public virtual int Next(int minValue, int maxValue)
    {
      if (minValue > maxValue)
      {
        throw new ArgumentOutOfRangeException("minValue > maxValue");
      }

      long range = (long)maxValue - minValue;
      if (range <= (long)Int32.MaxValue)
      {
        return ((int)(Sample() * range) + minValue);
      }
      else
      {
        return (int)((long)(GetSampleForLargeRange() * range) + minValue);
      }
    }

    /// <param name="maxValue">One more than the greatest legal return value</param>
    /// <returns>int [0..maxValue)</returns>
    public virtual int Next(int maxValue)
    {
      if (maxValue < 0)
      {
        throw new ArgumentOutOfRangeException("maxValue < 0");
      }
      return (int)(Sample() * maxValue);
    }

    /// <returns>double [0..1)</returns>
    public virtual double NextDouble()
    {
      return Sample();
    }
  }
}

