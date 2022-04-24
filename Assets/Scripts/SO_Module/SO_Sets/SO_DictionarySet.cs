using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_DictionarySet<TKey, TValue> : SO_Base {
    #region Events
    [NonSerialized] public Action<TKey, TValue> OnCollectionChanged;
    [NonSerialized] public Action<TKey, TValue> OnItemChanged;
    [NonSerialized] public Action<TKey, TValue> OnAddNewItem;
    [NonSerialized] public Action<TKey> OnItemRemoved;
    #endregion

    public Dictionary<TKey, TValue> Items { get; private set; } = new Dictionary<TKey, TValue>();
    public bool IsRuntimeOnly = false;

    protected virtual void OnEnable() {
        if (IsRuntimeOnly) { 
            Items.Clear();
        }
    }

    public virtual void Add(TKey _key, TValue _value) {
        if (Items.ContainsKey(_key) && !Items.ContainsValue(_value)) {
            Items[_key] = _value;
            OnItemChanged?.Invoke(_key, _value);
            OnCollectionChanged?.Invoke(_key, _value);
        } else if (!Items.ContainsKey(_key)) {
            Items.Add(_key, _value);
            OnAddNewItem?.Invoke(_key, _value);
            OnCollectionChanged?.Invoke(_key, _value);
        } else {
            Debug.LogWarning("[SO_DictionarySet] Key Value pair already exists!");
        }
    }

    public virtual void Remove(TKey _key) {
        if (Items.TryGetValue(_key, out TValue _value) && Items.Remove(_key)) {
            OnItemRemoved?.Invoke(_key);
            OnCollectionChanged?.Invoke(_key, _value);
        }
    }

    public virtual void Clear() {
        if (Items.Count > 0) {
            foreach (TKey _key in Items.Keys) {
                Remove(_key);
            }
        }
    }
}
