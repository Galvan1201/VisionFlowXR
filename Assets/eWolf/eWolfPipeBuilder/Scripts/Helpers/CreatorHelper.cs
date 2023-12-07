using eWolf.Common.Helper;
using UnityEngine;

namespace eWolf.PipeBuilder.Helpers
{
    public static class CreatorHelper
    {
        public static GameObject CreateFilters(Vector3 position, Transform parent)
        {
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Fittings_pf"),
                position,
                Quaternion.identity);

            go.transform.parent = parent;
            return go;
        }

        public static GameObject CreatePipe(Vector3 position, Transform parent)
        {
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("PipeNode_pf"),
                position,
                Quaternion.identity);

            go.transform.parent = parent;

            ObjectHelper.RemoveComponent<MeshCollider>(go);

            return go;
        }

        public static GameObject CreateValveLeaver(Vector3 position, Transform parent, Vector3 rotation, float scale)
        {
            Vector3 vectorScale = new Vector3(scale, scale, scale);
            var vectorDirection = Quaternion.LookRotation(rotation);

            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("ValveLeaver_pf"),
                position,
                vectorDirection);

            go.transform.localScale = vectorScale;
            go.transform.parent = parent;
            return go;
        }

        public static GameObject CreateValveWheel(Vector3 position, Transform parent, Vector3 rotation, float scale)
        {
            Vector3 vectorScale = new Vector3(scale, scale, scale);
            var vectorDirection = Quaternion.LookRotation(rotation);
            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("GateValveWheel_pf"),
                position,
                vectorDirection);

            go.transform.localScale = vectorScale;
            go.transform.parent = parent;
            return go;
        }
    }
}