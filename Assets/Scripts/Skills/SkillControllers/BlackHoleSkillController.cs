using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSkillController : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;

    private int amountOfAttacks;
    private float cloneAttackCoolDown;
    private float blackHoleTimer;

    private float cloneAttackTimer;
    private float offset = 1.5f;

    private List<GameObject> targets = new();
    private List<GameObject> createdHotKeys = new();

    public bool playerCanExitState {  get; private set; }

    public void SetupBlackHole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCoolDown, float _blackHoleDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCoolDown = _cloneAttackCoolDown;
        blackHoleTimer = _blackHoleDuration;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackHoleTimer -= Time.deltaTime;

        if(blackHoleTimer < 0 )
        {
            blackHoleTimer = Mathf.Infinity;
            if (targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHole();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !cloneAttackReleased)
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if (cloneAttackReleased || targets.Count <=0 ) return;
        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;
        PlayerManager.instance.player.MakeTransparent(true);
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && targets.Count > 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCoolDown;
            float xOffset = Random.Range(0, 2) == 0 ? offset : -offset;
            SkillManager.instance.clone.CreateClone(targets[Random.Range(0, targets.Count)].transform, new Vector2(xOffset, 0));

            amountOfAttacks--;
            if (amountOfAttacks <= 0)
            {
                cloneAttackReleased = false;
                Invoke(nameof(FinishBlackHole), 1f);
            }
        }
    }

    private void FinishBlackHole()
    {
        DestroyHotKeys();
        playerCanExitState = true;
        canShrink = true;
    }

    private void DestroyHotKeys()
    {
        if (createdHotKeys.Count <= 0) return;

        foreach (GameObject obj in createdHotKeys)
        {
            Destroy(obj);
        }
        createdHotKeys.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {

        if (keyCodeList.Count <= 0 || !canCreateHotKeys) return;
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);

        createdHotKeys.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        BlackHoleHotKeyController hotKeyController = newHotKey.GetComponent<BlackHoleHotKeyController>();

        hotKeyController.SetupHotKey(choosenKey, collision.gameObject, this);
    }

    public void AddEnemyToList(GameObject _enemy) => targets.Add(_enemy);
}
