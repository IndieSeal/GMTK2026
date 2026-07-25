using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionNode
{
    Vector2 position;
    List<Vector2> neighbors;

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
}

public class AStarPathfinder : MonoBehaviour
{
    public Tilemap groundTilemap;
    public List<PositionNode> allPositions = new();

    void Start()
    {
        // for testing purposes start the flood at a default spot to test generating paths
        floodSearchFindTiles(new Vector3Int(14,0,0));
    }

    void Update()
    {
        //Debug.DrawLine(Vector2.zero, Vector2.one*3, Color.green, 1f); // -0.5, -0.5 to 0.5, 0.5
        

    }

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

            // add adjacent items to search
            toSearchTiles.Add(nw);
            toSearchTiles.Add(n);
            toSearchTiles.Add(ne);
            toSearchTiles.Add(w);
            toSearchTiles.Add(e);
            toSearchTiles.Add(sw);
            toSearchTiles.Add(s);
            toSearchTiles.Add(se);
            
            Debug.DrawLine(pos, pos + Vector3Int.one, Color.green, onSearchPutLineDuration);
        }

        //Debug.Log("Flood search discovery done!");
    }
}
