using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageQuantization
{
     //this priority queue implemented with array which gets nodes and sort it according to each node frequency 
    public class PriorityQueue<T>
     {
          
          
               private IComparer<int> _comparer;
               private node[] _data;
               private int _count = 0;

               public PriorityQueue()
                    : this(11)
               {

               }

               public PriorityQueue(int initialCapacity, IComparer<int> comparer)
               {
                    if (initialCapacity < 0)
                    {
                         throw new ArgumentOutOfRangeException("initialCapacity");
                    }

                    if (comparer == null)
                    {
                         comparer = Comparer<int>.Default;
                    }

                    _data = new node[initialCapacity];

                    _comparer = comparer;
               }

               public PriorityQueue(int initialCapacity)
                    : this(initialCapacity, null)
               {

               }

               private void IncreaseCapacity()
               {
                    int size = (_count + 1) << 1;

                    node[] data = new node[size];

                    Array.Copy(_data, data, _data.Length);

                    _data = data;
               }

               public void Push(node item)
               {
                    if (_count == _data.Length)
                    {
                         IncreaseCapacity();
                    }

                    int index = _count;

                    _data[_count] = item;

                    _count += 1;

                    while (index > 0)
                    {
                         int parent = (index - 1) / 2;

                         if (_comparer.Compare(_data[index].freq, _data[parent].freq) >= 0)
                         {
                              break;
                         }

                         node element = _data[index];

                         _data[index] = _data[parent];
                         _data[parent] = element;

                         index = parent;
                    }
               }

               public void Clear()
               {
                    if (_count > 0)
                    {
                         Array.Clear(this._data, 0, _count);

                         _count = 0;
                    }
               }
               public bool empty()
               {
                    if (_count == 0)
                    {
                         return true;
                    }
                    else return false;
               }

               public int size()
               {


                    return _count;

               }
               public node top()
               {
                    return _data[0];

               }

               public node pop()
               {
                    if (_count <= 0)
                    {
                         throw new InvalidOperationException("Queue empty.");
                    }

                    _count -= 1;

                    node first = _data[0];
                    node second = _data[1];
                    _data[0] = _data[_count];

                    _data[_count] = default(node);

                    int index = 0;

                    while (true)
                    {
                         int left = (index << 1) + 1;

                         if (left >= _count)
                         {
                              return first;
                         }

                         int right = left + 1;

                         if (right < _count)
                         {
                              if (_comparer.Compare(_data[left].freq, _data[right].freq) > 0)
                              {
                                   left = right;
                              }
                         }

                         if (_comparer.Compare(_data[index].freq, _data[left].freq) <= 0)
                         {
                              return first;
                         }


                         node element = _data[index];

                         _data[index] = _data[left];
                         _data[left] = element;

                         index = left;
                    }
               }
          }
     }

