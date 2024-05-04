using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowForPlayer : MonoBehaviour
{
    [SerializeField]
    public GameObject ArrowSprite;
    public GameObject ArrowImage;

    public bool trigered = false;

    public void Init()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        trigered = true;
    }

    private void Update()
    {
        if (!trigered)
        {
            Vector3 rotate = transform.eulerAngles;
            Vector3 normPos = (transform.position - IGame.Instance.playerController.transform.position).normalized;
            float yAngle = Mathf.Acos(normPos.z) * Mathf.Rad2Deg; 
            if (normPos.x < 0)
            {
                yAngle = -yAngle;
            }
            rotate.x = 90;
            rotate.y = yAngle;
            ArrowSprite.transform.rotation = Quaternion.Euler(rotate);

            ArrowSprite.transform.position = IGame.Instance.playerController.transform.position + new Vector3(0, 1, 0) + normPos*3;
        }
        else
            Destroy(gameObject);
    }
}
