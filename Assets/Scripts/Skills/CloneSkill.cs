using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;
    [Space]
    [SerializeField] private bool createCloneOnDashStart;
    [SerializeField] private bool createCloneOnDashOver;
    [SerializeField] private bool createCloneOnCounterAttack;

    [Header("Clone can duplicate")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float changeToDuplicate;

    [Header("Crystal instead of clone")]
    public bool crystalInsteadOfClone;

    public void CreateClone(Transform _cloneTransform, Vector3 _offset = default)
    {

        if (crystalInsteadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }
        GameObject newClone = Instantiate(clonePrefab);
        newClone.GetComponent<CloneSkillController>().SetupClone(_cloneTransform, cloneDuration, canAttack, canDuplicateClone, changeToDuplicate, _offset, FindClosestEnemy(_cloneTransform));
    }

    private IEnumerator CreateCloneEnumerator(float _delay, Transform _cloneTransform, Vector3 _offset = default)
    {
        yield return new WaitForSeconds(_delay);
        CreateClone(_cloneTransform, _offset);
    }

    public void CreateCloneWithDelay(float _delay, Transform _cloneTransform, Vector3 _offset = default)
    {
        StartCoroutine(CreateCloneEnumerator(_delay, _cloneTransform, _offset));
    }

    public void CreateCloneOnDashStart()
    {
        if (createCloneOnDashStart)
        {
            CreateClone(player.transform);
        }
    }

    public void CreateCloneOnDashOver()
    {
        if (createCloneOnDashOver)
        {
            CreateClone(player.transform);
        }
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (createCloneOnCounterAttack)
        {
            CreateCloneWithDelay(.4f, _enemyTransform, new Vector3(2 * player.facingDir, 0, 0));
        }
    }
}
