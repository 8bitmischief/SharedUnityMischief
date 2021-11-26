using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedUnityMischief
{
	// From https://github.com/upscalebaby/generic-serializable-dictionary
	[Serializable]
	public class GenericDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		[SerializeField] private List<KeyValuePair> _list = new List<KeyValuePair>();
		[SerializeField] private Dictionary<TKey, int> _indexByKey = new Dictionary<TKey, int>();
		[SerializeField, HideInInspector] private Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();
		#pragma warning disable 0414
		[SerializeField, HideInInspector] private bool _keyCollision = false;
		#pragma warning restore 0414

		public int Count => _dict.Count;
		public bool IsReadOnly { get; set; } = false;

		public void OnBeforeSerialize() {}

		public void OnAfterDeserialize()
		{
			_dict.Clear();
			_indexByKey.Clear();
			_keyCollision = false;
			for (int i = 0; i < _list.Count; i++)
			{
				var key = _list[i].Key;
				if (key != null && !ContainsKey(key))
				{
					_dict.Add(key, _list[i].Value);
					_indexByKey.Add(key, i);
				}
				else
				{
					_keyCollision = true;
				}
			}
		}

		// IDictionary
		public TValue this[TKey key]
		{
			get => _dict[key];
			set
			{
				_dict[key] = value;
				if (_indexByKey.ContainsKey(key))
				{
					var index = _indexByKey[key];
					_list[index] = new KeyValuePair(key, value);
				}
				else
				{
					_list.Add(new KeyValuePair(key, value));
					_indexByKey.Add(key, _list.Count - 1);
				}
			}
		}

		public ICollection<TKey> Keys => _dict.Keys;
		public ICollection<TValue> Values => _dict.Values;

		public void Add(TKey key, TValue value)
		{
			_dict.Add(key, value);
			_list.Add(new KeyValuePair(key, value));
			_indexByKey.Add(key, _list.Count - 1);
		}

		public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

		public bool Remove(TKey key) 
		{
			if (_dict.Remove(key))
			{
				var index = _indexByKey[key];
				_list.RemoveAt(index);
				UpdateIndexes(index);
				_indexByKey.Remove(key);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetValue(TKey key, out TValue value) => _dict.TryGetValue(key, out value);

		public void Add(KeyValuePair<TKey, TValue> pair) => Add(pair.Key, pair.Value);

		public void Clear()
		{
			_dict.Clear();
			_list.Clear();
			_indexByKey.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> pair)
		{
			TValue value;
			if (_dict.TryGetValue(pair.Key, out value))
			{
				return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
			}
			else
			{
				return false;
			}
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentException("The array cannot be null.");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
			}
			if (array.Length - arrayIndex < _dict.Count)
			{
				throw new ArgumentException("The destination array has fewer elements than the collection.");
			}

			foreach (var pair in _dict)
			{
				array[arrayIndex] = pair;
				arrayIndex++;
			}
		}

		public bool Remove(KeyValuePair<TKey, TValue> pair)
		{
			TValue value;
			if (_dict.TryGetValue(pair.Key, out value))
			{
				bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
				if (valueMatch)
				{
					return Remove(pair.Key);
				}
			}
			return false;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

		private void UpdateIndexes(int removedIndex)
		{
			for (int i = removedIndex; i < _list.Count; i++)
			{
				var key = _list[i].Key;
				_indexByKey[key]--;
			}
		}

		[Serializable]
		private struct KeyValuePair
		{
			public TKey Key;
			public TValue Value;

			public KeyValuePair(TKey Key, TValue Value)
			{
				this.Key = Key;
				this.Value = Value;
			}
		}
	}
}