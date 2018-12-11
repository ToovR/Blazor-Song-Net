using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Song.Net.Client.Shared
{
    public static class ObservableListExtensions
    {
        public static bool Exists<T>(this ObservableList<T> list, Predicate<T> match)
        {
            bool bReturn = false;

            Parallel.ForEach(list, item =>
            {
                if (match(item))
                {
                    bReturn = true;
                    return;
                }
            });

            return bReturn;
        }
    }

    public class ObservableList<T> : IList<T>, INotifyCollectionChanged
    {
        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void NotifyCollectionChanged(NotifyCollectionChangedAction pAction, object pItem)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(pAction, pItem));
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction pAction, object pItem, int piIndex)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(pAction, pItem, piIndex));
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction pAction, IList<T> pList)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(pAction, pList));
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction pAction)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(pAction));
        }

        #endregion INotifyCollectionChanged

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _List)
            {
                if (item == null)
                    break;

                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable

        #region Class Declarations

        private List<T> _List = new List<T>();

        #endregion Class Declarations



        #region IList Members

        public int Count
        {
            get { return _List.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get
            {
                if (index > -1 && _List.Count > 0)
                    return _List.ElementAt(index);
                else
                    return default;
            }
            set
            {
                if (index > -1)
                {
                    if (index > _List.Count)
                    {
                        _List.Add(value);
                        NotifyCollectionChanged(NotifyCollectionChangedAction.Add, value);
                    }
                    else
                    {
                        _List[index] = value;
                        NotifyCollectionChanged(NotifyCollectionChangedAction.Replace, value, index);
                    }
                }
            }
        }

        public void Add(T item)
        {
            if (item != null)
            {
                _List.Add(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
        }

        public void Clear()
        {
            _List.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(T item)
        {
            return _List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _List.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _List.Insert(index, item);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public bool Remove(T item)
        {
            bool bReturn = false;

            if (item != null)
            {
                int index = _List.IndexOf(item);

                if (index > -1)
                {
                    bReturn = _List.Remove(item);

                    if (bReturn)
                        NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
                }
            }

            return bReturn;
        }

        public void RemoveAt(int index)
        {
            if (index > -1)
            {
                _List.RemoveAt(index);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, _List[index], index);
            }
        }

        #endregion IList Members
    }
}