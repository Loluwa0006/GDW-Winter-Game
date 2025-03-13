using Unity.AppUI.UI;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;

    [SerializeField] float _moveSpeed = 40.0f;

    [SerializeField] float _cooldown = 2.0f;

    [SerializeField] Timer _restartTimer;

    [SerializeField] float _minTimeUntilDrop = 0.0f;
    [SerializeField] float _maxTimeUntilDrop = 6.0f;

    [SerializeField] List<MoveableObject> _objectPrefabs = new List<MoveableObject>();
    [SerializeField] float _groundCheckerLength = 45.0f;
    [SerializeField] LayerMask _groundMask;

    Vector2 _startingPosition;

    float _timeUntilDrop = 0.0f;

    bool itemDropped = false;

    MoveableObject _itemToDrop;



    
    private void Awake()
    {
        _restartTimer.timerOver.AddListener(StartItemDropProcess);

    }
    private void Start()
    {
        _startingPosition = transform.position;
        _rb.linearVelocity = new Vector2(_moveSpeed, 0);
        StartItemDropProcess();

    }

    private void Update()
    {
        _timeUntilDrop -= Time.deltaTime;
        if (_timeUntilDrop < 0.0f && !itemDropped && OverGround())
        {
            Debug.Log("Time to drop item!");
            DropItem();
        }
    }

    void DropItem()
    {
        
        _itemToDrop.transform.parent = null;
        itemDropped = true;
        _itemToDrop._rb.gravityScale = 1.0f;
        _itemToDrop._rb.linearVelocity = Vector2.zero;
        _itemToDrop.GetComponent<Collider2D>().enabled = true;
        //do not inherit velocity of parent or else it will go flying
    }
    void StartItemDropProcess()
    {
        _timeUntilDrop = Random.Range(_minTimeUntilDrop, _maxTimeUntilDrop);
        transform.position = _startingPosition;
        itemDropped = false;

        int index = Random.Range(0, _objectPrefabs.Count);
        _itemToDrop = Instantiate(_objectPrefabs[index], transform);

        CircleCollider2D circleCollider = _itemToDrop.GetComponent<CircleCollider2D>();

        if (circleCollider)
        {
            float diameter = circleCollider.radius * 2;
            _itemToDrop.transform.localPosition = new Vector2(diameter, diameter);
            circleCollider.enabled = false;
        }
        else
        {
            BoxCollider2D boxCollider = _itemToDrop.GetComponent<BoxCollider2D>();
            if (boxCollider)
            {
                _itemToDrop.transform.localPosition = new Vector2(boxCollider.size.x, boxCollider.size.y);
                boxCollider.enabled = false;
            }
        }
        _itemToDrop._rb.gravityScale = 0.0f;
        _itemToDrop._rb.linearVelocity = _rb.linearVelocity;
    }

    bool OverGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), _groundCheckerLength, _groundMask);
        Color rayColor = Color.red;
        if (hit)
        {
            rayColor = Color.green;
        }
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - _groundCheckerLength), rayColor);
        return hit;
    }
    private void OnBecameInvisible()
    {
        _restartTimer.StartTimer(_cooldown);
    }

   
}