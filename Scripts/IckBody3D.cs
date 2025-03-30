using Godot;

namespace Game;

using Extensions;
public partial class IckBody3D : CharacterBody3D
{
	public static implicit operator Rid(IckBody3D ickman) => ickman.GetRid();
	public MeshInstance3D MeshInstance { get; } = new()
	{
		Mesh = new SphereMesh
		{
			Material = new StandardMaterial3D { AlbedoColor = Color.Color8(200, 200, 200) }
		}
	};
	public CollisionShape3D Collider { get; } = new() { Shape = new SphereShape3D { } };
	public MeshZone3D Zone { get; } = MeshZone3D.ColoredSphere(color: new Color(.7f, .1f, .1f, .3f));
	public IckBody3D()
	{
		Name = "Ickman";
		this.Add(Zone, Collider, MeshInstance);
	}
}
