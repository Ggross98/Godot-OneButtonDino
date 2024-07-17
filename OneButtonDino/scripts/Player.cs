using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public const float Speed = 80.0f;
	public const float JumpVelocity = -270.0f;
	public const float ThrowForce = 300f;
	public const float ThrowAngularSpeed = 1000f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Dictionary<string, AudioStreamPlayer> soundPlayer;

	private AnimatedSprite2D sprite;
	private RayCast2D rayCast;
	private int direction = 1;

	private Timer restartTimer;
	private GameStats gameStats;


	public const int MaxJump = 2;
	private int jumpTime;

	private Arrow arrow;
	private Sword sword;
	public string id = "green";

	public enum PlayerState { Idle, Walking, Hit };
	private PlayerState state;

    public override void _Ready()
    {

        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		rayCast = GetNode<RayCast2D>("RayCast");
		arrow = GetNode<Arrow>("Arrow");

		restartTimer = GetNode<Timer>("/root/Game/RestartTimer");
		gameStats = GetNode<GameStats>("/root/GameStats");

		soundPlayer = new Dictionary<string, AudioStreamPlayer>();
		foreach(var key in new string[]{"Hit", "Jump", "Throw"}){
			soundPlayer[key] = GetNode<AudioStreamPlayer>("SoundPlayer/" + key);
		}

		jumpTime = 0;
		sword = null;
    }

	public void Init(string _id, Vector2 _pos, PlayerState _state = PlayerState.Idle){
		id = _id;
		GlobalPosition = _pos;

		state = _state;
	}

	public void SetState(PlayerState s){
		state = s;
	}

	public void SetDirection(int d){
		if(direction != d){
			direction = d;
			Scale = new Vector2(1, d);
			RotationDegrees = 180 - RotationDegrees;
		}
		
	}

	public void Flip(){
		SetDirection(-direction);
	}

	public void PickUp(Sword s){
		if(sword == null){

			GD.Print("Pick up " + s.Name);

			sword = s;
			sword.SetState(Sword.SwordState.Holding);

			// sword.SetCollisionMaskValue(1, false);
			// sword.SetCollision(false);
		}
	}

	public void Throw(){
		if(sword != null){
			
			GD.Print("Throw " + sword.Name);

			// sword.SetCollision(true);

			var angle = Mathf.Abs(arrow.GetAngle());
			var radians = Utils.DegreesToRadians(angle);
			var x = Mathf.Cos(radians) * ThrowForce;
			var y = Mathf.Sin(radians) * ThrowForce;
			var v = new Vector2(direction * x, -y);
			GD.Print(v);

			sword.velocity = v;
			sword.SetState(Sword.SwordState.Flying);
			sword.angularSpeed = ThrowAngularSpeed;

			sword = null;

			soundPlayer["Throw"].Play();
		}
	}

	public void Hit(){
		
		if(state != PlayerState.Walking){
			return;
		}

		state = PlayerState.Hit;

		// Drop the sword
		// sword.velocity = Vector2.Zero;
		// sword.state = Sword.SwordState.Flying;
		// sword.angularSpeed = 0;
		// sword = null;

		// Fall down
		// GetNode<CollisionShape2D>("CollisionShape2D").QueueFree();

		// Restart timer
		if(restartTimer.IsStopped()){
			restartTimer.Start();
		}
		
		// Add score
		if(id == "red"){
			gameStats.score0 += 1;
		}
		else{
			gameStats.score1 += 1;
		}

		// QueueFree();
		sprite.Play(id+"_hit");

		soundPlayer["Hit"].Play();
	}

	public bool IsHolding(){
		return sword != null;
	}

    public override void _PhysicsProcess(double delta)
	{

		switch(state){
			case PlayerState.Idle:
				Velocity = Vector2.Zero;
				sprite.Play(id+"_idle");
				break;
			case PlayerState.Walking:
				Control((float)delta);
				sprite.Play(id+"_walking");
				break;
			case PlayerState.Hit:
				Velocity = Vector2.Zero;
				break;
		}

		
	}

	private void Control(float delta){
		Vector2 velocity = Velocity;

		// Handle direction
		if(rayCast.IsColliding()){
			GD.Print("Colliding");
			Flip();
		}

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * delta;

		// Handle Jump.
		if(IsOnFloor()){
			jumpTime = MaxJump;
		}


		if (Input.IsActionJustPressed(id)){

			if(sword != null){
				Throw();
			}
			else if( jumpTime > 0) {
				velocity.Y = JumpVelocity;
				jumpTime -= 1;

				soundPlayer["Jump"].Play();
			}
			
		}

		// Handle sword
		if(sword != null){
			sword.SetGlobalPosition(GlobalPosition + new Vector2(-8 * direction, -2));
		}

		velocity.X = direction * Speed;

		this.Velocity = velocity;
		// GD.Print(velocity);
		MoveAndSlide();
	}
}
