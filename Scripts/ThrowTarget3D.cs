using Godot;

namespace Game;

using Extensions;

public sealed partial class ThrowIndicator3D : Node3D
{
	public Rid? ThrownID { get; private set; }
	public MeshInstance3D Mesh { get; } = new() { Mesh = new SphereMesh { } };

	public override void _Ready()
	{
		Name = "Throw Indicator";
		this.Add(Mesh);
	}
	public void BeginThrow(IckBody3D ick)
	{
		ThrownID = ick;
	}
	public void CheckThrownProgress(IckBody3D ick, Vector3 target)
	{
		if (ick.IsCloseTo(target))
		{
			ThrownID = null;
		}
	}
}
