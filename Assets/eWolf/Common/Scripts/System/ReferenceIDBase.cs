using System;
using UnityEngine;

namespace eWolf.Common.System
{
    public abstract class ReferenceIDBase : MonoBehaviour
    {
        protected Guid _id = Guid.Empty;

        public Guid ID
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    Register();
                }
                return _id;
            }
        }

        public static Guid GetID(GameObject go)
        {
            ReferenceIDBase referenceIDBase = go.GetComponent<ReferenceIDBase>();
            if (referenceIDBase != null)
            {
                return referenceIDBase.ID;
            }
            else
            {
                Debug.LogError("Failed to find the reference ID for " + go.name);
            }
            return new Guid();
        }

        protected abstract void Register();

        protected abstract void UnRegister();
    }
}