using Godot;

namespace Game;

using Extensions;

public sealed partial class MeshZone3D : Area3D
{
	public static MeshZone3D ColoredSphere(Color color)
	{
		MeshZone3D zone = new()
		{
			Monitoring = true,
			Mesh = new()
			{
				Mesh = color.TransparentMesh<SphereMesh>()
			},
			Collider = new() { Shape = new SphereShape3D { } }
		};
		zone.Scale *= 10;

		return zone;
	}

	public required MeshInstance3D Mesh { get; init; }
	public required CollisionShape3D Collider { get; init; }

	public override void _Ready()
	{
		this.Add(Mesh, Collider);
	}
}
