using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandPoseSetup : MonoBehaviour
{
    public Animator animator;
    public Transform rightItem;

    private Transform handPoseRigRoot; // Constraint들이 들어갈 Rig 오브젝트

    void Start()
    {
        if (animator == null || rightItem == null)
        {
            Debug.LogWarning("Animator or rightItem is missing.");
            return;
        }

        SetupHandPoseRig(); // handPoseRig GameObject 생성

        // 손가락 본들
        Transform index1 = animator.GetBoneTransform(HumanBodyBones.RightIndexProximal);
        Transform middle1 = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        Transform thumb1 = animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);

        CreateFingerConstraint("IndexFinger", index1, new Vector3(0, 0, 50));
        CreateFingerConstraint("MiddleFinger", middle1, new Vector3(0, 0, 50));
        CreateFingerConstraint("ThumbFinger", thumb1, new Vector3(0, 30, 40));
    }

    void SetupHandPoseRig()
    {
        var rigBuilder = GetComponent<RigBuilder>();
        if (rigBuilder == null)
        {
            rigBuilder = gameObject.AddComponent<RigBuilder>();
            Debug.Log("🧱 RigBuilder added.");
        }

        GameObject rigGO = new GameObject("handPoseRig");
        rigGO.transform.SetParent(transform, false);

        var rig = rigGO.AddComponent<Rig>();
        handPoseRigRoot = rigGO.transform;

        var layer = new RigLayer(rig);
        rigBuilder.layers.Add(layer);
        
        // ✅ 리깅 재빌드 (필수)
        rigBuilder.Build();
        Debug.Log("✅ RigBuilder Build() 호출 완료.");
    }


void CreateFingerConstraint(string name, Transform bone, Vector3 localEulerOffset)
{
    // 컨트롤러 생성 (손가락 기준 위치에 배치)
    GameObject ctrl = new GameObject(name + "_CTRL");

    ctrl.transform.position = bone.position;
    ctrl.transform.rotation = bone.rotation * Quaternion.Euler(localEulerOffset);

    // ✅ 손가락의 목표를 정의하는 것이므로 handPoseRig 하위로 넣는다
    ctrl.transform.SetParent(handPoseRigRoot, true);

    // Constraint 생성
    GameObject constraintGO = new GameObject(name + "_Rig");
    constraintGO.transform.SetParent(handPoseRigRoot, false);

    var constraint = constraintGO.AddComponent<MultiRotationConstraint>();
    constraint.data.constrainedObject = bone;

    var sources = new WeightedTransformArray();
    sources.Add(new WeightedTransform(ctrl.transform, 1f));
    constraint.data.sourceObjects = sources;

    constraint.weight = 1f;
}


}
