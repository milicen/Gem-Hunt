using Godot;
using System;
using System.Collections.Generic;

public class Pool : Node
{
    [Export] PackedScene enemyScene;
    int maxEnemy = 10;

    Queue<Enemy> enemyQueue = new Queue<Enemy>();
    
    public override void _Ready()
    {
        
        for(int i = 0; i < maxEnemy; i++) {
            var enemy = (Enemy) enemyScene.Instance();
            enemyQueue.Enqueue(enemy);
            AddChild(enemy);
        }

        GetNode<Timer>("Timer").Start();

    }

    void _on_Timer_timeout() {
        var enemy = enemyQueue.Dequeue();

        var randX = (float) GD.RandRange(200, 824);
        var randY = (float) GD.RandRange(100, 500);
        var position = new Vector2(randX, randY);

        enemy.Init(position);
        enemyQueue.Enqueue(enemy);

        GD.Print(enemy.Name + " requeued");
    }

}
