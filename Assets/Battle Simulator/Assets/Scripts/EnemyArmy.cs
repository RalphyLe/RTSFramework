using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyArmy : MonoBehaviour {
	
	//not visible in the inspector
	private LevelData levelData;
	private PlayMode playMode;
	private List<GameObject> spawnedEnemies = new List<GameObject>();
	private CharacterPlacement characterPlacement;
	
	void Start () {
		//find the level data object and get the current level
		levelData = Resources.Load("Level data") as LevelData;
		int level = PlayerPrefs.GetInt("level");
		playMode = levelData.playMode;
		//also find the character placement script
		characterPlacement = GameObject.FindObjectOfType<CharacterPlacement>();
		UnitGroupManager.Instance.ClearGroups();
		//spawn enemies if this level exists
		if(level < levelData.levels.Count)
			StartCoroutine(spawnEnemies(level));
	}
	
	IEnumerator spawnEnemies(int levelIndex){
		
		//get the gridsize and the space in between grid cells
		int levelGridSize = levelData.levels[levelIndex].gridSize;
		int sizeGrid = 2;
		
		//find the 3d start position for the grid
		Vector3 startPosition = new Vector3(transform.position.x + ((float)sizeGrid * ((float)levelGridSize/2f)), 100, transform.position.z + ((float)sizeGrid * ((float)levelGridSize/2f)));
		int currentPosition = 0;

        if (playMode == PlayMode.GROUP)
        {
			List<EnemyGroup> enemyGroups = levelData.levels[levelIndex].group;
			for(int i = 0; i < enemyGroups.Count; i++)
            {
				UnitGroup unitGroup = UnitGroupManager.Instance.GetValidGroup("enemy");
				var per_group = enemyGroups[i];
				for(int delta = 0; delta < per_group.units.Count; delta++)
                {
					EnemyUnit enemyUnit = per_group.units[delta];
					int offset_x = enemyUnit.index / levelGridSize;
					int offset_z = enemyUnit.index % levelGridSize;
					Vector3 position = new Vector3(startPosition.x - offset_x * sizeGrid, startPosition.y, startPosition.z - offset_z * sizeGrid);
					GameObject unit = enemyUnit.unit;
                    if (unit != null)
                    {
						unitGroup.AddUnit(spawnNew(position, unit));
						yield return new WaitForSeconds(levelData.spawnDelay);
					}
                }
            }
        }
        else
        {
			//for each position in the grid
			for (int x = 0; x < levelGridSize; x++)
			{
				for (int z = 0; z < levelGridSize; z++)
				{
					//get the 3d position and the unit for that position
					Vector3 position = new Vector3(startPosition.x - ((float)x * sizeGrid), startPosition.y, startPosition.z - ((float)z * sizeGrid));
					GameObject unit = levelData.levels[levelIndex].units[currentPosition].unit;

					if (unit != null)
					{
						//if there is a unit/character, spawn it and wait a moment for the spawn effect
						spawnNew(position, unit);
						yield return new WaitForSeconds(levelData.spawnDelay);
					}

					//increase the current position index
					currentPosition++;
				}
			}
		}
	}

	public void OnDrawGizmos()
	{

	}

	//spawn a new enemy
	public Unit spawnNew(Vector3 position, GameObject unit){
		//store the raycast hit
		RaycastHit hit;
		
		if(Physics.Raycast(position, -Vector3.up, out hit)){
			//if the raycast hits a terrain, spawn a unit at the hit point
			GameObject newUnit = Instantiate(unit, hit.point, Quaternion.Euler(0, 90, 0));
			spawnedEnemies.Add(newUnit);
			//disable the unit until the battle starts
			characterPlacement.disableUnit(newUnit);
			return newUnit.GetComponent<Unit>();
		}
		return null;
	}
	

	public void startEnemies(){
		//enable all enemies so they start the battle
		foreach(GameObject enemyUnit in spawnedEnemies){
			characterPlacement.enableUnit(enemyUnit);
		}
	}
}
