using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SO_Set<T> : SO_Base
{
    [NonSerialized] public Action OnSetChanged;
    [NonSerialized] public Action<T> OnItemAdded;
    [NonSerialized] public Action<T> OnItemRemoved;
    public List<T> Items => items;
    private List<T> items = new List<T>();
    public int Count => items.Count;
    public bool RuntimeOnly = false;

    protected virtual void OnEnable()
    {
        if (RuntimeOnly)
        {
            items.Clear();
        }
    }

    public virtual void Add(T _item)
    {
        if (!items.Contains(_item))
        {
            items.Add(_item);
            OnSetChanged?.Invoke();
            OnItemAdded?.Invoke(_item);
        }
    }

    public virtual void AddRange(IEnumerable<T> _items)
    {
        IEnumerable<T> nonDuplicates = _items.Where(i=>!items.Contains(i));
        items.AddRange(nonDuplicates);
        // TODO: Handle individual OnItemAdded or change to OnItemsAdded
        OnSetChanged?.Invoke();
    }

    public virtual void Remove(T _item)
    {
        if (items.Remove(_item))
        {
            OnSetChanged?.Invoke();
            OnItemRemoved?.Invoke(_item);
        }
    }

    /// <summary>
    /// empties the item list and calls Remove and SetChanged
    /// </summary>
    public virtual void Clear()
    {
        if(items.Count > 0)
        {
            for(int i = items.Count - 1; i >= 0; i--)
            {
                if(items[i] == null)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    Remove(items[i]);
                }
            }
            OnSetChanged?.Invoke();
        }
    }
}
