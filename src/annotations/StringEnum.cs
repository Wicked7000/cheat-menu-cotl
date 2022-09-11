using System;

namespace cheat_menu;

public class StringEnum : Attribute {
    private string value;

    public StringEnum(string value){
        this.value = value;
    }

    public virtual string Value {
        get { return value; }
    }
}