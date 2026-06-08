using UnityEngine;

public class PartyUIPrefabMarker : MonoBehaviour
{
    [SerializeField] private PartyUIPrefabType prefabType;

    public PartyUIPrefabType PrefabType => prefabType;
}
