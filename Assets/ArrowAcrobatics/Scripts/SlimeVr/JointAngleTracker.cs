using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointAngleTracker : MonoBehaviour
{
    // this tracker is the middle,
    // left and right are either children or the parent of this object.
    public Transform left;
    public Transform right;

    /**
     * just a nullable wrapper for float
     */
    [System.Serializable]
    public class NullableFloat
    {
        public float val;

        public NullableFloat(float v) {
            val = v;
        }

        public static implicit operator NullableFloat(float v) => new NullableFloat(v);
        public static implicit operator float(NullableFloat nf) => nf.val;
    }
    public NullableFloat angle;

    void Update() {
        if(left != null && right != null) {
            Vector3 pos = transform.position;
            angle = Vector3.Angle(left.position - pos, right.position - pos);
        } else {
            angle = null;
        }
    }
}
