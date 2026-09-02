# Mandatory project coding rules

Apply these rules to every code change in this repository.

1. All WPF UI belongs in XAML.
2. Every Window and UserControl must have both a `.xaml` and `.xaml.cs` file.
3. Never construct a Window or UserControl visual tree in C#.
4. Extend existing XAML UI in XAML.
5. Toolbar buttons use PNG icons, never text or Unicode glyphs.
6. Toolbars use regular Buttons, never RadioButtons.
7. Every interactive WPF control, including Button, ToggleButton, and RadioButton, must have a unique, purpose-specific `x:Name` following project conventions.
8. Never create visual trees in C#.
9. Reuse existing styles and resources before creating new ones.
10. Every C# statement ending in a semicolon must be on its own line. Every method signature or declaration must be on its own line. Never collapse method bodies onto one line.
11. Follow the existing project architecture and naming conventions.
12. Do not introduce dependency injection, frameworks, or architectural patterns unless the existing code uses them or there is a concrete reason.
13. Fix the underlying implementation rather than layering a workaround over it.
14. Do not refactor unrelated working code.
15. Keep changes narrowly scoped to the requested task.
16. Do not stage, commit, reset, revert, or otherwise modify Git repository state unless explicitly requested. Git status and diffs may be inspected.
17. If an image-button style does not exist, wire in a global style that makes image-button backgrounds transparent.
18. Generated code must never contain multiple commands on one line. After every statement-ending semicolon, generated output must continue on a new line.

Forbidden example:

```csharp
public void Close(){Dispose();_view.Close();}
```

Required form:

```csharp
public void Close()
{
    Dispose();
    _view.Close();
}
```
