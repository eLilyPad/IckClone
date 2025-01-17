using Godot;


namespace Game;

using Extensions;

public sealed partial class PlayerBody3D : CharacterBody3D
{
	public Camera3D Camera { get; } = new();

	public override void _Ready()
	{
		Name = "Player";
		MeshInstance3D mesh = new() { Mesh = new CapsuleMesh { } };
		CollisionShape3D collider = new() { Shape = new CapsuleShape3D { } };
		Camera.Position = Position with
		{
			Y = Position.Y + 10,
			Z = Position.X + 5
		};

		this.Add(Camera, collider, mesh);
	}
	public void Move(Vector2 direction)
	{
		float speed = 10;
		Velocity = new(direction.X * speed, 0, direction.Y * speed);
		MoveAndSlide();
	}
}
