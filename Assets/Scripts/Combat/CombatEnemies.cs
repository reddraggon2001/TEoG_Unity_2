﻿using System.Collections.Generic;
using UnityEngine;

public class CombatEnemies : MonoBehaviour
{
    public List<BasicChar> _enemies = new List<BasicChar>();
    public CombatButtons combatButtons;
    public CombatTeam enemyTeam;

    public void AddEnemy(EnemyPrefab enemy)
    {
        combatButtons._enemies.Add(enemy);
        _enemies.Add(enemy);
        enemyTeam.StartFight(_enemies);
    }

    private void OnDisable()
    {
        _enemies.Clear();
    }
}