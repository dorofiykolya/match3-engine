using System;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Engine.Utils
{
  public class UnsignedIntDictionary<T> : IEnumerable<KeyValuePair<int, T>>
  {
    private static readonly int[] _primeSizes = { 89, 179, 359, 719, 1439, 2879, 5779, 11579, 23159, 46327,
                                        92657, 185323, 370661, 741337, 1482707, 2965421, 5930887, 11861791,
                                        23723599};
    private T[] _values;
    private bool[] _exist;
    private int _count;
    private ValueCollection _valueCollection;

    public UnsignedIntDictionary(int capacity = 0)
    {
      if(!(capacity >= 0)) throw new ArgumentException("The capacity can not be negative");
      _values = new T[capacity];
      _exist = new bool[capacity];
    }

    public T this[int id]
    {
      get { return _values[id]; }
      set
      {
        if (id >= _values.Length)
        {
          var newSize = FindNewSize(id);
          Array.Resize(ref _values, newSize);
          Array.Resize(ref _exist, newSize);
        }
        if (!_exist[id])
        {
          _count++;
        }
        _values[id] = value;
        _exist[id] = true;
      }
    }

    public bool TryGetValue(int id, out T value)
    {
      if (id >= 0 && id < _exist.Length && _exist[id])
      {
        value = _values[id];
        return true;
      }
      value = default(T);
      return false;
    }


    public bool ContainsKey(int id)
    {
      if (id >= 0 && id < _exist.Length && _exist[id])
      {
        return true;
      }
      return false;
    }

    public int Capacity
    {
      get { return _exist.Length; }
      set
      {
        if (value < 0) { throw new ArgumentException("value < 0"); }
        if (value < _values.Length)
        {
          Array.Clear(_values, value, _values.Length - value);
          Array.Clear(_exist, value, _exist.Length - value);
        }
        Array.Resize(ref _values, value);
        Array.Resize(ref _exist, value);
      }
    }

    public int ExpandCapacity(int newCapacity)
    {
      if (Capacity < newCapacity)
      {
        Capacity = newCapacity;
      }
      return Capacity;
    }

    public int Count { get { return _count; } }

    public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
    {
      return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Clear()
    {
      _count = 0;
      Array.Clear(_values, 0, _values.Length);
      Array.Clear(_exist, 0, _exist.Length);
    }

    public ValueCollection Values
    {
      get
      {
        if (_valueCollection == null) _valueCollection = new ValueCollection(this);
        return _valueCollection;
      }
    }

    private int FindNewSize(int id)
    {
      for (int i = 0; i < _primeSizes.Length; i++)
      {
        if (_primeSizes[i] >= id)
          return _primeSizes[i];
      }

      throw new NotImplementedException("Too large array");
    }

    public class ValueCollection : IEnumerable<T>
    {
      private readonly UnsignedIntDictionary<T> _dictionary;

      public ValueCollection(UnsignedIntDictionary<T> dictionary)
      {
        _dictionary = dictionary;
      }

      public int Count
      {
        get { return _dictionary.Count; }
      }

      public IEnumerator<T> GetEnumerator()
      {
        return new Enumerator(_dictionary);
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return GetEnumerator();
      }

      public struct Enumerator : IEnumerator<T>
      {
        private UnsignedIntDictionary<T> _dictionary;
        private int _index;

        internal Enumerator(UnsignedIntDictionary<T> dictionary)
        {
          _dictionary = dictionary;
          _index = 0;
        }

        public bool MoveNext()
        {
          var len = _dictionary._exist.Length;
          while (_index < len)
          {
            if (_dictionary._exist[_index])
            {
              _index++;
              return true;
            }
            _index++;
          }
          return false;
        }

        public void Reset()
        {
          _index = 0;
        }

        public T Current
        {
          get { return _dictionary._values[_index - 1]; }
        }

        object IEnumerator.Current
        {
          get { return Current; }
        }

        public void Dispose()
        {
          _dictionary = null;
        }
      }
    }

    public struct Enumerator : IEnumerator<KeyValuePair<int, T>>
    {
      private readonly UnsignedIntDictionary<T> _dictionary;
      private int _index;

      internal Enumerator(UnsignedIntDictionary<T> dictionary)
      {
        _dictionary = dictionary;
        _index = 0;
      }

      public bool MoveNext()
      {
        var len = _dictionary._exist.Length;
        while (_index < len)
        {
          if (_dictionary._exist[_index])
          {
            _index++;
            return true;
          }
          _index++;
        }
        return false;
      }

      public void Reset()
      {
        _index = 0;
      }

      public KeyValuePair<int, T> Current
      {
        get { return new KeyValuePair<int, T>(_index - 1, _dictionary._values[_index - 1]); }
      }

      object IEnumerator.Current
      {
        get { return Current; }
      }

      public void Dispose()
      {

      }
    }
  }
}
