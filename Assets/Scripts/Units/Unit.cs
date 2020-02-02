using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the base class for all units (the player character and enemies)
// maybe make this abstract
public class Unit : MonoBehaviour
{
    public string UnitName { get; set; }
    public SpriteRenderer UnitSprite { get; set; }

    // the unit's current coords within the level
    public Vector2Int Coords { get; set; }

    public FaceDirection FaceDirection { get; set; }

    // lerp color for when being hit

    // offset so that unit sprites are centered in grid squares
    public static Vector2 Offset
    {
        get
        {
            return new Vector2(0.5f, 0.5f);
        }
    }

    public int Health { get; set; }

    public int Strength { get; set; }

    public Weapon CurrentWeapon { get; set; }

    protected virtual void Awake()
    {
        InitUnit();
    }

    // public void MoveByControl() { } // for player

    public void Attack(Unit beingAttacked) 
    {
        beingAttacked.TakeDamage(this.Strength);
        if(this.CurrentWeapon != null)
        {
            beingAttacked.TakeDamage(this.CurrentWeapon.Power);
        }

        Debug.Log($"{UnitName} attacked {beingAttacked.UnitName} at {beingAttacked.Coords}");
        
    }

    public void TakeDamage(int damage)
    {

        StartCoroutine(FlashHit());
        Health -= damage;

        if (this == UnitManager.Player)
        {
            HUD.instance.RemoveFullHeart();

            if (Health <= 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
            }
        }

        if (Health <= 0)
        {
            Die();
        }
    }

    public void GainHealth(int amount)
    {
        Health += amount;

        if (this == UnitManager.Player)
        {
            for (int i = 0; i < amount; i++)
            {
                HUD.instance.AddFullHeart();
            }
        }
    }


    // ### MOVEMENT & ORIENTATION METHODS ###################################################################################

    public void MoveToGridSquare(GridSquare formerGridSquare, GridSquare newGridSquare, bool changeFaceDirectionAccordingly = true)
    {
        bool movementSuccessful = false;

        float x = (float)newGridSquare.GridCoords.x + Offset.x;
        float y = (float)newGridSquare.GridCoords.y + Offset.y;
        gameObject.transform.position = new Vector3(x, y, 0);
        //gameObject.transform.Translate(x, y, 0f);
        movementSuccessful = true;

        if (movementSuccessful)
        {
            Coords = newGridSquare.GridCoords; // update self's coords accordingly

            if (formerGridSquare != null)
            {
                formerGridSquare.Occupiers.Remove(this); // update grid square's Occupiers list
                //Debug.Log($"Removed {UnitName} from Occupiers list of GS: {formerGridSquare.GridCoords}");

                if (changeFaceDirectionAccordingly)
                {
                    // if movement was leftward (NW, W, or SW) and face direction is not already left, face left
                    if (newGridSquare.GridCoords.x < formerGridSquare.GridCoords.x && FaceDirection != FaceDirection.Left)
                    {
                        ChangeDirection(FaceDirection.Left);
                    }
                    // else if movement direction was rightward (NE, E, or SE) and face direction is not already right, face right
                    else if (newGridSquare.GridCoords.x > formerGridSquare.GridCoords.x && FaceDirection != FaceDirection.Right)
                    {
                        ChangeDirection(FaceDirection.Right);
                    }
                }
            }

            if (!newGridSquare.Occupiers.Contains(this))
            {
                newGridSquare.Occupiers.Add(this);
                //Debug.Log($"Added {UnitName} to Occupiers list of GS: {newGridSquare.GridCoords}");
            }
        }
    }

    public void ChangeDirection(FaceDirection direction)
    {
        //Weapon wieldedWeapon = unit.Gear.WieldedWeapon; // CER

        if (direction == FaceDirection.Left && FaceDirection != FaceDirection.Left)
        {
            if (!UnitSprite.flipX)
            {
                UnitSprite.flipX = true;
            }

            //if (wieldedWeapon != null)
            //{
            //    wieldedWeapon.mb.weaponSR.flipX = true;
            //    PositionWieldedWeapon(wieldedWeapon, direction, wieldedWeapon.WeaponType.DefaultGraspHand);
            //}
            FaceDirection = FaceDirection.Left;
        }
        else if (direction == FaceDirection.Right && FaceDirection != FaceDirection.Right)
        {
            if (UnitSprite.flipX)
            {
                UnitSprite.flipX = false;
            }

            //if (wieldedWeapon != null)
            //{
            //    wieldedWeapon.mb.weaponSR.flipX = false;
            //    PositionWieldedWeapon(wieldedWeapon, direction, wieldedWeapon.WeaponType.DefaultGraspHand);
            //}
            FaceDirection = FaceDirection.Right;
        }
        else
        {
            Debug.LogError("Error! Ran into a problem changing direction!");
        }
    }


    private void Die()
    {
        GridSquare curGS = Game.MapGrid[Coords.x, Coords.y];
        curGS.Occupiers.Remove(this);

        if (this is Enemy)
        {
            UnitManager.instance.Enemies.Remove((Enemy)this);
            Debug.Log($"Removed {UnitName} from enemies list.");
        }

        Destroy(gameObject);
    }

    IEnumerator FlashHit()
    {
        float elapsedTime = 0;
        while(elapsedTime < 2)
        {

        this.UnitSprite.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(elapsedTime, 1));
            elapsedTime += Time.deltaTime * 8;
        yield return new WaitForEndOfFrame();
        }

        //playersTurn = true;
    }

    private void InitUnit()
    {
        Coords = new Vector2Int(0, 0);
    }
}
