using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the player character
public class Player : Unit
{
    private PlayerController controller;

    public List<Item> Inventory { get; set; }

    BoxCollider2D boxCollider; // used for Logan's movement implementation

    public int level;
    public int experience;

    protected override void Awake()
    {
        base.Awake();
        InitPlayer();
        Health = 3;
        Strength = 1;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    this.UnitSprite.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time, 1));

        //    Move();
    //}

    public bool AttemptMovementByPlayer(Vector2Int destinationCoords)
    {
        bool coordsAreValid = Utility.CheckIfCoordsAreValid(destinationCoords);

        if (coordsAreValid)
        {
            GridSquare destinationGS = Game.MapGrid[destinationCoords.x, destinationCoords.y];

            MoveToGridSquare(Game.MapGrid[Coords.x, Coords.y], destinationGS);

            if (destinationGS.GroundItem != null)
            {
                GetGroundItem(destinationGS.GroundItem);
            }

            if (destinationCoords.x == Game.instance.DungeonMap.KeyDoor.y && destinationCoords.y == Game.instance.DungeonMap.KeyDoor.x) // remember that x & y are flipped for dun map
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("VictoryScene");
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetGroundItem(Item groundItem)
    {
        // q&d
        if (groundItem.itemName == "Minor Health Potion")
        {
            UseItem(groundItem);
            ItemManager.instance.PurgeItem(groundItem);
        }
        else
        {
            Debug.LogWarning("This item type has yet to be accounted for in getting picked up!");
        }
    }

    private void UseItem(Item item)
    {
        // q&d
        if (item.itemName == "Minor Health Potion")
        {
            GainHealth(1);
        }
        else
        {
            Debug.LogWarning("This item type has yet to be accounted for in being used!");
        }
    }

    IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(0.9f);
        //playersTurn = true;
    }

    // SQ: use the MoveToGridSquare method of Unit for both Player movement and Enemy movement

    // Logan's implementation of move based on physics-based collision
    public void Move()
    {
        if (UnitManager.instance.playersTurn)
        {

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal != 0)
            {
                vertical = 0;
            }
            Vector3 delta = new Vector3(horizontal, vertical, 0);
            RaycastHit2D hit;

            // make sure we can move by casting a box there first, if there is no collision, we are free to move
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, delta.y), Mathf.Abs(delta.y), LayerMask.GetMask("Blocking"));
            if (hit.collider == null && delta.y != 0)
            {
                Debug.Log(transform.position);
                transform.Translate(0, delta.y, 0);
                UnitManager.instance.playersTurn = false;
                StartCoroutine(WaitRoutine());

            }
            if (hit.transform != null)
            {
                Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
                hitEnemy.TakeDamage(1);
                hitEnemy.UnitSprite.color = Color.red; // trying to manipulate the enemy sprite to give a on hit feedback, not currently working
                Debug.Log(hitEnemy.Health);
                UnitManager.instance.playersTurn = false;
                StartCoroutine(WaitRoutine());
                hitEnemy.UnitSprite.color = Color.white;


            }
            hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(delta.x, 0), Mathf.Abs(delta.x), LayerMask.GetMask("Blocking"));
            if (hit.collider == null && delta.x != 0)
            {
                if (delta.x > 0)
                {
                    UnitSprite.flipX = true;
                }
                if (UnitSprite.flipX && delta.x < 0)
                {
                    UnitSprite.flipX = false;
                }
                Debug.Log(transform.position);
                transform.Translate(delta.x, 0, 0);
                UnitManager.instance.playersTurn = false;
                StartCoroutine(WaitRoutine());

            }
            if (hit.transform != null)
            {
                Enemy hitEnemy = hit.transform.GetComponent<Enemy>();
                hitEnemy.TakeDamage(1);
                hitEnemy.UnitSprite.color = Color.red;
                Debug.Log(hitEnemy.Health);
                UnitManager.instance.playersTurn = false;
                StartCoroutine(WaitRoutine());
                hitEnemy.UnitSprite.color = Color.white;


            }
        }
    }

    private void InitPlayer()
    {
        Debug.Log("Player character's initialization runs.");

        UnitManager.Player = this;

        boxCollider = GetComponent<BoxCollider2D>();
        Health = 3;

        // TEMPORARY PLACEMENT?
        UnitName = "Player";
        UnitSprite = gameObject.AddComponent<SpriteRenderer>();
        UnitSprite.sprite = Resources.Load<Sprite>("Visuals/UnitSprites/player1");
        UnitSprite.sortingOrder = 3;

        controller = gameObject.AddComponent<PlayerController>();
    }

}
