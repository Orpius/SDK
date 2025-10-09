using System;
using System.Collections.Generic;

namespace Orpius.Platform.Collections
{
	class MutableKeyIndex<TKey, TValue> where TKey : notnull
	{
		readonly object syncRoot = new object();
		readonly IDictionary<TKey, TValue> map;
		readonly Func<TValue, TKey> keySelector;
		readonly IEqualityComparer<TKey> comparer;

		public MutableKeyIndex(
			IDictionary<TKey, TValue> map,
			Func<TValue, TKey> keySelector,
			IEqualityComparer<TKey>? comparer = null)
		{
			this.map         = map         ?? throw new ArgumentNullException(nameof(map));
			this.keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
			this.comparer    = comparer    ?? EqualityComparer<TKey>.Default;
		}

		/// <summary>
		/// Gets the value for key if present;
		/// otherwise attempts a repair and returns the value.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException">
		/// Thrown if no matching value exists.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if two values claim the same key.</exception>
		public TValue GetOrRepair(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			lock (syncRoot)
			{
				if (map.TryGetValue(key, out TValue value))
				{
					return value;
				}

				/* Find any value whose *current* key matches the requested key. */
				if (!TryFindByCurrentKey(key, out TValue matching))
				{
					throw new ArgumentException($"No value found for key: {key}", nameof(key));
				}

				/* Locate the old dictionary key for this object instance. */
				if (TryFindStoredKeyForValue(matching, out TKey oldKey))
				{
					TKey newKey = keySelector(matching);

					if (!comparer.Equals(oldKey, newKey))
					{
						map.Remove(oldKey);

						/* Prevent two distinct values sharing the same new key. */
						if (map.TryGetValue(newKey, out TValue other) 
							&& !ReferenceEquals(other, matching))
						{
							throw new InvalidOperationException(
								$"Two values claim the same key: {newKey}");
						}

						map[newKey] = matching;
					}
					else
					{
						/* Keys match but the entry might be missing; ensure present. */
						map.TryAdd(newKey, matching);
					}
				}
				else
				{
					/* Value not currently indexed under any key; index it now. */
					TKey newKey = keySelector(matching);

					if (map.TryGetValue(newKey, out TValue other) 
						&& !ReferenceEquals(other, matching))
					{
						throw new InvalidOperationException(
							$"Two values claim the same key: {newKey}");
					}

					map[newKey] = matching;
				}

				return matching;
			}
		}

		/// <summary>
		/// Fast path that returns false instead of throwing when not found after repair.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns><c>true</c> if the value is returned; <c>false</c> otherwise.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public bool TryGetOrRepair(TKey key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			lock (syncRoot)
			{
				if (map.TryGetValue(key, out value))
				{
					return true;
				}

				if (!TryFindByCurrentKey(key, out TValue matching))
				{
					value = default!;
					return false;
				}

				/* Perform the same repair steps as GetOrRepair. */
				if (TryFindStoredKeyForValue(matching, out TKey oldKey))
				{
					TKey newKey = keySelector(matching);

					if (!comparer.Equals(oldKey, newKey))
					{
						map.Remove(oldKey);

						if (map.TryGetValue(newKey, out TValue other)
							&& !ReferenceEquals(other, matching))
						{
							throw new InvalidOperationException(
								$"Two values claim the same key: {newKey}");
						}

						map[newKey] = matching;
					}
					else
					{
						if (!map.ContainsKey(newKey))
						{
							map[newKey] = matching;
						}
					}
				}
				else
				{
					TKey newKey = keySelector(matching);

					if (map.TryGetValue(newKey, out TValue other) 
						&& !ReferenceEquals(other, matching))
					{
						throw new InvalidOperationException(
							$"Two values claim the same key: {newKey}");
					}

					map[newKey] = matching;
				}

				value = matching;
				return true;
			}
		}

		/// <summary>
		/// Add a value ensuring its current key is unique.
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public void AddOrReplaceByCurrentKey(TValue value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			lock (syncRoot)
			{
				TKey key = keySelector(value);

				if (map.TryGetValue(key, out TValue existing) 
					&& !ReferenceEquals(existing, value))
				{
					throw new ArgumentException(
						$"Duplicate key detected: {key}", nameof(value));
				}

				map[key] = value;
			}
		}

		bool TryFindByCurrentKey(TKey key, out TValue value)
		{
			foreach (TValue candidate in map.Values)
			{
				if (comparer.Equals(keySelector(candidate), key))
				{
					value = candidate;
					return true;
				}
			}

			value = default!;
			return false;
		}

		bool TryFindStoredKeyForValue(TValue value, out TKey storedKey)
		{
			foreach (KeyValuePair<TKey, TValue> pair in map)
			{
				if (ReferenceEquals(pair.Value, value))
				{
					storedKey = pair.Key;
					return true;
				}
			}

			storedKey = default!;
			return false;
		}
	}
}
