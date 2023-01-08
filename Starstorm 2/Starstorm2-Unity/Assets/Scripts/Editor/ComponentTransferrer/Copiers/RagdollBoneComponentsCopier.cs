using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//used within ragdollcomponentcopier, not in main editorcopyandpastecomponents
public class RagdollBoneComponentsCopier : NotRetardedUnityComponentCopier {

    protected override void PasteStoredComponent(GameObject selected) {
        base.PasteStoredComponent(selected);

        for (int i = 0; i < storedComponents.Count; i++) {

            System.Type componentType = storedComponents[i].GetType();

            if(componentType == typeof(CharacterJoint)) {
                HandlePasteJoint((CharacterJoint)storedComponents[i], selected.GetComponent<CharacterJoint>());
            }

            //todo: working out rescaling colliders for fixing the stupid 100 armatures
            if (storedComponents[i] is Collider) {
                HandlePasteCollider((Collider)storedComponents[i], selected.GetComponent<Collider>());
            }
        }
    }

    private void HandlePasteJoint(CharacterJoint storedJoint, CharacterJoint newJoint) {

        if (storedJoint.connectedBody == null) {

            pasteReport += $"\noriginal joint {storedJoint} did not have a connectedBody";
            return;
        }

        newJoint.connectedBody = null;

        List<Transform> newChildren = newJoint.transform.root.GetComponentsInChildren<Transform>(true).ToList();

        //this will break hard if there's duplicates name anywhere on the rig
        //so don't do that
        Transform newConnectedBody = newChildren.Find(tran => tran.name == storedJoint.connectedBody.transform.name);

        if (newConnectedBody != null) {
            newJoint.connectedBody = newConnectedBody.GetComponent<Rigidbody>();

            if (newJoint.connectedBody == null) {
                pasteReport += $"\nfound connectedbody for {newJoint}, {newConnectedBody}, did not have a rigidbody";
            }
        } else {
            pasteReport += $"\ncould not find connectedbody for joint {newJoint}";
        }
    }

    private void HandlePasteCollider(Collider storedCollider, Collider newCollider) {

        float scaleDifference = storedCollider.transform.lossyScale.magnitude / newCollider.transform.lossyScale.magnitude;
        Debug.LogWarning(scaleDifference);

        if (storedCollider is SphereCollider) {

            SphereCollider storedSphereCollider = (SphereCollider)storedCollider;
            SphereCollider newSphereCollider = (SphereCollider)newCollider;

            newSphereCollider.center = storedSphereCollider.center * scaleDifference;
            newSphereCollider.radius = storedSphereCollider.radius * scaleDifference;

            return;
        }
        if (storedCollider is CapsuleCollider) {
            CapsuleCollider storedSphereCollider = (CapsuleCollider)storedCollider;
            CapsuleCollider newCapsuleCollider = (CapsuleCollider)newCollider;

            newCapsuleCollider.center =    storedSphereCollider.center * scaleDifference;
            newCapsuleCollider.radius =    storedSphereCollider.radius * scaleDifference;
            newCapsuleCollider.height =    storedSphereCollider.height * scaleDifference;
            newCapsuleCollider.direction = storedSphereCollider.direction;
            return;
        }
        //if (storedCollider is BoxCollider) {
        //    BoxCollider col = selected.GetComponent<BoxCollider>();

        //    col.center = (storedCollider as BoxCollider).center;
        //    col.size = (storedCollider as BoxCollider).size;
        //    return;
        //}
    }
}

