
using System;
using System.Collections.Generic;
using System.Text;

namespace Lufi
{
    public static class Utililties
    {
        public static int ConcurrentIncrement(ref int val) { 
          object _sync = new object();
          lock (_sync)
          {
              return val = val + 1;
          }
        }
        public static int ConcurrentDecrement(ref int val)
        {
            object _sync = new object();
            lock (_sync)
            {
                return val = val - 1;
            }
        }
    }
    public class ConcurrentQueue<T>
    {
        private List<T> _list = new List<T>();
        private object _sync = new object();
        public void Push(T value)
        {
            lock (_sync)
            {
                _list.Add(value);
            }
        }
        public T Find(Predicate<T> predicate)
        {
            lock (_sync)
            {
                return _list.Find(predicate);
            }
        }
        public T Pop()
        {
            lock (_sync)
            {
                if (_list.Count>0)
                {
                    var lastItem = _list[_list.Count - 1];
                    _list.RemoveAt(_list.Count - 1);
                    return lastItem;    
                }
                return default(T);
                
            }
        }

        public bool Any()
        {

            lock (_sync)
            {
                return _list.Count > 0 ? true : false;
            }
        }
    }

 
}
