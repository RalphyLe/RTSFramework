﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using System.Net.Http.Headers;
using System;

//troop class so we can build different troops/characters
[System.Serializable]
public class Troop{
	public GameObject deployableTroops;
	public int troopCosts;
	public Sprite buttonImage;
	
	[HideInInspector]
	public GameObject button;
}

public class CharacterPlacement : MonoBehaviour {
	
	//variables visible in the inspector
	[Header("Objects:")]
	public Animator leftPanelAnimator;
	public Animator endPanel;
	public Animator buttonsAnimator;
	public Animator gamePanel;
	public Animator grid;
	public Animator transition;
	public Animator cameraAnimator;
	
	[Space(5)]
	public GameObject characterPanel;
	public GameObject button;
	public GameObject indicator;
	public GameObject characterStatsPanel;
	public GameObject topDownMapPanel;
	public GameObject gridCell;
	public GameObject gridButton;
	
	[Space(5)]
	public Image eraseButton;
	public Image statsButton;
	public Image topDownButton;
	public Image battleIndicator;
	
	[Space(5)]
	public Text statsName;
	public Text statsDamage;
	public Text statsHealth;
	public Text statsRange;
	public Text statsSpeed;
	public Text coinsText;
	public Text levelInfo;
	public Text gridButtonText;
	
	[Space(5)]
	public Dropdown speedSetting;
	
	[Space(5)]
	public Transform gridPanel;
	public Transform gridArrow;
	
	[Header("Troops:")]
	public List<Troop> troops;
	
	//not visible in the inspector
	private int selected;
	private GameObject currentDemoCharacter;
	private UnitGroup currentGroup;
	private int rotation = -90;
	private List<GameObject> placedUnits = new List<GameObject>();
	
	private bool erasing;
	private Color eraseStartColor;
	private int coins;
	private bool battleStarted;
	private bool erasingUsingKey;
	private LevelData levelData;
	private bool characterStats;
	private Vector3 gridCenter;
	private GameObject border;
	
	private int gridSize;
	private int tileSize = 2;
	private Rect placeArea;
	
	void Awake(){
		//get the level data object and check if we're using mobile controls
		levelData = Resources.Load("Level data") as LevelData;
		
		//double the grid size so it's always even
		gridSize = levelData.gridSize * tileSize;
		if (levelData.playMode == PlayMode.SINGLE)
			levelData.groupSize = Vector2.one;
		//get the grid center by taking the opposite of the the enemy army position
		//gridCenter = GameObject.FindObjectOfType<EnemyArmy>().gameObject.transform.position;
		gridCenter = GridManager.Instance.placeCenter;
		placeArea = new Rect(new Vector2(0, 0 - gridSize * 2), Vector2.one * gridSize);


		//if we're using the grid, create a 3D border and a 2D grid
		if(levelData.grid){
			GridManager.Instance.InitilizeGrid(gridCenter, gridSize);
			createBorder();
			initializeGrid();
		}
		else{
			gridButton.SetActive(false);
		}
		
		//if the level exists, show some level info, else load the end screen
		if(PlayerPrefs.GetInt("level") >= levelData.levels.Count){
			SceneManager.LoadScene("End screen");
		}
		else{
			levelInfo.text = "Level " + (PlayerPrefs.GetInt("level") + 1) + " - " + levelData.levels[PlayerPrefs.GetInt("level")].scene;
		}
	}
	
	//create the 3d border for grid mode
	void createBorder(){
		//get the border start position
		Vector3 borderStart = new Vector3(0, 100, -gridSize);
		//store the current border position (to use during the loop)
		Vector3 current = borderStart;
		
		//create a new gameobject to store the border
		border = new GameObject();
		border.transform.position = gridCenter;
		border.name = "3D grid Border";
		
		//loop through both axis
		for(int z = 0; z <= gridSize; z++){
			for(int x = 0; x <= gridSize; x++){
				//get the edge of the square to place the border
				if(z == 0 || z == gridSize || x == 0 || x == gridSize){
					//store the hit
					RaycastHit hit;
					
					//if there's a terrain at this position..
					if(Physics.Raycast(current, -Vector3.up, out hit)){
						//create a new border object
						GameObject borderPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
						//parent it to the main border object and position it correctly
						borderPoint.transform.SetParent(border.transform, false);
						borderPoint.transform.position = hit.point;
						//remove the collider and change the border material
						Destroy(borderPoint.GetComponent<Collider>());
						Material mat = borderPoint.GetComponent<Renderer>().material;
						mat.shader = Shader.Find("Unlit/UnlitAlphaWithFade");
						mat.color = levelData.borderColor;
						
						if((z == 0 || z == gridSize) && (x == 0 || x == gridSize)){
							//square object for the corners
							borderPoint.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
						}
						else{
							//rectangle for the sides
							if(z == 0 || z == gridSize){
								borderPoint.transform.localScale = new Vector3(0.7f, 0.2f, 0.1f);
							}
							else{
								borderPoint.transform.localScale = new Vector3(0.1f, 0.2f, 0.7f);
							}
						}
					}
				}
				
				//change the current border position on the x axis
				current = new Vector3(current.x + tileSize, current.y, current.z);
			}
			
			//change the z axis for the current border position
			current = new Vector3(borderStart.x, current.y, current.z + tileSize);
		}
	}
	
	//create the 2d grid
	void initializeGrid(){
		//find the grid layout group component
		GridLayoutGroup gridGroup = gridPanel.GetComponent<GridLayoutGroup>();
		
		//calculate the spacing and cell size based on the grid size
		gridGroup.cellSize = new Vector2(400f/gridSize, 400f/gridSize);
		gridGroup.spacing = new Vector2(2.6f/(gridSize * 1.05f) * 20f, 2.6f/(gridSize * 1.1f) * 20f);
		
		//loop through all cells
		for(int i = 0; i < (gridSize * gridSize); i++){
			//create the cell and parent it to the grid
			GameObject cell = Instantiate(gridCell);
			cell.transform.SetParent(gridPanel, false);
			cell.transform.GetChild(0).gameObject.SetActive(false);
			
			//set the cell name to its index
			cell.transform.name = "" + i;
			
			//add a onclick function to the cell
			cell.GetComponent<Button>().onClick.AddListener(
			() => { 
				gridClick(int.Parse(cell.transform.name), cell); 
			}
			);
		}
		
		//place the red arrow at the bottom of the grid hierarchy so it doesn't change the place index
		gridArrow.SetSiblingIndex(gridSize * gridSize);
	}
	
	void Start(){
		//get the erase button color and store it
		eraseStartColor = eraseButton.color;
		
		//show the character buttons in the left panel
		StartCoroutine(addCharacterButtons());
		
		//get the coins for this level and show them
		coins = levelData.levels[PlayerPrefs.GetInt("level")].playerCoins;
		coinsText.text = coins + "";
		
		//initialize some boolean values
		characterStats = true;
		switchPanelContent(false);
		
		characterStatsPanel.SetActive(false);
		topDownMapPanel.SetActive(true);
		setStats(0);
	}
	
	void Update(){
		//if the battle has started
		if(battleStarted){
			//check if the enemies or allies are all dead and if that's the case, end the game
			if(GameObject.FindGameObjectsWithTag("Knight").Length == 0){
				endPanel.SetTrigger("defeat");
				
				endGame();
			}
			else if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0){
				endPanel.SetTrigger("victory");
				PlayerPrefs.SetInt("level" + (PlayerPrefs.GetInt("level") + 1), 1);
				PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
				
				endGame();
			}
			
			//get the current battle status to show in the indicator
			float fill = BattleStatus();
			
			//change the indicator fill based on the status
			if(battleIndicator.fillAmount < fill){
				battleIndicator.fillAmount += Time.deltaTime * 0.1f;
				
				if(battleIndicator.fillAmount >= fill)
					battleIndicator.fillAmount = fill;
			}
			else if(battleIndicator.fillAmount > fill){
				battleIndicator.fillAmount -= Time.deltaTime * 0.1f;
				
				if(battleIndicator.fillAmount <= fill)
					battleIndicator.fillAmount = fill;
			}
		}
		else if(GameObject.FindGameObjectsWithTag("Knight").Length == 0){
			battleIndicator.fillAmount -= Time.deltaTime * 0.5f;
		}
		else if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0){
			battleIndicator.fillAmount += Time.deltaTime * 0.5f;
		}
		
		
		//remove the demo character when hiding the left character panel
		if(leftPanelAnimator.gameObject.activeSelf && leftPanelAnimator.GetBool("hide panel")){
			if(currentDemoCharacter)
				Destroy(currentDemoCharacter);
			
			//return so it will not use the demo
			return;
		}
		
		//check for the x key to erase characters
		if((Input.GetKeyDown("x") && !erasing) || (Input.GetKeyUp("x") && erasingUsingKey)){
			erasingUsingKey = !erasingUsingKey;
			erasingMode();
		}
		
		//if there is a demo character on the battlefield
		if(currentDemoCharacter){
			//get the position of the mouse relative to the terrain
			Vector3 position = getPosition();
			int tileIndex = -1;
			if (levelData.grid)
            {
				Vector3 offset = position - currentDemoCharacter.transform.position;
				for (int childId = 0; childId < currentDemoCharacter.transform.childCount; childId++)
				{
					Vector3 demoPos = currentDemoCharacter.transform.GetChild(childId).position;
					Vector3 demo_offset = demoPos + offset;
					tileIndex = GridManager.Instance.GetTileWithPos(demo_offset);
					if (tileIndex != -1)
					{
						GridManager.Instance.curIndex = tileIndex;
						Vector2 tilePos = GridManager.Instance.GetTilePos(tileIndex);
						position.x = tilePos.x - demoPos.x + currentDemoCharacter.transform.position.x;
						position.z = tilePos.y - demoPos.z + currentDemoCharacter.transform.position.z;
						break;
					}
				}
			}
			//move the demo with the mouse 
			currentDemoCharacter.transform.position = position;
			
			//if we're not currently erasing characters
			if(!erasing){	
				//use right mouse button to rotate the character			
				if(Input.GetMouseButtonDown(1)){
					rotation += levelData.rotationStep;
					updateRotation(currentDemoCharacter);
				}

				//place a unit when the left mouse button is down
				if (Input.GetMouseButton(0) && position.x > 0)
                {
                    if (groupCanPlace())
                    {
						if(levelData.playMode==PlayMode.GROUP)
							currentGroup = UnitGroupManager.Instance.GetValidGroup("player");
						for(int placeId = 0; placeId < currentDemoCharacter.transform.childCount; placeId++)
                        {
							Transform demo = currentDemoCharacter.transform.GetChild(placeId);
							if (levelData.grid)
							{
								int demoTileId = GridManager.Instance.GetTileWithPos(demo.position);
								placeUnitInTile(demoTileId, demo.position, false);
							}
							else
								placeUnit(demo.position, false);
                        }
                    }
                }
					//placeUnitInTile(tileIndex,position, false);//placeUnit(position, false);
				
				//get a color for the demo character and change it based on the validity of the current mouse position
				Color color = Color.white;
				if(!groupCanPlace() || position.x < 0 || troops[selected].troopCosts * levelData.groupSize.x * levelData.groupSize.y > coins || 
					Vector3.Distance(Camera.main.transform.position, position) > levelData.placeRange || 
					!withinGrid(position)){
					color = levelData.invalidPosition;
				}
				else{
					color = levelData.tileColor;
				}
				for(int i = 0; i < currentDemoCharacter.transform.childCount; i++)
                {
					//change the indicator color
					foreach (Renderer renderer in currentDemoCharacter.transform.GetChild(i).Find("Indicator(Clone)").GetComponentsInChildren<Renderer>())
					{
						renderer.material.color = color;
					}
				}
			}
			else if(Input.GetMouseButton(0) || erasingUsingKey){
				//if we're erasing, check for left mouse button to erase units/characters
				if (levelData.playMode == PlayMode.GROUP)
                {
					Unit unit = unitsInRange(position).GetComponent<Unit>();
					eraseGroupUnit(unit.groupId, false, false);
				}
				else
                {
					if (levelData.grid)
						eraseUnitInTile(tileIndex, position, false, false);
					else
						eraseUnit(position, false, false);
				}
			}
			
			//if the demo character is not playing idle animations, make sure to play idle animations on all of its animators
			if(currentDemoCharacter.activeSelf && currentDemoCharacter.GetComponent<Animator>() && currentDemoCharacter.GetComponent<Animator>().GetBool("Start") != false){
				foreach(Animator animator in currentDemoCharacter.GetComponentsInChildren<Animator>()){
					animator.SetBool("Start", false);
				}
			}
		}
	}
	
	//check if the position is within the 3D grid
	bool withinGrid(Vector3 position){
		//if we're not using any grid, it's inside the grid by default
		if(!levelData.grid)
			return true;

		//else, compare the position to the grid
		if (position.x > gridCenter.x || position.x < gridCenter.x - 2 * gridSize || position.z < gridCenter.z - gridSize || position.z > gridCenter.z + gridSize)
			return false;
		
		return true;
	}
	

	bool groupCanPlace()
    {
		if (currentDemoCharacter == null)
			return false;
		for(int i = 0; i < currentDemoCharacter.transform.childCount; i++)
        {
			var demoPos = currentDemoCharacter.transform.GetChild(i).position;
			if (!canPlace(demoPos, false))
				return false;

		}
		return true;
    }

	void placeUnitInTile(int index,Vector3 pos,bool flag)
	{
		if (GridManager.Instance.HasPlaceUnit(index) || index == -1)
			return;
		else
			placeUnit(pos, flag);
	}

	//calculate the battle status by comparing the number of enemies vs the number of allies
	float BattleStatus(){
		int knightsLeft = GameObject.FindGameObjectsWithTag("Knight").Length;
		int enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
		int total = knightsLeft + enemiesLeft;
		
		return (float)knightsLeft/(float)total;
	}
	
	//change erasing mode
	public void erasingMode(){
		erasing = !erasing;
		
		if(erasing){
			//if we're erasing, don't display a character
			if(currentDemoCharacter)
				Destroy(currentDemoCharacter);
			currentDemoCharacter = new GameObject();
			//instead of the character, just show the red tile
			newTile(levelData.removeColor).transform.parent = currentDemoCharacter.transform;
			eraseButton.color = levelData.eraseButtonColor;
		}
		else{
			//if we're not erasing anymore, create a new demo character
			changeDemo();
			eraseButton.color = eraseStartColor;
		}
	}
	
	//place a new unit
	public void placeUnit(Vector3 position, bool placingGridCell){
		//check if the position is valid
		if(canPlace(position, placingGridCell)){
			//create a new unit/character and prevent it from moving
			GameObject unit = Instantiate(troops[selected].deployableTroops, position, Quaternion.identity);
			disableUnit(unit);
            if (levelData.grid)
            {
				var unitComp = unit.GetComponent<Unit>();
				GridManager.Instance.SetCurrentTileUnit(position, unitComp);
			}

			//set the correct rotation
			updateRotation(unit);
			
			//add it to the list of placed units
			placedUnits.Add(unit);
			if (levelData.playMode == PlayMode.GROUP)
				currentGroup.AddUnit(unit.GetComponent<Unit>());
			//decrease the number of coins left
			coins -= troops[selected].troopCosts;
			coinsText.text = coins + "";
		}
	}

	//check if the character can be placed at this position
	bool canPlace(Vector3 position, bool placingGridCell){
		//check if there's units too close to the current position
		if(unitsInRange(position) != null || troops[selected].troopCosts > coins || !withinGrid(position))
			return false;
		
		//check if we're within the maximum place range
		if(!placingGridCell && (EventSystem.current.IsPointerOverGameObject() || Vector3.Distance(Camera.main.transform.position, position) > levelData.placeRange))
			return false;
			
		return true;
	}
	
	//translate a 3d position to a 2d grid index
	int positionToGridIndex(Vector3 position){
		position = new Vector3(Mathf.RoundToInt(position.x) - gridCenter.x, position.y, Mathf.RoundToInt(position.z)  - gridCenter.z);
		int index = 0;
		index += (int)(Mathf.Abs(-(gridSize - 1) - position.z)/2);
		index += (int)(gridSize * Mathf.Abs(-(gridSize - 1) - position.x)/2);
		return index;
	}
	
	//translate a 2d grid index to a 3d position
	Vector3 gridIndexToPosition(int index){
		Vector2 tilePos = GridManager.Instance.GetTilePos(index);
		Vector3 position = new Vector3(tilePos.x, 100, tilePos.y);

		Debug.Log(position);

		RaycastHit hit;
		if(Physics.Raycast(position, -Vector3.up, out hit))
			return hit.point;
		
		return Vector3.zero;
	}
	
	//called when you click anywhere in the grid
	public void gridClick(int clickedIndex, GameObject cell){
		//get the 3d position of this click
		Vector3 position = gridIndexToPosition(clickedIndex);
		
		//if there's a unit already, remove it. Else, add a new one
		if(GridManager.Instance.HasPlaceUnit(clickedIndex)){
            if (levelData.playMode == PlayMode.GROUP)
            {
				Unit unit = GridManager.Instance.grids[clickedIndex].unit;
				int[] cellIndices = UnitGroupManager.Instance.groups[unit.groupId].GetUnitsTileIndex();
				for (int cellId = 0; cellId < cellIndices.Length; cellId++)
				{
					gridPanel.GetChild(cellIndices[cellId]).GetComponent<Image>().color = Color.white;
				}
				eraseGroupUnit(unit.groupId, false, true);
				return;
			}
			cell.GetComponent<Image>().color = Color.white;
			eraseUnit(position, false, true);
		}
		else{
            if (levelData.playMode == PlayMode.GROUP)
            {
				for (int cellId = 0; cellId < levelData.groupSize.x * levelData.groupSize.y; cellId++)
                {
					int clickCellId = clickedIndex + (int)(cellId / levelData.groupSize.x) * gridSize + (int)(cellId % levelData.groupSize.x);
					if (GridManager.Instance.HasPlaceUnit(clickCellId))
						return;
                }
				currentGroup = UnitGroupManager.Instance.GetValidGroup("player");
				for (int cellId = 0; cellId < levelData.groupSize.x * levelData.groupSize.y; cellId++)
				{
					int clickCellId = clickedIndex + (int)(cellId / levelData.groupSize.x) * gridSize + (int)(cellId % levelData.groupSize.x);
					Vector3 pos = gridIndexToPosition(clickCellId);
					placeUnitInTile(clickCellId, pos, true);
					gridPanel.GetChild(clickCellId).GetComponent<Image>().color = Color.red;
                }
                return;
			}
			cell.GetComponent<Image>().color = Color.red;
			placeUnit(position, true);
		}
	}

	void eraseUnitInTile(int index, Vector3 position, bool clearing, bool erasingGridCell)
	{
		if (GridManager.Instance.HasPlaceUnit(index))
		{
			GridManager.Instance.grids[index].unit = null;
			eraseUnit(position, clearing, erasingGridCell);
		}
	}

	void eraseGroupUnit(int groupId,bool clearing,bool erasingGridCell)
    {
		UnitGroup group = UnitGroupManager.Instance.groups[groupId];
		for (int i = 0; i < group.units.Count; i++)
			eraseUnit(group.units[i].transform.position, clearing, erasingGridCell);
		group.RemoveAllUnit();
	}

	public void eraseUnit(Vector3 position, bool clearing, bool erasingGridCell){
		//get the unit to erase
		GameObject unit = unitsInRange(position);
		
		//check if the unit exists and if it's not an enemy
		if(unit != null && unit.name.Length - 7 > 0 && unit.gameObject.tag != "Enemy" && (!EventSystem.current.IsPointerOverGameObject() || clearing || erasingGridCell)){
			if(!clearing)
				placedUnits.Remove(unit);
			
			//remove the unit
			Destroy(unit);
			
			//give the player back his coins
			coins += troops[unitIndex(unit)].troopCosts;
			coinsText.text = coins + "";
		}
	}
	
	//get the index in the troops list for this unit
	int unitIndex(GameObject unit){
		for(int i = 0; i < troops.Count; i++){
			if(troops[i].deployableTroops.name == unit.name.Substring(0, unit.name.Length - 7))
				return i;
		}
		
		return 0;
	}
	
	//get all units in range of a certain position
	public GameObject unitsInRange(Vector3 position){
		//store the units in an array
		Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
		
		//foreach unit, check if it's in range and return as soon as one of them is
		foreach(Unit unit in allUnits){
			if(Vector3.Distance(unit.gameObject.transform.position, position) < levelData.checkRange && !unitIsDemoCharacter(unit.gameObject))
				return unit.gameObject;
		}
		
		//after checking all units, return null
		return null;
	}
	
	public bool unitIsDemoCharacter(GameObject unit)
    {
		int childCount = currentDemoCharacter.transform.childCount;
		for(int i = 0; i < childCount; i++)
        {
			if (unit == currentDemoCharacter.transform.GetChild(i).gameObject)
				return true;
        }
		return false;
    }

	//hide or show the panel on the left
	public void showHideLeftPanel(){
		leftPanelAnimator.SetBool("hide panel", !leftPanelAnimator.GetBool("hide panel"));
		if (!leftPanelAnimator.GetBool("hide panel"))
			changeDemo();
	}
	
	IEnumerator addCharacterButtons(){
		//for all troops...
		for(int i = 0; i < troops.Count; i++){
			//add a button to the list of buttons
			GameObject newButton = Instantiate(button);
			RectTransform rectTransform = newButton.GetComponent<RectTransform>();
			rectTransform.SetParent(characterPanel.transform, false);
			
			//set button outline
			newButton.GetComponent<Outline>().effectColor = levelData.buttonHighlight;
			
			//set the correct button sprite
			newButton.gameObject.GetComponent<Image>().sprite = troops[i].buttonImage;
			
			//only enable outline for the first button
			if(i == 0){
				newButton.GetComponent<Outline>().enabled = true;
			}
			else{
				newButton.GetComponent<Outline>().enabled = false;	
			}
			
			//set button name to its position in the list
			newButton.transform.name = "" + i;
			
			newButton.GetComponentInChildren<Text>().text = "" + troops[i].troopCosts;
			
			//this is the new button
			troops[i].button = newButton;
			
			//wait to create the button spawn effect
			yield return new WaitForSeconds(levelData.buttonEffectTime/(float)troops.Count);
		}
		
		//update the demo character
		changeDemo();
	}
	
	public void selectTroop(int index){
		//remove all outlines and set the current button outline visible
		for(int i = 0; i < troops.Count; i++){
			troops[i].button.GetComponent<Outline>().enabled = false;	
		}
		troops[index].button.GetComponent<Outline>().enabled = true;
		
		//update the selected unit
		selected = index;
		
		//stop erasing
		erasing = false;
		
		//update the demo character
		changeDemo();
		
		eraseButton.color = Color.white;
		
		//change the character statistics
		setStats(index);
	}
	
	public void changeDemo(){
		//if there is one, remove the current demo
		if(currentDemoCharacter)
			Destroy(currentDemoCharacter);
		Debug.Log("ChangeDemo!");
		//create a new demo and name and tag it
		//currentDemoCharacter = Instantiate(troops[selected].deployableTroops);
		//currentDemoCharacter.name = "demo";
		//currentDemoCharacter.tag = "Untagged";
		currentDemoCharacter = new GameObject("demo");
		currentDemoCharacter.transform.eulerAngles = Vector3.zero;
		int size = 1;
		if (levelData.playMode == PlayMode.GROUP)
			size = (int)(levelData.groupSize.x * levelData.groupSize.y);
		Vector3 offset = new Vector3((levelData.groupSize.y - 1) * tileSize / 2, 0, (levelData.groupSize.x - 1) / 2 * tileSize) * (-1);
		for(int i = 0; i < size; i++)
        {
			GameObject demo = Instantiate(troops[selected].deployableTroops,currentDemoCharacter.transform);
			demo.name = "demo" + i;
			demo.tag = "Untagged";
			int offset_x = (int)(i / levelData.groupSize.x);
			int offset_y = (int)(i % levelData.groupSize.x);
			Vector3 releventPos = new Vector3(offset_x * tileSize, 0, offset_y * tileSize);
			demo.transform.localPosition = offset + releventPos;
			//disable the new demo so it doesn't move around using the navmesh
			disableUnit(demo);
			//change the demo colors
			foreach (Renderer renderer in demo.GetComponentsInChildren<Renderer>())
			{
				foreach (Material material in renderer.materials)
				{
					material.shader = Shader.Find("Unlit/UnlitAlphaWithFade");
					float colorStrength = (material.color.r + material.color.g + material.color.b) / 3f;
					material.color = new Color(material.color.r, material.color.g, material.color.b, levelData.demoCharacterAlpha * colorStrength);
				}
			}

			//create the demo tile and parent it to the demo character
			GameObject tile = newTile(levelData.tileColor);
			tile.transform.SetParent(demo.transform, false);

			//update the demo rotation
			updateRotation(demo);
		}
		Debug.Log("Change Demo");
	}
	
	//change all the unit stats in the character panel
	public void setStats(int index){
		GameObject troop = troops[index].deployableTroops;
		Unit unit = troop.GetComponent<Unit>();
		statsName.text = troop.name;
		statsDamage.text = unit.damage + "";
		statsHealth.text = unit.lives + "";
		statsRange.text = troop.GetComponent<NavMeshAgent>().stoppingDistance + "";
		statsSpeed.text = troop.GetComponent<NavMeshAgent>().speed + "";
	}
	
	//create new demo tile
	public GameObject newTile(Color color){
		//instantiate the tile and scale it
		GameObject tile = Instantiate(indicator);
		tile.transform.localScale = new Vector3(2, 0.1f, 2);
		
		//change the look of the tile
		foreach(Renderer renderer in tile.GetComponentsInChildren<Renderer>()){
			renderer.material.shader = Shader.Find("Unlit/UnlitAlphaWithFade");
			renderer.material.color = color;
		}
		
		//if we're erasing, destroy the demo character
		if(erasing)
			Destroy(tile.transform.GetChild(0).gameObject);
		
		return tile;
	}
	
	//update the character placement rotation
	public void updateRotation(GameObject unit){
		Vector3 characterRotation = unit.transform.localEulerAngles;
		unit.transform.localEulerAngles = new Vector3(characterRotation.x, rotation, characterRotation.z);
	}
	
	//using raycasting, get the mouse position compared to the terrain
	public Vector3 getPosition(){
		//initialize a ray and a hit object
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		//check if there's terrain below the current mouse position
		if(Physics.Raycast(ray, out hit) && hit.collider != null && !EventSystem.current.IsPointerOverGameObject() && hit.collider.gameObject.tag == "Battle ground"){
			//enable the demo character if there's a valid position
			if(!currentDemoCharacter.activeSelf)
            {
				currentDemoCharacter.SetActive(true);
				for(int i = 0; i < currentDemoCharacter.transform.childCount; i++)
                {
					disableUnit(currentDemoCharacter.transform.GetChild(i).gameObject);
					updateRotation(currentDemoCharacter.transform.GetChild(i).gameObject);
                }
			}
			//normally, return the hit point
			if (!Input.GetKey(levelData.snappingKey)){
				return hit.point;
			}
			else{
				//if we're using snapping, change the position so it snaps in place
				Vector3 pos = hit.point;
				pos -= Vector3.one;
				pos /= 2f;
				pos = new Vector3(Mathf.Round(pos.x), pos.y, Mathf.Round(pos.z));
				pos *= 2f;
				pos += Vector3.one;
				return pos;
			}
		}
		else if(currentDemoCharacter.activeSelf){
            //don't show the character if it didn't find a position on the terrain
            currentDemoCharacter.SetActive(false);
        }

        //if there's no position, return vector3.zero
        return Vector3.zero;
	}
	
	public void disableUnit(GameObject unit){
		if (erasing)
			return;
		//disable the navmesh agent component
		unit.GetComponent<NavMeshAgent>().enabled = false;
		
		//disable the unit script
		Unit unitScript = unit.GetComponent<Unit>();
		unitScript.spread = levelData.spreadUnits;
		unitScript.enabled = false;
		
		//disable the collider
		unit.GetComponent<Collider>().enabled = false;
		
		//if this is an archer, disable the archer functionality
		if(unit.GetComponent<Archer>())
			unit.GetComponent<Archer>().enabled = false;
		
		//disable the health object
		unit.transform.Find("Health").gameObject.SetActive(false);	
		
		//disable any particles
		foreach(ParticleSystem particles in unit.GetComponentsInChildren<ParticleSystem>()){
			particles.gameObject.SetActive(false);
		}
		
		//make sure it's playing an idle animation
		foreach(Animator animator in unit.GetComponentsInChildren<Animator>()){
			animator.SetBool("Start", false);
		}
	}
	
	public void enableUnit(GameObject unit){
		//enable all the components
		unit.GetComponent<NavMeshAgent>().enabled = true;
		unit.GetComponent<Unit>().enabled = true;
		unit.GetComponent<Collider>().enabled = true;
		unit.GetComponent<AudioSource>().Play();
		
		//enable the archer
		if(unit.GetComponent<Archer>())
			unit.GetComponent<Archer>().enabled = true;
		
		//show the healthbar
		unit.transform.Find("Health").gameObject.SetActive(true);	
		
		//show particles
		foreach(ParticleSystem particles in unit.GetComponentsInChildren<ParticleSystem>()){
			particles.gameObject.SetActive(true);
		}
		Debug.Log("Enable Unit!");
		//start the animators
		foreach(Animator animator in unit.GetComponentsInChildren<Animator>()){
			animator.SetBool("Start", true);
		}
	}
	
	public void reloadScene(){
		//reload the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void nextLevel(){
		//open the next level
		StartCoroutine(openLevel());
	}
	
	IEnumerator openLevel(){
		//wait for the fade transition to end
		transition.SetTrigger("fade");
		
		yield return new WaitForSeconds(0.5f);
		
		//check if the next level exist and load it if it does
		if(PlayerPrefs.GetInt("level") < levelData.levels.Count){
			SceneManager.LoadScene(levelData.levels[PlayerPrefs.GetInt("level")].scene);
		}
		else{
			SceneManager.LoadScene("End screen");
		}
	}
	
	public void menu(){
		//load the menu scene
		SceneManager.LoadScene(0);
	}
	
	//change the UI in the bottom of the left panel
	public void switchPanelContent(bool statsButtonActive){
		if(statsButtonActive && !characterStats){
			statsButton.color = Color.white;
			topDownButton.color = levelData.selectedPanelColor;
			
			statsButton.gameObject.GetComponentInChildren<Text>().color = levelData.selectedPanelColor;
			topDownButton.gameObject.GetComponentInChildren<Text>().color = Color.white;
		}
		else if(!statsButtonActive && characterStats){
			statsButton.color = levelData.selectedPanelColor;
			topDownButton.color = Color.white;
			
			statsButton.gameObject.GetComponentInChildren<Text>().color = Color.white;
			topDownButton.gameObject.GetComponentInChildren<Text>().color = levelData.selectedPanelColor;
		}
		else{
			return;
		}
		
		characterStats = !characterStats;
		
		//set the panels active if they should be active and turn them off if they should not be active
		characterStatsPanel.SetActive(!characterStatsPanel.activeSelf);
		topDownMapPanel.SetActive(!topDownMapPanel.activeSelf);
	}
	
	public void clear(){	
		//go through all units on the battlefield	
		for(int i = 0; i < placedUnits.Count; i++){
			eraseUnit(placedUnits[i].transform.position, true, false);
		}
		
		//clear the unit list and shake the camera
		placedUnits.Clear();
		cameraAnimator.SetTrigger("shake");
	}
	
	public void startBattle(){
		//enable all units so they start fighting
		foreach(GameObject ally in placedUnits){
			enableUnit(ally);
		}
		
		//start all enemies as well
		FindObjectOfType<EnemyArmy>().startEnemies();
		
		//show the new UI
		StartCoroutine(battleUI());
		battleStarted = true;
		
		//destroy the border object
		if(border != null)
			Destroy(border);
	}
	
	public void endGame(){
		//end the battle
		battleStarted = false;
		gamePanel.SetBool("show", false);
	}
	
	public void setSpeed(){
		//change the timescale based on the selected setting
		switch(speedSetting.value){
			case 0: Time.timeScale = 0; break;
			case 1: Time.timeScale = 0.5f; break;
			case 2: Time.timeScale = 1; break;
			case 3: Time.timeScale = 1.5f; break;
			case 4: Time.timeScale = 2; break;
		}
		
		//stop audio if the timescale is 0
		if(Time.timeScale == 0){
			AudioListener.volume = 0;
		}
		else{
			AudioListener.volume = 1;
		}
	}
	
	public void showGrid(){
		//show or hide the grid and change the button text
		if (gridButtonText.text == "Grid Layout")
		{
			gridButtonText.text = "Default 3D Layout";
		}
		else
		{
			gridButtonText.text = "Grid Layout";
		}

		grid.SetBool("show", !grid.GetBool("show"));
	}
	
	IEnumerator battleUI(){
		//hide the character panel
		if(!leftPanelAnimator.GetBool("hide panel"))
			leftPanelAnimator.SetBool("hide panel", true);
		
		//hide the grid
		grid.SetBool("show", false);
		
		//wait a moment and remove the panels
		yield return new WaitForSeconds(0.5f);
		
		leftPanelAnimator.gameObject.SetActive(false);
		buttonsAnimator.SetBool("hide", true);
		
		//show the game panel
		gamePanel.SetBool("show", true);
	}

    private void OnDrawGizmos()
    {
		Gizmos.DrawWireCube(gridCenter+Vector3.up, Vector3.one * gridSize*4);
    }
}
