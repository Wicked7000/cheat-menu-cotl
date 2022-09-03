using System;

namespace cheat_menu;

public class CheatDetails : Attribute {
    private string title;
    private string description;
    private string onTitle;
    private string offTitle;
    private bool isMultiNameFlagCheat = false;

    //Used for simple cheats and mode cheats that have the same state no matter the flag state.
    public CheatDetails(string title, string description){
        this.title = title;
        this.description = description;
    }

    //Used for mode cheats that have different 'names' when being off and on
    //Hide/Show UI is the simpliest example of this
    public CheatDetails(string cheatTitle, string onTitle, string offTitle, string description){
        this.onTitle = onTitle;
        this.offTitle = offTitle;
        this.description = description;
        this.title = cheatTitle;
        this.isMultiNameFlagCheat = true;
    }

    public virtual string Title
    {
        get {return title;}
    }

    public virtual string Description
    {
        get {return description;}
    }

    public virtual string OnTitle
    {
        get {return onTitle;}
    }

    public virtual string OffTitle
    {
        get {return offTitle;}
    }

    public virtual bool IsMultiNameFlagCheat
    {
        get {return isMultiNameFlagCheat;}
    }
}