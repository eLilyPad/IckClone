using Game.Extensions;
using Godot;

namespace Game;

[Tool, GlobalClass]
public partial class Flower3D : Node3D
{
	[Export]
	private bool ResetMesh { get => field; set { FlowerMesh.ClearSurfaces(); } }
	[Export]
	private int Segments
	{
		get => field; set
		{
			if (value <= 0) { return; }
			field = value;
			RefreshMesh();
		}
	} = 4;

	[Export]
	private int Radius
	{
		get => field; set
		{
			field = value;
			RefreshMesh();
		}
	} = 4;

	[Export]
	private int Height
	{
		get => field; set
		{
			field = value;
			RefreshMesh();
		}
	} = 1;

	private ArrayMesh FlowerMesh = new();
	public Flower3D()
	{
		AddChild(new MeshInstance3D { Mesh = FlowerMesh });
	}

	private void RefreshMesh()
	{
		FlowerMesh.GenerateFlower(Height, Segments);
		//FlowerMesh.GenerateCircle(Radius, Segments);
		//FlowerMesh.GenerateCylinder(Radius, Height, Segments);
	}
}
