using Sandbox;
using System;
using System.Collections.Generic;  

public class EventBase : LibraryAttribute{
    public virtual string name {get;set;} = "Blank Event";
    public virtual string text {get;set;} = " plates will do nothing in ";
    public virtual string subtext {get;set;} = "";
    public virtual EventType type {get;set;} = EventType.Plate;

    public virtual int minAffected {get;set;} = 2;
    public virtual int maxAffected {get;set;} = 4;

    public virtual void OnEvent() {}
    public virtual void OnEvent(Entity ent) {}
    public virtual void OnEvent(Plate plate) {}

}

public class RoundTypeBase : LibraryAttribute{
    public virtual string name {get;set;} = "Normal";
    public virtual string command {get;set;} = "round_normal";

    public virtual void OnEvent() {}

}

public enum EventType{ Player,Plate,Arena }

public class LoadUI : LibraryAttribute{}