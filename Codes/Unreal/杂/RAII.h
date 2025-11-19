#pragma once

PUSH_RELAXED_COMPILER_WARNING_LEVEL
#include <utility>
POP_RELAXED_COMPILER_WARNING_LEVEL

/**
 * RAII wrapper to call a function on scope exit
 */
// "Consider explicit construction and if/how to force a non-null Function."
struct FAtScopeExit
{
	FAtScopeExit(TFunction<void()>&& InFunction) : Function(MoveTemp(InFunction)) {}
	FAtScopeExit(const FAtScopeExit&) = delete;
	FAtScopeExit(FAtScopeExit&&) = default;
	~FAtScopeExit()
	{
		Function();
	}

	FAtScopeExit& operator=(const FAtScopeExit&) = delete;
	FAtScopeExit& operator=(FAtScopeExit&&) = default;

	/// The function to call at scope exit
	TFunction<void()> Function;
};

/// Legacy helper function to create a FAtScopeExit (previously required for a template workaround)
template <typename TFunction>
FAtScopeExit AtScopeExit(TFunction Function)
{
	return { std::move(Function) };
}

/**
 * RAII wrapper to handle setting/clearing a flag based on scope lifetime
 */
struct FScopedSetFlag
{
	/// Sets the flag to the given value on scope entry and reverts to original value on exit
	FScopedSetFlag(
		bool& bInFlagToSet,
		const bool bInValueOnEntry = true)
		: FScopedSetFlag(bInFlagToSet, bInValueOnEntry, bInFlagToSet)
	{}

	/// Sets the flag to the given values on scope entry and exit 
	FScopedSetFlag(
		bool& bInFlagToSet,
		const bool bInValueOnEntry,
		const bool bInValueOnExit)
		: bFlagToSet(bInFlagToSet)
		, bValueOnExit(bInValueOnExit)
	{
		bFlagToSet = bInValueOnEntry;
	}

	~FScopedSetFlag()
	{
		bFlagToSet = bValueOnExit;
	}

private:

	bool& bFlagToSet;
	const bool bValueOnExit;
};
