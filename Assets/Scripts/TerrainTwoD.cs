using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTwoD : MonoBehaviour
{
    public Camera mainCamera;

    public Texture2D terrainMap;

    public TerrainType[] TerrainTypes;

    public static Dictionary<Color32, TerrainType> TerrainTypeByColor;

    //holds all child game objects that are displaying terrain
    TerrainType[][] terrainData;

    //holds all the chunks 


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        if (TerrainTypeByColor == null)
        {
            TerrainTypeByColor = new Dictionary<Color32, TerrainType>();

            foreach (TerrainType tT in TerrainTypes)
            {
                TerrainTypeByColor.Add(tT.mapColor, tT);
            }
        }

        createTerrainSprites();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void loadTerrain()
    {

    }

    private void createTerrainSprites()
    {
        //get pixel data from image
        Color32[] colors = terrainMap.GetPixels32();
        int width = terrainMap.width;
        int height = terrainMap.height;

        TerrainType tt;
        GameObject go;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(!TerrainTwoD.TerrainTypeByColor.ContainsKey(colors[x + width * y]))
                {
                    Debug.Log("color not found in dictionary " + colors[x + width * y].ToString());
                    continue;
                }
                tt = TerrainTwoD.TerrainTypeByColor[colors[x + width * y]];
                go = new GameObject();
                go.transform.parent = gameObject.transform;
                go.transform.localPosition = new Vector3(x, y);
                go.AddComponent<SpriteRenderer>();
                go.GetComponent<SpriteRenderer>().sprite = tt.TTSprite;
                go.name = "Terrain_" + x + "_" + y;

            }
        }
    }
}

public class TerrainChunck: MonoBehaviour
{
    private Texture2D chunkMap;

    private void Start()
    {
        
    }

    private void createTerrainSprites()
    {
        //get pixel data from image
        Color32[] colors = chunkMap.GetPixels32();
        int width = chunkMap.width;
        int height = chunkMap.height;

        TerrainType tt;
        GameObject go;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tt = TerrainTwoD.TerrainTypeByColor[colors[x + width * y]];
                go = new GameObject();
                go.AddComponent<SpriteRenderer>();
                go.GetComponent<SpriteRenderer>().sprite = tt.TTSprite;
                go.name = "Terrain_" + x + "_" + y;

            }
        }


    }
}

[System.Serializable]
public class TerrainType
{
    public Sprite TTSprite;
    public bool isPassable;
    public float speedMod;
    //color used for loading from an image or display on a map;
    public Color32 mapColor;

}