﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class Configuration {
    
    private ConfigurationPumpSettings[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PumpSettings", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ConfigurationPumpSettings[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class ConfigurationPumpSettings {
    
    private ConfigurationPumpSettingsPumpSetting[] pumpSettingField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PumpSetting", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ConfigurationPumpSettingsPumpSetting[] PumpSetting {
        get {
            return this.pumpSettingField;
        }
        set {
            this.pumpSettingField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class ConfigurationPumpSettingsPumpSetting {
    
    private string addressField;
    
    private string portNameField;
    
    private string baudrateField;
    
    private string parityField;
    
    private string databitsField;
    
    private string stopbitsField;
    
    private ConfigurationPumpSettingsPumpSettingNozzlesNozzle[][] nozzlesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address {
        get {
            return this.addressField;
        }
        set {
            this.addressField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string PortName {
        get {
            return this.portNameField;
        }
        set {
            this.portNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Baudrate {
        get {
            return this.baudrateField;
        }
        set {
            this.baudrateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Parity {
        get {
            return this.parityField;
        }
        set {
            this.parityField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Databits {
        get {
            return this.databitsField;
        }
        set {
            this.databitsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Stopbits {
        get {
            return this.stopbitsField;
        }
        set {
            this.stopbitsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    [System.Xml.Serialization.XmlArrayItemAttribute("Nozzle", typeof(ConfigurationPumpSettingsPumpSettingNozzlesNozzle), Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public ConfigurationPumpSettingsPumpSettingNozzlesNozzle[][] Nozzles {
        get {
            return this.nozzlesField;
        }
        set {
            this.nozzlesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public partial class ConfigurationPumpSettingsPumpSettingNozzlesNozzle {
    
    private string gradeField;
    
    private string unitPriceField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Grade {
        get {
            return this.gradeField;
        }
        set {
            this.gradeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string UnitPrice {
        get {
            return this.unitPriceField;
        }
        set {
            this.unitPriceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}
