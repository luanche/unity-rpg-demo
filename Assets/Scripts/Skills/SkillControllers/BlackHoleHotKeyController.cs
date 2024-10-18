using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackHoleHotKeyController : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode hotKey;
    private TextMeshProUGUI text;

    private GameObject enemy;
    private BlackHoleSkillController blackHole;

    private bool isAdded;

    public void SetupHotKey(KeyCode _hotKey, GameObject _enemy, BlackHoleSkillController _blackHole)
    {
        sr = GetComponent<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        enemy = _enemy;
        blackHole = _blackHole;

        hotKey = _hotKey;
        text.text = hotKey.ToString();
        isAdded = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(hotKey) && !isAdded)
        {
            blackHole.AddEnemyToList(enemy);
            text.color = Color.clear;
            isAdded = true;
        }
    }
}
