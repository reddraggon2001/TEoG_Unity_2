﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class TelePort : MonoBehaviour
{
    private MapEvents mapEvents;
    public WorldMaps toWorld;
    public Tilemap toMap;

    [Header("Optional landing platform")]
    public Tilemap toPlatform;

    public void Start()
    {
        mapEvents = GetComponentInParent<MapEvents>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mapEvents.Teleport(toWorld, toMap, toPlatform == null ? null : toPlatform);
    }
}