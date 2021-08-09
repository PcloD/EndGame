using Mirror;

namespace FirstGearGames.Mirrors.ConservingBandwidth
{


    public class SendByte : NetworkBehaviour
    {

        public bool TwoBytes = false;


        private void FixedUpdate()
        {
            if (base.isServer)
            {
                if (TwoBytes)
                    RpcSendTwoBytes(0, 1);
                else
                    RpcSendOneByte(0);
            }
        }

        [ClientRpc]
        private void RpcSendOneByte(byte heyheyYouYou) { }
        [ClientRpc]
        private void RpcSendTwoBytes(byte iDontLike, byte yourGirlfriend) { }


        //private Animator _animator;
        //private float _pitchTarget;
        //private float? _lastSentPitch;

        //private void Update()
        //{
        //    float moveRate = 1f;

        //    float x = _animator.GetFloat("Pitch");
        //    _animator.SetFloat("Pitch", Mathf.MoveTowards(x, _pitchTarget, moveRate * Time.deltaTime));
        //}

        //private void FixedUpdate()
        //{
        //    float currentPitch = 10f;
        //    /* In addition to sending only in fixed update rather than every frame
        //     * you can choose to determine if a value is worth sending. For example,
        //     * will sending a pitch difference of anything less than 5f really worth
        //     * sending? In this example only send pitch if it has changed more than 5f. */
        //    if (_lastSentPitch == null || Mathf.Abs(_lastSentPitch.Value - currentPitch) > 5f)
        //    {
        //        RpcSetPitchTarget(currentPitch);
        //        _lastSentPitch = currentPitch;
        //    }
        //}

        //[ClientRpc]
        //private void RpcSetPitchTarget(float value)
        //{
        //    _pitchTarget = value;
        //}
    }


}