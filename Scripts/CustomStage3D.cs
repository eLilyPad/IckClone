using Godot;

namespace Game;

public sealed partial class CustomStage3D : Stage3D
{
	protected override Vector3 PlayerSpawnPosition => PlayerSpawnTransform?.Position
		?? throw new ArgumentException("Player Spawn is not set");
	protected override Vector3 ShipSpawnPosition => ShipSpawnTransform?.Position
		?? throw new ArgumentException("Ship Spawn is not set");

	[Export] private Node3D? PlayerSpawnTransform;
	[Export] private Node3D? ShipSpawnTransform;
}
