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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (_move)
        {
            var dist = Vector3.Distance(transform.position, _targetPos);
            if (dist < 2)
                _move = false;

            var inputDirection = (_targetPos - transform.position).normalized;
            var thrust = Vector3.Dot(inputDirection.normalized, transform.forward);
            var rotation = Vector3.Dot(inputDirection.normalized, transform.right);

            float rotationAmount;
            if (thrust >= 0)
                _rigidbody.AddForce(transform.forward * (thrust * inputDirection.magnitude * movementSpeed));

            if (thrust < -0.98f)
            {
                rotationAmount = rotationSpeed * rotation * 8;
            }
            else
            {
                rotationAmount = rotationSpeed * rotation;
            }
            
            _rigidbody.AddTorque(0, rotationAmount, 0);
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
