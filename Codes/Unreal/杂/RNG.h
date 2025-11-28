#pragma once

PUSH_RELAXED_COMPILER_WARNING_LEVEL
#include <random>
POP_RELAXED_COMPILER_WARNING_LEVEL

#include "RandomNumberGeneration.generated.h"

namespace RNG
{
	using SeedType = uint64;

	/** Returns a non-deterministic, uniformly-distributed random value */
	SeedType RandomSeed();
}

UENUM()
enum class ERandomNumberGeneratorDistribution : uint8
{
	/* Generates an absolute value */
	AbsoluteValue,

	/** 
	 * Generates random values uniformly distributed in the range [a, b)
	 * @see https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)
	 */
	Uniform,

	/**
	 * Generates random values according to a normal (or Gaussian) distribution
	 * @see https://en.wikipedia.org/wiki/Normal_distribution
	 */
	Normal,

	/**
	 * Generates random values according to a Cauchy distribution
	 * @see https://en.wikipedia.org/wiki/Cauchy_distribution
	 */
	Cauchy,

	Count UMETA(Hidden)
};

namespace RNG
{
	/**
	 *	Tag for use in FRandomNumberGenerator_X constructors
	 */
	template<ERandomNumberGeneratorDistribution InDistribution>
	struct FRandomNumberGeneratorDistribution
	{
		static constexpr ERandomNumberGeneratorDistribution Distribution = InDistribution;
	};

	/// Tag definitions to keep call sites clean
	using FAbsoluteValue = FRandomNumberGeneratorDistribution<ERandomNumberGeneratorDistribution::AbsoluteValue>;
	using FUniform = FRandomNumberGeneratorDistribution<ERandomNumberGeneratorDistribution::Uniform>;
	using FNormal = FRandomNumberGeneratorDistribution<ERandomNumberGeneratorDistribution::Normal>;
	using FCauchy = FRandomNumberGeneratorDistribution<ERandomNumberGeneratorDistribution::Cauchy>;

	/**
	 *  An implementation of a WyHash random number generator that implements (part of) the RandomNumberEngine named requirement set
	 *  Reference implementation of the hash itself: https://github.com/wangyi-fudan/wyhash
	 */
	struct FWyHashGenerator
	{
	public:

		using result_type = uint64;

		explicit FWyHashGenerator(uint64 InState = 0ull)
			: State(InState)
		{}

		static constexpr uint64 min() { return std::numeric_limits<uint64>::min(); }
		static constexpr uint64 max() { return std::numeric_limits<uint64>::max(); }

		void seed(const uint64 NewState = 0ull) { State = NewState; }

		uint64 operator()();
		void discard(unsigned long long NumValuesToDiscard);

		bool operator==(const FWyHashGenerator& Other) { return State == Other.State; }
		bool operator!=(const FWyHashGenerator& Other) { return State != Other.State; }

	private:
		uint64 State = 0ull;
	};
} // namespace RNG

/**
 * Generates a random number using the selected distribution
 *
 * @see ERandomNumberGeneratorDistribution
 */
USTRUCT(BlueprintInternalUseOnly)
struct FRandomNumberGenerator_Float
{
	GENERATED_BODY()

	using ResultType = float;
	using SeedType = RNG::SeedType;
	
	/** State which can be stored inside or outside of this class, depending on constness */
	struct FState
	{
		using GeneratorType = RNG::FWyHashGenerator;
		GeneratorType Generator;
	};

	/** The distribution with which numbers should be generated */
	UPROPERTY(EditAnywhere)
	ERandomNumberGeneratorDistribution Distribution = ERandomNumberGeneratorDistribution::AbsoluteValue;

	/** The mean or expected value of the distribution */
	UPROPERTY(EditAnywhere)
	float ExpectedValue = 0.0f;

	/** The standard deviation of the distribution */
	UPROPERTY(EditAnywhere)
	float StandardDeviation = 1.0f;

	/** The minimum value of the distribution */
	UPROPERTY(EditAnywhere)
	float Min = 0.0f;

	/** The maximum value of the distribution */
	UPROPERTY(EditAnywhere)
	float Max = 1.0f;

	/** The scale of the Cauchy Distribution, lower = more concentrated */
	UPROPERTY(EditAnywhere)
	float Lambda = 1.f;

	/** The x-offset of the Cauchy Distribution */
	UPROPERTY(EditAnywhere)
	float Offset = 0.f;

	explicit FRandomNumberGenerator_Float(SeedType Seed = RNG::RandomSeed());

	FRandomNumberGenerator_Float(float InExpectedValue);
	FRandomNumberGenerator_Float(RNG::FUniform InDistribution, float InMin, float InMax);

	/** Returns a number generated using the selected distribution */
	float operator()();
	float operator()(FState& InState) const;

	/** Returns a number generated using the selected distribution and static state */
	float operator()() const;

	/** Returns a number generated using the selected distribution */
	float GetValue() { return this->operator()(); }

	/** Re-initializes the generator with the specified seed */
	void Reset(SeedType Seed);

	/** Returns a copy of the generator's internal state */
	FState CloneState() const { return State; }

	/** Returns a reference to the generator's internal state */
	FState& GetState() { return State; }
	const FState& GetState() const { return State; }

private:
	/** Runtime state for when generation is performed in a non-const manner */
	FState State;
};

namespace Lex
{
	FString ToString(const FRandomNumberGenerator_Float& Generator);
}

//-----------------------------------------
#include "RandomNumberGeneration.h"

PUSH_RELAXED_COMPILER_WARNING_LEVEL
#include <Math/BigInt.h>
POP_RELAXED_COMPILER_WARNING_LEVEL


namespace RNG
{
	SeedType RandomSeed()
	{
		std::random_device RandomDevice;
		const uint32 Low = RandomDevice();
		const uint32 High = RandomDevice();
		return uint64(High) << 32 | uint64(Low);
	}

	using uint128 = TBigInt<128, false>;

	uint128 Mult64x64to128(const uint64 Left, const uint64 Right)
	{
		static_assert(sizeof(uint32) * 2 == sizeof(uint64), ":(");

		uint32 LeftBits[4];
		FMemory::Memset(LeftBits, uint8(0u), sizeof(LeftBits));
		FMemory::Memcpy(LeftBits, &Left, sizeof(uint64));

		uint32 RightBits[4];
		FMemory::Memset(RightBits, uint8(0u), sizeof(RightBits));
		FMemory::Memcpy(RightBits, &Left, sizeof(uint64));

		return uint128{ LeftBits } * uint128{ RightBits };
	}

	uint64 GetHighValue(uint128& Value)
	{
		return uint64(Value.GetBits()[3]) << 32 | uint64(Value.GetBits()[2]);
	}
	uint64 GetLowValue(uint128& Value)
	{
		return uint64(Value.GetBits()[1]) << 32 | uint64(Value.GetBits()[0]);
	}

	FWyHashGenerator::result_type FWyHashGenerator::operator()()
	{
		State += 0x60bee2bee120fc15ull;
		const uint64 Step1 = State ^ 0x1b03738712fad5c9ull;
		uint128 Step2 = Mult64x64to128(Step1, State);
		return GetHighValue(Step2) ^ GetLowValue(Step2);
	}
	void FWyHashGenerator::discard(unsigned long long NumValuesToDiscard)
	{
		while (NumValuesToDiscard > 0ull)
		{
			State += 0x60bee2bee120fc15ull;
			--NumValuesToDiscard;
		}
	}
}

FRandomNumberGenerator_Float::FRandomNumberGenerator_Float(const SeedType Seed)
	: State{ FState::GeneratorType { static_cast<decltype(FState::Generator)::result_type>(Seed) } }
{
}

FRandomNumberGenerator_Float::FRandomNumberGenerator_Float(const float InExpectedValue)
	: FRandomNumberGenerator_Float{}
{
	ExpectedValue = InExpectedValue;
}

FRandomNumberGenerator_Float::FRandomNumberGenerator_Float(RNG::FUniform InDistribution, const float InMin, const float InMax)
	: FRandomNumberGenerator_Float{}
{
	Distribution = InDistribution.Distribution;
	Min = InMin;
	Max = InMax;
}

float FRandomNumberGenerator_Float::operator()() const
{ 
	static thread_local FState StaticState{ FState::GeneratorType { static_cast<decltype(FState::Generator)::result_type>(FMath::Rand()) } };
	return (*this)(StaticState);
}

float FRandomNumberGenerator_Float::operator()()
{
	return (*this)(State);
}

float FRandomNumberGenerator_Float::operator()(FState& InState) const
{
	using namespace std;

	switch (Distribution)
	{
		case ERandomNumberGeneratorDistribution::AbsoluteValue:
		{
			return ExpectedValue;
		}

		case ERandomNumberGeneratorDistribution::Uniform:
		{
			return uniform_real_distribution<float>{ Min, Max }(InState.Generator);
		}

		case ERandomNumberGeneratorDistribution::Normal:
		{
			// Consider caching the distribution as a member of this class 
			// "The normal_distribution in particular generates two random numbers at once and "
			// "caches the second for use in a subsequent call, so not creating a new one each "
			// "time would be preferable."

			return normal_distribution<float>{ ExpectedValue, StandardDeviation }(InState.Generator);
		}
		case ERandomNumberGeneratorDistribution::Cauchy:
		{
			return cauchy_distribution<float>{ Offset, Lambda }(InState.Generator);
		}
	}

	ASSERT_SHOULD_NEVER_EXECUTE(TEXT("FRandomNumberGenerator_Float::operator() has been called with an invalid distribution"));
	return 0.0f;
}

void FRandomNumberGenerator_Float::Reset(const SeedType Seed)
{
	State.Generator.seed(Seed);
}

namespace Lex
{
	FString ToString(const FRandomNumberGenerator_Float& Generator)
	{
		switch (Generator.Distribution)
		{
			case ERandomNumberGeneratorDistribution::AbsoluteValue:
			{
				return FString::Printf(TEXT("Value: %.2f"), 
					Generator.ExpectedValue);
			}

			case ERandomNumberGeneratorDistribution::Uniform:
			{
				return FString::Printf(TEXT("Min: %.2f, Max: %.2f"), 
					Generator.Min, Generator.Max);
			}

			case ERandomNumberGeneratorDistribution::Normal:
			{
				return FString::Printf(TEXT("Mean: %.2f, Std. Deviation: %.2f"), 
					Generator.ExpectedValue, Generator.StandardDeviation);
			}

			case ERandomNumberGeneratorDistribution::Cauchy:
			{
				return FString::Printf(TEXT("Lambda: %.2f, Offset: %.2f"),
					Generator.Lambda, Generator.Offset);
			}
		}

		ASSERT_SHOULD_NEVER_EXECUTE(
			TEXT("Lex::ToString(const FRandomNumberGenerator_Float&) has been called with an invalid distribution"));

		return {};
	}
}
