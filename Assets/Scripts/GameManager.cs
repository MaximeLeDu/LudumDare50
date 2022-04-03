using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public GameObject food;
    public GameObject soap;
    public GameObject broom;
    public GameObject poop;

    public List<PoopScript> harmfulObjects;

    public int days;

    public int animalHealth;
    public bool isHappy;
    public bool isSad;
    public bool doingSetup;
    public bool isNewDay;

    public AnimalNeeds needs;
    public int satisfaction;
    private bool isGoingOnWalk = false;

    private GameObject nightObject;
    private Text dayText;
    private GameObject dayImage;
    private readonly float levelStartDelay = 2.0f;

    private readonly float timeToNight = 0.5f;

    public bool hasSpawnedObject;
    private bool hasGoneOnWalk = false;
    private GameObject objectSpawned;

    public readonly float xMin = -8;
    public readonly float xMax = 8;
    public readonly float yMin = -2;
    public readonly float yMax = 4;

    public AudioClip calmMusic;
    public AudioClip dynamicMusic;
    public AudioClip sadMusic;


    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        animalHealth = 20;
        days = 1;
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1 || SceneManager.GetActiveScene().buildIndex == 0)
            InitGame();
        else
        {
            nightObject = GameObject.Find("Night Image");
        }

    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    static private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 && Instance.isNewDay)
        {
            Instance.InitGame();
            Instance.InvokePoop();
            Instance.InvokeHarmful();
            Instance.isNewDay = false;
        }
        else if (scene.buildIndex == SceneManager.sceneCountInBuildSettings - 1 || scene.buildIndex == 0)
            Instance.InitGame();
        else
        {
            Instance.nightObject = GameObject.Find("Night Image");
        }
    }

    private void InitGame()
    {
        doingSetup = true;

        nightObject = dayImage = GameObject.Find("Night Image");
        nightObject.SetActive(false);

        dayImage = GameObject.Find("Day Image");
        dayText = GameObject.Find("Day Text").GetComponent<Text>();
        dayText.text = "Day " + days;
        dayImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);
    }

    private void InvokePoop()
    {
        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);

        //If the object is too close to the dog on spawn
        if (Mathf.Abs(x) < 1 && Mathf.Abs(y) < 1)
            x = xMax - 1;

        Vector3 initPosition = new Vector3(x, y, 0);
        Instantiate(poop, initPosition, Quaternion.identity);
    }

    private void InvokeHarmful()
    {
        int randomIndex = Random.Range(0, harmfulObjects.Count);

        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);
        //If the object is too close to the dog on spawn
        if (Mathf.Abs(x) < 1 && Mathf.Abs(y) < 1)
            x = xMax - 1;

        Vector3 initPosition = new Vector3(x, y, 0);
        Instantiate(harmfulObjects[randomIndex].gameObject, initPosition, Quaternion.identity);
    }

    void HideLevelImage()
    {
        dayImage.SetActive(false);
        Instance.doingSetup = false;
    }

    public void SpawnFood()
    {
        if (!Instance.hasSpawnedObject)
        {
            Instance.objectSpawned = Instantiate(food, new Vector3(-10, -10, 0), Quaternion.identity);
            Instance.hasSpawnedObject = true;
        }
    }

    public void SpawnSoap()
    {
        if (!Instance.hasSpawnedObject)
        {
            Instance.objectSpawned = Instantiate(soap, new Vector3(-10, -10, 0), Quaternion.identity);
            Instance.hasSpawnedObject = true;
        }
    }

    public void SpawnBroom()
    {
        if (!Instance.hasSpawnedObject)
        {
            Instance.objectSpawned = Instantiate(broom, new Vector3(-10, -10, 0), Quaternion.identity);
            Instance.hasSpawnedObject = true;
        }
    }

    public void ClickLeash()
    {
        if (!Instance.hasSpawnedObject && !Instance.hasGoneOnWalk)
        {
            Instance.hasGoneOnWalk = true;
            Instance.isGoingOnWalk = true;
            AnimalScript animal = GameObject.Find("Animal").GetComponent<AnimalScript>();
            Instance.needs = animal.needs;
            Instance.satisfaction = animal.satisfaction + 10; //Going on a walk is satisfying
            SoundManager.Instance.FadeMusic(1);
            Instance.StartCoroutine(NightShift());
        }
    }

    private void Update()
    {
        Debug.Log(Instance.animalHealth);
        Debug.Log(Instance.hasGoneOnWalk);
        if (Input.GetMouseButtonDown(1) && hasSpawnedObject)
        {
            Destroy(objectSpawned);
            hasSpawnedObject = false;
        }
            
    }

    public void EndDay()
    {
        Instance.isSad = false;
        Instance.isHappy = false;
        Instance.animalHealth -= 1;
        Instance.days += 1;
        AnimalScript animal = GameObject.Find("Animal").GetComponent<AnimalScript>();

        if (animal.IsSatisfied())
            Instance.isHappy = true;
        else
        {
            Instance.isSad = true;
            Instance.animalHealth -= 3;
        }
        if (Instance.animalHealth <= 0)
            SoundManager.Instance.FadeMusic(1);
        Instance.StartCoroutine(NightShift());
    }

    public void EndGame()
    {
        SoundManager.Instance.FadeMusic(25);
        Instance.StartCoroutine(LastNight());
    }

    public void LoadMain()
    {
        GameObject spawnManager = GameObject.Find("SpawnManager");
        spawnManager.GetComponent<SpawnManager>().isSpawning = false;
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Poop"))
            Destroy(enemy);
        SoundManager.Instance.FadeMusic(1);
        StartCoroutine(BackToMain());
    }

    IEnumerator NightShift()
    {
        Instance.nightObject.SetActive(true);
        Image night = Instance.nightObject.GetComponent<Image>();
        while(night.color.a < 1)
        {
            night.color = new Color(night.color.r, night.color.g, night.color.b, night.color.a + Time.deltaTime * timeToNight);
            yield return null;
        }

        if (Instance.isGoingOnWalk)
        {
            SoundManager.Instance.ChangeMusic(dynamicMusic);
            Instance.isGoingOnWalk = false;
            SceneManager.LoadSceneAsync(1);
        }
        else
        {

            Instance.needs = new AnimalNeeds();
            Instance.satisfaction = 0;
            Instance.hasGoneOnWalk = false;

            if (Instance.animalHealth > 0)
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                Instance.isNewDay = true;
            }
            else
            {
                SoundManager.Instance.ChangeMusic(sadMusic);
                SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1); //Ending scene is last scene
            }
        }
    }

    //Good bye good doggo ;(
    IEnumerator LastNight()
    {
        Instance.nightObject.SetActive(true);
        Image night = Instance.nightObject.GetComponent<Image>();
        while (night.color.a < 1)
        {
            night.color = new Color(night.color.r, night.color.g, night.color.b, night.color.a + Time.deltaTime * timeToNight/7f);
            yield return null;
        }
        Application.Quit();
    }

    IEnumerator BackToMain()
    {
        Instance.nightObject.SetActive(true);
        Image night = Instance.nightObject.GetComponent<Image>();
        while (night.color.a < 1)
        {
            night.color = new Color(night.color.r, night.color.g, night.color.b, night.color.a + Time.deltaTime * timeToNight/1.5f);
            yield return null;
        }
        SoundManager.Instance.ChangeMusic(calmMusic);
        SceneManager.LoadSceneAsync(0);
    }
}
