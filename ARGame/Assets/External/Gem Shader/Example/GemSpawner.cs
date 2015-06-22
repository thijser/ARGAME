using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public Transform Gem1;
    
    public Transform Gem2;
    
    public Transform Gem3;
    
    public Transform Gem4;

    public float Spread = 5;

    public void Update()
    {
        Vector3 gemPosition = transform.position + (Random.insideUnitSphere * this.Spread);

        if (Input.GetKey("1"))
        {
            Instantiate(this.Gem1).transform.position = gemPosition;
        }

        if (Input.GetKey("2"))
        {
            Instantiate(this.Gem2).transform.position = gemPosition;
        }

        if (Input.GetKey("3"))
        {
            Instantiate(this.Gem3).transform.position = gemPosition;
        }

        if (Input.GetKey("4"))
        {
            Instantiate(this.Gem4).transform.position = gemPosition;
        }
    }
}
