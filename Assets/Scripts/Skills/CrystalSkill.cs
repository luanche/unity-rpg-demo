using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi stacking crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalList = new List<GameObject>();

    public override void useSkill()
    {
        base.useSkill();

        if (canUseMultiCrystal())
        {
            return;
        }

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            CrystalSkillController controller = currentCrystal.GetComponent<CrystalSkillController>();
            controller.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
        }
        else if (!canMoveToEnemy)
        {
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            currentCrystal.GetComponent<CrystalSkillController>()?.FinishCrystal();
        }
    }

    private bool canUseMultiCrystal()
    {

        if (canUseMultiStacks)
        {
            if (crystalList.Count > 0)
            {
                if (crystalList.Count == amountOfStacks)
                {
                    Invoke(nameof(ResetAblity), useTimeWindow);
                }
                coolDown = 0;
                GameObject crystalToSpawn = crystalList[crystalList.Count - 1];
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);
                crystalList.Remove(crystalToSpawn);
                CrystalSkillController controller = newCrystal.GetComponent<CrystalSkillController>();
                controller.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if (crystalList.Count <= 0)
                {
                    coolDown = multiStackCooldown;
                    RefillCrystal();
                }
                return true;
            }
        }
        return false;
    }

    private void RefillCrystal()
    {
        while (crystalList.Count < amountOfStacks)
        {
            crystalList.Add(crystalPrefab);
        }
    }

    private void ResetAblity()
    {
        if (coolDownTimer > 0)
        {
            return;
        }
        coolDownTimer = multiStackCooldown;
        RefillCrystal();
    }
}
