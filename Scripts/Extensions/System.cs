using System.Text;

namespace Game.Extensions;

public static class System
{
	#region Random

	public static bool NextBool(this Random random, float threshold = 0.5f)
	{
		return random.NextSingle() < threshold;
	}
	public static float Offset(this Random random, float a)
	{
		float offset = a / 2 * random.NextSingle();
		return random.NextBool() ? offset : -offset;
	}

	#endregion
	#region string

	public static string CondenseSpaces(this string s)
	{
		return s
			.Aggregate(seed: new StringBuilder(), func: AppendIfNotWhiteSpace)
			.ToString();

		static StringBuilder AppendIfNotWhiteSpace(StringBuilder acc, char c)
		{
			bool
			noWhiteSpace = c != ' ' || acc[^1] != ' ',
			emptyString = acc.Length == 0;
			return noWhiteSpace || emptyString ? acc.Append(value: c) : acc;
		}
	}
	public static object ParseObject(this string s)
	{
		return s switch
		{
			_ when int.TryParse(s, out int result) => result,
			_ when float.TryParse(s, out float result) => result,
			_ when char.TryParse(s, out char result) => result,
			_ when bool.TryParse(s, out bool result) => result,
			null => "",
			_ => s
		};
	}

	#endregion
	#region IDictionary

	public static IEnumerable<TValue> Select<TKey, TValue>(
		this IDictionary<TKey, List<TKey>> d,
		TKey key,
		Func<TKey, TValue> selector
	)
	{
		bool found = d.TryGetValue(key, out List<TKey>? l);
		if (!found || l is null)
		{
			return [];
		}
		return l.Select(selector);
	}

	public static void Add<TKey>(this IDictionary<TKey, List<TKey>> d, TKey key, TKey add)
	{
		if (d.TryGetValue(key, out List<TKey>? keys))
		{
			keys.Add(add);
		}
		else
		{
			d[key] = [add];
		}
	}

	public static bool Remove<TKey>(this IDictionary<TKey, List<TKey>> d, TKey key, TKey remove)
	{
		bool removedKey = false;
		if (d.TryGetValue(key, out List<TKey>? keys))
		{
			removedKey = keys.Remove(remove);
		}
		if (d.Count(key) == 0)
		{
			return d.Remove(key);
		}

		return removedKey;
	}

	public static int Count<TKey>(this IDictionary<TKey, List<TKey>> d, TKey key)
	{
		if (d.TryGetValue(key, out List<TKey>? values))
		{
			return values.Count;
		}
		return 0;
	}

	public static bool TryGetKey<TKey, TValue>(
		this Dictionary<TKey, TValue> d,
		TValue value,
		out TKey key
	)
	where TKey : struct
	where TValue : class
	{
		key = default;

		foreach (var a in d)
		{
			if (a.Value != value) { continue; }
			key = a.Key;
			return true;
		}
		return false;
	}

	#endregion

	public static IEnumerable<T?> TakeOrNull<T>(this IEnumerable<T> collection, int count)
	where T : class
	{
		return Range(0, count)
			.Select(i => i < collection.Count() ? collection.ElementAt(i) : null);
	}

	public static bool TryGetWithConditonMet<TKey, T>(this T list, out TKey value)
	where T : IEnumerable<(Func<bool> When, TKey To)>
	where TKey : struct
	{
		value = default;
		foreach (var (when, to) in list)
		{
			if (!when()) { continue; }
			value = to;
			return true;
		}
		return false;
	}
	public static bool TryGetWithConditonMet<TKey, T>(this IDictionary<TKey, T> d, TKey key, out TKey value)
	where T : IEnumerable<(Func<bool> When, TKey To)>
	where TKey : struct
	{
		value = default;
		if (d.TryGetValue(key, value: out var t))
		{
			return t.TryGetWithConditonMet(out value);
		}
		return false;
	}
}