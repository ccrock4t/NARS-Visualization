using UnityEngine;
using UnityEngine.UI;

public class Concept : MonoBehaviour
{
    public string _name;
    public Text _nameText;
    [SerializeField] bool _isStatementTerm;
    [SerializeField] bool _isCompoundTerm;

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
        
    }

    public bool isStatementTerm()
    {
        return _isStatementTerm;
    }

    public bool isCompoundTerm()
    {
        return _isCompoundTerm;
    }

}
