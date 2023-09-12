using Sandbox;
using System;
using System.Collections.Generic;  

namespace Plates;

[Library]
public class PlatesEvent
{
    public virtual string name {get;set;} = "Nothing";
    public virtual string command {get;set;} = "nothing";
    public virtual string text {get;set;} = " plates will do nothing in ";
    public virtual string subtext {get;set;} = "";
    public virtual EventType type {get;set;} = EventType.Plate;
    public virtual EventRarity rarity {get;set;} = EventRarity.Common;

    public virtual bool hidden {get;set;} = false;

    public virtual int minAffected {get;set;} = 2;
    public virtual int maxAffected {get;set;} = 4;

    public virtual void OnEvent() {}
    public virtual void OnEvent(Entity ent) {}
    public virtual void OnEvent(Plate plate) {}

}

[Library]
public class PlatesRound
{
    public virtual string name {get;set;} = "Normal Round";
    public virtual string command {get;set;} = "round_normal";
    public virtual string description {get;set;} = "Nothing out of the ordinary here";

    public virtual void OnEvent() {}

}

public enum EventType { Player,Plate,Arena }
public enum EventRarity { Common, Uncommon, Rare, Epic }

public class LoadUI : LibraryAttribute {}