using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{

    [SerializeField] GameObject bulletPrefab = null;
    [SerializeField] Vector2 gunOffset = new Vector2(0.5f, 0.1f);
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletReloadTime = 0.2f;
    [SerializeField] float inconsistency = 5f;
    float bulletTimer;

    Camera cam;
    Animator animator;
    PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        bulletTimer = bulletReloadTime;

        cam = Camera.main;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        bulletTimer -= Time.deltaTime;
        if (Input.GetAxisRaw("Fire1") != 0 && bulletTimer <= 0) {
            Fire();
            bulletTimer = bulletReloadTime;
        }
    }

    void Fire() {
        Vector2 fireVector = GetFireVector();
        float fireRotation = ClampGunAngle(GetAngle(fireVector));

        // Graphics update
        bool flipSprite = !playerController.flipped ? fireVector.x < 0f : fireVector.x > 0f;
        playerController.FlipSprite(flipSprite);
        animator.SetTrigger("shoot");
        
        // Lock onto proner
        GameObject droneToFireAt = GetObjectToFireAt ("Proner", fireRotation, fireVector.x > 0);
        if (droneToFireAt) {
            fireVector = droneToFireAt.transform.position - GunPosition();
            fireRotation = ClampGunAngle(GetAngle(fireVector));
        }

        
        // Create bullet here
        GameObject newBullet = Instantiate(bulletPrefab, GunPosition(), transform.rotation);
        fireRotation *= Mathf.Deg2Rad;
        Vector2 bulletVector = new Vector2(Mathf.Cos(fireRotation), Mathf.Sin(fireRotation));
        newBullet.GetComponent<Rigidbody2D>().velocity = bulletVector * bulletSpeed;
        Destroy(newBullet, 5.0f);

        // Knockback
        playerController.AddKnockback(-bulletVector * bulletSpeed / 10);
    }

    Vector2 GetFireVector () {
        float minMag = 0.1f;
        Vector2 fireVector;
        if (Input.GetMouseButton(0)) {
            fireVector = cam.ScreenToWorldPoint(Input.mousePosition) - GunPosition();
        } else {
            fireVector = new Vector2(playerController.flipped ? -1f : 1f, 0f);
        }
        return fireVector.normalized;
    }

    float ClampGunAngle (float rot) {
        float maxGunAngle = 10f;
        float newRot = rot;
        if (rot > maxGunAngle && rot < 90f) {
            newRot = maxGunAngle;
        } else if (rot >= 90f && rot < 180f - maxGunAngle) {
            newRot = 180f - maxGunAngle;
        } else if (rot < -maxGunAngle && rot > -90f) {
            newRot = -maxGunAngle;
        } else if (rot <= -90f && rot > -180f + maxGunAngle) {
            newRot = -180f + maxGunAngle;
        }
        return newRot + Random.Range(-inconsistency, inconsistency);
    }

    GameObject GetObjectToFireAt(string tag, float fireAngle, bool goesRight) {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        GameObject bestObject = null;
        float closestDistance = 10f;
        foreach (GameObject obj in objectsWithTag) {
            Vector2 displacement = obj.transform.position - GunPosition();
            float distance = displacement.magnitude;
            // Checks if it is the right direction
            if ((displacement.x > 0 && goesRight) || (displacement.x < 0 && !goesRight)) {
                // How far out something can be
                float margin = 5f;
                float relativeAngle = GetAngle(displacement) - fireAngle;
                // Check if it is out of the fire range
                if (relativeAngle > -margin && relativeAngle < margin) {
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        bestObject = obj;
                    }
                }
            }
        }
        return bestObject;
    }

    float GetAngle(Vector2 vector) {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    Vector3 GunPosition() {
        float flipDirection = playerController.flipped ? -1 : 1;
        Vector3 gunOffset3 = new Vector3(gunOffset.x * flipDirection, gunOffset.y, 0.1f);
        return transform.position + gunOffset3;
    }
}
