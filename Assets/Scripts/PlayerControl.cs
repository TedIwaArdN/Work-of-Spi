using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public float initialSpeed;
    public float jumpForce;
    public int jumpLimit;
    private int jumpTimes;
    private bool moveLeft;
    private bool moveRight;
    public bool moveUp;
    public bool moveDown;
    private bool isNearDataCenter;
    private bool hasProcessed;
    private bool isReadyToPickUp;
    private bool isReadyToEscape;
    private bool isOnGround;
    public bool isOnLadder;
    private bool isAttacking;
    public bool isDead;
    public bool isWin;
    public int attackCounter;
    private float speedMultiplier;

    public GameObject[] attackEffects;
    private Rigidbody2D playerRb;
    public GameObject textWhenApprochingDataCenter;
    public GameObject textWhenFinishProcessing;
    public GameObject textWhenToEscape;
    public TextMeshProUGUI textWhenProcessingData;
    public Animator playerAnimator;
    public GameObject gameOverMenu;
    public GameObject normalBGM;
    public GameObject escapingBGM;
    public GameObject slashSound;
    public GameObject hiddenWayBlock;
    public GameObject win;
    public GameObject escapeInstruction;
    public GameObject instructionArrow;

    // Start is called before the first frame update
    void Start()
    {
        speedMultiplier = 1;

        playerRb = GetComponent<Rigidbody2D>();
        isNearDataCenter = false;
        isOnLadder = false;
        moveUp = moveDown = false;
        isAttacking = false;
        isDead = false;
        isWin = false;

        textWhenApprochingDataCenter.SetActive(false);
        textWhenFinishProcessing.SetActive(false);
        textWhenToEscape.SetActive(false);
        textWhenProcessingData.gameObject.SetActive(false);
        gameOverMenu.SetActive(false);
        slashSound.SetActive(false);
        hiddenWayBlock.gameObject.SetActive(true);
        win.gameObject.SetActive(false);
        escapeInstruction.gameObject.SetActive(false);
        instructionArrow.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) {
            normalBGM.gameObject.SetActive(false);
            playerAnimator.SetBool("isDead", true);
            gameOverMenu.SetActive(true);
        }

        else if (isWin) {
            escapingBGM.gameObject.SetActive(false);
            win.gameObject.SetActive(true);
        }

        else {
            if (isOnLadder) {
                MoveOnLadder();
            }
            else {
                MoveLeftOrRight();
                Jump();
                OnGroundReset();

                if (Input.GetKeyDown(KeyCode.Z) && !isAttacking) {
                    StartCoroutine(Attack());
                }
            }
            if (isReadyToEscape) {
                textWhenToEscape.gameObject.SetActive(true);
                hiddenWayBlock.gameObject.SetActive(false);
                escapeInstruction.gameObject.SetActive(true);
                instructionArrow.gameObject.SetActive(false);
            }

            if (isNearDataCenter) {
                // Display "Press C to get data"
                if (!hasProcessed) {
                    textWhenApprochingDataCenter.gameObject.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.C)) {
                    if (!hasProcessed) {
                        // Get data
                        textWhenApprochingDataCenter.gameObject.SetActive(false);
                        textWhenProcessingData.gameObject.SetActive(true);
                        StartCoroutine(GetData());
                        hasProcessed = !hasProcessed;
                    }
                    if (isReadyToPickUp) {
                        // Take away
                        textWhenFinishProcessing.SetActive(false);
                        isReadyToPickUp = false;
                        isReadyToEscape = true;
                    }
                }
            }

        }
    }

    void MoveOnLadder() {
        playerRb.velocity = Vector2.zero;
        jumpTimes = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            moveUp = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            moveDown = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow)) {
            moveUp = false;
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) {
            moveDown = false;
        }

        if (moveDown) {
            transform.Translate(Vector2.down * 10 * Time.deltaTime);
        }
        else if (moveUp) {
            transform.Translate(Vector2.up * 10 * Time.deltaTime);
        }
    }

    void MoveLeftOrRight() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            moveLeft = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            moveRight = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            speedMultiplier = 1.5f;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
            speedMultiplier = 1;
        }

        if (moveLeft) {
            playerAnimator.SetBool("isFacingLeft", true);
            playerRb.velocity = new Vector2(-initialSpeed * speedMultiplier, playerRb.velocity.y);
        }
        if (moveRight) {
            playerAnimator.SetBool("isFacingLeft", false);
            playerRb.velocity = new Vector2(initialSpeed * speedMultiplier, playerRb.velocity.y);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) {
            moveLeft = false;
            playerRb.velocity = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow)) {
            moveRight = false;
            playerRb.velocity = Vector2.zero;
        }

    }

    void Jump() {
        if (jumpTimes < jumpLimit) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.X)) {
                playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpTimes++;
            }
        }
        //if (Input.GetKeyDown(KeyCode.DownArrow)) {
        //    playerRb.AddForce(Vector2.down * jumpForce * 4, ForceMode2D.Impulse);
        //}
    }

    private void OnGroundReset() {
        if (isOnGround) {
            jumpTimes = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Trap")) {
            isDead = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isOnGround = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("DataCenter")) {
            isNearDataCenter = true;
        }

        if (collision.gameObject.CompareTag("Ladder")) {
            jumpTimes = 0;
            isOnLadder = true;
            moveLeft = moveRight = false;
        }

        if (collision.gameObject.CompareTag("SafePlace")) {
            textWhenToEscape.gameObject.SetActive(false);
            normalBGM.gameObject.SetActive(false);
            escapingBGM.gameObject.SetActive(true);
            isReadyToEscape = false;
            hiddenWayBlock.gameObject.SetActive(true);
        }

        if (collision.gameObject.CompareTag("EscapePlace")) {
            isWin = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("DataCenter")) {
            isNearDataCenter = false;
            textWhenApprochingDataCenter.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Ladder")) {
            isOnLadder = false;
            moveUp = moveDown = false;
            moveLeft = moveRight = false;
            jumpTimes = 0;
        }
    }

    IEnumerator GetData() {
        for (int i = 19; i >= 0; i--) {
            yield return new WaitForSeconds(1);
            textWhenProcessingData.text = "Still have " + i + " seconds to finish";
        }
        textWhenProcessingData.gameObject.SetActive(false);
        textWhenFinishProcessing.gameObject.SetActive(true);
        isReadyToPickUp = true;

    }

    private IEnumerator Attack() {
        isAttacking = true;
        attackEffects[attackCounter].gameObject.SetActive(true);
        slashSound.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        attackEffects[attackCounter].gameObject.SetActive(false);
        slashSound.gameObject.SetActive(false);
        attackCounter = (attackCounter + 1) % attackEffects.Length;
    }

}
