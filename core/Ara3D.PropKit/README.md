# Ara3D.PropKit

**Ara3D.PropKit** is a toolkit for generic run-time property management in C# which is compatible with WinForms and WPF data binding. 

The primary use cases are auto UI, data serialization, change monitoring, and notification. 

## Overview 

In the context of this library a "property" is any piece of data, whether it is a field, a property, or an entry into a dictionary,
that has associated meta-data (such as a name, type, UI label) and a validation mechanism. 
Properties are described by `PropDescriptor` instances, which can validate values and provide metadata about the property.

It complements classes in `System.ComponentModel`, but is easier to work with and has improved separation of concerns. 

For example the `Ara3D.PropKit.PropDescriptor` differs from 
`System.ComponentModel.PropertyDescriptor` in that it has no knowledge about getting / setting values. It simply describes a property,
and how to validate it.  

Getting and setting of property values is done via the `PropAccessor` class, which can be used to wrap any getter or setter function. 
The `PropProvider` class acts as a container around a collection of `PropAccessor`s, and implements the `IPropContainer` interface.

The `IPropContainer` interface provides a way to get or set values by name, or all at once. It also inherts from `INotifyPropertyChanged` so that changes to properties can be monitored.
It also implements `ICustomTypeProvider` so that it can be used in data binding scenarios by providing run-time type information about the properties it contains
in a way that is compatible with existing frameworks that use `System.ComponentModel`, such as WPF and WinForms.

Values can be more easily validated, because they are typically passed and returned as `PropValue` instances which wrap the the value and the `PropDescriptor`
together. The construction of a `PropValue` forces validation of the value against the `PropDescriptor`.

You can use a `PropContainerWrapper` as a dynamic view model around any class, and you can then bind to any property or field on that class.

## Classes and Interfaces

* `PropDescriptor` - A class which provides a type, name, UI label, default value, and validation mechanism for a specific kind of property. 
* `PropValue` - A `PropDescriptor` and an associated value. Provides validation upon construction. 
* `PropAccessor` - A class which associates a `PropDescriptor` with a getter and optional setter function. 
* `IPropContainer` - An interface inheriting `INotifyPropertyChanged` and `ICustomTypeProvider` to provide access to get or set values individually by name, or in bulk as collections of `PropValue`
* `PropProvider` - Implements `IPropContainer` by wrapping a collection of `PropAccessor`s.
* `PropContainerDictionary` - Implements `IPropContainer`. This is a generic key/value collection that stores `PropValue`. 
* `PropContainerWrapper` - Implements `IPropContainer`. Provides a wrapper around any C# object to provide `IPropContainer` access to its properties and fields. 

## License

This project is licensed under the [MIT License](LICENSE).

**Enjoy building dynamic, descriptor-driven UIs in WPF with Ara3D.PropKit!**
