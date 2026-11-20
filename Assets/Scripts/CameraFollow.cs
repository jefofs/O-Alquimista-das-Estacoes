using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _alvo;
    [SerializeField] private float _suavizacao = 6f;

    private const float PosicaoZ = -10f;

    private void Start()
    {
        if (_alvo == null)
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                _alvo = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (_alvo == null) return;

        Vector3 posicaoDesejada = new Vector3(_alvo.position.x, _alvo.position.y, PosicaoZ);
        Vector3 posicaoSuavizada = Vector3.Lerp(transform.position, posicaoDesejada, _suavizacao * Time.deltaTime);
        posicaoSuavizada.z = PosicaoZ;
        transform.position = posicaoSuavizada;
    }
}
