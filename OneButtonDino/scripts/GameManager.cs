using Godot;
using System;
using System.Data.Common;

public partial class GameManager : Node2D
{   
    PackedScene playerPrefab = GD.Load<PackedScene>("res://scenes/dino.tscn");
    PackedScene swordPrefab = GD.Load<PackedScene>("res://scenes/sword.tscn");
    PackedScene platformPrefab = GD.Load<PackedScene>("res://scenes/platform.tscn");

    private Node2D playerParent, swordParent, platformParent;


    public override void _Ready()
    {

        playerParent = GetNode<Node2D>("PlayerParent");
        swordParent = GetNode<Node2D>("SwordParent");
        platformParent = GetNode<Node2D>("PlatformParent");

        StartGame();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Quit")){
            GetTree().Quit();
        }
        else if(Input.IsActionJustPressed("Replay")){
            Restart();
        }
    }

    private void StartGame(){
        
        float x, y;

        // Create platforms
        for(x = -140; x <= 140; x += 56){
            var flag = GD.RandRange(-1, 3);
            if(flag > -1){
                y = -40f + 20 * flag;

                CreatePlatform(new Vector2(x, y));
            }
        }

        // Create players
        
        x = -140f + GD.RandRange(0, 2) * 56;
        y = -40f + GD.RandRange(0, 1) * 60 + 9f;
        var player0 = CreatePlayer("green", new Vector2(x, y));
        player0.SetDirection(1);
        player0.SetState(Player.PlayerState.Walking);

        x = 140f - GD.RandRange(0, 2) * 56;
        y = -40f + GD.RandRange(0, 1) * 60 + 9f;
        var player1 = CreatePlayer("red", new Vector2(x, y));
        player1.SetDirection(-1);
        player1.SetState(Player.PlayerState.Walking);

        // Create swords
        var sword0 = CreateSword(Vector2.Zero);
        player0.PickUp(sword0);

        var sword1 = CreateSword(Vector2.Zero);
        player1.PickUp(sword1);

        
        

    }

    public void Restart(){
        GetTree().ReloadCurrentScene();
    }

    private Player CreatePlayer(string id, Vector2 pos){
        
        var player = playerPrefab.Instantiate<Player>();
        playerParent.AddChild(player);

        player.Init(id, pos);

        return player;
    }

    private Sword CreateSword(Vector2 pos){

        var sword = swordPrefab.Instantiate<Sword>();
        swordParent.AddChild(sword);

        sword.Init(pos);

        return sword;
    }

    private Platform CreatePlatform(Vector2 pos){
        
        var platform = platformPrefab.Instantiate<Platform>();
        platformParent.AddChild(platform);

        platform.Init(pos);

        return platform;

    }

}
