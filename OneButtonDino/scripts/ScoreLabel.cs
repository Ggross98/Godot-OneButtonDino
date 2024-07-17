using Godot;
using System;

public partial class ScoreLabel : Label
{
    private GameStats gameStats;

    public override void _Ready()
    {
        gameStats = GetNode<GameStats>("/root/GameStats");
    }

    public override void _Process(double delta)
    {
        Text = string.Format("{0} : {1}", gameStats.score0, gameStats.score1);
    }

}
