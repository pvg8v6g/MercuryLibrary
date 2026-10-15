- use pattern matching for anything that can use it
* ❌ **Avoid:**
  ```csharp
  var text = obj as string;
  if (text != null) { /* use text */ }
  ```
* ✔️ **Use:**
  ```csharp
  if (obj is string text) { /* use text */ }
  ```
  ```csharp
  if (someVar is 0) { /* use text */ }
  ```
