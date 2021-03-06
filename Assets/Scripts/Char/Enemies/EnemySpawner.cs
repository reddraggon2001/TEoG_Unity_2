﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private PlayerMain Player;

    [SerializeField] private List<Tilemap> dontSpawnOn = new List<Tilemap>();

    private MapEvents MapEvents => MapEvents.GetMapEvents;

    [Header("Settings")]
    [SerializeField] private int enemyToAdd = 6;

    [Range(5, 25)]
    [SerializeField] private int distFromPlayer = 5;

    [Range(0, 10)]
    [SerializeField] private int distFromBorder = 2;

    // Private
    private readonly List<Vector3> _empty = new List<Vector3>();

    private readonly List<EnemyPrefab> _CurrEnemies = new List<EnemyPrefab>();
    private Tilemap _currMap;
    private readonly System.Random rnd = new System.Random();

    private void Start()
    {
        Player = Player != null ? Player : PlayerMain.GetPlayer;
        _currMap = MapEvents.CurrentMap;
        MapEvents.WorldMapChange += DoorChanged;
        CurrentEnemies();
        Movement.TriggerEnemy += RePosistion;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_empty.Count < 1) { AvailblePos(); }
        else if (transform.childCount < enemyToAdd && _CurrEnemies.Count > 0)
        {
            int index = rnd.Next(_empty.Count);
            EnemyPrefab prefab = _CurrEnemies[rnd.Next(_CurrEnemies.Count)];
            Instantiate(prefab, _empty[index], Quaternion.identity, transform).name = prefab.name;
            _empty.RemoveAt(index);
        }
    }

    private void AvailblePos()
    {
        for (int n = _currMap.cellBounds.xMin + distFromBorder; n < _currMap.cellBounds.xMax - distFromBorder; n++)
        {
            for (int p = _currMap.cellBounds.yMin + distFromBorder; p < _currMap.cellBounds.yMax - distFromBorder; p++)
            {
                Vector3Int localPlace = new Vector3Int(n, p, (int)_currMap.transform.position.z);
                if (_currMap.HasTile(localPlace) && !dontSpawnOn.Exists(t => t.HasTile(localPlace)) && AroundPlayer(localPlace))
                {
                    Vector3 place = _currMap.CellToWorld(localPlace);
                    _empty.Add(place);
                }
            }
        }
    }

    public bool AroundPlayer(Vector3 vector3) => Vector3.Distance(Player.transform.position, vector3) > distFromPlayer;

    public void RePosistion(BasicChar toRePos)
    {
        int index = rnd.Next(_empty.Count);
        int tries = 0;
        while (!AroundPlayer(_empty[index]))
        {
            index = rnd.Next(_empty.Count);
            tries++;
            if (tries > 100)
            {
                break;
            }
        }
        toRePos.transform.position = _empty[index];
        _empty.RemoveAt(index);
    }

    private void DoorChanged(Tilemap _currMap)
    {
        this._currMap = _currMap;
        AvailblePos();
        CurrentEnemies();
        transform.KillChildren();
        _empty.Clear();
    }

    private void CurrentEnemies()
    {
        _CurrEnemies.Clear();
        if (MapEvents.CurMapScript != null)
        {
            if (MapEvents.CurMapScript.Enemies.Count > 0)
            {
                _CurrEnemies.AddRange(MapEvents.CurMapScript.Enemies);
                enemyToAdd = MapEvents.CurMapScript.EnemyCount;
            }
        }

        #region old code

        /*
        switch (mapEvents.CurrentMap.name)
        {
            case "Ground-Room-Start":
                _CurrEnemies.AddRange(_startEnemies);
                break;

            case "Ground-Room-ToVillage":
                _CurrEnemies.AddRange(_roadTovillage);
                break;

            default:
                break;
        }
        */

        #endregion old code
    }
}