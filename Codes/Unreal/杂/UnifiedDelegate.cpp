// declaration
DECLARE_MULTICAST_DELEGATE_TwoParams(FOnBestTargetChangedMulticast,
	FTargetID /*TargetID*/, FName /*Interest*/);
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnBestTargetChangedDynamic,
	FTargetID, TargetID, FName, Interest);
using FOnBestTargetChangedUnified = TUnifiedDelegate<FOnBestTargetChangedDynamic, FOnBestTargetChangedMulticast>;


// private
TMap<FName, FOnBestTargetChangedUnified> OnBestTargetChanged;


// public
// Add or remove a move target event delegate for move targets with the given name
UFUNCTION(BlueprintCallable)
void AddOnBestTargetChanged(FName Interest, FOnBestTargetChangedDynamic Delegate);
void UTargetInterestComponent::AddOnBestTargetChanged(FName Interest, FOnBestTargetChangedDynamic Delegate)
{
	auto Result = OnBestTargetChanged.FindOrAdd(Interest);
	Result.Bind(Delegate);
}

UFUNCTION(BlueprintCallable)
void RemoveOnBestTargetChanged(FName Interest, FOnBestTargetChangedDynamic Delegate);
void UTargetInterestComponent::RemoveOnBestTargetChanged(FName Interest, FOnBestTargetChangedDynamic Delegate)
{
	if (auto Iterator = OnBestTargetChanged.Find(Interest))
	{
		Iterator->Remove(Delegate);
	}
}


// Gets a delegate that is called when the best target for the given interest changes.
FOnBestTargetChangedMulticast& GetOnBestTargetChanged(FName Interest);
FOnBestTargetChangedMulticast& UTargetInterestComponent::GetOnBestTargetChanged(FName Interest)
{
	auto& Result = OnBestTargetChanged.FindOrAdd(Interest);
	return Result.GetDelegate();
}

// acutal usage
FDelegateHandle OnBestTargetChangedHandle = TargetInterestComponent->GetOnBestTargetChanged(CombatInterestName)
		.AddUObject(this, &UTargetInterestTest_SetGetBestTarget::OnBestTargetChangedCallback);


TargetInterestComponent->GetOnBestTargetChanged(CombatInterestName)
		.Remove(OnBestTargetChangedHandle);



// in component's SetBestTarget
GetOnBestTargetChanged(Interest).Broadcast(BestTargetID, Interest);


// --------------------


// declaration
DECLARE_MULTICAST_DELEGATE_TwoParams(FOnInterestsChangedMulticast,
	FTargetID /*TargetID*/, const TSet<FName>& /*CurrentInterests*/);
DECLARE_DYNAMIC_DELEGATE_TwoParams(FOnInterestsChangedDynamic,
	FTargetID, TargetID, const TSet<FName>&, CurrentInterests);
using FOnInterestsChanged = FOnInterestsChangedMulticast::FDelegate;
using FOnInterestsChangedUnified = TUnifiedDelegate<FOnInterestsChangedDynamic, FOnInterestsChangedMulticast>;

// private
struct FOnInterestsChangedDelegateInfo
{
    FOnInterestsChangedUnified Delegate;

    TMap<FGuid, FDelegateHandle> DelegateHandles;
};

TMap<FTargetID, FOnInterestsChangedDelegateInfo> OnInterestsChanged;

// public
// Register callbacks for interest change events.
UFUNCTION(BlueprintCallable)
FGuid AddOnInterestsChanged(FTargetID TargetID, FOnInterestsChangedDynamic Callback);

FGuid UTargetInterestComponent::AddOnInterestsChanged(FTargetID TargetID, FOnInterestsChangedDynamic Callback)
{
	FOnInterestsChangedDelegateInfo& DelegateInfo = OnInterestsChanged.FindOrAdd(TargetID);
	FDelegateHandle Handle = DelegateInfo.Delegate.Bind(Callback);
	FGuid CallbackID = FGuid::NewGuid();
	DelegateInfo.DelegateHandles.Emplace(CallbackID, Handle);

	return CallbackID;
}

FGuid AddOnInterestsChanged(FTargetID TargetID, FOnInterestsChanged Callback);
FGuid UTargetInterestComponent::AddOnInterestsChanged(FTargetID TargetID, FOnInterestsChanged Callback)
{
	FOnInterestsChangedDelegateInfo& DelegateInfo = OnInterestsChanged.FindOrAdd(TargetID);
	FDelegateHandle Handle = DelegateInfo.Delegate.GetDelegate().Add(Callback);
	FGuid CallbackID = FGuid::NewGuid();
	DelegateInfo.DelegateHandles.Emplace(CallbackID, Handle);

	return CallbackID;
}



// Removes a registered callback for interest change events.
UFUNCTION(BlueprintCallable)
void RemoveOnInterestsChanged(FTargetID TargetID, FGuid CallbackID);

void UTargetInterestComponent::RemoveOnInterestsChanged(FTargetID TargetID, FGuid CallbackID)
{
	if (FOnInterestsChangedDelegateInfo* const DelegateInfo = OnInterestsChanged.Find(TargetID))
	{
		if (FDelegateHandle* const DelegateHandle = DelegateInfo->DelegateHandles.Find(CallbackID))
		{
			DelegateInfo->Delegate.Remove(*DelegateHandle);
			DelegateInfo->DelegateHandles.Remove(CallbackID);
			if (DelegateInfo->DelegateHandles.Num() == 0)
			{
				OnInterestsChanged.Remove(TargetID);
			}
		}
		else
		{
			LOG_ERROR(LogTargetInterests, TEXT("Specified delegate handle for CallbackID does not exist!!!"));
		}
	}
	else
	{
		LOG_WARN(LogTargetInterests, TEXT("Specified delegate info for TargetID does not exist!"));
	}
}



// acutal usage
FGuid Target1CallbackID = TargetInterestComponent->AddOnInterestsChanged(Target1.GetTargetID(),
    FOnInterestsChanged::CreateUObject(this, &UTargetInterestTest_DelegateTest::OnInterestsChangedCallback));
TargetInterestComponent->RemoveOnInterestsChanged(Target1.GetTargetID(), Target1CallbackID);

void UTargetInterestTest_DelegateTest::OnInterestsChangedCallback(
	FTargetID TargetID, const TSet<FName>& CurrentInterests)
{
	bOnInterestsChangedCalled = true;
	TestEqual(TEXT("Got ExpectedTargetID from callback"), TargetID, ExpectedTargetID);

	TestEqual(TEXT("CurrentInterests number matches"), CurrentInterests.Num(), ExpectedCurrentInterests.Num());
	for (const FName Interest : ExpectedCurrentInterests)
	{
		TestTrue(TEXT("CurrentInterests elements match"), CurrentInterests.Contains(Interest));
	}
}

// when interest changes
if (FOnInterestsChangedDelegateInfo* Info = OnInterestsChanged.Find(TargetID))
{
    Info->Delegate.Broadcast(TargetID, TargetState.Interests);
}

// --------------------------------------------
// UnifiedDelegate.h
#pragma once

PUSH_RELAXED_COMPILER_WARNING_LEVEL
#include <algorithm>
#include <Delegates/IDelegateInstance.h>
#include <Logging/LogMacros.h>
#include <map>
POP_RELAXED_COMPILER_WARNING_LEVEL

/**
 * FUnifiedDelegate
 *
 * This helper class provides an interface for delegates that wish to be handled both by
 * blueprints and by code.
 *
 * Typical usage would be to store this class as a member in your UCLASS, provide UFUNCTIONs
 * for binding delegates (which pass their parameters down to this class). Code users just
 * need access to this class's GetDelegate member function in order to bind lambdas.
 */
template <typename DynamicSinglecastDelegateType, typename MulticastDelegateType>
class TUnifiedDelegate
{
public:

	/** Binds by object and function name */
	FDelegateHandle Bind(UObject* Object, FName FunctionName)
	{
		DynamicSinglecastDelegateType ScriptDelegate;
		ScriptDelegate.BindUFunction(Object, FunctionName);

		return Bind(ScriptDelegate);
	}

	/** Binds by delegate */
	FDelegateHandle Bind(DynamicSinglecastDelegateType& InDelegate)
	{
		if (FDelegateHandle* const Handle = DelegateHandleMap.Find(InDelegate))
		{
			LOG_WARN(UnifiedDelegateLog, TEXT("Adding function %s::%s to a unified delegate more than once. Probably a setup error."), *GetNameSafe(InDelegate.GetUObject()), *InDelegate.GetFunctionName().ToString());
			return *Handle;
		}
		else
		{
			FDelegateHandle AddedHandle = MulticastDelegate.AddUFunction(InDelegate.GetUObject(), InDelegate.GetFunctionName());
			DelegateHandleMap.Emplace(InDelegate, AddedHandle);
			return AddedHandle;
		}
	}

	/** Returns the internal multicast delegate, use this to bind by lambdas */
	MulticastDelegateType& GetDelegate()
	{
		return MulticastDelegate;
	}

	/** Broadcast the given arguments to all bound functions */
	template <typename ... Args>
	void Broadcast(Args&&... args) const
	{
		MulticastDelegate.Broadcast(std::forward<Args>(args)...);
	}

	/** Unbind a delegate by handle */
	void Remove(const FDelegateHandle& Handle)
	{
		MulticastDelegate.Remove(Handle);

		if (const DynamicSinglecastDelegateType* const Delegate = DelegateHandleMap.FindKey(Handle))
		{
			DelegateHandleMap.Remove(*Delegate);
		}
	}

	/** Unbind a delegate */
	void Remove(const DynamicSinglecastDelegateType& InDelegate)
	{
		if (const FDelegateHandle* const Handle = DelegateHandleMap.Find(InDelegate))
		{
			MulticastDelegate.Remove(*Handle);
			DelegateHandleMap.Remove(InDelegate);
		}
	}

	/** Returns whether one or more callbacks are bound to this delegate */
	bool IsBound() const
	{
		return MulticastDelegate.IsBound();
	}

private:
	MulticastDelegateType MulticastDelegate;
	TMap<DynamicSinglecastDelegateType, FDelegateHandle> DelegateHandleMap;
};
