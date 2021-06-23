using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SortLayerOrder : MonoBehaviour
{
    SortingGroup sortingGroup;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        sortingGroup.sortingOrder = (int)(transform.position.y * -100);
    }
}
