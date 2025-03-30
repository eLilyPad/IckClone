using Godot;

namespace Game;

using Extensions;

public sealed partial class ShipBody3D : StaticBody3D
{
	public static implicit operator Rid(ShipBody3D ickman) => ickman.GetRid();
	public MeshInstance3D Mesh { get; } = new() { Mesh = new BoxMesh { } };
	public CollisionShape3D Collider { get; } = new() { Shape = new BoxShape3D { } };
	public MeshZone3D Zone { get; } = MeshZone3D.ColoredSphere(color: new Color(.7f, .5f, .4f, .3f));

	public Vector3 PositionAbove => Position with { Y = Position.Y + (Scale.Y / 2) };

	public int IcksAboard { get; private set; } = 0;

	public override void _Ready()
	{
		Scale *= 2;
		this.Add(Mesh, Collider, Zone);
	}
}
