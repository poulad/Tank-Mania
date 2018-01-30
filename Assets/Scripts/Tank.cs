using UnityEngine;

// ReSharper disable once CheckNamespace
public class Tank : MonoBehaviour
{
    public Rigidbody2D Shell;

    private Rigidbody2D _rigidbody;

    private SpriteRenderer _spriteRenderer;

    private AudioSource _audioSource;

    private bool _fired;

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void FixedUpdate()
    {
        _audioSource.pitch = Random.Range(.9f, 1.1f);

        float moveH = Input.GetAxis("Horizontal");
        if (0 < Mathf.Abs(moveH))
        {
            _rigidbody.velocity = new Vector2(moveH, _rigidbody.velocity.y);
            bool spriteIsInRightDirection = (moveH > 0 && !_spriteRenderer.flipX) ||
                                            (moveH < 0 && _spriteRenderer.flipX);
            if (!spriteIsInRightDirection)
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        if (!_fired && Input.GetKey(KeyCode.F))
        {
            //_fired = true;
            var shellInstance = Instantiate(Shell, transform.position, Quaternion.identity);
            shellInstance.velocity = 2.5f * (_spriteRenderer.flipX ? Vector3.left : Vector3.right);
        }
    }
}
