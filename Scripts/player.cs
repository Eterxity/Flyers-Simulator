using Godot;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

public partial class player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	[Export]
	public float Sensitivity = 0.01f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public Node3D _head;
	public Camera3D _camera;
	public RayCast3D _rayCast;
	public PhysicsDirectSpaceState3D _spaceState;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;

		_head = GetNode<Node3D>("Head");
		_camera = GetNode<Camera3D>("Head/Camera3D");
		_rayCast = GetNode<RayCast3D>("Head/Camera3D/RayCast3D");
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion mouseMotion = @event as InputEventMouseMotion;
			_head.RotateY(-mouseMotion.Relative.X * Sensitivity);
			_camera.RotateX(-mouseMotion.Relative.Y * Sensitivity);

			Vector3 cameraRot = _camera.Rotation;
			cameraRot.X = Mathf.Clamp(cameraRot.X, Mathf.DegToRad(-90f), Mathf.DegToRad(90f));
			_camera.Rotation = cameraRot;
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("moveLeft", "moveRight", "moveForward", "moveBackward");
		Vector3 direction = (_head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
