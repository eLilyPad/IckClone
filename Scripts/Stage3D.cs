using Godot;

namespace Game;

using Extensions;

public abstract partial class Stage3D : Node3D
{
	public DirectionalLight3D Light { get; } = new()
	{
		Position = new(0, 10, 0),
		Rotation = new(-90, 0, 0)
	};

	public ShipBody3D Ship { get; } = new();
	[Export] protected float ShipSpawnHeight = 2;

	public override void _Ready()
	{
		Name = "Level";
		Ship.Position = ShipSpawnPosition;
		this.Add(Light, Ship);
	}

	protected abstract Vector3 PlayerSpawnPosition { get; }
	protected abstract Vector3 ShipSpawnPosition { get; }

	public virtual Vector3 IckSpawnJumpPosition => Ship.Position with
	{
		Z = Ship.Position.Z - 10
	};

	public virtual void SetSpawnPosition<T>(T node) where T : Node3D
	{
		node.Position = node switch
		{
			PlayerBody3D => PlayerSpawnPosition,
			ShipBody3D => ShipSpawnPosition,
			IckBody3D => Ship.PositionAbove,
			CounterFlowerBody3D => ShipSpawnPosition with { X = Ship.Position.X + 27 },
			IckCounterBody3D => ShipSpawnPosition with { X = Ship.Position.X + 25 },
			_ => node.Position
		};
	}
}
