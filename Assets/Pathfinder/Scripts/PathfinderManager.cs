using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


namespace VargMurtter.Pathfinder
{
    public class PathfinderManager : MonoBehaviour
    {

        [SerializeField] private Vector2 _size;
        [SerializeField] private float _frenqucy = 0.2f;
        [SerializeField] private float _sphereGizmosSize = 0.3f;
        [SerializeField] private LayerMask _mask;
        [SerializeField] private bool _drawGizmos = true;

        float gridX = 0, gridY = 0;

        List<List<Vector3>> points;
        List<List<Vector3>> walkablePoints;

        private void OnDrawGizmos()
        {

            points = new List<List<Vector3>>();

            gridX = transform.position.x;
            gridY = transform.position.y;

            for (int x = 0; x < _size.x; x++)
            {
                points.Add(new List<Vector3>());
                gridX += _frenqucy;

                for (int y = 0; y < _size.y; y++)
                {
                    points[x].Add(Vector3.zero);
                    gridY += _frenqucy;

                    Vector3 position = new Vector3(gridX, gridY, transform.position.z);

                    Gizmos.color = Color.green;

                    if(walkablePoints != null)
                    {
                        if(position != walkablePoints[x][y])
                        {
                            Gizmos.color = Color.red;
                        }
                    }

                    if(_drawGizmos)
                        Gizmos.DrawSphere(position, _sphereGizmosSize);

                    points[x][y] = position;
                }

                gridY = transform.position.y;
            }

        }

        [EditorButton]
        public void Scan()
        {

            _drawGizmos = !_drawGizmos;

            Debug.Log("Scanning started...");

            walkablePoints = new List<List<Vector3>>();

            for (int x = 0; x < points.Count; x++)
            {
                walkablePoints.Add(new List<Vector3>());
                for (int y = 0; y < points[x].Count; y++)
                {
                    walkablePoints[x].Add(Vector3.zero);

                    RaycastHit2D hit = Physics2D.Raycast(points[x][y], Vector3.forward, 100f, _mask);

                    if (hit.collider != null)
                    {
                        walkablePoints[x][y] = Vector2.positiveInfinity;
                    }
                    else
                    {
                        walkablePoints[x][y] = points[x][y];
                    }
                }
            }

            Debug.Log("Scanning completed!");

            _drawGizmos = !_drawGizmos;
        }

        [EditorButton]
        public void Bake()
        {
            Debug.Log("Baking...");

            PathData asset = ScriptableObject.CreateInstance<PathData>();

            Vector2[,] temp = new Vector2[(int)_size.x, (int)_size.y];

            for (int x = 0; x < walkablePoints.Count; x++)
            {
                for (int y = 0; y < walkablePoints[x].Count; y++)
                {
                    temp[x,y] = walkablePoints[x][y];
                }
            }

            asset.GridData = PointsToString(temp);

            AssetDatabase.CreateAsset(asset, "Assets/" + SceneManager.GetActiveScene().name + "_Baked_Grid.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;

        }

        private string PointsToString(Vector2[,] data)
        {

            string res = "";

            for (int x = 0; x < _size.x; x++)
            {
                for (int y = 0; y < _size.y; y++)
                {

                    Vector2 point = data[x, y];
                    if (point.x != float.PositiveInfinity)
                    {
                        res += string.Format("{0},{1};", point.x.ToString().Replace(',', '.'), point.y.ToString().Replace(',', '.'));
                        //print(point);
                    }
                    else
                    {
                        res += "EL,EL;";
                        print("endless");
                    }

                }

                res += "\n";
            }

            return res;
        }
    }
}


