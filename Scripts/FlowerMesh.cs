using Game.Extensions;
using Godot;

namespace Game;

public partial class FlowerMesh : ArrayMesh
{
	[Export]
	private int Segments
	{
		get => field; set
		{
			if (value <= 0) return;
			field = value;
			GenerateMesh();
		}
	} = 16;
	[Export]
	private int Radius
	{
		get => field; set
		{
			field = value;
			GenerateMesh();
		}
	} = 4;
	[Export]
	private int Height
	{
		get => field; set
		{
			field = value;
			GenerateMesh();
		}
	} = 1;
	private readonly List<int> _topVertexIndices = [];
	public FlowerMesh()
	{
		GenerateMesh();
		SurfaceSetMaterial(0, new StandardMaterial3D { AlbedoColor = Color.Color8(200, 200, 200) });
	}

	public void GenerateMesh()
	{
		//this.GenerateCube();
		this.GenerateCylinder(Radius, Height, Segments);
	}

}