using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DualPantoFramework;
using SpeechIO;
using UnityEngine;

public class LabyrinthGameManager : MonoBehaviour
{
    GameObject[] forceFields;
    GameObject forceFieldPrefab;

    GameObject upper;
    GameObject lower;
    private SpeechOut speech;
    bool isRunning = false;
    public int currentLevel = 0;
    private int maxLevel = 3;
    Vector3 startPos = new Vector3(0, 0, -4);
    PantoHandle handle;

    // level configs
    // TODO: load from JSON
    Dictionary<int, List<(Vector3, int)>> forceFieldPositions = new Dictionary<int, List<(Vector3, int)>>{
        {
            0, new List<(Vector3, int)>
            {
                { (new Vector3(2.8f, 0, -5.6f),5) },
                { (new Vector3(-0.5f, 0, -9.2f),5) },
                { (new Vector3(-2.9f, 0, -15.0f),3) },
                { (new Vector3(-5.9f, 0, -10.0f),5) },
                { (new Vector3(3.4f, 0, -13.0f),5) },
            }
        },
        {
            1, new List<(Vector3, int)>
            {
                { (new Vector3(2.8f, 0, -5.6f),5) },
                { (new Vector3(-0.5f, 0, -9.2f),5) },
                { (new Vector3(-2.9f, 0, -15.0f),3) },
                { (new Vector3(-5.9f, 0, -10.0f),5) },
                { (new Vector3(3.4f, 0, -13.0f),5) },
                { (new Vector3(0.9f, 0, -7.0f),3) },
                { (new Vector3(2.9f, 0, -9.0f),3) },
                { (new Vector3(-5.2f, 0, -13.0f),4) },
                { (new Vector3(6.4f, 0, -7.2f),3) },
            }
        },
        {
            2, new List<(Vector3, int)>
            {
                { (new Vector3(2.8f, 0, -5.6f),5) },
                { (new Vector3(-0.5f, 0, -9.2f),5) },
                { (new Vector3(-2.9f, 0, -15.0f),3) },
                { (new Vector3(-5.9f, 0, -10.0f),5) },
                { (new Vector3(3.4f, 0, -13.0f),5) },
                { (new Vector3(0.9f, 0, -14.0f),3) },
                { (new Vector3(2.9f, 0, -9.0f),3) },
                { (new Vector3(-5.2f, 0, -13.0f),4) },
                { (new Vector3(6.4f, 0, -7.2f),3) },
                { (new Vector3(-0.4f, 0, -13.2f),3) },
                { (new Vector3(-6.4f, 0, -4.2f),3) },
                { (new Vector3(7.4f, 0, -7.2f),3) },
            }
        },
        {
            3, new List<(Vector3, int)>
            {
                { (new Vector3(2.8f, 0, -5.6f),5) },
                { (new Vector3(-0.5f, 0, -9.2f),5) },
                { (new Vector3(-2.9f, 0, -15.0f),3) },
                { (new Vector3(-5.9f, 0, -10.0f),5) },
                { (new Vector3(3.4f, 0, -13.0f),5) },
                { (new Vector3(0.9f, 0, -14.0f),3) },
                { (new Vector3(2.9f, 0, -9.0f),3) },
                { (new Vector3(-5.2f, 0, -13.0f),4) },
                { (new Vector3(6.4f, 0, -7.2f),3) },
                { (new Vector3(-0.4f, 0, -13.2f),3) },
                { (new Vector3(-6.4f, 0, -4.2f),3) },
                { (new Vector3(7.4f, 0, -7.2f),3) },
                { (new Vector3(3.4f, 0, -14.2f),2) },
                { (new Vector3(-2.4f, 0, -13.7f),2) },
                { (new Vector3(5.4f, 0, -12.2f),2) },
            }
        },
    };

    void Start()
    {
        // get forcefields
        forceFields = GameObject.FindGameObjectsWithTag("ForceField");
        forceFieldPrefab = GameObject.Find("ForceField");
        forceFieldPrefab.SetActive(false);
        upper = GameObject.FindGameObjectWithTag("MeHandle");
        lower = GameObject.FindGameObjectWithTag("ItHandle");
        isRunning = true;
        speech = new SpeechOut();
        Debug.Log("Starting game");
        StartLevel();
    }

    private void SpawnForceFields()
    {
        List<(Vector3, int)> currentForceFields = forceFieldPositions[currentLevel];
        List<GameObject> ffs = new List<GameObject>();
        for (int i=0;i< currentForceFields.Count;i++)
        {
            //clone default force field
            GameObject ff = Instantiate(forceFieldPrefab);
            ff.transform.position = currentForceFields[i].Item1;
            int scale = currentForceFields[i].Item2;
            ff.transform.localScale = new Vector3(scale,scale,scale);
            ff.SetActive(true);
            ffs.Add(ff);
        }
        forceFields = ffs.ToArray();
    }

    private void DisableForceFields()
    {
        foreach(GameObject ff in forceFields)
        {
            ff.SetActive(false);
        }
    }

    async private void StartLevel()
    {
        isRunning = false;
        DisableForceFields();
        await Task.Delay(100);
        handle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        Debug.Log("Moving to start");
        await handle.MoveToPosition(startPos, 10f, true);
        Debug.Log("Moved to start");
        await speech.Speak("Level: " + currentLevel, 1);
        //spawn force fields according to level
        SpawnForceFields();
        isRunning = true;
    }

    async public void LevelCompleted()
    {
        if (isRunning)
        {
            isRunning = false;
            if (currentLevel == maxLevel)
            {
                await speech.Speak("Congratulations: You've finished all levels", 1);
                return;
            }
            await speech.Speak("Level " + currentLevel + " completed", 1);
            currentLevel++;
            StartLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if player position distance to center of any force field is too short -> lose
        foreach(GameObject ff in forceFields)
        {
            if (isRunning && (Vector3.Distance(ff.transform.position, upper.transform.position) < 0.4 || Vector3.Distance(ff.transform.position, lower.transform.position) < 0.4))
            {

                Debug.Log("Game over");
                StartLevel();
                Debug.Log("New game, new luck! :)");
            }
        }
    }
}
