using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    private enum State
    {
        Alive,
        Dead
    }
    [SerializeField] private float speed;
    private State state;
    private Vector2Int gridposition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private Direction gridMoveDirection;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;
    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    private void Awake()
    {
        gridposition = new Vector2Int(10, 10);
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;
        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }
    private void Update()
    {
        switch (state)
        {
            case State.Alive:
        HandleInput();
        HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
    }
    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridposition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector =  new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector =    new Vector2Int(0, +1); break;
                case Direction.Down: gridMoveDirectionVector =  new Vector2Int(0, -1); break;
            }
            gridposition += gridMoveDirectionVector;
            gridposition = levelGrid.ValidateGridPosition(gridposition);
            bool SnakeAteFood = levelGrid.TrySnakeEatFood(gridposition);
            if (SnakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            UpdateSnakeBodyParts();
            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridposition == snakeBodyPartGridPosition)
                {
                    //Game Over
                    state = State.Dead;
                }
            }
                transform.position = new Vector3(gridposition.x, gridposition.y);
                transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
         }
    }   
        private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }
    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            //Vector3 snakeBodyPosition = new Vector3(snakeMovePositionList[i].x, snakeMovePositionList[i].y);
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }
    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
    public Vector2Int GetGridPosition()
    {
        return gridposition;
    }
    //Return full list of positions occupied by snake: Head + Body
      public List<Vector2Int> GetFullSnakeGridPositionList()
        {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridposition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
            //       gridPositionList.AddRange(snakeMovePositionList);
            return gridPositionList;
        }
      private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            //            snakeBodyTransformList.Add(snakeBodyGameObject.transform);
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;

        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: //Currently move Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0; break;
                        case Direction.Left: //Previously going down
                            angle = 0 + 45; break;
                        case Direction.Right:
                            angle = 0 - 45; break;
                    }
                    break;
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180; break;
                        case Direction.Left: //Previously going left
                            angle = 180 + 45; break;
                        case Direction.Right:
                            angle = 180 - 45; break;
                    }
                    break;
                case Direction.Left: //currently moving left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90; break;
                        case Direction.Down: //Previously going down
                            angle = -45; break;
                        case Direction.Up:
                            angle = 45; break;
                    }
                    break;
                case Direction.Right: // Current moving right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90; break;
                        case Direction.Down: //Previously going down
                            angle = 45; break;
                        case Direction.Up: 
                            angle = -45; break;
                    }
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition(); 
        }
    }
    //Handles one move position from the snake
    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;
        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        public Direction GetDirection()
        {
            return direction;
        }
        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else {
                return previousSnakeMovePosition.direction;
            }
        }
    }
}

