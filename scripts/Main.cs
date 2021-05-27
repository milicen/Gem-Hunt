using Godot;
using System;
using System.Collections.Generic;

public class Main : Node
{

    // export array of enemy types
    // [Export] Godot.Collections.Array<PackedScene> enemyTypes = new Godot.Collections.Array<PackedScene>();

    // in ready function :
    // loop through every enemy types
    // for each type, make 10 instance of enemy
    // add each of them into a specified array (queue)

    // make a dictionary that will contain all queues
    // public Dictionary<string, Queue<Enemy>> enemyPool = new Dictionary<string, Queue<Enemy>>();

    [Export] PackedScene enemyScene;

    Godot.Collections.Array<Enemy> enemies = new Godot.Collections.Array<Enemy>();

    int maxEnemySpawn = 0;

    const int startLevel = 0;
    int level;
    int points;

    // level 1 : bullet hell
    // level 2 : bullet hell + static enemy
    // level 3 : bullet hell + bouncing enemy

    Timer waveTimer;
    Timer delayTimer;
    float waveWaitTime = 20f;
    float delayWaitTime = 5f;

    Label pointsLabel;

    Label interactionLabel;
    string interaction = "Moving on to Wave ";
    string retry = "Press R to retry";

    Label timerLabel;
    float minutes;
    float seconds;
    string minuteString;
    string secondsString;

    Label waveCountLabel;
    string wave = "Wave ";

    bool isOnPlay = false;

    Vector2 screenCenter;

    public override void _Ready()
    {
        GD.Randomize();

        screenCenter = GetViewport().Size / 2;

        waveTimer = GetNode<Timer>("Timers/WaveTimer");
        delayTimer = GetNode<Timer>("Timers/DelayTimer");

        interactionLabel = GetNode<Label>("InteractionLabel");

        timerLabel = GetNode<Label>("HUD/TimerLabel");
        waveCountLabel = GetNode<Label>("HUD/WaveCountLabel");
        pointsLabel = GetNode<Label>("HUD/PointsLabel");

        pointsLabel.Text = "Gems " + points;

        level = startLevel;
        waveTimer.WaitTime = waveWaitTime;
        delayTimer.WaitTime = delayWaitTime;
        delayTimer.Start();

        interactionLabel.Text = interaction + (level+1).ToString();

        // make enemy pool
        // for(int i = 0; i < enemyTypes.Count; i++) {
        //     Queue<Enemy> enemyQueue = new Queue<Enemy>();
        //     for(int size = 0; size < 10; size++) {
        //         var enemy = (Enemy) enemyTypes[i].Instance();
        //         // set position
        //         // set rotation
        //         enemy.Name = "enemy" + (i+1).ToString() + (size+1).ToString();
        //         enemyQueue.Enqueue(enemy);
        //         AddChild(enemy);           
        //     }
        //     enemyPool.Add("enemy-type-1", enemyQueue);
            
        // }


    }

    public override void _Process(float delta)
    {
        if(level >= 1) {
            // instance 3 enemies
            if(enemies.Count < maxEnemySpawn) {
                for (int i = 0; i < maxEnemySpawn; i++) {
                    var enemy = (Enemy) enemyScene.Instance();
                    // set random spawn position in viewport for each enemy
                    // set enemies position to random position
                    enemy.Position = new Vector2((float) GD.RandRange(112f, 912f), (float) GD.RandRange(100f, 500f));

                    // if(level > 2) {
                        enemy.SetSpeed((float) GD.RandRange(100f, 300f));
                    // }

                    enemies.Add(enemy);
                    // add child to main
                    AddChild(enemy);
                }
            }
        }

        // if(enemies.Count < maxEnemySpawn) {
        //     for(int i = 0; i < maxEnemySpawn; i++) {
        //         var enemy = enemyPool["enemy-type-1"].Dequeue();
        //         enemy.Visible = true;
        //         enemy.Position = new Vector2((float) GD.RandRange(112f, 912f), (float) GD.RandRange(100f, 500f));
        //         enemy.Rotation = (float) GD.RandRange(0, 2 * Mathf.Pi);

        //         if(level > 2) {
        //             enemy.SetSpeed((float) GD.RandRange(100f, 300f));
        //         }

        //     }
        // }

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

        pointsLabel.Text = "Gems " + points;

        timerLabel.Text = minuteString + " : " + secondsString;

        if(!GetNode<Player>("Player").Visible) {
            isOnPlay = false;
            ClearLevel();
            interactionLabel.Text = retry;
            interactionLabel.PercentVisible = 1;
            GetNode<Spawner>("SpawnerPath/Spawner").StopTimer();
            waveTimer.Stop();
            delayTimer.Stop();
            // SetProcess(false);
        }

        // as wave progress, bullet spawn rate up and bullet speed down / up, enemy spawn up, enemy movement rate up

    }

    public override void _UnhandledInput(InputEvent @event) {
        if(Input.IsActionJustPressed("restart")) {
            // restart all variables and timer
            GetNode<Player>("Player").Show();
            GetNode<Player>("Player").CenterPosition(screenCenter);
            GetNode<Boomerang>("Boomerang").Show();
            level = 0;
            points = 0;
            waveTimer.WaitTime = waveWaitTime;
            delayTimer.WaitTime = delayWaitTime;
            interactionLabel.Text = interaction + (level + 1);

            delayTimer.Start();
        }
    }

    // on wavetimer timeout, clean enemy and bullets, start delaytimer
    void _on_WaveTimer_timeout() {
        isOnPlay = false;

        ClearLevel();

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
            maxEnemySpawn = 5;
        }

        interactionLabel.PercentVisible = 0;
        waveTimer.WaitTime += 10f;
        waveTimer.Start();
        GetNode<Spawner>("SpawnerPath/Spawner").StartTimer();
    }

    public int GetLevel() {
        return level;
    }

    public void SetPoint() {
        points++;
    }

    void ClearLevel() {
        GetTree().CallGroup("enemy", "queue_free");
        GetTree().CallGroup("bullet", "queue_free");
        GetTree().CallGroup("gem", "queue_free");
    }
}
