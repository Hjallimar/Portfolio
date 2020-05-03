using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWeapons : MonoBehaviour
{
    [SerializeField] private Transform spawnpoint = null;
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private LayerMask obscureLayer;
    [SerializeField] private GameObject fireArm = null;
    [SerializeField] private GameObject fireParticles = null;
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioSource audio = null;
    [SerializeField] private AudioSource[] hitAudioSources = null;

    [Header("Defence system")]
    [SerializeField] private GameObject baseStation = null;
    [SerializeField] private float attackRange = 100.0f;
    [SerializeField] private GameObject primaryTarget = null;
    [SerializeField] private LayerMask radarMask;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();


    [Header("Balance")]
    [SerializeField] private float ammo = 100;
    [SerializeField] private float damage = 10;
    [SerializeField] private DamageType type = null;
    [SerializeField] private float reloadTime = 20;
    [SerializeField] private float musVelocity;
    [SerializeField] private float rpm = 100;

    [Space]
    [Header("Istället för debug")]
    [SerializeField] bool attacking = false;
    float timer;
    float currentAmmo;
    [SerializeField] bool active = true;
    float cd = 0;

    private int currentHitAudio = 0;
    public float Damage { get { return damage; } }

    // Start is called before the first frame update
    void Start()
    {
        cd = 60 / rpm;
        currentAmmo = ammo;
        float customBalanceDamage = GameController.GetBalanceVariable(BalanceFileHolder.BalanceVariableNames.turret_damage);
        if(customBalanceDamage >= 0.0f)
        {
            damage = customBalanceDamage;
        }
        EventCoordinator.RegisterEventListener<EnemyDiedEventInfo>(EnemyDied);
        EventCoordinator.RegisterEventListener<GateDestroyedEventInfo>(GateDestroyed);
    }

    void OnDestroy()
    {
        EventCoordinator.UnregisterEventListener<EnemyDiedEventInfo>(EnemyDied);
        EventCoordinator.UnregisterEventListener<GateDestroyedEventInfo>(GateDestroyed);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForEnemies();
        if (active)
        {
            timer += Time.deltaTime;
            if (attacking && timer > cd && currentAmmo > 0 && primaryTarget != null)
            {
                transform.LookAt(primaryTarget.transform.position);
                if (!Physics.Linecast(spawnpoint.transform.position, primaryTarget.transform.position, obscureLayer))
                {
                    Fire();
                }
            }
            else if (currentAmmo <= 0)
            {
                if (timer > reloadTime)
                {
                    currentAmmo = ammo;
                    timer = 0;
                }
            }
        }

        if (attacking)
        {
            //if(audio.isPlaying == false)
            //{
            //    audio.Play();
            //}
            fireArm.transform.RotateAroundLocal(Vector3.forward, 5);
        }
        else
        {
            //audio.Stop();
        }
    }

    void Fire()
    {
        timer = 0;
        ProjectileBase projB = Instantiate(projectile, spawnpoint.position, spawnpoint.rotation).GetComponent<ProjectileBase>();
        projB.Velocity = spawnpoint.forward * musVelocity;
        projB.Damage = damage;
        projB.DmgType = type;
        currentAmmo -= 1;
        if(hitAudioSources != null && hitAudioSources.Length > 0)
        {
            projB.audioSource = hitAudioSources[currentHitAudio];
        }
        if(hitAudioSources.Length == ++currentHitAudio)
        {
            currentHitAudio = 0;
        }
        if (audio.isPlaying)
        {
            audio.Stop();
        }
        audio.Play();
        ParticleEventInfo psei = new ParticleEventInfo(gameObject,"Particles", fireParticles, spawnpoint.position, spawnpoint.rotation);
        EventCoordinator.ActivateEvent(psei);
        //SoundEventInfo sei = new SoundEventInfo(gameObject, "Firesound", fireSound, spawnpoint.position);
        //EventCoordinator.ActivateEvent(sei);

    }

    void GateDestroyed(EventInfo ei)
    {
        if (ei.GO == baseStation)
        {
            active = false;
        }
    }

    void CheckForEnemies()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, attackRange, radarMask);
        if (targets.Length != 0)
        {
            if (primaryTarget == null)
            {
                primaryTarget = targets[0].gameObject;
                enemies.Add(targets[0].gameObject);
                attacking = true;
            }

            foreach (Collider col in targets)
            {
                if (enemies.Contains(col.gameObject)) { }
                else
                {
                    enemies.Add(col.gameObject);
                }
            }

            CheckRemainingEnemies(targets);
        }
        else
        {
            primaryTarget = null;
            attacking = false;
        }
    }

    void CheckRemainingEnemies(Collider[] targets)
    {
        List<GameObject> gone = new List<GameObject>();
        bool check = false;

        foreach (GameObject go in enemies)
        {
            foreach (Collider col in targets)
            {
                if (go == col.gameObject)
                    check = true;
            }

            if (!check)
                gone.Add(go);
            check = false;
        }

        foreach (GameObject go in gone)
        {
            enemies.Remove(go);
        }
    }

    void EnemyDied(EventInfo ei)
    {
        if (primaryTarget == ei.GO)
        {
            primaryTarget = null;
        }

        if (enemies.Contains(ei.GO))
            enemies.Remove(ei.GO);
    }
}
