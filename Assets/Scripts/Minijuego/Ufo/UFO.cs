using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{

    private Rigidbody2D _rigidbody;

    public float speed = 3f;

    private Vector2 _movement;
    private bool _facingRight = true;

    private bool _isLive = true;

    private Transform _transform;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _transform = this.transform;
    }

    void FixedUpdate()
    {
        if (_isLive != false)
        {
            float horizontalVelocity = _movement.normalized.x * speed;
            _rigidbody.velocity = new Vector2(horizontalVelocity, _rigidbody.velocity.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLive != false)
        {
            // Movement
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            _movement = new Vector2(horizontalInput, 0f);

            // Flip character
            if (horizontalInput < 0f && _facingRight == true)
            {
                Flip();
            }
            else if (horizontalInput > 0f && _facingRight == false)
            {
                Flip();
            }
        }
    }
    private void Flip()
    {
        _facingRight = !_facingRight;
        float localScaleX = transform.localScale.x;
        localScaleX = localScaleX * -1f;
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isLive)
        {
            _isLive = false;
            _rigidbody.gravityScale = 0.2f;
            Invoke("Reset", 2.0f);
        }
    }

    private void Reset()
    {
        Debug.Log("Reseteando");
        this.transform.position = new Vector3(0.469999999f, -4.17999983f, 0f);
        _rigidbody.gravityScale = 0f;
        _isLive = true;
    }
}
