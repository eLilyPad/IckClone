using Godot;

namespace Game;

using Extensions;

public partial class CounterFlowerBody3D : StaticBody3D
{
	public static implicit operator Rid(CounterFlowerBody3D flower) => flower.GetRid();
	public MeshInstance3D MeshInstance { get; } = new()
	{
		Mesh = new BoxMesh
		{
			Material = new StandardMaterial3D { AlbedoColor = Color.Color8(100, 200, 100) }
		}
	};
	public CollisionShape3D Collider { get; } = new() { Shape = new CylinderShape3D { } };

	private float GrowthStage
	{
		get => field;
		set
		{
			if (value < _maxGrowthStage)
			{
				Scale = Scale with { Y = 1 + value };
				field = value;
			}
			else if (value >= _maxGrowthStage && !_hasCounter)
			{
				SpawnCounter();
			}
		}
	} = 0;
	private float _maxGrowthStage = 10;
	private bool _hasCounter = false;
	private float _growthSpeed = .01f;

	public override void _Ready()
	{
		Name = "Counter Flower";
		this.Add(MeshInstance, Collider);
	}
	public override void _Process(double delta)
	{
		GrowthStage += _growthSpeed;
	}

	private void SpawnCounter()
	{
		var counter = new IckCounterBody3D();
		counter.TreeExiting += OnCounterExitingTree;
		GetParent().AddChild(counter);
		_hasCounter = true;

		void OnCounterExitingTree()
		{
			_hasCounter = false;
			GrowthStage = 0;
		}
	}
}
