# DBT_API
A C# API for making Unity DBTs (Direct Blend Trees) - These are used as numerous layers is a big performance killer.

# Example Usage:
```csharp
  var DBT = new DBT_API.DBT_Instance(controller, "LayerNameHere");
  
  DBT.masterTree.Trees.Add(new DBT_API.DBT_Instance.BlendState(DBT.masterTree, DBT.controller.parameters.First(o => o.name == "YourToggleParamName"), OffToggleClip, OnToggleClip, "ToggleNameHere"));
```
