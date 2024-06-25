using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public  float Speed { get; set; } = 5.0f;
	public float JumpVelocity { get; set; } = 4.5f;
	public float mouseSensitivity = 0.01f;

	private Camera3D _camera;
	private Vector2 _cameraRotation = Vector2.Zero;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera");

		// Hide the mouse cursor and grab it.
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (@event is InputEventMouseMotion) {
			// Input.MouseMode = Input.MouseModeEnum.Captured; // o cara fica bugado aqui para sempre kk
			InputEventMouseMotion MouseEvent = @event as InputEventMouseMotion;
			CameraLook(MouseEvent.Relative);
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

		Velocity = velocity;
		MoveAndSlide();
	}

	public void CameraLook(Vector2 relative)
	{
		_cameraRotation.X -= relative.Y * mouseSensitivity;
		_cameraRotation.Y -= relative.X * mouseSensitivity;

		_cameraRotation.X = Mathf.Clamp(_cameraRotation.X, -Mathf.Pi / 2, Mathf.Pi / 2);

		_camera.Rotation = new Vector3(_cameraRotation.X, 0, 0);
		Rotation = new Vector3(0, _cameraRotation.Y, 0);
	}
}
