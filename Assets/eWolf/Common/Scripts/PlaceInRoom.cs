using eWolf.Common.Interfaces;
using UnityEngine;

namespace eWolf.Common
{
    public class PlaceInRoom : MonoBehaviour, IFixAsset
    {
        public void Fix()
        {
            var mapHolder = ServiceLocator.Instance.GetService<IMapController>();
            if (mapHolder != null)
            {
                mapHolder.ParentObjectToRoom(this);
            }
        }
    }
}