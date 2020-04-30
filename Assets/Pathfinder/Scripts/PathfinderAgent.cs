using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VargMurtter.Pathfinder
{
    public class PathfinderAgent : MonoBehaviour
    {

        [SerializeField] private PathData _data;
        [SerializeField] private Transform _target;
        [SerializeField] private float _agentSpeed = 5f;
        [SerializeField] private float _stopOnDistance = 2f;
        [SerializeField] private float _pathRebuildFrenqucy = 0.5f;

        Vector2[,] grid;

        Vector2 startPoint;
        int startGridX;
        int startGridY;

        int[] aroundPointX = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] aroundPointY = new int[8] { 1, 1, 1, 0, 0, -1, -1, -1 };

        List<Vector2> route;


        float timer;

        private void Start()
        {
            ParseData();

            Init();

            timer = _pathRebuildFrenqucy;
        }

        private void Init()
        {
            FindStartPointOnGrid();

            route = new List<Vector2>();
            route.Add(startPoint);

            FindRoute(startGridX, startGridY);
        }

        int movePointIndex = 0;
        private void Update()
        {

            
            if (movePointIndex < route.Count) {
                transform.position = Vector2.MoveTowards(transform.position, route[movePointIndex], _agentSpeed * Time.deltaTime);

                if ((Vector2)transform.position == route[movePointIndex])
                {
                    route.RemoveAt(movePointIndex);
                    movePointIndex++;
                    
                }
            }

            BuildRoute();
        }

        private void OnDrawGizmos()
        {
            if (route != null)
            {
                Gizmos.color = Color.yellow;
                for (int i = 0; i < route.Count; i++)
                {
                    if (i + 1 < route.Count)
                        Gizmos.DrawLine(route[i], route[i + 1]);
                }
            }
        }

        private void BuildRoute()
        {

            foreach (Vector2 item in route)
            {
                print(item);
            }

            if (Vector2.Distance(route[route.Count - 1], _target.position) > _stopOnDistance)
            {
                bool breaked = false;

                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        if (grid[x, y] == route[route.Count - 1])
                        {
                            FindRoute(x, y);
                            breaked = true;
                            break;
                        }
                    }

                    if (breaked) break;
                }
            }
            else
            {
                foreach (Vector2 item in route)
                {
                    print(item);
                }
            }
        }

        private void FindRoute(int indexX, int indexY)
        {
            Vector2 min = Vector2.positiveInfinity;

            for (int i = 0; i < 8; i++)
            {
                Vector2 nextPoint = grid[indexX + aroundPointX[i], indexY + aroundPointY[i]];
                
                if (nextPoint == Vector2.positiveInfinity || route.Contains(nextPoint)) continue;
                
                if (Vector2.Distance(nextPoint, _target.position) < Vector2.Distance(min, _target.position))
                {
                    min = nextPoint;
                }

            }

            route.Add(min);
            
        }

        private void ParseData()
        {
            string[] rows = _data.GridData.Split('\n');
            for (int i = 0; i < rows.Length - 1; i++)
            {
                string[] column = rows[i].Split(';');
                for (int j = 0; j < column.Length - 1; j++)
                {
                    if (grid == null)
                        grid = new Vector2[rows.Length - 1, column.Length - 1];

                    string x = column[j].Split(',')[0];
                    string y = column[j].Split(',')[1];

                    if(x != "EL" && y != "EL")
                    {
                        grid[i, j] = new Vector2(float.Parse(x.Replace('.', ',')), float.Parse(y.Replace('.', ',')));
                    }
                    else
                    {
                        grid[i, j] = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                    }

                }
            }
        }

        private void FindStartPointOnGrid()
        {
            Vector2 min = Vector2.positiveInfinity;

            int xGrid = 0;
            int yGrid = 0;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (Vector2.Distance(transform.position, grid[x, y]) < Vector2.Distance(transform.position, min))
                    {
                        min = grid[x, y];
                        xGrid = x;
                        yGrid = y;
                    }
                }
            }

            startPoint = min;
            startGridX = xGrid; 
            startGridY = yGrid;
        }
    }
}


