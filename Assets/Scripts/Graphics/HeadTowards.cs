using UnityEngine;

[ExecuteAlways]
public class HeadTowards : MonoBehaviour
{
    public Material FaceMaterial;

    private void SetHeadDirection()
    {
        if (FaceMaterial != null)
        {
            FaceMaterial.SetVector("_HeadForward", transform.forward);
            FaceMaterial.SetVector("_HeadRight", transform.right);
        }
    }

    void Update()
    {
        SetHeadDirection();
    }
}
