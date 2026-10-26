using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Linq;
namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    private readonly Dictionary<int,Recipe> _recipes = new();
    private readonly List<string> _shoppingList= new();
    private readonly LinkedList<int> _cookingPlan = new();
    private readonly Stack<int> _removedRecipeHistory = new();
    private readonly Queue<string> _instructionQueue = new();
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        if (recipes == null) throw new ArgumentNullException(nameof(recipes));
        foreach(Recipe recipe in recipes)
        {
            if (recipe == null) throw new ArgumentNullException(nameof(recipe));
            if (recipe.Id <= 0) throw new ArgumentException($"Your recipe id {recipe.Id} is lower or equal to 0.");
            if (string.IsNullOrWhiteSpace(recipe.Title)) throw new ArgumentException("Your recipe title is currently null");
            if (_recipes.ContainsKey(recipe.Id)) throw new ArgumentException($"Duplicate recipe ID found: {recipe.Id}");
            _recipes.Add(recipe.Id,recipe);
        }
    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructionQueue.Count;
    public int RemovedRecipeCount => _removedRecipeHistory.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null) throw new ArgumentNullException(nameof(recipe));
        if (recipe.Id <= 0) return false;
        if (string.IsNullOrWhiteSpace(recipe.Title)) return false;
        if(_recipes.ContainsKey(recipe.Id)) return false;
        _recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        _recipes.TryGetValue(recipeId,out var value);
        return value; 
    }

    public bool RemoveRecipe(int recipeId)
    {
        if(FindRecipe(recipeId) == null) return false;
        if(_cookingPlan.Contains(recipeId)) return false;
        return _recipes.Remove(recipeId);
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if(recipe == null) return 0;
        _shoppingList.AddRange(recipe.Ingredients);
        return recipe.Ingredients.Count;   
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return _shoppingList.AsReadOnly(); // AsReadOnly() return read-only wrapper around a collection
    }
    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if(FindRecipe(recipeId) == null) return false;
        if(_cookingPlan.Contains(recipeId)) return false;
        _cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if(!_cookingPlan.Remove(recipeId)) return false;
        _removedRecipeHistory.Push(recipeId);
        return true;
    }
  
    public bool RestoreLastRemovedRecipe()
    {
        if(_removedRecipeHistory.Count == 0) return false;
        int recipeId = _removedRecipeHistory.Pop();
        if(FindRecipe(recipeId) == null) return false;
        if(_cookingPlan.Contains(recipeId)) return false;
        _cookingPlan.AddLast(recipeId);
        return true;
    }
    public int? PeekLastRemovedRecipe()
    {
        if(_removedRecipeHistory.Count == 0) return null;
        return _removedRecipeHistory.Peek(); 
    }
    public IReadOnlyList<int> GetCookingPlan()
    {
        return _cookingPlan.ToList();
    }

    public bool StartCooking(int recipeId)
    {
        Recipe? recipe = FindRecipe(recipeId);
        if(recipe == null) return false;
        if(recipe.Instructions.Count == 0) return false;
        _instructionQueue.Clear();
        foreach(string instruction in recipe.Instructions)
        {
            _instructionQueue.Enqueue(instruction);
        }
        return true;
    }

    public string? PeekNextInstruction()
    {
        if(_instructionQueue.Count == 0) return null;
        return _instructionQueue.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if(_instructionQueue.Count == 0) return null;
        return _instructionQueue.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if(string.IsNullOrWhiteSpace(searchText))
        {
            return _recipes.Values.ToList();
        }
        return _recipes.Values.Where(recipe => recipe.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if(string.IsNullOrWhiteSpace(searchText))
        {
            return _recipes.Values.ToList();
        }
        return _recipes.Values.Where(recipe => 
        recipe.Ingredients.Any(ingredient => 
        ingredient.Contains(searchText,StringComparison.OrdinalIgnoreCase))).ToList();
    }

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
