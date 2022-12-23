using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stats;
using UnityEngine;

namespace stats
{
    public class SwatAttack
    {
        public Props prop;
        public Transform transform;
        public Animator anim;

        public Transform target;
        public ParticleSystem[] particles;
        public Transform spawnPosition;
        public GameObject gun;
        public GameObject minerGun;

        // trigger attack and wait to complete attack state, then set reload and after that set wait state again.
        // Attack ==> Reload ==> Attack
        public enum AttackStates
        {
            reload,
            attack
        }

        public AttackStates state;

        private float timer;
        internal float WaitTime;

        public SwatAttack(Props _prop, Transform _transform, ParticleStruct[] ps = null)
        {
            prop = _prop;

            for (var i = 0; i < ps.Length; i++)
                if (ps[i]._class == prop._class)
                    particles = ps[i].ps;

            transform = _transform;
            anim = transform.GetComponentInChildren<Animator>();
            WaitTime = 0f;

            var spawnPositions = transform.GetComponentsInChildren<Transform>();

            for (var i = 0; i < spawnPositions.Length; i++)
            {
                spawnPosition = spawnPositions[i];
                if (spawnPosition.name == "Pistol")
                {
                    minerGun = spawnPosition.gameObject;
                    spawnPosition.gameObject.SetActive(false);
                }
                if (spawnPosition != transform)
                {
                    if (spawnPosition.name == prop._class + "_spawnPosition")
                    {
                        spawnPosition.gameObject.SetActive(true);
                        Debug.Log(prop._class + "_spawnPosition has been found for " + transform.name);
                    }
                    else if (spawnPosition.transform.name.EndsWith("_spawnPosition"))
                    {
                        spawnPosition.gameObject.SetActive(false);
                    }

                    if (spawnPosition.name.StartsWith(prop._class + "_ac"))
                    {
                        spawnPosition.gameObject.SetActive(true);
                        gun = spawnPosition.gameObject;
                    }
                    else if (spawnPosition.name.EndsWith("_ac")) spawnPosition.gameObject.SetActive(false);
                    Debug.Log(prop._class.ToString());
                }
            }

            state = AttackStates.attack;
        }

        public void StateSetter()
        {
            timer += Time.deltaTime;

            if (timer > WaitTime)
            {
                timer = 0f;
                switch (state)
                {
                    case AttackStates.attack:
                        WaitTime = prop.AttackTime;
                        StartAttack();
                        state = AttackStates.reload;
                        break;

                    case AttackStates.reload:
                        WaitTime = prop.ReloadTime;
                        StartReload();
                        state = AttackStates.attack;
                        break;
                }
            }
        }

        // this will be executed once after reload switch to the attack state
        public virtual void StartAttack()
        {
            if (target != null) LookSmooth(transform, target);
            if (target == null) return;
        }

        // this will be executed once after reload switch to the reload state
        public virtual void StartReload()
        {
            if (target != null) LookSmooth(transform, target);
        }

        // this will be executed according to animation event
        public virtual void AttackEvent()
        {
        }

        // this will be executed according to animation event
        public virtual void ReloadEvent()
        {
        }

        // this will be excecuted after enemy ai execute attack function
        public virtual void GetDamage(Transform attacker)
        {
        }

        // execute always in attack mode
        public virtual void Update()
        {
        }

        public virtual void TriggerStay(Collider other)
        {
        }

        public virtual void TriggerEnter(Collider other)
        {
        }

        public virtual void TriggerExit(Collider other)
        {
        }

        public void LookSmooth(Transform main, Transform toAttack, float speed = 5f)
        {
            if (toAttack == null) return;
            toAttack = target;
            var lookPos = new Vector3(toAttack.position.x, main.position.y, toAttack.position.z) - main.position;

            main.localRotation = Quaternion.Lerp(main.localRotation, Quaternion.LookRotation(lookPos),
                Time.deltaTime * speed);

        }


        public Transform FindFarest(Transform[] SearchArray, Transform main)
        {
            var toAttack = transform;

            for (var i = 0; i < SearchArray.Length; i++)
            {
                
                var old = Vector3.Distance(main.position,
                    new Vector3(toAttack.position.x, main.position.y, toAttack.position.z));
                var current = Vector3.Distance(main.position,
                    new Vector3(SearchArray[i].position.x, main.position.y, SearchArray[i].position.z));
                if (current > old && !AIscript(SearchArray[i]).bDead) toAttack = SearchArray[i];
            }

            return toAttack;
        }

        public void SetFarest()
        {
            target = FindFarest(EnemyAIHandler.I.aiS_transform, transform);
        }

        public Transform FindClosest(Transform[] SearchArray, Transform main)
        {
            Transform toAttack = GameObject.FindGameObjectWithTag("FarestObj").transform;
            

            for (int i = 0; i < SearchArray.Length; i++)
            {
                
                float old = Vector3.Distance(main.position,
                    new Vector3(toAttack.position.x, main.position.y, toAttack.position.z));
                var current = Vector3.Distance(main.position,
                    new Vector3(SearchArray[i].position.x, main.position.y, SearchArray[i].position.z));
                if (current < old && !AIscript(SearchArray[i]).bDead) toAttack = SearchArray[i];
            }

            return toAttack;
        }

        public void SetClosest()
        {
            target = FindClosest(EnemyAIHandler.I.aiS_transform, transform);
        }

        public void SetAnimation(string name, float trans = 0f)
        {
            anim.CrossFade(name, trans, 0);
        }

        public bool bInRange()
        {
            if (target == null) return  false;
            return Vector3.Distance(transform.position,
                       new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z)) <=
                   prop.AttackRange;
        }

        public EnemyAI AIscript(Transform _target)
        {
            return EnemyAIHandler.I.dic[_target];
        }
    }

    public enum _Class
    {
        Shield = 0,
        Bomber = 1,
        Rammer = 2,
        Assult = 3,
        Sniper = 4,
        AntiMiner = 5
    }

    [Serializable]
    public struct ParticleStruct
    {
        [ReadOnly] public _Class _class;
        public ParticleSystem[] ps;
    }

    [Serializable]
    public struct Props
    {
        [ReadOnly] public _Class _class;
        public float Damage;
        public float AttackRange;
        public float HP;
        public bool bDeath;
        public float AttackTime;
        public float ReloadTime;

        public GameObject[] spawnObjects;

        //public string IdleAnimation;
        //public string RunAnimation;
        //public string AttackAnimation;
        //public string ReloadAnimation;
    }
}

[CreateAssetMenu(fileName = "SwatStats", menuName = "Stats")]
public class SwatStats : ScriptableObject
{
    private List<string> foo;
    public Props[] props;
}