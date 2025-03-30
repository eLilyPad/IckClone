using Godot;

namespace Game;

using Extensions;

public partial class DialogueWindow : MarginContainer
{
	public RichTextLabel MessageLabel { get; } = new()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill,
		SizeFlagsVertical = SizeFlags.ExpandFill,
		Text = ". . ."
	};
	public RichTextLabel TitleLabel { get; } = new()
	{
		SizeFlagsHorizontal = SizeFlags.ExpandFill,
		SizeFlagsVertical = SizeFlags.ExpandFill,
		Text = "Speaker"
	};
	public VBoxContainer ChoicesContainer { get; } = new VBoxContainer { };
	public VBoxContainer MessageContainer { get; } = new VBoxContainer
	{
		Alignment = BoxContainer.AlignmentMode.End,
		GrowHorizontal = GrowDirection.End,
		GrowVertical = GrowDirection.End,
	};

	public override void _Ready()
	{
		Name = "Dialogue Window";
		SizeFlagsHorizontal = SizeFlags.ExpandFill;
		SizeFlagsVertical = SizeFlags.ExpandFill;

		SetAnchorsAndOffsetsPreset(
			preset: LayoutPreset.FullRect,
			resizeMode: LayoutPresetMode.KeepSize,
			20
		);
		MessageContainer.SetAnchorsAndOffsetsPreset(
			preset: LayoutPreset.FullRect,
			resizeMode: LayoutPresetMode.KeepSize
		);

		Control
		spacer = new()
		{
			SizeFlagsStretchRatio = 0.7f,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		},
		contentControl = new()
		{
			SizeFlagsStretchRatio = 0.3f,
			SizeFlagsHorizontal = SizeFlags.ExpandFill,
			SizeFlagsVertical = SizeFlags.ExpandFill
		};

		this.Add(
			new VBoxContainer
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlags.ExpandFill
			}.Add(
				spacer,
				contentControl.Add(MessageContainer, ChoicesContainer)
			)
		);
	}
}
