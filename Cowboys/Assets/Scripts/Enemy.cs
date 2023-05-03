using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float rotateSpeed = 5;
    private Animator _animator;
    private Transform playerTransform;
    private bool isDead = false;
    private static readonly int Death = Animator.StringToHash("Death");

    // Start is called before the first frame update
    private void Start()
    {
        playerTransform = GameManager.instance.playerTransform;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDead) return;
        var transform1 = transform;
        var dir = (playerTransform.position - transform1.position).normalized;
        transform1.Translate(dir * (moveSpeed * Time.deltaTime), Space.World);
        var rotation = Quaternion.LookRotation(playerTransform.position - transform1.position);
        transform1.rotation = Quaternion.RotateTowards(transform1.rotation, rotation, rotateSpeed);
    }

    public IEnumerator Die()
    {
        if (isDead)
            yield return null;
        isDead = true;
        //Prevents player from shooting a dead enemy again
        gameObject.layer = 0;
        _animator.SetTrigger(Death);
        yield return new WaitForSeconds(1.2f);
        Destroy(gameObject);
    }
}
