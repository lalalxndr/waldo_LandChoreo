using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    //
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    //
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                int state = 0;
                if ((Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))) state = 1;

                grid[x, y] = new Node(state, worldPoint, x, y);
            }
        }
    }

    // ==== Reset node state ==== //
    public void nodeStateReset()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                if ((Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))) grid[x, y].state = 1;
            }
        }

        BlueArea.Clear();
        Dst = 0;
    }

    // ==== Green ==== //
    public List<Node> GetGreenArea(Node conner, int _GreenSizeX, int _GreenSizeY)
    {
        List<Node> GreenArea = new List<Node>();

        GreenArea.Add(conner);

        for (int x = 0; x <= _GreenSizeX; x++)
        {
            for (int y = 0; y <= _GreenSizeY; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (grid[conner.gridX + x, conner.gridY + y].state == 1)
                {
                    GreenArea.Add(grid[conner.gridX + x, conner.gridY + y]);
                }
            }
        }
        return GreenArea;
    }

    // ==== Blue ==== //
    int Dst = 0;// dst from the green building center to grid[x,y] 
    public List<Node> BlueArea = new List<Node>();

    public List<Node> GetBlueArea(Node greenCenter, int greenSize, int _multiple)
    {
        ///////
        if (BlueArea.Count < greenSize * _multiple)
        {
            Dst += 2;

            if (greenSize != 0)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int y = 0; y < gridSizeY; y++)
                    {
                        if (grid[x, y].state == 1)
                        {
                            if (GetDistance(greenCenter, grid[x, y]) < Dst)
                            {
                                grid[x, y].state = 3;
                                BlueArea.Add(grid[x, y]);
                            }
                        }
                    }
                }
            }
        }

        return BlueArea;
    }

    // ==== place pattern ==== //
    public List<Node> BlueAble = new List<Node>();

    public void PlacePattern(Node greenCenter, int patternX, int patternY)
    {
        // check which voxel is able to place pattern1
        if (BlueArea.Count > 0)
        {
            BlueAble.Clear();
            foreach (Node x in BlueArea)
            {
                bool able = true;
                for (int i = 0; i < patternX; i++)
                {
                    for (int j = 0; j < patternY; j++)
                    {
                        if (grid[x.gridX + i, x.gridY + j].state == 3 || grid[x.gridX + i, x.gridY + j].state == 4)
                        {
                            continue;
                        }
                        else
                        {
                            able = false;
                            break;
                        }
                    }
                    if (!able) break;
                }

                if (able)
                {
                    grid[x.gridX, x.gridY].state = 4;
                    BlueAble.Add(grid[x.gridX, x.gridY]);
                }
            }
        }


        if (BlueAble.Count > 0)
        {
            //
            foreach (Node x in BlueAble)
            {
                for (int i = 0; i < patternX; i++)
                {
                    for (int j = 0; j < patternY; j++)
                    {
                        if (grid[x.gridX + i, x.gridY + j].state == 3 || grid[x.gridX + i, x.gridY + j].state == 4)
                        {
                            continue;
                        }
                        else
                        {
                            grid[x.gridX, x.gridY].state = 3;
                        }
                    }
                }
            }

            // get the farest able voxel in blueable list. 
            int maxDst = 0;
            int indexOfFarest = 0;
            Node farestNode = BlueAble[0];

            foreach (Node x in BlueAble)
            {
                if (GetDistance(x, greenCenter) >= maxDst)
                {
                    maxDst = GetDistance(x, greenCenter);
                    farestNode = grid[x.gridX, x.gridY];
                    indexOfFarest = BlueAble.IndexOf(x);
                }
            }

            //Debug.Log("Grid: [ " + farestNode.gridX.ToString() + " , " + farestNode.gridY.ToString() + " ], Blueable: " + BlueAble.Count.ToString());

            // place the pattern 
            grid[farestNode.gridX, farestNode.gridY].state = 5;

            for (int i = 0; i < patternX; i++)
            {
                for (int j = 0; j < patternY; j++)
                {
                    grid[farestNode.gridX + i, farestNode.gridY + j].state = 5;
                }
            }

            for (int i = 0; i < XXX01.Count; i++)
            {
                grid[farestNode.gridX + XXX01[i], farestNode.gridY + YYY01[i]].state = 6;
            }

        }
    }

    //pattern 1
    List<int> XXX01 = new List<int> { 2, 3, 2, 3, 1, 2, 3, 4, 2, 3, 4, 5, 2, 3, 4, 5, 2, 3, 4, 5, 4 };
    List<int> YYY01 = new List<int> { 1, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7 };


    // ==== dst fome A to B  ==== //
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // ==== get node by position ==== //
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    // ==== display node ==== //
    void OnDrawGizmos()
    {
        
        if (grid != null)
        {
            foreach (Node n in grid)
            {

                if (n.state == 0) Gizmos.color = new Color(0, 0, 0, 0);// transparent 
                if (n.state == 1) Gizmos.color = Color.green / 4;//state 1 inside of area
                if (n.state == 2) Gizmos.color = Color.green;//state 2  inside of area and is Green 
                if (n.state == 3) Gizmos.color = Color.blue;//state 3 inside of area and is blue 
                if (n.state == 4) Gizmos.color = Color.blue; //state 4 inside of area and is inside of blue and can place pattern 1   
                if (n.state == 5) Gizmos.color = Color.white; //state 5 inside of area and is inside of blue and is occupied but not a building  
                if (n.state == 6) Gizmos.color = Color.black;//state 6 inside of area and is inside of blue and is occupied by a building  

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }


}