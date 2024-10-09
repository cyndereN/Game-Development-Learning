## Doggy Example

Imagine we're making a pet shelter game, where players can search for dogs that they want to rescue and give a good home.

In our example, players can set any number of search parameters of the kind of dog they would like.

### Without TOptional\<T\>
We might start out with a search parameters struct like this:

```cpp
struct FDogSearchParams
{
	// Treat -1 as unset
	int32 MinimumAge = -1;
	int32 MaximumAge = -1;

	// Treat Magenta as unset
	FLinearColor FurColor = FLinearColor::Magenta;

	// Oh, boolean only has 2 values, what do we use for "unset"?
	bool bHasVaccinations = false;
}

```

```cpp
// Has the player has set a minimum age?
if ( params.MinimumAge != -1 || params.MaximumAge != -1 )
{
	// Filter by age
}

// Has the player chosen a color? Treat FLinearColor::Magenta as
// unset, because pink dogs don't exist
if ( params.FurColor != FLinearColor::Magenta )
{
	// Filter by fur color
}

// Oh true/false could both be valid, now what?
if ( params.bHasVaccinations )
{
}
```

### With TOptional\<T\>

```cpp
struct FDogSearchParams
{
	TOptional<int32> MinimumAge;
	TOptional<int32> MaximumAge;
	TOptional<FLinearColor> FurColor;
	TOptional<bool> bHasVaccinations;
}
```

```cpp
// Has the player has set a minimum age?
if ( params.MinimumAge.IsSet() || params.MaximumAge.IsSet() )
{
	// filter by age ...
}

// Has the player chosen a color?
if ( params.FurColor.IsSet() )
{
	// Filter by fur color
}

if ( params.bHasVaccinations.IsSet() )
{
	// Filter by vaccination state
}
```

### TOptional Usage Overview

```cpp
// After creation, Size.IsSet() returns false
TOptional<int32> Size;

// Assignment is standard, and after this, Size.IsSet() returns true
Size = 16;

// Get the current value with GetValue()
UE_LOG( LogTemp, Verbose, TEXT( "Size is: %d:" ), Size.GetValue() );

// == works as you'd expect, and will return false if IsSet() is false
if ( Size == 16 )
{
	// returns true
}
```

- There is a checkf( IsSet() ) inside GetValue(), so if you call GetValue() on an uninitialized value, your game will crash.
- Get(DefaultValue) is an alternative that if unset, it will return the provided value. e.g. MyDog.IsCute.Get(true) because all dogs are cute unless otherwise specified.

### UI Example

A common way to use TOptional<T> is to make it clear when a newly-instantiated widget's internal cached state has never been set, and so its visual should be refreshed the first time its state is set.

That means that when the widget is instantiated, consider both its cached state and visual state to be undefined

```cpp
UCLASS( Abstract )
class UDogStatusPanel : public UUserWidget
{
	void SetIsTailWagging( bool bIsTailWagging );

protected:
	TOptional<bool> bIsTailWagging;
};
```

```cpp
void UDogStatusPanel::SetIsTailWagging( bool bInIsTailWagging )
{
	// Only change visual state if we have undefined state
	// or if the values differ
	if ( bIsTailWagging.IsSet()
		&& bIsTailWagging == bInIsTailWagging )
	{
		return;
	}

	bIsTailWagging = bInIsTailWagging;
		
	// Set up some expensive visual state...
}
```


扩展：https://juejin.cn/post/7162882431932956680