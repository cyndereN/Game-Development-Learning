using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ubiq.XR;
using UnityEngine;

namespace Ubiq.Samples
{
    public interface IBasketball
    {
        void Grasp(Hand hand);
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Basketballs : MonoBehaviour, IUseable, IGraspable
    {
        public GameObject prefab;

        private Hand follow;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Grasp(Hand controller)
        {
            follow = controller;
        }

        public void Release(Hand controller)
        {
            follow = null;
        }

        public void UnUse(Hand controller)
        {
        }

        public void Use(Hand controller)
        {
            var baskeball = NetworkSpawner.SpawnPersistent(this, prefab).GetComponents<MonoBehaviour>().Where(mb => mb is IBasketball).FirstOrDefault() as IBasketball;
            if (baskeball != null)
            {
                baskeball.Grasp(controller);
            }
        }

        private void Update()
        {
            if (follow != null)
            {
                transform.position = follow.transform.position;
                transform.rotation = follow.transform.rotation;
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
            }
        }
    }
}