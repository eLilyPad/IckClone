using Godot;


namespace Game;

using Extensions;

public sealed partial class Level3D : Node3D
{
	public int GridScale { get; } = 10;

	public IImmutableDictionary<Vector2, StaticBody3D> Body => _bodies.ToImmutableDictionary();

	private readonly Dictionary<Vector2, StaticBody3D> _bodies = [];

	public override void _Ready()
	{
		Name = "Level";
		foreach (int x in Range(0, GridScale))
		{
			foreach (int z in Range(0, GridScale))
			{
				Color floorColor = Color.Color8(
					r8: (byte)(x * GridScale),
					g8: (byte)(x + z),
					b8: (byte)(z * GridScale)
				);
				Vector3 position = new Vector3(x, 0, z) * GridScale;
				_bodies[new(x, z)] = PlaneFloor(position, floorColor);
			}
		}
		this.Add([
			new DirectionalLight3D
			{
				Position = new(0, 10, 0),
				Rotation = new(-90, 0, 0)
			},
			.. _bodies.Values
		]);
	}

	StaticBody3D PlaneFloor(
		Vector3 position,
		Color floorColor
	)
	{
		StaticBody3D body = new StaticBody3D
		{
			Name = $"Floor ({position.X}, {position.Z})",
			Position = position
		}.Add(
			new MeshInstance3D
			{
				Mesh = new QuadMesh
				{
					Size = new(GridScale, GridScale),
					Orientation = PlaneMesh.OrientationEnum.Y,
					Material = new StandardMaterial3D
					{
						AlbedoColor = floorColor
					}
				}
			},
			new CollisionShape3D
			{
				Shape = new BoxShape3D { Size = new Vector3(GridScale, 0.1f, GridScale) }
			}
		);
		return body;
	}
}