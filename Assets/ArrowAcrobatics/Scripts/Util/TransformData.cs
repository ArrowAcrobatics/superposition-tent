using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    
    public enum Relation {
        Local,
        Global
    }
    public Relation relation;

    public TransformData(Vector3 pos, Quaternion rot, Relation rel) {
        position = pos;
        rotation = rot;
        relation = rel;
    }

    public static TransformData Slerp(TransformData from, TransformData to, float t) {
        if(from.relation != to.relation) {
            Debug.LogWarning("Slerping between local and global data, using from.relation as fallback!");
        }

        return new TransformData(
                Vector3.Slerp(from.position, to.position, t),
                Quaternion.Slerp(from.rotation, to.rotation, t),
                from.relation
            );
    }
}

namespace ArrowAcrobatics 
{
    namespace TransformExtensions
    {
        public static class Ext
        {
            public static TransformData GlobalData(this Transform transform) {
                if(transform == null) {
                    return null;
                }
                return new TransformData(transform.position, transform.rotation, TransformData.Relation.Global);
            }

            public static TransformData LocalData(this Transform transform) {
                if(transform == null) {
                    return null;
                }
                return new TransformData(transform.localPosition, transform.localRotation, TransformData.Relation.Local);
            }

            public static void Set(this Transform transform, TransformData data) {
                switch(data.relation) {
                    case TransformData.Relation.Local: {
                        transform.localPosition = data.position;
                        transform.localRotation = data.rotation;
                        break;
                    }
                    case TransformData.Relation.Global: {
                        transform.SetPositionAndRotation(data.position, data.rotation);
                        break;
                    }
                }
                
                
            }
        }
    }
}


