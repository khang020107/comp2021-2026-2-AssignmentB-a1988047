# Recipe Management System — Student Starter

Starter repository for Parts A and B. Implement `RecipeManager` in Core; the Application menu and JSON loader are supplied.

## What is supplied

- `RecipeManagement.Core/Models/` — recipe model classes
- `RecipeManagement.Core/RecipeLoader.cs` — reads `data/recipes.json`
- `RecipeManagement.Core/IRecipeManager.cs` — public API
- `RecipeManagement.Application/` — console menu (options labelled PartA / PartB)
- `RecipeManagement.Tests/` — example tests
- `data/recipes.json` — recipe dataset

## What you implement

**Part A** — `RecipeManager.cs` using:

- `Dictionary<int, Recipe>`
- `List<string>`
- `LinkedList<int>`
- `Stack<int>`
- `Queue<string>`

**Part B** — LINQ searches, protein report, saved-recipe collection, `Design.md`, and more tests.

## Build and run

Whether you download the submission from Canvas or clone the GitHub repository,
use the repository root (the folder containing `RecipeManagement.sln`) as your
working directory. Run the following commands in a terminal from that folder:

```bash
dotnet build
dotnet test
dotnet run --project RecipeManagement.Application -- data/recipes.json
```

## AI acknowledgement

AI acknowledgement: I used Claude to help me understand Dictionary key lookup, encapsulation with IReadOnlyList, xUnit Assert.Throws, how to select suitable data structure for PartB, How to correctly use LINQ, .?, HashSet, logic thinking and step by step guide to write tests that cover all conditions of the function, and generate plan to complete the assignment. I did not copy or adapt AI-generated code or other material into my submission. I developed the submitted solution myself based on my understanding of the course material.
