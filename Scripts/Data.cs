using Godot;

namespace Game;

public partial class Data : Resource
{
	public const float RAY_DISTANCE = 1000;
}
public static class Paths
{
	public const string
	Gravity = "physics/3d/default_gravity",
	Local = "res://",
	Art = $"{Local}/Art/",
	Audio = $"{Local}/Audio/";
}