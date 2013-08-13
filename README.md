Dapper.ColumnMapper
===================

Simple extension to [Dapper](https://code.google.com/p/dapper-dot-net/) to allow arbitrary column to property mapping


Why?
----

Dapper is a wonderfully simple, lightweight object mapper for .NET.

Without any configuration or code whatsoever, it will map the columns in a SQL result set to an object. In order for this to work, the property name and the column in the resultset must be identical - including the case.

If you're writing arbitrary select statements in your code, this normally works quite well. But if you need to use stored procedures written by others, or if the objects have slightly different semantic meaning than do the database columns, you end up having to override Dapper's default type mapping behavior.

I found myself doing this on just about every project I was using, so rather than copy and paste the (admittedly straightforward) mapping logic, I decided to turn it into a reuseable package delivered over NuGet.

How?
---

Using the ColumnMapper is a two step process.

First, decorate the target properties with the `[ColumnMapping('Foo')]` attribute, where 'Foo' is the name of the column in the result set. _Note: You can skip this step if you're just trying to take advantage of the case-insensitive mapping provided by the ColumnMapper._

Second, register the `ColumnTypeMapper` with Dapper for the target object types. 

You can use the traditional Dapper method:

```C#
	 SqlMapper.SetTypeMap(typeof(Bar), new ColumnTypeMapper(typeof(Bar)));
````

Or the provided convenience method that lets you register multiple types at once:

```C#
	ColumnTypeMapper.RegisterForTypes(typeof(Bar), typeof(Baz));
```

Where?
------

The easiest way is to use the NuGet package manager from within Visual Studio and install the Dapper.ColumnMapper package.

If you really want to, you can download and build from source.
