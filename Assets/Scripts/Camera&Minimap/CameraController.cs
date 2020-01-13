﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    // Public
    [SerializeField] private Transform _player = null;

    [SerializeField] private KeyBindings keyBindings = null;

    [Header("Settings")]
    [SerializeField] private float _smoothing = 1f;

    [SerializeField] private float _maxCam = 20f;

    [Range(0.01f, 0.5f)]
    [SerializeField] private float zoomSpeed = 0.1f;

    [Tooltip("Less is more, changes how much out of map camera can see")]
    [Range(0.1f, 0.6f)]
    [SerializeField] private float viewLimit = 0.4f;

    [SerializeField] private Vector3 _offset = new Vector3(1f, 0, -10);

    // Private
    private Tilemap _map;

    private Camera cam;
    private float _xMax, _xMin, _yMin, _yMax;

    [SerializeField] private float _orthSize = 8f;

    private float _lastOrthSize;
    private Vector3 minTile, maxTile;

    // Start is called before the first frame update
    private void Start()
    {
        if (_player == null)
        {
            _player = PlayerMain.GetPlayer.transform;
        }
        _map = MapEvents.CurrentMap;
        MapEvents.WorldMapChange += DoorChanged;
        cam = GetComponent<Camera>();
        minTile = _map.CellToWorld(_map.cellBounds.min);
        maxTile = _map.CellToWorld(_map.cellBounds.max);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 _target = _player.transform.position + _offset;
        _target.x = Mathf.Clamp(_target.x, _xMin, _xMax);
        _target.y = Mathf.Clamp(_target.y, _yMin, _yMax);
        // if Camera controll and check if bigger than tilemap

        // Mobile zoom copied from unity learn
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // ... change the orthographic size based on the change in distance between the touches.
            _orthSize += deltaMagnitudeDiff * zoomSpeed;
        }
        if (keyBindings.zoomInKey.GetKey())
        {
            _orthSize -= zoomSpeed;
        }
        else if (keyBindings.zoomOutKey.GetKey())
        {
            _orthSize += zoomSpeed;
        }
        else
        {
            _orthSize -= Input.GetAxis("Mouse ScrollWheel"); // times zoom speed
        }
        if (_orthSize != _lastOrthSize)
        {
            _orthSize = Mathf.Clamp(_orthSize, 4, _maxCam);
            _lastOrthSize = _orthSize;
            cam.orthographicSize = _orthSize;
            TilemapLimits();
        }
        transform.position = Vector3.Lerp(transform.position, _target, _smoothing);
    }

    private void TilemapLimits()
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        _xMin = minTile.x + (width * viewLimit);
        _xMax = maxTile.x - (width * viewLimit);

        _yMin = minTile.y + (height * viewLimit);
        _yMax = maxTile.y - (height * viewLimit);

        //_maxCam = Mathf.Min((maxTile.y - minTile.y) / 2f, (maxTile.x - minTile.x) / (cam.aspect * 2f));
    }

    private void DoorChanged()
    {
        _map = MapEvents.CurrentMap;
        minTile = _map.CellToWorld(_map.cellBounds.min);
        maxTile = _map.CellToWorld(_map.cellBounds.max);
        TilemapLimits();
    }
}