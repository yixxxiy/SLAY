using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float radius = 1.5f;
    public int damage = 30;
    public float coolDown = 2f;
    public bool gizmosEnabled = false;
    public Vector3 offset;

    private float _wait = 0f;
    private Vector3 _center => transform.position + offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Regain();
    }

    private void _Regain()
    {
        if (_wait > 0f)
        {
            _wait -= Time.deltaTime;
            if (_wait < 0f)
            {
                _wait = 0f;
                Debug.Log("melee attack ready");
            }
        }
    }

    public void Use()
    {
        if (_wait > 0f)
        {
            Debug.Log("not available yet");
            return;
        }

        _wait = coolDown;
        var colliders = Physics2D.OverlapCircleAll(_center, radius);
        foreach (var collider in colliders)
        {
            if (collider.tag != "Pal") continue;
            Debug.Log("melee attack " + collider.name);
            var palScript = collider.GetComponent<PalScript>();
            if (palScript == null || palScript.pal.isCaptured) continue;
            palScript.Hurt(damage);
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_center, radius);
    }
}
