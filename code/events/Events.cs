using Sandbox;
using System;
using System.Collections.Generic;  

public class EventBase : LibraryAttribute{
    public virtual string name {get;set;} = "Blank Event";
    public virtual string text {get;set;} = " plates will do nothing in ";
    public virtual string subtext {get;set;} = "There is no subtext";
    public virtual EventType type {get;set;} = EventType.Plate;

    public virtual int minAffected {get;set;} = 2;
    public virtual int maxAffected {get;set;} = 4;

    public virtual void OnEvent() {}
    public virtual void OnEvent(Entity ent) {}
    public virtual void OnEvent(Plate plate) {}

}

public enum EventType{ Player,Plate,Arena }