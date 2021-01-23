using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
	public int rows = 5; 
	public int cols = 5; 
	public GameObject Wall; 
	public GameObject Cell; 

	private MazeCell[,] grid; 
    private int currRow = 0; 
    private int currCol = 0; 
    private bool mazeDone = false; 

    // Start is called before the first frame update
    void Start()
    {
        //sets up grid with all walls filled in
        GridSetup(); 

        //generates maze path by deleting walls
        GenerateMaze();      

    }

    void GridSetup() {

        float size = Wall.transform.localScale.x; 
        grid = new MazeCell[rows, cols]; 

        List<GameObject> horWalls = new List<GameObject>(); 
        List<GameObject> vertWalls = new List<GameObject>(); 

        for (int i=0; i < rows; i++) {
            for (int j=0; j < cols; j++) {

                GameObject cell = Instantiate(Cell, new Vector3(j*size + 330, 205, (-i * size) + 225), Quaternion.identity); 
                cell.name = "Cell (" + i + "," + j + ")"; 
                cell.transform.parent = transform; 
                cell.tag = "cell"; 

                GameObject upWall = Instantiate(Wall, new Vector3(j*size + 330, 205,-i * size + 233), Quaternion.identity);
                upWall.name = "UpWall (" + i + ":" + j + ")";
                upWall.transform.parent = transform; 

                GameObject downWall = Instantiate(Wall, new Vector3(j*size + 330, 205,-i * size + 218), Quaternion.identity);
                downWall.name = "DownWall (" + i + ":" + j + ")";
                downWall.transform.parent = transform; 

                GameObject leftWall = Instantiate(Wall, new Vector3(j*size + 323, 205,-i * size + 225), Quaternion.Euler(0,90,0));
                leftWall.name = "LeftWall (" + i + ":" + j + ")";
                leftWall.transform.parent = transform; 

                GameObject rightWall = Instantiate(Wall, new Vector3(j*size + 338, 205,-i * size + 225), Quaternion.Euler(0,90,0));
                rightWall.name = "RightWall (" + i + ":" + j + ")";
                rightWall.transform.parent = transform; 

                grid[i,j] = new MazeCell(); 
                grid[i,j].upWall = upWall; 
                grid[i,j].downWall = downWall;
                grid[i,j].leftWall = leftWall;
                grid[i,j].rightWall = rightWall;

                if (i == 0 && j == 0) {
                    Destroy(leftWall);
                }

                if (i == rows -1 && j == cols - 1) {
                    Destroy(rightWall);
                }

            }
        }

    }

    void GenerateMaze() {

        //marks start cell as visited
        grid[currRow, currCol].visited = true;

        //while there are still unvisited cells in the grid
        while (!mazeDone) { 

            //start from current cell and walk around to unvisited neighbors until you reach a dead-end
            MazePathPhase1(); 

            //select an unvisited cell that has a visited neighbor, and destroy wall between it
            MazePathPhase2(); 

            mazeDone = isMazeDone(); 
        }        
        
    }


    void MazePathPhase1() {

        while (UnvisitedNeighbors()) {

            //chooses which direction to walk in from current cell
            int direction = Random.Range(0,4); 

            //checks upward wall
            if (direction == 0) {
                if (UnvisitedCellInBounds(currRow - 1, currCol)) {

                    if (grid[currRow,currCol].upWall) {
                        Destroy(grid[currRow,currCol].upWall); 
                    }
                    
                    currRow--; 
                    grid[currRow,currCol].visited = true; 

                    if (grid[currRow,currCol].downWall) {
                        Destroy(grid[currRow,currCol].downWall); 
                    }
                }
            }

            // checks downward wall
            else if (direction == 1) {
                if (UnvisitedCellInBounds(currRow + 1, currCol)) {

                    if (grid[currRow,currCol].downWall) {
                        Destroy(grid[currRow,currCol].downWall); 
                    }

                    currRow++; 
                    grid[currRow,currCol].visited = true; 

                    if (grid[currRow,currCol].upWall) {
                        Destroy(grid[currRow,currCol].upWall); 
                    }
                }
            }

            //checks left wall 
            else if (direction == 2) {
                if (UnvisitedCellInBounds(currRow, currCol -1)) {

                    if (grid[currRow,currCol].leftWall) {
                        Destroy(grid[currRow,currCol].leftWall); 
                    }

                    currCol--; 
                    grid[currRow,currCol].visited = true; 

                    if (grid[currRow,currCol].rightWall) {
                        Destroy(grid[currRow,currCol].rightWall); 
                    }
                }

            }

            //checks right wall
            else if (direction == 3) {
                if (UnvisitedCellInBounds(currRow, currCol + 1)) {

                    if (grid[currRow,currCol].rightWall) {
                        Destroy(grid[currRow,currCol].rightWall); 
                    }

                    currCol++; 
                    grid[currRow,currCol].visited = true; 

                    if (grid[currRow,currCol].leftWall) {
                        Destroy(grid[currRow,currCol].leftWall); 
                    }
                }

            }
        }

        
    }

    void MazePathPhase2() {

        mazeDone = true; 

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {

                //find an unvisited cell that has visited neighbors
                if (!(grid[i,j].visited) && VisitedNeighbors(i,j)) { 

                    mazeDone = false; 

                    //go to that cell
                    currRow = i; 
                    currCol = j;  

                    //mark it visited
                    grid[currRow, currCol].visited = true; 

                    //destroy wall between itself and and its visited neighbor
                    DestroyAdjacentWall(); 

                    return; 
                }
            }
        }

    }

    bool UnvisitedNeighbors() {

        //check upward
        if (UnvisitedCellInBounds(currRow - 1, currCol)) {
            return true; 
        }

        //check down
        if (UnvisitedCellInBounds(currRow + 1, currCol)) {
            return true; 
        }

        //check left 
        if (UnvisitedCellInBounds(currRow, currCol + 1)) {
            return true; 
        }

        //check right 
        if (UnvisitedCellInBounds(currRow, currCol - 1)) {
            return true; 
        }

        return false; 
    }

    //check if the cell has been visited, and if it is within the gridspace
    bool UnvisitedCellInBounds(int row, int col) {

        //check boundaries of cell and visited boolean
        if (row >= 0 && row < rows &&  col >= 0 && col < cols && !grid[row,col].visited) {
            return true; 
        }

        return false; 
    }

    public bool VisitedNeighbors(int row, int col) {

        //check upward
        if (row > 0 && grid[row - 1, col].visited) {
            return true; 
        }

        //check downward
        if (row < rows && grid[row + 1, col].visited) {
            return true; 
        }

        //check left
        if (col > 0 && grid[row, col - 1].visited) {
            return true; 
        }

        //check left
        if (col < cols && grid[row, col + 1].visited) {
            return true; 
        }


        return false; 
    }

    void DestroyAdjacentWall() {

        bool destroyed = false; 

        while (!destroyed) {

            int direction = Random.Range(0,4); 

            //destroy up
            if (direction == 0) { 

                if (currRow > 0 && grid[currRow - 1, currCol].visited) {
                    if (grid[currRow, currCol].upWall) {
                         Destroy(grid[currRow, currCol].upWall);
                    }
                    if (grid[currRow - 1, currCol].downWall) {
                        Destroy(grid[currRow - 1, currCol].downWall); 
                    }
                   
                    destroyed = true; 
                }

            }

            //down
            else if (direction == 1) {

                if (currRow < rows - 1 && grid[currRow + 1, currCol].visited) {
                    if (grid[currRow, currCol].downWall) {
                         Destroy(grid[currRow, currCol].downWall);
                    }
                    if (grid[currRow + 1, currCol].upWall) {
                        Destroy(grid[currRow + 1, currCol].upWall); 
                    }

                    destroyed = true; 
                }

            }

            //left
            else if (direction == 2) {

                if (currCol > 0 && grid[currRow, currCol - 1].visited) {
                    if (grid[currRow, currCol].leftWall) {
                         Destroy(grid[currRow, currCol].leftWall);
                    }
                    if (grid[currRow, currCol - 1].rightWall) {
                        Destroy(grid[currRow, currCol - 1].rightWall); 
                    }
                    destroyed = true; 
                }

            }

            //right
            else if (direction == 3) {

                if (currCol < cols - 1 && grid[currRow, currCol + 1].visited) {
                    if (grid[currRow, currCol].rightWall) {
                         Destroy(grid[currRow, currCol].rightWall);
                    }
                    if (grid[currRow, currCol + 1].leftWall) {
                        Destroy(grid[currRow, currCol + 1].leftWall); 
                    }
                    destroyed = true; 
                    
                }

            }

        }
        
    }

    bool isMazeDone() {

        bool done = true; 

        for (int i = 0; i < rows ; i++) {
            for (int j = 0; j < cols; j++) {
                if (!grid[i,j].visited) {
                    done = false; 
                }
            }
        }
        return done; 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
