using UnityEngine;

public class newRound : MonoBehaviour {

    public GameObject rock;
    public GameObject paper;
    public GameObject scissor;

    //Initalize game when player press restart.
    void OnMouseDown()
    {
        GameObject []rocks;
        GameObject []papers;
        GameObject []scissors;

        rocks = GameObject.FindGameObjectsWithTag("Rock");
        papers = GameObject.FindGameObjectsWithTag("Paper");
        scissors = GameObject.FindGameObjectsWithTag("Scissor");

        //clear screen
        foreach (GameObject thing in rocks)
        {
            Destroy(thing);
        }
        foreach (GameObject thing in papers)
        {
            Destroy(thing);
        }
        foreach (GameObject thing in scissors)
        {
            Destroy(thing);
        }


        float x = -5;
        float y = 0;

        gameHandler.boolChoose = true;  

        //fill screen
        GameObject newRock = (GameObject)Instantiate(rock, new Vector3(x, y, 0), Quaternion.identity);
        GameObject newPaper = (GameObject)Instantiate(paper, new Vector3(x + 5, y, 0), Quaternion.identity);
        GameObject newScissor = (GameObject)Instantiate(scissor, new Vector3(x + 10, y, 0), Quaternion.identity);

        newRock.name = "Rock";
        newPaper.name = "Paper";
        newScissor.name = "Scissor";



    }
}
