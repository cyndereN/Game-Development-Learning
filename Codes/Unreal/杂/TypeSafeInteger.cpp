/// Provides type safety and validity checking for integral IDs/handles. Child classes store the underlying
/// value, meaning that those values can be reflected properties and gain the associated benefits.
template<typename TChild, typename TNumberType>
struct TTypeSafeNumberInterface
{
	constexpr TTypeSafeNumberInterface()
	{
		//Make sure the underlying value is initialised. This will be overwritten
		//by any initialisation in the child class. Note that this overwriting is
		//why child classes must provide explicit constructors from TIntegerType.
		AsChild().GetValueRef() = { };
	}

	/// Explicit conversions to underlying type
	auto GetValue() const { return AsChild().GetValueRef(); }
	explicit operator TNumberType() const { return GetValue(); }
	TNumberType operator*() const { return GetValue(); }

	/// Explicit validation against Invalid
	bool IsValid() const { return GetValue() != GetInvalid(); }
	explicit operator bool() const { return IsValid(); }

	/// Reset to the Invalid value
	void Reset() { AsChild() = TChild{ }; }

	bool operator==(const TTypeSafeNumberInterface& Other) const
	{
		return GetValue() == Other.GetValue();
	}
	bool operator!=(const TTypeSafeNumberInterface& Other) const { return !(*this == Other); }

private:

	TChild& AsChild() { return *static_cast<TChild*>(this); }
	const TChild& AsChild() const { return *static_cast<const TChild*>(this); }

	/// Treat the default constructed value as invalid
	static TNumberType GetInvalid() { return TChild{ }.GetValue(); }
};


/// Usable as a TMap/TSet key/element
template<typename TChild, typename TIntegerType>
uint32 GetTypeHash(const TTypeSafeNumberInterface<TChild, TIntegerType>& Value)
{
	return GetTypeHash(Value.GetValue());
}


/// Implements generic inline storage for TTypeSafeNumberInterface, and functions as an example of how to do so.
/// Salt should be a unique type and is used to force different template instantiations for different ID types.
template<typename Salt, typename TNumberType, TNumberType Invalid = 0>
struct TTypeSafeNumber : TTypeSafeNumberInterface<TTypeSafeNumber<Salt, TNumberType, Invalid>, TNumberType>
{
	// Friending the base class allows GetValueRef to be private
	using Super = TTypeSafeNumberInterface<TTypeSafeNumber, TNumberType>;
	friend Super;

	TTypeSafeNumber() = default;

	/// Construction from int must be explicit (this is the main type safety mechanism)
	explicit TTypeSafeNumber(const TNumberType InNumber) : Number(InNumber) { }

	// Import super's operator/s
	using Super::operator*;

	/// Explicit conversions to int (legacy interface implemented here to reduce int version to a typedef)
	TNumberType Int() const { return Super::GetValue(); }

private:

	/// Required functions for TTypeSafeNumberInterface
	TNumberType& GetValueRef() { return Number; }
	const TNumberType& GetValueRef() const { return Number; }

	TNumberType Number = Invalid;
};


/// General case usage with int type. Note that in newer C++ versions we should
/// be able to infer TTypeSafeNumber::TNumberType and dispense with this.
template<typename TSalt, int Invalid = 0>
using TTypeSafeInteger = TTypeSafeNumber<TSalt, int, Invalid>;


namespace TypeSafeNumberExamples
{
	/// Declare a unique type safe int using a type salt
	struct FTypeSafeIntegerTest_Salt { };
	using FTypeSafeIntegerTest = TTypeSafeInteger<FTypeSafeIntegerTest_Salt, -1>;

	// Declare an instance of this struct somewhere to perform test
	struct FTestTypeSafeNumber
	{
		FTestTypeSafeNumber()
		{
			const FTypeSafeIntegerTest InvalidNumber;
			const FTypeSafeIntegerTest ValidNumber{ 0 };
			ASSERT(!InvalidNumber, TEXT("Invalid number should be invalid."));
			ASSERT(ValidNumber, TEXT("Valid number should be valid."));
			ASSERT(InvalidNumber != ValidNumber, TEXT("Distinct numbers should not be considered equal."));

			TMap<FTypeSafeIntegerTest, void*> NumberKeyedMap;
			NumberKeyedMap.Add(InvalidNumber, nullptr);
			NumberKeyedMap.Add(ValidNumber, nullptr);
			//Emplace works here but Add wouldn't, due to the explicit constructor required
			NumberKeyedMap.Emplace(2, nullptr);
			ASSERT(NumberKeyedMap.Num() == 3, TEXT("Unexpected map size - is GetTypeHash functioning correctly?"));
		}
	};
}

// another usage example

struct FTargetID;
using FTargetIDBase = TTypeSafeNumberInterface<FTargetID, int64>;

// Target ID is a unique ID for a particular target
USTRUCT(BlueprintType)
struct TARGETINTERESTS_API FTargetID : public FTargetIDBase
{
	GENERATED_BODY()
	friend FTargetIDBase;
public:

	FTargetID() {}
	explicit FTargetID(const int64 InValue) : Value(InValue) {}
	using FTargetIDBase::operator*;

private:

	int64& GetValueRef() { return Value; }
	const int64& GetValueRef() const { return Value; }

	// -1 is the invalid value as default
	UPROPERTY(Transient)
	int64 Value = -1;
};