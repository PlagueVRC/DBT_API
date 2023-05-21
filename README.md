# DBT_API
A C# api for making Unity DBT's (Direct Blend Tree's) - These are used as numerous layers is a big performance killer.

# Example Usage:
```csharp
  var DBT = new DBT_API.DBT_Instance(controller, "LayerNameHere");
  
  DBT.masterTree.Trees.Add(new DBT_API.DBT_Instance.BlendState(DBT.masterTree, DBT.controller.parameters.First(o => o.name == "YourToggleParamName"), OffToggleClip, OnToggleClip, "ToggleNameHere"));
```
