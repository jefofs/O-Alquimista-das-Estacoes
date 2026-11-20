using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _velocidade = 5f;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _direcaoInput;

    public Vector2 UltimaDirecao { get; private set; } = Vector2.down;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        LerInput();
        AtualizarOrientacaoVisual();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _direcaoInput * _velocidade;
    }

    private void LerInput()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            _direcaoInput = Vector2.zero;
            return;
        }

        float x = 0f;
        float y = 0f;

        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) x -= 1f;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) x += 1f;
        if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) y -= 1f;
        if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) y += 1f;

        Vector2 bruto = new Vector2(x, y);
        _direcaoInput = bruto.sqrMagnitude > 1f ? bruto.normalized : bruto;

        if (_direcaoInput.sqrMagnitude > 0.0001f)
        {
            UltimaDirecao = _direcaoInput.normalized;
        }
    }

    private void AtualizarOrientacaoVisual()
    {
        if (_spriteRenderer == null) return;

        if (_direcaoInput.x < -0.01f)
        {
            _spriteRenderer.flipX = true;
        }
        else if (_direcaoInput.x > 0.01f)
        {
            _spriteRenderer.flipX = false;
        }
    }
}
