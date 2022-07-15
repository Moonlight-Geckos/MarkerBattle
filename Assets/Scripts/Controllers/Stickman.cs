using UnityEngine;
using UnityEngine.AI;

public class Stickman : MonoBehaviour, IDamagable
{
    private Animator _animator;
    private SkinnedMeshRenderer _skin;
    private float _currentHealth;
    private NavMeshAgent _navAgent;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _navAgent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(Color skinColor, float health, Vector3 destination)
    {
        _currentHealth = health;
        _animator.Rebind();
        _animator.Update(0f);
        _navAgent.destination = destination;
        _skin.material.color = skinColor;
    }

    public void GetDamage(float damage, float cooldown = -1)
    {
        _currentHealth--;
    }

    public void StopDamage(float damage)
    {
        // Do nothing
    }
}
