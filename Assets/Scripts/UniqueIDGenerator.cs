using System;
using UnityEngine;

public class UniqueIDGenerator : MonoBehaviour
{
    // Generates a unique ID
    public static string GenerateUniqueID()
    {
        Guid uniqueID = Guid.NewGuid();
        return uniqueID.ToString();
    }
}
