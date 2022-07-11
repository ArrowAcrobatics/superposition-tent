using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SlimeVrClient))]
public class SkeletonEnforcer : MonoBehaviour
{
   [System.Serializable]
    public class BoneRelation
    {
        public string parent, child;
        public bool satisfied = false;
    }

    public List<BoneRelation> relations = new List<BoneRelation>();

    private SlimeVrClient slime = null;
    private bool fullySatisfied = false;

    void Awake() {
        slime = GetComponent<SlimeVrClient>();
    }

    void Update() { 
        EnforceParentRelationship(); 
    }

    void EnforceParentRelationship() {
        if(fullySatisfied) {
            return;
        }
        foreach (BoneRelation relation in relations) {
            if (!relation.satisfied) {
                GameObject par = null;
                GameObject child = null;

                foreach(GameObject g in slime.trackerObjects) {
                    if( par != null && child != null) {
                        break; // found both
                    }
                    if (g == null) {
                        continue;
                    }
                    if (g.name == relation.parent) {
                        par = g;
                        continue;
                    }
                    if(g.name == relation.child) {
                        child = g;
                        continue;
                    }
                }
                if(child!= null && par != null && child.transform.parent != par) {
                    Debug.Log("rebone " + relation.parent + " " + relation.child);
                    child.transform.SetParent(par.transform, true);
                    relation.satisfied = true;
                }
            }
        }

        fullySatisfied = relations.All(r => r.satisfied);
    }
}
