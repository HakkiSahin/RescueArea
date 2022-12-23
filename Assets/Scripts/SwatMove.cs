using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class SwatMove : MonoBehaviour
{
    /////---------/////
    /////-Classes-/////
    /////---------/////

    public class Shield : stats.SwatAttack {
	public Shield(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	}

	// this will be executed once after reload switch to the attack state
	public override void StartAttack() {
	    base.StartAttack();
	    if(!prop.bDeath)SetClosest();

	    if (bInRange() && !prop.bDeath)
	    {
		    SetAnimation("ShieldKick");
	    }
	    if(!bInRange() && !prop.bDeath) SetAnimation("ShieldIdle");
	    
	}

	// this will be executed once after reload switch to the reload state
	public override void StartReload() {
	    base.StartReload();
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    SetAnimation("ShieldIdle");
	    }
	    //    if(bInRange()) {
	    // SetAnimation("Spin In Place");
	    //    } else {
	    // SetAnimation("Flair");
	    //    }
	}

	// this will be executed according to animation event
	public override void AttackEvent() {
	    base.AttackEvent();
	    Debug.Log(transform.name + " hitted to " + target.name + "  -" + prop.Damage);
	    if(!prop.bDeath) AIscript(target).GetDamage(prop.Damage);
	}

	// this will be executed according to animation event
	public override void ReloadEvent() {
	    base.ReloadEvent();
	    if(!prop.bDeath) SetAnimation("ShieldIdle");
	}

	// this will be excecuted after enemy ai execute attack function
	public override void GetDamage(Transform attacker)
	{
			
	    if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
		Debug.Log(transform.name + " is a shield man, and this character is in Defence Mode");
		return;
	    }

	    if(!prop.bDeath)prop.HP -= 20f;
	    
	}
	public override void Update() {
		LookSmooth(transform, target, 3.5f);
		Debug.Log(prop.HP);
		if (prop.HP <= 0 && !prop.bDeath)
		{
			prop.HP = 0;
			SetAnimation("Death");
			prop.bDeath = true;
		}
		
	}
    } 

    public class Bomber : stats.SwatAttack {
	    private GameObject spawnedOne;

	public Bomber(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	}

	public override void StartAttack() {
	    base.StartAttack();
	    


	    if (!prop.bDeath)
	    {
		    SetClosest();
		    
	    }

	    if (bInRange() && !prop.bDeath)
	    {
		    SetAnimation("Toss Grenade");
		    spawnedOne = Instantiate(prop.spawnObjects[0], spawnPosition.position, Quaternion.identity);
		    spawnedOne.transform.SetParent(spawnPosition);
	    }
	}

	public override void StartReload() {
	    base.StartReload();
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    SetAnimation("Idle");
	    }
	}

	public override void AttackEvent() {
	    base.AttackEvent();

	    if (!prop.bDeath)
	    {
		    if(spawnedOne != null) spawnedOne.GetComponent<Bomber_bomb_c>().Set(target.position, true);
		    spawnedOne = null;
	    }
	}
	

	public override void ReloadEvent() {
	    base.ReloadEvent();
	}
	public override void Update() {
		LookSmooth(transform, target, 3.5f);
		if (prop.HP <= 0)
		{
			prop.HP = 0;
			prop.bDeath = true;
		}
		if(prop.bDeath) SetAnimation("Death");
	}
	
	public override void GetDamage(Transform attacker) {
		
		if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
			
			return;
		}

		prop.HP -= 20f;
	}
    } 

    public class Rammer : stats.SwatAttack {
	public bool bCrashedDoor = false;
	private Transform door;
	
	// wait to turn to run
	private float timer = 0f;
	private float timerLimit = 0f;

	public Rammer(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	    door = GameObject.FindGameObjectWithTag("Door").transform;
	    state = AttackStates.attack;
	}

	public override void StartAttack() {
	    base.StartAttack();
	    if(!prop.bDeath) SetClosest();
	}

	public override void StartReload() {
	    base.StartReload();
	    if(!prop.bDeath) SetClosest();
	}

	public override void AttackEvent() {
	    base.AttackEvent();
	    if(!prop.bDeath) door.GetComponent<EnemyAI>().GetDamage(100f);
	    
	    
	}

	public override void ReloadEvent() {
	    base.ReloadEvent();
	    if(!prop.bDeath) SetAnimation("Idle");
	}
	public override void GetDamage(Transform attacker) {
		
		if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
			
			return;
		}
		
		prop.HP -= 20f;
	}

	public override void Update() {
		
		LookSmooth(transform, door, 3.5f);
		var doorPosition = new Vector3(door.transform.position.x, transform.position.y, door.transform.position.z);
		
		
		if (bCrashedDoor && !prop.bDeath)
		{
			
			if (Vector3.Distance(transform.position, doorPosition) <= 1.5f && door.CompareTag("enemy"))
			{
				if (!AIscript(door).bDead)
				{
					SetAnimation("Punch");
					
					//Debug.Log(456);
				}
				
			}
			else
			{
				transform.position = Vector3.MoveTowards(transform.position, doorPosition, Time.deltaTime * 15f);
			}
			return;
		}
		else if(!bCrashedDoor && !prop.bDeath)
		{
			transform.position = Vector3.MoveTowards(transform.position, doorPosition, Time.deltaTime * 15f);
		}
		

	    
	    timer += Time.deltaTime;
	    if(timer < timerLimit) return;

	    
	    
	    
	    if(Vector3.Distance(transform.position, doorPosition) <= 0.1f  && !prop.bDeath) {
			//SetAnimation("Throw");

			var animationPerc = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

		
			if (!door.CompareTag("enemy"))
			{
				door.GetChild(0).gameObject.SetActive(true);
				door.GetChild(1).gameObject.SetActive(true);
				door.GetComponent<MeshRenderer>().enabled = false;
				SetClosest();
				bCrashedDoor = true;
				
			}
		    
		

		
		    if(door.CompareTag("enemy")) {
				
				
				// door.GetComponent<EnemyAI>().GetDamage(150f);
				
		    } else {
				
		    }

		    door = target;
		
	    } //else SetAnimation("Rammer");
	}

	
	
    } 

    public class Assult : stats.SwatAttack {
	    private GameObject spawnedOne;
	public Assult(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	}

	public override void StartAttack() {
	    base.StartAttack();
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    
	    }

	    if (bInRange() && !prop.bDeath)
	    {
		    spawnedOne = Instantiate(prop.spawnObjects[0], spawnPosition.position, Quaternion.identity);
		    spawnedOne.transform.SetParent(spawnPosition);
		    SetAnimation("Firing Rifle 0");
	    }
	}

	public override void StartReload() {
	    base.StartReload();
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    SetAnimation("RifleIdle");
	    }
	}

	public override void AttackEvent() {
	    base.AttackEvent();
	    if (!prop.bDeath)
	    {
		    if(spawnedOne != null) spawnedOne.GetComponent<Assult_bullet_c>().Set(target.position, prop.Damage, target);
		    spawnedOne = null;
	    }
	    
	}

	public override void ReloadEvent() {
	    base.ReloadEvent();
	}
	public override void GetDamage(Transform attacker) {
		
		if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
			
			return;
		}

		prop.HP -= 20f;
	}
	public override void Update() {
		LookSmooth(transform, target, 3.5f);
		
	}
    } 

    public class Sniper : stats.SwatAttack {
	    private GameObject spawnedOne;
	public Sniper(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	}
	

	public override void StartAttack() {
	    base.StartAttack();
	    
	    if(!prop.bDeath) SetFarest();
	    
	    if (target != transform  && !prop.bDeath)
	    {
		    
		    //spawnedOne.transform.SetParent(spawnPosition);
		    //LookSmooth(transform, target, 3.5f);


		    if (bInRange())
		    {
			    SetAnimation("Firing Rifle");
			    spawnedOne = Instantiate(prop.spawnObjects[0], spawnPosition.position, Quaternion.identity);
		    }
	    }
	    
	}

	public override void StartReload() {
	    base.StartReload();
	    
	    if (!prop.bDeath)
	    {
		    SetFarest();
		    SetAnimation("RifleIdle");
	    }
	}

	public override void AttackEvent() {
	    base.AttackEvent();
	    if (!prop.bDeath)
	    {
		    if(spawnedOne != null) spawnedOne.GetComponent<Sniper_bullet_c>().Set(target.position, prop.Damage, target);
		    spawnedOne = null;
	    }
	    
	}

	public override void ReloadEvent() {
	    base.ReloadEvent();
	}
	public override void GetDamage(Transform attacker) {
		
		if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
			
			return;
		}

		prop.HP -= 20f;
	}
	public override void Update() {
		LookSmooth(transform, target, 3.5f);

		
	}
	
    } 

    public class AntiMiner : stats.SwatAttack {
	    private GameObject spawnedOne;
	    
	public AntiMiner(stats.Props prop, Transform transform, stats.ParticleStruct[] ps) : base(prop, transform, ps) {
	}

	public override void StartAttack() {
	    base.StartAttack();
	    minerGun.SetActive(true);
	    gun.gameObject.SetActive(false);
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    
	    }

	    if (bInRange() && !prop.bDeath)
	    {
		    spawnedOne = Instantiate(prop.spawnObjects[0], spawnPosition.position, Quaternion.identity);
		    spawnedOne.transform.SetParent(spawnPosition);
		    SetAnimation("Firing Pistol");
	    }
	}

	public override void StartReload() {
	    base.StartReload();
	    
	    if (!prop.bDeath)
	    {
		    SetClosest();
		    SetAnimation("Pistol Idle");
	    }
	}

	public override void AttackEvent() {
	    base.AttackEvent();
	    if (!prop.bDeath)
	    {
		    if(spawnedOne != null) spawnedOne.GetComponent<Assult_bullet_c>().Set(target.position, prop.Damage, target);
		    spawnedOne = null;
	    }
	}

	public override void ReloadEvent() {
	    base.ReloadEvent();
	}
	public override void GetDamage(Transform attacker) {
		
		if(Vector3.Distance(transform.position, attacker.position) > prop.AttackRange) {
			
			return;
		}

		prop.HP -= 20f;
	}
	public override void Update() {
		LookSmooth(transform, target, 3.5f);
		
	}
    } 


    // After Classes \\ 
    [FoldoutGroup("Development")]
    public stats.ParticleStruct[] ps;
    [FoldoutGroup("Development")]
    public SwatStats _props;
    public static bool _go;

    private SpriteRenderer sprite;

    // movement properties \\
    [HideInInspector] public Transform toGo;
    private Vector3[] toGoVectors;

    internal int current = 0;
    internal int current_end = 0;
    public bool stop = true;
    internal float totalMovement;
    [FoldoutGroup("Level Design")]
    public float speed;

    public enum States {
	Move,
	Attack,
	Wait
    }

    private States state;

    // squad \\
    internal bool lineCompleted;
    internal bool bTeam;
    internal List<SwatMove> squad = new();

    // comander \\
    private SwatMove comander;
    [FoldoutGroup("Level Design")]
    public float comanderDistance;

    // class \\
    [HideInInspector] public stats._Class _class;
    public stats.SwatAttack attack;
    internal bool bSelected = false;

    private void Start() {
	SetClass();
	transform.name = _class.ToString();
	attack.SetAnimation(_class.ToString() + "_Idle");
	_go = false;
	sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
	    if (Input.GetMouseButton(0))
	    {
		    stop = false;
		    GameHandler.i.StartMenu_disable();
	    }
	    
	    switch(state) {
		
			case States.Move:
				if(!stop)Move();
				break;

			case States.Attack:
				attack.StateSetter();
				attack.Update();
				break;
		
	}
	if (attack.prop.bDeath) attack.SetAnimation("Death");
	if (attack.prop.HP <= 0)
	{
		attack.prop.HP = 0;
		attack.prop.bDeath = true;
	}
    }

    public void AttackEvent() {
	attack.AttackEvent();
    }

    public void ReloadEvent() {
	attack.ReloadEvent();
    }

    private void Move() {
	if(toGo == null || !_go) return;
	

	var local_go = new Vector3(toGoVectors[current].x, transform.position.y, toGoVectors[current].z);
	if(comander != null)  { // if soldier has a comander
	    var comanderPosition = new Vector3(comander.transform.position.x, transform.position.y, comander.transform.position.z);
	    var currentDistance_toComander = Vector3.Distance(transform.position, comanderPosition);

	    var selfPerc = current / (float)toGoVectors.Length * 100f;
	    var comanderPerc = comander.current_end / (float)toGoVectors.Length * 100f;

	    // make soldier to wait its comander
	    var bBehind = selfPerc <= 2f && comanderPerc <= 2f || selfPerc <= comanderPerc || currentDistance_toComander > comanderDistance;
	    if(comander.totalMovement > 1f && bBehind) {
		totalMovement += Time.deltaTime;
		
		transform.position = Vector3.MoveTowards(transform.position, local_go, Time.deltaTime * speed);
		
		attack.SetAnimation(_class.ToString() + "_run");
	    }

	    if(comander.lineCompleted && selfPerc <= comanderPerc && currentDistance_toComander <= comanderDistance) {
		EndSwat();
		attack.SetAnimation(_class.ToString() + "_Idle" );
		// uncomment that if you need to debug this
		// Debug.Log("Comander of " + transform.name + " is completed its line, so " + transform.name + " is completed");	
		return;
	    }
	} else {
	    totalMovement += Time.deltaTime;
	    
		transform.position = Vector3.MoveTowards(transform.position, local_go, Time.deltaTime * speed); // if soldier does not have a comander
		
	    
		attack.SetAnimation(_class.ToString() + "_run");
	}

	transform.LookAt(local_go);

	if(Vector3.Distance(transform.position, local_go) < 0.1f)
	{
		
	    if(current+1 < toGoVectors.Length) {
		current++;
		current_end++;
	    } else {
		EndSwat();
	    }
	}
    }

    public void EndSwat() { // line ending
	if(bSelected) TouchHandler.I.endedCount++;
	//attack.SetAnimation("Drunk Idle Variation");
	attack.SetAnimation(_class.ToString() + "_Idle" );
	toGo = null;
	toGoVectors = new Vector3[0];
	totalMovement = 2f;
	current = 0;
	SetSelectedColor(new Color(0f, 0f, 0f, 0f));
	lineCompleted = true;
	state = States.Wait;
	Debug.Log(transform.name + " ended");
    }

    // when all squads ended (this will be triggered by TouchHandler)
    public void Reset() { 
	current_end = 0;
	lineCompleted = false;
	totalMovement = 0f;
	squad = new List<SwatMove>();
	state = States.Attack;
	if(_class == stats._Class.Rammer) {
	    
	}
    }

    // if this is a member of squad and user try to remap
    public void RemoveFromOthers() {
	for(var i = 0; i < squad.Count; i++)
		if(squad[i] != this) squad[i].squad.Remove(this);

	Debug.Log(transform.name + " is removed");

	squad = null;
    }

    [HideInInspector] private bool calculated = false; // prevent second calculation if first squad member have already calculated that
    public void SetComander() {
	if(squad.Count == 0) return;

	if(!calculated) {
	    comander = squad[0];
	    for(var i = 0; i < squad.Count; i++)
	    for(var j = 0; j < squad.Count; j++)
		    if( (int)squad[i]._class < (int)squad[j]._class ) {
			    var holder = squad[i];
			    squad[i] = squad[j];
			    squad[j] = holder;
		    }

	    for(var i = 0; i < squad.Count; i++)
		    if(squad[i] != this) {
			    squad[i].calculated = true;
			    squad[i].squad = squad;
		    }

	    calculated = true;
	}

	if(squad.IndexOf(this) == 0) comander = null;
	else comander = squad[squad.IndexOf(this) - 1];
    }

    private bool bTouchWall = false;
    public void OnTriggerStay(Collider other) {
	    if (other.CompareTag("SphereCollider") && !bTouchWall && _class != stats._Class.Rammer)
	    {
		    EndSwat();
		    bTouchWall = true;
	    }
	attack.TriggerStay(other);
    }

    public void OnTriggerExit(Collider other) {
	attack.TriggerExit(other);
    }

    public void OnTriggerEnter(Collider other) {
	attack.TriggerEnter(other);
    }

    public void SetSelectedColor(Color color) {
	sprite.color = color;
    }

    public void SetRoad(Transform ToGo, Vector3[] vectorArray) {
	toGo = ToGo;
	toGoVectors = vectorArray;
    }

    // TODO: run this with some on gui function to change class (odin)
    private void SetClass() {
	switch(_class.ToString()) {
	    case "Shield":
		attack = new Shield(_props.props[(int)_class], transform, ps);
		break;

	    case "Bomber":
		attack = new Bomber(_props.props[(int)_class], transform, ps);
		break;

	    case "Rammer":
		attack = new Rammer(_props.props[(int)_class], transform, ps);
		break;

	    case "Assult":
		attack = new Assult(_props.props[(int)_class], transform, ps);
		break;

	    case "Sniper":
		attack = new Sniper(_props.props[(int)_class], transform, ps);
		break;

	    case "AntiMiner":
		attack = new AntiMiner(_props.props[(int)_class], transform, ps);
		break;
	}

	Debug.Log(transform.name + " is initialized as: " + _class.ToString());
    }

    [FoldoutGroup("Level Design")]
    [Button("Previous Class")]
    [ButtonGroup("Level Design/Class Group")]
    public void SetClassGUI_Decrease() {
	_class--;
	if((int)_class < 0) _class = stats._Class.AntiMiner;
	_Class = _class.ToString();

	if(MeshParent == null)
		for(var i = 0; i < transform.childCount; i++)
			if(transform.GetChild(i).name == "MeshParent") MeshParent = transform.GetChild(i);

	for(var i = 0; i < MeshParent.childCount; i++) MeshParent.GetChild(i).gameObject.SetActive(false);

	transform.name = _class.ToString();
	MeshParent.GetChild((int)_class).gameObject.SetActive(true);
    }


    [Button("Next Class")]
    [ButtonGroup("Level Design/Class Group")]
    public void SetClassGUI_Increase() {
	_class++;
	if((int)_class > 5) _class = 0;
	_Class = _class.ToString();

	if(MeshParent == null)
		for(var i = 0; i < transform.childCount; i++)
			if(transform.GetChild(i).name == "MeshParent") MeshParent = transform.GetChild(i);

	for(var i = 0; i < MeshParent.childCount; i++) MeshParent.GetChild(i).gameObject.SetActive(false);

	transform.name = _class.ToString();
	MeshParent.GetChild((int)_class).gameObject.SetActive(true);
    }

    private Transform MeshParent;
    [FoldoutGroup("Level Design")]
    [ReadOnly] public string _Class;
}
