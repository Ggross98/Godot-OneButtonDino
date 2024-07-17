using Godot;
using System;

public partial class Sword : StaticBody2D
{
	public enum SwordState { Holding, OnFloor, Flying };

	private SwordState state;

	private CollisionShape2D physicsShape, eventShape;

	public Vector2 velocity;
	public float angularSpeed;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float gravityScale = 0.7f;

	public override void _Ready()
	{
		physicsShape = GetNode<CollisionShape2D>("CollisionShape2D");
		eventShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");

		// state = SwordState.Flying;
		velocity = Vector2.Zero;
		angularSpeed = 0;
	}

	public void Init(Vector2 _pos, SwordState _state = SwordState.OnFloor){
		GlobalPosition = _pos;
		state = _state;
	}


	public void SetGlobalPosition(Vector2 pos){
		GlobalPosition = pos;
	}

	public void SetState(SwordState s){
		state = s;
	}


	public void OnBodyEntered(Node2D body){
		GD.Print("Body entered: " + body.ToString());
		
		if(body.GetType() == typeof(TileMap) ){

			if(state == SwordState.Flying){
				state = SwordState.OnFloor;
				velocity = Vector2.Zero;
			}
			
		}

		else if(body.GetType() == typeof(Platform)){

			if(state == SwordState.Flying && velocity.Y >= 0){
				state = SwordState.OnFloor;
				velocity = Vector2.Zero;
			}
		}

		else if(body.GetType() == typeof(Player)){

			// GD.Print("Player entered");
			var player = (Player)body;

			switch(state){
				case SwordState.Flying:

					player.Hit();

					break;

				case SwordState.OnFloor:

					if(!player.IsHolding()){
						player.PickUp(this);
						state = SwordState.Holding;
					}
					
					break;

				case SwordState.Holding:
					break;
			}
		}

		else if(body.GetType() == typeof(Wall)){
			velocity.X = -velocity.X;
		}
	}

	
	public override void _PhysicsProcess(double delta)
	{   
		switch(state){
			case SwordState.Flying:
				velocity.Y += gravity * gravityScale * (float)delta;
				MoveAndCollide(velocity * (float)delta);

				RotationDegrees += angularSpeed * (float)delta;

				break;
			case SwordState.OnFloor:
				velocity = Vector2.Zero;
				break;
			case SwordState.Holding:
				velocity = Vector2.Zero;
				break;
		}
		
	}
}
