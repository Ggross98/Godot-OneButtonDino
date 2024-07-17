using Godot;
using System;

public partial class PlayerObsolete : CharacterBody2D
{
	public const float Speed = 100.0f;
	public const float JumpVelocity = -300.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();


	public int direction = 1;

	private AnimatedSprite2D sprite;
	private RayCast2D rayCastRight, rayCastLeft;


	public const int MaxJump = 2;
	private int jumpTime;

	
    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		rayCastLeft = GetNode<RayCast2D>("RayCast/Left");
		rayCastRight = GetNode<RayCast2D>("RayCast/Left");

		jumpTime = 0;
    }

	public void Flip(){
		direction = -direction;
	}


    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Handle direction
		if(rayCastLeft.IsColliding() && direction == -1){
			direction = 1;
			// GD.Print("Left colliding");
		}
		else if(rayCastRight.IsColliding() && direction == 1){
			direction = -1;
			// GD.Print("Right colliding");
		}
		sprite.FlipH = direction == -1;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if(IsOnFloor()){
			jumpTime = MaxJump;
		}
		if (Input.IsActionJustPressed("LeftPlayer") && jumpTime > 0){
			velocity.Y = JumpVelocity;
			jumpTime -= 1;
		}
			

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		// Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		// if (direction != Vector2.Zero)
		// {
		// 	velocity.X = direction.X * Speed;
		// }
		// else
		// {
		// 	velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		// }
		velocity.X = direction * Speed;

		this.Velocity = velocity;
		MoveAndSlide();
	}
}
