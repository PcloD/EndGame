using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace FirstGearGames.Mirrors.InteractingSceneObjects
{


    public class ClickAndMove : NetworkBehaviour
    {
        [SerializeField]
        private bool _leftMouse = true;

        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (base.isClient)
                MoveToClick();
        }


        [Client]
        private void MoveToClick()
        {
            if (!base.hasAuthority)
                return;

            /* Only continue if mouse is pressed. */
            //If using left mouse.
            if (_leftMouse && !Input.GetKeyDown(KeyCode.Mouse0))
                return;
            //If using right mouse.
            else if (!_leftMouse && !Input.GetKeyDown(KeyCode.Mouse1))
                return;

            //Raycast for ground and tell the navmeshagent to move towards hit.
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                _agent.SetDestination(hit.point);
        }
    }


}