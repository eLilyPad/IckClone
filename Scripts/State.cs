using Game.Extensions;
using Godot;

namespace Game;

public abstract class State
{
	public sealed class FollowPlayer : State
	{
		public static FollowPlayer Icks(Core core, IckBody3D ick)
		{
			return new()
			{
				Core = core,
				Ick = ick,
			};
		}

		public required Core Core { private get; init; }
		public required IckBody3D Ick { private get; init; }
		public float StoppingDistance { get; init; } = 10;
		public float Speed { get; init; } = 500;

		public override void Start()
		{
			Core.AddFollower(Ick);
		}
		public override void Tick(double delta)
		{
			Ick.Follow(delta, target: Core.Player.Position, stoppingDistance: StoppingDistance, speed: Speed);
		}
		public override void Stop()
		{
			Core.RemoveFollower(Ick);
		}
	}
	public sealed class Jump : State
	{
		public static Jump Icks(Core core, IckBody3D ick)
		{
			return new()
			{
				Target = core.CurrentStage.IckSpawnJumpPosition,
				Body = ick,
				JumpAudio = core.IckJumpAudio,
				Audio3D = core.Audio3DPlayers[ick],
			};
		}
		public required Vector3 Target { get; init; }
		public required AudioStream JumpAudio { get; init; }
		public required AudioStreamPlayer3D Audio3D { get; init; }
		public required CharacterBody3D Body { get; init; }
		private Vector3 _startPosition;
		public override void Start()
		{
			if (Body is IckBody3D ick)
			{
				ick.Zone.Monitoring = false;
				ick.Zone.Hide();
			}
			_startPosition = Body.Position;
			Audio3D.Stream = JumpAudio;
			Audio3D.Play();
		}
		public override void Tick(double delta)
		{
			Body.JumpTo(delta, start: _startPosition, target: Target);
		}
		public override void Stop()
		{
			if (Body is IckBody3D ick)
			{
				ick.Zone.Monitoring = true;
				ick.Zone.Show();
			}
		}
		public bool CloseToTarget() => Body.IsCloseTo(target: Target);
	}
	public sealed class Throw : State
	{
		public static Throw Icks(Core core, IckBody3D ick)
		{
			return new()
			{
				Indicator = core.ThrowIndicator,
				Ick = ick,
				Audio3D = core.Audio3DPlayers[ick],
				ThrowAudio = core.IckJumpAudio,
			};
		}
		public required ThrowIndicator3D Indicator { private get; init; }
		public required IckBody3D Ick { private get; init; }
		public required AudioStream ThrowAudio { private get; init; }
		public required AudioStreamPlayer3D Audio3D { private get; init; }
		private Vector3 _endPosition;
		private Vector3 _startPosition;
		public override void Start()
		{
			Ick.Zone.Monitoring = false;
			Ick.Zone.Hide();

			(_startPosition, _endPosition) = (Ick.Position, Indicator.Position);
			Audio3D.Stream = ThrowAudio;
			Audio3D.Play();
		}
		public override void Tick(double delta)
		{
			Ick.JumpTo(delta, start: _startPosition, target: _endPosition);
			Indicator.CheckThrownProgress(ick: Ick, target: _endPosition);
		}
		public override void Stop()
		{
			Ick.Zone.Monitoring = true;
			Ick.Zone.Show();
		}
		public bool TargetToThrow() => Indicator.ThrownID == Ick;
		public bool CloseToTarget() => Ick.IsCloseTo(target: _endPosition);
	}
	public sealed class CarryCounter : State
	{
		public static CarryCounter Icks(Core core, IckBody3D ick)
		{
			return new()
			{
				Ick = ick,
				Core = core,
			};
		}
		public required IckBody3D Ick { private get; init; }
		public required Core Core { private get; init; }
		public float Speed { get; init; } = 500;
		public override void Tick(double delta)
		{
			if (!TryGetCarried(carried: out IckCounterBody3D? counter)) { return; }
			Ick.Velocity = Ick.Position.DirectionTo(Core.CurrentStage.Ship.Position) * Speed * (float)delta;
			Ick.MoveAndSlide();
			counter.Position = Ick.Position with { Y = Ick.Position.Y + 2 };
		}
		public bool TryGetCarried([MaybeNullWhen(false)] out IckCounterBody3D carried)
		{
			return Core.TryGetCarrying(Ick, carried: out carried);
		}
		public bool Carrying() => TryGetCarried(carried: out _);
		public bool NotCarrying() => !TryGetCarried(carried: out _);
	}

	public virtual void Start() { }
	public virtual void Tick(double delta) { }
	public virtual void Stop() { }
}
