using System.Collections;
using System.Collections.Generic;

namespace Orpius.Platform.Collections
{
	class TrackedDictionary : IDictionary<string, string>
	{
		readonly IDictionary<string, string> inner;

		public TrackedDictionary(IDictionary<string, string>? inner = null)
		{
			this.inner = inner ?? new Dictionary<string, string>();
		}

		/// <summary>
		///     True if the dictionary has been modified since the last call to ResetVersion().
		/// </summary>
		public bool Dirty { get; private set; }

		/// <summary>
		///     Call this after you detect/handle the change. Resets the dirty flag.
		/// </summary>
		public void ResetDirty()
		{
			Dirty = false;
		}

		// Whenever someone writes to the dictionary, flip IsDirty = true
		void MarkDirty()
		{
			Dirty = true;
		}

		#region IDictionary implementation (write operations mark dirty)

		public string this[string key]
		{
			get => inner[key];
			set
			{
				// Only mark dirty if this is truly a *change* (optional check)
				if (!inner.TryGetValue(key, out string? oldValue) || oldValue != value)
				{
					inner[key] = value;
					MarkDirty();
				}
			}
		}

		public ICollection<string> Keys       => inner.Keys;
		public ICollection<string> Values     => inner.Values;
		public int                 Count      => inner.Count;
		public bool                IsReadOnly => false;

		public void Add(string key, string value)
		{
			inner.Add(key, value);
			MarkDirty();
		}

		public bool Remove(string key)
		{
			bool removed = inner.Remove(key);
			if (removed)
			{
				MarkDirty();
			}

			return removed;
		}

		public bool ContainsKey(string key)
		{
			return inner.ContainsKey(key);
		}

		public bool TryGetValue(string key, out string value)
		{
			return inner.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<string, string> item)
		{
			((IDictionary<string, string>)inner).Add(item);
			MarkDirty();
		}

		public void Clear()
		{
			if (inner.Count > 0)
			{
				inner.Clear();
				MarkDirty();
			}
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			return ((IDictionary<string, string>)inner).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			((IDictionary<string, string>)inner).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, string> item)
		{
			bool removed = ((IDictionary<string, string>)inner).Remove(item);
			if (removed)
			{
				MarkDirty();
			}

			return removed;
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return inner.GetEnumerator();
		}

		public void AddOrUpdate(string key, string value)
		{
			if (inner.TryGetValue(key, out string? oldValue))
			{
				if (oldValue != value)
				{
					inner[key] = value;
					MarkDirty();
				}
			}
			else
			{
				inner[key] = value;
				MarkDirty();
			}
		}

		#endregion
	}
}