using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionNode
{
    public Vector2 position;
    public List<Vector2> neighbors;

    // A*
    public PositionNode cameFrom;
    public float gScore;
    public float hScore;

    public PositionNode(Vector2 pos)
    {
        this.position = pos;
        this.neighbors = new();
    }

    public PositionNode(
        Vector2 nw, Vector2 n, Vector2 ne,
        Vector2 w, Vector2 pos, Vector2 e,
        Vector2 sw, Vector2 s, Vector2 se
    )
    {
        this.position = pos;
        this.neighbors = new(){ nw, n, ne, w, e, sw, s, se };
    }

    public void debugDrawMe(Color color, float duration)
    {
        Debug.DrawLine((Vector3)position, (Vector3)(position + Vector2.one), color, duration);
    }

    public float getFScore()
    {
        return gScore + hScore;
    }


    public bool equals( PositionNode other )
    {
        return other.position == this.position;
    }

    public bool contains( List<PositionNode> otherList )
    {
        for(int i=0; i<otherList.Count; i++)
        {
            PositionNode other = otherList[i];
            if(this.equals(other)){ return true; }
        }
        return false;
    }

    public float distance(PositionNode other)
    {
        return Vector2.Distance( this.position, other.position );
    }
}

public class AStarPathfinder : MonoBehaviour
{
    public static AStarPathfinder instance = null;
    public Tilemap groundTilemap;
    public List<PositionNode> allPositions = new();

    void Awake()
    {
        if(instance != null){
            Debug.Log("Already made an A* Pathfinder!!");
            Destroy(gameObject);
        }

        instance = this;
    }

    void Start()
    {
        // for testing purposes start the flood at a default spot to test generating paths
        //floodSearchFindTiles(new Vector3Int(14,0,0));

        //generatePath(allPositions[5], allPositions[allPositions.Count-5]);
    }

    void Update()
    {
        //Debug.DrawLine(Vector2.zero, Vector2.one*3, Color.green, 1f); // -0.5, -0.5 to 0.5, 0.5
    }

    void clearNodeScores()
    {
        for(int i=0; i<allPositions.Count; i++)
        {
            PositionNode pn = allPositions[i];
            pn.gScore = float.MaxValue;
            pn.cameFrom = null;
        }
    }

    public List<PositionNode> generatePath(PositionNode start, PositionNode end)
    {
        if(start == null || end == null){ return new(); }

        start.debugDrawMe(Color.blue, 0.1f);
        end.debugDrawMe(Color.aliceBlue, 0.1f);

        List<PositionNode> openSet = new();
        clearNodeScores();

        start.gScore = 0;
        start.hScore = start.distance(end);
        openSet.Add(start);

        while(openSet.Count > 0)
        {
            int lowestF = default;

            for(int i=0; i<openSet.Count; i++)
            {
                if(openSet[i].getFScore() < openSet[lowestF].getFScore())
                {
                    lowestF = i;
                }
            }

            PositionNode currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode.equals(end))
            {
                List<PositionNode> path = new();
                
                path.Insert(0, end);
                
                while(currentNode.equals(start) == false)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                    currentNode.debugDrawMe(Color.orange, 0.1f);
                }

                path.Reverse();
                return path;
            }

            List<PositionNode> neighbors = new();
            for(int i=0; i<allPositions.Count; i++)
            {
                if(currentNode.neighbors.Contains(allPositions[i].position) == false){ continue; }
                neighbors.Add(allPositions[i]);
            }
            foreach(PositionNode connectedNode in neighbors)
            {
                float heldGScore = currentNode.gScore + currentNode.distance(connectedNode);
                if(heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = connectedNode.distance(end);

                    if(!openSet.Contains(connectedNode)){ openSet.Add(connectedNode); }
                }
            }
        }

        return null;
    }

    public PositionNode getNearestNodeFromPos(Vector2 target)
    {
        Vector3Int v3i = Vector3Int.FloorToInt((Vector3)target);
        floodSearchFindTiles(v3i);

        PositionNode closest = null;
        float closestDist = float.MaxValue;
        for(int i=0; i<allPositions.Count; i++)
        {
            PositionNode pn = allPositions[i];
            float dist = Vector2.Distance(target, pn.position);
            if(dist > closestDist){ continue; }
            
            closest = pn;
            closestDist = dist;
        }

        return closest;
    }

    // lets us know what tiles we can and cannot walk on
    void floodSearchFindTiles(Vector3Int startPosition)
    {
        if(groundTilemap == null){ return; }
        float onSearchPutLineDuration = 5f;

        List<Vector3Int> toSearchTiles = new List<Vector3Int>(){ startPosition };

        for(int i=0; i<toSearchTiles.Count; i++)
        {
            if(i < 0){ break; }
            Vector3Int pos = toSearchTiles[i];
            Vector2 posVec2 = (Vector2)(Vector3)pos; // cast to a Vector3, THEN to a Vector2 because i cant do Vector3Int -> Vector2 :sob:

            Vector3Int nw = pos+Vector3Int.up+Vector3Int.left;
            Vector3Int n = pos+Vector3Int.up;
            Vector3Int ne = pos+Vector3Int.up+Vector3Int.right;
            Vector3Int w = pos+Vector3Int.left;
            Vector3Int e = pos+Vector3Int.right;
            Vector3Int sw = pos+Vector3Int.down+Vector3Int.left;
            Vector3Int s = pos+Vector3Int.down;
            Vector3Int se = pos+Vector3Int.down+Vector3Int.right;

            PositionNode pNode = new PositionNode(
                (Vector2)(Vector3)nw, (Vector2)(Vector3)n, (Vector2)(Vector3)ne,
                (Vector2)(Vector3)w, posVec2, (Vector2)(Vector3)e,
                (Vector2)(Vector3)sw, (Vector2)(Vector3)s, (Vector2)(Vector3)se
            );
            
            if(pNode.contains(allPositions)){ continue; } // we already searched this point, move on

            TileBase tile = groundTilemap.GetTile(pos);
            if(tile == null){ continue; } // empty tile, nothing to see here

            /*
            // we dont actually need to remove it since we are just going to increment `i` and continue on
            toSearchTiles.RemoveAt(i);
            i--; // preserve the index in our loop
            */
            allPositions.Add( pNode );
            pNode.debugDrawMe(Color.green, onSearchPutLineDuration);

            // add adjacent items to search
            toSearchTiles.Add(nw);
            toSearchTiles.Add(n);
            toSearchTiles.Add(ne);
            toSearchTiles.Add(w);
            toSearchTiles.Add(e);
            toSearchTiles.Add(sw);
            toSearchTiles.Add(s);
            toSearchTiles.Add(se);
        }

        //Debug.Log("Flood search discovery done!");
    }
}
