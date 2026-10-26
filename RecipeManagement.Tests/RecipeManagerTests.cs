using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualBasic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }
    // Constuctor Tests
    [Fact]
    public void Constructor_NullRecipes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_RecipesIdLowerThan0_ThrowsArgumentException()
    {
        var recipe1 = new Recipe{Id = -1, Title = "Pancakes"};
        Assert.Throws<ArgumentException>(() => new RecipeManager([recipe1]));
    }

    [Fact]
    public void Constructor_EmptyRecipeTitle_ThrowsArgumentException()
    {
        var recipe1 = new Recipe{Id = 34, Title = " "};
        Assert.Throws<ArgumentException>(() => new RecipeManager([recipe1]));
    }

    [Fact]
    public void Constructor_DuplicateId_ThrowsArgumentException()
    {
        var recipe1 = new Recipe{Id = 34, Title = "Pancakes"};
        var recipe2 = new Recipe{Id = 34, Title = "Job"};

        Assert.Throws<ArgumentException>(() => new RecipeManager([recipe1,recipe2]));
    }

    // FindRecipe() tests
    [Fact]
    public void FindRecipe_ExistingId_ReturnsRecipe()
    {
        var manager = CreateManager();
        var recipe = manager.FindRecipe(10);
        Assert.NotNull(recipe);
        Assert.Equal("Recipe A", recipe.Title);
    }

    [Fact]
    public void FindRecipe_MissingId_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.FindRecipe(999));
    }
    
    // AddRecipe() tests
    [Fact]
    public void AddRecipe_ValidRecipe_ReturnsTrueAndIncreasesCount()
    {
        var recipe1 = new Recipe{Id = 34, Title = "Pancakes"};
        var manager = CreateManager();
        Assert.True(manager.AddRecipe(recipe1));
        Assert.Equal(3,manager.RecipeCount);
        Assert.Same(recipe1,manager.FindRecipe(34));
    }

    [Fact]
    public void AddRecipe_DuplicateId_ReturnsFalseAndCountUnchanged()
    {
        var recipe1 = new Recipe{Id = 10, Title = "Pancakes"};
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(recipe1));
        Assert.Equal(2,manager.RecipeCount);
        var correct = manager.FindRecipe(10);
        Assert.Equal("Recipe A",correct?.Title);
    }

    [Fact]
    public void AddRecipe_NonPositiveId_ReturnsFalse()
    {
        var recipe1 = new Recipe{Id = -10, Title = "Pancakes"};
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(recipe1));
        Assert.Equal(2,manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_EmptyTitle_ReturnsFalse()
    {
        var recipe1 = new Recipe{Id = 99, Title = ""};
        var manager = CreateManager();
        Assert.False(manager.AddRecipe(recipe1));
        Assert.Equal(2,manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_NullRecipe_ThrowsArgumentNullException()
    {
        var manager = CreateManager();
        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    // RemoveRecipe tests
    [Fact]
    public void RemoveRecipe_ExistingId_ReturnsTrueAndDecreasesCount()
    {
        var manager = CreateManager();
        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1,manager.RecipeCount);
        Assert.Null(manager.FindRecipe(10));
    }

    [Fact]
    public void RemoveRecipe_MissingId_ReturnsFalseAndCountUnchanged()
    {
        var manager = CreateManager();
        Assert.False(manager.RemoveRecipe(100));
        Assert.Equal(2,manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_RecipeInCookingPlan_ReturnsFalseAndDoesNotRemove()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);          
        Assert.NotNull(manager.FindRecipe(10)); 
    }
    // AddIngredient to shoppinglist tests
    [Fact]
    public void AddIngredientsToShoppingList_ExistingRecipe_AddsIngredients()
    {
        var manager = CreateManager();
        var addedCount = manager.AddIngredientsToShoppingList(10);
        Assert.Equal(1, addedCount);
        Assert.Equal(1, manager.ShoppingItemCount);
        Assert.Equal(["1 apple"], manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_MissingRecipe_ReturnsZero()
    {
        var manager = CreateManager();
        var addedCount = manager.AddIngredientsToShoppingList(999);
        Assert.Equal(0, addedCount);
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_RecipeWithoutIngredients_ReturnsZero()
    {
        var manager = CreateManager();
        var addedCount = manager.AddIngredientsToShoppingList(20);
        Assert.Equal(0, addedCount);
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredientsToShoppingList_CalledTwice_AddsIngredientsTwice()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        manager.AddIngredientsToShoppingList(10);
        Assert.Equal(2, manager.ShoppingItemCount);
        Assert.Equal(new[] { "1 apple", "1 apple" },manager.GetShoppingList());
    }
    // GetShoppingList and ClearShoppingList tests
    [Fact]
    public void GetShoppingList_ReturnsIngredientsInListOrder()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        Assert.Equal(["1 apple"], manager.GetShoppingList());
    }
    
    [Fact]
    public void ClearShoppingList_RemoveAllItems()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);
        manager.AddIngredientsToShoppingList(10);
        manager.ClearShoppingList();
        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0,manager.ShoppingItemCount);
    }

    // AddRecipeToCookingPlan items
    [Fact]

    public void AddRecipeToCookingPlan_ExistingRecipe_AddsRecipe()
    {
        var manager = CreateManager();
        var result = manager.AddRecipeToCookingPlan(10);
        Assert.True(result);
        Assert.Equal(1,manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_MissingRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        var result = manager.AddRecipeToCookingPlan(9991);
        Assert.False(result);
        Assert.Equal(0,manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_DuplicateRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.True(manager.AddRecipeToCookingPlan(10));
        var result = manager.AddRecipeToCookingPlan(10);
        Assert.False(result);
        Assert.Equal(1,manager.CookingPlanCount);
    }
    // RemoveRecipeToCookingPlan items
    [Fact]
    public void RemoveRecipeFromCookingPlan_ExistingRecipe_RemovesAndRecordsHistory()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        var result = manager.RemoveRecipeFromCookingPlan(10);
        Assert.True(result);
        Assert.Equal(0,manager.CookingPlanCount);
        Assert.Equal(1,manager.RemovedRecipeCount);
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_MissingRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        var result = manager.RemoveRecipeFromCookingPlan(10);
        Assert.False(result);
        Assert.Equal(0,manager.CookingPlanCount);
        Assert.Equal(0,manager.RemovedRecipeCount);
    }

    //PeekLastRemoveRecipe tests
    [Fact]
    public void PeekLastRemovedRecipe_EmptyHistory_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void PeekLastRemovedRecipe_ReturnsMostRecentlyRemovedRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20,manager.PeekLastRemovedRecipe());
        Assert.Equal(2,manager.RemovedRecipeCount);

    }
    // RestoreLastRemoveRecipe tests
    [Fact]
    public void RestoreLastRemovedRecipe_EmptyHistory_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_RestoresLastRemovedRecipe()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(1,manager.CookingPlanCount);
        Assert.Equal(0,manager.RemovedRecipeCount);
    }
    
    /*
    RestoreLastRemovedRecipe pops the ID before checking eligibility, matching the order described in the spec. 
    If restoration fails, the ID is not returned to the stack.
    */
    [Fact]
    public void RestoreLastRemovedRecipe_RecipeDeletedFromCatalogue_FailsAndDoesNotReturnIdToStack()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipe(10);
        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(0,manager.CookingPlanCount);
        Assert.Equal(0,manager.RemovedRecipeCount);
    }

    // GetCookingPlan tests
    [Fact]
    public void GetCookingPlan_ReturnsIdsInOrder()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        Assert.Equal([10,20],manager.GetCookingPlan());
    }

    [Fact]
    public void GetCookingPlan_ModifyingResult_DoesNotAffectInternalPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        var cooking = manager.GetCookingPlan();
        manager.AddRecipeToCookingPlan(20);
        Assert.Equal([10], cooking);
    }

    //Start cooking test
    [Fact]
    public void StartCooking_ExistingRecipeWithInstructions_LoadsQueueInOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal(2,manager.PendingInstructionCount);
        Assert.Equal("First step", manager.PeekNextInstruction());
    }

    [Fact]
    public void StartCooking_MissingRecipe_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.StartCooking(992));
        Assert.Equal(0,manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_RecipeWithNoInstructions_ReturnsFalse()
    {
        var manager = CreateManager();
        Assert.False(manager.StartCooking(20));
        Assert.Equal(0,manager.PendingInstructionCount);
    }

    // Peek and Complete next instruction tests
    [Fact]
    public void PeekNextInstruction_EmptyQueue_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.PeekNextInstruction());
    }
    
    [Fact]
    public void PeekNextInstruction_DoesNotRemoveItem()
    {
        var manager = CreateManager();
        manager.StartCooking(10);
        var value1 = manager.PeekNextInstruction();
        var value2 = manager.PeekNextInstruction();
        Assert.Equal(value1,value2);
        Assert.Equal(2,manager.PendingInstructionCount);
    }

    [Fact]
    public void CompleteNextInstruction_EmptyQueue_ReturnsNull()
    {
        var manager = CreateManager();
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void CompleteNextInstruction_RemovesExactlyOneItemFromFront()
    {
        var manager = CreateManager();
        manager.StartCooking(10);
        Assert.Equal("First step",manager.CompleteNextInstruction());
        Assert.Equal(1,manager.PendingInstructionCount);
        Assert.Equal("Second step",manager.PeekNextInstruction());
    }

    // T5: Test for interaction between 2 components Dictionary and Queue
    [Fact]
    public void AddRecipeThenStartCooking_LoadsInstructionsFromNewlyAddedRecipe()
    {
        var manager = CreateManager();
        var newRecipe = new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Ingredients = new() {"1 apple", "2 coconut"},
            Instructions = new() { "Wash", "Chop", "Mix"}
        };
        Assert.True(manager.AddRecipe(newRecipe));
        Assert.True(manager.StartCooking(30));
        Assert.Equal(3,manager.PendingInstructionCount);
        Assert.Equal("Wash",manager.PeekNextInstruction());
    }
    // SearchByTitle tests
    [Fact]
    public void SearchByTitle_PartialMatch_ReturnsMatchingRecipe()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("pe A");
        var recipe = Assert.Single(results);
        Assert.Equal(10,recipe.Id);
    }

    [Fact]
    public void SearchByTitle_DifferentCase_ReturnsMatchingRecipe()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("recipe A");
        var recipe = Assert.Single(results);
        Assert.Equal(10,recipe.Id);
    }

    [Fact]
    public void SearchByTitle_BlankText_ReturnsAllRecipes()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("");
        Assert.Equal(2,results.Count);
        Assert.Contains(results,recipe => recipe.Id == 10);
        Assert.Contains(results,recipe => recipe.Id == 20);
    }

    [Fact]
    public void SearchByTitle_BlankTextWithSpaces_ReturnsAllRecipes()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("    ");
        Assert.Equal(2,results.Count);
        Assert.Contains(results,recipe => recipe.Id == 10);
        Assert.Contains(results,recipe => recipe.Id == 20);
    }

    [Fact]
    public void SearchByTitle_NullText_ReturnsAllRecipes()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle(null!);
        Assert.Equal(2,results.Count);
        Assert.Contains(results,recipe => recipe.Id == 10);
        Assert.Contains(results,recipe => recipe.Id == 20);
    }

    [Fact]
    public void SearchByTitle_NoMatch_ReturnsEmptyList()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("Recipe C");
        Assert.Empty(results);
    }
    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
