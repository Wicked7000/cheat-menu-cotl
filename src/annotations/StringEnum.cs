using System;

namespace CheatMenu;

public class StringEnum : Attribute {
    private readonly string _value;

    public StringEnum(string value){
        this._value = value;
    }

    public virtual string Value {
        get { return _value; }
    }
}