using UnityEngine;
using System.Collections;

public class ClickRock : MonoBehaviour {
      

    //handle user input to choose rock paper or scissor
    void OnMouseDown()
    {
        //boolchoose is the boolean that checks if the user can play or not.
        if (gameHandler.boolChoose)
        {
            //player chose rock
            if (gameObject.CompareTag("Rock"))
            {
                //move rock to new position
                GameObject.Find("Rock").transform.position = new Vector3(-3, 0, 0);

                //remove other options
                Destroy(GameObject.Find("Scissor"));
                Destroy(GameObject.Find("Paper"));
                
                //make users choice be 1, that is rock.
                gameHandler.choice = 1;
            }
            else if (gameObject.CompareTag("Scissor"))
            {
                GameObject.Find("Scissor").transform.position = new Vector3(-3, 0, 0);
                Destroy(GameObject.Find("Paper"));
                Destroy(GameObject.Find("Rock"));
                gameHandler.choice = 3;
            }
            else if (gameObject.CompareTag("Paper"))
            {
                GameObject.Find("Paper").transform.position = new Vector3(-3, 0, 0);
                Destroy(GameObject.Find("Rock"));
                Destroy(GameObject.Find("Scissor"));
                gameHandler.choice = 2;
            }
            gameHandler.boolChoose = false;
        }

    }
}
