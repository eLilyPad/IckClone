using Godot;

namespace Game;

using Extensions;

public partial class Windows : Resource, IHaveParentNode
{
	public enum Names { DialogueWindow, Default }
	public Node? Parent { private get; set; }
	public Node Current => Selected switch
	{
		Names.DialogueWindow => _windows.Dialogue,
		_ => _windows.Default
	};

	[Export(PropertyHint.Enum)]
	public Names Selected
	{
		get => field;
		set
		{
			var previous = Current;
			field = value;
			Parent?.ReplaceChild(currentNode: Current, previousNode: previous);
		}
	}

	private (Node Dialogue, Node Default) _windows = (new DialogueWindow(), new Control());
}
