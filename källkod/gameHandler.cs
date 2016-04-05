using UnityEngine;
using System.Collections;
using System.IO;
using System;
//using Boo.Lang;
using System.Linq;
using System.Collections.Generic;

public class gameHandler : MonoBehaviour {

    /// file variables
    private static string line;
    private static List<int> dataList;
    private static int matrixSizeX = 0;
    private static int[,] xMatrix;
    private static int[,] choiceArray = new int[3, 2];

    //1 = rock 
    //2 = paper 
    //3 = scissor, 
    //0 = no choice;
    //user choice
    public static int choice = 0;

    public static bool debug = false;
    //player can choose an object
    public static bool boolChoose = true;
    public static bool gameOver = false;

    //score keeping variables
    public int pScore = 0;
    public int cScore = 0;

    public GameObject rock;
    public GameObject paper;
    public GameObject scissor;
    public GameObject button;

    //font for score and text display.
    public Font myFont;
    
    //initialize score variable
    public string score = "";

    private static bool tiedGame = false;

    //game tracking variables.
    private static int roundNumber = 0;
    private static int roundPlayerChoiceInt;
    private static int round1Choice = 0;
    private static int round2Choice = 0;

	// Use this for initialization
	void Start () {
        //loadChoices();
	}
	
	// Update is called once per frame
	void OnGUI () {
        //set text color to black and draw score.
        GUI.color = Color.black;
        GUI.Label(new Rect(250, 25, 100, 20), "You: " + pScore);
        GUI.Label(new Rect(250, 50, 100, 20), "Computer: " + cScore);
        GUI.Label(new Rect(160, 200, 400, 20), score);
	}

    void Update()
    {
        //while game is not over continue playing.
        if (!gameOver)
        {
            //if player chooses anything let ai make a choice.
            if (choice != 0)
            {
                //positions for ai choice.
                int x = 4;
                int y = 0;

                //if(debug) Debug.Log("RN: " + roundNumber);

                //let computer make their choice based on textfile with previous user data.
                int compChoice = makeChoice();

                if (debug) { Debug.Log("Choice is: " + compChoice); }

                //buffer for player choice.
                roundPlayerChoiceInt = choice;

                //See who wins
                if (compChoice % 3 + 1 == choice)
                {
                    pScore++;
                    tiedGame = false;
                }
                else if (choice % 3 + 1 == compChoice)
                {
                    cScore++;
                    tiedGame = false;
                }
                else
                {
                    tiedGame = true;
                }

                //add choices to array to save to computer for learning
                choiceArray[roundNumber, 0] = choice;

                choiceArray[roundNumber, 1] = compChoice;

                if (debug) { Debug.Log("First is: " + choiceArray[roundNumber, 0] + "Second is: " + choiceArray[roundNumber, 1]); }

                //create computers choice on screen.
                if (compChoice == 1 && rock != null)
                {
                    GameObject newRock = (GameObject)Instantiate(rock, new Vector3(x, y, 0), Quaternion.identity);
                    newRock.name = "Rock";
                    choice = 0;
                }
                if (compChoice == 2)
                {
                    GameObject newPaper = (GameObject)Instantiate(paper, new Vector3(x, y, 0), Quaternion.identity);
                    newPaper.name = "Paper";
                    choice = 0;
                }
                if (compChoice == 3)
                {
                    GameObject newScissor = (GameObject)Instantiate(scissor, new Vector3(x, y, 0), Quaternion.identity);
                    newScissor.name = "Scissor";
                    choice = 0;
                }

                if (!tiedGame)
                {
                    roundNumber++;
                }


                //if game is over, write data to computers learning file and end round/game.
                if (pScore >= 2 || cScore >= 2)
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("rawDataRPS.txt", true);

                    file.Write("[");

                    gameOver = true;

                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            //Debug.Log(choiceArray[i, j]);
                            file.Write(choiceArray[i, j]);
                        }
                        file.Write(",");


                        if (debug) { Debug.Log(","); }
                    }
                    file.Write("]");

                    file.Close();

                    roundNumber = 0;
                    
                }



            }
        }
        //game is over clear screen and write winner.
        else
        {
            //Initiate game objects
            GameObject[] rocks;
            GameObject[] papers;
            GameObject[] scissors;

            //find game objects
            rocks = GameObject.FindGameObjectsWithTag("Rock");
            papers = GameObject.FindGameObjectsWithTag("Paper");
            scissors = GameObject.FindGameObjectsWithTag("Scissor");

            //destroy game objects.
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
            Destroy(GameObject.Find("newRound"));

            //GUIStyle myStyle = new GUIStyle();
            //myStyle.font = myFont;  

            //display winner
            if (pScore > cScore)
            {
                score = "YOU WIN! PRESS SPACE TO PLAY AGAIN.";
            }
            else if(cScore > pScore)
            {
                score = "YOU LOST. PRESS SPACE TO TRY AGAIN";
            }
            else
            {
                score = "TIED GAME, COOL. PRESS SPACE TO TRY AGAIN";
            }

            

        }
        //make new game if user presses space.
        if (Input.GetKey("space") && gameOver)
        {
            gameOver = false;
            Debug.Log("HELLO");

            GameObject newButton = (GameObject)Instantiate(button, new Vector3(0, -4, 0), Quaternion.identity);
            newButton.name = "newRound";

            float x = -5;
            float y = 0;

            boolChoose = true;

            GameObject newRock = (GameObject)Instantiate(rock, new Vector3(x, y, 0), Quaternion.identity);
            GameObject newPaper = (GameObject)Instantiate(paper, new Vector3(x + 5, y, 0), Quaternion.identity);
            GameObject newScissor = (GameObject)Instantiate(scissor, new Vector3(x + 10, y, 0), Quaternion.identity);

            newRock.name = "Rock";
            newPaper.name = "Paper";
            newScissor.name = "Scissor";


            score = "";
            pScore = 0;
            cScore = 0;
        }
    }
    //machine learning choices
    public int makeChoice()
    { 
        //randomize compchoice if no options is available.
        //for example if there is no data for current play.
        System.Random rnd = new System.Random();
        int compChoice = rnd.Next(1, 4);

        //load all previous games.
        int[,] fullGameStats = loadChoices();

        //Make new arrays for stats for different rounds.
        int[] round1Stats = new int[matrixSizeX];
        int[] round2Stats = new int[matrixSizeX];
        int[] round3Stats = new int[matrixSizeX];

        //split all stats into rounds.
        for (int i = 0; i < matrixSizeX; i++)
        {
            round1Stats[i] = fullGameStats[i, 0];
            round2Stats[i] = fullGameStats[i, 1];
            round3Stats[i] = fullGameStats[i, 2];
            
        }
        //change behaviour if last round was tied. New behaviour is to randomize choice.
        if (tiedGame)
        {
            System.Random newChooser = new System.Random();
            var query = newChooser.Next(1,4);

            query = reverseChoice(query);

            compChoice = query;
        }
        else if(roundNumber == 0)
        {
            //get most common choice for that round.
            var query = (from item in round1Stats
                         group item by item into g
                         orderby g.Count() descending
                         select g.Key).First();

            

            //make the most common choice into computers choice.
            query = reverseChoice(query);
            if (debug) { Debug.Log("Query: " + query); }
            compChoice = query;
        } else if(roundNumber == 1)
        {
            //set first round choice to what the player choose for round 1.
            round1Choice = roundPlayerChoiceInt;

            //make list of all round two choices.
            List<int> newRound2Stats = new List<int>();

            for(int i = 0; i < matrixSizeX; i++)
            {
                if(round1Stats[i] == roundPlayerChoiceInt)
                {
                    newRound2Stats.Add(round2Stats[i]);

                    if (debug) { Debug.Log("Data I have: " + round2Stats[i]); }

                }
            }

            //if there is no data for the players choice on round 1 randomize choice. else choose most common choice for round 2.
            System.Random rand = new System.Random();

            var query = rand.Next(1, 4);

            if(newRound2Stats.Count > 0)
            {
                query = (from item in newRound2Stats
                         group item by item into g
                         orderby g.Count() descending
                         select g.Key).First();
                Debug.Log("choice!!!! " + query);
                
            }

            query = reverseChoice(query);
            compChoice = query;

        }
        else if(roundNumber == 2)
        {
            //save players choice for round two.
            round2Choice = roundPlayerChoiceInt;


            //make list of all round three choices.
            List<int> newRound3Stats = new List<int>();
            List<int> newRound3StatsFull = new List<int>();

            for(int i = 0; i < matrixSizeX; i++)
            {
                if(round1Stats[i] == round1Choice && round2Stats[i] == round2Choice)
                {
                    newRound3Stats.Add(round3Stats[i]);
                }
            }
            if(newRound3Stats.Count <= 0)
            {
                for(int i = 0; i < matrixSizeX; i++)
                {
                    if(round2Stats[i] == round2Choice)
                    {
                        newRound3Stats.Add(round3Stats[i]);
                    }
                }
            }

            //randomize choice if there is no previous data on round two choice and round three choice. otherwise choose most common player choice for round three.
            System.Random rand = new System.Random();
            var query = rand.Next(1, 4);

            if(newRound3Stats.Count > 0)
            {
                query = (from item in newRound3Stats
                         group item by item into g
                         orderby g.Count() descending
                         select g.Key).First();
            }
            query = reverseChoice(query);
            compChoice = query;

        }

        //if computers choice cails get random choice.
        if(compChoice == 0)
        {
            System.Random newChooser = new System.Random();
            var query = newChooser.Next(1, 4);

            query = reverseChoice(query);

            compChoice = query;
        }

        return compChoice;
    }

    //since computer chooses what the player chose, it needs to revers it to win.
    public static int reverseChoice(int query)
    {
        if(query == 3)
        {
            query = 1;
        }
        else if(query == 2)
        {
            query = 3;
        } else if(query == 1)
        {
            query = 2;
        }
        return query;
    }



    //read all data from previous games.
    public static int[,] loadChoices()
    {
        try
        {
            using (StreamReader sr = new StreamReader("rawDataRPS.txt"))
            {
                line = sr.ReadToEnd();
                if (debug) { Debug.Log(line); }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        char[] delimiterChars = { '[', ']', ',', '\n', ' ', '\t'};

        string[] notReadable = line.Split(delimiterChars);

        dataList = new List<int>();

        foreach(string s in notReadable)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string s2 = s[0].ToString();

                dataList.Add(int.Parse(s2));
            }
        }

        int matrixSizeY = 3;
        matrixSizeX = dataList.Count / 3;

        xMatrix = new int[matrixSizeX, matrixSizeY];

        int fullCounter = 0;

        for(int i = 0; i < matrixSizeX; i++)
        {
            for(int j = 0; j < matrixSizeY; j++)
            {
                xMatrix[i, j] = dataList[fullCounter];
                fullCounter++;
            }
        }

        return xMatrix;

    }
}
