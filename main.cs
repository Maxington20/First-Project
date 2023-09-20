using Godot;
using System;

public partial class main : Node
{
    [Export]
    public PackedScene MobScene {get; set;}

    private int _score;

    public void GameOver()
    {
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<hud>("HUD").ShowGameOver();
    }

    public void NewGame()
    {    
        // play music
        GetNode<AudioStreamPlayer>("Music").Play();

        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        _score = 0;

        var hud = GetNode<hud>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");


        var player = GetNode<player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
        GetNode<hud>("HUD").UpdateScore(_score);
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnMobTimerTimeout()
    {
        // Note: Normally it is best to use explicit types rather than the `var`
        // keyword. However, var is acceptable to use here because the types are
        // obviously Mob and PathFollow2D, since they appear later on the line.

        // Create a new instance of the Mob scene.
        mob mob = MobScene.Instantiate<mob>();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // Set the mob's direction perpendicular to the path direction.
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }

    public override void _Ready()
    {
        
    }
}
