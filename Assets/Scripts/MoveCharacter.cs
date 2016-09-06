using UnityEngine;
using System.Collections;

public class MoveCharacter : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;

    public float pushPower = 2000.0f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    private float angle = 0.0f;

    public bool bouncingOnEnemy = false;
    public bool hasBouncedOnEnemy = false;


    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        // Select movement direction
        if (bouncingOnEnemy) {
            moveDirection.y = jumpSpeed;
            bouncingOnEnemy = false;
        } else if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            moveDirection *= speed;
            if (Input.GetButton("Jump")) {
                moveDirection.y = jumpSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;

        // Rotate the jumping character
        if (!controller.isGrounded) {
            float targetAngle = 0.0f;
            if (moveDirection.x < 0) {
                targetAngle = 180.0f;
            } else if (moveDirection.x > 0) {
                targetAngle = -180.0f;
            }
            angle = Mathf.Lerp(angle, targetAngle, 5 * Time.deltaTime);
        } else {
            angle = Mathf.Lerp(angle, 0.0f, 10 * Time.deltaTime);
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);

        controller.Move(moveDirection * Time.deltaTime);
    }


    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.1f, 0);

        if (hit.moveDirection.y < -0.3f) {
            bouncingOnEnemy = true;
            // pushDir = new Vector3(-controller.velocity.x, Mathf.Abs(controller.velocity.x), 0);
            // pushDir *= 0.1f;

            ParticleSystem particleSystem = GameObject.Find("OrangeExplosion").GetComponent<ParticleSystem>();
            Vector3 position = hit.gameObject.transform.position;
            particleSystem.transform.localPosition = new Vector3(position.x, position.y, -1);
            particleSystem.Play();

            Animator orangeSplatAnimator = GameObject.Find("orangejuice_0").GetComponent<Animator>();
            orangeSplatAnimator.SetTrigger("StartAnimation");

            if (GameObject.Find("orangejuice_0").GetComponent<FollowGameObject>() == null) {
                GameObject.Find("orangejuice_0").AddComponent<FollowGameObject>();
            }
            FollowGameObject follow = GameObject.Find("orangejuice_0").GetComponent<FollowGameObject>();
            follow.gameObjectToFollow = Camera.main.gameObject;
            follow.offset = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 2.0f);

            // orangeSplatAnimator.transform.localPosition = Camera.main.transform.position + new Vector3(0, 0, 2); // new Vector3(position.x, position.y, -2);

            GameObject.Destroy(hit.gameObject);
            return;
        }

        body.AddForce(pushPower * pushDir);
    }
}
