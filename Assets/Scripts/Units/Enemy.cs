using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    public ArtInt AI { get; set; }

    protected override void Awake()
    {
        base.Awake();
        InitEnemy();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AI.DestinationCoords = new Vector2Int(15, 3); // test enemy movement
        //AI.FindPath(Game.MapGrid[Coords.x, Coords.y], Game.MapGrid[AI.DestinationCoords.x, AI.DestinationCoords.y]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitEnemy()
    {
        UnitManager.instance.Enemies.Add(this);

        UnitName = $"Enemy {UnitManager.instance.Enemies.Count}";
        AI = new ArtInt(this);
        Health = 2;
        Strength = 1;

        //Game.MapGrid[5, 1].Occupiers.Add(this);

        // TEMPORARY PLACEMENT
        UnitSprite = gameObject.AddComponent<SpriteRenderer>();
        UnitSprite.sprite = Resources.Load<Sprite>("Visuals/UnitSprites/goblin1");
        UnitSprite.sortingOrder = 3;
    }

}
