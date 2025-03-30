using Godot;

namespace Game;

using Extensions;

public sealed partial class PlayerBody3D : CharacterBody3D
{
	public (Key Up, Key Down, Key Left, Key Right) WalkKeys { get; } = (Key.W, Key.S, Key.A, Key.D);
	public Key InteractKey { get; } = Key.E;
	public Camera3D Camera { get; } = new();
	public MeshInstance3D Mesh { get; } = new() { Mesh = new CapsuleMesh { } };
	public CollisionShape3D Collider { get; } = new() { Shape = new CapsuleShape3D { } };

	public float WalkSpeed { get; set; } = 10;
	public Vector3 CameraOffset { get; } = new(0, 5, 5);

	public float MaxCameraHeight { get; init; } = 20;
	public float MinCameraHeight { get; init; } = 2;

	public Vector3 MousePositionFromPosition => Camera.TransformPositionFromMouse(Position, CameraOffset);

	public override void _Ready()
	{
		Name = "Player";

		this.Add(Camera, Collider, Mesh);

		Camera.Position = Camera.Position with
		{
			Y = Camera.Position.Y + CameraOffset.Y,
			Z = Camera.Position.Z + CameraOffset.Z
		};
		Camera.LookAt(Position);
	}
	public override void _Input(InputEvent input)
	{
		switch (input)
		{
			case InputEventMouseButton { ButtonIndex: MouseButton.WheelUp } _
			when Camera.Position.Y < MaxCameraHeight:
				Camera.Position = Camera.Position with { Y = Camera.Position.Y + 2 };
				Camera.LookAt(Position);
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.WheelDown } _
			when Camera.Position.Y > MinCameraHeight:
				Camera.Position = Camera.Position with { Y = Camera.Position.Y - 2 };
				Camera.LookAt(Position);
				break;
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = new(WalkKeys.Direction().X, 0, WalkKeys.Direction().Y);
		Velocity = direction * WalkSpeed;
		MoveAndSlide();
	}

}
