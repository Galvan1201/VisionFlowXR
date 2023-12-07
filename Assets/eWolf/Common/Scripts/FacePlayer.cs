using UnityEngine;

namespace eWolf.Common
{
    public class FacePlayer : MonoBehaviour
    {
        private Transform _target = null;

        private Transform Target
        {
            get
            {
                if (_target == null)
                {
                    var player = GameObject.Find("FirstPersonCharacter");

                    if (player == null)
                        player = GameObject.Find("Main Camera");

                    if (player == null)
                        player = GameObject.Find("TempPlayer_pf");

                    if (player != null)
                        _target = player.transform;
                }
                return _target;
            }
        }

        public void Update()
        {
            var target = Target;

            if (target != null)
                transform.LookAt(Target);
        }
    }
}