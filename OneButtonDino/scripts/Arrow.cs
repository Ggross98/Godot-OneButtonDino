using Godot;
using System;

public partial class Arrow : Sprite2D
{   
    public const float Speed = 200;
    // public const float StartAngle = -10;
    public const float MinAngle = -100, MaxAngle = 0;
    
    private int direction;

    public override void _Ready()
    {
        RotationDegrees = (float)GD.RandRange(MinAngle, MaxAngle);
        GD.Print(RotationDegrees);
        direction = 1;
    }

    public override void _PhysicsProcess(double delta){
        RotationDegrees = RotationDegrees + (float)(delta * Speed * direction);

        if(RotationDegrees >= MaxAngle){
            RotationDegrees = MaxAngle;
            direction = -1;
        }
        else if(RotationDegrees <= MinAngle){
            RotationDegrees = MinAngle;
            direction = 1;
        }
    }

    public float GetAngle(){
        return RotationDegrees;
    }
}
