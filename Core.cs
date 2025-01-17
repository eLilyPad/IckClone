using Godot;


namespace Game;

using Extensions;
using MovementKeys = (Key Up, Key Down, Key Left, Key Right);

public sealed partial class Core : Node
{
	readonly PlayerBody3D _player = new();
	readonly Level3D _level = new();
	private readonly Dictionary<Key, Action> _triggers = [];
	private readonly Dictionary<MovementKeys, Action<Vector2>> _movements = [];

	public Core()
	{
		Name = "Pickim Game Clone";
	}
	public override void _Ready()
	{
		this.Add(
			_level.Add(ReadyIckmanShip()),
			_player
		);

		_player.Camera.LookAt(_player.Position);
		_movements[(Key.W, Key.S, Key.A, Key.D)] = _player.Move;

		CharacterBody3D ReadyIckman()
		{
			return new CharacterBody3D { }.Add(
				new CollisionShape3D { Shape = new SphereShape3D { } },
				new MeshInstance3D
				{
					Mesh = new SphereMesh
					{
						Material = new StandardMaterial3D { AlbedoColor = Color.Color8(200, 200, 200) },
					}
				}
			);
		}
		StaticBody3D ReadyIckmanShip()
		{
			float midPoint = _level.GridScale * _level.GridScale / 2;
			(StaticBody3D Body, MeshInstance3D Mesh, CollisionShape3D Collider) ship = (
				new() { Position = new(midPoint, 2, midPoint) },
				new() { Mesh = new BoxMesh { } },
				new() { Shape = new BoxShape3D { } }
			);
			(Area3D Zone, MeshInstance3D Mesh, CollisionShape3D Collider) area = (
				new() { Monitoring = true, },
				new()
				{
					Mesh = new SphereMesh
					{
						Material = new StandardMaterial3D
						{
							AlbedoColor = new(.7f, .5f, .4f, .3f),
							Transparency = BaseMaterial3D.TransparencyEnum.Alpha
						}
					}
				},
				new() { Shape = new SphereShape3D { } }
			);
			area.Zone.Scale *= 10;
			ship.Body.Scale *= 2;

			ship.Body.Add(
				ship.Mesh,
				ship.Collider,
				area.Zone.Add(area.Mesh, area.Collider)
			);

			return ship.Body;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		foreach ((MovementKeys keys, Action<Vector2> movement) in _movements)
		{
			movement(keys.Direction());
		}
	}
	public override void _Input(InputEvent input)
	{
		switch (input)
		{
			case InputEventMouseButton { ButtonIndex: MouseButton.WheelUp } _:
				_player.Camera.Position = _player.Camera.Position with { Y = _player.Camera.Position.Y + 2 };
				_player.Camera.LookAt(_player.Position);
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.WheelDown } _:
				_player.Camera.Position = _player.Camera.Position with { Y = _player.Camera.Position.Y - 2 };
				_player.Camera.LookAt(_player.Position);
				break;
			case InputEventKey key:
				_triggers.FirstPressed(input: key, defaultValue: UnAssignedTrigger)();
				break;
		}

		static void UnAssignedTrigger() { }
	}
}
