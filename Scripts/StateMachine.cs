using Godot;

namespace Game;

using Extensions;

public readonly record struct StateID
{
	public static StateID New => new(Guid.NewGuid().GetHashCode());
	public int ID { get; }

	private StateID(int id) => ID = id;
}

public sealed partial class StateMachine : Node
{
	public static StateMachine Icks(IckBody3D ick, Core core)
	{
		var followPlayer = State.FollowPlayer.Icks(core, ick);
		var carryCounter = State.CarryCounter.Icks(core, ick);
		var jumpFromSpawn = State.Jump.Icks(core, ick);
		var jumpToTarget = State.Throw.Icks(core, ick);
		var machine = new StateMachine
		{
			Name = "Ick's State Machine",
			States = new Config()
				.AddTransition(from: jumpFromSpawn, [
					(When: jumpFromSpawn.CloseToTarget, To: followPlayer)
				])
				.AddTransition(from: jumpToTarget, [
					(When: jumpToTarget.CloseToTarget, To: followPlayer),
					(When: carryCounter.Carrying, To: carryCounter)
				])
				.AddTransition(from: followPlayer, [
					(When: jumpToTarget.TargetToThrow, To: jumpToTarget),
					(When: carryCounter.Carrying, To: carryCounter)
				])
				.AddTransition(from: carryCounter, [
					(When: carryCounter.NotCarrying, To: followPlayer)
				])
		};
		ick.AddChild(machine);
		return machine.ChangeState(next: jumpFromSpawn);
	}

	public required Config States { private get; init; }

	[Export]
	private string CurrentStateName
	{
		get => TryGetCurrent(current: out State? s) ? s.GetType().Name : "None";
		set { }
	}
	[Export]
	private string[] PotentialNextStates
	{
		get => [.. States.NextStatesInfo(_currentID)];
		set { }
	}

	private StateID _currentID;

	public override void _Process(double delta)
	{
		if (States.TryGetWithConditionMet(key: _currentID, id: out StateID id))
		{
			_ = ChangeState(id);
		}
		if (TryGetCurrent(current: out State? current))
		{
			current.Tick(delta);
		}
	}

	public StateMachine ChangeState(State next)
	{
		if (!States.TryGetKey(value: next, key: out StateID id))
		{
			GD.PrintErr("Cannot change to state that is not in the state machine.");
			return this;
		}
		return ChangeState(id: id);
	}
	public StateMachine ChangeState(StateID id)
	{
		if (TryGetCurrent(current: out State? current))
		{
			current.Stop();
		}
		if (States.TryGetState(key: id, state: out State? next))
		{
			next.Start();
			_currentID = id;
		}
		return this;
	}

	private bool TryGetCurrent([MaybeNullWhen(false)] out State current)
	{
		return States.TryGetState(key: _currentID, state: out current);
	}

	public sealed record Config
	{
		private readonly Dictionary<StateID, State> _states = [];
		private readonly Dictionary<StateID, List<(Func<bool> When, StateID To)>> _transitions = [];
		private readonly List<(Func<bool> When, StateID To)> _anyTransitions = [];

		public Config AddAnyTransition(Func<bool> when, State to)
		{
			_ = TryGetKey(value: to, key: out var id);
			_anyTransitions.Add((when, id));
			return this;
		}
		public Config AddTransition(State from, params Span<(Func<bool> When, State To)> transitions)
		{
			foreach (var (when, to) in transitions) { AddTransition(from, when, to); }
			return this;
		}
		public Config AddTransition(State from, Func<bool> when, State to)
		{
			_ = TryGetKey(value: from, key: out var fromID);
			_ = TryGetKey(value: to, key: out var toID);
			if (_transitions.TryGetValue(key: fromID, value: out var transitions))
			{
				transitions.Add((when, toID));
			}
			else
			{
				_transitions[fromID] = [(when, toID)];
			}
			return this;
		}

		public bool TryGetWithConditionMet(StateID key, [MaybeNullWhen(false)] out StateID id)
		{
			if (_transitions.TryGetWithConditonMet(key, value: out id)) { return true; }
			return _anyTransitions.TryGetWithConditonMet(value: out id);
		}

		public bool TryGetKey(State value, out StateID key, bool addNew = true)
		{
			if (_states.TryGetKey(value, out key)) { return true; }
			if (addNew)
			{
				_states.Add(key: key = StateID.New, value);
				return true;
			}
			return false;
		}
		public bool TryGetState(StateID key, [MaybeNullWhen(false)] out State state) => _states.TryGetValue(key, value: out state);
		public IEnumerable<string> NextStatesInfo(StateID id) => PotentialTransitions(id)
			.Select((n, c) => $"{n}: {c}\n");
		public IEnumerable<(string Name, bool ConditionMet)> PotentialTransitions(StateID id)
		{
			var info = SelectInfo(transitions: _anyTransitions);
			if (_transitions.TryGetValue(key: id, value: out var transitions))
			{
				info = [.. info, .. SelectInfo(transitions)];
			}
			return info;

			IEnumerable<(string Name, bool ConditionMet)> SelectInfo(IEnumerable<(Func<bool> When, StateID To)> transitions)
			{
				return transitions.Select(selector: _ => (StateName: _states[_.To].GetType().Name, ConditionMet: _.When()));
			}
		}

	}
}