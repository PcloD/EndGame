using System;
using Pathfinding;
using UnityEngine;

[Serializable]
public class EntityEnemyTracker
{
    private Transform ourEntityTransform;
    private AIPath aStar;
    private EntityAbilityHandler abilityHandler;

    public float AttackRange = 2f;
    
    private Transform _trackedEnemyTransform;
    public Transform TrackedEnemyTransform
    {
        get { return _trackedEnemyTransform; }
        set
        {
            if (value == _trackedEnemyTransform) return;
            if (_trackedEnemyTransform && value)
            {
                // new target cancel ability
                abilityHandler.currentAbility = null;
                Debug.Log($"New target cancel ability");
            }
            _trackedEnemyTransform = value;

            if (_trackedEnemyTransform == null) return;
            aStar.destination = TrackedEnemyTransform.position;
            aStar.SearchPath();
        }
    }
    
    public EntityEnemyTracker(Transform ourTransform, AIPath astarAI, EntityAbilityHandler abilHandler)
    {
        ourEntityTransform = ourTransform;
        aStar = astarAI;
        abilityHandler = abilHandler;
    }

    private float DistanceToEnemy()
    {
        return Vector3.Distance(TrackedEnemyTransform.position, ourEntityTransform.position);
    }

    public bool IsInRange()
    {
       return DistanceToEnemy() <= AttackRange;
    }

    public bool IsTrackingAvailable()
    {
        return !IsInRange() && abilityHandler.currentAbility == null;
    }

    public void OnUpdate()
    {
        if (!TrackedEnemyTransform) return;
        
        // clear tracked as we're too far away
        if (DistanceToEnemy() > 10f)
        {
            TrackedEnemyTransform = null;
            aStar.destination = ourEntityTransform.forward + ourEntityTransform.position;
            aStar.SearchPath();
            return;
        }
        
        aStar.isStopped = !IsTrackingAvailable();

        if (!IsTrackingAvailable())
        {
            return;
        }

        aStar.destination = TrackedEnemyTransform.position;

    }

}
