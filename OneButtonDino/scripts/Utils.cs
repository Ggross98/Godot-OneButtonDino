using Godot;
using System;

public partial class Utils
{

    public static float DegreesToRadians(float degrees){
        return (float)(degrees * (Math.PI / 180));
    }
}
