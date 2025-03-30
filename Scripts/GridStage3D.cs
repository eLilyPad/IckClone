using Godot;

namespace Game;

using Extensions;
public sealed partial class GridStage3D : Stage3D
{
	public Vector3 MiddlePosition => new(GridScale * GridScale / 2, 0, GridScale * GridScale / 2);
	public int GridScale { get; } = 10;

	protected override Vector3 ShipSpawnPosition => MiddlePosition with { Y = ShipSpawnHeight };
	protected override Vector3 PlayerSpawnPosition => MiddlePosition with
	{
		X = MiddlePosition.X - 20,
		Z = MiddlePosition.Z - 20
	};

	private readonly Dictionary<Vector2, StaticBody3D> _bodies = [];

	public override void _Ready()
	{
		base._Ready();
		for (int x = 0; x < GridScale; x++)
		{
			for (int z = 0; z < GridScale; z++)
			{
				Vector2 position = new(x, z);
				var floor = _bodies[position] = PlaneFloor(gridPosition: position, gridScale: GridScale);
				AddChild(floor);
			}
		}
	}

	private static StaticBody3D PlaneFloor(Vector2 gridPosition, in float gridScale)
	{
		var (x, z) = gridPosition;
		MeshInstance3D mesh = new()
		{
			Mesh = new QuadMesh
			{
				Size = new(gridScale, gridScale),
				Orientation = PlaneMesh.OrientationEnum.Y
			},
			MaterialOverride = new StandardMaterial3D
			{
				AlbedoColor = Color.Color8(
					r8: (byte)(x * gridScale),
					g8: (byte)(x + z),
					b8: (byte)(z * gridScale)
				)
			}
		};
		CollisionShape3D collider = new()
		{
			Shape = new BoxShape3D
			{
				Size = new(gridScale, 0.1f, gridScale)
			}
		};

		return new StaticBody3D
		{
			Name = $"Floor: ({gridPosition})",
			Position = new Vector3(x, 0, z) * gridScale
		}.Add(mesh, collider); ;
	}
}