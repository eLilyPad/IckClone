using Godot;

namespace Game.Extensions;

//	Keys
public static partial class Godot
{
	public static string DefaultInput(this MouseButton key) => key switch
	{
		_ => "None"
	};
	public static string DefaultInput(this Key key) => key switch
	{
		Key.A => "PlayerLeft",
		Key.D => "PlayerRight",
		Key.W => "PlayerUp",
		Key.S => "PlayerDown",
		Key.Escape => "Pause",
		Key.F11 => "FullScreen",
		_ => "None"
	};
	public static Vector2 Direction(this (Key Up, Key Down, Key Left, Key Right) movementKeys)
	{
		return Input.GetVector(
			movementKeys.Left.DefaultInput(),
			movementKeys.Right.DefaultInput(),
			movementKeys.Up.DefaultInput(),
			movementKeys.Down.DefaultInput()
		);
	}
	public static InputEventKey AsEvent(this Key key) => new()
	{
		ResourceName = key.DefaultInput(),
		PhysicalKeycode = key
	};
	public static Action FirstPressed(
		this Dictionary<Key, Action> triggers,
		InputEventKey input,
		Action defaultValue
	)
	{
		return triggers.Keys.FirstPressedKey(input) switch
		{
			Key key => triggers.GetValueOrDefault(key, defaultValue),
			_ => defaultValue,
		};
	}
	public static Key? FirstPressedKey(this IEnumerable<Key> triggers, InputEventKey input)
	{
		return triggers
			.Cast<Key?>()
			.FirstOrDefault(predicate: Pressed, null);

		bool Pressed(Key? t) => input.IsActionPressed(t?.DefaultInput() ?? "");
	}
}
// 	CharacterBody3D
public static partial class Godot
{
	//public static void T 
}
// 	Node
public static partial class Godot
{
	public static T Add<T>(this T parent, params ReadOnlySpan<Node> children)
	where T : Node
	{
		foreach (Node node in children)
		{
			parent.AddChild(node);
		}
		return parent;
	}
}