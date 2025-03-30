using Godot;

namespace Game;

using Extensions;

[GlobalClass]
public sealed partial class Stages : Resource, IHaveParentNode
{
	public static void Initialize(ref Stages current, Stages next, Node parent)
	{
		current.Parent = null;
		next.Parent = parent;
		current = next;
	}

	public enum Names { Custom, Grid }

	public Node? Parent { private get; set; }
	public Stage3D Current => Selected switch
	{
		Names.Custom => _stages.Custom,
		Names.Grid => _stages.Grid,
		_ => _stages.Grid
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
	[Export]
	private PackedScene? CustomScene
	{
		get => field;
		set
		{
			if (value is null) { return; }
			if (!value.CanInstantiate()) { return; }
			if (value.Instantiate() is CustomStage3D stage)
			{
				_stages.Custom = stage;
				field = value;
			}
		}
	}
	private (Stage3D Custom, Stage3D Grid) _stages = (new CustomStage3D(), new GridStage3D());
}
