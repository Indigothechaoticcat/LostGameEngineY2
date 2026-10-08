using UnityEngine;

public class Enemy
{
	public int health = 180;
	public string enemyName;
	
	public void TakeDamage(int amount)
	{
		health -= amount;
	}
}

Enemy goblin = new Enemy();
goblin.enemyName = "Goblin";
goblin.TakeDamage(10);

