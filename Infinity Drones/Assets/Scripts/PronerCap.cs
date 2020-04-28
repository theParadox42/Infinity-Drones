using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PronerCap : MonoBehaviour
{
    [SerializeField] int maxProners = 20;
    int currentProners;
    public bool maxedOut = false;

    // Start is called before the first frame update
    void Start() {
        currentProners = FindObjectsOfType<Proner>().Length;
        UpdateStatus();
    }
    void UpdateStatus () {
        maxedOut = currentProners > maxProners;
        if (maxedOut) {
            Destroy(FindObjectOfType<Proner>().gameObject);
            currentProners --;
        }
    }
    public void AddProner () {
        currentProners ++;
        UpdateStatus();
    }
    public void RemoveProner () {
        currentProners --;
        UpdateStatus();
    }
}
