using System.Text;

namespace Game.Extensions;

public static class System
{
	public static bool NextBool(this Random random, float threshold = 0.5f)
	{
		return random.NextSingle() > threshold;
	}
	public static float Offset(this Random random, float a)
	{
		float offset = a / 2 * random.NextSingle();
		return random.NextBool() ? offset : -offset;
	}
	public static float Offset(this Random random, float a, float b)
	{
		return (b * a) + random.Offset(a);
	}

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
	public static object ParseObject(this string str)
	{
		return str switch
		{
			_ when int.TryParse(str, out int result) => result,
			_ when float.TryParse(str, out float result) => result,
			_ when char.TryParse(str, out char result) => result,
			_ when bool.TryParse(str, out bool result) => result,
			null => "",
			_ => str
		};
	}
	public static IEnumerable<T?> TakeOrNull<T>(this IEnumerable<T> collection, int count) where T : class
	{
		return Enumerable
			.Range(start: 0, count)
			.Select(selector: (int i) => i < collection.Count() ? collection.ElementAt(i) : null);
	}
}