using Godot;

namespace Game.Extensions;

public static class GodotMeshes
{
	public static void GenerateFlower(
		this ArrayMesh mesh,
		float height,
		int segments
	)
	{
		List<Vector3> vertices = [];
		List<Vector3> normals = [];
		List<Vector2> uvs = [];
		List<int> indices = [];
		mesh.ClearSurfaces();

		foreach ((int segment, float x, float z) in PolygonPoints(segments, 5))
		{
			// Bottom vertex
			vertices.Add(new Vector3(x, 0, z));
			normals.Add(Vector3.Down);
			uvs.Add(new Vector2(segment / 4, 1));

			int next = (segment + 1) % segments;
			indices.Add(segment);
			indices.Add(4);
			indices.Add(next);

			// Top vertex
			vertices.Add(new Vector3(x, height, z));
			normals.Add(Vector3.Up);
			uvs.Add(new Vector2(segment / segments, 1));

			indices.Add(segment);
			indices.Add(4);
			indices.Add(next);
		}

		for (int i = 0; i < segments; i++)
		{
			int next = (i + 1) % segments;
			indices.Add(segments);
			indices.Add(i);
			indices.Add(next);
		}
		for (int i = segments; i < segments * 2; i++)
		{
			int next = (i + 1) % segments;
			indices.Add(segments);
			indices.Add(i);
			indices.Add(next);
		}
		mesh.AddSurface(vertices, normals, uvs, indices);

	}
	public static void GenerateCircle(
		this ArrayMesh mesh,
		int radius,
		int segments
	)
	{
		MeshArray arrays = new();
		mesh.ClearSurfaces();
		int height = 0;

		// Generate bottom and top circle vertices
		foreach ((int i, float x, float z) in PolygonPoints(segments, radius))
		{
			arrays.Vertices.Add(new Vector3(x, height, z));
			arrays.Normals.Add(Vector3.Up);
			arrays.UVs.Add(new Vector2(i / (float)segments, 1));

			int next = (i + 1) % segments;
			arrays.Indices.Add(i);
			arrays.Indices.Add(segments);
			arrays.Indices.Add(next);
		}

		arrays.Vertices.Add(new Vector3(0, height, 0));
		arrays.Normals.Add(Vector3.Down);
		arrays.UVs.Add(new Vector2(.5f, 1));

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Points, arrays);
	}
	public static void GenerateCylinder(
		this ArrayMesh mesh,
		int radius,
		int height,
		int segments
	)
	{
		mesh.ClearSurfaces();
		MeshArray meshArray = new();
		int baseHeight = 0;

		// Generate bottom and top circle vertices
		foreach ((float segment, float x, float z) in PolygonPoints(segments, radius))
		{
			// Bottom vertex
			meshArray.Vertices.Add(new Vector3(x, baseHeight, z));
			meshArray.Normals.Add(Vector3.Down);
			meshArray.UVs.Add(new Vector2(segment / segments, 1));

			//// Top vertex
			meshArray.Vertices.Add(new Vector3(x, height * .5f, z));
			meshArray.Normals.Add(Vector3.Up);
			meshArray.UVs.Add(new Vector2(segment / segments, 0));
		}

		// Add center vertices for caps
		meshArray.Vertices.Add(new Vector3(0, y: baseHeight, 0));
		meshArray.Normals.Add(Vector3.Down);
		meshArray.UVs.Add(new Vector2(0.5f, 1));

		//int topCenterIndex = vertices.Count;
		//vertices.Add(new Vector3(0, height * .5f, 0));
		//normals.Add(Vector3.Up);
		//uvs.Add(new Vector2(0.5f, 0));

		// Generate indices for bottom cap
		for (int i = 0; i < segments; i++)
		{
			BottomCap(i, bottomCenterIndex: segments);
			//TopCap(i, topCenterIndex);
			//SideFaces(i);
		}

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArray);

		void BottomCap(int i, int bottomCenterIndex)
		{
			int next = (i + 1) % segments;
			meshArray.Indices.Add(bottomCenterIndex);
			meshArray.Indices.Add(i);
			meshArray.Indices.Add(next);
		}
		void TopCap(int i, int topCenterIndex)
		{
			int next = (i + 1) % segments;
			meshArray.Indices.Add(topCenterIndex);
			meshArray.Indices.Add(next + 1);
			meshArray.Indices.Add(i + 1);
		}
		void SideFaces(int i)
		{
			int next = (i + 1) % segments;
			int bottom1 = i;
			int bottom2 = next;
			int top1 = i + 1;
			int top2 = next + 1;

			// First triangle
			meshArray.Indices.Add(bottom1);
			meshArray.Indices.Add(top1);
			meshArray.Indices.Add(bottom2);

			// Second triangle
			meshArray.Indices.Add(bottom2);
			meshArray.Indices.Add(top1);
			meshArray.Indices.Add(top2);
		}
	}

	public static void GenerateCube(this ArrayMesh mesh)
	{
		mesh.ClearSurfaces();

		List<Vector3>
		vertices = [
			new (-1, -1, -1),
			new (1, -1, -1),
			new (1, 1, -1),
			new (-1, 1, -1),
			new (-1, -1, 1),
			new (1, -1, 1),
			new (1, 1, 1),
			new (-1, 1, 1)
		], normals = [
			new (0, 0, -1),
			new (0, 0, 1),
			new (0, -1, 0),
			new (0, 1, 0),
			new (-1, 0, 0),
			new (1, 0, 0)
		];
		List<Vector2> uvs = [
			new(0,0),
			new(1,0),
			new(1,1),
			new(0,1)
		];
		List<int> indices = [
			0, 2, 1, 0, 3, 2, // Front
			4, 5, 6, 4, 6, 7, // Back
			0, 1, 5, 0, 5, 4, // Bottom
			3, 6, 2, 3, 7, 6, // Top
			1, 2, 6, 1, 6, 5, // Right
			0, 4, 7, 0, 7, 3  // Left
		];
		Godot.Collections.Array arrays = [];
		arrays.Resize((int)Mesh.ArrayType.Max);

		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
		arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

		mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
		//mesh.AddSurfaceFromCollections(vertices, normals, uvs, indices);
	}

	private static IEnumerable<(int i, float x, float z)> SquarePoints(int length)
	{
		for (int i = 0; i < 4; i++)
		{
			float
			x = i % 2 == 0 ? length : -length,
			z = i < 2 ? length : -length;
			yield return (i, x, z);
		}
	}
	private static IEnumerable<(int segment, float x, float z)> PolygonPoints(
		int segments,
		int radius,
		int offset = 0
	)
	{
		for (int i = 0; i < segments; i++)
		{
			float angle = i * (2.0f * Mathf.Pi / segments) + offset;
			yield return (
				i,
				Mathf.Cos(angle) * radius,
				Mathf.Sin(angle) * radius
			);
		}
	}
	private static void AddSurface(
		this ArrayMesh mesh,
		List<Vector3> vertices,
		List<Vector3> normals,
		List<Vector2> uvs,
		List<int> indices,
		Mesh.PrimitiveType primitive = Mesh.PrimitiveType.Triangles
	)
	{
		Godot.Collections.Array arrays = [];
		arrays.Resize((int)Mesh.ArrayType.Max);

		arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		arrays[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		arrays[(int)Mesh.ArrayType.TexUV] = uvs.ToArray();
		arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

		mesh.AddSurfaceFromArrays(primitive, arrays);
	}
	private record MeshArray
	{
		public static implicit operator Godot.Collections.Array(MeshArray array)
		{
			Godot.Collections.Array arrays = [];
			arrays.Resize((int)Mesh.ArrayType.Max);
			arrays[(int)Mesh.ArrayType.Vertex] = array.Vertices.ToArray();
			arrays[(int)Mesh.ArrayType.Normal] = array.Normals.ToArray();
			arrays[(int)Mesh.ArrayType.TexUV] = array.UVs.ToArray();
			arrays[(int)Mesh.ArrayType.Index] = array.Indices.ToArray();
			return arrays;
		}
		public List<Vector3> Vertices { get; init; } = [];
		public List<Vector3> Normals { get; init; } = [];
		public List<Vector2> UVs { get; init; } = [];
		public List<int> Indices { get; init; } = [];
	}
}

public static class GodotExtensions
{
	private static float Gravity => ProjectSettings.GetSetting(Paths.Gravity).AsSingle();

	#region CharacterBody3D

	public static void Follow<T>(
		this T body,
		double delta,
		in Vector3 target,
		float stoppingDistance = 10,
		float speed = 5
	) where T : CharacterBody3D
	{
		float targetDistance = body.Position.DistanceTo(target);
		Vector3
		targetDirection = body.Position.DirectionTo(target),
		reverse = targetDirection with { X = -targetDirection.X, Z = -targetDirection.Z };
		bool
		nearTarget = Mathf.IsEqualApprox(targetDistance, stoppingDistance),
		tooClose = targetDistance < stoppingDistance,
		tooFar = targetDistance > stoppingDistance;

		body.Velocity = nearTarget switch
		{
			false when tooClose => reverse,
			false when tooFar => targetDirection,
			_ => Vector3.Zero
		} * speed * (float)delta;

		if (!body.IsOnFloor())
		{
			body.Velocity = body.Velocity with { Y = body.Velocity.Y - Gravity } * (float)delta;
		}
		body.MoveAndSlide();
	}
	public static void JumpTo<T>(
		this T body,
		double delta,
		in Vector3 start,
		in Vector3 target
	) where T : CharacterBody3D
	{
		float
		speed = 500,
		height = 10,
		distance = (body.Position with { Y = start.Y })
			.DistanceTo(target with { Y = start.Y }),
		totalDistance = start
			.DistanceTo(target with { Y = start.Y });
		Vector3
		targetDirection = body.Position.DirectionTo(target),
		peakHeight = body.Position.Lerp(target, .5f) with { Y = start.Y + height };

		body.Velocity = (distance / totalDistance * 100) switch
		{
			>= 50 => body.Position.DirectionTo(peakHeight) * speed,
			<= 50 when body.Position.Y >= peakHeight.Y => targetDirection with { Y = targetDirection.Y - Gravity },
			<= 50 when body.Position.Y <= peakHeight.Y => targetDirection * speed,
			_ => Vector3.Zero
		} * (float)delta;

		body.MoveAndSlide();

	}

	#endregion
	#region Keys

	public const string UnAssignedKey = "None";

	public static string DefaultInput(this Key key) => key switch
	{
		Key.A => "PlayerLeft",
		Key.D => "PlayerRight",
		Key.W => "PlayerUp",
		Key.S => "PlayerDown",
		Key.E => "Interact",
		Key.Escape => "Pause",
		Key.F11 => "FullScreen",
		_ => UnAssignedKey
	};
	public static Vector2 Direction(this (Key Up, Key Down, Key Left, Key Right) movementKeys)
	{
		return Input.GetVector(
			movementKeys.Left.DefaultInput(),
			movementKeys.Right.DefaultInput(),
			movementKeys.Up.DefaultInput(),
			movementKeys.Down.DefaultInput()
		);
	}
	public static InputEventKey AsEvent(this Key key) => new()
	{
		ResourceName = key.DefaultInput(),
		PhysicalKeycode = key
	};
	public static Action FirstPressed(
		this Dictionary<Key, Action> triggers,
		InputEventKey input,
		Action defaultValue
	)
	{
		return triggers.Keys.FirstPressedKey(input) switch
		{
			Key key => triggers.GetValueOrDefault(key, defaultValue),
			_ => defaultValue,
		};
	}
	public static Key? FirstPressedKey(this IEnumerable<Key> triggers, InputEventKey input)
	{
		return triggers
			.Cast<Key?>()
			.FirstOrDefault(predicate: Pressed, null);

		bool Pressed(Key? t) => input.IsActionPressed(t?.DefaultInput() ?? "");
	}

	#endregion
	#region Maths

	public static Vector3 CapVelocity(this Vector3 velocity, float MaxSpeed)
	{
		return velocity.Length() > MaxSpeed ? velocity.Normalized() * MaxSpeed : velocity;
	}

	#endregion
	#region Node3D

	public static bool IsCloseTo(this Node3D node, Vector3 target, float distance = .1f)
	{
		return node.Position.DistanceTo(target) < distance;
	}
	public static bool IsCloseTo(this Node3D node, Node3D target, float distance = .1f)
	{
		return node.IsCloseTo(target: target.Position, distance);
	}

	#endregion
	#region Node

	public static T Add<T>(
		this T parent,
		params ReadOnlySpan<Node> children
	)
	where T : Node
	{
		foreach (Node node in children)
		{
			parent.AddChild(node);
		}
		return parent;
	}

	public static void ReplaceChild<TNode>(
		this Node parent,
		TNode currentNode,
		TNode previousNode
	) where TNode : Node
	{
		Assert(condition: previousNode is not null, "No previous node is available");
		Assert(condition: currentNode is not null, "No current node is available");
		if (previousNode.IsInsideTree() && parent.HasNode(previousNode.GetPath()))
		{
			parent.RemoveChild(previousNode);
		}
		parent.AddChild(currentNode);
	}

	#endregion
	#region Camera3D

	public static Vector3 TransformPositionFromMouse(this Camera3D camera, Vector3 Position, Vector3 CameraOffset)
	{
		Vector2 screenPoint = camera.GetViewport().GetMousePosition();
		Vector3 offsetCameraPosition = camera.Position with
		{
			Z = camera.Position.Z - CameraOffset.Z
		},
		cameraProjection = camera
			.ProjectPosition(screenPoint, zDepth: Data.RAY_DISTANCE)
			.DirectionTo(to: offsetCameraPosition) * 10;

		return Position with
		{
			X = Position.X - cameraProjection.X,
			Z = Position.Z - cameraProjection.Z
		};
	}

	#endregion
	#region Mesh

	public static Mesh TransparentMesh<T>(this Color AlbedoColor) where T : PrimitiveMesh, new()
	{
		return new T
		{
			Material = new StandardMaterial3D
			{
				AlbedoColor = AlbedoColor,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha
			}
		};
	}

	#endregion
	#region Rid

	public static IEnumerable<KeyValuePair<T1, T2>> SelectRids<T1, T2>(
		this IEnumerable<KeyValuePair<Rid, Rid>> dictionary,
		Func<Rid, T1> key,
		Func<Rid, T2> value
	)
	{
		return dictionary
			.Select(selector: pair => new KeyValuePair<T1, T2>(key(pair.Key), value(pair.Value)));
	}

	#endregion
}