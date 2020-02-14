using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Members : MonoBehaviour
{
    [SerializeField] Text population;

    void Update() => population.text = transform.childCount.ToString();
}
