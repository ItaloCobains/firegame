using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Player")]
	[Export]
	public  float Speed { get; set; } = 5.0f;
	[Export]
	public float JumpVelocity { get; set; } = 4.5f;
	[Export]
	public float MouseSensitivity { get; set; } = 0.1f;
	[Export]
	public int MaxHealth { get; set; } = 100;
	[Export]
	public int CurrentHealth { get; set; } = 100;
	[Export]
	public string Name { get; set; } = "Player";

	private Camera3D _camera;
	private Vector2 _mouseDelta = Vector2.Zero;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Input(InputEvent @event)
    {
			if (@event is InputEventMouseMotion mouseMotionEvent)
			{
				_mouseDelta = mouseMotionEvent.Relative;
			}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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

		RotateY(Mathf.DegToRad(-_mouseDelta.x * MouseSensitivity));
		_camera.RotateX(Mathf.DegToRad(-_mouseDelta.y * MouseSensitivity));

		Vector3 cameraRotation = _camera.RotationDegrees;
		cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);
		_camera.RotationDegrees = cameraRotation;

		Velocity = velocity;
		MoveAndSlide();
	}
}
