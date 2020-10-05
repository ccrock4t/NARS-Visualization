using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TermLink : MonoBehaviour
{
    Concept _statement, _constituent;
    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            UpdatePositions();
        }
    }
    private void UpdatePositions()
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = _statement.transform.position;
        positions[1] = _constituent.transform.position;
        this.gameObject.GetComponent<LineRenderer>().SetPositions(positions);

        this.GetComponent<LineRenderer>().enabled = _statement.gameObject.activeSelf && _constituent.gameObject.activeSelf;
        
    }
    public void Init(Concept subject, Concept predicate)
    {
        _statement = subject;
        _constituent = predicate;

        UpdatePositions();

        Color startColor = this.gameObject.GetComponent<LineRenderer>().startColor;
        Color endColor = this.gameObject.GetComponent<LineRenderer>().endColor;
        startColor = new Color(startColor.r, startColor.g, startColor.b, 0.7f);
        endColor = new Color(endColor.r, endColor.g, endColor.b, 0.7f);
        this.gameObject.GetComponent<LineRenderer>().startColor = startColor;
        this.gameObject.GetComponent<LineRenderer>().endColor = endColor;

        initialized = true;
    }

    public bool ConnectsCompoundTerm()
    {
        return _statement.isCompoundTerm() || _constituent.isCompoundTerm();
    }
}
