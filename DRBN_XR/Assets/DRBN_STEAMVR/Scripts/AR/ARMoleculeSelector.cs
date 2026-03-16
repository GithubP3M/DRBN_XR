using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

public class ARMoleculeSelector : MonoBehaviour
{
    [Header("XML Data")]
    public string xmlFilePath = "XML/PrefabList";

    [Header("UI References")]
    public Transform scrollViewContent;
    public Button addButton;
    public Button removeButton;
    public GameObject moleculeEntryPrefab;

    [Header("Spawn")]
    public float spawnDistance = 0.5f;

    [Header("Glass Cube")]
    public float cubeSize = 0.4f;
    public Color glassColor = new Color(0.5f, 0.8f, 1f, 0.08f);
    public Color edgeColor = new Color(0.5f, 0.8f, 1f, 0.3f);
    public float wallThickness = 0.02f;

    [Header("Temperature")]
    public Slider temperatureSlider;
    public TextMeshProUGUI temperatureLabel;
    public float minTemperature = 0f;
    public float maxTemperature = 1000f;

    [Header("Colors")]
    public Color selectedColor = new Color(0.3f, 0.8f, 0.3f, 0.8f);
    public Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    public Color removeColor = new Color(1f, 0.4f, 0.4f, 0.6f);

    class MolData { public string name; public Sprite image; public GameObject prefab, uiEntry; public bool selected; }

    List<MolData> molecules = new List<MolData>();
    List<GameObject> spawned = new List<GameObject>();
    Dictionary<GameObject, bool> removeSelections = new Dictionary<GameObject, bool>();
    GameObject glassCube;
    Vector3 cubeCenter;
    Langevin_v2 langevin;
    bool removeMode;

    readonly Color coldCol = new Color(0.2f, 0.4f, 1f);
    readonly Color warmCol = new Color(1f, 0.8f, 0f);
    readonly Color hotCol  = new Color(1f, 0.15f, 0.05f);


    void Start()
    {
        LoadXML();
        ShowAddView();

        if (addButton != null)    addButton.onClick.AddListener(OnAdd);
        if (removeButton != null) removeButton.onClick.AddListener(OnRemove);

        if (temperatureSlider != null)
        {
            temperatureSlider.minValue = minTemperature;
            temperatureSlider.maxValue = maxTemperature;
            temperatureSlider.value = 0f;
            temperatureSlider.onValueChanged.AddListener(OnTempChanged);
            OnTempChanged(0f);
        }
        langevin = FindObjectOfType<Langevin_v2>();
        if (langevin == null)
        {
            langevin = new GameObject("Langevin_Simulation").AddComponent<Langevin_v2>();
            langevin.GOS = new List<Rigidbody>();
        }
    }

    void LoadXML()
    {
        var xml = Resources.Load<TextAsset>(xmlFilePath);
        if (xml == null) return;

        var doc = new XmlDocument();
        try { doc.LoadXml(xml.text); } catch { return; }

        var nodes = doc.SelectNodes("Images/Image");
        if (nodes == null) return;

        foreach (XmlNode n in nodes)
        {
            string name = n.Attributes["name"]?.Value, img = n.Attributes["path"]?.Value, pfb = n.Attributes["prefab"]?.Value;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pfb)) continue;
            var prefab = Resources.Load<GameObject>(pfb);
            if (prefab == null) continue;
            molecules.Add(new MolData { name = name, image = !string.IsNullOrEmpty(img) ? Resources.Load<Sprite>(img) : null, prefab = prefab });
        }
        molecules = molecules.OrderBy(m => m.name).ToList();
    }


    void ClearList()
    {
        if (scrollViewContent == null) return;
        for (int i = scrollViewContent.childCount - 1; i >= 0; i--)
            Destroy(scrollViewContent.GetChild(i).gameObject);
    }

    void SetupLayout()
    {
        var lg = scrollViewContent.GetComponent<VerticalLayoutGroup>();
        if (lg == null) return;
        lg.spacing = 8f;
        lg.padding = new RectOffset(10, 10, 10, 10);
        lg.childForceExpandHeight = false;
        lg.childControlHeight = false;
    }

    GameObject CreateEntry(string label, Transform parent)
    {
        var entry = Instantiate(moleculeEntryPrefab, parent);
        var le = entry.GetComponent<LayoutElement>() ?? entry.AddComponent<LayoutElement>();
        le.minHeight = le.preferredHeight = 80f;

        var tmp = entry.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = label;
        else { var t = entry.GetComponentInChildren<Text>(); if (t != null) t.text = label; }

        var btn = entry.GetComponent<Button>() ?? entry.AddComponent<Button>();
        var bg = entry.GetComponent<Image>();
        if (bg != null) { bg.raycastTarget = true; btn.targetGraphic = bg; }

        return entry;
    }

    void SetEntryColor(GameObject entry, Color color)
    {
        var bg = entry.GetComponent<Image>();
        if (bg != null) bg.color = color;
    }


    void ShowAddView()
    {
        if (scrollViewContent == null || moleculeEntryPrefab == null) return;
        ClearList();
        SetupLayout();
        removeMode = false;

        foreach (var mol in molecules)
        {
            var entry = CreateEntry(mol.name, scrollViewContent);
            entry.name = "Entry_" + mol.name;
            mol.uiEntry = entry;

            var images = entry.GetComponentsInChildren<Image>();
            if (mol.image != null)
            {
                int idx = images.Length > 1 ? 1 : 0;
                images[idx].sprite = mol.image;
                images[idx].preserveAspect = true;
            }

            var captured = mol;
            entry.GetComponent<Button>().onClick.AddListener(() => { captured.selected = !captured.selected; SetEntryColor(captured.uiEntry, captured.selected ? selectedColor : normalColor); });
            SetEntryColor(entry, mol.selected ? selectedColor : normalColor);
        }
    }

    void ShowRemoveView()
    {
        if (scrollViewContent == null || moleculeEntryPrefab == null) return;
        ClearList();
        SetupLayout();
        removeMode = true;
        removeSelections.Clear();
        spawned.RemoveAll(m => m == null);

        if (spawned.Count == 0) { ShowAddView(); return; }

        foreach (var obj in spawned)
        {
            var entry = CreateEntry(obj.name, scrollViewContent);
            entry.name = "Remove_" + obj.name;
            SetEntryColor(entry, normalColor);
            removeSelections[obj] = false;
            var originalMol = molecules.FirstOrDefault(m => obj.name.StartsWith(m.name));
            
            if (originalMol != null && originalMol.image != null)
            {
                var images = entry.GetComponentsInChildren<Image>();
                int idx = images.Length > 1 ? 1 : 0;
                images[idx].sprite = originalMol.image;
                images[idx].preserveAspect = true;
            }
            var cap = obj; var capEntry = entry;
            entry.GetComponent<Button>().onClick.AddListener(() =>
            {
                removeSelections[cap] = !removeSelections[cap];
                SetEntryColor(capEntry, removeSelections[cap] ? removeColor : normalColor);
            });
        }
    }


    void OnAdd()
    {
        if (removeMode) { ShowAddView(); return; }
        SpawnSelected();
    }

    void OnRemove()
    {
        if (removeMode) { DespawnSelected(); return; }
        ShowRemoveView();
    }

 

    void SpawnSelected()
    {
        var sel = molecules.Where(m => m.selected).ToList();
        if (sel.Count == 0) return;

        if (glassCube == null)
        {
            var cam = Camera.main.transform;
            cubeCenter = cam.position + cam.forward * spawnDistance;
            BuildGlassCube(cubeCenter);
        }

        float r = cubeSize * 0.3f;
        foreach (var m in sel)
        {
            var pos = cubeCenter + new Vector3(Random.Range(-r, r), Random.Range(-r, r), Random.Range(-r, r));
            var obj = Instantiate(m.prefab, pos, Quaternion.identity);
            obj.name = m.name ;
            FitInCube(obj);
            RegisterPhysics(obj);
            spawned.Add(obj);
        }

        foreach (var m in molecules) { m.selected = false; SetEntryColor(m.uiEntry, normalColor); }
    }

    void DespawnSelected()
    {
        var toRemove = removeSelections.Where(kv => kv.Value && kv.Key != null).Select(kv => kv.Key).ToList();

        foreach (var obj in toRemove)
        {
            if (langevin != null)
                foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
                    langevin.GOS.Remove(rb);
            spawned.Remove(obj);
            Destroy(obj);
        }
        removeSelections.Clear();

        if (spawned.Count == 0 && glassCube != null) { Destroy(glassCube); glassCube = null; }

        if (spawned.Count > 0) ShowRemoveView(); else ShowAddView();
    }


    void FitInCube(GameObject mol)
    {
        var rends = mol.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return;

        var b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        float max = Mathf.Max(b.size.x, b.size.y, b.size.z);
        float target = cubeSize * 0.3f;
        if (max > target && max > 0f) mol.transform.localScale *= target / max;
    }

    void BuildGlassCube(Vector3 c)
    {
        glassCube = new GameObject("GlassCube");
        glassCube.transform.position = c;

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(glassCube.transform);
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localScale = Vector3.one * cubeSize;
        Destroy(cube.GetComponent<Collider>());

        var rend = cube.GetComponent<Renderer>();
        if (rend != null)
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.color = glassColor;
            mat.SetFloat("_Glossiness", 0.95f);
            mat.SetFloat("_Metallic", 0.1f);
            rend.material = mat;
        }

        float h = cubeSize / 2f;
        AddWall(c + Vector3.up * h,      new Vector3(cubeSize, wallThickness, cubeSize));
        AddWall(c + Vector3.down * h,    new Vector3(cubeSize, wallThickness, cubeSize));
        AddWall(c + Vector3.left * h,    new Vector3(wallThickness, cubeSize, cubeSize));
        AddWall(c + Vector3.right * h,   new Vector3(wallThickness, cubeSize, cubeSize));
        AddWall(c + Vector3.forward * h, new Vector3(cubeSize, cubeSize, wallThickness));
        AddWall(c + Vector3.back * h,    new Vector3(cubeSize, cubeSize, wallThickness));

        BuildEdges(c);
    }

    void AddWall(Vector3 pos, Vector3 size)
    {
        var wall = new GameObject("Wall");
        wall.transform.SetParent(glassCube.transform);
        wall.transform.position = pos;
        wall.AddComponent<BoxCollider>().size = size;
    }

    void BuildEdges(Vector3 c)
    {
        float h = cubeSize / 2f;
        var co = new Vector3[] {
            c+new Vector3(-h,-h,-h), c+new Vector3(h,-h,-h), c+new Vector3(h,h,-h), c+new Vector3(-h,h,-h),
            c+new Vector3(-h,-h,h),  c+new Vector3(h,-h,h),  c+new Vector3(h,h,h),  c+new Vector3(-h,h,h)
        };
        int[,] e = { {0,1},{1,2},{2,3},{3,0}, {4,5},{5,6},{6,7},{7,4}, {0,4},{1,5},{2,6},{3,7} };
        var mat = new Material(Shader.Find("Sprites/Default"));

        for (int i = 0; i < 12; i++)
        {
            var lr = new GameObject("Edge").AddComponent<LineRenderer>();
            lr.transform.SetParent(glassCube.transform);
            lr.positionCount = 2;
            lr.SetPositions(new[] { co[e[i,0]], co[e[i,1]] });
            lr.startWidth = lr.endWidth = 0.002f;
            lr.startColor = lr.endColor = edgeColor;
            lr.material = mat;
            lr.useWorldSpace = true;
        }
    }


    void RegisterPhysics(GameObject mol)
    {
        var rbs = mol.GetComponentsInChildren<Rigidbody>();
        
        foreach (var rb in rbs)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.maxLinearVelocity = 2f;
        }
        
        if (rbs.Length > 0 && langevin != null) 
        {
            langevin.GOS.AddRange(rbs);
        }
    }

    void OnTempChanged(float val)
    {
        Langevin_v2.temp = val;
        Langevin_v2.sigma = System.Math.Sqrt(6.0 * Langevin_v2.friction * Langevin_v2.kB * val / Langevin_v2.dt);
        Langevin_v2.sigmaf = (float)Langevin_v2.sigma;

        if (temperatureLabel != null)
            temperatureLabel.text = val.ToString("F0") + " K";

        if (temperatureSlider == null) return;

        float t = Mathf.InverseLerp(minTemperature, maxTemperature, val);
        Color col = t < 0.5f ? Color.Lerp(coldCol, warmCol, t * 2f) : Color.Lerp(warmCol, hotCol, (t - 0.5f) * 2f);

        var fill = temperatureSlider.fillRect?.GetComponent<Image>();
        if (fill != null) fill.color = col;
        if (temperatureLabel != null) temperatureLabel.color = col;
    }
}
