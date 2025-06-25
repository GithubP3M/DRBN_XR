using System;
using System.Collections.Generic;
using Unity.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class chooseCollition : MonoBehaviour
{   
    public static ConcurrentDictionary<int, Collider> m_instanceMap = new ConcurrentDictionary<int, Collider>();
    public static ConcurrentStack<Collider[]> m_collisions = new ConcurrentStack<Collider[]>();
    public void Start()
    {
        m_instanceMap.Clear();
        List<Collider> gos = new List<Collider>();
        foreach (Collider go in Resources.FindObjectsOfTypeAll(typeof(Collider)))
        {
            if (gos.Contains(go))
            {
                continue;
            }
            gos.Add(go);
            m_instanceMap[go.GetInstanceID()] = go;
        }
    }
    public void OnEnable()
    {
        Physics.ContactModifyEvent += ModificationEvent;
    }

    public void OnDisable()
    {
        Physics.ContactModifyEvent -= ModificationEvent;
    }
    
    public void ModificationEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
    {
        
        m_collisions.Clear();
        foreach (var pair in pairs)
        {
            var obj1name = m_instanceMap[pair.colliderInstanceID].gameObject.transform.parent.name;
            var obj2name = m_instanceMap[pair.otherColliderInstanceID].gameObject.transform.parent.name;
            
            if (obj1name == obj2name)
            {
              m_collisions.Push(new Collider[] {m_instanceMap[pair.colliderInstanceID],m_instanceMap[pair.otherColliderInstanceID]});
              for (int i=0; i < pair.contactCount; ++i)
              {
                 pair.IgnoreContact(i);
              }
            }
        }
    }
    
    public void FixedUpdate()
    {
        
        foreach( var collision in m_collisions)
        {
            Collider collider1 = collision[0];
            Collider collider2 = collision[1];
            float scalingFactor = 0.01F;
            Vector3 direction;
            float distance;
            bool overlapped = Physics.ComputePenetration(
                collider1, collider1.transform.position, collider1.transform.rotation,
                collider2, collider2.transform.position, collider2.transform.rotation,
                out direction, out distance
            );
            
            if (overlapped)
            {
                collision[0].attachedRigidbody.AddForce(direction*Mathf.Exp(distance/2)*scalingFactor,ForceMode.Impulse);
                collision[1].attachedRigidbody.AddForce(-direction*Mathf.Exp(distance/2)*scalingFactor,ForceMode.Impulse);
            }
        }
        
        
    }
   }
