using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float armorPiecePushForce = 5f;
    [SerializeField] private float armorPieceWeight = 1f;
    [SerializeField] private float armorPiecePushHeight = 0.3f;

    [Header("References")]
    [SerializeField] public List<SerializableList> armorParts;

    private int nextRemovedPieceIndex = 0;
    public void RemoveNextArmorPiece(Vector3 _hitOrigin)
    {
        Renderer[] _nextPiecesToRemove = armorParts[nextRemovedPieceIndex].renderers;

        foreach (Renderer _r in _nextPiecesToRemove)
        {
            _r.enabled = false;
            //Generate a new object with the same renderer

            GameObject _detachedArmorPart = new GameObject("DetachedArmorPart");
            _detachedArmorPart.transform.position = _r.transform.position;
            _detachedArmorPart.transform.rotation = _r.transform.rotation;
            MeshFilter _mf = _detachedArmorPart.AddComponent<MeshFilter>();
            _mf.sharedMesh = _r.GetComponent<MeshFilter>().sharedMesh;

            MeshRenderer _mr = _detachedArmorPart.AddComponent<MeshRenderer>();
            _mr.sharedMaterials = _r.sharedMaterials;
            
            _detachedArmorPart.AddComponent<SphereCollider>().radius = 0.4f;
            Rigidbody _rb = _detachedArmorPart.AddComponent<Rigidbody>();

            _detachedArmorPart.layer = LayerMask.NameToLayer("ArmorPieces");

            _rb.mass = armorPieceWeight;
            Vector3 _pushDirection = (_hitOrigin - _r.transform.position).normalized;
            _pushDirection.y += armorPiecePushHeight;
            _rb.AddForce(-_pushDirection * armorPiecePushForce);

            _detachedArmorPart.AddComponent<DetachedArmorPart>();
        }
        nextRemovedPieceIndex++;
    }

    [System.Serializable]
    public class SerializableList
    {
        public Renderer[] renderers;
    }
}
