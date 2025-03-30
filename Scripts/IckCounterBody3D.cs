using Godot;

namespace Game;

using Extensions;

public partial class IckCounterBody3D : RigidBody3D
{
	public static implicit operator Rid(IckCounterBody3D ickman) => ickman.GetRid();

	public int AmountNeededToCarry { get; init; } = 1;

	public override void _Ready()
	{
		Name = "Counter";
		this.Add(
			new CollisionShape3D
			{
				Shape = new CylinderShape3D { }
			},
			MeshZone3D.ColoredSphere(color: new(.7f, .1f, .1f, .3f)),
			new MeshInstance3D
			{
				Mesh = new CylinderMesh
				{
					Material = new StandardMaterial3D { AlbedoColor = Color.Color8(200, 200, 200) },
					Height = 1,
				},
				Scale = new(2, 1, 2)
			},
			new Label3D
			{
				FontSize = 160,
				Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
				Modulate = new(1, 1, 1, 1),
				Position = new(0, 1, 0),
				Text = $"{AmountNeededToCarry}"
			}
		);
	}
}
