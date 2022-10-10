using System;

namespace CheatMenu;

public class CheatDetails : Attribute {
    private readonly string _title;
    private readonly string _description;
    private readonly string _onTitle;
    private readonly string _offTitle;
    private readonly bool _isMultiNameFlagCheat = false;
    private readonly bool _isFlagCheat = false;

    //Used for simple cheats and mode cheats that have the same state no matter the flag state.
    public CheatDetails(string title, string description, bool isFlagCheat = false){
        this._isFlagCheat = isFlagCheat;
        this._title = title;
        this._description = description;
    }

    //Used for mode cheats that have different 'names' when being off and on
    //Hide/Show UI is the simpliest example of this
    public CheatDetails(string cheatTitle, string offTitle, string onTitle, string description, bool isFlagCheat = true){
        if(isFlagCheat == false){
            throw new Exception("Multi name flag cheat can not have isFlagCheat set to false!");
        }

        this._onTitle = onTitle;
        this._offTitle = offTitle;
        this._description = description;
        this._title = cheatTitle;
        this._isMultiNameFlagCheat = true;
        this._isFlagCheat = true;
    }

    public virtual string Title
    {
        get {return _title;}
    }

    public virtual string Description
    {
        get {return _description;}
    }

    public virtual string OnTitle
    {
        get {return _onTitle;}
    }

    public virtual string OffTitle
    {
        get {return _offTitle;}
    }

    public virtual bool IsFlagCheat
    {
        get {return _isFlagCheat;}
    }

    public virtual bool IsMultiNameFlagCheat
    {
        get {return _isMultiNameFlagCheat;}
    }
}