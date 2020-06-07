using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//enemy army class that holds variables for the enemies of each level
[System.Serializable]
public class EnemyArmyLevel{
	public int gridSize;
	public string groupSize;
	public List<EnemyUnit> units;
	public List<EnemyGroup> group;
	public int playerCoins;
	public string scene;
}

[System.Serializable]
public class EnemyGroup
{
	public List<EnemyUnit> units;
    private int _index;
    public int Index
    {
        get
        {
			return _index;
		}
		set
        {
			_index = value;
			SetUnitGroupID(value);
        }
    }

    private void SetUnitGroupID(int id)
    {
		for (int i = 0; i < units.Count; i++)
			units[i].groupID = id;
    }

    public EnemyGroup()
    {
		units = new List<EnemyUnit>();
    }
}

[System.Serializable]
public class EnemyUnit
{
	public GameObject unit;
	public int groupID;
}

[System.Serializable]
public enum PlayMode
{
    SINGLE,GROUP
}

public class LevelData : ScriptableObject {

	[HideInInspector]
	public List<EnemyArmyLevel> levels;
	[HideInInspector]
	public int armyToEdit;
	[HideInInspector]
	public List<Texture2D> customEnemyImages;
	
	//editor variables
	public bool customImages;

	[Space(10)]
	public PlayMode playMode;

	[Space(10)]
	public float spawnDelay;
	public bool grid;
	
	[Space(5)]
	public Color buttonHighlight;
	public Color tileColor;
	public Color invalidPosition;
	public Color removeColor;
	public Color eraseButtonColor;
	public Color selectedPanelColor;
	public Color borderColor;
	
	[Space(5)]
	public float demoCharacterAlpha;
	public float buttonEffectTime;
	public float checkRange;
	
	[Space(5)]
	public int rotationStep;
	public int placeRange;
	public int gridSize;
	
	[Space(5)]
	public KeyCode snappingKey;
	
	[Space(5)]
	public bool spreadUnits;
}
