using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {
    public static Board Instance;


    //settings
    [SerializeField]
    int gridSize = 9;

    [SerializeField]
    float cellSizeInWorld = 3;

    [SerializeField]
    GameObject squarePrefab = null;

    List<Square> generatedSquares = new List<Square>();


    void Awake ()
    {
        Instance = this;
        InitSquares();
    }
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void InitSquares ()
    {
        int x = 0;
        int y = 0;

        for ( int i = 0; i < gridSize; i++ )
        {
            for ( int j = 0; j < gridSize; j++ )
            {
                CreateSquare(x, y);
                y++;
            }
            y = 0;
            x++;
        }

    }

    void CreateSquare ( int x, int y )
    {
        GameObject Host = GameObject.Instantiate(squarePrefab) as GameObject;
        Host.name = "Square_" + x + "_" + y;
        Square square = Host.GetComponent<Square>();

        square.xIndex = x;
        square.yIndex = y;

        Host.transform.position = new Vector3(x * cellSizeInWorld, y * cellSizeInWorld, 0);
        Host.transform.SetParent(transform);

        generatedSquares.Add(square);
    }

    public Square GetSquare ( int x, int y )
    {
        foreach ( Square square in generatedSquares )
        {
            if ( square.xIndex == x && square.yIndex == y )
            {
                return square;
            }
        }

        return null;
    }
}
