using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int pickupScore = 5;

    private bool picked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!picked && other.TryGetComponent(out PlayerController _player))
        {
            ScoreBoard.instance.IncreaseScore(pickupScore);
            picked = true;
            Delete();
        }
    }
    private void Delete()
    {
        transform.DOJump(transform.position, 3f, 1, 0.5f);
        transform.DOScale(0, 0.5f).SetEase(Ease.OutQuad);
        Invoke("EndDelete", 0.5f);
    }

    private void EndDelete()
    {
        Destroy(this.gameObject);
    }

}
