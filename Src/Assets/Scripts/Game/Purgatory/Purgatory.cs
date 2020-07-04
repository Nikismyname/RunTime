//private IEnumerator JumpCoroutine()
//{
//    yield return new WaitForSeconds(0.2f);

//    if (this.lastOnGroundExit == null || this.lastOnGroundExit.Value < this.lastJumpTime.Value)
//    {
//        this.grounded = true;
//        this.isJumpInProgress = false;

//        if (this.lastOnGroundExit == null)
//        {
//            Debug.Log("Player is at reset after jump! NULL");

//        }
//        else
//        {
//            if (this.lastOnGroundExit.Value < this.lastJumpTime.Value)
//            {
//                Debug.Log("Player is at reset after jump! TIME");
//            }
//        }
//    }
//}

/// https://answers.unity.com/questions/163337/velocity-before-collision.html
//Vector3 ComputeIncidentVelocity(Rigidbody body, Collision collision, out Vector3 otherVelocity)
//{
//    Vector3 impulse = collision.impulse;
//    // Both participants of a collision see the same impulse, so we need to flip it for one of them.
//    if (Vector3.Dot(collision.GetContact(0).normal, impulse) < 0f)
//        impulse *= -1f;
//    otherVelocity = Vector3.zero;
//    // Static or kinematic colliders won't be affected by impulses.
//    var otherBody = collision.rigidbody;
//    if (otherBody != null)
//    {
//        otherVelocity = otherBody.velocity;
//        if (!otherBody.isKinematic)
//            otherVelocity += impulse / otherBody.mass;
//    }

//    var bodyVel = body.velocity;
//    var inpuls = impulse / body.mass;

//    Debug.DrawLine(rb.transform.position, rb.transform.position + bodyVel.normalized * 4, Color.green, 4);
//    Debug.DrawLine(rb.transform.position, rb.transform.position + inpuls.normalized * 5, Color.red, 4);

//    return bodyVel - inpuls;
//}