using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NecromancerController : Unit
{
    //speed in unit per second
    public float speed;

    private Transform NecroTransform;
    
    //prefab for the creation of minions
    public GameObject minionPrefab;

    public Camera mainCamera;

    public float regen = 10;

    private float lastRez = 1000;

    //used for drawing boxes
    bool boxDrag;
    //used to box select minions
    private Vector3 initMousePosition;
    private Vector3 currentMousePosition;
    private Rect rect;

    //texture for drawing selection box
    private static Texture2D rectTexture;
    public static Texture2D getRectTexture
    {
        get
        {
            if (rectTexture == null)
            {
                rectTexture = new Texture2D(1, 1);
                rectTexture.SetPixel(0, 0, Color.white);
                rectTexture.wrapMode = TextureWrapMode.Repeat;
                rectTexture.Apply();
            }
            return rectTexture;

        }
    }

    //list of selected minions
    List<MinionConroller> selectedUnits;

    private void Awake()
    {
        if (unitsByType == null)
        {
            unitsByType = new Dictionary<string, List<Unit>>();
        }
        if (unitsByType.ContainsKey(type) == false)
        {
            unitsByType.Add(type, new List<Unit>());
        }
        unitsByType[type].Add(this);
    }
    // Start is called before the first frame update
    new void Start()
    {
        mainCamera = Camera.main;
        NecroTransform = gameObject.GetComponent<Transform>();
        selectedUnits = new List<MinionConroller>();
        //base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        minionControl();
        resurrection();
        applyDamage(-1 * regen * Time.deltaTime);
    }

    //creates a new minion from a grave
    private void resurrection()
    {
        lastRez += Time.deltaTime;
        if(Grave.allGraves == null || Grave.allGraves.Count == 0 || lastRez < 1)
        {
            return;
        }
        Grave closest = null;
        if (Input.GetKey(KeyCode.Space))
        {
            //find the nearest grave
            foreach(Grave g in Grave.allGraves)
            {
                // if null or g is closer than closest
                if(closest == null || (g.gameObject.transform.position - gameObject.transform.position).magnitude < (closest.gameObject.transform.position - gameObject.transform.position).magnitude)
                {
                    closest = g;
                }
            }
            //use the grave
            spawnMinion(closest.gameObject.transform.position);
            closest.useGrave();
            //sacrafice life force
            applyDamage(10f);
            lastRez = 0;
        }
    }

    private void movement()
    {

        Vector3 veloctiy = Vector3.zero;
        float dTime = Time.deltaTime;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            veloctiy.y = veloctiy.y + speed * dTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            veloctiy.y = veloctiy.y - speed * dTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            veloctiy.x = veloctiy.x - speed * dTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            veloctiy.x = veloctiy.x + speed * dTime;
        }

        veloctiy.Normalize();

        NecroTransform.position = NecroTransform.position + (veloctiy * (speed * dTime));
        //Debug.Log(veloctiy.magnitude);
    }

    private void spawnMinion(Vector3 pos)
    {
        GameObject go = Instantiate(minionPrefab);
        go.transform.position = pos;
    }

    private void minionControl()
    {
        
        //left click
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("dowm");
            initMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            //Debug.Log("still");
            boxDrag = true;
            drawSelectionRectangle(initMousePosition, Input.mousePosition);//, Color.red, .1f, Color.blue);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("select");
            boxDrag = false;
            selectedUnits.Clear();
            Debug.Log("Number of minions to select from " + unitsByType["Minion"].Count);
            foreach(MinionConroller mc in unitsByType["Minion"])
            {
                //Debug.Log(initMousePosition + " " + Input.mousePosition + " " + Camera.main.WorldToScreenPoint(mc.gameObject.GetComponent<Transform>().position));
                if (pointIsContained(initMousePosition, Input.mousePosition, mainCamera.WorldToScreenPoint(mc.gameObject.GetComponent<Transform>().position)))
                {
                    selectedUnits.Add(mc);
                }
            }
            Debug.Log("number of units selected " + selectedUnits.Count);
        }

        //right click
        if (Input.GetMouseButtonUp(1))
        {
            Debug.Log("rightMouse up. size of selected " + selectedUnits.Count);
            foreach(MinionConroller mc in selectedUnits)
            {
                mc.setMoveTarget(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        //scroll wheel for zoom level
        if(Input.GetAxis("Mouse ScrollWheel") > 0f && mainCamera.orthographicSize>.5f)
        {
            mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && mainCamera.orthographicSize < 5f)
        {
            mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
        }
    }

    //checks if the point is within bounds defined by other points
    public bool pointIsContained(Vector3 firstCorner, Vector3 secondCorner, Vector3 point)
    {
        bool isContained = true;
        for(int i = 0; i < 2; i++)//check x and y dimensions
        {
            if(firstCorner[i] < secondCorner[i])//if first is less than second
            {
                if(point[i] < firstCorner[i] || point[i] > secondCorner[i])
                {
                    isContained = false;
                }
            }
            else
            {
                if (point[i] > firstCorner[i] || point[i] < secondCorner[i])
                {
                    isContained = false;
                }
            }
        }
        return isContained;
    }

    public void drawSelectionRectangle(Vector3 firstCorner, Vector3 secondCorner)//, Color mainColor, float borderThickness, Color borderColor)
    {
        //Rect mainRect = new Rect(firstCorner.x, firstCorner.y, secondCorner.x - firstCorner.x, secondCorner.y - firstCorner.y);
        rect = new Rect(firstCorner.x, Screen.height - firstCorner.y, secondCorner.x - firstCorner.x, -1 * ((Screen.height - firstCorner.y) - (Screen.height - secondCorner.y)));
    }


    public void OnGUI()
    {
        if(boxDrag == true)
        {
            //Debug.Log("Drawing");
            GUI.color = new Color(.8f, .1f, .8f, .2f);//transparent purple
            GUI.DrawTexture(rect, getRectTexture);
        }
    }

    protected override void onDeath()
    {
        Debug.Log("exitToMenu");
        SceneManager.LoadScene("OpeningScreen");
    }
}