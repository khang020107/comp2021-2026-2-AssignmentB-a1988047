# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

- The C# collection that I have selected for the saved feature is 'HashSet<int>', stored in RecipeManager.cs. The set stores the Ids of saved recipes and for me it's suitable because HashSet only stores unique values and doesn't require order, which are 2 features in the instruction for PartB "Only existing recipe IDs may be added and duplicates are not allowed","The order of saved recipe IDs is not assessed." 

- When a recipe is saved, AddSavedRecipe first checks whether the recipe exists in the recipe catalogue. If the recipe does not exist, the operation returns false. If the recipe exists, _savedRecipeIds.Add(recipeId) adds its ID to the set. The Add method returns true when the ID is newly added and returns false when the ID is already present, so duplicate saved recipes are prevented automatically.

- When removing a saved recipe, RemoveSavedRecipe calls _savedRecipeIds.Remove(recipeId). This returns true if the ID was in the set and was successfully removed. It returns false otherwise, if item was not saved, so attemping to remove the same recipe twice is rejected.

- For membership checking, IsRecipeSaved uses _savedRecipeIds.Contains(recipeId). A HashSet<int> normally performs Add, Remove and Contains in average O(1) time because it is hash-based (it uses the hash of the ID to locate it directly).

- A List<int> would have not been less suitable for me because it would allow duplication Ids unless duplicates were checked manually using .Contains(). It also requires O(n) time complexity to search for duplicate prevention and membership checks.

## 2. Integration

In a short paragraph, explain how the new Part B features use the existing Part A `RecipeManager` and recipe catalogue.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | Your chosen collection | | Explain how your collection performs membership checks. |