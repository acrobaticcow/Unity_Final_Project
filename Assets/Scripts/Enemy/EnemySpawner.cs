using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
	public enum SpawnType { RoundRobin, Random }
	public Transform Target;
	public int SpawnCount;
	public int SpawnDelay;
	public List<Enemy> EnemyPrefabs = new();
	public SpawnType SpawnMethod = SpawnType.RoundRobin;
	NavMeshTriangulation triangulation;
	Dictionary<int, ObjectPool> EnemyObjectPools = new();
	void Awake()
	{
		for (int i = 0; i < EnemyPrefabs.Count; i++)
		{
			EnemyObjectPools.Add(i, ObjectPool.CreateInstance(EnemyPrefabs[i], SpawnCount));
		}
	}
	void Start()
	{
		triangulation = NavMesh.CalculateTriangulation();
		StartCoroutine(SpawnEnemies());
	}

	private IEnumerator SpawnEnemies()
	{
		int currentCount = 0;
		WaitForSeconds wait = new(SpawnDelay);

		while (currentCount < SpawnCount)
		{
			switch (SpawnMethod)
			{
				case SpawnType.RoundRobin:
					RoundRobinSpawnMethod(currentCount);
					break;
				case SpawnType.Random:
					RandomSpawnMethod(Random.Range(0, EnemyPrefabs.Count));
					break;
			}

			currentCount++;
			yield return wait;
		}
	}

	private void RandomSpawnMethod(int enemyPrefabsIndex)
	{
		SpawnEnemy(enemyPrefabsIndex);
	}

	private void RoundRobinSpawnMethod(int currentSpawnCount)
	{
		int enemyPrefabsIndex = currentSpawnCount % EnemyPrefabs.Count;
		SpawnEnemy(enemyPrefabsIndex);
	}

	private void SpawnEnemy(int enemyPrefabsIndex)
	{
		PoolableObject poolObject = EnemyObjectPools[enemyPrefabsIndex].GetObject();

		if (poolObject != null)
		{
			Enemy enemy = poolObject.GetComponent<Enemy>();


			int navMeshIndex = Random.Range(0, triangulation.vertices.Length);


			NavMeshHit hit;
			if (NavMesh.SamplePosition(triangulation.vertices[navMeshIndex], out hit, 2f, -1))
			{
				enemy.Agent.Warp(hit.position);
				enemy.Agent.enabled = true;
				enemy.Movement.Target = Target;
				enemy.Movement.StartChasing();
			}
			else
			{
				Debug.LogError($"Unable to place enemy on navMesh, tried to use {triangulation.vertices[navMeshIndex]}");
			}
		}
		else
		{
			Debug.LogError("Unable to fetch enemy");
		}

	}
}
