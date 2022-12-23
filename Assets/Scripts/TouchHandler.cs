using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class TouchHandler : MonoBehaviour
{
    public static TouchHandler I;
    private List<Transform> Lines = new();

    private LineRenderer LastRenderer;

    [Range(0, 1)]
    public float width;

    public Material mat;
    public Color[] color;
    private int currentColor;

    internal GameObject[] ToCarry;
    public List<GameObject> selectedList = new();
    private List<SwatMove> selectedList_scripts = new();
    private GameObject selected;

    internal int endedCount;
    internal SwatMove[] swatMoves;

    public float Smoothness = 0.1f;
    public int selectedCount = 0;
    private List<Transform> selecteds = new List<Transform>();

    private enum State {
	SetMovement,
	AttackState
    }

    private State state;
	
	
    private void Start()
    {
	    
	I = this;
	ToCarry = GameObject.FindGameObjectsWithTag("ToCarry");
	swatMoves = FindObjectsOfType<SwatMove>();

	GameObject.FindGameObjectWithTag("GoButton").GetComponent<Button>().onClick.AddListener(delegate {
	    Invoke("DoButton", 0.3f);
	    GameObject.FindGameObjectWithTag("GoButton").GetComponent<Animator>().enabled = true;
	    GameObject.FindGameObjectWithTag("GoButton").GetComponent<Button>().enabled = false;
	});
    }

    public bool chrcDeath()
    {
	    int deathCount = 0;
	    for (int i = 0; i < swatMoves.Length; i++)
	    {
		    if (swatMoves[i].attack.prop.bDeath) deathCount++;
	    }

	    return deathCount >= swatMoves.Length; 
    }

    public void DoButton()
    {
	    SwatMove._go = true;

	    for (int i = 0; i < swatMoves.Length; i++)
	    {
		    swatMoves[i].SetComander();
		    if (swatMoves[i]._class == stats._Class.Rammer)
		    {
			    swatMoves[i].RemoveFromOthers();
			    swatMoves[i].EndSwat();
			    swatMoves[i].Reset();// this part can cause some bugs
			    swatMoves[i].attack.SetAnimation("Rammer_run");
		    }
		    
		    state = State.AttackState;
	    }
    }

    private void Update() {
	if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(0);
	clearLines();

	switch(state) {
	    case State.SetMovement:
		startSelection();
		Draw();
		endSelection();
		break;

	    case State.AttackState:
		break;
	}
    }

    private void startSelection() {
	if(Input.GetMouseButtonDown(0)) {
	    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	    RaycastHit hit;

	    if( Physics.Raycast(ray, out hit) ) {
		if(hit.transform.CompareTag("ToCarry"))
		{
		    SetLayer(false);
		    selected = hit.transform.gameObject;

		    var swat = selected.GetComponent<SwatMove>();
		    if(!selecteds.Contains(swat.transform))selecteds.Add(swat.transform);
		    if(swat.toGo != null) {
			if(!swat.bTeam)
				Destroy(swat.toGo.gameObject, 0f);
			else // check if that is belong to team
				swat.RemoveFromOthers();
		    } else {
			selectedCount++;
			
			swat.bSelected = true;
			Debug.Log("New swat is selected");
		    }

		    swat.SetSelectedColor(color[currentColor]);

		    Debug.Log(hit.transform.name + " ---> is selected");
		    Debug.Log("Line is started");

		    CreateLineRenderer(ref LastRenderer, hit.point);
		} else if(selectedList.Count > 0) { // create line for squad
		    CreateLineRenderer(ref LastRenderer, hit.point);
		}
	    }
	}
    }

    private void endSelection() {
	if(Input.GetMouseButtonUp(0)) {
	    SetLayer(true);
	    if(LastRenderer == null) return;

	    if(LastRenderer.positionCount < 7 && selected != null) { // add to squad

			selectedList.Add(selected);
			selectedList_scripts.Add(selected.GetComponent<SwatMove>());
			Debug.Log(selected.name + " added to squad");
			selected = null;
			Destroy(LastRenderer.gameObject, 0f);
			if(currentColor > 0) currentColor--;
	    } else {
			if(selectedList.Count > 0) { // end multi selection
				Debug.Log("Squad created with: " + selectedList.Count);
				Debug.Log("Line is ended with: " + LastRenderer.positionCount + " points for squad");

				if(selected != null) {
					selectedList.Add(selected);
					selectedList_scripts.Add(selected.GetComponent<SwatMove>());
				}

				for(var i = 0; i < selectedList.Count; i++) SetSwats(selectedList_scripts[i], true); // second parameted is tell swat it is in team

		    selected = null;
		    LastRenderer = null;
		    selectedList.Clear();
		    selectedList_scripts = new List<SwatMove>();
			}

			if(selected == null) return; 

			Debug.Log("Line is ended with: " + LastRenderer.positionCount + " points");
			SetSwats(selected.GetComponent<SwatMove>());
			selected = null;
			LastRenderer = null;
	    }
	}
    }

    private Vector3 oldPos;

    private void Draw()
    {
	    if (LastRenderer != null)
	    {
		    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		    RaycastHit hit;

		    if (Physics.Raycast(ray, out hit))
			    if (Vector3.Distance(hit.point, oldPos) > Smoothness)
			    {
				    LastRenderer.positionCount++;
				    LastRenderer.SetPosition(LastRenderer.positionCount - 1, hit.point + Vector3.up * 0.2f);
				    oldPos = hit.point;
			    }
	    }
    }

    private void clearLines() {
	if(Lines.Count > 0 && AllEnded()) {
	    for(var i = 0; i < Lines.Count; i++)
		    if(Lines[i] != null) Destroy(Lines[i].gameObject, 0f);

	    Lines.Clear();
	    endedCount = 0;
	    currentColor = 0;
	    SwatMove._go = false;

	    for(var i = 0; i < swatMoves.Length; i++) swatMoves[i].Reset();
	}
    }

    public bool AllEnded() {
	    Debug.Log(endedCount + "  " + selecteds.Count );
	return selecteds.Count <= endedCount && SwatMove._go;
    }

    private void CreateLineRenderer(ref LineRenderer lr, Vector3 startPos) {
	var LR_object = new GameObject();
	LR_object.transform.position = new Vector3(LR_object.transform.position.x, 0.9297745f, LR_object.transform.position.z);
	LR_object.transform.name = "===Line===";

	lr = LR_object.AddComponent<LineRenderer>();
	lr.material = mat;
	lr.SetPosition(0, startPos);
	lr.SetPosition(1, startPos);
	lr.SetWidth(width, width);

	lr.material.color = color[currentColor];
	currentColor++;

	if(currentColor >= color.Length) currentColor = 0;

	Lines.Add(lr.transform);
    }

    private void SetLayer(bool bDo) {
	for(var i = 0; i < ToCarry.Length; i++) ToCarry[i].layer = bDo ? 0:2;
    }

    public void SetSwats(SwatMove _selectedS, bool bTeam = false) {
	var poses = new Vector3[LastRenderer.positionCount];

	LastRenderer.GetPositions(poses);

	_selectedS.SetRoad(LastRenderer.gameObject.transform, poses);
	_selectedS.bTeam = bTeam;
	if(bTeam) _selectedS.squad = selectedList_scripts;
    }
}
