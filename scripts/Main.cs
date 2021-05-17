using Godot;
using System;
using System.Collections.Generic;

public class Main : Node
{
    [Export] PackedScene enemyScene;

    Godot.Collections.Array<Enemy> enemies = new Godot.Collections.Array<Enemy>();

    int maxEnemySpawn = 0;

    const int startLevel = 0;
    int level;

    // level 1 : bullet hell
    // level 2 : bullet hell + static enemy
    // level 3 : bullet hell + bouncing enemy

    Timer waveTimer;
    Timer delayTimer;
    float waveWaitTime = 20f;
    float delayWaitTime = 5f;

    Label interactionLabel;
    string interaction = "Moving on to Wave ";

    Label timerLabel;
    float minutes;
    float seconds;
    string minuteString;
    string secondsString;

    Label waveCountLabel;
    string wave = "Wave ";

    bool isOnPlay = false;

    public override void _Ready()
    {
        GD.Randomize();

        waveTimer = GetNode<Timer>("Timers/WaveTimer");
        delayTimer = GetNode<Timer>("Timers/DelayTimer");

        interactionLabel = GetNode<Label>("InteractionLabel");

        timerLabel = GetNode<Label>("HUD/TimerLabel");
        waveCountLabel = GetNode<Label>("HUD/WaveCountLabel");

        level = startLevel;
        waveTimer.WaitTime = waveWaitTime;
        delayTimer.WaitTime = delayWaitTime;
        delayTimer.Start();

        interactionLabel.Text = interaction + (level+1).ToString();
    }

    public override void _Process(float delta)
    {
        if(level > 1) {
            // instance 3 enemies
            if(enemies.Count < maxEnemySpawn) {
                for (int i = 0; i < maxEnemySpawn; i++) {
                    var enemy = (Enemy) enemyScene.Instance();
                    // set random spawn position in viewport for each enemy
                    // set enemies position to random position
                    enemy.Position = new Vector2((float) GD.RandRange(112f, 912f), (float) GD.RandRange(100f, 500f));

                    if(level > 2) {
                        enemy.SetSpeed((float) GD.RandRange(100f, 300f));
                    }

                    enemies.Add(enemy);
                    // add child to main
                    GetNode<Main>("/root/Main").AddChild(enemy);
                }
            }
        }

        if(isOnPlay) {
            minutes = Mathf.Floor(waveTimer.TimeLeft / 60);
            seconds = Mathf.Floor(waveTimer.TimeLeft % 60);
        }
        else {
            minutes = Mathf.Floor(delayTimer.TimeLeft / 60);
            seconds = Mathf.Floor(delayTimer.TimeLeft % 60);
        }

        if(minutes < 10) {
            minuteString = "0" + minutes;
        }
        else {
            minuteString = minutes.ToString();
        }

        if(seconds < 10) {
            secondsString = "0" + seconds;
        } 
        else {
            secondsString = seconds.ToString();
        }

        timerLabel.Text = minuteString + " : " + secondsString;

        // as wave progress, bullet spawn rate up and bullet speed down / up, enemy spawn up, enemy movement rate up

        GD.Print("wavetimer: " + Mathf.Floor(waveTimer.TimeLeft));
        GD.Print("delaytimer: " + Mathf.Floor(delayTimer.TimeLeft));

    }

    // on wavetimer timeout, clean enemy and bullets, start delaytimer
    void _on_WaveTimer_timeout() {
        isOnPlay = false;

        GetTree().CallGroup("enemy", "queue_free");
        GetTree().CallGroup("bullet", "queue_free");

        delayTimer.Start();
        GetNode<Spawner>("SpawnerPath/Spawner").StopTimer();
        
        interactionLabel.Text = interaction + (level+1).ToString();
        interactionLabel.PercentVisible = 1;

        waveCountLabel.Text = wave + (level+1).ToString();
    }

    // on delaytimer timeout, level++, start wavetimer
    void _on_DelayTimer_timeout() {
        isOnPlay = true;
        level++;
        enemies.Clear();

        if(level < 5) {
            maxEnemySpawn = level;
        }
        else {
            maxEnemySpawn = 4;
        }

        interactionLabel.PercentVisible = 0;
        waveTimer.WaitTime += 10f;
        waveTimer.Start();
        GetNode<Spawner>("SpawnerPath/Spawner").StartTimer();
    }

    public int GetLevel() {
        return level;
    }
}
