using UnityEngine;

public class PaintGunController : MonoBehaviour
{
    public GameObject GunObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            this.GunObj.SetActive(true);
        } else
        {
            this.GunObj.SetActive(false);
        }
    }
}
