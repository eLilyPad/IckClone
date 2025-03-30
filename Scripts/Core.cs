using Godot;

namespace Game;

using Extensions;

public interface IHaveParentNode { Node? Parent { set; } }

public sealed partial class Core : Node
{
	public Stage3D CurrentStage => Stages.Current;
	public PlayerBody3D Player { get; } = new();
	public ThrowIndicator3D ThrowIndicator { get; } = new();

	public ImmutableDictionary<Rid, IckCounterBody3D> Counters => _counters.ToImmutableDictionary();
	public ImmutableDictionary<Rid, IckBody3D> Icks => _icks.ToImmutableDictionary();
	public ImmutableDictionary<Rid, AudioStreamPlayer3D> Audio3DPlayers => _audibleNodes.ToImmutableDictionary();
	[Export] public AudioStream IckJumpAudio { get; private set; } = GD.Load<AudioStream>("res://Audio/Wee.wav");


	private readonly Dictionary<Rid, IckCounterBody3D> _counters = [];
	private readonly Dictionary<Rid, IckBody3D> _icks = [];
	private readonly Dictionary<Rid, AudioStreamPlayer3D> _audibleNodes = [];
	private readonly Dictionary<Rid, StateMachine> _states = [];
	private readonly List<Rid> _followingPlayer = [];
	private readonly Dictionary<Rid, Rid> _carriers = [];
	private readonly Dictionary<Key, Action> _triggers = [];

	[Export]
	private Windows Windows
	{
		get => field;
		set
		{
			field.ReplaceParent(nextHolder: value, parent: this);
			field = value;
		}
	} = new();

	[Export]
	private Stages Stages
	{
		get => field;
		set => Stages.Initialize(current: ref field, next: value, parent: this);
	} = new();
	[Export]
	private IckBody3D[] IcksFollowing
	{
		get => [.. _icks.Values.Following(_followingPlayer)];
		set { }
	}
	public override void _Ready()
	{
		Name = "Pickim Game Clone";
		ChildExitingTree += OnChildExitingTree;
		ChildEnteredTree += OnChildEnteredTree;
		Stages.Selected = Stages.Names.Grid;
		this.Add(
			Player,
			ThrowIndicator,
			new CounterFlowerBody3D()
		);
	}
	public override void _Process(double delta)
	{
		ThrowIndicator.Position = Player.MousePositionFromPosition;
	}
	public override void _Input(InputEvent input)
	{
		switch (input)
		{
			case InputEventKey key
			when _triggers.TryGetPressed(key, trigger: out Action? trigger):
				trigger();
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false } _
			when TryGetThrowable(out IckBody3D? ick):
				ThrowIndicator.BeginThrow(ick);
				break;
		}
	}

	public void AddFollower(IckBody3D ick)
	{
		_followingPlayer.Add(ick);
	}
	public bool RemoveFollower(IckBody3D ick)
	{
		return _followingPlayer.Remove(ick);
	}
	public bool TryGetCarrying(IckBody3D carrier, [MaybeNullWhen(false)] out IckCounterBody3D carried)
	{
		return _counters.TryGetCarrying(carriers: _carriers, carrier, out carried);
	}
	public bool TryGetThrowable([MaybeNullWhen(false)] out IckBody3D ick)
	{
		return _icks.TryGetThrowable(throwables: _followingPlayer, node: out ick);
	}
	public void Carry(IckBody3D carrier, IckCounterBody3D carried)
	{
		RemoveFollower(carrier);
		_carriers[carrier] = carried;
	}

	private void OnIckZoneEntered(IckBody3D ick, Node3D node)
	{
		if (node is not IckCounterBody3D counter) { return; }
		Carry(carrier: ick, carried: counter);
	}
	private void OnShipZoneEntered(Node3D node)
	{
		switch (node)
		{
			case PlayerBody3D when _icks.Count == 0:
				AddChild(new IckBody3D());
				break;
			case IckCounterBody3D counter:
				counter.QueueFree();
				AddChild(new IckBody3D());
				break;
		}
	}
	private void OnChildExitingTree(Node child)
	{
		switch (child)
		{
			case IckBody3D ick:
				_ = _followingPlayer.Remove(ick);
				_ = _carriers.Remove(ick);
				_ = _icks.Remove(ick);
				_ = _states.Remove(ick);
				break;
			case IckCounterBody3D counter:
				_ = _counters.Remove(counter);
				break;
		}
	}
	private void OnChildEnteredTree(Node child)
	{
		switch (child)
		{
			case Stage3D stage:
				stage.SetSpawnPosition(node: Player);
				stage.Ship.Zone.BodyEntered += OnShipZoneEntered;
				break;
			case PlayerBody3D player:
				Stages.Current.SetSpawnPosition(node: player);
				break;
			case IckBody3D ick:
				AudioStreamPlayer3D audioPlayer = _audibleNodes[ick] = new() { Autoplay = false };
				ick.AddChild(audioPlayer);
				ick.Zone.BodyEntered += node => OnIckZoneEntered(ick, node);
				Stages.Current.SetSpawnPosition(node: ick);
				_icks[ick] = ick;
				_states[ick] = StateMachine.Icks(ick, core: this);
				break;
			case IckCounterBody3D counter:
				_counters[counter] = counter;
				Stages.Current.SetSpawnPosition(node: counter);
				break;
			case CounterFlowerBody3D flower:
				Stages.Current.SetSpawnPosition(node: flower);
				break;
		}
	}
}

public static class CoreExtensions
{
	public static void ReplaceParent(
		this IHaveParentNode parentHolder,
		IHaveParentNode nextHolder,
		Node parent
	)
	{
		parentHolder.Parent = null;
		nextHolder.Parent = parent;
	}
	public static void Add<T>(this Dictionary<Rid, T> nodes, T node)
	where T : PhysicsBody3D
	{
		nodes.Add(key: node.GetRid(), value: node);
	}
	public static bool TryGetPressed(
		this Dictionary<Key, Action> triggers,
		InputEventKey key,
		[MaybeNullWhen(false)] out Action trigger
	)
	{
		trigger = null;
		var actionName = key.Keycode.DefaultInput();

		if (actionName is GodotExtensions.UnAssignedKey) { return false; }
		if (!key.IsAction(actionName)) { return false; }
		if (key.IsActionPressed(actionName)) { return false; }

		return triggers.TryGetValue(key.Keycode, out trigger);
	}
	public static bool TryGetCarrying<TCarrier, TCarried>(
		this IDictionary<Rid, TCarried> nodes,
		IDictionary<Rid, Rid> carriers,
		TCarrier carrier,
		[MaybeNullWhen(false)] out TCarried? carried
	)
	where TCarried : PhysicsBody3D
	where TCarrier : PhysicsBody3D
	{
		Assert(condition: carrier is not null, "Carrier is null");
		carried = null;
		return carriers.TryGetValue(key: carrier.GetRid(), value: out Rid counterID)
			&& nodes.TryGetValue(key: counterID, value: out carried);
	}
	public static IEnumerable<T> Following<T>(this IEnumerable<T> nodes, IEnumerable<Rid> followers)
	where T : PhysicsBody3D
	{
		return nodes.Where(node => followers.Contains(node.GetRid()));
	}
	public static bool TryGetThrowable<T>(
		this IDictionary<Rid, T> nodes,
		IEnumerable<Rid> throwables,
		[MaybeNullWhen(false)] out T? node
	)
	where T : PhysicsBody3D
	{
		node = null;
		return throwables.Any()
			&& nodes.TryGetValue(throwables.First(), out node);
	}
}
