using UnityEngine;
using Lean.Touch;

public class TestShip : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;
    public bool isSelected;
    
    private Rigidbody _rigidbody;
    
    private Vector3 _targetPos;
    private bool _move;

    private Vector3 _lastMove;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_move)
        {
            var dist = Vector3.Distance(transform.position, _targetPos);
            if (dist < 4)
                _move = false;

            var inputDirection = (_targetPos - transform.position).normalized;
            var thrust = Vector3.Dot(inputDirection.normalized, transform.forward);
            var rotation = Vector3.Dot(inputDirection.normalized, transform.right);

            float rotationAmount;
            if (thrust >= 0)
            {
                _lastMove = transform.forward * (thrust * inputDirection.magnitude * movementSpeed);
                _rigidbody.AddForce(_lastMove);
            }

            if (thrust < 0.999f)
            {
                if (rotation < 0)
                    rotationAmount = -rotationSpeed;
                else
                    rotationAmount = rotationSpeed;
                _rigidbody.AddTorque(0, rotationAmount, 0);
            }
        }
        else
        {
            _lastMove *= 0.95f;
            _rigidbody.AddForce(_lastMove);
        }
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += HandleFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= HandleFingerTap;
    }
    
    void HandleFingerTap(LeanFinger finger)
    {
        if (Physics.Raycast(finger.GetRay(), out var hit, Mathf.Infinity))
        {
            var testShip = hit.collider.GetComponent<TestShip>();
            if (testShip != null)
            {
                if (hit.collider.GetComponent<TestShip>() == this)
                {
                    var allShips = FindObjectsOfType<TestShip>();
                    foreach (var ship in allShips)
                    {
                        ship.isSelected = false;
                    }

                    isSelected = true;
                }
            }
            else
            {
                if (!isSelected) return;
                _targetPos = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                _move = true;
            }
        }
    }
}
