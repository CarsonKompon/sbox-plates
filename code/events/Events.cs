using Sandbox;
using System;
using System.Collections.Generic;  

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class PlatesEventAttribute : LibraryAttribute
{
    public virtual string name {get;set;} = "nothing";
    public virtual string text {get;set;} = " plates will do nothing in ";
    public virtual string subtext {get;set;} = "";
    public virtual EventType type {get;set;} = EventType.Plate;

    public virtual bool hidden {get;set;} = false;

    public virtual int minAffected {get;set;} = 2;
    public virtual int maxAffected {get;set;} = 4;

    public virtual void OnEvent() {}
    public virtual void OnEvent(Entity ent) {}
    public virtual void OnEvent(Plate plate) {}

}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class PlatesRoundAttribute : LibraryAttribute
{
    public virtual string name {get;set;} = "Normal Round";
    public virtual string command {get;set;} = "round_normal";
    public virtual string description {get;set;} = "Nothing out of the ordinary here";

    public virtual void OnEvent() {}

}

public enum EventType { Player,Plate,Arena }

public class LoadUI : LibraryAttribute {}