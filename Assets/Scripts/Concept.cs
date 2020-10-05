using UnityEngine;
using UnityEngine.UI;

public class Concept : MonoBehaviour
{
    public string _name;
    public Text _nameText;
    [SerializeField] bool _isStatementTerm;
    [SerializeField] bool _isCompoundTerm;

    private float FIRE_MAX_TIMER = 1.0f;
    private float fireTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(string name)
    {
        _name = name;
        _nameText.text = name;
        _isStatementTerm = NarseseParser.ContainsStatementCopula(name);
        _isCompoundTerm = NarseseParser.ContainsTermConnector(name);
    }

    // Update is called once per frame
    void Update()
    {
        if(fireTimer > 0f)
        {
            fireTimer -= Time.deltaTime;
            this.GetComponent<MeshRenderer>().material.color = new Color((FIRE_MAX_TIMER - fireTimer)/FIRE_MAX_TIMER, 1, 1);
        }
    }

    public bool isStatementTerm()
    {
        return _isStatementTerm;
    }

    public bool isCompoundTerm()
    {
        return _isCompoundTerm;
    }

    public void Fire()
    {
        Grow();
        this.GetComponent<MeshRenderer>().material.color = Color.cyan;
        fireTimer = FIRE_MAX_TIMER;
    }

    public void Grow()
    {
        Transform conceptSprite = this.transform;
        conceptSprite.localScale += new Vector3(1f, 1f, 1f)* .01f;
    }

}
