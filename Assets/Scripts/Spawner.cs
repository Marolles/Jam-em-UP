using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Transform spawnStartPoint;
    [SerializeField] private Transform doorPoint;
    [SerializeField] private Transform spawnEndPoint;
    [SerializeField] private float spawnMovementSpeed = 5f;
    public GameObject SpawnEnemy(GameObject _enemyPrefab)
    {
        //Instantiate enemy
        GameObject _spawnedObject = Instantiate(_enemyPrefab);

        if (_spawnedObject.TryGetComponent(out CharacterController _charController))
        {
            _charController.enabled = false;
        }
        _spawnedObject.transform.position = spawnStartPoint.position;
        Vector3 _forward = doorPoint.position - spawnStartPoint.position;
        _forward.y = 0f;
        _spawnedObject.transform.forward = _forward;

        //Move enemy toward doors
        float _distance = Vector3.Distance(spawnStartPoint.position, doorPoint.position);
        _spawnedObject.transform.DOMove(doorPoint.position, _distance / spawnMovementSpeed).SetEase(Ease.OutSine).OnComplete(() => MovePastDoor(_spawnedObject));

        return _spawnedObject;
    }

    private void MovePastDoor(GameObject _spawnedObject)
    {
        float _distance = Vector3.Distance(doorPoint.position, spawnEndPoint.position);
        doorAnimator.SetTrigger("Open");
        _spawnedObject.transform.DOMove(spawnEndPoint.position, _distance / spawnMovementSpeed).SetEase(Ease.InSine).OnComplete(() => EndMovement(_spawnedObject));
    }

    private void EndMovement(GameObject _spawnedObject)
    {
        if (_spawnedObject.TryGetComponent(out CharacterController _charController))
        {
            _charController.enabled = true;
        }
    }
}
