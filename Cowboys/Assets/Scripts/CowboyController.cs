using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CowboyController : MonoBehaviour
{
    public float moveSpeed;
    public float shootCooldown = 2.5f;
    public float detectRadius = 10;
    public float fireRadius = 5;
    public float rotateSpeed = 0.5f;
    public Camera camera;
    private CinemachineFreeLook _freeLookComponent;
    public GameObject armature;
    private InputHandler _inputHandler;
    private Animator _animator;
    [SerializeField] private int maxEnemiesDetectable = 5;
    private List<GameObject> enemies;
    private GameObject nearestEnemy;
    private float currShootCooldown = 0;
    private LayerMask enemyLayer;
    private bool isDead = false;
    private ParticleSystem gunshotEffect;

    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Death = Animator.StringToHash("Death");

    // Start is called before the first frame update
    void Start()
    {
        gunshotEffect = GetComponentInChildren<ParticleSystem>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
        enemies = new List<GameObject>(maxEnemiesDetectable);
        _inputHandler = GetComponent<InputHandler>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDead) return;
        if (currShootCooldown > 0)
        {
            currShootCooldown -= Time.deltaTime;
        }
        var targetVec = new Vector3(_inputHandler.InputVector.x, 0, _inputHandler.InputVector.y);
        MoveTowardTarget(targetVec);
        CheckForTargets();
        RotateToTarget();
    }

    private void RotateToTarget()
    {
        if (!nearestEnemy && enemies.Count == 0)
        {
            return;
        } else if (!nearestEnemy)
        {
            nearestEnemy = enemies[0];
        }
        var nearestDist = Vector3.Distance(transform.position, nearestEnemy.transform.position);
        foreach (var enemy in enemies)
        {
            var newDist = Vector3.Distance(transform.position, enemy.transform.position);
            if (!(newDist < nearestDist)) continue;
            nearestEnemy = enemy;
            nearestDist = newDist;
        }
        var rotation = Quaternion.LookRotation(nearestEnemy.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
        if (Vector3.Distance(transform.position, nearestEnemy.transform.position) >= fireRadius || currShootCooldown > 0) return;
        ShootEnemy(nearestEnemy);
        enemies.Remove(nearestEnemy);
        nearestEnemy = null;
    }

    private void CheckForTargets()
    {
        if (enemies.Count == maxEnemiesDetectable) return;
        var trans = transform;
        var results = new RaycastHit[maxEnemiesDetectable];
        Physics.SphereCastNonAlloc(trans.position, detectRadius, trans.forward, results, 9999, 1 <<enemyLayer);
        if (results.Length == 0) return;
        foreach (var hit in results)
        {
            if (!hit.collider) continue;
            var enemy = hit.collider.gameObject;
            if (enemies.Contains(enemy)) continue;
            enemies.Add(enemy);
        }
    }

    private void MoveTowardTarget(Vector3 targetVec)
    {
        var speed = moveSpeed * Time.deltaTime;
        targetVec = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * targetVec;
        var targetPos = targetVec * speed;
        transform.Translate(targetPos, Space.World);
    }

    private void ShootEnemy(GameObject enemy)
    {
        currShootCooldown = shootCooldown;
        _animator.SetTrigger(Shoot);
        gunshotEffect.Play();
        var enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript)
            StartCoroutine(enemyScript.Die());
        //yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        //Destroy(enemy);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Enemy") || isDead) return;
        Die();
    }

    private void Die()
    {
        isDead = true;
        _animator.SetTrigger(Death);
    }
}
