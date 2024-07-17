using Godot;
using System;

public partial class Platform : StaticBody2D
{
    public void Init(Vector2 pos){
        GlobalPosition = pos;
    }
}
