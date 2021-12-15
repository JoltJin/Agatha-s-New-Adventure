using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health = 12;
    private int maxHearts = 14;
    public int numOfHearts = 3;
    
    public GameObject origin;

    public List<GameObject> heartContainers;

    public Sprite fullHeart,
                  threeQuarterHeart,
                  halfHeart,
                  quarterHeart,
                  emptyHeart;

    // Update is called once per frame
    private void Start()
    {
        health = numOfHearts * 4;
    }




    private void ClearHearts()
    {
        for (int i = 0; i < heartContainers.Count; i++)
        {
            Destroy(heartContainers[i]);
        }
        heartContainers.Clear();
    }
    void Update()
    {
        if (numOfHearts < 3)
        {
            numOfHearts = 3;
        }

        if (numOfHearts > maxHearts )
        {
            numOfHearts = maxHearts;
            ClearHearts();
        }

        if(numOfHearts < heartContainers.Count)
        {
            ClearHearts();
        }

        while (heartContainers.Count < numOfHearts)
        {
            for (int i = 0; i < numOfHearts && numOfHearts - heartContainers.Count > 0; i++)
            {
                GameObject heart = Instantiate(origin) as GameObject;
                heart.SetActive(true);

                heart.transform.SetParent(origin.transform.parent, false);

                heartContainers.Add(heart.gameObject);
            }
        }
        
        if(health > numOfHearts * 4)
        {
            health = numOfHearts * 4;
        }

        if(health < 0)
        {
            health = 0;
        }
        
        for (int i = 0; i < numOfHearts; i++)
        {
            //if(health < i * 4)
            //{
            //    //empty
            //    heartContainers[i].GetComponent<Image>().sprite = emptyHeart;
            //    continue;
            //}
            //else if(health >= i * 4)
            //{
            //    heartContainers[i].GetComponent<Image>().sprite = fullHeart;
            //    continue;
            //}
            if(health - i * 4 >= 4)
            {
                heartContainers[i].GetComponent<Image>().sprite = fullHeart;
            }
            else if (health - i * 4 == 1)
            {
                heartContainers[i].GetComponent<Image>().sprite = quarterHeart;
            }
            else if (health - i * 4 == 2)
            {
                heartContainers[i].GetComponent<Image>().sprite = halfHeart;
            }
            else if (health - i * 4 == 3)
            {
                heartContainers[i].GetComponent<Image>().sprite = threeQuarterHeart;
            }
            else
            {
                heartContainers[i].GetComponent<Image>().sprite = emptyHeart;
            }
        }
    }
}
