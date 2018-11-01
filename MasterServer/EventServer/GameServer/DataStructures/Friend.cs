using System;

[Serializable]
public class Friend
{
    public uint Id;
    public string Name;
    public string Avatar;
    public bool OnlineStatus; //online or not
    public int Status; //0- request, 1 - friend, 2- blocked
    public string Activity; //soon we do this because this is to say if he's playing, idle etc
}