using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DunMap 
{
    int mapWidth;
    int mapHeight;
    public List<Room> RoomList { get; set; }
    public Vector2Int KeyDoor { get; set; }
    System.Random rand = new System.Random();
    public string[,] MapArr {get; set;}



    public DunMap(int mapW, int mapH)
    {
        mapWidth = mapW;
        mapHeight = mapH;
        RoomList = new List<Room>();

        MapArr = new string[mapHeight, mapWidth];
        Fill2DArray(MapArr);
        
    }

    public static void Fill2DArray(string[,] arr)
    {
        int numRows = arr.GetLength(0);
        int numCols = arr.GetLength(1);

        for (int y = 0; y < numRows; ++y)
        {
            for (int x = 0; x < numCols; ++x)
            {
                arr[y, x] = "defaultWall";
            }
        }
    }
    public void FillMap(int maxRooms, int minRoomSize = 4, int maxRoomSize = 10)
    {
        int iRoom = maxRooms * 5;
        for (int i = 0; i < iRoom && RoomList.Count < maxRooms; i++)
        {
            RoomGen(minRoomSize, maxRoomSize);
         
        }
        int doorX = RoomList[RoomList.Count - 1].Center.x;
        int doorY = RoomList[RoomList.Count - 1].Center.y;
        while (MapArr[doorY, doorX] != "defaultWall")
        {
            doorX++;
        }
        MapArr[doorY,doorX] = "defaultDoor";
        KeyDoor = new Vector2Int(doorX, doorY);

    }
    public void RoomGen(int minRoomSize, int maxRoomSize)
    {
        bool failed = false;
        int width = rand.Next(minRoomSize, maxRoomSize);
        int heigth = rand.Next(minRoomSize, maxRoomSize);
        int x = rand.Next(1, mapWidth - width - 1);
        int y = rand.Next(1, mapHeight - heigth - 1);
        Room newRoom = new Room(x, y, width, heigth);
        //Debug.Log($"Room x:{x} Room y:{y}");
        foreach (Room other in RoomList)
        {
            if (newRoom.Intercept(other))
            {
                failed = true;
                //Debug.Log("intercept");
                break;
            }
        }
        if (!failed)
        {
            RoomList.Add(newRoom);
            //Debug.Log($"room center:{newRoom.Center.x}, {newRoom.Center.y}");
            for (int rX = newRoom.X1; rX <= newRoom.X2; rX++)
            {
                for (int rY = newRoom.Y1; rY <= newRoom.Y2; rY++)
                {
                    int rando = rY%5;
                    MapArr[rY, rX] = "ground"+rando;
                }
            }
        }
        if (RoomList.Count > 1)
        {
            int x1 = RoomList[RoomList.Count -1].Center.x;
            int y1 = RoomList[RoomList.Count -1].Center.y;
            int x2 = RoomList[RoomList.Count - 2].Center.x;
            int y2 = RoomList[RoomList.Count - 2].Center.y;

            int coin_flip = rand.Next(0, 2);
            if (coin_flip == 0)
            {
                for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
                    MapArr[y1, i] = "ground"+i%5;
                for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
                    MapArr[j, x2] = "ground"+j%5;
                //MapArr[RoomList[RoomList.Count - 1].Center.y, RoomList[RoomList.Count - 1].Center.x] = "defaultWater";

            }
            else if(coin_flip ==1)
            {
                for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
                    MapArr[j, x1] = "ground"+j%5;
                for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
                    MapArr[y2, i] = "ground"+i%5;
                //MapArr[RoomList[RoomList.Count - 1].Center.y, RoomList[RoomList.Count - 1].Center.x] = "defaultWater";

            }
            foreach (Vector2Int vec in RoomList[RoomList.Count-1].WaterList)
            {
                MapArr[vec.y, vec.x] = "defaultWater";
            }
        }
    }
}

public class Room
    {
        public int X1 { get; set; }
        public int X2 { get; set; }

        public int Y1 { get; set; }
        public int Y2 { get; set; }

        public Vector2Int Center { get; set; }
        public List<Vector2Int> WaterList { get; set; }
        System.Random rand = new System.Random();


        public Room(int x, int y, int width, int hight)
        {
            X1 = x;
            Y1 = y;
            X2 = x + width - 1;
            Y2 = y + hight - 1;
            int centerX = (X1 + X2) / 2;
            int CenterY = (Y1 + Y2) / 2;
            WaterList = new List<Vector2Int>();
            Center = new Vector2Int(centerX, CenterY);
            int waterI = rand.Next(2, 8);
            for (int i = 0; i < waterI; i++)
            {
                int wX = rand.Next(X1 + 1, X2 - 1);
                int wY = rand.Next(Y1 + 1, Y2 - 1);
                Vector2Int newVector = new Vector2Int(wX, wY);
                WaterList.Add(newVector);
                
            }
           
        }
        public bool Intercept(Room other)
        {
            return (X1 <= other.X2 + 1 && X2 >= other.X1 - 1 && Y1 <= other.Y2 + 1 && Y2 >= other.Y1 - 1);
        }
    }

